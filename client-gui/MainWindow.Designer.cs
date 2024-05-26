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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            addServerButton = new Button();
            removeServerButton = new Button();
            tableLayout = new TableLayoutPanel();
            settingsButton = new Button();
            hookButton = new Button();
            serverListView = new DataGridView();
            Server = new DataGridViewTextBoxColumn();
            tableLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)serverListView).BeginInit();
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
            // tableLayout
            // 
            tableLayout.ColumnCount = 2;
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayout.ColumnStyles.Add(new ColumnStyle());
            tableLayout.Controls.Add(removeServerButton, 1, 1);
            tableLayout.Controls.Add(addServerButton, 1, 0);
            tableLayout.Controls.Add(settingsButton, 1, 3);
            tableLayout.Controls.Add(hookButton, 1, 2);
            tableLayout.Controls.Add(serverListView, 0, 0);
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
            // serverListView
            // 
            serverListView.AllowUserToAddRows = false;
            serverListView.AllowUserToDeleteRows = false;
            serverListView.AllowUserToResizeRows = false;
            serverListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            serverListView.BackgroundColor = SystemColors.Window;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            serverListView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            serverListView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            serverListView.ColumnHeadersVisible = false;
            serverListView.Columns.AddRange(new DataGridViewColumn[] { Server });
            serverListView.EnableHeadersVisualStyles = false;
            serverListView.GridColor = SystemColors.Control;
            serverListView.Location = new Point(3, 3);
            serverListView.Name = "serverListView";
            serverListView.ReadOnly = true;
            serverListView.RowHeadersVisible = false;
            tableLayout.SetRowSpan(serverListView, 4);
            serverListView.RowTemplate.Height = 20;
            serverListView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            serverListView.Size = new Size(477, 281);
            serverListView.TabIndex = 6;
            serverListView.CellFormatting += ServerListView_CellFormatting;
            // 
            // Server
            // 
            Server.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Server.DataPropertyName = "DisplayName";
            Server.HeaderText = "Server";
            Server.Name = "Server";
            Server.ReadOnly = true;
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
            ((System.ComponentModel.ISupportInitialize)serverListView).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button addServerButton;
        private Button removeServerButton;
        private TableLayoutPanel tableLayout;
        private Button settingsButton;
        private Button hookButton;
        private DataGridView serverListView;
        private DataGridViewTextBoxColumn Server;
    }
}
