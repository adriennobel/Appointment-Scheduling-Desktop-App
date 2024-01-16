namespace Scheduling_Desktop_App
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonLogin = new System.Windows.Forms.Button();
            this.flowLayoutPanelPassword = new System.Windows.Forms.FlowLayoutPanel();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.flowLayoutPanelUsername = new System.Windows.Forms.FlowLayoutPanel();
            this.labelUsername = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.flowLayoutPanelLogin = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanelControls = new System.Windows.Forms.FlowLayoutPanel();
            this.labelLocation = new System.Windows.Forms.Label();
            this.flowLayoutPanelPassword.SuspendLayout();
            this.flowLayoutPanelUsername.SuspendLayout();
            this.flowLayoutPanelLogin.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanelControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(3, 13);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(150, 23);
            this.buttonLogin.TabIndex = 2;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // flowLayoutPanelPassword
            // 
            this.flowLayoutPanelPassword.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanelPassword.AutoSize = true;
            this.flowLayoutPanelPassword.Controls.Add(this.labelPassword);
            this.flowLayoutPanelPassword.Controls.Add(this.textBoxPassword);
            this.flowLayoutPanelPassword.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelPassword.Location = new System.Drawing.Point(3, 55);
            this.flowLayoutPanelPassword.Name = "flowLayoutPanelPassword";
            this.flowLayoutPanelPassword.Size = new System.Drawing.Size(156, 46);
            this.flowLayoutPanelPassword.TabIndex = 1;
            this.flowLayoutPanelPassword.WrapContents = false;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPassword.Location = new System.Drawing.Point(3, 0);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(69, 17);
            this.labelPassword.TabIndex = 0;
            this.labelPassword.Text = "Password";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPassword.Location = new System.Drawing.Point(3, 20);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '•';
            this.textBoxPassword.Size = new System.Drawing.Size(150, 23);
            this.textBoxPassword.TabIndex = 1;
            // 
            // flowLayoutPanelUsername
            // 
            this.flowLayoutPanelUsername.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanelUsername.AutoSize = true;
            this.flowLayoutPanelUsername.Controls.Add(this.labelUsername);
            this.flowLayoutPanelUsername.Controls.Add(this.textBoxUsername);
            this.flowLayoutPanelUsername.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelUsername.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanelUsername.Name = "flowLayoutPanelUsername";
            this.flowLayoutPanelUsername.Size = new System.Drawing.Size(156, 46);
            this.flowLayoutPanelUsername.TabIndex = 0;
            this.flowLayoutPanelUsername.WrapContents = false;
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUsername.Location = new System.Drawing.Point(3, 0);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(73, 17);
            this.labelUsername.TabIndex = 0;
            this.labelUsername.Text = "Username";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxUsername.Location = new System.Drawing.Point(3, 20);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(150, 23);
            this.textBoxUsername.TabIndex = 1;
            // 
            // flowLayoutPanelLogin
            // 
            this.flowLayoutPanelLogin.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanelLogin.AutoSize = true;
            this.flowLayoutPanelLogin.Controls.Add(this.buttonLogin);
            this.flowLayoutPanelLogin.Location = new System.Drawing.Point(3, 107);
            this.flowLayoutPanelLogin.Name = "flowLayoutPanelLogin";
            this.flowLayoutPanelLogin.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.flowLayoutPanelLogin.Size = new System.Drawing.Size(156, 39);
            this.flowLayoutPanelLogin.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanelControls, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelLocation, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(624, 321);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // flowLayoutPanelControls
            // 
            this.flowLayoutPanelControls.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanelControls.AutoSize = true;
            this.flowLayoutPanelControls.Controls.Add(this.flowLayoutPanelUsername);
            this.flowLayoutPanelControls.Controls.Add(this.flowLayoutPanelPassword);
            this.flowLayoutPanelControls.Controls.Add(this.flowLayoutPanelLogin);
            this.flowLayoutPanelControls.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelControls.Location = new System.Drawing.Point(231, 86);
            this.flowLayoutPanelControls.Name = "flowLayoutPanelControls";
            this.flowLayoutPanelControls.Size = new System.Drawing.Size(162, 149);
            this.flowLayoutPanelControls.TabIndex = 3;
            this.flowLayoutPanelControls.WrapContents = false;
            // 
            // labelLocation
            // 
            this.labelLocation.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelLocation.AutoSize = true;
            this.labelLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLocation.Location = new System.Drawing.Point(288, 299);
            this.labelLocation.Name = "labelLocation";
            this.labelLocation.Size = new System.Drawing.Size(48, 13);
            this.labelLocation.TabIndex = 4;
            this.labelLocation.Text = "Location";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 321);
            this.Controls.Add(this.tableLayoutPanel2);
            this.MinimumSize = new System.Drawing.Size(480, 250);
            this.Name = "LoginForm";
            this.Text = "Login";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.flowLayoutPanelPassword.ResumeLayout(false);
            this.flowLayoutPanelPassword.PerformLayout();
            this.flowLayoutPanelUsername.ResumeLayout(false);
            this.flowLayoutPanelUsername.PerformLayout();
            this.flowLayoutPanelLogin.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanelControls.ResumeLayout(false);
            this.flowLayoutPanelControls.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelPassword;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelUsername;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelLogin;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelControls;
        private System.Windows.Forms.Label labelLocation;
    }
}

