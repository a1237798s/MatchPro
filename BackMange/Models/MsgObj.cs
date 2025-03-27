namespace BackMange.Models
{  // 給讀取聊天訊息的API調用
    public class MsgObj
    {
        public string msgType { get; set; }
        public int userId { get; set; }         
        public int? posterId { get; set; }
    }
}
