using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Win32;

namespace ACCConnector {
    public partial class MainWindow : Form {
        private readonly BindingList<ServerInfo> serverList;
        private Settings settings;

        public MainWindow(BindingList<ServerInfo> serverList, Settings settings) {
            this.serverList = serverList;
            this.settings = settings;
            InitializeComponent();
            serverListBox.DataSource = serverList;
        }

        protected override void OnLoad(EventArgs e) {
            User32.SetWindowLongPtr(Handle, User32.GWLP_USERDATA, Constants.TAG);

            base.OnLoad(e);
            CheckHookVersion();
            UpdateHookButton();
        }

        private void AddFromURI(string uri) {
            serverList.Add(ServerInfo.FromUri(new Uri(uri)));
        }

        private void HandleCopyData(User32.COPYDATASTRUCT copydata) {
            switch (copydata.dwData) {
                case Constants.COPYDATA_SET_URI:
                    AddFromURI(Marshal.PtrToStringUTF8(copydata.lpData, (int)copydata.cbData));
                    break;
            }

        }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case User32.WM_COPYDATA:
                    User32.COPYDATASTRUCT copydata = (User32.COPYDATASTRUCT)m.GetLParam(typeof(User32.COPYDATASTRUCT))!;
                    HandleCopyData(copydata);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void CheckHookVersion() {
            if (ACCHook.IsHookOutdated(settings.AccInstallPath)) {
                var msg = """
                    Hook from a different version of ACC Connector seems to be installed.
                    Do you want to replace it with the hook from this version?
                    """;
                if (MessageBox.Show(msg, "ACC Connector", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                    InstallHook();
                }
            }
        }

        private void InstallHook() {
            if (ACCHook.IsACCRunning()) {
                var message = """
                        ACC seems to be running.
                        The hook may fail to install, and even if it succceeds it won't take effect until you restart the game.
                        """;
                if (MessageBox.Show(message, "ACC Connector", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel) {
                    return;
                }
            }
            ACCHook.InstallHook(settings.AccInstallPath);
        }

        private void UpdateHookButton() {
            if (ACCHook.IsHookInstalled(settings.AccInstallPath)) {
                hookButton.Text = "Remove\nhook";
            } else {
                hookButton.Text = "Install\nhook";
            }
        }

        private void AddServerButton_Click(object sender, EventArgs e) {
            using AddServerDialog addServerDialog = new();
            if (addServerDialog.ShowDialog() == DialogResult.OK) {
                serverList.Add(addServerDialog.ServerInfo!);
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e) {
            var tempSettings = settings with { };
            using SettingsDialog settingsDialog = new(tempSettings);
            if (settingsDialog.ShowDialog() == DialogResult.OK) {
                settings = tempSettings;
                Settings.Save(settings);
                UpdateHookButton();
            }
        }

        private void RemoveServerButton_Click(object sender, EventArgs e) {
            var toRemove = serverListBox.SelectedItems.OfType<ServerInfo>().ToList();
            foreach (var i in toRemove) {
                serverList.Remove(i);
            }
        }

        private void ServerListBox_Format(object sender, ListControlConvertEventArgs e) {
            var item = (ServerInfo)e.ListItem!;
            var sb = new StringBuilder();
            if (item.Persistent) {
                sb.Append('\u2605');
            }
            sb.Append(item.DisplayName);
            e.Value = sb.ToString();
        }

        private void HookButton_Click(object sender, EventArgs e) {
            if (ACCHook.IsHookInstalled(settings.AccInstallPath)) {
                ACCHook.RemoveHook(settings.AccInstallPath);
            } else {
                InstallHook();
            }
            UpdateHookButton();
        }
    }
}
