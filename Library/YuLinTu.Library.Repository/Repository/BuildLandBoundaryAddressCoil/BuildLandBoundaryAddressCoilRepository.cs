// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.Data;
using YuLinTu.Data;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 集体建设用地使用权界址线的数据访问类
    /// </summary>
    public class BuildLandBoundaryAddressCoilRepository : RepositoryDbContext<BuildLandBoundaryAddressCoil>, IBuildLandBoundaryAddressCoilRepository
    {
        #region Ctor

        private IDataSourceSchema m_DSSchema = null;

        public BuildLandBoundaryAddressCoilRepository(IDataSource ds)
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
            //    return  m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, typeof(BuildLandBoundaryAddressCoil).GetAttribute<DataTableAttribute>().TableName);
            //}
            //catch (Exception)
            //{
            return true;
            //}
        }

        /// <summary>
        /// 获取权证对象
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>对象</returns>
        public BuildLandBoundaryAddressCoil Get(Guid id)
        {
            //if (!CheckTableExist())
            //{
            //    throw new ArgumentNullException("数据库不存在表："
            //        + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            //}
            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return null;

            object data = Get(c => c.ID == id);
            return data as BuildLandBoundaryAddressCoil;
        }

        /// <summary>
        /// 检查是否存在指定id的权证对象
        /// </summary>
        /// <param name="id">id</param>
        public bool Exist(Guid id)
        {
            //if (!CheckTableExist())
            //{
            //    throw new ArgumentNullException("数据库不存在表："
            //         + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            //}
            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return false;

            return Count(c => c.ID == id) > 0;
        }

        /// <summary>
        /// 通过集体建设用地使用权ID获取权证对象集合
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>对象集合</returns>
        public List<BuildLandBoundaryAddressCoil> GetByLandID(Guid landID)
        {
            //if (!CheckTableExist())
            //{
            //    throw new ArgumentNullException("数据库不存在表："
            //        + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            //}
            if (!CheckRule.CheckGuidNullOrEmpty(landID))
                return null;
            return Get(c => c.LandID == landID);
        }

        /// <summary>
        /// 通过集体建设用地使用权ID获取权证对象数量
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>-1(参数错误)/int(数量))</returns>
        public int CountByLandID(Guid landID)
        {
            //if (!CheckTableExist())
            //{
            //    throw new ArgumentNullException("数据库不存在表："
            //        + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            //}
            if (!CheckRule.CheckGuidNullOrEmpty(landID))
                return -1;
            return Count(c => c.LandID == landID);
        }

        /// <summary>
        /// 更新权证
        /// </summary>
        /// <param name="coil">新的值</param>
        /// <returns>-1(错误)/0(失败)/1(成功)</returns>
        public int Update(BuildLandBoundaryAddressCoil coil)
        {
            //if (!CheckTableExist())
            //{
            //    throw new ArgumentNullException("数据库不存在表："
            //        + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            //}
            if (coil == null || !CheckRule.CheckGuidNullOrEmpty(coil.ID))
                return -1;

            int cnt = 0;
            cnt = Update(coil, c => c.ID == coil.ID);
            return cnt;
        }

        /// <summary>
        /// 删除权证对象
        /// </summary>
        /// <param name="ID">权证对象ID</param>
        /// <returns>-1(错误)/0(失败)/1(成功)</returns>
        public int Delete(Guid ID)
        {
            //if (!CheckTableExist())
            //{
            //    throw new ArgumentNullException("数据库不存在表："
            //        + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            //}
            if (!CheckRule.CheckGuidNullOrEmpty(ID))
                return -1;
            int cnt = 0;
            cnt = Delete(c => c.ID == ID);
            return cnt;
        }

        /// <summary>
        /// 通过属性ID删除权证对象
        /// </summary>
        /// <param name="ID">对象ID</param>
        /// <returns>-1(错误)/0(失败)/1(成功)</returns>
        public int DeleteByProperty(Guid propertyID)
        {
            //if (!CheckTableExist())
            //{
            //    throw new ArgumentNullException("数据库不存在表："
            //        + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            //}
            if (!CheckRule.CheckGuidNullOrEmpty(propertyID))
                return -1;
            int cnt = 0;
            cnt = Delete(c => c.LandID == propertyID);
            return cnt;
        }

        /// <summary>
        /// 根据地域代码获取权证
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="landType">土地权属类型</param>
        /// <returns>对象集合</returns>
        public List<BuildLandBoundaryAddressCoil> GetByZoneCode(string zoneCode, eSearchOption searchOption, eLandPropertyRightType landType)
        {
            //if (!CheckTableExist())
            //{
            //    throw new ArgumentNullException("数据库不存在表："
            //         + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            //}
            if (string.IsNullOrEmpty(zoneCode))
            {
                return null;
            }

            if (searchOption == eSearchOption.Fuzzy)
                return Get(c => c.ZoneCode.Contains(zoneCode) && c.LandType.Equals(landType));
            else
                return Get(c => c.ZoneCode.Equals(zoneCode) && c.LandType.Equals(landType));
        }

        /// <summary>
        /// 根据地域获取数据
        /// </summary>
        public List<BuildLandBoundaryAddressCoil> GetByZoneCode(string zoneCode, eLevelOption levelOption = eLevelOption.Self)
        {
            if (string.IsNullOrEmpty(zoneCode))
                return null;
            List<BuildLandBoundaryAddressCoil> entity = Get(c => c.ZoneCode.Equals(zoneCode));
            if (levelOption == eLevelOption.SelfAndSubs)
                entity = Get(c => c.ZoneCode.StartsWith(zoneCode));
            else if (levelOption == eLevelOption.Subs)
                entity = Get(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode);
            return entity;
        }

        /// <summary>
        /// 根据地块标识获取界址线集合
        /// </summary>
        public List<BuildLandBoundaryAddressCoil> GetByLandId(Guid id)
        {
            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return null;
            var coils = (from coil in DataSource.CreateQuery<BuildLandBoundaryAddressCoil>()
                         where coil.LandID == id
                         select coil).ToList();
            return coils;
        }

        /// <summary>
        /// 根据地域代码删除数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="landType">土地权属类型</param>
        /// <returns>-1(参数错误)/0(失败)/1(成功))</returns>
        public int DeleteByZoneCode(string zoneCode, eSearchOption searchType, string landType)
        {
            if (string.IsNullOrEmpty(zoneCode))
            {
                return -1;
            }
            int cnt = 0;

            if (searchType == eSearchOption.Fuzzy)
                cnt = Delete(c => c.ZoneCode.Contains(zoneCode) && c.LandType.Equals(landType));
            else
                cnt = Delete(c => c.ZoneCode.Equals(zoneCode) && c.LandType.Equals(landType));
            return cnt;
        }

        /// <summary>
        /// 根据地域代码删除集体建设用地使用权界址线对象
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eSearchOption searchType)
        {
            //if (!CheckTableExist())
            //{
            //    throw new ArgumentNullException("数据库不存在表："
            //        + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            //}
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
        /// 根据地块ID删除界址线信息
        /// </summary>
        public int DeleteByLandIds(Guid[] ids)
        {
            if (ids == null || ids.Length == 0)
                return -1;

            return AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(t => ids.Contains(t.LandID)).Delete());
        }

        /// <summary>
        /// 根据地域代码删除指定承包方状态的界址线数据
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
                    cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(b.ToString(), listObj.ToArray()).Delete());
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
                cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(b.ToString(), listObj.ToArray()).Delete());
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
                cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(b.ToString(), landId.Cast<object>().ToArray()).Delete());
            }

            return cnt;
        }

        /// <summary>
        /// 根据承包地块删除界址线数据
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
                    cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(b.ToString(), listObj.ToArray()).Delete());
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
                cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(b.ToString(), listObj.ToArray()).Delete());
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
                cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(b.ToString(), landId.Cast<object>().ToArray()).Delete());
            }

            return cnt;
        }

        /// <summary>
        /// 根据地域代码删除指定承包方状态的界址线数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
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
        /// 按地域统计
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <param name="landType">土地权属类型</param>
        /// <returns>-1(参数错误)/int(数量))</returns>
        public int Count(string zoneCode, eLevelOption searchOption)
        {
            //if (!CheckTableExist())
            //{
            //    throw new ArgumentNullException("数据库不存在表："
            //            + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            //}
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
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
        /// 批量添加界址点线
        /// </summary>
        /// <param name="coils"></param>
        /// <param name="dots"></param>
        /// <returns></returns>
        public int AddDotCoilList(List<BuildLandBoundaryAddressCoil> coils, List<BuildLandBoundaryAddressDot> dots)
        {
            var qc1 = DataSource.CreateQuery<BuildLandBoundaryAddressCoil>();
            AppendEdit(qc1.AddRange(coils.ToArray()));

            var qc2 = DataSource.CreateQuery<BuildLandBoundaryAddressDot>();
            AppendEdit(qc2.AddRange(dots.ToArray()));

            return coils.Count + dots.Count;
        }

        /// <summary>
        /// 按照SQL语句在数据访问层中插入批量的界址数据
        /// </summary>
        /// <param name="coils">待插入保存的线集</param>
        /// <param name="srid">点集的空间参考索引</param>
        public void SQLaddCoilsIntoSqilite(List<BuildLandBoundaryAddressCoil> coils, int srid)
        {
            try
            {
                var qc = DataSource.CreateQuery();
                for (int i = 0; i < coils.Count; i++)
                {
                    //string sql = string.Format(
                    //    "insert into [JZD] ([OBJECTID], [ID], [DWMC], [Shape]) values ({0}, '{1}', '{2}', {3})",
                    //    i, Guid.NewGuid(), "test", string.Format("GeomFromText('{0}', {1})", geo.AsText(), 0));

                    string sql = string.Format(
                       "INSERT INTO [JZX] ([ID], [JZXCD], [JXXZ], [JZXLB], [JZXWZ], [XYSBH], [XYS], [ZYYYSBH], [ZYYYS], [JZXSXH], [JZXQD], [JZXZD], [DKBS], [CJZ], [CJSJ], [ZHXGZ], [ZHXGSJ], [JZXSM], [PLDWZJR], [PLDWQLR], [DYDM], [TDQSLX], [BZXX], [DKBM], [JZXQDH], [JZXZDH], [Shape]) VALUES " +
                       "('{0}', {1}, '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9},'{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}','{22}', '{23}', '{24}', '{25}', {26})",
                       coils[i].ID, coils[i].CoilLength, coils[i].LineType, coils[i].CoilType, coils[i].Position, coils[i].AgreementNumber, coils[i].AgreementBook, coils[i].ControversyNumber, coils[i].ControversyBook,
                       coils[i].OrderID, coils[i].StartPointID, coils[i].EndPointID, coils[i].LandID, coils[i].Founder, string.Format("{0}", ((DateTime)coils[i].CreationTime).ToString("s")), coils[i].Modifier, string.Format("{0}", ((DateTime)coils[i].ModifiedTime).ToString("s")),
                       coils[i].Description, coils[i].NeighborFefer, coils[i].NeighborPerson, coils[i].ZoneCode, coils[i].LandType, coils[i].Comment, coils[i].LandNumber,
                       coils[i].StartNumber, coils[i].EndNumber, string.Format("GeomFromText('{0}', {1})", coils[i].Shape.AsText(), srid));

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

        /// <summary>
        /// 批量修改界址线类别
        /// </summary>
        public int UpdateCoilCollectionType(List<Guid> coilids, string coilType)
        {
            int cnt = 0;
            try
            {
                cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(s=>coilids.ToArray().Contains(s.ID)).Update(c => new BuildLandBoundaryAddressCoil() { CoilType = coilType }));
            }
            catch (Exception ex)
            {
                throw ex;
            } 
            return cnt;
        }

        /// <summary>
        /// 07/21——电话沟通，专为鹿邑定制，只更新界址线的界址线说明
        /// </summary>
        /// <param name="coil"></param>
        /// <returns></returns>
        public int UpdateLuYi(BuildLandBoundaryAddressCoil coil, bool isInitQlrAndJzxWZ = false)
        {
            if (coil == null || !CheckRule.CheckGuidNullOrEmpty(coil.ID))
                return -1;

            int cnt = 0;
            if (!isInitQlrAndJzxWZ)
            {
                cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(c => c.ID == coil.ID).
                    Update(s => new BuildLandBoundaryAddressCoil()
                    {
                        Description = coil.Description,
                    }));
            }
            else
            {
                // 2017/07/26——更新权利人和界址线位置时，用生成方提供的界址线类别提取算法
                cnt = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(c => c.ID == coil.ID).
                 Update(s => new BuildLandBoundaryAddressCoil()
                 {
                     CoilType = coil.CoilType,
                     Position = coil.Position,
                 }));
            }
            return cnt;
        }
        #endregion
    }
}