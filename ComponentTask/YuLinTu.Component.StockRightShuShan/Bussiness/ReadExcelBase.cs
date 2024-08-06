using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Component.StockRightShuShan.Entity;
using YuLinTu.Component.StockRightShuShan.Model;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;

namespace YuLinTu.Component.StockRightShuShan.Bussiness
{
    public partial class ReadExcelBase : ExcelBase
    {
        protected Zone _currentZone;
        protected int _breakIndex = 5;//中断读取的行
        protected Dictionary<string, Guid> _landIdDic = new Dictionary<string, Guid>();//地块编码和地块ID的字典
        private IDbContext _dbContext;
        protected object[,] _allItem;
        protected int _rangeCount;//行数
        private int _columnCount;//列数
        protected BussinessData _bussinessData;
        public List<ConvertEntity> ExcelData;//Excel表数据集合
        protected List<ExcelReadEntity> _readEntity;
        public List<LandContractor> LandContractors { get; set; } = new List<LandContractor>();

        public List<LandEx> LandExs { get; set; } = new List<LandEx>();

        protected StockRightBussinessObject _bussnessObject;


        /// <summary>
        /// 导入excel表的名称
        /// </summary>
        public string ExcelName { get; set; }

        /// <summary>
        /// 当前行政区域
        /// </summary>
        public Zone CurrentZone
        {
            get { return _currentZone; }
            set
            {
                _currentZone = value;
                _bussinessData = new BussinessData(DbContext, _currentZone);
            }
        }

        /// <summary>
        /// 数据库实例
        /// </summary>
        public IDbContext DbContext
        {
            get { return _dbContext; }
            set
            {
                _dbContext = value;
                _bussinessData = new BussinessData(DbContext, CurrentZone);
            }
        }


        public ReadExcelBase()
        {
            _bussinessData = new BussinessData(DbContext, CurrentZone);
        }

