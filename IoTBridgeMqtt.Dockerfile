FROM mcr.microsoft.com/dotnet/runtime-deps:8.0

VOLUME ["/var/lib/IoT Gateway"]
WORKDIR "/opt/IoTGateway/"

ARG XMPP_HOST
ENV XMPP_HOST=""
LABEL env.XMPP_HOST="XMPP broker to connect to."

ARG XMPP_PORT
ENV XMPP_PORT="5222"
LABEL env.XMPP_PORT="Optional Port number to use when connecting to host. (If C2S binding has been selected.) If not provided, the default port number will be used."

ARG XMPP_USERNAME
ENV XMPP_USERNAME=""
LABEL env.XMPP_USERNAME="Name of account."

ARG XMPP_PASSWORD
ENV XMPP_PASSWORD=""
LABEL env.XMPP_PASSWORD="Password of account. If creating an account, this variable is optional. If not available, a secure password will be generated."

ARG XMPP_PASSWORDHASHMETHOD
ENV XMPP_PASSWORDHASHMETHOD=""
LABEL env.XMPP_PASSWORDHASHMETHOD="Method used to hash password."

ARG XMPP_APIKEY
ENV XMPP_APIKEY=""
LABEL env.XMPP_APIKEY="API-Key to use when creating account."

ARG XMPP_APISECRET
ENV XMPP_APISECRET=""
LABEL env.XMPP_APISECRET="API-Key secret to use when creating account."

ARG MQTT_HOST
ENV MQTT_HOST=""
LABEL env.MQTT_HOST="MQTT broker to connect to."

ARG MQTT_TLS
ENV MQTT_TLS="true"
LABEL env.MQTT_TLS="If MQTT connection should use TLS."

ARG MQTT_PORT
ENV MQTT_PORT="8883"
LABEL env.MQTT_PORT="Optional Port number to use when connecting to host."

ARG MQTT_USERNAME
ENV MQTT_USERNAME=""
LABEL env.MQTT_USERNAME="Name of account."

ARG MQTT_PASSWORD
ENV MQTT_PASSWORD=""
LABEL env.MQTT_PASSWORD="Password of account. If creating an account, this variable is optional. If not available, a secure password will be generated."

ARG REGISTRY_LOCATION
ENV REGISTRY_LOCATION=""
LABEL env.REGISTRY_LOCATION=""

ARG REGISTRY_COUNTRY
ENV REGISTRY_COUNTRY=""
LABEL env.REGISTRY_COUNTRY="Country to register in Thing Registry."

ARG REGISTRY_REGION
ENV REGISTRY_REGION=""
LABEL env.REGISTRY_REGION="Region to register in Thing Registry."

ARG REGISTRY_CITY
ENV REGISTRY_CITY=""
LABEL env.REGISTRY_CITY="City to register in Thing Registry."

ARG REGISTRY_AREA
ENV REGISTRY_AREA=""
LABEL env.REGISTRY_AREA="Area to register in Thing Registry."

ARG REGISTRY_SRTEET
ENV REGISTRY_SRTEET=""
LABEL env.REGISTRY_SRTEET="Street to register in Thing Registry."

ARG REGISTRY_STREETNR
ENV REGISTRY_STREETNR=""
LABEL env.REGISTRY_STREETNR="StreetNr to register in Thing Registry."

ARG REGISTRY_BUILDING
ENV REGISTRY_BUILDING=""
LABEL env.REGISTRY_BUILDING="Building to register in Thing Registry."

ARG REGISTRY_APARTMENT
ENV REGISTRY_APARTMENT=""
LABEL env.REGISTRY_APARTMENT="Apartment to register in Thing Registry."

ARG REGISTRY_ROOM
ENV REGISTRY_ROOM=""
LABEL env.REGISTRY_ROOM="Room to register in Thing Registry."

ARG REGISTRY_NAME
ENV REGISTRY_NAME=""
LABEL env.REGISTRY_NAME="Name to register in Thing Registry."

