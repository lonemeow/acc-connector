namespace ACCConnector {
    partial class MainWindow {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            addServerButton = new Button();
            removeServerButton = new Button();
            serverListBox = new ListBox();
            tableLayout = new TableLayoutPanel();
            settingsButton = new Button();
            hookButton = new Button();
            tableLayout.SuspendLayout();
            SuspendLayout();
            // 
            // addServerButton
            // 
            addServerButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            addServerButton.Location = new Point(486, 3);
            addServerButton.Name = "addServerButton";
            addServerButton.Size = new Size(75, 23);
            addServerButton.TabIndex = 1;
            addServerButton.Text = "Add...";
            addServerButton.UseVisualStyleBackColor = true;
            addServerButton.Click += AddServerButton_Click;
            // 
            // removeServerButton
            // 
            removeServerButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            removeServerButton.Location = new Point(486, 32);
            removeServerButton.Name = "removeServerButton";
            removeServerButton.Size = new Size(75, 23);
            removeServerButton.TabIndex = 2;
            removeServerButton.Text = "Remove";
            removeServerButton.UseVisualStyleBackColor = true;
            removeServerButton.Click += RemoveServerButton_Click;
            // 
            // serverListBox
            // 
            serverListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            serverListBox.FormattingEnabled = true;
            serverListBox.IntegralHeight = false;
            serverListBox.ItemHeight = 15;
            serverListBox.Location = new Point(3, 3);
            serverListBox.Name = "serverListBox";
            tableLayout.SetRowSpan(serverListBox, 4);
            serverListBox.SelectionMode = SelectionMode.MultiExtended;
            serverListBox.Size = new Size(477, 281);
            serverListBox.TabIndex = 3;
            serverListBox.Format += ServerListBox_Format;
            // 
            // tableLayout
            // 
            tableLayout.ColumnCount = 2;
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayout.ColumnStyles.Add(new ColumnStyle());
            tableLayout.Controls.Add(removeServerButton, 1, 1);
            tableLayout.Controls.Add(serverListBox, 0, 0);
            tableLayout.Controls.Add(addServerButton, 1, 0);
            tableLayout.Controls.Add(settingsButton, 1, 3);
            tableLayout.Controls.Add(hookButton, 1, 2);
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.Location = new Point(0, 0);
            tableLayout.Name = "tableLayout";
            tableLayout.RowCount = 4;
            tableLayout.RowStyles.Add(new RowStyle());
            tableLayout.RowStyles.Add(new RowStyle());
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayout.RowStyles.Add(new RowStyle());
            tableLayout.Size = new Size(564, 287);
            tableLayout.TabIndex = 4;
            // 
            // settingsButton
            // 
            settingsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            settingsButton.Location = new Point(486, 261);
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(75, 23);
            settingsButton.TabIndex = 4;
            settingsButton.Text = "Settings...";
            settingsButton.UseVisualStyleBackColor = true;
            settingsButton.Click += SettingsButton_Click;
            // 
            // hookButton
            // 
            hookButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            hookButton.AutoSize = true;
            hookButton.Location = new Point(486, 215);
            hookButton.Name = "hookButton";
            hookButton.Size = new Size(75, 40);
            hookButton.TabIndex = 5;
            hookButton.Text = "Install\r\nhook\r\n";
            hookButton.UseVisualStyleBackColor = true;
            hookButton.Click += HookButton_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(564, 287);
            Controls.Add(tableLayout);
            MaximizeBox = false;
            Name = "MainWindow";
            Text = "ACC Connector";
            tableLayout.ResumeLayout(false);
            tableLayout.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button addServerButton;
        private Button removeServerButton;
        private ListBox serverListBox;
        private TableLayoutPanel tableLayout;
        private Button settingsButton;
        private Button hookButton;
    }
}
