using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.IO.Compression;

namespace Network_Study2___Chat_Client
{
    public partial class Form1 : Form
    {
        public static Socket Client,ImageClient,InputClient;
        public static byte[] getBytes;
        public static byte[] setBytes;
        public int sPort = 9000;
        public static int ServerCount;
        public const int DivideSize = 16;
        public const int DivideN = 4;
        public Size Client_Resoultion = new Size(800, 700);
        public const int SizeForOne = 20000;
        public bool sw;
        Thread Server_Thread;
        Thread Server_OpenThread;
        public Form1 CS;
        IPAddress ServerIP;
        IPEndPoint IPBindPoint,IPBindPoint_ForImage,IPBindPoint_ForInput;
        Thread GetData_From_Client;
        Thread SendImage_To_Server_Thread;
        Thread GetInput_Thread;
        //InterceptMouse MouseActionClass;

        public Form1()
        {
            InitializeComponent();
            DisConnectButton.Enabled = false;
            SendButton.Enabled = false;
            SendImageButton.Enabled = false;
            this.FormClosing += CloseAll;
        }

        private void CloseAll(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Client.Connected) Client.Close();
                if (ImageClient.Connected) ImageClient.Close();
                if (InputClient.Connected) InputClient.Close();
                GetData_From_Client.Abort();
                SendImage_To_Server_Thread.Abort();
                GetInput_Thread.Abort();
            }
            catch (Exception E)
            {
            }
        }

        public Bitmap CaptureImage(int SizeW,int SizeH,int RsizeW,int RsizeH)
        {
            Bitmap ImageBit = new Bitmap(SizeW, SizeH);
            Graphics g = Graphics.FromImage(ImageBit);
            g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(SizeW, SizeH));
            
            return Resize_Image(ImageBit, RsizeW, RsizeH);
        }

        public Bitmap Resize_Image(Bitmap Image, int SizeW, int SizeH)
        {
            Size S = new Size(SizeW, SizeH);

            return new Bitmap(Image, S);
        }

        public Bitmap Get_SubImage(Bitmap GetImage,int X_Size,int Y_Size,int X_Pos,int Y_Pos)
        {
            Bitmap R = new Bitmap(GetImage,new Size(X_Size+1,Y_Size+1));

            for (int i = 0; i < Y_Size;i++)
            {
                for(int a=0;a<X_Size;a++)
                {
                    R.SetPixel(a, i, GetImage.GetPixel(a + X_Pos, i + Y_Pos));
                }
            }

            return R;
        }

        public bool Equal_Compare_Image(Bitmap I1,Bitmap I2,int X_Area,int Y_Area)
        {
            for(int i=0;i<Y_Area;i++)
            {
                for(int a=0;a<X_Area;a++)
                {
                    if (!I1.GetPixel(a, i).Equals(I2.GetPixel(a, i))) return true;
                }
            }
            return false;
        }

        public void CheckChange(Bitmap ScreenImage,Bitmap Now_PictureBox_Image)
        {
            //Bitmap Now_PictureBox_Image = (Bitmap)ImagePictureBox.Image;
            MemoryStream ms = new MemoryStream();
            int X_Area = Client_Resoultion.Width / DivideN;
            int Y_Area = Client_Resoultion.Height / DivideN;
            for(int X=0;X<DivideN;X++)
            {
                for(int Y=0;Y<DivideN;Y++)
                {
                    Bitmap I1 = Get_SubImage(ScreenImage, X_Area, Y_Area, X * X_Area, Y * Y_Area);
                    Bitmap I2 = Get_SubImage(Now_PictureBox_Image, X_Area, Y_Area, X * X_Area, Y * Y_Area);
                    if(Equal_Compare_Image(I1,I2,X_Area,Y_Area))
                    {
                        SendCapture(I1,X*DivideN+Y);
                    }
                }
            }
        }


        public void GetInput()
        {
            byte[] SwB= new byte[4];
            int sw;

            try
            {
                while (InputClient.Connected)
                {
                    InputClient.Receive(SwB, 4, SocketFlags.None);
                    sw = BitConverter.ToInt32(SwB, 0);
                    if(sw==0)
                    {
                        byte[] KeyValue = new byte[4];
                        InputClient.Receive(KeyValue, 4, SocketFlags.None);
                        int KeyCode = BitConverter.ToInt32(KeyValue, 0);
                        InterceptKeyBoard.InputKeyDown(KeyCode);
                    }
                    else if (sw == 1)
                    {
                        byte[] KeyValue = new byte[4];
                        InputClient.Receive(KeyValue, 4, SocketFlags.None);
                        int KeyCode = BitConverter.ToInt32(KeyValue, 0);
                        InterceptKeyBoard.InputKeyUp(KeyCode);
                    }
                    else if (sw == 2)
                    {
                        byte[] Xp = new byte[4];
                        byte[] Yp = new byte[4];
                        InputClient.Receive(Xp, 4, SocketFlags.None);
                        InputClient.Receive(Yp, 4, SocketFlags.None);
                        int X, Y;
                        X = BitConverter.ToInt32(Xp, 0);
                        Y = BitConverter.ToInt32(Yp, 0);
                        InterceptMouse.MouseDown_Left(X, Y);
                        ChatBox.Items.Add("MouseDown " + X + " " + Y);
                    }
                    else if (sw == 3)
                    {
                        byte[] Xp = new byte[4];
                        byte[] Yp = new byte[4];
                        InputClient.Receive(Xp, 4, SocketFlags.None);
                        InputClient.Receive(Yp, 4, SocketFlags.None);
                        int X, Y;
                        X = BitConverter.ToInt32(Xp, 0);
                        Y = BitConverter.ToInt32(Yp, 0);
                        InterceptMouse.MouseUp_Left(X, Y);
                        ChatBox.Items.Add("Mouseup " + X + " " + Y);
                    }
                    else if(sw==6)
                    {
                        byte[] Xp = new byte[4];
                        byte[] Yp = new byte[4];
                        InputClient.Receive(Xp, 4, SocketFlags.None);
                        InputClient.Receive(Yp, 4, SocketFlags.None);
                        int X, Y;
                        X = BitConverter.ToInt32(Xp, 0);
                        Y = BitConverter.ToInt32(Yp, 0);
                        InterceptMouse.MouseDown_Right(X, Y);
                        ChatBox.Items.Add("MouseRightDown " + X + " " + Y);
                    }
                    else if(sw==7)
                    {
                        byte[] Xp = new byte[4];
                        byte[] Yp = new byte[4];
                        InputClient.Receive(Xp, 4, SocketFlags.None);
                        InputClient.Receive(Yp, 4, SocketFlags.None);
                        int X, Y;
                        X = BitConverter.ToInt32(Xp, 0);
                        Y = BitConverter.ToInt32(Yp, 0);
                        InterceptMouse.MouseUp_Right(X, Y);
                        ChatBox.Items.Add("MouseRightUp " + X + " " + Y);
                    }
                    else if(sw==8)
                    {
                        byte[] Xp = new byte[4];
                        InputClient.Receive(Xp, 4, SocketFlags.None);
                        int X;
                        X = BitConverter.ToInt32(Xp, 0);
                        InterceptMouse.MouseWheel(X);
                        ChatBox.Items.Add("MouseWheel " + X);
                    }
                }
            }
            catch(Exception Ex)
            {
            }
        }


        public void SendCapture(Bitmap C_Image,int Pos)
        {
            MemoryStream ms = new MemoryStream();
            C_Image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Position = 0;
            Byte[] SendBytes = new Byte[(int)ms.Length];
            ms.Read(SendBytes, 0, SendBytes.Length);
            ImageClient.Send(BitConverter.GetBytes(SendBytes.Length));
            ImageClient.Send(BitConverter.GetBytes(Pos));
            ImageClient.Send(SendBytes);
        }

        public void SendCaptureImageMethod()
        {
            MemoryStream ms = new MemoryStream();
            Bitmap OrgImage;
            try
            {
                Bitmap C_Image = CaptureImage(1920, 1080, 800, 700);
                Byte[] SendBytes = new Byte[(int)ms.Length];
                Byte[] SendBytesR;
                byte[] testbytes = new byte[15];
                int width,height;
                width = Screen.PrimaryScreen.Bounds.Width;
                height = Screen.PrimaryScreen.Bounds.Height;
                OrgImage = C_Image;
                Client.Send(BitConverter.GetBytes(Screen.PrimaryScreen.Bounds.Width));
                Client.Send(BitConverter.GetBytes(Screen.PrimaryScreen.Bounds.Height));
                do
                {
                    ms = new MemoryStream();
                    C_Image = CaptureImage(width,height,width,height);
                    C_Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //CheckChange(C_Image,OrgImage);
                    var CompressedStream = new MemoryStream();
                    //var CompressStream = new DeflateStream(CompressedStream, CompressionMode.Compress,true);
                    var CompressStream = new GZipStream(CompressedStream, CompressionMode.Compress, true);
                    //ms = new MemoryStream();
                    ms.Position = 0;
                    Client.Send(BitConverter.GetBytes((int)ms.Length));
                    ms.CopyTo(CompressStream);
                    CompressStream.Close();
                    SendBytes = new byte[(int)CompressedStream.Length];
                    //SendBytes = new byte[3004];
                    CompressedStream.Position = 0;
                    CompressedStream.Read(SendBytes, 0, SendBytes.Length);
                    ChatBox.Items.Add(ms.Length + " : " + CompressedStream.Length);
                    CompressedStream.Position = 0;
                    Client.Send(BitConverter.
                    GetBytes(SendBytes.Length));
                    ms.Close();
                    CompressedStream.Close();
                    sw = false;
                    int count = 0;
                    do
                    {
                        SendBytesR = new Byte[SizeForOne + 4];
                        Client.Receive(new byte[4]);
                        Thread.Sleep(50);
                        for (int i = 0; i < SendBytes.Length / SizeForOne; i++)
                        {
                            Array.Copy(BitConverter.GetBytes(i),0,SendBytesR,0,4);
                            Array.Copy(SendBytes, i*SizeForOne, SendBytesR, 4, SizeForOne);
                            //SendBytes.CopyTo(SendBytesR, 4);
                            //ImageClient.SendTo(SendBytes, i * SizeForOne, SizeForOne, SocketFlags.None, IPBindPoint_ForImage);
                            ImageClient.SendTo(SendBytesR, 0, SizeForOne + 4, SocketFlags.None, IPBindPoint_ForImage);
                            //ChatBox.Items.Add(ImageClient.ReceiveBufferSize);
                            Thread.Sleep(25);
                            //ImageClient.BeginSendTo(SendBytesR, 0, SizeForOne + 4, SocketFlags.None, IPBindPoint_ForImage,new AsyncCallback(MessageCallBack),Tuple.Create(SendBytes,count,(int)ms.Length));
                            //if((i+1)%5==0) Thread.Sleep(30);
                            //ChatBox.Items.Add(i + "번째");
                        }
                        Client.Send(new byte[4]);
                        //Thread.Sleep(10);
                        if (SendBytes.Length % SizeForOne != 0) ImageClient.SendTo(SendBytes, (SendBytes.Length / SizeForOne) * SizeForOne, SendBytes.Length - (SendBytes.Length / SizeForOne) * SizeForOne, SocketFlags.None, IPBindPoint_ForImage);
                        Client.Receive(testbytes,0,4,SocketFlags.None);
                        count = BitConverter.ToInt32(testbytes, 0);
                    } while (count!=-1);
                    GC.Collect(0);
                    GC.Collect(1);
                    ms.Close();
                    SendBytes = null;
                    SendBytesR = null;
                    Client.Receive(testbytes, 0, 4, SocketFlags.None);
                } while (Client.Connected);
            }
            catch (OverflowException Ex)
            {
                ChatBox.Items.Add(Ex.ToString());
            }
        }

        private void MessageCallBack(IAsyncResult Result)
        {
            Tuple<byte[], int, int> RData = (Tuple<byte[], int, int>)Result.AsyncState;
            byte[] SendData = RData.Item1;
            byte[] SendBytesR = new byte[SizeForOne+4];
            int SendCount = RData.Item2;
            int TotalLength = RData.Item3;
            //ChatBox.Items.Add(SendCount.ToString() + ":" + TotalLength.ToString());
            SendCount++;
            if (SendCount * (SizeForOne) > TotalLength)
            {
                return;
            }
            else if ((SendCount + 1) * (SizeForOne) >= TotalLength)
            {
                //ImageClient.BeginSendTo(SendData, SizeForOne * SendCount, TotalLength - (SizeForOne * SendCount), SocketFlags.None, IPBindPoint_ForImage, new AsyncCallback(MessageCallBack), Tuple.Create(SendData, SendCount, TotalLength));
                Client.Send(new byte[4]);
                ImageClient.SendTo(SendData, (SendData.Length / SizeForOne) * SizeForOne, SendData.Length - (SendData.Length / SizeForOne) * SizeForOne, SocketFlags.None, IPBindPoint_ForImage);
            }
            else
            {
                Array.Copy(BitConverter.GetBytes(SendCount), 0, SendBytesR, 0, 4);
                Array.Copy(SendData, SendCount * SizeForOne, SendBytesR, 4, SizeForOne);
                ImageClient.BeginSendTo(SendBytesR,0, SizeForOne+4, SocketFlags.None, IPBindPoint_ForImage, new AsyncCallback(MessageCallBack), Tuple.Create(SendData, SendCount, TotalLength));
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            ServerIP = IPAddress.Parse(IPText.Text);
            IPBindPoint = new IPEndPoint(ServerIP, sPort);
            IPBindPoint_ForImage = new IPEndPoint(ServerIP, sPort + 1);
            IPBindPoint_ForInput = new IPEndPoint(ServerIP, sPort + 2);
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ImageClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            InputClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //ImageClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Client.Bind(IPBindPoint);
            //ImageClient.Bind(IPBindPoint_ForImage);
            do
            {
                Client.Connect(IPBindPoint);
                InputClient.Connect(IPBindPoint_ForInput);
                //ImageClient.Connect(IPBindPoint_ForImage);
            } while (!Client.Connected);

            GetData_From_Client = new Thread(new ThreadStart(ReceiveData));
            SendImage_To_Server_Thread = new Thread(new ThreadStart(SendCaptureImageMethod));
            SendImage_To_Server_Thread.Start();
            GetData_From_Client.Start();
            GetInput_Thread = new Thread(new ThreadStart(GetInput));
            GetInput_Thread.Start();
            SendButton.Enabled = true;
            DisConnectButton.Enabled = true;
            ConnectButton.Enabled = false;
            SendImageButton.Enabled = true;
        }

        private void DisConnectButton_Click(object sender, EventArgs e)
        {
            GetData_From_Client.Abort();
            SendImage_To_Server_Thread.Abort();
            Client.Disconnect(false);
            Client.Shutdown(SocketShutdown.Both);
            Client.Close();
            ImageClient.Disconnect(false);
            ImageClient.Shutdown(SocketShutdown.Both);
            ImageClient.Close();
            SendButton.Enabled = false;
            DisConnectButton.Enabled = false;
            ConnectButton.Enabled = true;
            SendImageButton.Enabled = false;
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            setBytes = Encoding.UTF7.GetBytes(ChatText.Text.ToCharArray());
            Client.Send(setBytes);
        }

        public void ReceiveData()
        {
            String GetString;
            try
            {
                while (Client.Connected)
                {
                    /*getBytes = new Byte[1024];
                    Client.Receive(getBytes, 0, getBytes.Length, SocketFlags.None);
                    GetString = Encoding.UTF7.GetString(getBytes);
                    if (GetString != null)
                    {
                        ChatBox.Items.Add(GetString);
                    }*/
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void OpenImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD_Image;

            OFD_Image = new OpenFileDialog();
            OFD_Image.DefaultExt = "jpg";
            OFD_Image.Filter = ".jpg|*.jpg|All(*.*)|*.*";
            OFD_Image.ShowDialog();

            if(OFD_Image.FileName.Length>0)
            {
                Image ImageFromDialog = Image.FromFile(OFD_Image.FileName);
                ImageFromDialog = (Image)new Bitmap(ImageFromDialog, new Size(400, 400));
                Bitmap BitmapFromDialog = new Bitmap(ImageFromDialog, new Size(400, 400));
                //ImagePictureBox.Image = ImageFromDialog;
                ImagePictureBox.Image = BitmapFromDialog;
            }
        }

        private void SendImageButton_Click(object sender, EventArgs e)
        {
            Bitmap ImageBit = (Bitmap)ImagePictureBox.Image;
            byte[] ImageByte;
            Image GetImageBit = ImagePictureBox.Image;
            MemoryStream ms = new MemoryStream();

            ImageBit.Save(ms,System.Drawing.Imaging.ImageFormat.Bmp);
            //GetImageBit.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Position = 0;
            ImageByte = new byte[(int)ms.Length];
            ms.Read(ImageByte, 0, (int)ms.Length);
            int L = (int)ms.Length;
            ChatBox.Items.Add(L.ToString());
            ImageClient.Send(BitConverter.GetBytes(ImageByte.Length),0,4,SocketFlags.None);
            ImageClient.Send(ImageByte);
            Thread.Sleep(200);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }

    public class InterceptKeyBoard
    {
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags,int dwExtraInfo);

        public static void InputKeyDown(int KeyV)
        {
            byte[] KeyByte1 = BitConverter.GetBytes(KeyV);
            byte KeyByte = KeyByte1[0];
            keybd_event(KeyByte, 0, 0, 0);
        }
        public static void InputKeyUp(int KeyV)
        {
            byte[] KeyByte1 = BitConverter.GetBytes(KeyV);
            byte KeyByte = KeyByte1[0];
            keybd_event(KeyByte, 0, 0x0002, 0);
        }
    }

    public class InterceptMouse
    {
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        static int hHook = 0;
        public const int WH_MOUSE = 7;

        private enum MouseMessages
        {
            MOUSEEVENTF_ABSOLUTE = 0x8000,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        private enum MouseEvent_Messages
        {
            MOUSEEVENTF_ABSOLUTE = 0x8000,
            MOUSEEVENTF_MOVE=0x0001,
            LBUTTONDOWN = 0x0002,
            LBUTTONUP = 0x0004,
            RBUTTONDOWN = 0x0008,
            RBUTTONUP = 0x0010,
            WHEEL = 0x0800
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        public static void MouseDown_Left(int x,int y)
        {
            SetCursorPos(x, y);
            //mouse_event((long)MouseEvent_Messages.LBUTTONDOWN, 0, 0, 0, 0);
            //long DX = y * 65535 / Screen.PrimaryScreen.Bounds.Height;
            //long DY = x * 65535 / Screen.PrimaryScreen.Bounds.Width;
            //x = x*65535 / Screen.PrimaryScreen.Bounds.Width;
            //y = y*65535 / Screen.PrimaryScreen.Bounds.Height;
            //mouse_event((long)MouseEvent_Messages.MOUSEEVENTF_ABSOLUTE | (long)MouseEvent_Messages.MOUSEEVENTF_MOVE, DX, DY, 0, 0);
            mouse_event((long)MouseEvent_Messages.LBUTTONDOWN, 0, 0, 0, 0);
        }

        public static void MouseDown_Right(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event((long)MouseEvent_Messages.RBUTTONDOWN, 0, 0, 0, 0);
        }

        public static void MouseUp_Left(int x,int y)
        {
            SetCursorPos(x, y);
            //mouse_event((long)MouseEvent_Messages.LBUTTONUP, 0, 0, 0, 0);
            //long DX = y * 65535 / Screen.PrimaryScreen.Bounds.Height;
            //long DY = x * 65535 / Screen.PrimaryScreen.Bounds.Width;
            //x = y * 65535 / Screen.PrimaryScreen.Bounds.Width;
            //y = y * 65535 / Screen.PrimaryScreen.Bounds.Height;
            //x = y / Screen.PrimaryScreen.Bounds.Width * 65535;
            //y = y / Screen.PrimaryScreen.Bounds.Height * 65535;
            //mouse_event((long)MouseEvent_Messages.MOUSEEVENTF_ABSOLUTE | (long)MouseEvent_Messages.MOUSEEVENTF_MOVE,DX, DY, 0, 0);
            mouse_event((long)MouseEvent_Messages.LBUTTONUP, 0, 0, 0, 0);
        }

        public static void MouseUp_Right(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event((long)MouseEvent_Messages.RBUTTONUP, 0, 0, 0, 0);
        }

        public static void MouseWheel(int Delta)
        {
            mouse_event((long)MouseEvent_Messages.WHEEL, 0,0, Delta, 0);
        }
    }

    /*public static class MouseHook
    {
        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public delegate void PointFunc(int x, int y);

        public static event PointFunc LeftMouseDown;
        public static event PointFunc MouseMove;
        public static event PointFunc LeftMouseUp;
        public static event PointFunc WheelClick;
        public static event PointFunc RightMouseDown;
        public static event PointFunc RightMouseUp;
        /// <summary>
        /// 마우스 후킹을 시작합니다.
        /// </summary>
        public static void HookStart()
        {
            _hookID = SetHook(_proc);
        }
        /// <summary>
        /// 마우스 후킹을 종료합니다.
        /// </summary>
        public static void Hookend()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {


            if (nCode >= 0)
            {
                switch ((MouseMessages)wParam)
                {
                    case MouseMessages.WM_LBUTTONDOWN:
                        if (LeftMouseDown != null)
                        {
                            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                            LeftMouseDown(hookStruct.pt.x, hookStruct.pt.y);
                        }
                        break;
                    case MouseMessages.WM_LBUTTONUP:
                        if (LeftMouseUp != null)
                        {
                            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                            LeftMouseUp(hookStruct.pt.x, hookStruct.pt.y);
                        }
                        break;
                    case MouseMessages.WM_MOUSEMOVE:
                        if (MouseMove != null)
                        {
                            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                            MouseMove(hookStruct.pt.x, hookStruct.pt.y);
                        }
                        break;
                    case MouseMessages.WM_MOUSEWHEEL:
                        if (WheelClick != null)
                        {
                            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                            WheelClick(hookStruct.pt.x, hookStruct.pt.y);
                        }
                        break;
                    case MouseMessages.WM_RBUTTONDOWN:
                        if (RightMouseDown != null)
                        {
                            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                            RightMouseDown(hookStruct.pt.x, hookStruct.pt.y);
                        }
                        break;
                    case MouseMessages.WM_RBUTTONUP:
                        if (RightMouseUp != null)
                        {
                            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                            RightMouseUp(hookStruct.pt.x, hookStruct.pt.y);
                        }
                        break;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        private enum MouseEvent_Messages
        {
            LBUTTONDOWN = 0x0002,
            LBUTTONUP = 0x0004,
            RBUTTONDOWN = 0x0008,
            RBUTTONUP = 0x0010
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        /// <summary>
        /// 현재 마우스의 위치에 클릭 이벤트를 발생시키는 함수입니다.
        /// </summary>
        public static void Click()
        {
            mouse_event((long)MouseEvent_Messages.LBUTTONDOWN, 0, 0, 0, 0);
            Thread.Sleep(100);
            mouse_event((long)MouseEvent_Messages.LBUTTONUP, 0, 0, 0, 0);
        }
        /// <summary>
        /// 설정된 마우스의 위치에 클릭 이벤트를 발생시키는 함수입니다.
        /// </summary>
        /// <param name="x">설정할 마우스의 x 좌표입니다.</param>
        /// <param name="y">설정할 마우스의 y 좌표입니다.</param>
        public static void Click(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event((long)MouseEvent_Messages.LBUTTONDOWN, 0, 0, 0, 0);
            Thread.Sleep(100);
            mouse_event((long)MouseEvent_Messages.LBUTTONUP, 0, 0, 0, 0);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
    }*/
}
