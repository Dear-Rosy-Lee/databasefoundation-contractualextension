//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using YuLinTu.Data;
//using YuLinTu.Library.Business;
//using YuLinTu.Library.Entity;

//namespace YuLinTu.Component.StockRightBase.Entity
//{
//    /// <summary>
//    /// 业务所用全部实体封装到此类中
//    /// </summary>
//    public class StockRightBussinessObject
//    {
//        private Zone _currentZone;

//        public Zone CurrentZone
//        {
//            get { return _currentZone; }
//            set { _currentZone = value; }
//        }

//        /// <summary>
//        /// 股农承包方
//        /// </summary>
//        public ObservableCollection<VirtualPerson> VirtualPersons { get; set; } =
//            new ObservableCollection<VirtualPerson>();

//        /// <summary>
//        /// 承包方全部
//        /// </summary>
//        public ObservableCollection<VirtualPerson> VirtualPersonsAll { get; set; } =
//            new ObservableCollection<VirtualPerson>();

//        /// <summary>
//        /// 确股地块
//        /// </summary>
//        public ObservableCollection<ContractLand> ContractLands { get; set; }
//            = new ObservableCollection<ContractLand>();

//        /// <summary>
//        /// 全部地块
//        /// </summary>
//        public ObservableCollection<ContractLand> ContractLandsAll { get; set; }
//            = new ObservableCollection<ContractLand>();


//        /// <summary>
//        /// 权属关系
//        /// </summary>
//        public List<BelongRelation> BelongRelations { get; set; } = new List<BelongRelation>();


//        /// <summary>
//        /// 全数据字典
//        /// </summary>
//        public List<Dictionary> DictList { get; set; } 
//            = new CDObjectList<Dictionary>();

//        /// <summary>
//        /// 地力等级
//        /// </summary>
//        public List<Dictionary> DicLandLevel
//        {
//            get { return DictList?.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ); }
//        }

//        /// <summary>
//        /// 土地用途
//        /// </summary>
//        public List<Dictionary> DicLandPurpose
//        {
//            get { return DictList?.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDYT); }
//        }

//        /// <summary>
//        /// 种植类型
//        /// </summary>
//        public List<Dictionary> DicPlantType
//        {
//            get { return DictList?.FindAll(c => c.GroupCode == DictionaryTypeInfo.ZZLX); }
//        }

//        /// <summary>
//        /// 土地利用类型
//        /// </summary>
//        public List<Dictionary> DicLandType
//        {
//            get { return DictList?.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDLYLX); }
//        }


//    }
//}
