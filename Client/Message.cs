using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Client
{
    /// <summary>
    /// 消息类，此类应该在服务端和客户端保持一致
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(List<Chess>))]
    [XmlInclude(typeof(List<int>))]
    public class Message
    {
        public const int MAX_LINE_COUNT = 15;
        public const string ID_STATUS_PUT = "落子";
        public const string ID_STATUS_PP = "匹配玩家";
        public const string ID_STATUS_OVER = "游戏结束";
        public const string ID_STATUS_MSG = "聊天";
        public const string ID_STATUS_INIT = "初始化";
        public const string ID_STATUS_BACK = "悔棋";
        public const string ID_STATUS_BACKREQUEST = "请求悔棋";
        public const string ID_STATUS_MSGREFUSED = "消息被拒收";
        public const string ID_STATUS_START = "重新开始";
        public const string ID_STATUS_STARTREQUEST = "请求重新开始";
        public const string ID_STATUS_UPDATEBOARD = "更新棋局";
        public const string ID_STATUS_OFFLINE = "掉线";
        public const string OPPONENT_A = "A";
        public const string OPPONENT_B = "B";
        public const string OPPONENT_NONE = "无色";

        //要执行的动作
        [XmlElement(Order = 1)]
        public string Action { get; set; }
        //接收者
        [XmlElement(Order = 2)]
        public string Receiver { get; set; }
        //发送者
        [XmlElement(Order = 3)]
        public string Sender { get; set; }
        //消息内容
        [XmlElement(Order = 4)]
        public string ExtraMsg { get; set; }
        //轮到谁落子
        [XmlElement(Order = 5)]
        public string WhoseTurn { get; set; }
        //最后一次落子的横坐标
        [XmlElement(Order = 6)]
        public int X { get; set; }
        //最后一次落子的纵坐标
        [XmlElement(Order = 7)]
        public int Y { get; set; }
        //游戏是否结束
        [XmlElement(Order = 8)]
        public bool IsGameOver { get; set; }
        //是否要更新棋盘
        [XmlElement(Order = 9)]
        public bool IsUpdateBoard { get; set; }
        //获胜者
        [XmlElement(Order = 10)]
        public string Winner { get; set; }
        //本方颜色
        [XmlElement(Order = 11)]
        public string Color { get; set; }
        //白子列表
        [XmlElement(Order = 12)]
        public List<Chess> APieces { get; set; }
        //黑子列表
        [XmlElement(Order = 13)]
        public List<Chess> BPieces { get; set; }
        //发送者的昵称
        [XmlElement(Order = 14)]
        public string Name { get; set; }
        //是否是系统消息
        [XmlElement(Order = 15)]
        public bool IsSysMsg { get; set; }
        //文件名
        [XmlElement(Order = 16)]
        public string FileName { get; set; }
        //是否同意重新开始
        [XmlElement(Order = 17)]
        public bool IsReStartAgree { get; set; }

        [XmlElement(Order = 18)]
        public List<int> AColorQ { get; set; }
        [XmlElement(Order = 19)]
        public List<int> BColorQ { get; set; }
        [XmlElement(Order = 20)]
        public bool IsBackAgree { get; set; }

        public Message()
        {

        }
    }
}