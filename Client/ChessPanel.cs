using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Media;
using System.Threading;
using System.Drawing.Text;

namespace Client
{
    public partial class ChessPanel : UserControl
    {
        Random rd = new Random();
        public const int MAX_LINE_COUNT = 15;//棋盘线的数量
        private float mLineLength;//每个格子的线长，也就是格子的边长
        private int mPanelWidth;//棋盘外围棋盘线的宽度，是个正方形
        private Image red1;
        private Image red2;
        private Image red3;
        private Image yellow1;
        private Image yellow2;
        private Image yellow3;
        public List<Image> colors = new List<Image>();//随机颜色集合
        public static List<Image> ACurrentColors = new List<Image>();
        public static List<Image> BCurrentColors = new List<Image>();
        public  static List<int> AColorQ = new List<int>();
        public  static List<int> BColorQ = new List<int>();
        private float ratioPieceOfLineHeight = 3.0f / 4;// 棋子占棋子格的比例，因为两个相邻棋子不能占完一个格子，要留一定空隙

        public static List<Chess> BPieces = new List<Chess>();//白棋点的坐标集合
        public static List<Chess> APieces = new List<Chess>();//黑棋点的坐标集合
        public static bool isGameOver = false;//游戏结束的标志
        public static string mColor = "";//本方颜色
        public static string mWhoseTurn = "";//轮到谁
        public static bool mIsUpdateBoard;//悔棋的标志，是否要更新棋盘
        private Socket mClientSocket;//客户端的套接字
        private int mOffset;//使线条画在棋盘居中位置，有一个偏移量



        public ChessPanel()
        {
            InitializeComponent();

            mPanelWidth = Math.Min(Width, Height);
            mLineLength = mPanelWidth * 1.0f / ChessPanel.MAX_LINE_COUNT;
            red1 = Resource.red1;
            red2 = Resource.red2;
            red3 = Resource.red3;
            yellow1 = Resource.yellow1;
            yellow2 = Resource.yellow2;
            yellow3 = Resource.yellow3;
            //定义底层颜色
            colors.Add(red1);
            colors.Add(red2);
            colors.Add(red3);
            colors.Add(yellow1);
            colors.Add(yellow2);
            colors.Add(yellow3);
            //初始化当前颜色队列
            ACurrentColors.Add(red1);
            ACurrentColors.Add(red2);
            ACurrentColors.Add(red3);

            AColorQ.Add(0);
            AColorQ.Add(1);
            AColorQ.Add(2);

            BCurrentColors.Add(yellow1);
            BCurrentColors.Add(yellow2);
            BCurrentColors.Add(yellow3);

            BColorQ.Add(3);
            BColorQ.Add(4);
            BColorQ.Add(5);
        }

        private void ChessPanel_Paint(object sender, PaintEventArgs e)
        {
            //画棋盘线
            DrawLines(e);
            //若果有棋子，把棋子也画上去
            DrawPieces();
        }

