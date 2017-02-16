using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Network_Study2___Chat_Server
{
    public partial class Form2 : Form
    {
        Form1 MainForm;
        public Form2(int width,int height,Form1 MainForm)
        {
            InitializeComponent();
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height-80;
            ImagePanel.Width = Screen.PrimaryScreen.Bounds.Width-20;
            ImagePanel.Height = Screen.PrimaryScreen.Bounds.Height-120;
            ImagePanel.AutoScroll = true;
            ImageBox.Width = width;
            ImageBox.Height = height;
            ImagePanel.Controls.Add(ImageBox);
            ImageBox.MouseDown += MouseDownPosition;
            ImageBox.MouseUp += MouseUpPosition;
            ImageBox.MouseWheel += MouseWheelEvent;
            //ImageBox.MouseClick += MouseDownPosition;
            this.MainForm = MainForm;
            this.KeyDown += KeyBoardInputDown;
            this.KeyUp += KeyBoardInputUp;
        }

        private void KeyBoardInputDown(object sender,KeyEventArgs e)
        {
            MainForm.SendInputDown(e.KeyValue);
        }
        private void KeyBoardInputUp(object sender, KeyEventArgs e)
        {
            MainForm.SendInputUp(e.KeyValue);
        }

        private void MouseWheelEvent(object sender,MouseEventArgs e)
        {
            MainForm.SendInputMouseWheel(e.Delta);
        }

        private void MouseDownPosition(object sender,MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left)
            {
                MainForm.SendInputMouseDown(e.X, e.Y);
                //MainForm.SendInputMouseUp(e.X, e.Y);
            }
            else if(e.Button==MouseButtons.Middle)
            {

            }
            else if (e.Button == MouseButtons.Right)
            {
                MainForm.SendInputRightMouseDown(e.X, e.Y);
            }
        }

        private void MouseUpPosition(object sender,MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left)
            {
                MainForm.SendInputMouseUp(e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Middle)
            {

            }
            else if (e.Button == MouseButtons.Right)
            {
                MainForm.SendInputRightMouseUp(e.X, e.Y);
            }
        }

        public void ImageChangeMethod_ForInvoke(Bitmap Image)
        {
          ImageBox.Image = Image;
        }

        public void MainPictureChange(Bitmap Image)
        {
            if (ImageBox.InvokeRequired)
            {
                Action<Bitmap> A = ImageChangeMethod_ForInvoke;
                ImageBox.Invoke(A, new object[] { Image });
            }
            else
            {
                ImageBox.Image = Image;
            }
        }

        private void ImageBox_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
