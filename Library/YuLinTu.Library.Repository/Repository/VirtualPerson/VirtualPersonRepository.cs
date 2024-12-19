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
    /// 用于承包方的数据访问类
    /// </summary>
    public class VirtualPersonRepository<T> : RepositoryDbContext<T>, IVirtualPersonRepository<T> where T : VirtualPerson
    {
        #region Ctor

        private IDataSourceSchema m_DSSchema = null;

        public VirtualPersonRepository(IDataSource ds)
            : base(ds)
        {
            m_DSSchema = ds.CreateSchema();
        }

        #endregion Ctor

        #region Fields

        private const string ORDERFIELD_DEFAULT = "Name";

        #endregion Fields

        #region Methods

        /// <summary>
        /// 检查表是否存在
        /// </summary>
        /// <returns></returns>
        private bool CheckTableExist()
        {
            //try
            //{
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, typeof(T).GetAttribute<DataTableAttribute>().TableName);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 根据id获取承包方对象
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>承包方对象</returns>
        public VirtualPerson Get(Guid id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return null;

            var data = Get(c => c.ID.Equals(id)).FirstOrDefault();
            return data as VirtualPerson;
        }

        /// <summary>
        /// 根据“承包方”名称从指定的地域中获取“承包方”的信息。
        /// </summary>
        /// <param name="name">“承包方”名称</param>
        /// <param name="code">地域代码</param>
        /// <returns>“承包方”的信息</returns>
        public VirtualPerson Get(string name, string code)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name) || !CheckRule.CheckStringNullOrEmpty(ref code))
                return null;

            var data = Get(c => c.Name.Equals(name) && c.ZoneCode.Equals(code)).FirstOrDefault();
            return data as VirtualPerson;
        }

        public VirtualPerson GetByHH(string hh, string code)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref hh) || !CheckRule.CheckStringNullOrEmpty(ref code))
                return null;

            var data = Get(c => c.FamilyNumber.Equals(hh) && c.ZoneCode.Equals(code)).FirstOrDefault();
            return data as VirtualPerson;
        }

        /// <summary>
        /// 根据“承包方”编号从指定的地域中获取“承包方”的信息
        /// </summary>
        /// <param name="number">“承包方”编号</param>
        /// <param name="code">地域代码</param>
        /// <returns>“承包方”的信息</returns>
        public VirtualPerson GetByNumber(string number, string code)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number) || !CheckRule.CheckStringNullOrEmpty(ref code))
                return null;

            var data = Get(c => c.Number.Equals(number) && c.ZoneCode.Equals(code)).FirstOrDefault();
            return data as VirtualPerson;
        }

        /// <summary>
        /// 根据“承包方”户号从指定的地域中获取“承包方”的信息
        /// </summary>
        /// <param name="familyNumber">“承包方”户号</param>
        /// <param name="code">地域代码</param>
        /// <returns>“承包方”的信息</returns>
        public VirtualPerson GetFamilyNumber(string familyNumber, string code)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref familyNumber) || !CheckRule.CheckStringNullOrEmpty(ref code))
                return null;

            var data = Get(c => c.FamilyNumber.Equals(familyNumber) && c.ZoneCode.Equals(code)).FirstOrDefault();
            return data as VirtualPerson;
        }

        /// <summary>
        /// 根据“承包方”名称、户号从指定的地域中获取“承包方”的信息
        /// </summary>
        /// <param name="name">“承包方”名称</param>
        /// <param name="number">“承包方”户号</param>
        /// <param name="code">地域代码</param>
        /// <returns>“承包方”的信息</returns>
        public VirtualPerson Get(string name, string number, string code)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name) || !CheckRule.CheckStringNullOrEmpty(ref code))
                return null;

            var data = Get(c => c.Name.Equals(name) && c.Number.Equals(number) && c.ZoneCode.Equals(code)).FirstOrDefault();
            return data as VirtualPerson;
        }

        /// <summary>
        /// 根据“承包方”名称及其类型从指定的地域中获取“承包方”的信息。
        /// </summary>
        /// <param name="name">“承包方”的名称。</param>
        /// <param name="zoneCode">地域编码。</param>
        /// <param name="virtualPersonType">“承包方”的类型。</param>
        /// <returns>返回 <see cref="VirtualPerson"/> 类的实例，如果未找到相应的“承包方”则返回 <c>null</c>。</returns>
        public VirtualPerson Get(string name, string zoneCode, eVirtualPersonType virtualPersonType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name) || !CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;

            var data = Get(c => c.Name.Equals(name) && c.ZoneCode.Equals(zoneCode) && c.VirtualType.Equals(virtualPersonType)).FirstOrDefault();
            return data as VirtualPerson;
        }

        /// <summary>
        /// 根据“承包方”名称从指定的地域中获取“承包方”的信息
        /// </summary>
        /// <param name="name">“承包方”名称</param>
        /// <param name="code">地域编码</param>
        /// <returns>“承包方”的信息</returns>
        public int Count(string name, string code)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name) || !CheckRule.CheckStringNullOrEmpty(ref code))
                return -1;

            return Count(c => c.Name.Equals(name) && c.ZoneCode.Equals(code));
        }

        /// <summary>
        /// 统计指定的地域中“承包方”的数量,包括子级地域
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int CountByZone(string code)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref code))
                return -1;
            return Count(c => c.ZoneCode.StartsWith(code));
        }

        /// <summary>
        /// 获取指定的地域中以名称排序的“承包方”的集合
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <returns>“承包方”的集合</returns>
        public List<VirtualPerson> GetByZoneCode(string code)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref code))
                return null;
            var q = from qa in DataSource.CreateQuery<T>()
                    where qa.ZoneCode.Equals(code)
                    orderby qa.Name
                    select qa;
            List<T> data = q.ToList();
            List<VirtualPerson> vps = new List<VirtualPerson>();
            if (data == null || data.Count == 0)
                return vps;
            foreach (var item in data)
            {
                vps.Add(item);
            }
            return vps;
        }

        /// <summary>
        /// 获取指定的地域中以名称排序的“承包方”的集合
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="levelOption">匹配级别</param>
        /// <returns>“承包方”的集合</returns>
        public List<VirtualPerson> GetByZoneCode(string code, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref code))
                return null;

            List<VirtualPerson> list = new List<VirtualPerson>();
            List<T> data = new List<T>();
            //2011年1月18日 21:12:49    Roc 修改数据获取方法，增加在查询时的数据排序
            if (levelOption == eLevelOption.Self)
                data = (from q in DataSource.CreateQuery<T>()
                        where q.ZoneCode.Equals(code)
                        orderby q.Name
                        select q).ToList();
            else if (levelOption == eLevelOption.Subs)
                data = (from q in DataSource.CreateQuery<T>()
                        where q.ZoneCode.StartsWith(code) && q.ZoneCode != code
                        orderby q.Name
                        select q).ToList();
            else if (levelOption == eLevelOption.SelfAndSubs)
                data = (from q in DataSource.CreateQuery<T>()
                        where q.ZoneCode.StartsWith(code)
                        orderby q.Name
                        select q).ToList();
            if (data != null && data.Count > 0)
            {
                data.ForEach(t => list.Add(t));
            }
            return list;
        }

        /// <summary>
        /// 根据承包方状态获取指定的地域中以名称排序的“承包方”的集合
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="status">承包方状态</param>
        /// <param name="levelOption">地域编码的匹配级别</param>
        /// <returns>以名称排序的“承包方”的集合</returns>
        public List<VirtualPerson> GetByZoneCode(string code, eVirtualPersonStatus status, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref code))
                return null;

            List<VirtualPerson> list = new List<VirtualPerson>();
            List<T> data = new List<T>();
            //2011年1月18日 21:12:49    Roc 修改数据获取方法，增加在查询时的数据排序
            if (levelOption == eLevelOption.Self)
                data = (from q in DataSource.CreateQuery<T>()
                        where q.ZoneCode.Equals(code) && q.Status.Equals(status)
                        orderby q.Name
                        select q).ToList();
            else if (levelOption == eLevelOption.Subs)
                data = (from q in DataSource.CreateQuery<T>()
                        where q.ZoneCode.StartsWith(code) && q.ZoneCode != code && q.Status.Equals(status)
                        orderby q.Name
                        select q).ToList();
            else if (levelOption == eLevelOption.SelfAndSubs)
                data = (from q in DataSource.CreateQuery<T>()
                        where q.ZoneCode.StartsWith(code) && q.Status.Equals(status)
                        orderby q.Name
                        select q).ToList();
            if (data != null && data.Count > 0)
            {
                data.ForEach(t => list.Add(t));
            }
            return list;
        }

        /// <summary>
        /// 根据承包方类型获取指定的地域中承包方集合
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="virtualType">承包方类型</param>
        /// <returns></returns>
        public List<VirtualPerson> GetCollection(string code, eVirtualPersonType virtualType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (string.IsNullOrEmpty(code))
                return null;
            List<VirtualPerson> list = new List<VirtualPerson>();
            List<T> data = new List<T>();

            //2011年1月18日 21:14:15    Roc 修改数据获取方法，增加在查询时的数据排序
            data = (from q in DataSource.CreateQuery<T>()
                    where q.ZoneCode.Equals(code) && q.VirtualType.Equals(virtualType)
                    orderby q.Name
                    select q).ToList();
            if (data != null && data.Count > 0)
            {
                data.ForEach(t => list.Add(t));
            }
            return list;
        }

        /// <summary>
        ///  根据承包方类型获取指定的地域中承包方集合
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="virtualType">承包方类型</param>
        /// <param name="levelOption">地域编码的匹配级别</param>
        /// <returns>“承包方”的集合</returns>
        public List<VirtualPerson> GetCollection(string code, eVirtualPersonType virtualType, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (string.IsNullOrEmpty(code))
                return null;

            object data = null;

            if (levelOption == eLevelOption.Subs)
            {
                data = (from q in DataSource.CreateQuery<T>()
                        where q.ZoneCode.StartsWith(code) && q.ZoneCode != code && q.VirtualType.Equals(virtualType)
                        orderby q.Name
                        select q).ToList();
            }
            else if (levelOption == eLevelOption.SelfAndSubs)
            {
                data = (from q in DataSource.CreateQuery<T>()
                        where q.ZoneCode.StartsWith(code) && q.VirtualType.Equals(virtualType)
                        orderby q.Name
                        select q).ToList();
            }
            else
            {
                data = (from q in DataSource.CreateQuery<T>()
                        where q.ZoneCode.Equals(code) && q.VirtualType.Equals(virtualType)
                        orderby q.Name
                        select q).ToList();
            }
            return (List<VirtualPerson>)data;
        }

        /// <summary>
        /// 根据承包方的名称获取指定的地域中承包方集合
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="Name">承包方的名称</param>
        /// <returns>“承包方”的集合</returns>
        public List<VirtualPerson> GetCollection(string code, string Name)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            var data = Get(c => c.ZoneCode.Equals(code) && c.Name.Equals(Name));
            List<VirtualPerson> vps = new List<VirtualPerson>();
            if (data != null && data.Count > 0)
            {
                data.ForEach(t => vps.Add(t));
            }
            return vps;
        }

        /// <summary>
        /// 更新承包方对象
        /// </summary>
        /// <param name="virtualPerson">承包方对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public void UpdateListZoneCode(List<LandVirtualPerson> virtualPersons)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (virtualPersons == null)
                return;
            var q = DataSource.CreateQuery<LandVirtualPerson>();
            foreach (var vp in virtualPersons)
            {
                AppendEdit(q.Where(c => c.ID.Equals(vp.ID)).Update(
                      c => new LandVirtualPerson
                      {
                          OldVirtualCode = vp.OldVirtualCode,
                          ZoneCode = vp.ZoneCode,
                          ModifiedTime = DateTime.Now
                      }));
            }
        }

        /// <summary>
        /// 更新承包方对象
        /// </summary>
        /// <param name="virtualPerson">承包方对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(VirtualPerson virtualPerson, bool onlycode = false)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (virtualPerson == null || !CheckRule.CheckGuidNullOrEmpty(virtualPerson.ID))
                return -1;
            virtualPerson.ModifiedTime = DateTime.Now;
            if (!onlycode)
            {
                return Update(virtualPerson, c => c.ID.Equals(virtualPerson.ID));
            }
            else
            {
                return AppendEdit(DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ID.Equals(virtualPerson.ID)).Update(
                     c => new LandVirtualPerson
                     {
                         OldVirtualCode = virtualPerson.OldVirtualCode,
                         ZoneCode = virtualPerson.ZoneCode,
                         ModifiedTime = DateTime.Now
                     }));
            }
        }
        public int UpdateZoneCode(VirtualPerson virtualPerson)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (virtualPerson == null || !CheckRule.CheckGuidNullOrEmpty(virtualPerson.ID))
                return -1;
            virtualPerson.ModifiedTime = DateTime.Now;
            int cnt = 0;
            cnt = AppendEdit(DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ID == virtualPerson.ID).
                Update(s => new LandVirtualPerson()
                {
                    ZoneCode = virtualPerson.ZoneCode,
                    OldVirtualCode = virtualPerson.OldVirtualCode
                }));
            return cnt;
        }
        /// <summary>
        /// 根据id删除承包方信息
        /// </summary>
        /// <param name="ID">id</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid ID)
        {
            //修改人：陈泽林 20160830 删除承包方时将下面的地块，合同，权证等信息全部删除掉
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(ID))
                return -1;

            int cnt = 0;
            var book = DataSource.CreateQuery<ContractConcord>();
            var concord = (from i in book where i.ContracterId.Equals(ID) select i).Select(c => c.ID).ToArray();
            Delete<ContractRegeditBook>(c => concord.Contains(c.ID));
            cnt = Delete<ContractConcord>(c => c.ContracterId.Equals(ID));
            var land = DataSource.CreateQuery<ContractLand>();
            var ld = (from i in land where i.OwnerId.Equals(ID) select i).Select(c => c.ID).ToArray();
            Delete<BuildLandBoundaryAddressDot>(c => ld.Contains(c.LandID));
            cnt = Delete<ContractLand>(c => c.OwnerId.Equals(ID));

            cnt = Delete(c => c.ID.Equals(ID));
            return cnt;
        }

        /// <summary>
        /// 根据承包方身份证号删除承包方信息
        /// </summary>
        /// <param name="ID">id</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(string cardId, bool isDeleteLands = false)
        {
            if (cardId.IsNullOrEmpty())
            {
                return -1;
            }
            VirtualPerson vp = Get(s => s.Number == cardId).FirstOrDefault();
            if (vp == null)
            {
                return -1;
            }

            var id = vp.ID;
            int cnt = 0;
            var book = DataSource.CreateQuery<ContractConcord>();
            var concord = (from i in book where i.ContracterId.Equals(id) select i).Select(c => c.ID).ToArray();
            Delete<ContractRegeditBook>(c => concord.Contains(c.ID));
            cnt = Delete<ContractConcord>(c => c.ContracterId.Equals(id));
            if (isDeleteLands)
            {
                var land = DataSource.CreateQuery<ContractLand>();
                var ld = (from i in land where i.OwnerId.Equals(id) select i).Select(c => c.ID).ToArray();
                Delete<BuildLandBoundaryAddressDot>(c => ld.Contains(c.LandID));
                cnt = Delete<ContractLand>(c => c.OwnerId.Equals(id));
            }

            cnt = Delete(c => c.ID.Equals(id));
            return cnt;
        }

        /// <summary>
        /// 删除指定地域下的所有承包方对象
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string code)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref code))
                return -1;

            int cnt = 0;
            cnt = Delete(c => c.ZoneCode.Equals(code));
            return cnt;
        }

        /// <summary>
        /// 删除指定地域下指定的承包方类型的所有承包方对象
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="virtualType">承包方类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string code, eVirtualPersonType virtualType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref code))
                return -1;

            int cnt = 0;
            cnt = Delete(c => c.ZoneCode.Equals(code) && c.VirtualType.Equals(virtualType));
            return cnt;
        }

        /// <summary>
        /// 删除指定地域下的所有承包方对象
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="levelOption">地域编码匹配级别</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string code, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref code))
                return -1;

            int cnt = 0;
            if (levelOption == eLevelOption.SelfAndSubs)
                cnt = Delete(c => c.ZoneCode.StartsWith(code));
            else if (levelOption == eLevelOption.Self)
                cnt = Delete(c => c.ZoneCode.Equals(code));
            else
                cnt = Delete(c => c.ZoneCode.StartsWith(code) && c.ZoneCode != code);
            return cnt;
        }

        /// <summary>
        /// 删除指定地域下指定的承包方类型的所有承包方对象
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="virtualType">承包方类型</param>
        /// <param name="levelOption">地域编码匹配级别</param>
        /// <returns></returns>
        public int DeleteByZoneCode(string code, eVirtualPersonType virtualType, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref code))
                return -1;

            int cnt = 0;
            if (levelOption == eLevelOption.SelfAndSubs)
                cnt = Delete(c => c.ZoneCode.StartsWith(code) && c.VirtualType.Equals(virtualType));
            else if (levelOption == eLevelOption.Self)
                cnt = Delete(c => c.ZoneCode.Equals(code) && c.VirtualType.Equals(virtualType));
            else
                cnt = Delete(c => c.ZoneCode.StartsWith(code) && c.ZoneCode != code && c.VirtualType.Equals(virtualType));
            return cnt;
        }

        /// <summary>
        /// 删除指定地域下指定的承包方状态的所有承包方对象
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="virtualType">承包方状态</param>
        /// <param name="levelOption">地域编码匹配级别</param>
        public int DeleteByZoneCode(string code, eVirtualPersonStatus virtualStatus, eLevelOption levelOption)
        {
            if (!CheckRule.CheckStringNullOrEmpty(ref code))
                return -1;

            int cnt = 0;
            if (levelOption == eLevelOption.SelfAndSubs)
                cnt = Delete(c => c.ZoneCode.StartsWith(code) && c.Status.Equals(virtualStatus));
            else if (levelOption == eLevelOption.Self)
                cnt = Delete(c => c.ZoneCode.Equals(code) && c.Status.Equals(virtualStatus));
            else
                cnt = Delete(c => c.ZoneCode.StartsWith(code) && c.ZoneCode != code && c.Status.Equals(virtualStatus));
            return cnt;
        }

        /// <summary>
        /// 根据地域代码与承包方名称判断是否存在承包方对象
        /// </summary>
        /// <param name="code">地域代码</param>
        /// <param name="Name">承包方名称</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool ExistsByZoneCodeAndName(string code, string Name)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref code) || !CheckRule.CheckStringNullOrEmpty(ref Name))
                return false;
            int count = Count(c => c.ZoneCode.Equals(code) && c.Name.Equals(Name));
            return count > 0 ? true : false;
        }

        /// <summary>
        /// 根据地域代码与承包方名称判断是否存在状态处于锁定的承包方对象
        /// </summary>
        /// <param name="code">地域代码</param>
        /// <param name="Name">承包方名称</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool ExistsLockByZoneCodeAndName(string code, string Name)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref code) || !CheckRule.CheckStringNullOrEmpty(ref Name))
                return false;
            int count = Count(c => c.ZoneCode.Equals(code) && c.Name.Equals(Name) && c.Status.Equals(eVirtualPersonStatus.Lock));

            return count > 0 ? true : false;
        }

        /// <summary>
        /// 判断指定地域下的“承包方”是否已经初始化。
        /// </summary>
        /// <param name="zoneCode">地域编码。</param>
        /// <returns>当指定的地域下包含有“承包方”数据时则返回 <c>true</c> 以表示已经初始化，否则返回 <c>false</c>。</returns>
        /// <history>
        ///     2011年1月17日 16:55:20  Roc 创建
        /// </history>
        public bool HasInitialized(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            int count = Count(c => c.ZoneCode.Equals(zoneCode));
            return count > 0;
        }

        /// <summary>
        /// 存在承包方数据的地域集合
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
            var qc = DataSource.CreateQuery<T>();
            var q1 = (from person in qc
                      group person by person.ZoneCode into gp
                      select new { ZoneCode = gp.Key, Count = gp.Count() });
            var q2 = (from z in zoneList
                      join gp in q1 on z.FullCode equals gp.ZoneCode
                      where gp.Count > 0
                      select z).ToList();
            return q2;
        }

        public List<LandVirtualPerson> GetVirtualPersonsByLand(Guid landId)
        {
            var qRelation = DataSource.CreateQuery<BelongRelation>();
            var qPerson = DataSource.CreateQuery<LandVirtualPerson>();
            var qLand = DataSource.CreateQuery<ContractLand>();
            var q = from contractLand in qLand
                    join relation in qRelation on contractLand.ID equals relation.LandID
                    join person in qPerson on relation.VirtualPersonID equals person.ID
                    where contractLand.ID.Equals(landId)
                    select person;
            return q.ToList();
        }

        public int AddBelongRelation(BelongRelation belongRelation)
        {
            var qRelation = DataSource.CreateQuery<BelongRelation>();
            var q = qRelation.Add(belongRelation).Save();
            return q;
        }

        /// <summary>
        /// 根据承包方id集合删除承包方关联数据
        /// </summary>
        public int DeleteRelationDataByVps(List<Guid> vpIds)
        {
            if (vpIds == null || vpIds.Count == 0)
                return -1;

            int delAppend = 0;
            var arrayVpid = vpIds.ToArray();
            //删除承包方
            delAppend = AppendEdit(DataSource.CreateQuery<T>().Where(c => arrayVpid.Contains(c.ID)).Delete());

            var landIds = DataSource.CreateQuery<ContractLand>().Where(c => c.OwnerId != null && arrayVpid.Contains((Guid)c.OwnerId)).Select(t => t.ID).ToArray();
            if (landIds == null || landIds.Length == 0)
                return delAppend;

            //删除承包地块
            delAppend = AppendEdit(DataSource.CreateQuery<ContractLand>().Where(c => c.OwnerId != null && arrayVpid.Contains((Guid)c.OwnerId)).Select(t => t).Delete());
            //删除界址点
            delAppend = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressDot>().Where(c => landIds.Contains(c.LandID)).Delete());
            //删除界址线
            delAppend = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(c => landIds.Contains(c.LandID)).Delete());

            var concordIds = DataSource.CreateQuery<ContractConcord>().Where(c => c.ContracterId != null && arrayVpid.Contains((Guid)c.ContracterId)).Select(t => t.ID).ToArray();
            if (concordIds == null || concordIds.Length == 0)
                return delAppend;
            //删除合同
            delAppend = AppendEdit(DataSource.CreateQuery<ContractConcord>().Where(c => c.ContracterId != null && arrayVpid.Contains((Guid)c.ContracterId)).Delete());
            //删除权证
            delAppend = AppendEdit(DataSource.CreateQuery<ContractRegeditBook>().Where(c => concordIds.Contains(c.ID)).Delete());

            return delAppend;
        }

        /// <summary>
        /// 删除当前地域下所有股权关系
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <returns></returns>
        public int DeleteRelationDataByZone(string zoneCode)
        {
            var qRelation = DataSource.CreateQuery<BelongRelation>();
            return AppendEdit(qRelation.Where(o => o.ZoneCode == zoneCode).Delete());
        }

        public BelongRelation GetRelationByID(Guid personId, Guid landId)
        {
            var qRelation = DataSource.CreateQuery<BelongRelation>();
            return qRelation.FirstOrDefault(o => o.VirtualPersonID == personId && o.LandID == landId);
        }

        public List<BelongRelation> GetRelationsByVpID(Guid personID)
        {
            var qRelation = DataSource.CreateQuery<BelongRelation>();
            return qRelation.Where(q => q.VirtualPersonID == personID).ToList();
        }

        public List<BelongRelation> GetRelationByZone(string zoneCode, eLevelOption option)
        {
            var qRelation = DataSource.CreateQuery<BelongRelation>();
            List<BelongRelation> list = new List<BelongRelation>();
            switch (option)
            {
                case eLevelOption.Self:
                    list = qRelation.Where(q => q.ZoneCode == zoneCode).ToList();
                    break;
                case eLevelOption.Subs:
                case eLevelOption.SelfAndSubs:

                    list = qRelation.Where(q => q.ZoneCode.StartsWith(zoneCode)).ToList();
                    break;
                default:
                    break;
            }
            return list;
        }

        #endregion Methods
    }
}