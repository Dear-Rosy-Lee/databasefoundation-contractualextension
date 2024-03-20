using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Library.Log
{
    /// <summary>
    /// 日志操作类
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 在日志文件中写入普通消息记录
        /// 该日志信息在运行目录下的Log文件夹中
        /// </summary>
        /// <param name="infoSource">信息来源对象</param>
        /// <param name="id">信息来源对象名称</param>
        /// <param name="msg">信息</param>
        /// <returns></returns>
        public static TrackerObject WriteInfomation(object infoSource, string name, string msg)
        {
            TrackerObject obj = new TrackerObject()
            {
                EventName = name,
                Grade = eMessageGrade.Infomation,
                Description = msg,
                Source = infoSource.GetType().FullName,
            };

            Tracker.WriteLineDebugOnly(obj);
            return obj;
        }

        /// <summary>
        /// 在日志文件中写入错误消息记录
        /// 该日志信息在运行目录下的Log文件夹中
        /// </summary>
        /// <param name="errorSource">错误信息来源对象</param>
        /// <param name="name">错误信息来源对象名称</param>
        /// <param name="msg">错误信息</param>
        /// <param name="methodName">来源方法</param>
        /// <returns></returns>
        public static TrackerObject WriteError(object errorSource, string name, string msg, string methodName = "")
        {
            TrackerObject obj = new TrackerObject()
            {
                EventName = name,
                Grade = eMessageGrade.Error,
                Description = msg,
                Source = errorSource.GetType().FullName,
                Target = string.Format("错误来源于方法：{0}", methodName)
            };

            Tracker.WriteLine(obj);
            return obj;
        }

        /// <summary>
        /// 在日志文件中写入警告消息记录
        /// 该日志信息在运行目录下的Log文件夹中
        /// </summary>
        /// <param name="errorSource">警告信息来源对象</param>
        /// <param name="id">警告信息来源对象名称</param>
        /// <param name="msg">警告信息</param>
        /// <param name="methodName">来源方法</param>
        /// <returns></returns>
        public static TrackerObject WriteWarn(object warnSource, string name, string msg, string methodName = "")
        {
            TrackerObject obj = new TrackerObject()
            {
                EventName = name,
                Grade = eMessageGrade.Warn,
                Description = msg,
                Source = warnSource.GetType().FullName,
                Target = string.Format("警告来源于方法：{0}", methodName)
            };

            Tracker.WriteLine(obj);
            return obj;
        }

        /// <summary>
        /// 在日志文件中写入异常消息记录
        /// 该日志信息在运行目录下的Log文件夹中
        /// </summary>
        /// <param name="exceptionSource">异常信息来源对象</param>
        /// <param name="name">异常信息来源对象名称</param>
        /// <param name="msg">异常信息</param>
        /// <returns></returns>
        public static TrackerObject WriteException(object exceptionSource, string name, string msg)
        {
            TrackerObject obj = new TrackerObject()
            {
                EventName = name,
                Grade = eMessageGrade.Exception,
                Description = msg,
                Source = exceptionSource.GetType().FullName,
            };

            Tracker.WriteLine(obj);
            return obj;
        }
    }
}