        /// <summary>
        /// 检查值
        /// </summary>
        private string CheckValue()
        {
            var openResult = OpenExcel();
            if (openResult != null)
                return openResult;
            var setResult = SetValue();
            if (setResult != null)
                return setResult;
            return null;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        private string SetValue()
        {
            _allItem = GetAllRangeValue();
            _rangeCount = GetRangeRowCount();
            _columnCount = GetRangeColumnCount();
            GetBreakRow();
            _readEntity = GetReadEntity();
            _landIdDic = GetLandIdDic();
            if (_allItem == null || _allItem.Length < 1)
                return (this.ExcelName + "表中可能没有数据或数据可能已经损坏,如果表中有数据请重新建张新表,然后将原数据拷贝过去,再执行该操作!");
            if (_rangeCount < 1 || _columnCount < 1)
                return (this.ExcelName + "表中可能没有数据或数据可能已经损坏,如果表中有数据请重新建张新表,然后将原数据拷贝过去,再执行该操作!");
            ExcelData = GetExcelData();
            if (ExcelData.Count == 0)
            {
                return (this.ExcelName + "导入模板中可能未填写任何导入数据，请检查导入表!");
            }
            return null;
        }

        /// <summary>
        /// 获取地块编码并配置相应的id
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Guid> GetLandIdDic()
        {
            Dictionary<string, Guid> landIdDic = new Dictionary<string, Guid>();

            if (_allItem != null && _allItem.Length != 0)
            {
                for (int i = 0; i < _rangeCount&&i<_breakIndex; i++)
                {
                    var landNumber = GetString(GetString(_allItem[i, ColumnDefine.LAND_NUMBER]));//字段：地块编码
                    
                    var landNumberNoWhite = landNumber?.Trim();
                    if (string.IsNullOrEmpty(landNumberNoWhite)||(!string.IsNullOrEmpty(landNumberNoWhite))&&landNumberNoWhite.Equals("地块编码"))
                    {
                        continue; 
                    }
                    else if(!landIdDic.ContainsKey(landNumberNoWhite))
                    {
                        landIdDic.Add(landNumberNoWhite,Guid.NewGuid());
                    }
                }
            }

            return landIdDic;
        }

        /// <summary>
        /// 获取Excel表数据的中间转化实体
        /// </summary>
        /// <returns></returns>
        private List<ConvertEntity> GetExcelData()
        {
            List<ConvertEntity> excelData = new List<ConvertEntity>();
            if (_readEntity.Count != 0)
            {
                foreach (var entity in _readEntity)
                {
                    var convertyEntity = new ConvertEntity();
                    convertyEntity.Number = entity.ContractorNumber;
                    GetContractorInfo(convertyEntity.Contractor, entity);
                    GetSharePersonList(convertyEntity, entity);
                    GetLands(convertyEntity, entity);

                    excelData.Add(convertyEntity);
                }
            }

            return excelData;
        }

        /// <summary>
        /// 获取读excel的帮助实体
        /// </summary>
        /// <returns></returns>
        private List<ExcelReadEntity> GetReadEntity()
        {
            _readEntity = new List<ExcelReadEntity>();
            var strList = new string[] {"合计",string.Empty,"/" };

            if (_allItem != null && _rangeCount > 5 && _columnCount >= 45)
            {
                for (int row = 5; row < _rangeCount&&row<_breakIndex; row ++)
                {
                    var contractorNumber = GetString(_allItem[row, ColumnDefine.CONTRACTOR_NUMBER]);
                    var contractorName = GetString(_allItem[row, ColumnDefine.CONTRACTOR_NAME]);
                    if (contractorNumber.IsNullOrEmpty() || contractorName.IsNullOrEmpty())
                    {
                        continue;
                    }
                    if (DataHelper.Match(contractorNumber.Trim(), strList))
                    {
                        break;
                    }
                    ExcelReadEntity readEntity = new ExcelReadEntity();
                    readEntity.ContractorNumber = contractorNumber.Trim();
                    readEntity.ContractorName = contractorName.Trim();
                    var personCount = GetString(_allItem[row, ColumnDefine.SHARE_PERSON_COUNT]);
                    readEntity.SharePersonCount = DataHelper.GetInt(personCount);
                    var landCount = GetString(_allItem[row, ColumnDefine.LAND_COUNT]);
                    readEntity.LandCount = DataHelper.GetInt(landCount);
                    readEntity.StartRow = row;

                    _readEntity.Add(readEntity);
                }
            }

            return _readEntity;
        }

        /// <summary>
        /// 打开表格
        /// </summary>
        private string OpenExcel()
        {
            try
            {
                Open(ExcelName);
            }
            catch (Exception ex)
            {
                Dispose();
                YuLinTu.Library.Log.Log.WriteException(this, "OpenExcel(打开表格失败)", ex.Message + ex.StackTrace);
                return ("打开Excel文件时出错,错误信息：" + ex.Message.ToString());
            }
            return null;
        }



        public string ReadTableInformation()
        {
            var checkResult = CheckValue();
            if (checkResult != null)
                return checkResult;
            //var readResult = ReadExcelToEntity();
            //if (readResult != null)
            //    return readResult;
            return null;
        }

        /// <summary>
        /// 获取中断读取的行号
        /// </summary>
        /// <returns></returns>
        private void GetBreakRow()
        {
            _breakIndex = 6;
            for (int i = 0; i < _rangeCount; i++)
            {
                var breakTag = GetString(GetString(_allItem[i, ColumnDefine.CONTRACTOR_NUMBER]));
                if (!breakTag.IsNullOrEmpty() && breakTag.Trim().Equals("合计"))
                {
                    _breakIndex = i;
                    break;
                }
            }
        }

        public virtual string ReadExcelToEntity()
        {
            return null;
        }


        /// <summary>
        /// 检查数据
        /// </summary>
        /// <returns></returns>
        public virtual string CheckData()
        {
            return null;
        }


        public override void Read()
        {
            throw new NotImplementedException();
        }

        public override void Write()
        {
            throw new NotImplementedException();
        }


    }
}
