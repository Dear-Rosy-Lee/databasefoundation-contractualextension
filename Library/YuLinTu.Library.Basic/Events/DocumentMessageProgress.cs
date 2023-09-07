using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum MessageType
    {
        UseToken = 1,//是否使用加密狗
    }

    /// <summary>
    /// 地域操作事件
    /// </summary>
    public class DocumentMessageProgressEventArgs : EventArgs
    {
        public object Entity { get; set; }//实体
        public object Arguments { get; set; }//参数
        public MessageType MessageType { get; set; }//操作类型
        public bool Success { get; set; }//成功标志
        public string Information { get; set; }//信息
    }

    /// <summary>
    /// Window消息处理
    /// </summary>
    public class DocumentMessageProgress
    {
        public delegate void DocumentMessageProgressDelegate(MessageType operateType, DocumentMessageProgressEventArgs args);
        public static event DocumentMessageProgressDelegate DocumentMessageProgressEvent;

        /// <summary>
        /// 数据处理
        /// </summary>
        /// <returns></returns>
        public static DocumentMessageProgressEventArgs UseToken(bool stop)
        {
            DocumentMessageProgressEventArgs args = new DocumentMessageProgressEventArgs();
            args.Success = stop;
            args.MessageType = MessageType.UseToken;
            if (DocumentMessageProgressEvent != null)
            {
                DocumentMessageProgressEvent(MessageType.UseToken, args);
            }
            return args;
        }

    }
}