COPY [ \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/ConsoleBridge.manifest", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/ConsoleBridge", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/ConsoleBridge.deps.json", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/ConsoleBridge.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/ConsoleBridge.pdb", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/ConsoleBridge.runtimeconfig.json", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/createdump", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/libclrgc.so", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/libclrjit.so", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/libcoreclr.so", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/libcoreclrtraceptprovider.so", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/libhostfxr.so", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/libhostpolicy.so", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/libmscordaccore.so", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/libmscordbi.so", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/libSystem.Globalization.Native.so", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/libSystem.IO.Compression.Native.so", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/libSystem.Native.so", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/libSystem.Net.Security.Native.so", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/libSystem.Security.Cryptography.Native.OpenSsl.so", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Microsoft.CodeAnalysis.CSharp.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Microsoft.CodeAnalysis.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Microsoft.CSharp.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Microsoft.VisualBasic.Core.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Microsoft.VisualBasic.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Microsoft.Win32.Primitives.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Microsoft.Win32.Registry.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/mscorlib.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/netstandard.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/SkiaSharp.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.AppContext.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Buffers.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Collections.Concurrent.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Collections.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Collections.Immutable.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Collections.NonGeneric.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Collections.Specialized.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.ComponentModel.Annotations.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.ComponentModel.DataAnnotations.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.ComponentModel.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.ComponentModel.EventBasedAsync.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.ComponentModel.Primitives.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.ComponentModel.TypeConverter.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Configuration.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Console.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Core.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Data.Common.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Data.DataSetExtensions.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Data.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Diagnostics.Contracts.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Diagnostics.Debug.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Diagnostics.DiagnosticSource.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Diagnostics.FileVersionInfo.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Diagnostics.Process.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Diagnostics.StackTrace.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Diagnostics.TextWriterTraceListener.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Diagnostics.Tools.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Diagnostics.TraceSource.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Diagnostics.Tracing.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Drawing.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Drawing.Primitives.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Dynamic.Runtime.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Formats.Asn1.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Formats.Tar.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Globalization.Calendars.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Globalization.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Globalization.Extensions.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.Compression.Brotli.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.Compression.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.Compression.FileSystem.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.Compression.ZipFile.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.FileSystem.AccessControl.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.FileSystem.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.FileSystem.DriveInfo.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.FileSystem.Primitives.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.FileSystem.Watcher.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.IsolatedStorage.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.MemoryMappedFiles.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.Pipes.AccessControl.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.Pipes.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.IO.UnmanagedMemoryStream.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Linq.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Linq.Expressions.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Linq.Parallel.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Linq.Queryable.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Memory.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.Http.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.Http.Json.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.HttpListener.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.Mail.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.NameResolution.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.NetworkInformation.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.Ping.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.Primitives.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.Quic.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.Requests.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.Security.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.ServicePoint.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.Sockets.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.WebClient.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.WebHeaderCollection.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.WebProxy.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.WebSockets.Client.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Net.WebSockets.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Numerics.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Numerics.Vectors.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.ObjectModel.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Private.CoreLib.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Private.DataContractSerialization.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Private.Uri.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Private.Xml.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Private.Xml.Linq.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Reflection.DispatchProxy.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Reflection.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Reflection.Emit.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Reflection.Emit.ILGeneration.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Reflection.Emit.Lightweight.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Reflection.Extensions.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Reflection.Metadata.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Reflection.Primitives.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Reflection.TypeExtensions.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Resources.Reader.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Resources.ResourceManager.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Resources.Writer.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.CompilerServices.Unsafe.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.CompilerServices.VisualC.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.Extensions.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.Handles.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.InteropServices.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.InteropServices.JavaScript.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.InteropServices.RuntimeInformation.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.Intrinsics.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.Loader.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.Numerics.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.Serialization.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.Serialization.Formatters.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.Serialization.Json.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.Serialization.Primitives.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Runtime.Serialization.Xml.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Security.AccessControl.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Security.Claims.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Security.Cryptography.Algorithms.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Security.Cryptography.Cng.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Security.Cryptography.Csp.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Security.Cryptography.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Security.Cryptography.Encoding.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Security.Cryptography.OpenSsl.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Security.Cryptography.Primitives.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Security.Cryptography.X509Certificates.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Security.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Security.Principal.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Security.Principal.Windows.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Security.SecureString.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.ServiceModel.Web.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.ServiceProcess.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Text.Encoding.CodePages.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Text.Encoding.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Text.Encoding.Extensions.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Text.Encodings.Web.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Text.Json.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Text.RegularExpressions.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Threading.Channels.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Threading.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Threading.Overlapped.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Threading.Tasks.Dataflow.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Threading.Tasks.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Threading.Tasks.Extensions.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Threading.Tasks.Parallel.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Threading.Thread.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Threading.ThreadPool.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Threading.Timer.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Transactions.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Transactions.Local.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.ValueTuple.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Web.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Web.HttpUtility.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Windows.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Xml.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Xml.Linq.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Xml.ReaderWriter.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Xml.Serialization.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Xml.XDocument.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Xml.XmlDocument.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Xml.XmlSerializer.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Xml.XPath.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/System.Xml.XPath.XDocument.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Content.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Content.Emoji.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Content.Html.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Content.Images.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Content.Markdown.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Content.QR.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Content.Xml.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Content.Xsl.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Events.Console.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Events.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Events.Statistics.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Events.XMPP.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.DNS.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.HTTP.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.MQTT.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.PeerToPeer.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.UPnP.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.WHOIS.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.XMPP.Chat.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.XMPP.Concentrator.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.XMPP.Contracts.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.XMPP.Control.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.XMPP.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.XMPP.P2P.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.XMPP.PEP.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.XMPP.Provisioning.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.XMPP.PubSub.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Networking.XMPP.Sensor.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Persistence.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Persistence.Files.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Persistence.Serialization.Compiled.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Runtime.Cache.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Runtime.Collections.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Runtime.Console.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Runtime.Geo.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Runtime.Inventory.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Runtime.IO.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Runtime.Language.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Runtime.Profiling.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Runtime.Queue.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Runtime.Settings.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Runtime.Temporary.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Runtime.Text.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.IoTGateway.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Runtime.Threading.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Runtime.Timing.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Script.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Script.Graphs.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Security.CallStack.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Security.ChaChaPoly.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Security.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Security.EllipticCurves.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Security.JWS.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Security.JWT.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Security.LoginMonitor.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Security.PQC.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Security.SHA3.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Security.Users.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Things.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Things.Ieee1451.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Things.Ip.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Things.Metering.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Things.Mqtt.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Things.Script.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Things.Virtual.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/Waher.Things.Xmpp.dll", \
	"ConsoleBridge/bin/Release/PublishOutputLinux/linux-x64/WindowsBase.dll", \
	"/opt/IoTGateway/"]

RUN ["cp", "-ru", "/var/lib/temp/.", "/var/lib/IoT Gateway"]

RUN ["rm", "-rf", "/var/lib/temp/"]

RUN ["chmod", "+x", "/opt/IoTGateway/ConsoleBridge"]

ENTRYPOINT ["/opt/IoTGateway/ConsoleBridge"]

