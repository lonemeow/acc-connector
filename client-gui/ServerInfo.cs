using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace ACCConnector {
    public class ServerInfo(string? name, string hostname, IPAddress? address, ushort port, bool persistent) {
        private const int MAX_SERVER_NAME_LEN_CHARS = 256;

        public string DisplayName {
            get {
                if (name == null) {
                    return $"{Hostname}:{Port}";
                } else {
                    return $"{name} ({Hostname}:{Port})";
                }
            }
        }

        public string? Name => name;
        public readonly string Hostname = hostname;
        public readonly IPAddress? Address = address;
        public readonly ushort Port = port;
        public readonly bool Persistent = persistent;

        public static ServerInfo FromUri(Uri uri) {
            if (uri.Scheme != Constants.URI_SCHEME) {
                throw new ArgumentException("Invalid URI scheme: " + uri.Scheme);
            }

            var hostname = uri.Host;
            var port = uri.Port;
            var queryParams = HttpUtility.ParseQueryString(uri.Query);
            var name = queryParams["name"];
            var persistent = queryParams["persistent"] == "true";

            IPAddress? address = null;
            try {
                address = Dns.GetHostAddresses(hostname)[0];
            } catch (SocketException e) {
                Logging.Log(Logging.Severity.ERROR, $"Failed to resolve hostname \"{hostname}\": {e.Message}");
            }

            return new ServerInfo(name, hostname, address, (ushort)port, persistent);
        }

        public Uri ToUri() {
            var builder = new UriBuilder {
                Scheme = Constants.URI_SCHEME,
                Host = Hostname,
                Port = Port
            };
            var queryParams = new Dictionary<string, string>();
            if (name != null) {
                queryParams["name"] = name;
            }
            if (Persistent) {
                queryParams["persistent"] = "true";
            }
            builder.Query = string.Join("&", queryParams.Select(kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));
            return builder.Uri;
        }

        public void Write(Stream stream) {
            if (Address == null) {
                return;
            }
            var serverName = DisplayName[..Math.Min(MAX_SERVER_NAME_LEN_CHARS, DisplayName.Length)];
            var serverNameBuffer = new byte[MAX_SERVER_NAME_LEN_CHARS * 4];
            var serverNameLenBytes = Encoding.UTF32.GetBytes(serverName, serverNameBuffer);
            var ip = Address.GetAddressBytes();
            var port = Port;

            stream.Write(serverNameBuffer);
            stream.WriteByte((byte)serverName.Length);
            stream.Write(ip);
            stream.WriteByte((byte)(port >> 8));
            stream.WriteByte((byte)port);
        }
    }
}
