using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    //定义委托类型
    //这些委托用来在子线程中更新界面
    public delegate void StepsChangeHandler(int changeValue);  //将步数设置到label上
    public delegate void HelpHandler(string helpOrColor);  //将帮助信息设置到label上        
    public delegate void ChessEventReceiveHander(object sender, Message e);  //消息事件委托。
    public delegate void PPHandler(Message e);  //匹配玩家后更新界面
    public delegate void PictureChooser(object tag);//选择表情后插入到richTextBox，窗体间传值
    public delegate void UpdatePicture();

    public partial class FormClient : Form
    {
        //声明委托变量
        public static StepsChangeHandler delStep;
        public static HelpHandler delHelp;
        public static UpdatePicture updataPic;
        ChessEventReceiveHander OnReceiveMsg;        //接收信息事件
        PPHandler delPP;

        const int BUFFER_SIZE = 1024*10;//接受数据的缓冲区大小10KB
        //创建 1个客户端套接字 和1个负责监听服务端请求的线程  
        Thread threadclient = null;
        Socket socketclient = null;
        public static string myUUID = "";
        public static string yourUUID = "";
        private DateTime startTime;//记录游戏开始时间
        public string[] mName;//系统自带的自定义昵称
        Random r = new Random();
        ChessPanel chessPanel;


        /// <summary>
        /// 匹配玩家后更新界面
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myColor"></param>
        private void SetPPInfo(Message e)
        {
            this.Invoke(new Action(() => {
                timer1.Start();
                lblWinner.Text = e.Winner;

                if (e.WhoseTurn == e.Color)
                {
                    lblMyColor.Text = "本方(先手)  ";
                    lblYourColor.Text = "对方  ";
                }
                else
                {
                    lblYourColor.Text = "对方(先手)  ";
                    lblMyColor.Text = "本方  ";
                }

                if (e.WhoseTurn == e.Color)
                {
                    lblHelp.Text = "请落子";
                }
                else
                {
                    lblHelp.Text = "等待对方落子";
                }


                if (e.Color == Message.OPPONENT_A)
                {
                    lblMyColor.Image = imageList1.Images["B"];
                    lblYourColor.Image = imageList1.Images["A"];
                }
                else
                {
                    lblMyColor.Image = imageList1.Images["A"];
                    lblYourColor.Image = imageList1.Images["B"];
                }

                chessPanel.Refresh();
            }));

        }
        /// <summary>
        /// 更新步数到界面上，如果传入0，则将步数清零
        /// </summary>
        /// <param name="changeValue">变化量</param>
        private void SetStepsOnLabel(int changeValue)
        {
            lblStep.Invoke(new Action(() =>
            {
                if (changeValue==0)
                {
                    lblStep.Text = "0";
                }
                else
                {
                    lblStep.Text = Convert.ToInt32(lblStep.Text) + changeValue + string.Empty;
                }
            }));
        }
        /// <summary>
        /// 更新帮助信息到界面
        /// </summary>
        /// <param name="helpString"></param>
        private void SetHelpOnLabel(string helpString)
        {
            lblHelp.Invoke(new Action(() =>
            {
                lblHelp.Text = helpString;
            }));
        }
        /// <summary>
        /// 窗体的构造函数
        /// </summary>
        public FormClient()
        {
            InitializeComponent();
            chessPanel = new ChessPanel();
            this.splitContainer2.Panel1.Controls.Add(this.chessPanel);
            chessPanel.BringToFront();
            //chesspanel的设置
            this.chessPanel.BackColor = System.Drawing.Color.DarkOrange;
            this.chessPanel.BackgroundImage = global::Client.Resource.back;
            this.chessPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.chessPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chessPanel.Location = new System.Drawing.Point(0, 67);
            this.chessPanel.MinimumSize = new System.Drawing.Size(100, 100);
            timer1.Stop();

            updataPic = new UpdatePicture(UpDatePicture);
            delStep = new StepsChangeHandler(SetStepsOnLabel);
            delHelp = new HelpHandler(SetHelpOnLabel);
            delPP = new PPHandler(SetPPInfo);

        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            //定义一个套接字  
            socketclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  //ipv4,支持可靠、双向、基于连接的字节流，而不重复数据，也不保留边界,tcp
            socketclient.DontFragment = true;
            //获取文本框中的IP地址  
            IPAddress address = IPAddress.Parse(txtIP.Text.Trim());

            //将获取的IP地址和端口号绑定在网络节点上  
            IPEndPoint point = new IPEndPoint(address, Convert.ToInt32(numericUpDown_port.Value));

            try
            {
                //客户端套接字连接到网络节点上，用的是Connect  
                socketclient.Connect(point);
                socketclient.NoDelay = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("连接失败");
                MessageBox.Show(ex.Message,"连接失败",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            //进行一系列初始化和设置
            this.btnConnect.Enabled = false;
            btnBack.Enabled = true;
            btnDisconnect.Enabled = true;
            btnChange.Enabled = false;
            txtName.Enabled = false;
            btnStart.Enabled = false;
            lblWinner.Text = "暂无";
            ChessPanel.APieces.Clear();
            ChessPanel.BPieces.Clear();
            chessPanel.Refresh();


            OnReceiveMsg += new ChessEventReceiveHander(ManageChessEvent);

            threadclient = new Thread(Receive);
            threadclient.IsBackground = true;
            threadclient.Start();
        }

        /// <summary>
        /// 接收服务端发来信息的方法
        /// </summary>
        private void Receive()
        {
            //持续监听服务端发来的消息 
            while (true)
            {
                try
                {
                    //定义一BUFFER_SIZE大小的内存缓冲区，用于临时性存储接收到的消息
                    //然后循环读取，将读取到的字节数组写入内存流，最后再赋给字节数组
                    byte[] buffer = new byte[BUFFER_SIZE];
                    int len;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        do
                        {
                            //Receive方法是阻塞式接收数据
                            //流中没有数据时会阻塞在这里
                            len = socketclient.Receive(buffer, BUFFER_SIZE, SocketFlags.None);
                            ms.Write(buffer, 0, len); //0-length的字节

                            //可以利用Available属性进行循环读取
                            if (socketclient.Available <= 0)
                            {
                                Thread.Sleep(50);//等待数据全部到达
                                //if (socketclient.Poll(1000,SelectMode.SelectRead))
                                //{

                                //}
                            }
                        } while (socketclient.Available > 0); //在网络缓冲区中没有排队的数据，则 Available 返回 0
                        buffer = ms.GetBuffer();
                    }
                    // 读取的字节数为0说明socket断开，字节数=0不等价于流中没有数据
                    if (buffer.Length == 0)
                    {
                        timer1.Stop();
                        ChessPanel.isGameOver = true;
                        yourUUID = string.Empty;
                        btnStart.Invoke(new Action(() => { btnStart.Enabled = false; }));
                        delHelp("您已经掉线");
                        MessageBox.Show("您已经掉线，请重新连接！","服务器连接失败",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        return;
                    }
                    try
                    {
                        //将套接字获取到的字符数组转换为人可以看懂的字符串  
                        string xml = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        //当数据包很小时或者发送间隔很短
                        //可能得到多条消息，造成xml中有多个根节点，反序列化的时候出错
                        //我们在这里判断，如果是多条消息合并在一起，要进行分离
                        //这种方法比较笨，只是简单的字符串处理
                        do
                        {
                            //从第二个字符开始搜索，如果找到，说明是第二个xml声明
                            int secondIndex = xml.IndexOf("<?xml version=", 1);  //???
                            if (secondIndex > 0)
                            {
                                Message mes = XmlUtils.DeserializeObject<Message>(xml.Substring(0, secondIndex));
                                OnReceiveMsg(this, mes);
                                xml = xml.Substring(secondIndex, xml.Length - secondIndex);
                            }
                            //如果没找到，说明只有一条消息
                            else
                            {
                                Message mes = XmlUtils.DeserializeObject<Message>(xml);
                                OnReceiveMsg(this, mes);
                                break;
                            }
                        } while (true);

                    }
                    catch (Exception exx)
                    {

                        MessageBox.Show(exx.Message);
                        break;
                    }

                }
                catch (Exception ex)
                {
                    //throw;
                    Console.WriteLine("远程服务器已经中断连接"+ex.Message);
                    break;
                }
            }
        }

        public void updataImage()
        {
            if (ChessPanel.ACurrentColors.Count == ChessPanel.AColorQ.Count)
            {
                for (int i = 0; i < ChessPanel.AColorQ.Count; i++)
                {
                    ChessPanel.ACurrentColors[i] = chessPanel .colors[ChessPanel.AColorQ[i]];
                }
            }

            if (ChessPanel.BCurrentColors.Count == ChessPanel.BColorQ.Count)
            {
                for (int i = 0; i < ChessPanel.BColorQ.Count; i++)
                {
                    ChessPanel.BCurrentColors[i] = chessPanel .colors[ChessPanel.BColorQ[i]];
                }
            }
        }
        private void UpDatePicture()
        {
            updataImage();
            /*
            pictureBox1.BackgroundImage = chessPanel.ACurrentColors[0];
            pictureBox2.BackgroundImage = chessPanel.ACurrentColors[1];
            pictureBox3.BackgroundImage = chessPanel.ACurrentColors[2];
            pictureBox4.BackgroundImage = chessPanel.BCurrentColors[0];
            pictureBox5.BackgroundImage = chessPanel.BCurrentColors[1];
            pictureBox6.BackgroundImage = chessPanel.BCurrentColors[2];
            */
            pictureBox1.Invoke(new Action(() =>
            {
                pictureBox1.BackgroundImage = ChessPanel.ACurrentColors[0];

            }));
            pictureBox2.Invoke(new Action(() =>
            {
                pictureBox2.BackgroundImage = ChessPanel.ACurrentColors[1];
            }));
            pictureBox3.Invoke(new Action(() =>
            {
                pictureBox3.BackgroundImage = ChessPanel.ACurrentColors[2];
            }));
            pictureBox4.Invoke(new Action(() =>
            {
                pictureBox4.BackgroundImage = ChessPanel.BCurrentColors[0];
            }));
            pictureBox5.Invoke(new Action(() =>
            {
                pictureBox5.BackgroundImage = ChessPanel.BCurrentColors[1];
            }));
            pictureBox6.Invoke(new Action(() =>
            {
                pictureBox6.BackgroundImage = ChessPanel.BCurrentColors[2];
            }));
        }
        /// <summary>
        /// 根据Action的不同，负责相应事件的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageChessEvent(object sender, Message e)
        {
            switch (e.Action)
            {
                case Message.ID_STATUS_INIT:
                    chessPanel.UpdateVar(e,socketclient);
                    delHelp("正在等待棋友加入");
                    break;
                case Message.ID_STATUS_PP:
                    myUUID = e.Receiver;
                    yourUUID = e.Sender;
                    chessPanel.UpdateVar(e, socketclient);
                    delPP(e);
                    delStep(0);
                    btnStart.Invoke(new Action(() => { btnStart.Enabled = true; }));
                    startTime = DateTime.Now;
                    break;
                case Message.ID_STATUS_OFFLINE:
                    ChessPanel.isGameOver = true;
                    yourUUID = "";
                    lblHelp.Invoke(new Action(() =>
                    {
                        timer1.Stop();
                        lblHelp.Text = e.ExtraMsg;
                        btnStart.Enabled = false;
                    }));
                    break;
                case Message.ID_STATUS_PUT:
                    break;
                case Message.ID_STATUS_BACK:
                    if (e.IsBackAgree)
                    {
                            ChessPanel.APieces = e.APieces;
                            ChessPanel.BPieces = e.BPieces;
                            ChessPanel.AColorQ = e.AColorQ;
                            ChessPanel.BColorQ = e.BColorQ;
                            if (ChessPanel.AColorQ.Count > 0 & ChessPanel.BColorQ.Count > 0 & ChessPanel.APieces.Count > 0 & ChessPanel.BPieces.Count > 0)
                            {
                            if (ChessPanel.mColor==Message.OPPONENT_A)
                            {
                                ChessPanel.AColorQ.RemoveAt(e.AColorQ.Count - 1);
                                ChessPanel.AColorQ.Insert(0, e.APieces.Count - 1);
                                ChessPanel.APieces.RemoveAt(e.APieces.Count - 1);
                            }
                            if (ChessPanel.mColor == Message.OPPONENT_B)
                            {
                                ChessPanel.BColorQ.RemoveAt(e.BColorQ.Count - 1);
                                ChessPanel.BColorQ.Insert(0, e.BPieces.Count - 1);
                                ChessPanel.BPieces.RemoveAt(e.BPieces.Count - 1);
                            }
                            else
                            { }
                            }
                            chessPanel.Invoke(new Action(() =>
                            {
                                chessPanel.Refresh();
                                chessPanel.DrawPieces();
                                UpDatePicture();
                            }));
                            delStep(-1);

                        Message a = new Message();
                        a.Action = Message.ID_STATUS_UPDATEBOARD;
                        a.AColorQ = ChessPanel.AColorQ;
                        a.BColorQ = ChessPanel.BColorQ;
                        a.APieces = ChessPanel.APieces;
                        a.BPieces = ChessPanel.BPieces;
                        a.WhoseTurn=ChessPanel.mColor == Message.OPPONENT_B ? Message.OPPONENT_A : Message.OPPONENT_B; //与本方相反
                        a.Sender = myUUID;
                        a.Receiver = yourUUID;
                        socketclient.Send(Encoding.UTF8.GetBytes(XmlUtils.XmlSerializer<Message>(a)));
                    }
                    else
                    { 
                        delHelp("对方已拒绝");
                    }
                    break;
                case Message.ID_STATUS_UPDATEBOARD:
                    chessPanel.UpdateVar2(e, socketclient);
                    //UpDatePicture();
                    break;
                case Message.ID_STATUS_OVER:
                    chessPanel.UpdateVar2(e, socketclient);
                    //UpDatePicture();
                    lblTime.Invoke(new Action(() =>
                    {
                        lblWinner.Text = e.Winner;
                        lblHelp.Text = Message.ID_STATUS_OVER;
                        timer1.Stop();
                    }));
                    break;
                case Message.ID_STATUS_START:
                    if (e.IsReStartAgree==false)
                    {
                        delHelp("对方已拒绝");
                    }
                    else
                    {
                        delStep(0);
                        lblTime.Invoke(new Action(() =>
                        {
                            timer1.Stop();
                        }));
                    }
                    btnStart.Invoke(new Action(() => { btnStart.Enabled = true; }));
                    break;
                case Message.ID_STATUS_STARTREQUEST:
                    DialogResult dr= MessageBox.Show(e.ExtraMsg, "重新开始", MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                    Message m = new Message();
                    m.Sender = myUUID;
                    m.Receiver = yourUUID;
                    m.Action = Message.ID_STATUS_START;
                    if (dr==DialogResult.Yes)
                    {
                        timer1.Stop();
                        m.IsReStartAgree = true;
                        delStep(0);
                    }
                    else
                    {
                        m.IsReStartAgree = false;
                    }
                    socketclient.Send(Encoding.UTF8.GetBytes(XmlUtils.XmlSerializer<Message>(m)));
                    break;
                case Message.ID_STATUS_BACKREQUEST:
                    DialogResult dr1 = MessageBox.Show(e.ExtraMsg, "对方请求悔棋", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    Message k = new Message();
                    k.Sender = myUUID;
                    k.Receiver = yourUUID;
                    k.APieces = ChessPanel.APieces;
                    k.BPieces = ChessPanel.BPieces;
                    k.AColorQ = ChessPanel.AColorQ;
                    k.BColorQ = ChessPanel.BColorQ;
                    k.IsUpdateBoard = true;
                    k.Action = Message.ID_STATUS_BACK;
                    if (dr1 == DialogResult.Yes)
                    {
                        timer1.Stop();
                        k.IsBackAgree = true;
                    }
                    else
                    {
                        k.IsBackAgree = false;
                    }
                    socketclient.Send(Encoding.UTF8.GetBytes(XmlUtils.XmlSerializer<Message>(k)));
                    break;
                case Message.ID_STATUS_MSGREFUSED:
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 定时器每隔1秒更新界面上的计时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine((DateTime.Now - startTime).ToString("hhmmss"));
            lblTime.Text = (DateTime.Now - startTime).ToString("m\\:ss");
        }
        /// <summary>
        /// 窗体加载的时候，随机设置昵称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_Load(object sender, EventArgs e)
        {
            mName = txtName.Lines;
            txtName.Text = mName[r.Next(mName.Length)];
            richTextMsg.Text = "欢迎" + txtName.Text + ",可在此记录你的想法:";
        }
        /// <summary>
        /// 更换昵称，发送框的文本随之改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChange_Click(object sender, EventArgs e)
        {
            txtName.Text = mName[r.Next(mName.Length)];
            richTextMsg.Text = "欢迎" + txtName.Text + ",可在此记录你的想法:";
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            socketclient.Close();
            timer1.Stop();
            ChessPanel.isGameOver = true;
            delStep(0);

            lblHelp.Text = "未连接网络";
            btnConnect.Enabled = true;
            btnBack.Enabled = false;
            btnStart.Enabled = false;
            btnDisconnect.Enabled = false;
            btnChange.Enabled = true;
            txtName.Enabled = true;

            OnReceiveMsg -= new ChessEventReceiveHander(ManageChessEvent);
        }
        /// <summary>
        /// 设置棋盘纯色背景
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                DialogResult dr = dlg.ShowDialog();
                if (dr==DialogResult.OK)
                {
                    chessPanel.BackColor = dlg.Color;
                    chessPanel.BackgroundImage = null;

                    chessPanel.Refresh();
                    chessPanel.ChessPanel_DoubleClick(sender, e);
                }
            }
        }

        /// <summary>
        /// 悔棋
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack_Click(object sender, EventArgs e)
        {
            if (ChessPanel.mWhoseTurn==ChessPanel.mColor) //本方落子 对方可悔棋
            {
                return;
            }
            Message mes = new Message();
            mes.Action = Message.ID_STATUS_BACKREQUEST;
            mes.Sender = myUUID;
            mes.Receiver = yourUUID;
            mes.Color = ChessPanel.mColor;
            mes.APieces = ChessPanel.APieces;
            mes.BPieces = ChessPanel.BPieces;
            mes.AColorQ = ChessPanel.AColorQ;
            mes.BColorQ = ChessPanel.BColorQ;
            socketclient.Send(Encoding.UTF8.GetBytes(XmlUtils.XmlSerializer<Message>(mes)));
        }
        /// <summary>
        /// 设置棋盘的图片背景
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPicBack_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg=new OpenFileDialog())
            {
                dlg.Filter = "位图图像|*.jpg;*.png;*.bmp";
                dlg.InitialDirectory = Application.StartupPath+"\\background\\";
                if (dlg.ShowDialog()==DialogResult.OK)
                {
                    chessPanel.BackgroundImage= Image.FromFile(dlg.FileName);
                    chessPanel.BackColor = Color.Transparent;

                    chessPanel.Refresh();
                    chessPanel.ChessPanel_DoubleClick(sender, e);
                }
                
            }
            
        }

        /// <summary>
        /// 重新开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            lblHelp.Text= "等待对方确认";
            btnStart.Enabled = false;

            Message m = new Message();
            m.Action = Message.ID_STATUS_STARTREQUEST;
            m.Sender = myUUID;
            m.Receiver = yourUUID;
            socketclient.Send(Encoding.UTF8.GetBytes(XmlUtils.XmlSerializer<Message>(m)));
        }

        /// <summary>
        /// Close the socket safely.
        /// </summary>
        /// <param name="socket">The socket.</param>
        public void SafeClose(Socket socket)
        {
            if (socket == null)
                return;

            if (!socket.Connected)
                return;

            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }

            try
            {
                socket.Close();
            }
            catch
            {
            }
        }

    }
}
