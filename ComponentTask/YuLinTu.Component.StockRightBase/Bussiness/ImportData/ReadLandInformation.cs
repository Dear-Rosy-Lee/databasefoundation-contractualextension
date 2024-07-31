using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Component.StockRightBase.Model;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;

namespace YuLinTu.Component.StockRightBase.Bussiness
{
    public partial class ReadExcelBase : ExcelBase
    {
        /// <summary>
        /// 读取地块数据
        /// </summary>
        /// <param name="contractor"></param>
        /// <param name="entity"></param>
        public void GetLands(ConvertEntity convertEntity, ExcelReadEntity readEntity)
        {

            _bussnessObject = _bussinessData.GetBussinessObject(CurrentZone);
            var dicLandLevel = _bussnessObject.DictList?.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ);
            var dicLandPurpose = _bussnessObject.DictList?.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDYT);
            var dicLandType = _bussnessObject.DictList?.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDLYLX);
            var currentIndex = readEntity.StartRow;
            for (int i = 0; i < readEntity.LandCount; i++, currentIndex++)
            {
                ContractLand land = new ContractLand();
                AgricultureLandExpand landExpand = new AgricultureLandExpand();
                land.Name = GetString(_allItem[currentIndex, ColumnDefine.LAND_NAME]);//字段：地块名称
                var landNumber = GetString(_allItem[currentIndex, ColumnDefine.LAND_NUMBER]);//字段：地块编码
                land.LandNumber = landNumber;
                land.ID = _landIdDic.ContainsKey(landNumber) ? _landIdDic[landNumber] : new Guid();
                land.LandExpand.ImageNumber = GetString(_allItem[currentIndex, ColumnDefine.LAND_PICTURE_NUM]);//字段：图幅编号
                var tableArea = DataHelper.GetDouble(GetString(_allItem[currentIndex, ColumnDefine.LAND_TABLEAREA]));
                var area = tableArea.HasValue ? tableArea.Value : 0;
                land.TableArea = Math.Round(area, 2);//字段：二轮合同面积（台账面积）
                var actualArea = GetString(_allItem[currentIndex, ColumnDefine.LAND_ACTUALAREA]);
                land.ActualArea = GetDouble(actualArea);//字段：实测面积
                land.NeighborEast = GetString(_allItem[currentIndex, ColumnDefine.LAND_EAST]);//字段：东至
                land.NeighborSouth = GetString(_allItem[currentIndex, ColumnDefine.LAND_SOUTH]);//字段：南至
                land.NeighborWest = GetString(_allItem[currentIndex, ColumnDefine.LAND_WEST]);//字段：西至
                land.NeighborNorth = GetString(_allItem[currentIndex, ColumnDefine.LAND_NORTH]);//字段：北至
                land.ZoneCode = _currentZone.FullCode;

                land.Purpose = dicLandPurpose?.Find(o => o.Name == (_allItem[currentIndex, ColumnDefine.LAND_PURPOSE]?.ToString()))?.Code;//字段：土地用途
                land.LandLevel = dicLandLevel?.Find(o => o.Name == (_allItem[currentIndex, ColumnDefine.LAND_LEVEL]?.ToString()))?.Code;//字段：等级
                land.LandCode = dicLandType?.Find(o => o.Name == (_allItem[currentIndex, ColumnDefine.LAND_USEDTYPE]?.ToString()))?.Code;//字段：土地利用类型
                land.LandName = new LandUseTypeConvert().Convert(land.LandCode, null, null, null) as string;
                land.IsFarmerLand = GetString(_allItem[currentIndex, ColumnDefine.LAND_ISFARM]) == "是";//字段：是否基本农田
                land.Comment = GetString(_allItem[currentIndex, ColumnDefine.LAND_COMMENT]);//字段：备注
                land.ZoneCode = _currentZone.FullCode;
                land.IsStockLand = true;
                land.LandCategory = "10";
                land.SurveyNumber = land.LandNumber.GetLastString(5);
                land.ConstructMode = "110";

                landExpand.ReferPerson = GetString(_allItem[currentIndex, ColumnDefine.LAND_REF]);//字段：指界人
                landExpand.SurveyPerson = GetString(_allItem[currentIndex, ColumnDefine.LAND_SUVPERSON]);//字段：调查员
                landExpand.SurveyDate = GetDateTime(_allItem[currentIndex, ColumnDefine.LAND_SUVDATE]);//字段：调查日期
                landExpand.SurveyChronicle = GetString(_allItem[currentIndex, ColumnDefine.LAND_SUVREMARK]);//字段：调查记事

                landExpand.CheckPerson = GetString(_allItem[currentIndex, ColumnDefine.LAND_CHECKPERSON]);//字段：审核人
                landExpand.CheckDate = GetDateTime(_allItem[currentIndex, ColumnDefine.LAND_CHECKDATE]);//字段：审核日期
                landExpand.CheckOpinion = GetString(_allItem[currentIndex, ColumnDefine.LAND_CHECKOPINION]);//字段：审核意见

                land.LandExpand = landExpand;

                //确股信息
                var shareArea = GetString(_allItem[currentIndex, ColumnDefine.LAND_SHAREAREA]);//字段：共用面积
                land.ShareArea = Math.Round(GetDouble(shareArea), 2).ToString();
                land.ConcordArea = GetString(_allItem[currentIndex, ColumnDefine.LAND_CONCORDAREA]);//字段：合同面积
                land.QuantificicationArea = GetDouble(_allItem[currentIndex, ColumnDefine.LAND_QUAAREA]);//字段：量化户面积

                convertEntity.LandList.Add(land);
            }
        }
    }
}
