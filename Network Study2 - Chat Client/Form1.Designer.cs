namespace Network_Study2___Chat_Client
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.ChatBox = new System.Windows.Forms.ListBox();
            this.IPText = new System.Windows.Forms.TextBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.DisConnectButton = new System.Windows.Forms.Button();
            this.ChatText = new System.Windows.Forms.TextBox();
            this.SendButton = new System.Windows.Forms.Button();
            this.ImagePictureBox = new System.Windows.Forms.PictureBox();
            this.OpenImageButton = new System.Windows.Forms.Button();
            this.SendImageButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ImagePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ChatBox
            // 
            this.ChatBox.FormattingEnabled = true;
            this.ChatBox.Location = new System.Drawing.Point(759, 12);
            this.ChatBox.Name = "ChatBox";
            this.ChatBox.Size = new System.Drawing.Size(487, 537);
            this.ChatBox.TabIndex = 0;
            // 
            // IPText
            // 
            this.IPText.Location = new System.Drawing.Point(48, 31);
            this.IPText.Name = "IPText";
            this.IPText.Size = new System.Drawing.Size(179, 20);
            this.IPText.TabIndex = 1;
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(53, 81);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(173, 43);
            this.ConnectButton.TabIndex = 2;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // DisConnectButton
            // 
            this.DisConnectButton.Location = new System.Drawing.Point(54, 144);
            this.DisConnectButton.Name = "DisConnectButton";
            this.DisConnectButton.Size = new System.Drawing.Size(173, 43);
            this.DisConnectButton.TabIndex = 3;
            this.DisConnectButton.Text = "DisConnect";
            this.DisConnectButton.UseVisualStyleBackColor = true;
            this.DisConnectButton.Click += new System.EventHandler(this.DisConnectButton_Click);
            // 
            // ChatText
            // 
            this.ChatText.Location = new System.Drawing.Point(12, 568);
            this.ChatText.Name = "ChatText";
            this.ChatText.Size = new System.Drawing.Size(1143, 20);
            this.ChatText.TabIndex = 4;
            // 
            // SendButton
            // 
            this.SendButton.Location = new System.Drawing.Point(1161, 568);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(85, 20);
            this.SendButton.TabIndex = 5;
            this.SendButton.Text = "Send";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // ImagePictureBox
            // 
            this.ImagePictureBox.Location = new System.Drawing.Point(252, 31);
            this.ImagePictureBox.Name = "ImagePictureBox";
            this.ImagePictureBox.Size = new System.Drawing.Size(400, 400);
            this.ImagePictureBox.TabIndex = 6;
            this.ImagePictureBox.TabStop = false;
            // 
            // OpenImageButton
            // 
            this.OpenImageButton.Location = new System.Drawing.Point(351, 425);
            this.OpenImageButton.Name = "OpenImageButton";
            this.OpenImageButton.Size = new System.Drawing.Size(313, 35);
            this.OpenImageButton.TabIndex = 7;
            this.OpenImageButton.Text = "Open Image";
            this.OpenImageButton.UseVisualStyleBackColor = true;
            this.OpenImageButton.Click += new System.EventHandler(this.OpenImageButton_Click);
            // 
            // SendImageButton
            // 
            this.SendImageButton.Location = new System.Drawing.Point(351, 466);
            this.SendImageButton.Name = "SendImageButton";
            this.SendImageButton.Size = new System.Drawing.Size(313, 35);
            this.SendImageButton.TabIndex = 8;
            this.SendImageButton.Text = "Send Image";
            this.SendImageButton.UseVisualStyleBackColor = true;
            this.SendImageButton.Click += new System.EventHandler(this.SendImageButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1258, 600);
            this.Controls.Add(this.SendImageButton);
            this.Controls.Add(this.OpenImageButton);
            this.Controls.Add(this.ImagePictureBox);
            this.Controls.Add(this.SendButton);
            this.Controls.Add(this.ChatText);
            this.Controls.Add(this.DisConnectButton);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.IPText);
            this.Controls.Add(this.ChatBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ImagePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ChatBox;
        private System.Windows.Forms.TextBox IPText;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Button DisConnectButton;
        private System.Windows.Forms.TextBox ChatText;
        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.PictureBox ImagePictureBox;
        private System.Windows.Forms.Button OpenImageButton;
        private System.Windows.Forms.Button SendImageButton;
    }
}

