using SkiaSharp;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Waher.Content;
using Waher.Content.Html.Elements;
using Waher.Content.Markdown;
using Waher.Content.QR;
using Waher.Content.QR.Encoding;
using Waher.Content.Xml;
using Waher.Events;
using Waher.Events.Console;
using Waher.Networking.XMPP;
using Waher.Networking.XMPP.BitsOfBinary;
using Waher.Networking.XMPP.Chat;
using Waher.Networking.XMPP.Control;
using Waher.Networking.XMPP.Provisioning;
using Waher.Networking.XMPP.Sensor;
using Waher.Networking.XMPP.ServiceDiscovery;
using Waher.Persistence;
using Waher.Persistence.Files;
using Waher.Persistence.Serialization;
using Waher.Runtime.Inventory;
using Waher.Runtime.Settings;
using Waher.Script;
using Waher.Script.Graphs;
using Waher.Security;

internal class Program
{
	private static FilesProvider? db = null;
	private static XmppClient? xmppClient = null;

	private static readonly QrEncoder? qrEncoder = new();
	private static string? deviceId;
	private static string? thingRegistryJid = string.Empty;
	private static string? provisioningJid = string.Empty;
	private static string? ownerJid = string.Empty;
	private static SensorServer? sensorServer = null;
	private static ControlServer? controlServer = null;
	private static ThingRegistryClient? registryClient = null;
	private static ProvisioningClient? provisioningClient = null;
	private static BobClient? bobClient = null;
	private static ChatServer? chatServer = null;

