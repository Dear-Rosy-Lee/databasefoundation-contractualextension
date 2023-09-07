using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using YuLinTu.Data;
using YuLinTu;
using System.Xml.Linq;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// Ҫ�����ʹ���
    /// </summary>
    [Serializable]
    public class FeatureTypeCode
    {
        #region Filds

        public const string JCDLXX = "100000";// ����������ϢҪ��a
        public const string DWJC = "110000";// ��λ����
        public const string KZD = "111000";// ���Ƶ�
        public const string KZDZJ = "112000";// ���Ƶ�ע��
        public const string JJYGXQY = "160000";// �������Ͻ����b
        public const string GXQYHJ = "161000";// ��Ͻ���򻮽�
        public const string QYJX = "161051";// �������
        public const string QYZJ = "161052";// ����ע��
        public const string GXQY = "162000";// ��Ͻ����
        public const string XJXZQ = "162010";// �ؼ�������
        public const string XJQY = "162020";// �缶����
        public const string CJQY = "162030";// �弶����
        public const string ZJQY = "162040";// �鼶����
        public const string QTDW = "190000";// ��������c
        public const string DZDW = "196011";// ��״����
        public const string DZDWZJ = "196012";// ��״����ע��
        public const string XZDW = "196021";// ��״����
        public const string XZDWZJ = "196022";// ��״����ע��
        public const string MZDW = "196031";// ��״����
        public const string MZDWZJ = "196032";// ��״����ע��
        public const string NCTDQS = "200000";// ũ������Ȩ��Ҫ��
        public const string CBD = "210000";// �а��ؿ�Ҫ��
        public const string DK = "211011";// �ؿ�
        public const string DKZJ = "211012";// �ؿ�ע��
        public const string JZD = "211021";// ��ַ��
        public const string JZDZJ = "211022";// ��ַ��ע��
        public const string JZX = "211031";// ��ַ��
        public const string JZXZJ = "211032";// ��ַ��ע��
        public const string JBNT = "250000";// ����ũ��Ҫ��
        public const string JBNTBHQY = "251000";// ����ũ�ﱣ������
        public const string JBNTBHQ = "251100";// ����ũ�ﱣ����
        public const string SGSJ = "300000";// դ������
        public const string SZZSYXT = "310000";// ��������Ӱ��ͼ
        public const string SZSGDT = "320000";// ����դ���ͼ
        public const string QTSGSJ = "390000";// ����դ������

        #endregion

        #region Ctor

        public FeatureTypeCode()
        {
        }

        #endregion

    }
}