// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 集体建设用地使用权界址点的数据访问类
    /// </summary>
    public class BuildLandBoundaryAddressDotRepository : RepositoryDbContext<BuildLandBoundaryAddressDot>, IBuildLandBoundaryAddressDotRepository
    {
        #region Ctor

        private IDataSourceSchema m_DSSchema = null;

        public BuildLandBoundaryAddressDotRepository(IDataSource ds)
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, typeof(BuildLandBoundaryAddressDot).GetAttribute<DataTableAttribute>().TableName);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 根据标识码获取集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="id">标识码</param>
        /// <returns>集体建设用地使用权界址点对象</returns>
        public BuildLandBoundaryAddressDot Get(Guid id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return null;

            var data = Get(c => c.ID == id).FirstOrDefault();
            return data;
        }

        /// <summary>
        /// 判断标识码对象是否存在？
        /// </summary>
        /// <param name="id">标识码</param>
        public bool Exist(Guid id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                            + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return false;

            return Count(c => c.ID == id) > 0 ? true : false;
        }

        /// <summary>
        /// 根据集体建设用地使用权ID获取集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>集体建设用地使用权界址点对象集合</returns>
        public List<BuildLandBoundaryAddressDot> GetByLandID(Guid landID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                            + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(landID))
                return null;

            var data = Get(c => c.LandID == landID);
            return data;
        }

        /// <summary>
        /// 根据承包地块ID获取有效的界址点对象集合
        /// </summary>
        /// <param name="landID">承包地块ID</param>
        /// <param name="isValid">是否可用</param>
        /// <returns>集体建设用地使用权界址点对象集合</returns>
        public List<BuildLandBoundaryAddressDot> GetByLandID(Guid landID, bool isValid)
        {
            if (!CheckRule.CheckGuidNullOrEmpty(landID))
                return new List<BuildLandBoundaryAddressDot>();

            var data = Get(c => c.LandID == landID && c.IsValid == isValid);
            return data;
        }

        /// <summary>
        /// 根据集体建设用地使用权ID统计集体建设用地使用权界址点对象数量
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int CountByLandID(Guid landID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                            + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(landID))
                return -1;

            return Count(c => c.LandID == landID);
        }

        /// <summary>
        /// 更新集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="dot">集体建设用地使用权界址点对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(BuildLandBoundaryAddressDot dot)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                            + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (dot == null || !CheckRule.CheckGuidNullOrEmpty(dot.ID))
                return -1;

            int cnt = 0;
            cnt = Update(dot, c => c.LandNumber == dot.LandNumber && c.ID == dot.ID);
            return cnt;
        }

        /// <summary>
        /// 根据标识码删除集体建设用地使用权界址点对象数量
        /// </summary>
        /// <param name="ID">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid ID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                            + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(ID))
                return -1;
            int cnt = 0;
            cnt = Delete(c => c.ID == ID);
            return cnt;
        }

        /// <summary>
        /// 根据集体建设用地使用权ID删除集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="propertyID">集体建设用地使用权ID</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByProperty(Guid propertyID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                            + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(propertyID))
                return -1;
            int cnt = 0;
            cnt = Delete(c => c.LandID == propertyID);
            return cnt;
        }

        /// <summary>
        /// 根据地域代码、土地权属类型获取集体建设用地使用权界址点对象集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="landType">土地权属类型</param>
        /// <returns>集体建设用地使用权界址点对象集合</returns>
        public List<BuildLandBoundaryAddressDot> GetByZoneCode(string zoneCode, eSearchOption searchType, string landType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return null;
            }
            List<BuildLandBoundaryAddressDot> entity = null;
            if (searchType == eSearchOption.Fuzzy)
                entity = Get(c => c.ZoneCode.Contains(zoneCode) && c.LandType.Equals(landType));
            else
                entity = Get(c => c.ZoneCode.Equals(zoneCode) && c.LandType.Equals(landType));
            return entity;
        }

        /// <summary>
        /// 根据地域代码及地域匹配级别获取集体建设用地使用权界址点对象集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>集体建设用地使用权界址点对象集合</returns>
        public List<BuildLandBoundaryAddressDot> GetByZoneCode(string zoneCode, eLevelOption levelOption = eLevelOption.Self)
        {
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;
            List<BuildLandBoundaryAddressDot> entity = Get(c => c.ZoneCode.Equals(zoneCode));
            if (levelOption == eLevelOption.Subs)
                entity = Get(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode);
            else if (levelOption == eLevelOption.SelfAndSubs)
                entity = Get(c => c.ZoneCode.StartsWith(zoneCode));
            return entity;
        }

        /// <summary>
        /// 根据地块标识获取界址点集合
        /// </summary>
        public List<BuildLandBoundaryAddressDot> GetByLandId(Guid id)
        {
            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return null;
            var dots = (from dot in DataSource.CreateQuery<BuildLandBoundaryAddressDot>()
                        where dot.LandID == id
                        select dot).ToList();
            return dots;
        }

        /// <summary>
        /// 根据地域代码、土地权属类型删除集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="landType">土地权属类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eSearchOption searchType, string landType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (string.IsNullOrEmpty(zoneCode))
            {
                return 0;
            }
            int cnt = 0;
            if (searchType == eSearchOption.Fuzzy)
                cnt = Delete(c => c.ZoneCode.Contains(zoneCode) && c.LandType.Equals(landType));
            else
                cnt = Delete(c => c.ZoneCode.Equals(zoneCode) && c.LandType.Equals(landType));
            return cnt;
        }

        /// <summary>
        /// 根据地域代码删除集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (string.IsNullOrEmpty(zoneCode))
            {
                return 0;
            }
            int cnt = 0;
            if (searchType == eSearchOption.Fuzzy)
                cnt = Delete(c => c.ZoneCode.Contains(zoneCode));
            else
                cnt = Delete(c => c.ZoneCode.Equals(zoneCode));
            return cnt;
        }

        /// <summary>
        /// 根据地域代码删除指定承包方状态的界址点数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="virtualStatus">对应承包方状态</param>
        /// <returns>-1(参数错误)/0(失败)/1(成功))</returns>
        public int DeleteByZoneCode(string zoneCode, eLevelOption levelOption, eVirtualPersonStatus virtualStatus)
        {
            if (string.IsNullOrEmpty(zoneCode))
            {
                return -1;
            }
            int cnt = 0;
            int compare = 50;

            if (levelOption == eLevelOption.SelfAndSubs)
            {
                var lnSelfAndSubs = (from person in DataSource.CreateQuery<LandVirtualPerson>()
                                     join land in DataSource.CreateQuery<ContractLand>() on person.ID equals land.OwnerId
                                     where person.Status.Equals(virtualStatus) && person.ZoneCode.StartsWith(zoneCode)
                                     select new { LandId = land.ID }).ToList().Select(c => c.LandId).ToList();
                if (lnSelfAndSubs.Count == 0)
                    return 0;
                cnt = DeleteByVitualPersonStatue(compare, lnSelfAndSubs);
            }
            else if (levelOption == eLevelOption.Self)
            {
                var lnSelf = (from person in DataSource.CreateQuery<LandVirtualPerson>()
                              join land in DataSource.CreateQuery<ContractLand>() on person.ID equals land.OwnerId
                              where person.Status.Equals(virtualStatus) && person.ZoneCode.Equals(zoneCode)
                              select new { LandId = land.ID }).ToList().Select(c => c.LandId).ToList();
                if (lnSelf.Count == 0)
                    return 0;
                cnt = DeleteByVitualPersonStatue(compare, lnSelf);
            }
            else
            {
                var lnSubs = (from person in DataSource.CreateQuery<LandVirtualPerson>()
                              join land in DataSource.CreateQuery<ContractLand>() on person.ID equals land.OwnerId
                              where person.Status.Equals(virtualStatus) && person.ZoneCode.StartsWith(zoneCode) && person.ZoneCode != zoneCode
                              select new { LandId = land.ID }).ToList().Select(c => c.LandId).ToList();
                if (lnSubs.Count == 0)
                    return 0;
                cnt = DeleteByVitualPersonStatue(compare, lnSubs);
            }

            return cnt;
        }

        /// <summary>
        /// 根据地块ID删除界址点信息
        /// </summary>
        public int DeleteByLandIds(Guid[] ids)
        {
            if (ids == null || ids.Length == 0)
                return -1;

            return AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressDot>().Where(c => ids.Contains(c.LandID)).Delete());
        }

        /// <summary>
        /// 根据承包方状态删除数据
        /// </summary>
        /// <param name="compare">拼接Whrer语句最大个数(此数字过大会导致程序异常)</param>
        /// <param name="landNumber">查找的地块编码集合</param>
        private int DeleteByVitualPersonStatue(int compare, List<Guid> landId)
        {
            int cnt = 0;
            StringBuilder b = new StringBuilder();
            if (landId.Count > compare)
            {
                int count = landId.Count / compare;
                List<object> listObj = new List<object>();
                int index = 0;
                for (int i = 0; i < count; i++)
                {
                    int j = 0;
                    while (j < compare)
                    {
                        if (j == compare - 1)
                            b.AppendFormat("LandID == @{0}", j);
                        else
                            b.AppendFormat("LandID == @{0}" + " || ", j);
                        listObj.Add(landId[index]);
                        j++;
                        index++;
                    }
                    cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressDot>().Where(b.ToString(), listObj.ToArray()).Delete());
                    b.Clear();
                    listObj.Clear();
                }
                int other = landId.Count - (compare * count);
                if (other == 0)
                    return cnt;
                for (int i = 0; i < other; i++)
                {
                    listObj.Add(landId[index + i]);
                    if (i == other - 1)
                        b.AppendFormat("LandID == @{0}", i);
                    else
                        b.AppendFormat("LandID == @{0}" + " || ", i);
                }
                cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressDot>().Where(b.ToString(), listObj.ToArray()).Delete());
            }
            else
            {
                for (int i = 0; i < landId.Count; i++)
                {
                    if (i == landId.Count - 1)
                        b.AppendFormat("LandID == @{0}", i);
                    else
                        b.AppendFormat("LandID == @{0}" + " || ", i);
                }
                cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressDot>().Where(b.ToString(), landId.Cast<object>().ToArray()).Delete());
            }

            return cnt;
        }

        /// <summary>
        /// 根据承包地块删除界址点数据
        /// </summary>
        /// <param name="compare">拼接Whrer语句最大个数(此数字过大会导致程序异常)</param>
        /// <param name="landNumber">查找的地块编码集合</param>
        private int DeleteByContractLand(int compare, List<Guid> landId)
        {
            int cnt = 0;
            StringBuilder b = new StringBuilder();
            if (landId.Count > compare)
            {
                int count = landId.Count / compare;
                List<object> listObj = new List<object>();
                int index = 0;
                for (int i = 0; i < count; i++)
                {
                    int j = 0;
                    while (j < compare)
                    {
                        if (j == compare - 1)
                            b.AppendFormat("LandID != @{0}", j);
                        else
                            b.AppendFormat("LandID != @{0}" + " || ", j);
                        listObj.Add(landId[index]);
                        j++;
                        index++;
                    }
                    cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressDot>().Where(b.ToString(), listObj.ToArray()).Delete());
                    b.Clear();
                    listObj.Clear();
                }
                int other = landId.Count - (compare * count);
                if (other == 0)
                    return cnt;
                for (int i = 0; i < other; i++)
                {
                    listObj.Add(landId[index + i]);
                    if (i == other - 1)
                        b.AppendFormat("LandID != @{0}", i);
                    else
                        b.AppendFormat("LandID != @{0}" + " || ", i);
                }
                cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressDot>().Where(b.ToString(), listObj.ToArray()).Delete());
            }
            else
            {
                for (int i = 0; i < landId.Count; i++)
                {
                    if (i == landId.Count - 1)
                        b.AppendFormat("LandID != @{0}", i);
                    else
                        b.AppendFormat("LandID != @{0}" + " || ", i);
                }
                cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressDot>().Where(b.ToString(), landId.Cast<object>().ToArray()).Delete());
            }

            return cnt;
        }

        /// <summary>
        /// 根据地域代码删除指定承包方状态的界址点数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatus">对应承包方状态</param>
        /// <returns>-1(参数错误)/0(失败)/1(成功))</returns>
        public int DeleteByZoneCode(string zoneCode, eLevelOption levelOption)
        {
            if (string.IsNullOrEmpty(zoneCode))
            {
                return -1;
            }
            int cnt = 0;
            int compare = 50;

            if (levelOption == eLevelOption.SelfAndSubs)
            {
                int count = DataSource.CreateQuery<ContractLand>().Where(c => c.SenderCode.StartsWith(zoneCode)).Count();//.Select(c => c.ID)
                if (count == 0)
                    return Delete(c => c.ZoneCode.StartsWith(zoneCode));
                var idsSelf = DataSource.CreateQuery<ContractLand>().Where(c => c.SenderCode.StartsWith(zoneCode)).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByContractLand(compare, idsSelf);
            }
            else if (levelOption == eLevelOption.Self)
            {
                int count = DataSource.CreateQuery<ContractLand>().Where(c => c.SenderCode.Equals(zoneCode)).Count();//.Select(c => c.ID)
                if (count == 0)
                    return Delete(c => c.ZoneCode.Equals(zoneCode));
                var idsSelf = DataSource.CreateQuery<ContractLand>().Where(c => c.SenderCode.Equals(zoneCode)).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByContractLand(compare, idsSelf);
            }
            else
            {
                int count = DataSource.CreateQuery<ContractLand>().Where(c => c.SenderCode.StartsWith(zoneCode) && c.SenderCode != zoneCode).Count();//.Select(c => c.ID)
                if (count == 0)
                    return Delete(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode);
                var idsSelf = DataSource.CreateQuery<ContractLand>().Where(c => c.SenderCode.StartsWith(zoneCode) && c.SenderCode != zoneCode).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByContractLand(compare, idsSelf);
            }

            return cnt;
        }

        /// <summary>
        /// 按地域、土地权属类型统计集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int Count(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (string.IsNullOrEmpty(zoneCode))
            {
                return -1;
            }
            int cnt = 0;
            if (searchOption == eLevelOption.SelfAndSubs)
                cnt = Count(c => c.ZoneCode.StartsWith(zoneCode));
            else if (searchOption == eLevelOption.Self)
                cnt = Count(c => c.ZoneCode.Equals(zoneCode));
            else if (searchOption == eLevelOption.Subs)
                cnt = Count(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode);
            return cnt;
        }

        /// <summary>
        /// 按照SQL语句在数据访问层中插入批量的界址数据
        /// </summary>
        /// <param name="dots">待插入保存的点集</param>
        /// <param name="srid">点集的空间参考索引</param>
        public void SQLaddDotsIntoSqilite(List<BuildLandBoundaryAddressDot> dots, int srid)
        {
            try
            {
                var qc = DataSource.CreateQuery();
                for (int i = 0; i < dots.Count; i++)
                {
                    //string sql = string.Format(
                    //    "insert into [JZD] ([OBJECTID], [ID], [DWMC], [Shape]) values ({0}, '{1}', '{2}', {3})",
                    //    i, Guid.NewGuid(), "test", string.Format("GeomFromText('{0}', {1})", geo.AsText(), 0));


                    //string sql = string.Format(
                    //   "insert into[JZD] ([ID], [BSM], [TBJZDH], [JZDH], [JBLX], [JZDLX], [DKID], [CJZ], [CJSJ], [XGZ], [XGSJ], [MSXX], [DYBM], [JZDSSQLLX], [DKBM], [SFKY], [Shape]) values" +
                    //   "('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}','{10}', '{11}', '{12}', '{13}', '{14}', '{15}', {16})",
                    //   dots[i].ID, dots[i].DotCode, dots[i].UniteDotNumber, dots[i].DotNumber, dots[i].LandMarkType, dots[i].DotType, dots[i].LandID,
                    //   dots[i].Founder,string.Format("'{0}'", ((DateTime)dots[i].CreationTime).ToString("s")), dots[i].Modifier, string.Format("'{0}'", ((DateTime)dots[i].ModifiedTime).ToString("s")),dots[i].Description, dots[i].SenderCode,
                    //   dots[i].LandType, dots[i].LandNumber, dots[i].IsValid, string.Format("GeomFromText('{0}', {1})", dots[i].Shape.AsText(), srid));

                    string sql = string.Format(
                       "insert into[JZD] ([ID], [BSM], [TBJZDH], [JZDH], [JBLX], [JZDLX], [DKID], [CJZ], [CJSJ], [XGZ], [XGSJ], [MSXX], [DYBM], [JZDSSQLLX], [DKBM], [SFKY], [Shape]) values" +
                       "('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}','{10}', '{11}', '{12}', '{13}', '{14}', {15}, {16})",
                       dots[i].ID, dots[i].DotCode, dots[i].UniteDotNumber, dots[i].DotNumber, dots[i].LandMarkType, dots[i].DotType, dots[i].LandID,
                       dots[i].Founder, string.Format("{0}", ((DateTime)dots[i].CreationTime).ToString("s")), dots[i].Modifier, string.Format("{0}", ((DateTime)dots[i].ModifiedTime).ToString("s")), dots[i].Description, dots[i].ZoneCode,
                       dots[i].LandType, dots[i].LandNumber, string.Format("{0}",((bool)dots[i].IsValid)?1:0), string.Format("GeomFromText('{0}', {1})", dots[i].Shape.AsText(), srid));

                    qc.CommandContext.CommandText = new StringBuilder(sql);
                    qc.CommandContext.Type = eCommandType.Insert;
                    qc.CommandContext.ExecuteArgument = eDbExecuteType.Scalar;
                    qc.Save();
                }
            }
            catch (Exception addinfo)
            {
                var adderrorinfo = addinfo.Message;
            }
        }
        #endregion
    }
}