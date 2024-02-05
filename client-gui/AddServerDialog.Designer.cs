namespace ACCConnector {
    partial class AddServerDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            serverNameText = new TextBox();
            serverNameLabel = new Label();
            serverAddressLabel = new Label();
            serverAddressText = new TextBox();
            serverPortText = new TextBox();
            serverPortLabel = new Label();
            persistentCheckbox = new CheckBox();
            addButton = new Button();
            cancelButton = new Button();
            errorProvider = new ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            // 
            // serverNameText
            // 
            errorProvider.SetIconPadding(serverNameText, -20);
            serverNameText.Location = new Point(90, 12);
            serverNameText.Name = "serverNameText";
            serverNameText.PlaceholderText = "(Optional server name)";
            serverNameText.Size = new Size(398, 23);
            serverNameText.TabIndex = 2;
            // 
            // serverNameLabel
            // 
            serverNameLabel.AutoSize = true;
            serverNameLabel.Location = new Point(12, 15);
            serverNameLabel.Name = "serverNameLabel";
            serverNameLabel.Size = new Size(72, 15);
            serverNameLabel.TabIndex = 1;
            serverNameLabel.Text = "Server name";
            // 
            // serverAddressLabel
            // 
            serverAddressLabel.AutoSize = true;
            serverAddressLabel.Location = new Point(12, 44);
            serverAddressLabel.Name = "serverAddressLabel";
            serverAddressLabel.Size = new Size(49, 15);
            serverAddressLabel.TabIndex = 2;
            serverAddressLabel.Text = "Address";
            // 
            // serverAddressText
            // 
            errorProvider.SetIconPadding(serverAddressText, -20);
            serverAddressText.Location = new Point(90, 41);
            serverAddressText.Name = "serverAddressText";
            serverAddressText.PlaceholderText = "(Server DNS name or IP address)";
            serverAddressText.Size = new Size(284, 23);
            serverAddressText.TabIndex = 0;
            serverAddressText.Validating += ServerAddressText_Validating;
            // 
            // serverPortText
            // 
            errorProvider.SetIconPadding(serverPortText, -20);
            serverPortText.Location = new Point(415, 41);
            serverPortText.MaxLength = 5;
            serverPortText.Name = "serverPortText";
            serverPortText.PlaceholderText = "(Server port)";
            serverPortText.Size = new Size(73, 23);
            serverPortText.TabIndex = 1;
            serverPortText.Validating += ServerPortText_Validating;
            // 
            // serverPortLabel
            // 
            serverPortLabel.AutoSize = true;
            serverPortLabel.Location = new Point(380, 44);
            serverPortLabel.Name = "serverPortLabel";
            serverPortLabel.Size = new Size(29, 15);
            serverPortLabel.TabIndex = 5;
            serverPortLabel.Text = "Port";
            // 
            // persistentCheckbox
            // 
            persistentCheckbox.AutoSize = true;
            persistentCheckbox.Location = new Point(90, 70);
            persistentCheckbox.Name = "persistentCheckbox";
            persistentCheckbox.Size = new Size(77, 19);
            persistentCheckbox.TabIndex = 3;
            persistentCheckbox.Text = "Persistent";
            persistentCheckbox.UseVisualStyleBackColor = true;
            // 
            // addButton
            // 
            addButton.DialogResult = DialogResult.OK;
            addButton.Location = new Point(332, 95);
            addButton.Name = "addButton";
            addButton.Size = new Size(75, 23);
            addButton.TabIndex = 4;
            addButton.Text = "Add";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += AddButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(413, 95);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 5;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider.ContainerControl = this;
            // 
            // AddServerDialog
            // 
            AcceptButton = addButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(497, 127);
            Controls.Add(cancelButton);
            Controls.Add(addButton);
            Controls.Add(persistentCheckbox);
            Controls.Add(serverPortLabel);
            Controls.Add(serverPortText);
            Controls.Add(serverAddressText);
            Controls.Add(serverAddressLabel);
            Controls.Add(serverNameLabel);
            Controls.Add(serverNameText);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "AddServerDialog";
            Text = "Add server";
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox serverNameText;
        private Label serverNameLabel;
        private Label serverAddressLabel;
        private TextBox serverAddressText;
        private TextBox serverPortText;
        private Label serverPortLabel;
        private CheckBox persistentCheckbox;
        private Button addButton;
        private Button cancelButton;
        private ErrorProvider errorProvider;
    }
}
