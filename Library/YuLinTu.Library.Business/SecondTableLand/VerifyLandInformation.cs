/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 检查二轮台账调查表操作
    /// </summary>
    public class VerifyLandInformation
    {
        #region Fields

        private List<LandFamily> landFamilys;
        private LandImportDefine landDefine;//导入定义
        private string errorInformation = string.Empty;//错误信息
        private string warnInformation = string.Empty;//警告信息

        #endregion

        #region Properties

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorInformation
        {
            get { return errorInformation; }
            set { errorInformation = value; }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string WarnInformation
        {
            get { return warnInformation; }
            set { warnInformation = value; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="landFamily">户地对象</param>
        public VerifyLandInformation(List<LandFamily> landFamilys)
        {
            this.landFamilys = landFamilys;
            InitalizeInnerData();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitalizeInnerData()
        {
            string filePath = Application.StartupPath + @"\Config\" + "LandImportDefine.xml";
            if (File.Exists(filePath))
            {
                landDefine = ToolSerialization.DeserializeXml(filePath, typeof(LandImportDefine)) as LandImportDefine;
            }
            if (landDefine == null)
            {
                landDefine = new LandImportDefine();
            }
        }

        #endregion

        #region Method—Check

        /// <summary>
        /// 检查导入数据
        /// </summary>
        public bool CheckImportSecondTableData()
        {
            bool check = true;
            for (int i = 0; i < landFamilys.Count; i++)
            {
                LandFamily onefamily = landFamilys[i];
                if (string.IsNullOrEmpty(onefamily.TableFamily.Name))
                {
                    continue;
                }
                for (int j = i + 1; j < landFamilys.Count; j++)
                {
                    LandFamily twofamily = landFamilys[j];
                    if (onefamily.TableFamily.Name == twofamily.TableFamily.Name && landDefine.SecondNameIndex > 0)
                    {
                        string warnInfo = string.Format("承包方{0}在表中重复存在!", onefamily.TableFamily.Name);
                        RecordErrorInformation(warnInfo);
                    }
                    if (onefamily.TableFamily.Name == twofamily.TableFamily.Name && onefamily.TableFamily.Number != twofamily.TableFamily.Number && landDefine.SecondNameIndex > 0)
                    {
                        string warnInfo = string.Format("承包方{0}在表中重复存在!", onefamily.TableFamily.Name);
                        RecordErrorInformation(warnInfo);
                    }
                    if (onefamily.TableFamily.Name == twofamily.TableFamily.Name && onefamily.TableFamily.Number == twofamily.TableFamily.Number && !string.IsNullOrEmpty(onefamily.TableFamily.Number) && landDefine.SecondNameIndex > 0)
                    {
                        string errorInfo = string.Format("承包方{0}在表中重复存在，并且身份证号码{1}相同!", onefamily.TableFamily.Name, onefamily.TableFamily.Number);
                        RecordErrorInformation(errorInfo);
                        check = false;
                    }
                    for (int l = 0; l < onefamily.Persons.Count; l++)
                    {
                        Person per = onefamily.Persons[l];
                        if (string.IsNullOrEmpty(per.ICN))
                        {
                            continue;
                        }
                        for (int k = l + 1; k < onefamily.Persons.Count; k++)
                        {
                            Person pe = onefamily.Persons[k];
                            if (string.IsNullOrEmpty(pe.ICN))
                            {
                                continue;
                            }
                            if (pe.ICN == per.ICN && landDefine.SecondNumberIcnIndex > 0)
                            {
                                string errorInfo = string.Format("家庭成员{0}身份证号码{1}与家庭成员{2}身份证号码{3}重复!", per.Name, per.ICN, pe.Name, pe.ICN);
                                RecordErrorInformation(errorInfo);
                                check = false;
                            }
                        }
                    }
                    foreach (Person p1 in onefamily.Persons)
                    {
                        if (string.IsNullOrEmpty(p1.ICN))
                        {
                            continue;
                        }
                        foreach (Person p2 in twofamily.Persons)
                        {
                            if (string.IsNullOrEmpty(p2.ICN))
                            {
                                continue;
                            }
                            if (p1.ICN == p2.ICN && landDefine.SecondNumberIcnIndex > 0)
                            {
                                string errorInfo = string.Format("家庭成员{0}身份证号码{1}与家庭成员{2}身份证号码{3}重复!", p1.Name, p1.ICN, p2.Name, p2.ICN);
                                RecordErrorInformation(errorInfo);
                                check = false;
                            }
                        }
                    }
                }
            }
            return check;
        }

        #endregion

        #region Method—Error

        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="errorInfo">错误信息</param>
        private void RecordErrorInformation(string errorInfo)
        {
            if (string.IsNullOrEmpty(errorInformation) || errorInformation.IndexOf(errorInfo) < 0)
            {
                errorInformation += errorInfo;
            }
        }

        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="errorInfo">错误信息</param>
        private void RecordWareInformation(string warnInfo)
        {
            if (string.IsNullOrEmpty(warnInformation) || warnInformation.IndexOf(warnInfo) < 0)
            {
                warnInformation += warnInfo;
            }
        }

        #endregion
    }
}
