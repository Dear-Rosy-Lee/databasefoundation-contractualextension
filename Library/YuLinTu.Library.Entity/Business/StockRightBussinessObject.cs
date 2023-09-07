using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 确权确股业务所用全部实体封装到此类中 by 江宇，安徽插件
    /// </summary>
    public class StockRightBussinessObject
    {
        private Zone _currentZone;

        public Zone CurrentZone
        {
            get { return _currentZone; }
            set { _currentZone = value; }
        }

        private ObservableCollection<VirtualPerson> _virtualPersons;
        /// <summary>
        /// 股农承包方
        /// </summary>
        public ObservableCollection<VirtualPerson> VirtualPersons
        {
            get { return _virtualPersons; }
            set
            {
                _virtualPersons = value;
            }
        }

        private ObservableCollection<VirtualPerson> _virtualPersonsAll = new ObservableCollection<VirtualPerson>();
        /// <summary>
        /// 承包方全部
        /// </summary>
        public ObservableCollection<VirtualPerson> VirtualPersonsAll
        {
            get
            {
                return _virtualPersonsAll;
            }
            set
            {
                _virtualPersonsAll = value;
            }
        }

        private ObservableCollection<ContractLand> _contractLands = new ObservableCollection<ContractLand>();
        /// <summary>
        /// 确股地块
        /// </summary>
        public ObservableCollection<ContractLand> ContractLands
        {
            get
            {
                return _contractLands;
            }
            set
            {
                _contractLands = value;
            }
        }


        private ObservableCollection<ContractLand> _contractLandsAll;
        /// <summary>
        /// 全部地块
        /// </summary>
        public ObservableCollection<ContractLand> ContractLandsAll
        {
            get
            {
                return _contractLandsAll;
            }
            set
            {
                _contractLandsAll = value;
            }
        }



        private List<BelongRelation> _belongRelations = new List<BelongRelation>();
        /// <summary>
        /// 权属关系
        /// </summary>
        public List<BelongRelation> BelongRelations
        {
            get
            {
                return _belongRelations;
            }
            set
            {
                _belongRelations = value;
            }
        }


        private List<Dictionary> _dictList = new List<Dictionary>();
        /// <summary>
        /// 全数据字典
        /// </summary>
        public List<Dictionary> DictList
        {
            get
            {
                return _dictList;
            }
            set
            {
                _dictList = value;
            }
        } 

        /// <summary>
        /// 地力等级
        /// </summary>
        public List<Dictionary> DicLandLevel
        {
            get
            {
                if (DictList != null)
                {
                    return DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ);
                }
                return null;
            }
        }

        /// <summary>
        /// 土地用途
        /// </summary>
        public List<Dictionary> DicLandPurpose
        {

            get
            {
                if (DictList != null)
                {
                    return DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDYT);
                }
                return null;
            }
        }

        /// <summary>
        /// 种植类型
        /// </summary>
        public List<Dictionary> DicPlantType
        {

            get
            {
                if (DictList != null)
                {
                    return DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.ZZLX);
                }
                return null;
            }
        }

        /// <summary>
        /// 土地利用类型
        /// </summary>
        public List<Dictionary> DicLandType
        {
            get
            {
                if (DictList != null)
                {
                    return DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDLYLX);
                }
                return null;
            }
        }



    }
}
