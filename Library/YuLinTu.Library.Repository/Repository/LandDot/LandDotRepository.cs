// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 界址点表的数据访问类
    /// </summary>
    public class LandDotRepository : RepositoryDbContext<LandDot>,  ILandDotRepository
    {          
        #region Ctor
        
        private IDataSourceSchema m_DSSchema = null;

        public LandDotRepository(IDataSource ds)
            : base(ds) 
        {
            m_DSSchema = ds.CreateSchema();
        }
        #endregion

        #region Methods

        /// <summary>
        /// 检查表是否存在
        /// </summary>
        /// <returns></returns>
        private bool CheckTableExist()
        {
            //try
            //{
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, "LandDot");
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 根据标识码获取界址点表数据
        /// </summary>
        /// <param name="id">标识码</param>
        /// <returns>界址点表数据</returns>
        public LandDot Get(Guid id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return null;
            object obj = Get(c => c.ID.Equals(id));

            return (LandDot)obj;

        }

        /// <summary>
        /// 根据x、y坐标与区域代码获取界址点表数据
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>界址点表数据</returns>
        public LandDot Get(double x, double y, string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;
            object obj = Get(c => c.XCoordinate.Equals(x) && c.YCoordinate.Equals(y) && c.ZoneCode.Equals(zoneCode));
            return (LandDot)obj;

        }

        /// <summary>
        /// 根据区域代码获取界址点表数据
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>界址点表数据集合</returns>
        public LandDotCollection Get(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;
            object obj = Get(c => c.ZoneCode.Equals(zoneCode));

            return (LandDotCollection)obj;

        }

        /// <summary>
        /// 根据x、y坐标获取界址点表数据
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <returns>界址点表数据</returns>
        public LandDot Get(double x, double y)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            var data=Get(c => c.XCoordinate.Equals(x) && c.YCoordinate.Equals(y));
            if (data.Count == 0)
                return null;
            return (LandDot)data[0];

        }

        /// <summary>
        /// 更新界址点表数据
        /// </summary>
        /// <param name="landDot">实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(LandDot landDot)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (landDot == null)
                return -1;
            int val = 0;
            try
            {
                val = Update(landDot, c => c.ID.Equals(landDot.ID));
            }
            catch
            {
                return -1;
            }
            return val;

        }

        /// <summary>
        /// 删除界址点表数据
        /// </summary>
        /// <param name="landDotID">界址点表标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid landDotID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(landDotID))
                return -1;
            int val = 0;
            try
            {
                val = Delete(c => c.ID.Equals(landDotID));
            }
            catch
            {
                return -1;
            }
            return val;

        }

        /// <summary>
        /// 根据区域代码删除界址点表数据
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZone(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;
            int val = 0;
            try
            {
                val = Delete(c => c.ZoneCode.Equals(zoneCode));
            }
            catch
            {
                return -1;
            }
            return val;

        }

        #endregion
    }
}