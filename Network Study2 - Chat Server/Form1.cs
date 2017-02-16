using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.IO.Compression;
namespace Network_Study2___Chat_Server
{
    public partial class Form1 : Form
    {
        public static Socket Server,ImageServer,ImageClient;
        public static Socket[] Client = new Socket[15];
        public static Socket InputServer,InputClient;
        //public static byte[] ChatGetByte;
        public const int sPort = 9000;
        public const int DivideN = 4;
        public bool sw;
        public Size Client_Resoultion = new Size(1920,1080);
        public int ServerCount;
        public const int SizeForOne = 20000;
        public Form2 MainPicture;
        Bitmap Orginal_Image;
        Action T;
        Thread[] Chat_Thread = new Thread[15];
        Thread ServerOpenThread;
        Thread ReceiveImageThread;
        IPAddress ServerIP;
        IPEndPoint IPBindPoint,IPBindPoint_ForImage,IPBindPoint_ForInput;

        public class ServerNumberClass
        {
            public int T;
        }
        public Form1()
        {
            InitializeComponent();
            ServerCloseButton.Enabled = false;
            this.FormClosing += CloseAll;   
        }

        private void CloseAll(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Client[0].Connected) Client[0].Close();
                if (InputClient.Connected) InputClient.Close();
                if (ImageClient.Connected) ImageClient.Close();
                ServerOpenThread.Abort();
                ReceiveImageThread.Abort();
            }
            catch(Exception E)
            {
            }
        }

        public void SendInputDown(int KeyValue)
        {
            byte[] Sw = new byte[4];
            byte[] Xp = new byte[4];
            Sw = BitConverter.GetBytes(0);
            Xp = BitConverter.GetBytes(KeyValue);
            InputClient.Send(Sw);
            InputClient.Send(Xp);
            //InputClient.Send(Yp);
        }

        public void SendInputUp(int KeyValue)
        {
            byte[] Sw = new byte[4];
            byte[] Xp = new byte[4];
            Sw = BitConverter.GetBytes(1);
            Xp = BitConverter.GetBytes(KeyValue);
            InputClient.Send(Sw);
            InputClient.Send(Xp);
            //InputClient.Send(Yp);
        }

        public void SendInputMouseUp(int X,int Y)
        {
            byte[] Sw = new byte[4];
            byte[] Xp = new byte[4];
            byte[] Yp = new byte[4];
            Sw = BitConverter.GetBytes(3);
            Xp = BitConverter.GetBytes(X);
            Yp = BitConverter.GetBytes(Y);
            InputClient.Send(Sw);
            InputClient.Send(Xp);
            InputClient.Send(Yp);
        }

        public void SendInputMouseDown(int X, int Y)
        {
            byte[] Sw = new byte[4];
            byte[] Xp = new byte[4];
            byte[] Yp = new byte[4];
            Sw = BitConverter.GetBytes(2);
            Xp = BitConverter.GetBytes(X);
            Yp = BitConverter.GetBytes(Y);
            InputClient.Send(Sw);
            InputClient.Send(Xp);
            InputClient.Send(Yp);
        }

        public void SendInputRightMouseUp(int X, int Y)
        {
            byte[] Sw = new byte[4];
            byte[] Xp = new byte[4];
            byte[] Yp = new byte[4];
            Sw = BitConverter.GetBytes(7);
            Xp = BitConverter.GetBytes(X);
            Yp = BitConverter.GetBytes(Y);
            InputClient.Send(Sw);
            InputClient.Send(Xp);
            InputClient.Send(Yp);
        }

        public void SendInputRightMouseDown(int X, int Y)
        {
            byte[] Sw = new byte[4];
            byte[] Xp = new byte[4];
            byte[] Yp = new byte[4];
            Sw = BitConverter.GetBytes(6);
            Xp = BitConverter.GetBytes(X);
            Yp = BitConverter.GetBytes(Y);
            InputClient.Send(Sw);
            InputClient.Send(Xp);
            InputClient.Send(Yp);
        }

        public void SendInputMouseWheel(int MouseDelta)
        {
            byte[] Sw = new byte[4];
            byte[] Xp = new byte[4];
            Sw = BitConverter.GetBytes(8);
            Xp = BitConverter.GetBytes(MouseDelta);
            InputClient.Send(Sw);
            InputClient.Send(Xp);
        }

        private void ServerOpenButton_Click(object sender, EventArgs e)
        {
            //ServerIP = IPAddress.Parse("127.0.0.1");
            ServerIP = IPAddress.Any;
            IPBindPoint = new IPEndPoint(ServerIP, sPort);
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Server.Bind(IPBindPoint);
            Server.Listen(1);
            IPBindPoint_ForImage = new IPEndPoint(ServerIP, sPort+1);
            //ImageServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ImageServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ImageServer.Bind(IPBindPoint_ForImage);
            //ImageServer.Listen(1);
            IPBindPoint_ForInput = new IPEndPoint(ServerIP, sPort + 2);
            InputServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            InputServer.Bind(IPBindPoint_ForInput);
            InputServer.Listen(1);
            ServerOpenThread = new Thread(new ThreadStart(ServerOpen));
            ServerOpenThread.Start();

            ServerOpenButton.Enabled = false;
            ServerCloseButton.Enabled = true;
        }
        private void ServerCloseButton_Click(object sender, EventArgs e)
        {
            for(int i=0;i<Chat_Thread.Length;i++)
            {
                if(Chat_Thread[i]!=null) Chat_Thread[i].Abort();
                if (Client[i]!=null && Client[i].Connected) Client[i].Close();
            }
            Server.Close();
            Server = null;
            ImageServer.Close();
            ImageServer = null;
            ServerOpenThread.Abort();
            ReceiveImageThread.Abort();
            ServerOpenButton.Enabled = true;
            ServerCloseButton.Enabled = false;
        }

        public Bitmap Resize_Image(Bitmap Image,int SizeW,int SizeH)
        {
            Size S = new Size(SizeW, SizeH);

            return new Bitmap(Image, S);
        }

        public void Server_Send_Chat(Byte[] SetBytes)
        {
            for (int i = 0; i < 15; i++)
            {
                if (Client[i] != null && Client[i].Connected)
                {
                    Client[i].Send(SetBytes);
                    //ChatBoxAdd("Test" + i);
                }
            }
        }

        public Bitmap ChangePicture(Bitmap OrgImage,Bitmap ChangeImage,int Position)
        {
            int X_Pos = Client_Resoultion.Width / DivideN * (Position / DivideN);
            int Y_Pos = Client_Resoultion.Height / DivideN * (Position % DivideN);
            int X_Area = Client_Resoultion.Width / DivideN;
            int Y_Area = Client_Resoultion.Height / DivideN;

            for (int i = 0; i < Y_Area;i++)
            {
                for(int a=0;a<X_Area;a++)
                {
                    Color C = ChangeImage.GetPixel(a, i);
                    OrgImage.SetPixel(a + X_Pos, i + Y_Pos, C);
                }
            }

            return OrgImage;
        }

        EndPoint senderp;
        byte[] GetByte;
        byte[] GetImageBytes;
        byte[] GetImageSize = new byte[15];
        byte[] GetImageAreaByte = new byte[15];
        public void ReceiveImage()
        {
            int GetImageLength;
            int GetImageArea;
            int RealImageLength;
            MemoryStream ms = new MemoryStream();
            IPBindPoint_ForImage = new IPEndPoint(ServerIP, sPort + 1);
            IPBindPoint_ForImage = new IPEndPoint(((IPEndPoint)Client[0].RemoteEndPoint).Address, sPort + 1);
            senderp = (IPEndPoint)IPBindPoint_ForImage;
            Bitmap GetImageBit;
            //Thread.Sleep(10000);
            try
            {
                ImageServer.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 500); 
                while (Client[0].Connected)
                {
                    //Get Image Size
                    //ImageServer.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 0);
                    Client[0].Receive(GetImageSize, 0, 4, SocketFlags.None);
                    RealImageLength = BitConverter.ToInt32(GetImageSize,0);
                    Client[0].Receive(GetImageSize, 0, 4, SocketFlags.None);
                    GetImageLength = BitConverter.ToInt32(GetImageSize,0);
                    if (GetImageLength == 0) continue;
                    //Get Image Area
                    /*ImageClient.Receive(GetImageAreaByte, 0, 4, SocketFlags.None);
                    GetImageArea = BitConverter.ToInt32(GetImageAreaByte, 0);*/
                    GetImageBytes = new byte[GetImageLength];
                    //ImageServer.Receive(GetImageBytes, 0, GetImageLength, SocketFlags.None);
                    //ImageServer.ReceiveFrom(GetImageBytes, 0, GetImageLength, SocketFlags.None, ref senderp);
                    int count=0;
                    int icount = 0;
                    sw = false;
                    do
                    {
                        try
                        {
                            GetByte = new byte[SizeForOne + 4];
                            for (int i=icount; i < GetImageLength / SizeForOne;i++)
                            {
                                //ChatBoxAdd(ImageServer.ReceiveFrom(GetByte, 0, SizeForOne + 4, SocketFlags.None, ref senderp).ToString());
                                ImageServer.BeginReceiveFrom(GetByte, 0, SizeForOne + 4, SocketFlags.None, ref senderp,new AsyncCallback(MessageCallBack),Tuple.Create(GetByte,0,GetImageLength));
                            }
                            Client[0].Send(new byte[4]);
                            Client[0].Receive(new byte[4]);
                            if (GetImageLength % SizeForOne != 0) ImageServer.ReceiveFrom(GetImageBytes, GetImageLength - (GetImageLength % SizeForOne), GetImageLength % SizeForOne, SocketFlags.None, ref senderp);
                            sw = true;
                            Client[0].Send(BitConverter.GetBytes(-1));
                        }
                        catch (Exception Ex3)
                        {
                            ChatBoxAdd(Ex3.ToString());
                            ChatBoxAdd("UDP 데이터 소실 발생.");
                            sw = false;
                            icount = 0;
                            Client[0].Send(BitConverter.GetBytes(-1));
                            break;
                            Client[0].Send(BitConverter.GetBytes(icount));
                        }
                    } while (!sw);
                    try
                    {
                        if (sw == false) throw new Exception();
                        ms = new MemoryStream();
                        ms.Write(GetImageBytes, 0, (int)GetImageBytes.Length);
                        ms.Position = 0;
                        GetImageBytes = new byte[RealImageLength];
                        //var DecompressStream = new DeflateStream(ms, CompressionMode.Decompress,true);
                        var DecompressStream = new GZipStream(ms, CompressionMode.Decompress, true);
                        var DecompressedStream = new MemoryStream();
                        DecompressStream.CopyTo(DecompressedStream);
                        DecompressStream.Close();
                        DecompressedStream.Position = 0;
                        Bitmap Image = new Bitmap(DecompressedStream);
                        //Bitmap Image = new Bitmap(ms);
                        ChatBoxAdd("Success!");
                        MainPicture.MainPictureChange(Image);
                        //pictureBox1Change(Image);
                        Orginal_Image = Image;
                        ms.Close();
                        DecompressedStream.Close();
                        ImageServer.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 0);
                        Client[0].Send(new byte[4]);
                        sw = true;
                    }
                    catch (Exception ex)
                    {
                        ChatBoxAdd(ex.ToString());
                        ImageServer.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 0);
                        Client[0].Send(new byte[4]);
                        sw = true;
                    }
                    GetImageBytes = null;
                    //Thread.Sleep(100);
                    GetImageBit = null;
                    GC.Collect(0);
                    GC.Collect(1);
                }
            }
            catch(Exception Ex2)
            {
                ChatBoxAdd(Ex2.ToString());
            }
        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            Tuple<byte[], int, int> ResultD = (Tuple<byte[], int, int>)aResult.AsyncState;
            byte[] RData = ResultD.Item1;
            int ReceiveCount = ResultD.Item2 + 1;
            int TotalLength = ResultD.Item3;
            int count = BitConverter.ToInt32(RData, 0);

            ChatBoxAdd(count + "번째 데이터를 받았습니다!");
            try
            {
                Array.Copy(GetByte, 4, GetImageBytes, count * SizeForOne, SizeForOne);
                //ImageServer.BeginReceiveFrom(RData, ReceiveCount * SizeForOne, SizeForOne, SocketFlags.None, ref senderp, new AsyncCallback(MessageCallBack), Tuple.Create(RData,ReceiveCount,TotalLength));
            }
            catch(Exception EX4)
            {
                ChatBoxAdd(EX4.ToString());
            }
        }

        public void ImageChangeMethod_ForInvoke(Bitmap Image)
        {
            pictureBox1.Image = Image;
        }

        public void pictureBox1Change(Bitmap Image)
        {
            if(pictureBox1.InvokeRequired)
            {
                Action<Bitmap> A = ImageChangeMethod_ForInvoke;
                pictureBox1.Invoke(A,new object[]{Image});
            }
            else
            {
                pictureBox1.Image = Image;
            }
        }
        delegate void TestD();
        public void ImageFormActivate()
        {
            byte[] width = new byte[4];
            byte[] height = new byte[4];
            Client[0].Receive(width);
            Client[0].Receive(height);
            MainPicture = new Form2(BitConverter.ToInt32(width, 0), BitConverter.ToInt32(height, 0),this);
            MainPicture.Activate();
            MainPicture.Show();
        }
        public void ActivateRealImage()
        {
        }

        public void ServerOpen()
        {
            do{
                if (Server == null) break;
                Client[ServerCount] = Server.Accept();
                if (Client[ServerCount].Connected)
                {
                    //ImageClient = ImageServer.Accept();
                    //Thread ImageFormThread = new Thread(new ThreadStart(ImageFormActivate));
                    //ImageFormThread.Start();
                    //Thread.Sleep(100);
                    InputClient = InputServer.Accept();
                    TestD T = new TestD(ImageFormActivate);
                    BeginInvoke(T);
                    Thread.Sleep(1000);
                    Chat_Thread[ServerCount] = new Thread(new ParameterizedThreadStart(ChatReceive));
                    ServerNumberClass SP = new ServerNumberClass();
                    SP.T = ServerCount;
                    Chat_Thread[ServerCount].Start(SP);
                    ServerCount++;
                    ReceiveImageThread = new Thread(new ThreadStart(ReceiveImage));
                    ReceiveImageThread.Start();
                }
            }while(Client[ServerCount]==null || !Client[ServerCount].Connected);
        }

        public void ChatReceive(object Data)
        {
            ServerNumberClass P = (ServerNumberClass)Data;
            int ServerN = P.T;
            byte[] ChatGetByte = new byte[1024];
            String ChatString;
            try
            {
                while (Client[ServerN].Connected)
                {
                   /* Client[ServerN].Receive(ChatGetByte, 0, ChatGetByte.Length, SocketFlags.None);
                    if (Client[ServerN].Connected == false) break;
                    //Console.WriteLine("Test");
                    ChatString = Encoding.UTF7.GetString(ChatGetByte);
                    if (ChatString != null)
                    {
                        ChatBoxAdd(ChatString);
                        Server_Send_Chat(ChatGetByte);
                    }*/
                }
            }
            catch(Exception ex)
            {
                ChatBoxAdd("Connection Disconnected!");
                Server_Send_Chat(Encoding.UTF7.GetBytes("Connection Disconnected!".ToCharArray()));
            }
        }

        String InvokeString;
        Action<String> InvokeChat;

        public void ChatBoxInvokeVoid(String ChatString)
        {
            ChatBox.Items.Add(ChatString);
        }
        public void ChatBoxAdd(String ChatString)
        {
            InvokeString=ChatString;
            InvokeChat = ChatBoxInvokeVoid;
            if (ChatBox.InvokeRequired) ChatBox.Invoke(InvokeChat,new object[]{ChatString});
            else ChatBox.Items.Add(ChatString);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }
    }
}
