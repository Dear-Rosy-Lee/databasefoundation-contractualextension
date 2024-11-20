/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Spatial;
using Microsoft.Practices.Unity;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// WorkStation 扩展类
    /// </summary>
    public static class WorkStationExtend
    {
        #region 承包方

        /// <summary>
        /// 创建承包方WorkStation
        /// </summary>
        public static IVirtualPersonWorkStation<T> CreateVirtualPersonStation<T>(this ContainerFactory factory) where T : VirtualPerson
        {
            var rep = factory.CreateRepository<IVirtualPersonRepository<T>>();
            var belongRep = factory.CreateRepository<IBelongRelationRespository>();
            IVirtualPersonWorkStation<T> station = factory.CreateWorkstation<IVirtualPersonWorkStation<T>>(
                new ParameterOverride("rep", rep),
                new ParameterOverride("belongRep", belongRep)
                );
            return station;
        }


        /// <summary>
        /// 创建承包方WorkStation
        /// </summary>
        public static IVirtualPersonWorkStation<T> CreateVirtualPersonStation<T>(this IDbContext db) where T : VirtualPerson
        {
            if (db == null)
            {
                return null;
            }
            var factory = new ContainerFactory(db);
            return factory.CreateVirtualPersonStation<T>();
            //var rep = factory.CreateRepository<IVirtualPersonRepository<T>>();
            //var belongRep = factory.CreateRepository<IBelongRelationRespository>();
            //IVirtualPersonWorkStation<T> station = new ContainerFactory(db).CreateWorkstation<IVirtualPersonWorkStation<T>>(
            //    new ParameterOverride("rep", rep),
            //    new ParameterOverride("belongRep", belongRep)
            //    );
            //return station;
        }

        #endregion

        #region 发包方

        /// <summary>
        /// 创建发包方WorkStation
        /// </summary>
        public static ISenderWorkStation CreateSenderWorkStation(this ContainerFactory factory)
        {
            var repTissue = factory.CreateRepository<ICollectivityTissueRepository>();
            var repZone = factory.CreateRepository<IZoneRepository>();
            ISenderWorkStation station = factory.CreateWorkstation<ISenderWorkStation>(
                  new ParameterOverride("rep", repTissue),
                  new ParameterOverride("zonerep", repZone));
            return station;
        }

        /// <summary>
        /// 创建发包方WorkStation
        /// </summary>
        public static ISenderWorkStation CreateSenderWorkStation(this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            ContainerFactory factory = new ContainerFactory(db);
            var repTissue = factory.CreateRepository<ICollectivityTissueRepository>();
            var repZone = factory.CreateRepository<IZoneRepository>();
            ISenderWorkStation station = factory.CreateWorkstation<ISenderWorkStation>(
                  new ParameterOverride("rep", repTissue),
                  new ParameterOverride("zonerep", repZone));
            return station;
        }

        #endregion

        #region 地域

        /// <summary>
        /// 创建地域WorkStation
        /// </summary>
        public static IZoneWorkStation CreateZoneWorkStation(this ContainerFactory factory)
        {
            var rep = factory.CreateRepository<IZoneRepository>();
            IZoneWorkStation station = factory.CreateWorkstation<IZoneWorkStation>(
                  new ParameterOverride("rep", rep));
            return station;
        }

        /// <summary>
        /// 创建地域WorkStation
        /// </summary>
        public static IZoneWorkStation CreateZoneWorkStation(this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            ContainerFactory factory = new ContainerFactory(db);
            var rep = factory.CreateRepository<IZoneRepository>();
            IZoneWorkStation station = factory.CreateWorkstation<IZoneWorkStation>(
                  new ParameterOverride("rep", rep));
            return station;
        }

        #endregion

        #region 数据字典

        /// <summary>
        /// 创建数据字典WorkStation
        /// </summary>
        public static IDictionaryWorkStation CreateDictWorkStation(this ContainerFactory factory)
        {
            var rep = factory.CreateRepository<IDictionaryRepository>();
            IDictionaryWorkStation station = factory.CreateWorkstation<IDictionaryWorkStation>(
                                             new ParameterOverride("rep", rep));
            return station;
        }

        /// <summary>
        /// 创建数据字典WorkStation
        /// </summary>
        public static IDictionaryWorkStation CreateDictWorkStation(this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            ContainerFactory factory = new ContainerFactory(db);
            var rep = factory.CreateRepository<IDictionaryRepository>();
            IDictionaryWorkStation station = factory.CreateWorkstation<IDictionaryWorkStation>(
                                             new ParameterOverride("rep", rep));

            return station;
        }

        #endregion

        #region 二轮台账地块

        /// <summary>
        /// 创建二轮台账地块WorkStation
        /// </summary>
        public static ISecondTableLandWorkStation CreateSecondTableLandWorkstation(this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            ISecondTableLandWorkStation station = new ContainerFactory(db).CreateWorkstation<ISecondTableLandWorkStation, ISecondTableLandRepository>();
            return station;
        }

        #endregion

        #region 承包台账地块

        /// <summary>
        /// 创建承包台账地块WorkStation
        /// </summary>
        public static IContractLandWorkStation CreateContractLandWorkstation(this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            var factory = new ContainerFactory(db);
            var landRep = factory.CreateRepository<IContractLandRepository>();
            var zoneWork = factory.CreateWorkstation<IZoneWorkStation, IZoneRepository>();
            var senderRep = factory.CreateRepository<ICollectivityTissueRepository>();
            var virtualPersonWork = factory.CreateVirtualPersonStation<LandVirtualPerson>();//factory.CreateWorkstation<IVirtualPersonWorkStation<LandVirtualPerson>, IVirtualPersonRepository<LandVirtualPerson>>();
            var concordWork = factory.CreateWorkstation<IConcordWorkStation, IContractConcordRepository>();
            var bookWork = factory.CreateWorkstation<IContractRegeditBookWorkStation, IContractRegeditBookRepository>();
            var dotWork = factory.CreateWorkstation<IBuildLandBoundaryAddressDotWorkStation, IBuildLandBoundaryAddressDotRepository>();
            var coilWork = factory.CreateWorkstation<IBuildLandBoundaryAddressCoilWorkStation, IBuildLandBoundaryAddressCoilRepository>();
            var belongRep = factory.CreateRepository<IBelongRelationRespository>();
            var landDelRep = factory.CreateRepository<IContractLandDeleteRepository>();
            IContractLandWorkStation station = new ContainerFactory(db).CreateWorkstation<IContractLandWorkStation>(
                new ParameterOverride("rep", landRep),
                new ParameterOverride("repZone", zoneWork.DefaultRepository),
                new ParameterOverride("workZone", zoneWork),
                new ParameterOverride("repSender", senderRep),
                new ParameterOverride("repVirtualPerson", virtualPersonWork.DefaultRepository),
                new ParameterOverride("workVirtualPerson", virtualPersonWork),
                new ParameterOverride("repConcord", concordWork.DefaultRepository),
                new ParameterOverride("workConcord", concordWork),
                new ParameterOverride("repBook", bookWork.DefaultRepository),
                new ParameterOverride("workBook", bookWork),
                new ParameterOverride("repDot", dotWork.DefaultRepository),
                new ParameterOverride("workDot", dotWork),
                new ParameterOverride("repCoil", coilWork.DefaultRepository),
                new ParameterOverride("workCoil", coilWork),
                new ParameterOverride("belongRep", belongRep),
                new ParameterOverride("landdelrep", landDelRep)
                );
            return station;
        }

        public static IContractLandMarkWorkStation CreateContractLandMarkWorkstation(this IDbContext db)
        {
            if (db == null)
                return null;
            IContractLandMarkWorkStation station = new ContainerFactory(db).CreateWorkstation<IContractLandMarkWorkStation, IContractLandMarkRepository>();
            return station;
        }
        #endregion

        #region 承包合同

        /// <summary>
        /// 创建承包合同WorkStation
        /// </summary>
        public static IConcordWorkStation CreateConcordStation(this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            IConcordWorkStation station = new ContainerFactory(db).CreateWorkstation<IConcordWorkStation, IContractConcordRepository>();
            return station;
        }

        #endregion

        #region 承包权证

        /// <summary>
        /// 创建承包权证WorkStation
        /// </summary>
        public static IContractRegeditBookWorkStation CreateRegeditBookStation(this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            IContractRegeditBookWorkStation station = new ContainerFactory(db).CreateWorkstation<IContractRegeditBookWorkStation, IContractRegeditBookRepository>();
            return station;
        }

        #endregion

        #region 农村土地承包经营权申请书

        /// <summary>
        /// 创建农村土地承包经营权申请书数据访问层接口
        /// </summary>
        public static IContractRequireTableWorkStation CreateRequireTableWorkStation(this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            IContractRequireTableWorkStation station = new ContainerFactory(db).CreateWorkstation<IContractRequireTableWorkStation, IContractRequireTableRepository>();
            return station;
        }

        #endregion

        #region 界址点

        /// <summary>
        /// 创建界址点业务逻辑层接口
        /// </summary>
        public static IBuildLandBoundaryAddressDotWorkStation CreateBoundaryAddressDotWorkStation
            (this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            IBuildLandBoundaryAddressDotWorkStation station = new ContainerFactory(db).CreateWorkstation<IBuildLandBoundaryAddressDotWorkStation, IBuildLandBoundaryAddressDotRepository>();
            return station;
        }

        #endregion

        #region 界址线

        /// <summary>
        /// 创建界址线业务逻辑层接口
        /// </summary>
        public static IBuildLandBoundaryAddressCoilWorkStation CreateBoundaryAddressCoilWorkStation
            (this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            IBuildLandBoundaryAddressCoilWorkStation station = new ContainerFactory(db).CreateWorkstation<IBuildLandBoundaryAddressCoilWorkStation, IBuildLandBoundaryAddressCoilRepository>();
            return station;
        }

        #endregion

        #region 基本农田保护区

        /// <summary>
        /// 创建基本农田保护区业务逻辑层接口
        /// </summary>
        public static IFarmLandConserveWorkStation CreateFarmLandConserveWorkStation
            (this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            IFarmLandConserveWorkStation station = new ContainerFactory(db).CreateWorkstation<IFarmLandConserveWorkStation, IFarmLandConserveRepository>();
            return station;
        }

        #endregion

        #region 点状地物

        /// <summary>
        /// 创建点状地物业务逻辑层接口
        /// </summary>
        public static IDZDWWorkStation CreateDZDWWorkStation
            (this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            IDZDWWorkStation station = new ContainerFactory(db).CreateWorkstation<IDZDWWorkStation, IDZDWRepository>();
            return station;
        }

        #endregion

        #region 线状地物

        /// <summary>
        /// 创建线状地物业务逻辑层接口
        /// </summary>
        public static IXZDWWorkStation CreateXZDWWorkStation
            (this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            IXZDWWorkStation station = new ContainerFactory(db).CreateWorkstation<IXZDWWorkStation, IXZDWRepository>();
            return station;
        }

        #endregion

        #region 面状地物

        /// <summary>
        /// 创建面状地物业务逻辑层接口
        /// </summary>
        public static IMZDWWorkStation CreateMZDWWorkStation
            (this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            IMZDWWorkStation station = new ContainerFactory(db).CreateWorkstation<IMZDWWorkStation, IMZDWRepository>();
            return station;
        }

        #endregion

        #region 调查宗地

        /// <summary>
        /// 创建调查宗地业务逻辑层接口
        /// </summary>
        public static IDCZDWorkStation CreateDCZDWorkStation
            (this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            IDCZDWorkStation station = new ContainerFactory(db).CreateWorkstation<IDCZDWorkStation, IDCZDRepository>();
            return station;
        }

        #endregion

        #region 控制点

        /// <summary>
        /// 创建控制点业务逻辑层接口
        /// </summary>
        public static IControlPointWorkStation CreateControlPointWorkStation
            (this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            IControlPointWorkStation station = new ContainerFactory(db).CreateWorkstation<IControlPointWorkStation, IControlPointRepository>();
            return station;
        }

        #endregion

        #region 区域界线

        /// <summary>
        /// 创建区域界线业务逻辑层接口
        /// </summary>
        public static IZoneBoundaryWorkStation CreateZoneBoundaryWorkStation
            (this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            IZoneBoundaryWorkStation station = new ContainerFactory(db).CreateWorkstation<IZoneBoundaryWorkStation, IZoneBoundaryRepository>();
            return station;
        }

        #endregion

        #region    确权确股
        public static IBelongRelationWorkStation CreateBelongRelationWorkStation(this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            var factory = new ContainerFactory(db);
            var landRep = factory.CreateRepository<IContractLandRepository>();
            var rep = factory.CreateRepository<IBelongRelationRespository>();
            var personRep = factory.CreateRepository<IVirtualPersonRepository<LandVirtualPerson>>();
            IBelongRelationWorkStation station = factory.CreateWorkstation<IBelongRelationWorkStation>(
                 new ParameterOverride("rep", rep),
                 new ParameterOverride("landRep", landRep),
                 new ParameterOverride("personRep", personRep)
                 );
            return station;
        }


        public static IStockConcordWorkStation CreateStockConcordWorkStation(this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            var factory = new ContainerFactory(db);
            var rep = factory.CreateRepository<IStockConcordRespository>();
            var warrantRep= factory.CreateRepository<IStockWarrantRespository>();
            var station = factory.CreateWorkstation<IStockConcordWorkStation>(
                 new ParameterOverride("rep", rep),
                 new ParameterOverride("warrantRep", warrantRep)
                 );
            return station;
        }



        public static IStockWarrantWorkStation CreateStockWarrantWorkStation(this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            var factory = new ContainerFactory(db);
            var rep = factory.CreateRepository<IStockWarrantRespository>();
            var station = factory.CreateWorkstation<IStockWarrantWorkStation>(
                 new ParameterOverride("rep", rep));
            return station;
        }

        #endregion
        
        #region 配置获取

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public static string GetSystemSetReplacement()
        {
            var center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<SystemSetDefine>();
            var section = profile.GetSection<SystemSetDefine>();
            var config = section.Settings as SystemSetDefine;
            var emptyReplacement = config == null ? "" : (config.EmptyReplacement == null ? "" : config.EmptyReplacement);
            return emptyReplacement;
        }

        #endregion
    }

    /// <summary>
    /// 创建业务逻辑类
    /// </summary>
    public class CreateWorkStation
    {
        #region Properties

        /// <summary>
        /// 行政地域业务逻辑
        /// </summary>
        public IZoneWorkStation ZoneStation { get; set; }

        /// <summary>
        /// 发包方业务逻辑
        /// </summary>
        public ISenderWorkStation SenderStation { get; set; }

        /// <summary>
        /// 承包方业务逻辑
        /// </summary>
        public IVirtualPersonWorkStation<LandVirtualPerson> LandVirtualPersonStation { get; set; }

        /// <summary>
        /// 承包台账业务逻辑
        /// </summary>
        public IContractLandWorkStation ContractLandStation { get; set; }

        /// <summary>
        /// 权属关系业务逻辑
        /// </summary>
        public IBelongRelationWorkStation BelongRelationStation { get; set; }

        /// <summary>
        /// 承包合同业务逻辑
        /// </summary>
        public IConcordWorkStation ConcordStation { get; set; }

        /// <summary>
        /// 承包权证业务逻辑
        /// </summary>
        public IContractRegeditBookWorkStation BookStation { get; set; }

        /// <summary>
        /// 二轮台账业务逻辑
        /// </summary>
        public ISecondTableLandWorkStation SecondTableStation { get; set; }

        /// <summary>
        /// 二轮承包方业务逻辑
        /// </summary>
        public IVirtualPersonWorkStation<TableVirtualPerson> SecondPersonStation { get; set; }

        /// <summary>
        /// 界址点业务逻辑
        /// </summary>
        public IBuildLandBoundaryAddressDotWorkStation DotStation { get; set; }

        /// <summary>
        /// 界址线业务逻辑
        /// </summary>
        public IBuildLandBoundaryAddressCoilWorkStation CoilStation { get; set; }

        /// <summary>
        /// 申请登记簿业务逻辑
        /// </summary>
        public IContractRequireTableWorkStation RequireTableStation { get; set; }

        /// <summary>
        /// 基本农田保护区业务逻辑
        /// </summary>
        public IFarmLandConserveWorkStation FarmLandConserveStation { get; set; }

        /// <summary>
        /// 点状地物业务逻辑
        /// </summary>
        public IDZDWWorkStation PointStation { get; set; }

        /// <summary>
        /// 线状地物业务逻辑
        /// </summary>
        public IXZDWWorkStation LineStation { get; set; }

        /// <summary>
        /// 面状地物业务逻辑
        /// </summary>
        public IMZDWWorkStation PolygonStation { get; set; }

        /// <summary>
        /// 调查宗地业务逻辑
        /// </summary>
        public IDCZDWorkStation SurveyLandStation { get; set; }

        /// <summary>
        /// 控制点业务逻辑
        /// </summary>
        public IControlPointWorkStation ControlPointStation { get; set; }

        /// <summary>
        /// 区域界线业务逻辑
        /// </summary>
        public IZoneBoundaryWorkStation ZoneBoundaryStation { get; set; }
        /// <summary>
        /// 确股合同
        /// </summary>
        public IStockConcordWorkStation StockConcordWorkStation { get; set; }
        /// <summary>
        /// 确股权证
        /// </summary>
        public IStockWarrantWorkStation StockWarrantWorkStation { get; set; }
        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public CreateWorkStation()
        { }

        #endregion

        #region Method

        /// <summary>
        /// 创建业务逻辑
        /// </summary>
        /// <param name="dbContext">数据源</param>
        public bool Create(IDbContext dbContext)
        {
            try
            {
                ZoneStation = dbContext.CreateZoneWorkStation();
                SenderStation = dbContext.CreateSenderWorkStation();
                LandVirtualPersonStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                ContractLandStation = dbContext.CreateContractLandWorkstation();
                BelongRelationStation = dbContext.CreateBelongRelationWorkStation();
                ConcordStation = dbContext.CreateConcordStation();
                BookStation = dbContext.CreateRegeditBookStation();
                SecondTableStation = dbContext.CreateSecondTableLandWorkstation();
                SecondPersonStation = dbContext.CreateVirtualPersonStation<TableVirtualPerson>();
                DotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                CoilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                RequireTableStation = dbContext.CreateRequireTableWorkStation();
                FarmLandConserveStation = dbContext.CreateFarmLandConserveWorkStation();
                PointStation = dbContext.CreateDZDWWorkStation();
                LineStation = dbContext.CreateXZDWWorkStation();
                PolygonStation = dbContext.CreateMZDWWorkStation();
                SurveyLandStation = dbContext.CreateDCZDWorkStation();
                ControlPointStation = dbContext.CreateControlPointWorkStation();
                ZoneBoundaryStation = dbContext.CreateZoneBoundaryWorkStation();
                StockWarrantWorkStation = dbContext.CreateStockWarrantWorkStation();
                StockConcordWorkStation = dbContext.CreateStockConcordWorkStation();
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
