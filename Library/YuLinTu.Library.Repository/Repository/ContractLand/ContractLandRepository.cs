// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.Data;
using System.Data.SqlClient;
using YuLinTu.Data;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 农村土地承包地的数据访问类
    /// </summary>
    public class ContractLandRepository : AgricultureLandRepository<ContractLand>, IContractLandRepository
    {
        #region Ctor

        private IDataSourceSchema m_DSSchema = null;

        public ContractLandRepository(IDataSource ds)
            : base(ds)
        {
            m_DSSchema = ds.CreateSchema();
        }
        #endregion

        /// <summary>
        /// 跟新地块Shape数据
        /// </summary>
        /// <param name="entity">地块实体对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int UpdateShape(ContractLand entity)
        {
            if (entity == null)
                return -1;

            int cnt = 0;
            cnt = AppendEdit(DataSource.CreateQuery<ContractLand>().Where(c => c.ID == entity.ID).
                Update(s => new ContractLand()
                {
                    Shape = entity.Shape
                }));
            return cnt;
        }

        public int UpdateOldLandCode(ContractLand entity)
        {
            if (entity == null)
                return -1;

            int cnt = 0;
            cnt = AppendEdit(DataSource.CreateQuery<ContractLand>().Where(c => c.ID == entity.ID).
                Update(s => new ContractLand()
                {
                    OldLandNumber = entity.OldLandNumber
                }));
            return cnt;
        }

        public int UpdateLandCode(List<ContractLand> entitys)
        {
            if (entitys == null)
                return -1;

            int cnt = 0;
            foreach(var entity in entitys)
            {
                cnt = AppendEdit(DataSource.CreateQuery<ContractLand>().Where(c => c.ID == entity.ID).
                Update(s => new ContractLand()
                {
                    OldLandNumber = entity.OldLandNumber,
                    LandNumber = entity.LandNumber,
                    CadastralNumber = entity.CadastralNumber
                }));
            }
            
            return cnt;
        }

        public int UpdateZoneCode(ContractLand entity)
        {
            if (entity == null)
                return -1;

            int cnt = 0;
            cnt = AppendEdit(DataSource.CreateQuery<ContractLand>().Where(c => c.ID == entity.ID).
                Update(s => new ContractLand()
                {
                    ZoneCode = entity.ZoneCode
                }));
            return cnt;
        }

        public int UpdateLandBoundary(ContractLand entity)
        {
            if (entity == null)
                return -1;

            int cnt = 0;
            cnt = AppendEdit(DataSource.CreateQuery<ContractLand>().Where(c => c.ID == entity.ID).
                Update(s => new ContractLand()
                {
                    NeighborEast = entity.NeighborEast,
                    NeighborWest = entity.NeighborWest,
                    NeighborSouth = entity.NeighborSouth,
                    NeighborNorth = entity.NeighborNorth,
                    LandName = entity.LandName
                })); ; ;
            return cnt;
        }
    }
}