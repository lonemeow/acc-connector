using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace ACCConnector {
    public partial class SettingsDialog : Form {
        public SettingsDialog(Settings settings) {
            InitializeComponent();

            propertyGrid.SelectedObject = settings;
        }
    }
}