	private static async Task Main(string[] args)
	{
		try
		{
			Console.ForegroundColor = ConsoleColor.White;

			#region Initializing system

			Log.Register(new ConsoleEventSink(false, false));
			Log.Informational("Starting application.");

			Types.Initialize(
				typeof(Database).GetTypeInfo().Assembly,
				typeof(FilesProvider).GetTypeInfo().Assembly,
				typeof(ObjectSerializer).GetTypeInfo().Assembly,    // Waher.Persistence.Serialization was broken out of Waher.Persistence.FilesLW after the publishing of the MIoT book.
				typeof(RuntimeSettings).GetTypeInfo().Assembly,
				typeof(IContentEncoder).GetTypeInfo().Assembly,
				typeof(XmppClient).GetTypeInfo().Assembly,
				typeof(MarkdownDocument).GetTypeInfo().Assembly,
				typeof(XML).GetTypeInfo().Assembly,
				typeof(Expression).GetTypeInfo().Assembly,
				typeof(Graph).GetTypeInfo().Assembly,
				typeof(Select).GetTypeInfo().Assembly,
				typeof(Program).GetTypeInfo().Assembly);

			#endregion

			#region Setting up database

			db = await FilesProvider.CreateAsync(Path.Combine(Environment.CurrentDirectory, "Data"),
				"Default", 8192, 1000, 8192, Encoding.UTF8, 10000);
			await db.RepairIfInproperShutdown(null);

			Database.Register(db);

			await Types.StartAllModules(60000);

			#region Device ID

			deviceId = await RuntimeSettings.GetAsync("DeviceId", string.Empty);
			if (string.IsNullOrEmpty(deviceId))
			{
				deviceId = Guid.NewGuid().ToString().Replace("-", string.Empty);
				await RuntimeSettings.SetAsync("DeviceId", deviceId);
			}

			Log.Informational("Device ID: " + deviceId);

			#endregion

			#endregion

			#region XMPP Connection

			string Host = await EnvironmentSettings.GetAsync("XMPP_HOST", "XmppHost", "lab.tagroot.io");
			int Port = (int)await EnvironmentSettings.GetAsync("XMPP_PORT", "XmppPort", 5222);
			string UserName = await EnvironmentSettings.GetAsync("XMPP_USERNAME", "XmppUserName", string.Empty);
			string PasswordHash = await EnvironmentSettings.GetAsync("XMPP_PASSWORD", "XmppPasswordHash", string.Empty);
			string PasswordHashMethod = await EnvironmentSettings.GetAsync("XMPP_PASSWORDHASHMETHOD", "XmppPasswordHashMethod", string.Empty);
			string ApiKey = await EnvironmentSettings.GetAsync("XMPP_APIKEY", "XmppApiKey", string.Empty);
			string ApiSecret = await EnvironmentSettings.GetAsync("XMPP_APISECRET", "XmppApiSecret", string.Empty);
			bool Updated = false;

			while (true)
			{
				if (!string.IsNullOrEmpty(Host) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(PasswordHash))
				{
					try
					{
						SafeDispose(ref xmppClient);

						if (string.IsNullOrEmpty(PasswordHashMethod))
							xmppClient = new XmppClient(Host, Port, UserName, PasswordHash, "en", typeof(Program).GetTypeInfo().Assembly);
						else
							xmppClient = new XmppClient(Host, Port, UserName, PasswordHash, PasswordHashMethod, "en", typeof(Program).GetTypeInfo().Assembly);

						xmppClient.AllowCramMD5 = false;
						xmppClient.AllowDigestMD5 = false;
						xmppClient.AllowPlain = false;
						xmppClient.AllowScramSHA1 = true;
						xmppClient.AllowScramSHA256 = true;

						if (string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(ApiSecret))
							xmppClient.AllowRegistration();
						else
							xmppClient.AllowRegistration(ApiKey, ApiSecret);

						xmppClient.OnStateChanged += (sender, State) =>
						{
							Log.Informational("Changing state: " + State.ToString());

							switch (State)
							{
								case XmppState.Connected:
									Log.Informational("Connected as " + xmppClient.FullJID);
									break;
							}

							return Task.CompletedTask;
						};

						xmppClient.OnConnectionError += (sender, ex) =>
						{
							Log.Error(ex.Message);
							return Task.CompletedTask;
						};

						Log.Informational("Connecting to " + xmppClient.Host + ":" + xmppClient.Port.ToString());
						xmppClient.Connect();

						switch (await xmppClient.WaitStateAsync(10000, XmppState.Connected, XmppState.Error, XmppState.Offline))
						{
							case 0: // Connected
								break;

							case 1: // Error
								throw new Exception("An error occurred when trying to connect. Please revise parameters and try again.");

							case 2: // Offline
							default:
								throw new Exception("Unable to connect to host. Please revise parameters and try again.");
						}

						if (Updated)
						{
							await RuntimeSettings.SetAsync("XmppHost", Host = xmppClient.Host);
							await RuntimeSettings.SetAsync("XmppPort", Port = xmppClient.Port);
							await RuntimeSettings.SetAsync("XmppUserName", UserName = xmppClient.UserName);
							await RuntimeSettings.SetAsync("XmppPasswordHash", PasswordHash = xmppClient.PasswordHash);
							await RuntimeSettings.SetAsync("XmppPasswordHashMethod", PasswordHashMethod = xmppClient.PasswordHashMethod);

							// Note: Do not store API Key and API Secret
						}

						break;
					}
					catch (Exception ex)
					{
						Log.Error("Unable to connect to XMPP Network. The following error was reported:\r\n\r\n" + ex.Message);
					}
				}

				Host = InputString("XMPP Broker", Host);
				Port = InputString("Port", Port);
				UserName = InputString("User Name", UserName);
				PasswordHash = InputString("Password", "Leave blank to randomize password.", PasswordHash);
				if (string.IsNullOrEmpty(PasswordHash))
				{
					using RandomNumberGenerator Rnd = RandomNumberGenerator.Create();
					byte[] Bin = new byte[32];  // 256 random bits

					Rnd.GetBytes(Bin);
					PasswordHash = Hashes.BinaryToString(Bin);
				}

				PasswordHashMethod = string.Empty;
				ApiKey = InputString("API Key", "Provide API Key to create account.", ApiKey);
				ApiSecret = InputString("API Secret", "Provide API Secret to create account.", ApiSecret);
				Updated = true;
			}

			#endregion

			#region Basic XMPP events & features

			xmppClient.OnError += (Sender, ex) =>
			{
				Log.Error(ex);
				return Task.CompletedTask;
			};

			xmppClient.OnPasswordChanged += (Sender, e) => Log.Informational("Password changed.", xmppClient.BareJID);

			xmppClient.OnPresenceSubscribed += (Sender, e) =>
			{
				Log.Informational("Friendship request accepted.", xmppClient.BareJID, e.From);
				return Task.CompletedTask;
			};

			xmppClient.OnPresenceUnsubscribed += (Sender, e) =>
			{
				Log.Informational("Friendship removal accepted.", xmppClient.BareJID, e.From);
				return Task.CompletedTask;
			};

			RegisterVCard();

			#endregion

			#region Configuring Decision Support & Provisioning

			thingRegistryJid = await RuntimeSettings.GetAsync("ThingRegistry.JID", string.Empty);
			provisioningJid = await RuntimeSettings.GetAsync("ProvisioningServer.JID", thingRegistryJid);
			ownerJid = await RuntimeSettings.GetAsync("ThingRegistry.Owner", string.Empty);

			if (string.IsNullOrEmpty(thingRegistryJid) || string.IsNullOrEmpty(provisioningJid))
			{
				Log.Informational("Searching for Thing Registry and Provisioning Server.");

				ServiceItemsDiscoveryEventArgs e = await xmppClient.ServiceItemsDiscoveryAsync(xmppClient.Domain);
				foreach (Item Item in e.Items)
				{
					ServiceDiscoveryEventArgs e2 = await xmppClient.ServiceDiscoveryAsync(Item.JID);

					try
					{
						if (e2.HasAnyFeature(ProvisioningClient.NamespacesProvisioningDevice))
						{
							await RuntimeSettings.SetAsync("ProvisioningServer.JID", provisioningJid = Item.JID);
							Log.Informational("Provisioning server found.", provisioningJid);
						}

						if (e2.HasAnyFeature(ThingRegistryClient.NamespacesDiscovery))
						{
							await RuntimeSettings.SetAsync("ThingRegistry.JID", thingRegistryJid = Item.JID);
							Log.Informational("Thing registry found.", thingRegistryJid);
						}
					}
					catch (Exception ex)
					{
						Log.Exception(ex);
					}
				}
			}

			if (!string.IsNullOrEmpty(provisioningJid))
			{
				provisioningClient = new ProvisioningClient(xmppClient, provisioningJid, ownerJid);

				provisioningClient.CacheCleared += (sender, e) =>
				{
					Log.Informational("Rule cache cleared.");
				};
			}

			if (!string.IsNullOrEmpty(thingRegistryJid))
			{
				registryClient = new ThingRegistryClient(xmppClient, thingRegistryJid);

				registryClient.Claimed += async (sender, e) =>
				{
					try
					{
						Log.Notice("Owner claimed device.", string.Empty, e.JID);

						await RuntimeSettings.SetAsync("ThingRegistry.Owner", ownerJid = e.JID);
						await RuntimeSettings.SetAsync("ThingRegistry.Key", string.Empty);

						Reregister();
					}
					catch (Exception ex)
					{
						Log.Exception(ex);
					}
				};

				registryClient.Disowned += async (sender, e) =>
				{
					try
					{
						Log.Notice("Owner disowned device.", string.Empty, ownerJid);

						await RuntimeSettings.SetAsync("ThingRegistry.Owner", ownerJid = string.Empty);

						Reregister();
					}
					catch (Exception ex)
					{
						Log.Exception(ex);
					}
				};
			}

			await RegisterDevice();

			#endregion

			//SetupSensorServer();
			//SetupControlServer();
			//SetupChatServer();

			bool Running = true;

			Console.CancelKeyPress += (_, e) =>
			{
				Running = false;
				e.Cancel = true;
			};

			while (Running)
				await Task.Delay(1000);
		}
		catch (Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Out.WriteLine(ex.Message);
		}
		finally
		{
			//SafeDispose(ref pepClient);
			SafeDispose(ref chatServer);
			SafeDispose(ref bobClient);
			SafeDispose(ref sensorServer);
			SafeDispose(ref xmppClient);

			Log.Terminate();

			await Types.StopAllModules();
		}
	}

