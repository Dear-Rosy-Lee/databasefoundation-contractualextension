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
    partial class CadastralZoneRepository
    {
        #region Methods - Stable

        /// <summary>
        /// 添加地籍区对象
        /// </summary>
        /// <param name="zone">实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int S_Add(CadastralZone zone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Add(zone);
        }

        /// <summary>
        /// 批量添加地籍区对象
        /// </summary>
        /// <param name="listZone">地籍区对象集合</param>
        /// <param name="overwrite">如果已有对象存在是否覆盖</param>
        /// <param name="action"></param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int S_Add(CadastralList<Zone> listZone, bool overwrite, Action<CadastralZone, int, int> action)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            int count = 0;

            for (int i = 0; i < listZone.Count; i++)
            {
                CadastralZone zone = listZone[i];
                action(zone, i, listZone.Count);

                if (Count(zone.FullCode) > 0)
                {
                    if (!overwrite)
                        continue;

                    CadastralZone old = Get(zone.FullCode);
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
        /// 根据地域代码获得地籍区对象
        /// </summary>
        /// <param name="codeZone">地域代码</param>
        /// <returns>地籍区对象</returns>
        public CadastralZone S_Get(string codeZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Get(codeZone);
        }


        /// <summary>
        /// 根据地域标识码获得地籍区对象
        /// </summary>
        /// <param name="idZone">地域标识码</param>
        /// <returns>地籍区对象</returns>
        public CadastralZone S_Get(Guid idZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Get(idZone);
        }


        /// <summary>
        /// 获得指定区域的地籍区对象
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        public CadastralList<Zone> S_GetChildren(string codeZone, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            CadastralList<Zone> data = null;

            if (levelOption == eLevelOption.Self)
                data = S_GetCollection(codeZone,levelOption);
            else if (codeZone != CadastralZone.China.FullCode)
                data = S_GetCollection(codeZone, levelOption);
            else
            {
                CadastralList<Zone> list = new CadastralList<Zone>();

                data = S_GetCollection(codeZone,eLevelOption.Self);
                foreach (CadastralZone zone in data)
                    list.AddRange(S_GetCollection("UpLevelCode", eLevelOption.Subs));

                data.AddRange(list);
            }

            return data;
        }


        /// <summary>
        /// 获得指定区域的地籍区对象
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        public CadastralList<Zone> S_GetCollection(string codeZone, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            CadastralList<Zone> data = null;

            if (codeZone != CadastralZone.China.FullCode)
                data = levelOption == eLevelOption.Self ?
                       data = S_GetCollection(codeZone,eLevelOption.Self) :
                       data = S_GetCollection(codeZone, eLevelOption.Subs);
            else if (levelOption == eLevelOption.Self)
            {
                data = new CadastralList<Zone>();
                data.Add(Get(codeZone));
            }
            else
            {
                data = S_GetChildren(codeZone, levelOption);
                data.Insert(0, Get(codeZone));

            }

            return data;
        }

        /// <summary>
        /// 统计指定区域的地籍区数量
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <returns></returns>
        public int S_Count(string codeZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Count( codeZone);
        }

        /// <summary>
        /// 根据标识码删除地籍区对象
        /// </summary>
        /// <param name="idZone"></param>
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
        /// 删除指定区域的地籍区对象
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <returns></returns>
        public int S_Delete(string codeZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Delete(codeZone);
        }


        /// <summary>
        /// 删除指定区域的地籍区对象
        /// </summary>
        /// <param name="codeZone">区域代码</param>
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
                      cnt = Delete(c=>c.Code.Equals(codeZone)) :
                      cnt = Delete(c=>c.Code.StartsWith(codeZone));
            return cnt;
        }

        /// <summary>
        /// 删除指定区域的地籍区对象
        /// </summary>
        /// <param name="codeZone">区域代码</param>
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
                      cnt = S_Delete(codeZone) :
                      cnt = Delete(c => c.Code.StartsWith(codeZone));

            return cnt;
        }

        /// <summary>
        /// 更新地籍区对象
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public int S_Update(CadastralZone zone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return S_Update(zone);
        }

        #endregion
    }
}
