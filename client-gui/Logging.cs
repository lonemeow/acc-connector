using System.Diagnostics;

namespace ACCConnector {
    internal class Logging {
        public enum Severity {
            FATAL,
            ERROR,
            WARNING,
            INFO,
            DEBUG
        }

        private static StreamWriter? logStream;

        public static void Init(string logFolder) {
            Directory.CreateDirectory(logFolder);
            logStream = new StreamWriter(File.Open(Path.Join(logFolder, "app.log"), FileMode.Append, FileAccess.Write, FileShare.Read)) {
                AutoFlush = true
            };
        }

        public static void Log(Severity severity, string msg) {
            logStream?.WriteLine($"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff} {severity}: {msg}");
            Trace.WriteLine(msg);
        }
    }
}
