using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using YuLinTu.Data;
using YuLinTu;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 升级数据库扩展类
    /// </summary>
    public class UpgradeDatabaseExtent
    {
        #region Properties

        /// <summary>
        /// 升级表集合
        /// </summary>
        public static List<UpgradeDatabase> TableList { get; set; }

        #endregion Properties

        #region 升级数据库

        /// <summary>
        /// 序列化到文件
        /// </summary>
        public static bool SerializeUpgradeDatabaseInfo()
        {
            try
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\UpgradeDatabaseInformation20161031.xml";
                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }
                List<UpgradeDatabase> upList = new List<UpgradeDatabase>();
                var upJZD = AddJZDFields();
                var upQZ = AddQZFields();
                var upCBD = AddCBDFields(true);
                var upCBDMark = AddCBDFields(false);
                var upCBF = AddCBFFields();
                var upQLRELTZ = AddQLRELTZFields();
                var upELTZ = AddELTZFields();
                var upQSGX = AddQSGXFields();
                var upKZD = AddKZDFields();
                var upQYJX = AddQYJXFields();
                var upJBNTBHQ = AddJBNTBHQFields();
                var fbf = AddFBFFields();
                var QGQZ = AddQGQZFields();
                var QGHT = AddQGHTFields();
                upList.Add(upJZD);
                upList.Add(upQZ);
                upList.Add(upCBD);
                upList.Add(upCBDMark);
                upList.Add(upCBF);
                upList.Add(upQLRELTZ);
                upList.Add(upELTZ);
                upList.Add(upQSGX);
                upList.Add(upKZD);
                upList.Add(upQYJX);
                upList.Add(upJBNTBHQ);
                upList.Add(fbf);
                upList.Add(QGQZ);
                upList.Add(QGHT);

                ToolSerialization.SerializeXml(fileName, upList);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 添加界址点字段
        /// </summary>
        private static UpgradeDatabase AddJZDFields()
        {
            UpgradeDatabase upJZD = new UpgradeDatabase();
            upJZD.TableName = "JZD";
            List<UpdateField> upJZdFields = new List<UpdateField>();
            UpdateField sfkyField = new UpdateField();
            sfkyField.FieldName = "SFKY";
            sfkyField.FieldType = "BOOLEAN";
            sfkyField.IsNull = true;
            sfkyField.IsAdd = true;
            upJZdFields.Add(sfkyField);
            upJZD.FieldList = upJZdFields;
            return upJZD;
        }

        /// <summary>
        /// 添加控制点字段
        /// </summary>
        private static UpgradeDatabase AddKZDFields()
        {
            UpgradeDatabase upKZD = new UpgradeDatabase();
            upKZD.TableName = "KZD";
            List<UpdateField> upJZdFields = new List<UpdateField>();
            UpdateField sfkyField = new UpdateField();
            sfkyField.FieldName = "Shape";
            sfkyField.IsNull = true;
            sfkyField.IsAdd = true;
            upJZdFields.Add(sfkyField);
            upKZD.FieldList = upJZdFields;
            return upKZD;
        }

        /// <summary>
        /// 添加区域界限字段
        /// </summary>
        private static UpgradeDatabase AddQYJXFields()
        {
            UpgradeDatabase upQYJX = new UpgradeDatabase();
            upQYJX.TableName = "QYJX";
            List<UpdateField> upJZdFields = new List<UpdateField>();
            UpdateField sfkyField = new UpdateField();
            sfkyField.FieldName = "Shape";
            sfkyField.IsNull = true;
            sfkyField.IsAdd = true;
            upJZdFields.Add(sfkyField);
            upQYJX.FieldList = upJZdFields;
            return upQYJX;
        }

        /// <summary>
        /// 添加基本农田保护区字段
        /// </summary>
        private static UpgradeDatabase AddJBNTBHQFields()
        {
            UpgradeDatabase upJBNTBHQ = new UpgradeDatabase();
            upJBNTBHQ.TableName = "JBNTBHQ";
            List<UpdateField> upJZdFields = new List<UpdateField>();
            UpdateField sfkyField = new UpdateField();
            sfkyField.FieldName = "Shape";
            sfkyField.IsNull = true;
            sfkyField.IsAdd = true;
            upJZdFields.Add(sfkyField);
            upJBNTBHQ.FieldList = upJZdFields;
            return upJBNTBHQ;
        }

        /// <summary>
        /// 【集体经济组织】表添加字段
        /// </summary>
        private static UpgradeDatabase AddFBFFields()
        {
            UpgradeDatabase fbf = new UpgradeDatabase();
            fbf.TableName = "JCSJ_FBF";

            List<UpdateField> fields = new List<UpdateField>();

            // 字段类型存疑
            UpdateField gender = new UpdateField(); // 性别
            gender.FieldName = "Gender";
            gender.FieldType = "TEXT";
            gender.FieldLength = "1";
            fields.Add(gender);

            UpdateField qsxz = new UpdateField(); // 权属性质
            qsxz.FieldName = "QSXZ";
            qsxz.FieldType = "TEXT";
            qsxz.FieldLength = "100";
            fields.Add(qsxz);

            UpdateField shxydm = new UpdateField(); //社会信用代码
            shxydm.FieldName = "SHXYDM";
            shxydm.FieldType = "TEXT";
            shxydm.FieldLength = "100";
            fields.Add(shxydm);

            fbf.FieldList = fields;

            return fbf;
        }

        /// <summary>
        /// 添加权证字段
        /// </summary>
        private static UpgradeDatabase AddQZFields()
        {
            UpgradeDatabase upQZ = new UpgradeDatabase();
            upQZ.TableName = "CBJYQ_QZ";
            List<UpdateField> upQZFields = new List<UpdateField>();
            UpdateField qzlshField = new UpdateField();
            qzlshField.FieldName = "QZLSH";
            qzlshField.FieldType = "TEXT";
            qzlshField.FieldLength = "100";
            qzlshField.IsNull = true;
            qzlshField.IsAdd = true;
            upQZFields.Add(qzlshField);

            UpdateField qzdbrField = new UpdateField();
            qzdbrField.FieldName = "QZDBR";
            qzdbrField.FieldType = "TEXT";
            qzdbrField.FieldLength = "100";
            qzdbrField.IsNull = true;
            qzdbrField.IsAdd = true;
            upQZFields.Add(qzdbrField);

            UpdateField qzdjrqField = new UpdateField();
            qzdjrqField.FieldName = "QZDJRQ";
            qzdjrqField.FieldType = "DATETIME";
            qzdjrqField.IsNull = true;
            qzdjrqField.IsAdd = true;
            upQZFields.Add(qzdjrqField);

            UpdateField sfqgdkField = new UpdateField();
            sfqgdkField.FieldName = "JZSFSH";
            sfqgdkField.FieldType = "BOOLEAN";
            sfqgdkField.IsNull = true;
            sfqgdkField.IsAdd = true;
            upQZFields.Add(sfqgdkField);

            UpdateField qzYSZSBHField = new UpdateField();
            qzYSZSBHField.FieldName = "YSZSBH";
            qzYSZSBHField.FieldType = "TEXT";
            qzYSZSBHField.FieldLength = "100";
            qzYSZSBHField.IsNull = true;
            qzYSZSBHField.IsAdd = true;
            upQZFields.Add(qzYSZSBHField);

            UpdateField qzfjField = new UpdateField();
            qzfjField.FieldName = "QZFJ";
            qzfjField.FieldType = "TEXT";
            qzfjField.IsNull = true;
            qzfjField.IsAdd = true;
            upQZFields.Add(qzfjField);

            //UpdateField jzsfshField = new UpdateField();
            //jzsfshField.FieldName = "JZSFSH";
            //jzsfshField.FieldType = "BOOL";
            //jzsfshField.IsNull = true;
            //jzsfshField.IsAdd = true;
            //upQZFields.Add(jzsfshField);

            upQZ.FieldList = upQZFields;
            return upQZ;
        }

        /// <summary>
        /// 确股权证
        /// </summary>
        private static UpgradeDatabase AddQGQZFields()
        {
            UpgradeDatabase upQZ = new UpgradeDatabase();
            upQZ.TableName = "QGCBJYQ_QZ";
            List<UpdateField> upQZFields = new List<UpdateField>();
            UpdateField qzlshField = new UpdateField();
            qzlshField.FieldName = "QZLSH";
            qzlshField.FieldType = "TEXT";
            qzlshField.FieldLength = "100";
            qzlshField.IsNull = true;
            qzlshField.IsAdd = true;
            upQZFields.Add(qzlshField);

            UpdateField qzdbrField = new UpdateField();
            qzdbrField.FieldName = "QZDBR";
            qzdbrField.FieldType = "TEXT";
            qzdbrField.FieldLength = "100";
            qzdbrField.IsNull = true;
            qzdbrField.IsAdd = true;
            upQZFields.Add(qzdbrField);

            UpdateField qzdjrqField = new UpdateField();
            qzdjrqField.FieldName = "QZDJRQ";
            qzdjrqField.FieldType = "DATETIME";
            qzdjrqField.IsNull = true;
            qzdjrqField.IsAdd = true;
            upQZFields.Add(qzdjrqField);

            UpdateField sfqgdkField = new UpdateField();
            sfqgdkField.FieldName = "JZSFSH";
            sfqgdkField.FieldType = "BOOLEAN";
            sfqgdkField.IsNull = true;
            sfqgdkField.IsAdd = true;
            upQZFields.Add(sfqgdkField);

            UpdateField qzYSZSBHField = new UpdateField();
            qzYSZSBHField.FieldName = "YSZSBH";
            qzYSZSBHField.FieldType = "TEXT";
            qzYSZSBHField.FieldLength = "100";
            qzYSZSBHField.IsNull = true;
            qzYSZSBHField.IsAdd = true;
            upQZFields.Add(qzYSZSBHField);

            UpdateField qzfjField = new UpdateField();
            qzfjField.FieldName = "QZFJ";
            qzfjField.FieldType = "TEXT";
            qzfjField.IsNull = true;
            qzfjField.IsAdd = true;
            upQZFields.Add(qzfjField);
            upQZ.FieldList = upQZFields;

            return upQZ;
        }

        /// <summary>
        /// 确股权证
        /// </summary>
        private static UpgradeDatabase AddQGHTFields()
        {
            UpgradeDatabase upQZ = new UpgradeDatabase();
            upQZ.TableName = "QGCBJYQ_HT";
            List<UpdateField> upQZFields = new List<UpdateField>();

            UpdateField qzlshField = new UpdateField();
            qzlshField.FieldName = "CBHTBM";
            qzlshField.FieldType = "TEXT";
            qzlshField.FieldLength = "100";
            qzlshField.IsNull = true;
            qzlshField.IsAdd = true;
            upQZFields.Add(qzlshField);

            UpdateField fbfbsField = new UpdateField();
            fbfbsField.FieldName = "FBFBS";
            fbfbsField.FieldType = "TEXT";
            fbfbsField.FieldLength = "100";
            fbfbsField.IsNull = true;
            fbfbsField.IsAdd = true;
            upQZFields.Add(fbfbsField);

            UpdateField fbfmcField = new UpdateField();
            fbfmcField.FieldName = "FBFMC";
            fbfmcField.FieldType = "TEXT";
            fbfmcField.FieldLength = "100";
            fbfmcField.IsNull = true;
            fbfmcField.IsAdd = true;
            upQZFields.Add(fbfmcField);

            UpdateField dybmField = new UpdateField();
            dybmField.FieldName = "DYBM";
            dybmField.FieldType = "TEXT";
            dybmField.FieldLength = "100";
            dybmField.IsNull = true;
            dybmField.IsAdd = true;
            upQZFields.Add(dybmField);

            UpdateField cbfsField = new UpdateField();
            cbfsField.FieldName = "CBFS";
            cbfsField.FieldType = "TEXT";
            cbfsField.FieldLength = "100";
            cbfsField.IsNull = true;
            cbfsField.IsAdd = true;
            upQZFields.Add(cbfsField);

            UpdateField CBQXQField = new UpdateField();
            CBQXQField.FieldName = "CBQXQ";
            CBQXQField.FieldType = "DATETIME";
            CBQXQField.IsNull = true;
            CBQXQField.IsAdd = true;
            upQZFields.Add(CBQXQField);

            UpdateField CBQXZField = new UpdateField();
            CBQXZField.FieldName = "CBQXZ";
            CBQXZField.FieldType = "DATETIME";
            CBQXZField.IsNull = true;
            CBQXZField.IsAdd = true;
            upQZFields.Add(CBQXZField);

            UpdateField SHDCBFSField = new UpdateField();
            SHDCBFSField.FieldName = "SHDCBFS";
            SHDCBFSField.FieldType = "TEXT";
            SHDCBFSField.IsNull = true;
            SHDCBFSField.IsAdd = true;
            upQZFields.Add(SHDCBFSField);

            UpdateField SHDCBQXQField = new UpdateField();
            SHDCBQXQField.FieldName = "SHDCBQXQ";
            SHDCBQXQField.FieldType = "DATETIME";
            SHDCBQXQField.IsNull = true;
            SHDCBQXQField.IsAdd = true;
            upQZFields.Add(SHDCBQXQField);

            UpdateField SHDCBQXZField = new UpdateField();
            SHDCBQXZField.FieldName = "SHDCBQXZ";
            SHDCBQXZField.FieldType = "DATETIME";
            SHDCBQXZField.IsNull = true;
            SHDCBQXZField.IsAdd = true;
            upQZFields.Add(SHDCBQXZField);

            UpdateField CBYTField = new UpdateField();
            CBYTField.FieldName = "CBYT";
            CBYTField.FieldType = "TEXT";
            CBYTField.IsNull = true;
            CBYTField.IsAdd = true;
            upQZFields.Add(CBYTField);

            UpdateField SHDCBYTField = new UpdateField();
            SHDCBYTField.FieldName = "SHDCBYT";
            SHDCBYTField.FieldType = "TEXT";
            SHDCBYTField.IsNull = true;
            SHDCBYTField.IsAdd = true;
            upQZFields.Add(SHDCBYTField);

            UpdateField ELCBTZMJField = new UpdateField();
            ELCBTZMJField.FieldName = "ELCBTZMJ";
            ELCBTZMJField.FieldType = "NUMERIC";
            ELCBTZMJField.IsNull = true;
            ELCBTZMJField.IsAdd = true;
            upQZFields.Add(ELCBTZMJField);

            UpdateField JYQXField = new UpdateField();
            JYQXField.FieldName = "JYQX";
            JYQXField.FieldType = "TEXT";
            JYQXField.IsNull = true;
            JYQXField.IsAdd = true;
            upQZFields.Add(JYQXField);

            UpdateField FBFQDSJField = new UpdateField();
            FBFQDSJField.FieldName = "FBFQDSJ";
            FBFQDSJField.FieldType = "DATETIME";
            FBFQDSJField.IsNull = true;
            FBFQDSJField.IsAdd = true;
            upQZFields.Add(FBFQDSJField);

            UpdateField CBFQDSJField = new UpdateField();
            CBFQDSJField.FieldName = "CBFQDSJ";
            CBFQDSJField.FieldType = "DATETIME";
            CBFQDSJField.IsNull = true;
            CBFQDSJField.IsAdd = true;
            upQZFields.Add(CBFQDSJField);

            UpdateField JZJGRQField = new UpdateField();
            JZJGRQField.FieldName = "JZJGRQ";
            JZJGRQField.FieldType = "DATETIME";
            JZJGRQField.IsNull = true;
            JZJGRQField.IsAdd = true;
            upQZFields.Add(JZJGRQField);

            UpdateField CBHTFJField = new UpdateField();
            CBHTFJField.FieldName = "CBHTFJ";
            CBHTFJField.FieldType = "TEXT";
            CBHTFJField.IsNull = true;
            CBHTFJField.IsAdd = true;
            upQZFields.Add(CBHTFJField);

            UpdateField ZTField = new UpdateField();
            ZTField.FieldName = "ZT";
            ZTField.FieldType = "TEXT";
            ZTField.IsNull = true;
            ZTField.IsAdd = true;
            upQZFields.Add(ZTField);

            UpdateField CBJYQZHField = new UpdateField();
            CBJYQZHField.FieldName = "CBJYQZH";
            CBJYQZHField.FieldType = "TEXT";
            CBJYQZHField.IsNull = true;
            CBJYQZHField.IsAdd = true;
            upQZFields.Add(CBJYQZHField);

            UpdateField CBFBSField = new UpdateField();
            CBFBSField.FieldName = "CBFBS";
            CBFBSField.FieldType = "TEXT";
            CBFBSField.IsNull = true;
            CBFBSField.IsAdd = true;
            upQZFields.Add(CBFBSField);

            UpdateField CBFMCField = new UpdateField();
            CBFMCField.FieldName = "CBFMC";
            CBFMCField.FieldType = "TEXT";
            CBFMCField.IsNull = true;
            CBFMCField.IsAdd = true;
            upQZFields.Add(CBFMCField);

            UpdateField DECBFMCField = new UpdateField();
            DECBFMCField.FieldName = "DECBFMC";
            DECBFMCField.FieldType = "TEXT";
            DECBFMCField.IsNull = true;
            DECBFMCField.IsAdd = true;
            upQZFields.Add(DECBFMCField);

            UpdateField DECBFZZField = new UpdateField();
            DECBFZZField.FieldName = "DECBFZZ";
            DECBFZZField.FieldType = "TEXT";
            DECBFZZField.IsNull = true;
            DECBFZZField.IsAdd = true;
            upQZFields.Add(DECBFZZField);

            UpdateField RJMJField = new UpdateField();
            RJMJField.FieldName = "RJMJ";
            RJMJField.FieldType = "NUMERIC";
            RJMJField.IsNull = true;
            RJMJField.IsAdd = true;
            upQZFields.Add(RJMJField);

            UpdateField CBFZJHMField = new UpdateField();
            CBFZJHMField.FieldName = "CBFZJHM";
            CBFZJHMField.FieldType = "TEXT";
            CBFZJHMField.IsNull = true;
            CBFZJHMField.IsAdd = true;
            upQZFields.Add(CBFZJHMField);

            UpdateField CBFFRDBXMField = new UpdateField();
            CBFFRDBXMField.FieldName = "CBFFRDBXM";
            CBFFRDBXMField.FieldType = "TEXT";
            CBFFRDBXMField.IsNull = true;
            CBFFRDBXMField.IsAdd = true;
            upQZFields.Add(CBFFRDBXMField);

            UpdateField CBFFRDBZJLXField = new UpdateField();
            CBFFRDBZJLXField.FieldName = "CBFFRDBZJLX";
            CBFFRDBZJLXField.FieldType = "TEXT";
            CBFFRDBZJLXField.IsNull = true;
            CBFFRDBZJLXField.IsAdd = true;
            upQZFields.Add(CBFFRDBZJLXField);

            UpdateField CBFFRDBZJHMField = new UpdateField();
            CBFFRDBZJHMField.FieldName = "CBFFRDBZJHM";
            CBFFRDBZJHMField.FieldType = "TEXT";
            CBFFRDBZJHMField.IsNull = true;
            CBFFRDBZJHMField.IsAdd = true;
            upQZFields.Add(CBFFRDBZJHMField);

            UpdateField CBFFRDBDHHMField = new UpdateField();
            CBFFRDBDHHMField.FieldName = "CBFFRDBDHHM";
            CBFFRDBDHHMField.FieldType = "TEXT";
            CBFFRDBDHHMField.IsNull = true;
            CBFFRDBDHHMField.IsAdd = true;
            upQZFields.Add(CBFFRDBDHHMField);

            UpdateField DLRXMField = new UpdateField();
            DLRXMField.FieldName = "DLRXM";
            DLRXMField.FieldType = "TEXT";
            DLRXMField.IsNull = true;
            DLRXMField.IsAdd = true;
            upQZFields.Add(DLRXMField);

            UpdateField DLRZJLXField = new UpdateField();
            DLRZJLXField.FieldName = "DLRZJLX";
            DLRZJLXField.FieldType = "TEXT";
            DLRZJLXField.IsNull = true;
            DLRZJLXField.IsAdd = true;
            upQZFields.Add(DLRZJLXField);

            UpdateField DLRZJHMField = new UpdateField();
            DLRZJHMField.FieldName = "DLRZJHM";
            DLRZJHMField.FieldType = "TEXT";
            DLRZJHMField.IsNull = true;
            DLRZJHMField.IsAdd = true;
            upQZFields.Add(DLRZJHMField);

            UpdateField DLRDHHMField = new UpdateField();
            DLRDHHMField.FieldName = "DLRDHHM";
            DLRDHHMField.FieldType = "TEXT";
            DLRDHHMField.IsNull = true;
            DLRDHHMField.IsAdd = true;
            upQZFields.Add(DLRDHHMField);

            UpdateField CBFLXField = new UpdateField();
            CBFLXField.FieldName = "CBFLX";
            CBFLXField.FieldType = "TEXT";
            CBFLXField.IsNull = true;
            CBFLXField.IsAdd = true;
            upQZFields.Add(CBFLXField);

            UpdateField CJBZField = new UpdateField();
            CJBZField.FieldName = "CJBZ";
            CJBZField.FieldType = "BOOL";
            CJBZField.IsNull = true;
            CJBZField.IsAdd = true;
            upQZFields.Add(CJBZField);

            UpdateField ZLDMJField = new UpdateField();
            ZLDMJField.FieldName = "ZLDMJ";
            ZLDMJField.FieldType = "NUMERIC";
            ZLDMJField.IsNull = true;
            ZLDMJField.IsAdd = true;
            upQZFields.Add(ZLDMJField);

            UpdateField CBJEField = new UpdateField();
            CBJEField.FieldName = "CBJE";
            CBJEField.FieldType = "NUMERIC";
            CBJEField.IsNull = true;
            CBJEField.IsAdd = true;
            upQZFields.Add(CBJEField);

            UpdateField QZSQSBZField = new UpdateField();
            QZSQSBZField.FieldName = "QZSQSBZ";
            QZSQSBZField.FieldType = "TEXT";
            QZSQSBZField.IsNull = true;
            QZSQSBZField.IsAdd = true;
            upQZFields.Add(QZSQSBZField);

            UpdateField CJZField = new UpdateField();
            CJZField.FieldName = "CJZ";
            CJZField.FieldType = "TEXT";
            CJZField.IsNull = true;
            CJZField.IsAdd = true;
            upQZFields.Add(CJZField);

            UpdateField CJSJField = new UpdateField();
            CJSJField.FieldName = "CJSJ";
            CJSJField.FieldType = "DATETIME";
            CJSJField.IsNull = true;
            CJSJField.IsAdd = true;
            upQZFields.Add(CJSJField);

            UpdateField ZHXGZField = new UpdateField();
            ZHXGZField.FieldName = "ZHXGZ";
            ZHXGZField.FieldType = "TEXT";
            ZHXGZField.IsNull = true;
            ZHXGZField.IsAdd = true;
            upQZFields.Add(ZHXGZField);

            UpdateField ZHXGSJField = new UpdateField();
            ZHXGSJField.FieldName = "ZHXGSJ";
            ZHXGSJField.FieldType = "DATETIME";
            ZHXGSJField.IsNull = true;
            ZHXGSJField.IsAdd = true;
            upQZFields.Add(ZHXGSJField);

            UpdateField BZXXField = new UpdateField();
            BZXXField.FieldName = "BZXX";
            BZXXField.FieldType = "TEXT";
            BZXXField.IsNull = true;
            BZXXField.IsAdd = true;
            upQZFields.Add(BZXXField);

            UpdateField SCZMJField = new UpdateField();
            SCZMJField.FieldName = "SCZMJ";
            SCZMJField.FieldType = "NUMERIC";
            SCZMJField.IsNull = true;
            SCZMJField.IsAdd = true;
            upQZFields.Add(SCZMJField);

            UpdateField QQZMJField = new UpdateField();
            QQZMJField.FieldName = "QQZMJ";
            QQZMJField.FieldType = "NUMERIC";
            QQZMJField.IsNull = true;
            QQZMJField.IsAdd = true;
            upQZFields.Add(QQZMJField);

            UpdateField JDDZMJField = new UpdateField();
            JDDZMJField.FieldName = "JDDZMJ";
            JDDZMJField.FieldType = "NUMERIC";
            JDDZMJField.IsNull = true;
            JDDZMJField.IsAdd = true;
            upQZFields.Add(JDDZMJField);

            UpdateField HTSFKYField = new UpdateField();
            HTSFKYField.FieldName = "HTSFKY";
            HTSFKYField.FieldType = "BOOL";
            HTSFKYField.IsNull = true;
            HTSFKYField.IsAdd = true;
            upQZFields.Add(HTSFKYField);

            UpdateField GSJSField = new UpdateField();
            GSJSField.FieldName = "GSJS";
            GSJSField.FieldType = "TEXT";
            GSJSField.IsNull = true;
            GSJSField.IsAdd = true;
            upQZFields.Add(GSJSField);

            UpdateField GSJSRField = new UpdateField();
            GSJSRField.FieldName = "GSJSR";
            GSJSRField.FieldType = "TEXT";
            GSJSRField.IsNull = true;
            GSJSRField.IsAdd = true;
            upQZFields.Add(GSJSRField);

            UpdateField GSRQField = new UpdateField();
            GSRQField.FieldName = "GSRQ";
            GSRQField.FieldType = "DATETIME";
            GSRQField.IsNull = true;
            GSRQField.IsAdd = true;
            upQZFields.Add(GSRQField);

            UpdateField GSJGYJField = new UpdateField();
            GSJGYJField.FieldName = "GSJGYJ";
            GSJGYJField.FieldType = "TEXT";
            GSJGYJField.IsNull = true;
            GSJGYJField.IsAdd = true;
            upQZFields.Add(GSJGYJField);

            UpdateField CBFDBField = new UpdateField();
            CBFDBField.FieldName = "CBFDB";
            CBFDBField.FieldType = "TEXT";
            CBFDBField.IsNull = true;
            CBFDBField.IsAdd = true;
            upQZFields.Add(CBFDBField);

            UpdateField GSJGRQField = new UpdateField();
            GSJGRQField.FieldName = "GSJGRQ";
            GSJGRQField.FieldType = "DATETIME";
            GSJGRQField.IsNull = true;
            GSJGRQField.IsAdd = true;
            upQZFields.Add(GSJGRQField);

            UpdateField GSSHYJField = new UpdateField();
            GSSHYJField.FieldName = "GSSHYJ";
            GSSHYJField.FieldType = "TEXT";
            GSSHYJField.IsNull = true;
            GSSHYJField.IsAdd = true;
            upQZFields.Add(GSSHYJField);

            UpdateField GSSHRField = new UpdateField();
            GSSHRField.FieldName = "GSSHR";
            GSSHRField.FieldType = "TEXT";
            GSSHRField.IsNull = true;
            GSSHRField.IsAdd = true;
            upQZFields.Add(GSSHRField);

            UpdateField GSSHRQField = new UpdateField();
            GSSHRQField.FieldName = "GSSHRQ";
            GSSHRQField.FieldType = "DATETIME";
            GSSHRQField.IsNull = true;
            GSSHRQField.IsAdd = true;
            upQZFields.Add(GSSHRQField);

            UpdateField YLAField = new UpdateField();
            YLAField.FieldName = "YLA";
            YLAField.FieldType = "TEXT";
            YLAField.IsNull = true;
            YLAField.IsAdd = true;
            upQZFields.Add(YLAField);

            UpdateField YLBField = new UpdateField();
            YLBField.FieldName = "YLB";
            YLBField.FieldType = "TEXT";
            YLBField.IsNull = true;
            YLBField.IsAdd = true;
            upQZFields.Add(YLBField);

            UpdateField YLCField = new UpdateField();
            YLCField.FieldName = "YLC";
            YLCField.FieldType = "TEXT";
            YLCField.IsNull = true;
            YLCField.IsAdd = true;
            upQZFields.Add(YLCField);

            upQZ.FieldList = upQZFields;

            return upQZ;
        }

        /// <summary>
        /// 添加承包地字段
        /// </summary>
        /// <param name="isCBD"> true ZD_CBD ,false ZD_CBD_Mark</param>
        /// <returns></returns>
        private static UpgradeDatabase AddCBDFields(bool isCBD)
        {
            UpgradeDatabase upCBD = new UpgradeDatabase();
            if (isCBD)
                upCBD.TableName = "ZD_CBD";
            else
                upCBD.TableName = "ZD_CBD_Mark";
            List<UpdateField> upCBDFields = new List<UpdateField>();
            UpdateField gymjField = new UpdateField();
            gymjField.FieldName = "GYMJ";
            gymjField.FieldType = "TEXT";
            gymjField.FieldLength = "100";
            gymjField.IsNull = true;
            gymjField.IsAdd = true;
            upCBDFields.Add(gymjField);

            UpdateField htmjField = new UpdateField();
            htmjField.FieldName = "HTMJ";
            htmjField.FieldType = "TEXT";
            htmjField.FieldLength = "100";
            htmjField.IsNull = true;
            htmjField.IsAdd = true;
            upCBDFields.Add(htmjField);

            UpdateField gqslField = new UpdateField();
            gqslField.FieldName = "GQSL";
            gqslField.FieldType = "NUMERIC";
            gqslField.IsNull = true;
            gqslField.IsAdd = true;
            upCBDFields.Add(gqslField);

            UpdateField rjgqslField = new UpdateField();
            rjgqslField.FieldName = "RJGQSL";
            rjgqslField.FieldType = "TEXT";
            rjgqslField.IsNull = true;
            rjgqslField.IsAdd = true;
            upCBDFields.Add(rjgqslField);

            UpdateField lhhmjdField = new UpdateField();
            lhhmjdField.FieldName = "LHHMJD";
            lhhmjdField.FieldType = "TEXT";
            lhhmjdField.IsNull = true;
            lhhmjdField.IsAdd = true;
            upCBDFields.Add(lhhmjdField);

            UpdateField lhhmjgField = new UpdateField();
            lhhmjgField.FieldName = "LHHMJG";
            lhhmjgField.FieldType = "TEXT";
            lhhmjgField.IsNull = true;
            lhhmjgField.IsAdd = true;
            upCBDFields.Add(lhhmjgField);

            UpdateField xsField = new UpdateField();
            xsField.FieldName = "XS";
            xsField.FieldType = "TEXT";
            xsField.IsNull = true;
            xsField.IsAdd = true;
            upCBDFields.Add(xsField);

            UpdateField lhgsField = new UpdateField();
            lhgsField.FieldName = "LHGS";
            lhgsField.FieldType = "NUMERIC";
            lhgsField.IsNull = true;
            lhgsField.IsAdd = true;
            upCBDFields.Add(lhgsField);

            UpdateField ylgsField = new UpdateField();
            ylgsField.FieldName = "YLGS";
            ylgsField.FieldType = "NUMERIC";
            ylgsField.IsNull = true;
            ylgsField.IsAdd = true;
            upCBDFields.Add(ylgsField);

            UpdateField lhmjField = new UpdateField();
            lhmjField.FieldName = "LHMJ";
            lhmjField.FieldType = "NUMERIC";
            lhmjField.IsNull = true;
            lhmjField.IsAdd = true;
            upCBDFields.Add(lhmjField);

            UpdateField ylmjField = new UpdateField();
            ylmjField.FieldName = "YLMJ";
            ylmjField.FieldType = "NUMERIC";
            ylmjField.IsNull = true;
            ylmjField.IsAdd = true;
            upCBDFields.Add(ylmjField);

            UpdateField sfqgdkField = new UpdateField();
            sfqgdkField.FieldName = "SFQGDK";
            sfqgdkField.FieldType = "BOOLEAN";
            sfqgdkField.IsNull = true;
            sfqgdkField.IsAdd = true;
            upCBDFields.Add(sfqgdkField);

            UpdateField qsxz = new UpdateField(); // 权属性质
            qsxz.FieldName = "QSXZ";
            qsxz.FieldType = "TEXT";
            qsxz.IsNull = true;
            qsxz.IsAdd = true;
            qsxz.FieldLength = "100";
            upCBDFields.Add(qsxz);

            UpdateField xkxxxgyj = new UpdateField(); // 权属性质
            qsxz.FieldName = "DKXXXGYJ";
            qsxz.FieldType = "TEXT";
            qsxz.IsNull = true;
            qsxz.IsAdd = true;
            qsxz.FieldLength = "999";
            upCBDFields.Add(xkxxxgyj);

            UpdateField ybmj = new UpdateField(); // 权属性质
            qsxz.FieldName = "YBMJ";
            qsxz.FieldType = "DECIMAL";
            qsxz.IsNull = true;
            qsxz.IsAdd = true;
            qsxz.FieldLength = "13";
            upCBDFields.Add(ybmj);
            upCBD.FieldList = upCBDFields;

            return upCBD;
        }

        /// <summary>
        /// 添加承包方字段
        /// </summary>
        private static UpgradeDatabase AddCBFFields()
        {
            UpgradeDatabase upCBF = new UpgradeDatabase();
            upCBF.TableName = "QLR_CBF";
            List<UpdateField> upCBFFields = new List<UpdateField>();
            UpdateField qgzsField = new UpdateField();
            qgzsField.FieldName = "QGZS";
            qgzsField.FieldType = "NUMERIC";
            qgzsField.IsNull = true;
            qgzsField.IsAdd = true;
            upCBFFields.Add(qgzsField);

            UpdateField qgzmjField = new UpdateField();
            qgzmjField.FieldName = "QGZMJ";
            qgzmjField.FieldType = "NUMERIC";
            qgzmjField.IsNull = true;
            qgzmjField.IsAdd = true;
            upCBFFields.Add(qgzmjField);

            UpdateField sfgnField = new UpdateField();
            sfgnField.FieldName = "SFGN";
            sfgnField.FieldType = "BOOLEAN";
            sfgnField.IsNull = true;
            sfgnField.IsAdd = true;
            upCBFFields.Add(sfgnField);

            upCBF.FieldList = upCBFFields;
            return upCBF;
        }

        /// <summary>
        /// 添加二轮承包方字段
        /// </summary>
        private static UpgradeDatabase AddQLRELTZFields()
        {
            UpgradeDatabase upQLRELTZ = new UpgradeDatabase();
            upQLRELTZ.TableName = "QLR_ELTZ";
            List<UpdateField> upQLRELTZFields = new List<UpdateField>();
            UpdateField qgzsField = new UpdateField();
            qgzsField.FieldName = "QGZS";
            qgzsField.FieldType = "NUMERIC";
            qgzsField.IsNull = true;
            qgzsField.IsAdd = true;
            upQLRELTZFields.Add(qgzsField);

            UpdateField qgzmjField = new UpdateField();
            qgzmjField.FieldName = "QGZMJ";
            qgzmjField.FieldType = "NUMERIC";
            qgzmjField.IsNull = true;
            qgzmjField.IsAdd = true;
            upQLRELTZFields.Add(qgzmjField);

            UpdateField sfgnField = new UpdateField();
            sfgnField.FieldName = "SFGN";
            sfgnField.FieldType = "BOOLEAN";
            sfgnField.IsNull = true;
            sfgnField.IsAdd = true;
            upQLRELTZFields.Add(sfgnField);

            upQLRELTZ.FieldList = upQLRELTZFields;
            return upQLRELTZ;
        }

        /// <summary>
        /// 添加二轮台账字段
        /// </summary>
        private static UpgradeDatabase AddELTZFields()
        {
            UpgradeDatabase upELTZ = new UpgradeDatabase();
            upELTZ.TableName = "ZD_ELTZ";
            List<UpdateField> upELTZFields = new List<UpdateField>();
            UpdateField gymjField = new UpdateField();
            gymjField.FieldName = "GYMJ";
            gymjField.FieldType = "TEXT";
            gymjField.FieldLength = "100";
            gymjField.IsNull = true;
            gymjField.IsAdd = true;
            upELTZFields.Add(gymjField);

            UpdateField htmjField = new UpdateField();
            htmjField.FieldName = "HTMJ";
            htmjField.FieldType = "TEXT";
            htmjField.FieldLength = "100";
            htmjField.IsNull = true;
            htmjField.IsAdd = true;
            upELTZFields.Add(htmjField);

            UpdateField gqslField = new UpdateField();
            gqslField.FieldName = "GQSL";
            gqslField.FieldType = "NUMERIC";
            gqslField.IsNull = true;
            gqslField.IsAdd = true;
            upELTZFields.Add(gqslField);

            UpdateField rjgqslField = new UpdateField();
            rjgqslField.FieldName = "RJGQSL";
            rjgqslField.FieldType = "TEXT";
            rjgqslField.IsNull = true;
            rjgqslField.IsAdd = true;
            upELTZFields.Add(rjgqslField);

            UpdateField lhhmjdField = new UpdateField();
            lhhmjdField.FieldName = "LHHMJD";
            lhhmjdField.FieldType = "TEXT";
            lhhmjdField.IsNull = true;
            lhhmjdField.IsAdd = true;
            upELTZFields.Add(lhhmjdField);

            UpdateField lhhmjgField = new UpdateField();
            lhhmjgField.FieldName = "LHHMJG";
            lhhmjgField.FieldType = "TEXT";
            lhhmjgField.IsNull = true;
            lhhmjgField.IsAdd = true;
            upELTZFields.Add(lhhmjgField);

            UpdateField xsField = new UpdateField();
            xsField.FieldName = "XS";
            xsField.FieldType = "TEXT";
            xsField.IsNull = true;
            xsField.IsAdd = true;
            upELTZFields.Add(xsField);

            UpdateField lhgsField = new UpdateField();
            lhgsField.FieldName = "LHGS";
            lhgsField.FieldType = "NUMERIC";
            lhgsField.IsNull = true;
            lhgsField.IsAdd = true;
            upELTZFields.Add(lhgsField);

            UpdateField ylgsField = new UpdateField();
            ylgsField.FieldName = "YLGS";
            ylgsField.FieldType = "NUMERIC";
            ylgsField.IsNull = true;
            ylgsField.IsAdd = true;
            upELTZFields.Add(ylgsField);

            UpdateField lhmjField = new UpdateField();
            lhmjField.FieldName = "LHMJ";
            lhmjField.FieldType = "NUMERIC";
            lhmjField.IsNull = true;
            lhmjField.IsAdd = true;
            upELTZFields.Add(lhmjField);

            UpdateField ylmjField = new UpdateField();
            ylmjField.FieldName = "YLMJ";
            ylmjField.FieldType = "NUMERIC";
            ylmjField.IsNull = true;
            ylmjField.IsAdd = true;
            upELTZFields.Add(ylmjField);

            UpdateField sfqgdkField = new UpdateField();
            sfqgdkField.FieldName = "SFQGDK";
            sfqgdkField.FieldType = "BOOLEAN";
            sfqgdkField.IsNull = true;
            sfqgdkField.IsAdd = true;
            upELTZFields.Add(sfqgdkField);

            upELTZ.FieldList = upELTZFields;

            return upELTZ;
        }

        /// <summary>
        /// 添加权属关系字段
        /// </summary>
        private static UpgradeDatabase AddQSGXFields()
        {
            UpgradeDatabase upQSGX = new UpgradeDatabase();
            upQSGX.TableName = "QSGX";
            List<UpdateField> upQSGXFields = new List<UpdateField>();
            UpdateField cbfidField = new UpdateField();
            cbfidField.FieldName = "CBFID";
            cbfidField.FieldType = "TEXT";
            cbfidField.IsNull = false;
            cbfidField.IsAdd = true;
            upQSGXFields.Add(cbfidField);

            UpdateField dkidField = new UpdateField();
            dkidField.FieldName = "DKID";
            dkidField.FieldType = "TEXT";
            dkidField.IsNull = false;
            dkidField.IsAdd = true;
            upQSGXFields.Add(dkidField);

            UpdateField szdyField = new UpdateField();
            szdyField.FieldName = "SZDY";
            szdyField.FieldType = "TEXT";
            szdyField.IsNull = true;
            szdyField.IsAdd = true;
            upQSGXFields.Add(szdyField);

            UpdateField lhhmjField = new UpdateField();
            lhhmjField.FieldName = "LHHMJ";
            lhhmjField.FieldType = "NUMERIC";
            lhhmjField.IsNull = true;
            lhhmjField.IsAdd = true;
            upQSGXFields.Add(lhhmjField);

            UpdateField tzmjField = new UpdateField();
            tzmjField.FieldName = "TZMJ";
            tzmjField.FieldType = "NUMERIC";
            tzmjField.IsNull = true;
            tzmjField.IsAdd = true;
            upQSGXFields.Add(tzmjField);

            UpdateField xsField = new UpdateField();
            xsField.FieldName = "XS";
            xsField.FieldType = "NUMERIC";
            xsField.IsNull = true;
            xsField.IsAdd = true;
            upQSGXFields.Add(xsField);

            upQSGX.FieldList = upQSGXFields;
            return upQSGX;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        public static List<UpgradeDatabase> DeserializeUpgradeDatabaseInfo()
        {
            try
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\UpgradeDatabaseInformation20161031.xml";
                if (!File.Exists(fileName))
                {
                    return new List<UpgradeDatabase>();
                }
                List<UpgradeDatabase> tableList = ToolSerialization.DeserializeXml(fileName, typeof(List<UpgradeDatabase>)) as List<UpgradeDatabase>;
                if (tableList == null)
                {
                    return new List<UpgradeDatabase>();
                }
                else
                {
                    return tableList;
                }
            }
            catch
            {
                return new List<UpgradeDatabase>();
            }
        }

        #endregion 升级数据库
    }

    /// <summary>
    /// 升级数据库表
    /// </summary>
    public class UpgradeDatabase
    {
        #region Filds

        #endregion Filds

        #region Property

        /// <summary>
        /// 表格名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 字段集合
        /// </summary>
        public List<UpdateField> FieldList { get; set; }

        #endregion Property

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public UpgradeDatabase()
        { }

        #endregion Ctor
    }

    /// <summary>
    /// 升级数据库字段
    /// </summary>
    public class UpdateField
    {
        #region Properties

        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string FieldType { get; set; }

        /// <summary>
        /// 字段长度
        /// </summary>
        public string FieldLength { get; set; }

        /// <summary>
        /// 是否可空
        /// </summary>
        public bool IsNull { get; set; }

        /// <summary>
        /// 是否添加
        /// </summary>
        public bool IsAdd { get; set; }

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public UpdateField()
        {
            IsNull = true;
            IsAdd = true;
        }

        #endregion Ctor
    }

    /// <summary>
    /// 升级数据执行
    /// </summary>
    public class UpdateDatabase
    {
        #region Ctor

        #endregion Ctor

        #region Method

        /// <summary>
        /// 升级数据库
        /// </summary>
        public bool UpgradeDatabase(IDbContext dbContext, List<UpgradeDatabase> tableList, IDataSource dataSource)
        {
            try
            {
                var elements = dbContext.DataSource.CreateSchema().GetElements();
                var srid = dbContext.DataSource.CreateSchema().GetElementSpatialReference(
                 ObjectContext.Create(typeof(YuLinTu.Library.Entity.Zone)).Schema,
                 ObjectContext.Create(typeof(YuLinTu.Library.Entity.Zone)).TableName).WKID;
                var query = dbContext.CreateQuery();
                string commandStrTableAdd = string.Empty;
                string commandStrFieldAdd = string.Empty;
                string commandStrUpdate = string.Empty;
                //var str = $"PRAGMA  table_info(ZD_CBD);";
                //var schema = dbContext.DataSource.CreateSchema();
                //var result = schema.GetElementProperties(null, "ZD_CBD");
                //List<string> tempList = new List<string>();
                //result.ForEach(x =>
                //{
                //    tempList.Add(x.AliasName);
                //});
                //if (tempList.Contains("YBMJ")||tempList.Contains("DKXXXGYJ"))
                //{

                //}
                //else
                //{
                //    ((IDbContext)dataSource).ExecuteBySQL($"ALTER TABLE ZD_CBD ADD DKXXXGYJ TEXT;");
                //    ((IDbContext)dataSource).ExecuteBySQL($"ALTER TABLE ZD_CBD ADD YBMJ DECIMAL;");
                //}

                foreach (var table in tableList)
                {
                    if (!elements.Any(c => !string.IsNullOrEmpty(c.TableName) && c.TableName == table.TableName))
                    {
                        //新增表 Create table JJ(ID TEXT PRIMARY KEY ,name char);
                        if (table.FieldList.Count > 0)
                        {
                            commandStrTableAdd = string.Format("Create table {0}({1} {2} {3},", table.TableName, "ID", "TEXT", "PRIMARY KEY");
                            for (int i = 0; i < table.FieldList.Count; i++)
                            {
                                if (i == table.FieldList.Count - 1)
                                {
                                    commandStrTableAdd += string.Format("{0} {1})", table.FieldList[i].FieldName, table.FieldList[i].FieldType);
                                }
                                else
                                {
                                    commandStrTableAdd += string.Format("{0} {1},", table.FieldList[i].FieldName, table.FieldList[i].FieldType);
                                }
                            }
                        }
                        else
                        {
                            commandStrTableAdd = string.Format("Create table {0}({1} {2} {3})", table.TableName, "ID", "TEXT", "PRIMARY KEY");
                        }

                        query.CommandContext.CommandText.Append(commandStrTableAdd);
                        query.Execute();
                        query.CommandContext.CommandText.Clear();
                        continue;
                    }
                    var elementProperties = dbContext.DataSource.CreateSchema().GetElementPropertyNames(null, table.TableName);
                    if (elementProperties == null)
                        continue;
                    foreach (var field in table.FieldList)
                    {
                        if (elementProperties.Any(c => c == field.FieldName))
                        {
                            continue;
                        }
                        if (field.FieldName == "Shape")
                        {
                            commandStrFieldAdd = string.Format("Select AddGeometryColumn('{0}', 'Shape',{1}, 'GEOMETRY', 'XY')", table.TableName, srid);
                            query.CommandContext.CommandText.Append(commandStrFieldAdd);
                            query.Execute();
                            query.CommandContext.CommandText.Clear();
                        }
                        else
                        {
                            //添加字段
                            commandStrFieldAdd = string.Format("Alter table {0} Add {1} {2}", table.TableName, field.FieldName, field.FieldType);
                            query.CommandContext.CommandText.Append(commandStrFieldAdd);
                            query.Execute();
                            query.CommandContext.CommandText.Clear();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var dd = e.Message;
                return false;
            }
            return true;
        }

        #endregion Method
    }
}