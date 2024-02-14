using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Win32;

namespace ACCConnector {
    public class Constants {
        public const int TAG = 12765;
        public const int COPYDATA_SET_URI = 1;
        public const string URI_SCHEME = "acc-connect";
        public const string NAMED_PIPE_NAME = "acc-connector-pipe";
    }

    public static class ProgramMain {

        private static void SendUriToWindow(IntPtr hWnd, string uri) {
            var utf8Bytes = Encoding.UTF8.GetBytes(uri);
            var handle = GCHandle.Alloc(utf8Bytes, GCHandleType.Pinned);
            var copydata = new User32.COPYDATASTRUCT {
                dwData = Constants.COPYDATA_SET_URI,
                cbData = (uint)utf8Bytes.Length,
                lpData = Marshal.UnsafeAddrOfPinnedArrayElement(utf8Bytes, 0)
            };

            var copydataNative = Marshal.AllocHGlobal(Marshal.SizeOf(copydata));
            Marshal.StructureToPtr(copydata, copydataNative, false);

            User32.SendMessage(hWnd, User32.WM_COPYDATA, IntPtr.Zero, copydataNative);

            Marshal.FreeHGlobal(copydataNative);
            handle.Free();
        }

        private static IntPtr? FindAlreadyOpenWindow() {
            IntPtr? existingWindowHandle = null;
            if (!User32.EnumWindows(delegate (IntPtr hWnd, IntPtr lParam) {
                if (User32.GetWindowLongPtr(hWnd, User32.GWLP_USERDATA) == Constants.TAG) {
                    existingWindowHandle = hWnd;
                }
                return true;
            }, IntPtr.Zero)) {
                throw new APIFailureException("User32.EnumWindows");
            }

            return existingWindowHandle;
        }

        [STAThread]
        static void Main(string[] args) {
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
#endif

            var settings = Settings.Load();
            if (settings == null) {
                var accPath = ACCHook.FindACCInstallDir();
                if (accPath == null) {
                    var msg = """
                        Unable to determine ACC install path
                        Configure it manually via the settings dialog
                        """;
                    MessageBox.Show(msg, "ACC Connector", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    accPath = "";
                }
                settings = new Settings { AccInstallPath = accPath };
            }


            ApplicationConfiguration.Initialize();

            string? serverToAdd = null;
            if (args.Length > 0) {
                serverToAdd = args[0];
            }

            IntPtr? openWindowHandle = FindAlreadyOpenWindow();
            if (openWindowHandle != null && serverToAdd != null) {
                Trace.WriteLine($"Sending URI {serverToAdd} to window {openWindowHandle}");
                SendUriToWindow(openWindowHandle.Value, serverToAdd);
                return;
            }

            var serverList = LoadServerList();
            var serverDataLock = new object();
            var serverData = Array.Empty<byte>();
            serverList.ListChanged += (sender, e) => {
                SaveServerList(serverList);
                lock (serverDataLock) {
                    serverData = BuildServerData(serverList);
                }
            };

            lock (serverDataLock) {
                serverData = BuildServerData(serverList);
            }

            using var cancelSource = new CancellationTokenSource();
            var pipeThread = new Thread(async () => {
                using var npss = new NamedPipeServerStream(Constants.NAMED_PIPE_NAME, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                while (true) {
                    try {
                        await npss.WaitForConnectionAsync(cancelSource.Token);
                        lock (serverDataLock) {
                            npss.Write(serverData);
                        }
                        npss.WaitForPipeDrain();
                        npss.Disconnect();
                    } catch (OperationCanceledException) {
                        break;
                    } catch (Exception ex) {
                        Trace.WriteLine($"Named pipe error: {ex.Message}");
                    }
                }
            });
            pipeThread.Start();

            if (serverToAdd != null) {
                serverList.Add(ServerInfo.FromUri(new Uri(serverToAdd)));
            }

            Application.Run(new MainWindow(serverList, settings));

            cancelSource.Cancel();
            pipeThread.Join();
        }

        private static byte[] BuildServerData(BindingList<ServerInfo> serverList) {
            var s = new MemoryStream();
            foreach (var server in serverList) {
                server.Write(s);
            }
            return s.ToArray();
        }

        private static BindingList<ServerInfo> LoadServerList() {
            try {
                using var fs = File.Open(GetServerListPath(), FileMode.Open);
                var servers = JsonSerializer.Deserialize<List<Uri>>(fs);
                return new BindingList<ServerInfo>(servers!.Select(u => ServerInfo.FromUri(u)).ToList());
            } catch (FileNotFoundException) {
                return [];
            }
        }

        private static void SaveServerList(BindingList<ServerInfo> servers) {
            using var fs = File.Create(GetServerListPath());
            var serverUriList = servers.Where(s => s.Persistent).Select(s => s.ToUri()).ToList();
            JsonSerializer.Serialize(fs, serverUriList, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        }

        private static string GetServerListPath() {
            return Path.Join(GetMyFolder(), "servers.json");
        }

        public static string GetMyFolder() {
            var p = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ACC Connector");
            Directory.CreateDirectory(p);
            return p;
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            var ex = e.ExceptionObject as Exception;
            var description = $"""
                Exception: {ex!.GetType().Name}
                Message: {ex.Message}
                Stacktrace:
                {ex.StackTrace}
                """;
            MessageBox.Show(description, "ACC Connector has crashed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Debug.Write($"Unhandled exception:\n{description}");
        }

        public static string GetMyVersion() {
            var ver = Assembly.GetExecutingAssembly().GetName().Version!;
            return $"{ver.Major}.{ver.Minor}.{ver.Build}";
        }
    }
}