        private void DrawLines(PaintEventArgs e)
        {
            mPanelWidth = Math.Min(Width, Height);
            mLineLength = mPanelWidth * 1.0f / ChessPanel.MAX_LINE_COUNT;
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Black, 2);
            //画棋盘线
            if (Width > Height)
            {
                for (int i = 0; i < ChessPanel.MAX_LINE_COUNT; i++)
                {
                    int startX = (int)(mLineLength / 2);
                    int endX = (int)(mPanelWidth - mLineLength / 2);
                    int y = (int)((0.5 + i) * mLineLength);
                    //横线
                    g.DrawLine(pen, startX + mOffset, y, endX + mOffset, y);
                    //竖线
                    g.DrawLine(pen, y + mOffset, startX, y + mOffset, endX);
                }
            }
            else
            {
                for (int i = 0; i < ChessPanel.MAX_LINE_COUNT; i++)
                {
                    int startX = (int)(mLineLength / 2);
                    int endX = (int)(mPanelWidth - mLineLength / 2);
                    int y = (int)((0.5 + i) * mLineLength);
                    //横线
                    g.DrawLine(pen, startX, y + mOffset, endX, y + mOffset);
                    //竖线
                    g.DrawLine(pen, y, startX + mOffset, y, endX + mOffset);
                }
            }
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChessPanel_MouseClick(object sender, MouseEventArgs e)
        {
            //isGameOver=true，游戏结束，不响应鼠标点击事件，直接返回
            //mWhoseTurn==Message.COLOR_NONE，没有指定轮到谁，那么谁都不响应
            //mWPieces=null当前还没有组队
            //mColor.Equals(mWhoseTurn)==false没有轮到自己
            if (isGameOver || mWhoseTurn == Message.OPPONENT_NONE || !mColor.Equals(mWhoseTurn) || BPieces == null || APieces == null)
            {
                return;  //
            }

            Point point = new Point();
            Chess chess = new Chess(0,0,0);

            // 将点击的坐标转换成棋盘交叉点的坐标   0-14
            point.X = (int)((e.X - mOffset) / mLineLength);
            point.Y = (int)(e.Y / mLineLength);
            chess._point = point;

            //如果点击的格子里已经有棋子了，就返回
            if (BPieces.Contains(chess) || APieces.Contains(chess))
            {
                return;
            }
            //判断是否点到外面去了
            if (point.X < 0 || point.Y < 0 || point.X >= ChessPanel.MAX_LINE_COUNT || point.Y >= ChessPanel.MAX_LINE_COUNT)
            {
                return;
            }

            //转换成棋子在控件里的位置坐标
            int x, y;
            if (Width > Height)
            {
                x = (int)((point.X + ratioPieceOfLineHeight / 4) * mLineLength) + mOffset;
                y = (int)((point.Y + ratioPieceOfLineHeight / 4) * mLineLength);
            }
            else
            {
                x = (int)((point.X + ratioPieceOfLineHeight / 4) * mLineLength);
                y = (int)((point.Y + ratioPieceOfLineHeight / 4) * mLineLength) - mOffset;
            }
            int width = (int)(mLineLength * ratioPieceOfLineHeight);
            //创建控件的GDI+，准备绘制棋子
            Graphics g = CreateGraphics();
            //待绘制的棋子的位置
            Rectangle rect = new Rectangle(x, y, width, width);
            //画棋子
            if (mColor.Equals(Message.OPPONENT_A))   //规定AB方  a=black
            {
                g.DrawImage(ACurrentColors[0], rect);
                chess._point = point;
                chess.color = colors.IndexOf(ACurrentColors[0]);

                APieces.Add(chess);
                isGameOver = ChessFinishiUtils.checkWin(APieces);
            }
            else if (mColor.Equals(Message.OPPONENT_B))
            {
                g.DrawImage(BCurrentColors[0], rect);
                chess.color = colors.IndexOf(BCurrentColors[0]);
                BPieces.Add(chess);
                isGameOver = ChessFinishiUtils.checkWin(BPieces);
            }
            //更新当前颜色队列
            int i = 0;
            //Image LastImage = colors[i];
            if (mColor.Equals(Message.OPPONENT_A))   //规定AB方  a=black
            {
               i = rd.Next(3);
                Image LastImage = colors[i];
                if (ACurrentColors.Count > 0)
                {
                    ACurrentColors.RemoveAt(0);
                }
                ACurrentColors.Add(LastImage);
                if (AColorQ.Count > 0)
                {
                    AColorQ.RemoveAt(0);
                }
                AColorQ.Add(i);
            }
            else if (mColor.Equals(Message.OPPONENT_B))
            {
                i = rd.Next(3) + 3;
                Image LastImage = colors[i];
                if(BCurrentColors.Count>0)
                {
                    BCurrentColors.RemoveAt(0);
                }
                BCurrentColors.Add(LastImage);
                if (BColorQ.Count > 0)
                {
                    BColorQ.RemoveAt(0);
                }
                BColorQ.Add(i);
            }


            //本方点击，向服务器落子消息
            Message m = new Message();
            m.Action = Message.ID_STATUS_PUT;
            m.WhoseTurn = mWhoseTurn;
            m.Receiver = FormClient.yourUUID;
            m.Sender = FormClient.myUUID;
            m.X = point.X;
            m.Y = point.Y;
            m.APieces = APieces;
            m.BPieces = BPieces;
            m.AColorQ = AColorQ;
            m.BColorQ = BColorQ;
            m.Color = mColor;

            if (isGameOver)
            {
                m.IsGameOver = true;
                m.Winner = mColor;
                DrawResult(m);
            }
            else { m.IsGameOver = false; }
            try
            {
                mClientSocket.Send(Encoding.UTF8.GetBytes(XmlUtils.XmlSerializer<Message>(m)));

                FormClient.delHelp("等待对方落子");
                FormClient.updataPic();
                FormClient.delStep(1);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            //禁止本方点击
            mWhoseTurn = Message.OPPONENT_NONE;
        }

        /// <summary>
        /// 更新变量
        /// </summary>
        public void UpdateVar(Message e, Socket client)
        {

            mWhoseTurn = e.WhoseTurn;
            mColor = e.Color;
            mClientSocket = client;
            APieces = e.APieces;
            BPieces = e.BPieces;


            mIsUpdateBoard = e.IsUpdateBoard;

            if (mIsUpdateBoard)
            {
                DrawPieces();
                FormClient.delHelp("请落子");
            }

            isGameOver = e.IsGameOver;
            if (isGameOver)
            {
                DrawResult(e);
            }

        }

        public void UpdateVar2(Message e, Socket client)
        {
            mWhoseTurn = e.WhoseTurn;
            //mColor = e.Color;
            mClientSocket = client;
            APieces = e.APieces;
            BPieces = e.BPieces;
            AColorQ = e.AColorQ;
            BColorQ = e.BColorQ;
            mIsUpdateBoard = e.IsUpdateBoard;
            if (mIsUpdateBoard)
            {
                this.Refresh();
                DrawPieces();
                FormClient.delHelp("请落子");
            }

            FormClient.updataPic();

            isGameOver = e.IsGameOver;

            if (isGameOver)
            {
                DrawResult(e);
            }

        }

        /// <summary>
        /// 胜负已分，在各自的棋盘上画相应的图片和文字
        /// </summary>
        /// <param name="e"></param>
        private void DrawResult(Message e)
        {
            string winner = e.Winner;
            //棋盘控件的gdi+
            Graphics g = CreateGraphics();
            Bitmap targetBmp = new Bitmap(Width / 2, Height / 2 + 80);
            //要画的图片的gdi+
            Graphics bmp = Graphics.FromImage(targetBmp);
            //把图片背景设为白色
            bmp.Clear(Color.White);
            //文本布局
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            //字体
            Font font1 = new Font("微软雅黑", 20F);
            Font font2 = new Font("微软雅黑", 10F);
            //矩形
            Rectangle rect1 = new Rectangle(0, targetBmp.Height - 80, targetBmp.Width, 50);
            Rectangle rect2 = new Rectangle(0, targetBmp.Height - 30, targetBmp.Width, 25);

            bmp.DrawString("双击任意位置继续", font2, Brushes.Black, rect2, format);
            bmp.DrawString(e.ExtraMsg, font1, Brushes.Blue, rect1, format);

            //胜利
            if (Message.OPPONENT_A.Equals(mColor) && Message.OPPONENT_A.Equals(winner) || Message.OPPONENT_B.Equals(mColor) && Message.OPPONENT_B.Equals(winner))
            {
                bmp.DrawImage(Resource.win, 0, 0, targetBmp.Width, Height / 2);
                g.DrawImage(targetBmp, (Width - targetBmp.Width) / 2, (Height - targetBmp.Height) / 2);
            }
            //失败
            else if (Message.OPPONENT_A.Equals(mColor) && Message.OPPONENT_B.Equals(winner) || Message.OPPONENT_B.Equals(mColor) && Message.OPPONENT_A.Equals(winner))
            {
                bmp.DrawImage(Resource.fail, 0, 0, targetBmp.Width, Height / 2);
                g.DrawImage(targetBmp, (Width - targetBmp.Width) / 2, (Height - targetBmp.Height) / 2);
            }
            //平局
            else if (e.Winner == Message.OPPONENT_NONE && e.ExtraMsg == "平局")
            {
                bmp.DrawImage(Resource.none, 0, 0, targetBmp.Width, Height / 2);
                g.DrawImage(targetBmp, (Width - targetBmp.Width) / 2, (Height - targetBmp.Height) / 2);
            }
            //其他情况忽略
        }

        private void ChessPanel_SizeChanged(object sender, EventArgs e)
        {
            mPanelWidth = Math.Min(this.Width, this.Height);
            mLineLength = mPanelWidth * 1.0f / ChessPanel.MAX_LINE_COUNT;
            mOffset = (Math.Max(Width, Height) - mPanelWidth) / 2;
            this.Refresh();
            DrawPieces();
        }

        /// <summary>
        /// 画棋子
        /// </summary>
        public void DrawPieces()
        {
            if (APieces == null || BPieces == null)
            {
                return;
            }
            Graphics g = this.CreateGraphics();
            //画A棋
            foreach (var item in APieces)
            {
                //转换成棋子在控件里的位置坐标
                int x = (int)((item._point.X + ratioPieceOfLineHeight / 4) * mLineLength) + mOffset;
                int y = (int)((item._point.Y + ratioPieceOfLineHeight / 4) * mLineLength);
                int width = (int)(mLineLength * ratioPieceOfLineHeight);
                Rectangle rect = new Rectangle(x, y, width, width);
                g.DrawImage(colors[item.color], rect);
            }
            //画B棋
            foreach (var item in BPieces)
            {
                //转换成棋子在控件里的位置坐标
                int x = (int)((item._point.X + ratioPieceOfLineHeight / 4) * mLineLength) + mOffset;
                int y = (int)((item._point.Y + ratioPieceOfLineHeight / 4) * mLineLength);
                int width = (int)(mLineLength * ratioPieceOfLineHeight);
                Rectangle rect = new Rectangle(x, y, width, width);
                g.DrawImage(colors[item.color], rect);
            }
        }



        private void ChessPanel_Resize(object sender, EventArgs e)
        {
            //先别删这部分
        }

        public void ChessPanel_DoubleClick(object sender, EventArgs e)
        {
            this.Refresh();
            ChessPanel_Paint(sender, new PaintEventArgs(CreateGraphics(), this.ClientRectangle));
        }


        ////避免闪屏
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;
        //        return cp;
        //    }
        //}

    }
}
