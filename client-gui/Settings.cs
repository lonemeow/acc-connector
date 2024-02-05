using System.ComponentModel;
using System.Drawing.Design;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows.Forms.Design;

namespace ACCConnector {
    public record Settings {


        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public required string AccInstallPath { get; init; }

        public static Settings? Load() {
            try {
                using var fs = File.Open(GetSettingsPath(), FileMode.Open);
                return JsonSerializer.Deserialize<Settings>(fs);
            } catch (FileNotFoundException) {
                return null;
            }
        }

        public static void Save(Settings settings) {
            using var fs = File.Create(GetSettingsPath());
            JsonSerializer.Serialize(fs, settings, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        }

        private static string GetSettingsPath() {
            return Path.Join(ProgramMain.GetMyFolder(), "settings.json");
        }
    }
}