	private static void SafeDispose<T>(ref T? Object)
		where T : IDisposable
	{
		try
		{
			Object?.Dispose();
			Object = default;
		}
		catch (Exception ex)
		{
			Log.Exception(ex);
		}
	}

	#region Console Input

	private static int InputString(string Prompt, int DefaultValue)
	{
		return InputString(Prompt, string.Empty, DefaultValue);
	}

	private static int InputString(string Prompt, string Comment, int DefaultValue)
	{
		while (true)
		{
			string s = InputString(Prompt, Comment, DefaultValue.ToString());
			if (int.TryParse(s, out int Result))
				return Result;

			Log.Error("Input must be an integer.");
		}
	}

	private static string InputString(string Prompt, string DefaultValue)
	{
		return InputString(Prompt, string.Empty, DefaultValue);
	}

	private static string InputString(string Prompt, string Comment, string DefaultValue)
	{
		Console.ForegroundColor = ConsoleColor.White;
		Console.Out.Write(Prompt);
		Console.Out.WriteLine(":");

		if (!string.IsNullOrEmpty(DefaultValue))
		{
			Console.Write("(Accept default value by pressing ENTER: ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write(DefaultValue);
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(")");
		}
		else if (!string.IsNullOrEmpty(Comment))
		{
			Console.Write("(");
			Console.Write(Comment);
			Console.WriteLine(")");
		}

		Console.Write("> ");
		Console.ForegroundColor = ConsoleColor.Yellow;

		string? s = Console.In.ReadLine();
		if (string.IsNullOrEmpty(s))
			s = DefaultValue;

		Console.ForegroundColor = ConsoleColor.White;

		return s;
	}

	#endregion

	#region vCard contact information for device

	// XEP-0054 - vcard-temp: http://xmpp.org/extensions/xep-0054.html

	private static void RegisterVCard()
	{
		xmppClient?.RegisterIqGetHandler("vCard", "vcard-temp", (sender, e) =>
		{
			e.IqResult(GetVCardXml());
			return Task.CompletedTask;
		}, true);

		Log.Informational("Setting vCard");
		xmppClient?.SendIqSet(string.Empty, GetVCardXml(), (sender, e) =>
		{
			if (e.Ok)
				Log.Informational("vCard successfully set.");
			else
				Log.Error("Unable to set vCard.");

			return Task.CompletedTask;

		}, null);
	}

	private static string GetVCardXml()
	{
		StringBuilder Xml = new();

		Xml.Append("<vCard xmlns='vcard-temp'>");
		Xml.Append("<FN>MQTT-XMPP Bridge</FN><N><FAMILY>Bridge</FAMILY><GIVEN>MQTT-XMPP</GIVEN><MIDDLE/></N>");
		Xml.Append("<URL>https://github.com/Neuro-Foundation/IoTBridgeMqtt</URL>");
		Xml.Append("<JABBERID>");
		Xml.Append(XML.Encode(xmppClient?.BareJID));
		Xml.Append("</JABBERID>");
		Xml.Append("<UID>");
		Xml.Append(deviceId);
		Xml.Append("</UID>");
		Xml.Append("<DESC>MQTT-XMPP Bridge Project (IoTBridgeMqtt), based on a concentrator example from the book Mastering Internet of Things, by Peter Waher.</DESC>");

		// XEP-0153 - vCard-Based Avatars: http://xmpp.org/extensions/xep-0153.html

		using Stream? fs = typeof(Program).Assembly.GetManifestResourceStream("ConsoleBridge.Assets.Icon.png");

		if (fs is not null)
		{
			int Len = (int)fs.Length;
			byte[] Icon = new byte[Len];
			fs.Read(Icon, 0, Len);

			Xml.Append("<PHOTO><TYPE>image/png</TYPE><BINVAL>");
			Xml.Append(Convert.ToBase64String(Icon));
			Xml.Append("</BINVAL></PHOTO>");
		}

		Xml.Append("</vCard>");

		return Xml.ToString();
	}

	#endregion

	#region Device Registration

	private static async Task RegisterDevice()
	{
		string Country = await EnvironmentSettings.GetAsync("REGISTRY_COUNTRY", "ThingRegistry.Country", string.Empty);
		string Region = await EnvironmentSettings.GetAsync("REGISTRY_REGION", "ThingRegistry.Region", string.Empty);
		string City = await EnvironmentSettings.GetAsync("REGISTRY_CITY", "ThingRegistry.City", string.Empty);
		string Area = await EnvironmentSettings.GetAsync("REGISTRY_AREA", "ThingRegistry.Area", string.Empty);
		string Street = await EnvironmentSettings.GetAsync("REGISTRY_SRTEET", "ThingRegistry.Street", string.Empty);
		string StreetNr = await EnvironmentSettings.GetAsync("REGISTRY_STREETNR", "ThingRegistry.StreetNr", string.Empty);
		string Building = await EnvironmentSettings.GetAsync("REGISTRY_BUILDING", "ThingRegistry.Building", string.Empty);
		string Apartment = await EnvironmentSettings.GetAsync("REGISTRY_APARTMENT", "ThingRegistry.Apartment", string.Empty);
		string Room = await EnvironmentSettings.GetAsync("REGISTRY_ROOM", "ThingRegistry.Room", string.Empty);
		string Name = await EnvironmentSettings.GetAsync("REGISTRY_NAME", "ThingRegistry.Name", string.Empty);
		bool HasLocation = await EnvironmentSettings.GetAsync("REGISTRY_LOCATION", "ThingRegistry.Location", false);
		bool Updated = false;

		while (true)
		{
			if (HasLocation)
			{
				try
				{
					List<MetaDataTag> MetaInfo =
						[
							new MetaDataStringTag("CLASS", "Bridge"),
							new MetaDataStringTag("TYPE", "MQTT<->XMPP"),
							new MetaDataStringTag("MAN", "neuro-foundation.io"),
							new MetaDataStringTag("MODEL", "IoTBridgeMqtt"),
							new MetaDataStringTag("PURL", "https://github.com/Neuro-Foundation/IoTBridgeMqtt"),
							new MetaDataStringTag("SN", deviceId),
							new MetaDataNumericTag("V", 1.0)
						];

					if (!string.IsNullOrEmpty(Country))
						MetaInfo.Add(new MetaDataStringTag("COUNTRY", Country));

					if (!string.IsNullOrEmpty(Region))
						MetaInfo.Add(new MetaDataStringTag("REGION", Region));

					if (!string.IsNullOrEmpty(City))
						MetaInfo.Add(new MetaDataStringTag("CITY", City));

					if (!string.IsNullOrEmpty(Area))
						MetaInfo.Add(new MetaDataStringTag("AREA", Area));

					if (!string.IsNullOrEmpty(Street))
						MetaInfo.Add(new MetaDataStringTag("STREET", Street));

					if (!string.IsNullOrEmpty(StreetNr))
						MetaInfo.Add(new MetaDataStringTag("STREETNR", StreetNr));

					if (!string.IsNullOrEmpty(Building))
						MetaInfo.Add(new MetaDataStringTag("BLD", Building));

					if (!string.IsNullOrEmpty(Apartment))
						MetaInfo.Add(new MetaDataStringTag("APT", Apartment));

					if (!string.IsNullOrEmpty(Room))
						MetaInfo.Add(new MetaDataStringTag("ROOM", Room));

					if (!string.IsNullOrEmpty(Name))
						MetaInfo.Add(new MetaDataStringTag("NAME", Name));

					if (string.IsNullOrEmpty(ownerJid))
						await RegisterDevice([.. MetaInfo]);
					else
						await UpdateRegistration([.. MetaInfo], ownerJid);

					if (Updated)
					{
						await RuntimeSettings.SetAsync("ThingRegistry.Country", Country);
						await RuntimeSettings.SetAsync("ThingRegistry.Region", Region);
						await RuntimeSettings.SetAsync("ThingRegistry.City", City);
						await RuntimeSettings.SetAsync("ThingRegistry.Area", Area);
						await RuntimeSettings.SetAsync("ThingRegistry.Street", Street);
						await RuntimeSettings.SetAsync("ThingRegistry.StreetNr", StreetNr);
						await RuntimeSettings.SetAsync("ThingRegistry.Building", Building);
						await RuntimeSettings.SetAsync("ThingRegistry.Apartment", Apartment);
						await RuntimeSettings.SetAsync("ThingRegistry.Room", Room);
						await RuntimeSettings.SetAsync("ThingRegistry.Name", Name);
					}
					break;
				}
				catch (Exception ex)
				{
					Log.Exception(ex);
				}
			}

			Country = InputString("Country", Country);
			Region = InputString("Region", Region);
			City = InputString("City", City);
			Area = InputString("Area", Area);
			Street = InputString("Street", Street);
			StreetNr = InputString("StreetNr", StreetNr);
			Building = InputString("Building", Building);
			Apartment = InputString("Apartment", Apartment);
			Room = InputString("Room", Room);
			Name = InputString("Name", Name);
			Updated = true;
			HasLocation = true;
		}
	}

	private static async Task RegisterDevice(MetaDataTag[] MetaInfo)
	{
		Log.Informational("Registering device.");

		string Key = await RuntimeSettings.GetAsync("ThingRegistry.Key", string.Empty);
		if (string.IsNullOrEmpty(Key))
		{
			using RandomNumberGenerator Rnd = RandomNumberGenerator.Create();
			byte[] Bin = new byte[32];

			Rnd.GetBytes(Bin);

			Key = Hashes.BinaryToString(Bin);
			await RuntimeSettings.SetAsync("ThingRegistry.Key", Key);
		}

		int c = MetaInfo.Length;
		MetaDataTag[] MetaInfo2 = new MetaDataTag[c + 1];
		Array.Copy(MetaInfo, 0, MetaInfo2, 0, c);
		MetaInfo2[c] = new MetaDataStringTag("KEY", Key);

		registryClient?.RegisterThing(false, MetaInfo2, async (sender, e) =>
		{
			try
			{
				if (e.Ok)
				{
					await RuntimeSettings.SetAsync("ThingRegistry.Location", true);
					await RuntimeSettings.SetAsync("ThingRegistry.Owner", ownerJid = e.OwnerJid);

					if (string.IsNullOrEmpty(e.OwnerJid))
						Log.Informational("Registration successful.");
					else
					{
						await RuntimeSettings.SetAsync("ThingRegistry.Key", string.Empty);
						Log.Informational("Registration updated. Device has an owner.",
							new KeyValuePair<string, object>("Owner", e.OwnerJid));

						MetaInfo2[c] = new MetaDataStringTag("JID", xmppClient?.BareJID);
					}

					if (xmppClient is not null)
						await GenerateIoTDiscoUri(MetaInfo2, xmppClient.Host);
				}
				else
				{
					Log.Error("Registration failed.");
					await RegisterDevice();
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex);
			}
		}, null);
	}

	private static async Task GenerateIoTDiscoUri(MetaDataTag[] MetaInfo, string Host)
	{
		if (registryClient is null || qrEncoder is null)
			return;

		string FilePath = Path.Combine(Environment.CurrentDirectory, "Bridge.iotdisco");
		string DiscoUri = registryClient.EncodeAsIoTDiscoURI(MetaInfo);
		QrMatrix M = qrEncoder.GenerateMatrix(CorrectionLevel.L, DiscoUri);
		string QrCode = M.ToQuarterBlockText();

		Log.Informational(QrCode);
		File.WriteAllText(FilePath, DiscoUri);

		StringBuilder sb = new();

		sb.AppendLine("[InternetShortcut]");
		sb.Append("URL=https://");
		sb.Append(Host);
		sb.Append("/QR/");
		sb.Append(HttpUtility.UrlEncode(DiscoUri));
		sb.AppendLine();

		string ShortcutFileName = FilePath + ".url";
		File.WriteAllText(ShortcutFileName, sb.ToString());

		byte[] Rgba = M.ToRGBA(400, 400);

		using SKData Data = SKData.Create(new MemoryStream(Rgba));
		using SKImage Bitmap = SKImage.FromPixels(new SKImageInfo(400, 400, SKColorType.Rgba8888), Data, 400 * 4);
		using SKData Data2 = Bitmap.Encode(SKEncodedImageFormat.Png, 100);
		byte[] Png = Data2.ToArray();

		string PngFileName = FilePath + ".png";
		await File.WriteAllBytesAsync(PngFileName, Png);

		Log.Informational("IoTDisco URI saved.",
			new KeyValuePair<string, object>("URI Filename", FilePath),
			new KeyValuePair<string, object>("Shortcut Filename", ShortcutFileName),
			new KeyValuePair<string, object>("QR Code Filename", PngFileName));
	}

	private static async Task UpdateRegistration(MetaDataTag[] MetaInfo, string OwnerJid)
	{
		if (string.IsNullOrEmpty(OwnerJid))
			await RegisterDevice(MetaInfo);
		else
		{
			Log.Informational("Updating registration of device.",
				new KeyValuePair<string, object>("Owner", OwnerJid));

			registryClient?.UpdateThing(MetaInfo, async (sender, e) =>
			{
				try
				{
					if (e.Disowned)
					{
						await RuntimeSettings.SetAsync("ThingRegistry.Owner", ownerJid = string.Empty);
						await RegisterDevice(MetaInfo);
					}
					else if (e.Ok)
					{
						Log.Informational("Registration update successful.");

						int c = MetaInfo.Length;
						MetaDataTag[] MetaInfo2 = new MetaDataTag[c + 1];
						Array.Copy(MetaInfo, 0, MetaInfo2, 0, c);
						MetaInfo2[c] = new MetaDataStringTag("JID", xmppClient?.BareJID);

						if (xmppClient is not null)
							await GenerateIoTDiscoUri(MetaInfo2, xmppClient.Host);
					}
					else
					{
						Log.Error("Registration update failed.");
						await RegisterDevice(MetaInfo);
					}
				}
				catch (Exception ex)
				{
					Log.Exception(ex);
				}
			}, null);
		}
	}

	private static void Reregister()
	{
		Task _ = Task.Run(async () =>
		{
			try
			{
				await Task.Delay(5000);
				await RegisterDevice();
			}
			catch (Exception ex)
			{
				Log.Exception(ex);
			}
		});
	}

	#endregion

}