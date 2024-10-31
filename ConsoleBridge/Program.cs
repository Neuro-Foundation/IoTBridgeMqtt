using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Waher.Content;
using Waher.Content.Html.Elements;
using Waher.Content.Markdown;
using Waher.Content.QR;
using Waher.Content.Xml;
using Waher.Events;
using Waher.Events.Console;
using Waher.Networking.XMPP;
using Waher.Networking.XMPP.BitsOfBinary;
using Waher.Networking.XMPP.Chat;
using Waher.Networking.XMPP.Control;
using Waher.Networking.XMPP.Provisioning;
using Waher.Networking.XMPP.Sensor;
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

			while (true)
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

}