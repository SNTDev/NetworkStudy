namespace Network_Study2___Chat_Server
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
            this.ServerOpenButton = new System.Windows.Forms.Button();
            this.ServerCloseButton = new System.Windows.Forms.Button();
            this.ChatBox = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ServerOpenButton
            // 
            this.ServerOpenButton.Location = new System.Drawing.Point(12, 9);
            this.ServerOpenButton.Name = "ServerOpenButton";
            this.ServerOpenButton.Size = new System.Drawing.Size(165, 42);
            this.ServerOpenButton.TabIndex = 1;
            this.ServerOpenButton.Text = "Server Start";
            this.ServerOpenButton.UseVisualStyleBackColor = true;
            this.ServerOpenButton.Click += new System.EventHandler(this.ServerOpenButton_Click);
            // 
            // ServerCloseButton
            // 
            this.ServerCloseButton.Location = new System.Drawing.Point(12, 57);
            this.ServerCloseButton.Name = "ServerCloseButton";
            this.ServerCloseButton.Size = new System.Drawing.Size(165, 42);
            this.ServerCloseButton.TabIndex = 2;
            this.ServerCloseButton.Text = "Server Close";
            this.ServerCloseButton.UseVisualStyleBackColor = true;
            this.ServerCloseButton.Click += new System.EventHandler(this.ServerCloseButton_Click);
            // 
            // ChatBox
            // 
            this.ChatBox.FormattingEnabled = true;
            this.ChatBox.Location = new System.Drawing.Point(242, 9);
            this.ChatBox.Name = "ChatBox";
            this.ChatBox.Size = new System.Drawing.Size(1189, 524);
            this.ChatBox.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(28, 193);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(187, 373);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1510, 725);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ChatBox);
            this.Controls.Add(this.ServerCloseButton);
            this.Controls.Add(this.ServerOpenButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ServerOpenButton;
        private System.Windows.Forms.Button ServerCloseButton;
        private System.Windows.Forms.ListBox ChatBox;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

