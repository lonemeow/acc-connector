using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace ACCConnector {
    public partial class AddServerDialog : Form {
        public ServerInfo? ServerInfo { get; set; }

        public AddServerDialog() {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, EventArgs e) {
            ValidateChildren();

            if (errorProvider.ReallyHasErrors()) {
                DialogResult = DialogResult.None;
            } else {
                try {
                    var name = serverNameText.Text.Length > 0 ? serverNameText.Text : null;
                    var hostname = serverAddressText.Text;
                    var address = Dns.GetHostAddresses(hostname, AddressFamily.InterNetwork)[0];
                    var port = ushort.Parse(serverPortText.Text);
                    var persistent = persistentCheckbox.Checked;
                    ServerInfo = new ServerInfo(name, hostname, address, port, persistent);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Error resolving server address", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                }
            }
        }

        private void ServerAddressText_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
            if (serverAddressText.Text.Length == 0) {
                errorProvider.SetError(serverAddressText, "Can not be empty");
            } else {
                errorProvider.SetError(serverAddressText, "");
            }
        }

        private void ServerPortText_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
            if (!ushort.TryParse(serverPortText.Text, out _)) {
                errorProvider.SetError(serverPortText, "Invalid port number");
            } else {
                errorProvider.SetError(serverPortText, "");
            }
        }
    }

    public static class ErrorProviderExtensions {

        public static bool ReallyHasErrors(this ErrorProvider provider) {
            // ErrorProvider.HasErrors doesn't actually work; see https://github.com/dotnet/winforms/issues/10424

            // There is no public property to get the list of Errors
            var privateItems = (System.Collections.IDictionary)(typeof(ErrorProvider).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(provider)!);
            // privateItems is Dictionary<Control, ControlItem> but ControlItem is an internal class.
            var controls = privateItems.Keys;

            foreach (Control item in controls) {
                if (!string.IsNullOrEmpty(provider.GetError(item))) { return true; }
            }

            return false;
        }
    }
}
