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

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 创建业务逻辑层类 
    /// </summary>
    public static class CreateWorkStation
    {

        #region 承包方

        /// <summary>
        /// 创建承包方WorkStation
        /// </summary>
        public static IVirtualPersonWorkStation<T> CreateVirtualPersonStationEx<T>(this ContainerFactory factory) where T : VirtualPerson
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
        public static IVirtualPersonWorkStation<T> CreateVirtualPersonStationEx<T>(this IDbContext db) where T : VirtualPerson
        {
            if (db == null)
            {
                return null;
            }
            var factory = new ContainerFactory(db);
            return factory.CreateVirtualPersonStationEx<T>();
            //var rep = factory.CreateRepository<IVirtualPersonRepository<T>>();
            //var belongRep = factory.CreateRepository<IBelongRelationRespository>();
            //IVirtualPersonWorkStation<T> station = new ContainerFactory(db).CreateWorkstation<IVirtualPersonWorkStation<T>>(
            //    new ParameterOverride("rep", rep),
            //    new ParameterOverride("belongRep", belongRep)
            //    );
            //return station;
        }

        #endregion

        #region 承包台账地块

        /// <summary>
        /// 创建承包台账地块WorkStation
        /// </summary>
        public static IContractLandWorkStation CreateAgriculturalLandWorkstation(this IDbContext db)
        {
            if (db == null)
            {
                return null;
            }
            var factory = new ContainerFactory(db);
            var landRep = factory.CreateRepository<IContractLandRepository>();
            var zoneWork = factory.CreateWorkstation<IZoneWorkStation, IZoneRepository>();
            var senderRep = factory.CreateRepository<ICollectivityTissueRepository>();
            var virtualPersonWork = factory.CreateVirtualPersonStationEx<LandVirtualPerson>();
            var concordWork = factory.CreateWorkstation<IConcordWorkStation, IContractConcordRepository>();
            var bookWork = factory.CreateWorkstation<IContractRegeditBookWorkStation, IContractRegeditBookRepository>();
            var dotWork = factory.CreateWorkstation<IBuildLandBoundaryAddressDotWorkStation, IBuildLandBoundaryAddressDotRepository>();
            var coilWork = factory.CreateWorkstation<IBuildLandBoundaryAddressCoilWorkStation, IBuildLandBoundaryAddressCoilRepository>();
            var belongRep = factory.CreateRepository<IBelongRelationRespository>();
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
                new ParameterOverride("belongRep",belongRep)
                );
            return station;
        }

        #endregion

        #region 发包方

        /// <summary>
        /// 创建发包方WorkStation
        /// </summary>
        public static ISenderWorkStation CreateCollectivityTissueWorkStation(this IDbContext db)
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

        #region 界址点

        /// <summary>
        /// 创建界址点业务逻辑层接口
        /// </summary>
        public static IBuildLandBoundaryAddressDotWorkStation CreateDotWorkStation
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
        public static IBuildLandBoundaryAddressCoilWorkStation CreateCoilWorkStation
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
    }
}
