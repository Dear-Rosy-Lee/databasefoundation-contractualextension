/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 用户
    /// </summary>
    public class ToolUser
    {
        /// <summary>
        /// 系列化用户信息
        /// </summary>
        /// <param name="objCom"></param>
        public static void SerializeUser(string userName, string password)
        {
            UserEntity entity = new UserEntity();
            entity.UserName = ToolCoding.InitalizeTokenFile(userName);
            entity.Password = ToolCoding.InitalizeTokenFile(password);
            string directoryName = ToolConfiguration.GetSpecialAppSettingValue("ProfileDirectoryName", "YuLinTu Files");
            string fileName = System.Windows.Forms.Application.StartupPath + @"\" + directoryName + @"\User.xml";
            ToolSerialization.SerializeBinary(fileName, entity);
        }

        /// <summary>
        /// 反系列化用户信息
        /// </summary>
        /// <param name="objCom"></param>
        public static UserEntity DeserializeUser()
        {
            string directoryName = ToolConfiguration.GetSpecialAppSettingValue("ProfileDirectoryName", "YuLinTu Files");
            string fileName = System.Windows.Forms.Application.StartupPath + @"\" + directoryName + @"\User.xml";
            if (string.IsNullOrEmpty(fileName)||!File.Exists(fileName))
            {
                return null;
            }
            UserEntity entity = ToolSerialization.DeserializeBinary(fileName, typeof(UserEntity)) as UserEntity;
            entity.UserName = ToolCoding.DeserializeTokenInformation(entity.UserName);
            entity.Password = ToolCoding.DeserializeTokenInformation(entity.Password);
            return entity;
        }

        /// <summary>
        /// 是否是已登录用户
        /// </summary>
        public static bool IsLogined()
        {
            UserEntity entity = DeserializeUser();
            return entity != null && entity.UserName.ToLower() != "guest";
        }
    }

    /// <summary>
    /// 用户实体
    /// </summary>
    [Serializable]
    public class UserEntity
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
