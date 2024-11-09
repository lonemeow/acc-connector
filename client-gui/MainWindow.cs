using System.ComponentModel;
using System.Runtime.InteropServices;
using Win32;

namespace ACCConnector {
    public partial class MainWindow : Form {
        private readonly BindingList<ServerInfo> serverList;
        private Settings settings;

        public MainWindow(BindingList<ServerInfo> serverList, Settings settings) {
            this.serverList = serverList;
            this.settings = settings;
            InitializeComponent();
            serverListView.AutoGenerateColumns = false;
            serverListView.DataSource = serverList;
            this.Text = "ACC Connector " + ProgramMain.GetMyVersion();
        }

        protected override void OnLoad(EventArgs e) {
            serverListView.ClearSelection();
            User32.SetWindowLongPtr(Handle, User32.GWLP_USERDATA, Constants.TAG);
            base.OnLoad(e);
            CheckHookVersion();
            UpdateHookButton();
        }

        private void AddFromURI(string uri) {
            var serverInfo = ServerInfo.FromUri(new Uri(uri));
            AddOrUpdateServer(serverInfo);
        }

        private void AddOrUpdateServer(ServerInfo newServer) {
            // Check for existing server with same IP:PORT
            var existingServer = serverList.FirstOrDefault(s => s.Hostname == newServer.Hostname && s.Port == newServer.Port);

            if (existingServer != null) {
                // If names differ, prompt user to update
                if (existingServer.Name != newServer.Name) {
                    var msg = $"""
                        A server with the IP address {newServer.Hostname}:{newServer.Port} already exists with name: {existingServer.Name}.
                        Would you like to update the server name to "{newServer.Name}"?
                        """;

                    var result = MessageBox.Show(
                        msg,
                        "Update Server Name",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes) {
                        // Remove old entry and add new server with updated name
                        serverList.Remove(existingServer);
                        serverList.Add(newServer); 
                    }
                } else {
                    // If the server exists with the same name, show a message indicating it already exists
                    MessageBox.Show(
                        $"A server with the IP address {newServer.Hostname}:{newServer.Port} and name \"{existingServer.Name}\" already exists.",
                        "Server Already Exists",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            } else {
                // Add new server if no duplicates
                serverList.Add(newServer);
            }
        }

        private void HandleCopyData(User32.COPYDATASTRUCT copydata) {
            switch (copydata.dwData) {
                case Constants.COPYDATA_SET_URI:
                    var uri = Marshal.PtrToStringUTF8(copydata.lpData, (int)copydata.cbData);
                    Logging.Log(Logging.Severity.DEBUG, $"Add URI request with payload {uri}");
                    AddFromURI(uri);
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
                AddOrUpdateServer(addServerDialog.ServerInfo!);
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
            foreach (DataGridViewRow row in serverListView.SelectedRows) {
                serverList.Remove((ServerInfo)row.DataBoundItem);
            }
        }

        private void HookButton_Click(object sender, EventArgs e) {
            if (ACCHook.IsHookInstalled(settings.AccInstallPath)) {
                ACCHook.RemoveHook(settings.AccInstallPath);
            } else {
                InstallHook();
            }
            UpdateHookButton();
        }

        private void ServerListView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            var item = serverListView.Rows[e.RowIndex].DataBoundItem as ServerInfo;
            if (item?.Address == null) {
                e.CellStyle!.ForeColor = Color.Red;
            }

            if (e.ColumnIndex == Server.Index) {
                if (item?.Persistent == true) {
                    e.Value = "\u2605 " + e.Value;
                }
            }
        }
    }
}
