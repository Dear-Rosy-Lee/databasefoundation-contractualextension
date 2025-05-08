/*
 * (C) 2016 鱼鳞图公司版权所有，保留所有权利
 * http://www.yulintu.com
 *
 * CLR 版本：   4.0.30319.34014            最低的 Framework 版本：4.0
 * 文 件 名：   Aspect
 * 创 建 人：   颜学铭
 * 创建时间：   2017/4/14 11:03:28
 * 版    本：   1.0.0
 * 备注描述：
 * 修订历史：
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.NetAux.CglLib;

namespace YuLinTu.Library.Aux
{
    /// <summary>
    /// 坡向：y轴反方向为0顺时针旋转的角度。（含义：0表示正北方，π/2表示正东方，π表示正南方，3π/2表示正西方）
    /// 单位：弧度
    /// </summary>
    public struct Aspect
    {
        /// <summary>
        /// 无意义的坡向
        /// </summary>
        public static readonly Aspect Empty;
        /// <summary>
        /// 值 单位：弧度，范围：[0~2π)
        /// </summary>
        private double _value;
        /// <summary>
        /// 值 单位：弧度。返回值范围：[0~2π)，设置值范围为任意弧度值
        /// </summary>
        public double Value
        {
            get { return _value; }
            set
            {
                _value = ConvertToValidValue(value);
            }
        }
        /// <summary>
        /// 用给定的弧度值构造一个新的实例
        /// </summary>
        /// <param name="value">单位：弧度，接受任意弧度值</param>
        public Aspect(double value)
        {
            this._value = ConvertToValidValue(value);
        }
             
        /// <summary>
        /// 根据起点坐标和终点坐标构造对象
        /// </summary>
        /// <param name="fromX">起点X</param>
        /// <param name="fromY">起点Y</param>
        /// <param name="toX">终点X</param>
        /// <param name="toY">终点Y</param>
        public void Assign(double fromX, double fromY, double toX, double toY)
        {
            this._value = ToValue(fromX, fromY, toX, toY);           
        }

        public override string ToString()
        {
            return _value.ToString();
        }
       
        /// <summary>
        /// 将给定的弧度值转换为一个有效的方向值[0~2π)之间
        /// </summary>
        /// <param name="radianValue">任意弧度值</param>
        /// <returns>[0~2π)之间</returns>
        public static double ConvertToValidValue(double radianValue)
        {
            return CglRadians.NormalizePositive(radianValue);
        }
        
        private static double ToValue(double fromX, double fromY, double toX, double toY)
        {
            double value = 0;
            double dx = toX - fromX;
            double dy = toY - fromY;
            if (dy == 0)
            {
                if (toX > fromX)
                   value = Math.PI / 2.0;
                else
                   value = 3 * Math.PI / 2.0;
            }
            else if (dx == 0)
            {
                if (toY > fromY)
                    value = 0;
                else
                    value = Math.PI;
            }
            else
            {
                value = Math.Abs(Math.Atan(dx / dy));
                if (toY < fromY)
                {
                    if (toX > fromX)
                        value = Math.PI - value;
                    else
                        value = Math.PI + value;
                }
                else
                {
                    if (toX < fromX)
                    {
                        value = 2 * Math.PI - value;
                    }
                }
            }
            return value;
        }


        /**
         * 将方位角转换为文本
         * @param azimuth 
         * @return
         */
        public String toAzimuthString()
        {
            var v = Value * 180 / Math.PI;
            if (v < 22.5 || v > 360 - 22.5)
            {
                return "北";
            }
            if (v >= 22.5 && v < 90 - 22.5)
            {
                return "东北";
            }
            if (v >= 90 - 22.5 && v < 90 + 22.5)
            {
                return "东";
            }
            if (v >= 90 + 22.5 && v < 180 - 22.5)
            {
                return "东南";
            }
            if (v >= 180 - 22.5 && v < 180 + 22.5)
            {
                return "南";
            }
            if (v >= 180 + 22.5 && v < 270 - 22.5)
            {
                return "西南";
            }
            if (v >= 270 - 22.5 && v < 270 + 22.5)
            {
                return "西";
            }
            return "西北";                       
           
        }
    }
}
