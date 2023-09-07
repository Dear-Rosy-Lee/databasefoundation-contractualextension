// (C) 2015 鱼鳞图公司版权所有，保留所有权利
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
    /// 农村土地承包经营权登记薄的数据访问类
    /// </summary>
    public class ContractRegeditBookRepository : RepositoryDbContext<ContractRegeditBook>, IContractRegeditBookRepository
    {
        #region Ctor

        private IDataSourceSchema m_DSSchema = null;

        public ContractRegeditBookRepository(IDataSource ds)
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, typeof(ContractRegeditBook).GetAttribute<DataTableAttribute>().TableName);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 删除农村土地承包经营权登记薄对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return -1;
            return Delete(c => c.ID.Equals(guid));
        }

        /// <summary>
        /// 删除农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="number">权证编号</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByRegeditNumber(string number)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number))
                return -1;
            return Delete(c => c.RegeditNumber.Equals(number));
        }


        /// <summary>
        /// 更新农村土地承包经营权登记薄对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(ContractRegeditBook entity)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (entity == null || !CheckRule.CheckGuidNullOrEmpty(entity.ID))
                return 0;
            entity.ModifiedTime = DateTime.Now;
            return Update(entity, c => c.ID.Equals(entity.ID));
        }

        /// <summary>
        /// 根据ID获取农村土地承包经营权登记薄对象
        /// </summary>
        public ContractRegeditBook Get(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return null;
            ContractRegeditBook book = Get(c => c.ID.Equals(guid)).FirstOrDefault();
            return book;
        }

        /// <summary>
        /// 根据合同id集合获取权证集合
        /// </summary>
        public List<ContractRegeditBook> GetByConcordIds(Guid[] ids)
        {
            if (ids == null || ids.Length == 0)
                return null;

            var data = DataSource.CreateQuery<ContractRegeditBook>().Where(c => ids.Contains(c.ID)).ToList();
            return data;
        }

        /// <summary>
        /// 根据ID判断农村土地承包经营权登记薄对象是否存在
        /// </summary>
        /// <param name="guid">ID</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return false;
            return Count(c => c.ID.Equals(guid)) > 0 ? true : false;
        }

        /// <summary>
        /// 通过登记薄编号查看是否存在有权证号相同但Guid不同的存在。
        /// </summary>
        /// <param name="concordNumber">登记薄编号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(string regeditNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref regeditNumber))
            {
                return false;
            }
            return Count(c => c.RegeditNumber.Equals(regeditNumber)) > 0 ? true : false;
        }

        /// <summary>
        /// 通过登记薄编号获取农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="regeditNumber">登记薄编号</param>
        public ContractRegeditBook Get(string regeditNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref regeditNumber))
            {
                return null;
            }
            ContractRegeditBook book = Get(c => c.RegeditNumber.Equals(regeditNumber)).FirstOrDefault();
            return book;
        }

        /// <summary>
        /// 根据权证流水号及其查找类型获取农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="number">权证流水号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包经营权登记薄对象</returns>
        public ContractRegeditBook GetByNumber(string number, eSearchOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number))
            {
                return null;
            }

            ContractRegeditBook entity = null;
            if (searchOption == eSearchOption.Fuzzy)
                entity = Get(c => c.SerialNumber.Contains(number)).FirstOrDefault();
            else
                entity = Get(c => c.SerialNumber.Equals(number)).FirstOrDefault();
            return entity;
        }

        /// <summary>
        /// 根据地域代码及其查找类型获取权证
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>权证</returns>
        public List<ContractRegeditBook> GetByZoneCode(string zoneCode, eSearchOption searchOption)
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
            List<ContractRegeditBook> entity = null;
            if (searchOption == eSearchOption.Fuzzy)
                entity = Get(c => c.ZoneCode.Contains(zoneCode));
            else
                entity = Get(c => c.ZoneCode.Equals(zoneCode));
            return entity;
        }

        /// <summary>
        /// 根据权证号获取及其查找类型获取权证
        /// </summary>
        /// <param name="number">权证号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>权证</returns>
        public List<ContractRegeditBook> GetCollection(string number, eSearchOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number))
            {
                return null;
            }

            List<ContractRegeditBook> entity = null;
            if (searchOption == eSearchOption.Precision)
                entity = Get(c => c.Number.Equals(number));
            else if (searchOption == eSearchOption.Fuzzy)
                entity = Get(c => c.Number.Contains(number));
            return entity;
        }

        /// <summary>
        /// 根据不同匹配等级获取该地域下的权证
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配级别</param>
        /// <returns>农村土地承包权证</returns>
        public List<ContractRegeditBook> GetContractsByZoneCode(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (zoneCode == null)
                return null;

            zoneCode = zoneCode.Trim();
            if (zoneCode == string.Empty)
                return null;

            object entity = 0;

            if (searchOption == eLevelOption.Self)
            {
                entity = Get(c => c.ZoneCode.Equals(zoneCode));
            }
            else if (searchOption == eLevelOption.Subs)
            {
                entity = Get(c => c.ZoneCode != zoneCode && c.ZoneCode.StartsWith(zoneCode));
            }
            else if (searchOption == eLevelOption.SelfAndSubs)
            {
                entity = Get(c => c.ZoneCode.StartsWith(zoneCode));
            }
            return entity as List<ContractRegeditBook>;
        }

        /// <summary>
        /// 根据地域代码及其查找类型删除权证
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eSearchOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return -1;
            }

            if (searchOption == eSearchOption.Precision)
                return Delete(c => c.ZoneCode.Equals(zoneCode));
            else if (searchOption == eSearchOption.Fuzzy)
                return Delete(c => c.ZoneCode.Contains(zoneCode));
            return 0;
        }

        /// <summary>
        /// 删除当前地域下所有指定承包方状态的权证
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatue">承包方状态</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
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
                                     join concord in DataSource.CreateQuery<ContractConcord>() on person.ID equals concord.ContracterId
                                     where person.Status.Equals(virtualStatus) && person.ZoneCode.StartsWith(zoneCode)
                                     select new { ConcordID = concord.ID }).ToList().Select(c => c.ConcordID).ToList();
                if (lnSelfAndSubs.Count == 0)
                    return 0;
                cnt = DeleteByVitualPersonStatue(compare, lnSelfAndSubs);
            }
            else if (levelOption == eLevelOption.Self)
            {
                var lnSelf = (from person in DataSource.CreateQuery<LandVirtualPerson>()
                              join concord in DataSource.CreateQuery<ContractConcord>() on person.ID equals concord.ContracterId
                              where person.Status.Equals(virtualStatus) && person.ZoneCode.Equals(zoneCode)
                              select new { ConcordID = concord.ID }).ToList().Select(c => c.ConcordID).ToList();
                if (lnSelf.Count == 0)
                    return 0;
                cnt = DeleteByVitualPersonStatue(compare, lnSelf);
            }
            else
            {
                var lnSubs = (from person in DataSource.CreateQuery<LandVirtualPerson>()
                              join concord in DataSource.CreateQuery<ContractConcord>() on person.ID equals concord.ContracterId
                              where person.Status.Equals(virtualStatus) && person.ZoneCode.StartsWith(zoneCode) && person.ZoneCode != zoneCode
                              select new { ConcordID = concord.ID }).ToList().Select(c => c.ConcordID).ToList();
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
        /// <param name="concordId">查找的合同标识集合</param>
        private int DeleteByVitualPersonStatue(int compare, List<Guid> concordId)
        {
            int cnt = 0;
            StringBuilder b = new StringBuilder();
            if (concordId.Count > compare)
            {
                int count = concordId.Count / compare;
                List<object> listObj = new List<object>();
                int index = 0;
                for (int i = 0; i < count; i++)
                {
                    int j = 0;
                    while (j < compare)
                    {
                        if (j == compare - 1)
                            b.AppendFormat("ID == @{0}", j);
                        else
                            b.AppendFormat("ID == @{0}" + " || ", j);
                        listObj.Add(concordId[index]);
                        j++;
                        index++;
                    }
                    cnt = AppendEdit(DataSource.CreateQuery<ContractRegeditBook>().Where(b.ToString(), listObj.ToArray()).Delete());
                    b.Clear();
                    listObj.Clear();
                }
                int other = concordId.Count - (compare * count);
                if (other == 0)
                    return cnt;
                for (int i = 0; i < other; i++)
                {
                    listObj.Add(concordId[index + i]);
                    if (i == other - 1)
                        b.AppendFormat("ID == @{0}", i);
                    else
                        b.AppendFormat("ID == @{0}" + " || ", i);
                }
                cnt = AppendEdit(DataSource.CreateQuery<ContractRegeditBook>().Where(b.ToString(), listObj.ToArray()).Delete());
            }
            else
            {
                for (int i = 0; i < concordId.Count; i++)
                {
                    if (i == concordId.Count - 1)
                        b.AppendFormat("ID == @{0}", i);
                    else
                        b.AppendFormat("ID == @{0}" + " || ", i);
                }
                cnt = AppendEdit(DataSource.CreateQuery<ContractRegeditBook>().Where(b.ToString(), concordId.Cast<object>().ToArray()).Delete());
            }

            return cnt;
        }

        /// <summary>
        /// 根据合同删除权证数据
        /// </summary>
        /// <param name="compare">拼接Whrer语句最大个数(此数字过大会导致程序异常)</param>
        /// <param name="concordId">查找的合同标识集合</param>
        private int DeleteByConcord(int compare, List<Guid> concordId)
        {
            int cnt = 0;
            StringBuilder b = new StringBuilder();
            if (concordId.Count > compare)
            {
                int count = concordId.Count / compare;
                List<object> listObj = new List<object>();
                int index = 0;
                for (int i = 0; i < count; i++)
                {
                    int j = 0;
                    while (j < compare)
                    {
                        if (j == compare - 1)
                            b.AppendFormat("ID != @{0}", j);
                        else
                            b.AppendFormat("ID != @{0}" + " || ", j);
                        listObj.Add(concordId[index]);
                        j++;
                        index++;
                    }
                    cnt = AppendEdit(DataSource.CreateQuery<ContractRegeditBook>().Where(b.ToString(), listObj.ToArray()).Delete());
                    b.Clear();
                    listObj.Clear();
                }
                int other = concordId.Count - (compare * count);
                if (other == 0)
                    return cnt;
                for (int i = 0; i < other; i++)
                {
                    listObj.Add(concordId[index + i]);
                    if (i == other - 1)
                        b.AppendFormat("ID != @{0}", i);
                    else
                        b.AppendFormat("ID != @{0}" + " || ", i);
                }
                cnt = AppendEdit(DataSource.CreateQuery<ContractRegeditBook>().Where(b.ToString(), listObj.ToArray()).Delete());
            }
            else
            {
                for (int i = 0; i < concordId.Count; i++)
                {
                    if (i == concordId.Count - 1)
                        b.AppendFormat("ID != @{0}", i);
                    else
                        b.AppendFormat("ID != @{0}" + " || ", i);
                }
                cnt = AppendEdit(DataSource.CreateQuery<ContractRegeditBook>().Where(b.ToString(), concordId.Cast<object>().ToArray()).Delete());
            }

            return cnt;
        }

        /// <summary>
        /// 删除当前地域下所有指定承包方状态的权证
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
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
                int count = DataSource.CreateQuery<ContractConcord>().Where(c => c.ZoneCode.StartsWith(zoneCode)).Count();//.Select(c => c.ID)
                if (count == 0)
                    return Delete(c => c.ZoneCode.StartsWith(zoneCode));
                var concord = DataSource.CreateQuery<ContractConcord>().Where(c => c.ZoneCode.StartsWith(zoneCode)).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByConcord(compare, concord);
            }
            else if (levelOption == eLevelOption.Self)
            {
                int count = DataSource.CreateQuery<ContractConcord>().Where(c => c.ZoneCode.Equals(zoneCode)).Count();//.Select(c => c.ID)
                if (count == 0)
                    return Delete(c => c.ZoneCode.Equals(zoneCode));
                var concord = DataSource.CreateQuery<ContractConcord>().Where(c => c.ZoneCode.Equals(zoneCode)).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByConcord(compare, concord);
            }
            else
            {
                int count = DataSource.CreateQuery<ContractConcord>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode).Count();//.Select(c => c.ID)
                if (count == 0)
                    return Delete(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode);
                var concord = DataSource.CreateQuery<ContractConcord>().Where(c => c.ZoneCode.Equals(zoneCode)).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByConcord(compare, concord);
            }

            return cnt;
        }

        /// <summary>
        /// 按地域及其匹配类型统计权证数量
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配类型</param>
        /// <returns>-1（参数错误）/int 权证数量</returns>
        public int Count(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }

            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return -1;
            }
            if (searchOption == eLevelOption.Self)
            {
                return Count(c => c.ZoneCode.Equals(zoneCode));
            }
            return Count(c => c.ZoneCode.StartsWith(zoneCode));
        }

        /// <summary>
        /// 存在权证数据的地域集合
        /// </summary>
        /// <param name="zoneCode">地域集合</param>  
        public List<Zone> ExistZones(List<Zone> zoneList)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (zoneList == null || zoneList.Count == 0)
            {
                return null;
            }
            var qc = DataSource.CreateQuery<ContractRegeditBook>();
            var q1 = (from book in qc
                      group book by book.ZoneCode into gp
                      select new { ZoneCode = gp.Key, Count = gp.Count() });
            var q2 = (from z in zoneList
                      join gp in q1 on z.FullCode equals gp.ZoneCode
                      where gp.Count > 0
                      select z).ToList();
            return q2;
        }

        /// <summary>
        /// 获取最新的权证流水号
        /// </summary>
        public string GetNewSerialNumber(int minNumber, int maxNumber)
        {
            var qcList = (from book in DataSource.CreateQuery<ContractRegeditBook>()
                          where book.SerialNumber != null && book.SerialNumber != ""
                          select new { Number = book.SerialNumber }).ToList();

            var bookList = qcList.OrderBy(c => Int32.Parse(c.Number)).ToList();
            for (int i = 0; i < bookList.Count; i++)
            {
                if (Convert.ToInt32(bookList[i].Number) < minNumber)
                    continue;
                if (bookList[i].Number.TrimStart('0') != minNumber.ToString())
                    break;
                minNumber++;
            }
            if (minNumber > maxNumber)
                return "";
            return minNumber.ToString();
        }

        /// <summary>
        /// 获取最大流水号
        /// </summary>
        /// <returns></returns>
        public int GetMaxSerialNumber()
        {
            var List = Get(c => c.SerialNumber != null && c.SerialNumber != "");
            if (List.Count <= 0)
                return 0;
            return List.Max(c => int.Parse(c.SerialNumber));
        }

        /// <summary>
        /// 在整库中获取最大流水号
        /// </summary>
        public int GetMaxSerialNumber1()
        {
            var qc = from book in DataSource.CreateQuery<ContractRegeditBook>()
                     select book.SerialNumber;

            var numbers = qc.ToList();
            if (numbers == null || numbers.Count == 0)
                return 0;
            return numbers.Max(c => Convert.ToInt32(c));
        }

        #endregion
    }
}