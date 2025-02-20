// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.Data;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    partial class ZoneRepository
    {
        #region Methods - Stable

        /// <summary>
        /// 添加地域对象
        /// </summary>
        /// <param name="zone">地域对象实体</param>
        /// <returns></returns>
        public int S_Add(Zone zone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Add(zone);
        }
        /// <summary>
        /// 批量添加地域对象
        /// </summary>
        /// <param name="listZone">地域对象集合</param>
        /// <param name="overwrite">是否覆盖</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public int S_Add(List<Zone> listZone, bool overwrite, Action<Zone, int, int> action)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            int count = 0;

            for (int i = 0; i < listZone.Count; i++)
            {
                Zone zone = listZone[i];
                action(zone, i, listZone.Count);

                if (Count(c=>c.FullCode.Equals(zone.FullCode)) > 0)
                {
                    if (!overwrite)
                        continue;

                    Zone old = Get(zone.FullCode);
                    zone.ID = old.ID;
                    zone.CreateTime = old.CreateTime;
                    zone.LastModifyTime = DateTime.Now;
                    count += Update(zone);
                }
                else
                {
                    zone.CreateTime = DateTime.Now;
                    zone.LastModifyTime = DateTime.Now;
                    count += Add(zone);
                }
            }

            return count;
        }

        /// <summary>
        /// 根据地域全编码获得地域
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <returns>地域</returns>
        public Zone S_Get(string codeZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Get(codeZone);
        }

        /// <summary>
        /// 根据地域ID获得地域
        /// </summary>
        /// <param name="idZone">地域ID</param>
        /// <returns>地域</returns>
        public Zone S_Get(Guid idZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Get(idZone);
        }

        /// <summary>
        /// 获取指定区域的地域集合
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        public List<Zone> S_GetChildren(string codeZone, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            List<Zone> data = null;

            if (levelOption == eLevelOption.Self)
                data = S_GetCollection(codeZone, levelOption);
            else if (codeZone !=  ZoneHelper.China.FullCode)
                data = S_GetCollection(codeZone, eLevelOption.Subs);
            else
            {
                List<Zone> list = new List<Zone>();

                data = S_GetCollection(codeZone, eLevelOption.Self);
                foreach (Zone zone in data)
                    list.AddRange(S_GetCollection(zone.FullCode, eLevelOption.Subs));

                data.AddRange(list);
            }

            return data;
        }

        /// <summary>
        /// 获取指定区域的地域集合
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        public List<Zone> S_GetCollection(string codeZone, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            List<Zone> data = null;
            if (codeZone != ZoneHelper.China.FullCode)
            {
                data =GetChildren(codeZone, levelOption);
            }
            else
            {
                List<Zone> zoneList = Get();
                if (zoneList.Count > 0)
                {
                    data = new List<Zone>();
                    zoneList.ForEach(t => data.Add(t));
                }
            }
            return data;
        }

        /// <summary>
        /// 根据地域全编码统计地域数量
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <returns></returns>
        public int S_Count(string codeZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Count(c=>c.FullCode.Equals(codeZone));
        }

        /// <summary>
        /// 根据地域id删除对象
        /// </summary>
        /// <param name="idZone">地域id</param>
        /// <returns></returns>
        public int S_Delete(Guid idZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Delete(idZone);
        }

        /// <summary>
        /// 根据地域全编码删除对象
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <returns></returns>
        public int S_Delete(string codeZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Delete( c=>c.FullCode.Equals(codeZone));
        }

        /// <summary>
        /// 根据地域全编码删除地域对象
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        public int S_DeleteChildren(string codeZone, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            int cnt = levelOption == eLevelOption.Self ?
                      cnt = Delete( codeZone) :
                      cnt = Delete(c => c.FullCode.StartsWith(codeZone));

            return cnt;
        }

        /// <summary>
        /// 根据地域全编码删除地域对象
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        public int S_DeleteCollection(string codeZone, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            int cnt = levelOption == eLevelOption.Self ?
                      cnt = Delete(codeZone) :
                      cnt = Delete(c => c.FullCode.StartsWith(codeZone));

            return cnt;
        }

        /// <summary>
        /// 更新地域对象
        /// </summary>
        /// <param name="zone">地域</param>
        /// <returns></returns>
        public int S_Update(Zone zone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Update(zone);
        }

        #endregion
    }
}
