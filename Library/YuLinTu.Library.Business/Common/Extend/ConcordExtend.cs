/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包合同扩展类
    /// </summary>
    public class ConcordExtend
    {
        #region 承包合同

        /// <summary>
        /// 序列化到文件
        /// </summary>
        public static bool SerializeContractInfo(List<string> famliyCollection, List<string> OtherCollection)
        {
            try
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\ConcordContractInfo.xml";
                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }
                ConcordContractInfo info = new ConcordContractInfo();
                info.FamliyCollection = famliyCollection;
                info.OtherCollection = OtherCollection;
                ToolSerialization.SerializeXml(fileName, info);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        public static List<string> DeserializeContractInfo(bool isfamily = false)
        {
            try
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\ConcordContractInfo.xml";
                if (!File.Exists(fileName))
                {
                    List<string> returnlist = new List<string>();
                    returnlist.Add("10");
                    return returnlist;
                }
                ConcordContractInfo info = ToolSerialization.DeserializeXml(fileName, typeof(ConcordContractInfo)) as ConcordContractInfo;
                if (info == null)
                    return new List<string>();
                else
                {
                    if (!isfamily)
                        return info.OtherCollection == null ? new List<string>() : info.OtherCollection;
                    else
                        return info.FamliyCollection == null ? new List<string>() : info.FamliyCollection;
                }
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// 序列化到文件
        /// </summary>
        public static bool SerializeSelectedInfo(List<PersonSelectedInfo> infos)
        {
            try
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\PersonStatusInfo.xml";
                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }
                ToolSerialization.SerializeXml(fileName, infos);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        public static List<PersonSelectedInfo> DeserializeSelectedInfo()
        {
            try
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\PersonStatusInfo.xml";
                if (!File.Exists(fileName))
                {
                    return new List<PersonSelectedInfo>();
                }
                List<PersonSelectedInfo> infos = ToolSerialization.DeserializeXml(fileName, typeof(List<PersonSelectedInfo>)) as List<PersonSelectedInfo>;
                if (infos == null)
                    return new List<PersonSelectedInfo>();
                else
                {
                    return infos;
                }
            }
            catch
            {
                return new List<PersonSelectedInfo>();
            }
        }

        /// <summary>
        /// 序列化到文件
        /// </summary>
        public static bool SerializeConcordContractTimeInfo(ConcordContractTimeInfo infos)
        {
            try
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\ConcordContractTimeInfo.xml";
                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }
                ToolSerialization.SerializeXml(fileName, infos);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        public static ConcordContractTimeInfo DeserializeSerializeConcordContractTimeInfo()
        {
            try
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\ConcordContractTimeInfo.xml";
                if (!File.Exists(fileName))
                {
                    return new ConcordContractTimeInfo();
                }
                ConcordContractTimeInfo infos = ToolSerialization.DeserializeXml(fileName, typeof(ConcordContractTimeInfo)) as ConcordContractTimeInfo;
                if (infos == null)
                    return new ConcordContractTimeInfo();
                else
                {
                    return infos;
                }
            }
            catch
            {
                return new ConcordContractTimeInfo();
            }
        }
        

        #endregion
    }

    /// <summary>
    /// 合同承包信息
    /// </summary>
    public class ConcordContractInfo
    {
        /// <summary>
        /// 家庭承包方式类型
        /// </summary>
        public List<string> FamliyCollection { get; set; }

        /// <summary>
        /// 其他承包方式类型
        /// </summary>
        public List<string> OtherCollection { get; set; }
    }


    /// <summary>
    /// 合同承包签订时间信息
    /// </summary>
    public class ConcordContractTimeInfo
    {
        /// <summary>
        /// 合同承包开始时间
        /// </summary>
        public DateTime ConcordStartTime { get; set; }

        /// <summary>
        /// 合同承包结束时间
        /// </summary>
        public DateTime ConcordEndTime { get; set; }
        
        /// <summary>
        /// 承包方签订时间
        /// </summary>
        public DateTime ContractDate { get; set; }

        /// <summary>
        /// 发包方签订时间
        /// </summary>
        public DateTime SenderDate { get; set; }
        
    }




}
