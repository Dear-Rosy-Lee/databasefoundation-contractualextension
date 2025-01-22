using System;
using System.Collections;
using System.Collections.Generic;

namespace YuLinTu.Component.CoordinateTransformTask
{
    internal class CurveFitHelper
    {
        ///<summary>
        ///用最小二乘法拟合二元多次曲线（点集内曲线不一定单调）
        ///</summary>
        ///<param name="arrX">已知点的x坐标集合</param>
        ///<param name="arrY">已知点的y坐标集合</param>
        ///<param name="length">已知点的个数</param>
        ///<param name="dimension">方程的最高次数</param>
        public static double[] MultiLine(double[] arrX, double[] arrY, int length, int dimension)//二元多次线性方程拟合曲线
        {

            int n = dimension + 1;                 //dimension次方程需要求 dimension+1个 系数
            double[,] Guass = new double[n, n + 1];     //高斯矩阵(增广矩阵) 例如：y=a0+a1*x+a2*x*x
            for (int i = 0; i < n; i++)
            {
                int j;
                for (j = 0; j < n; j++)
                {
                    Guass[i, j] = SumArr(arrX, j + i, length); //法矩阵
                }
                Guass[i, j] = SumArr(arrX, i, arrY, 1, length);//法矩阵加一列生成增广矩阵
            }

            return ComputGauss(Guass, n);

        }

        /// <summary>
        /// 求三元一次方程组
        /// </summary>
        /// <param name="xParams">三个点的X坐标</param>
        /// <param name="yParams">三个点的Y坐标</param>
        /// <returns>返回a,b,c</returns>
        public static double[] SolutionLinearEqations(double[] xParams, double[] yParams)
        {
            double a = Math.Pow(xParams[0], 2);
            double b = xParams[0];
            double c = 1;
            double a1 = yParams[0];

            double d = Math.Pow(xParams[1], 2);
            double e = xParams[1];
            double f = 1;
            double b1 = yParams[1];

            double g = Math.Pow(xParams[2], 2);
            double h = xParams[2];
            double i = 1;
            double c1 = yParams[2];

            double A = a * (e * i - h * f) + b * (g * f - d * i) + c * (d * h - g * e);

            double x = (a1 * (e * i - h * f) + b1 * (h * c - b * i) + c1 * (b * f - e * c)) / A;
            double y = (a1 * (g * f - d * i) + b1 * (a * i - g * c) + c1 * (d * c - a * f)) / A;
            double z = (a1 * (d * h - g * e) + b1 * (g * b - a * h) + c1 * (a * e - b * d)) / A;

            return new double[] { x, y, z };
        }


        /// <summary>
        /// 拟合二元多次曲线过原点（截距为0）
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <param name="length"></param>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public static double[] MultiLineToOrigin(double[] arrX, double[] arrY, int length, int dimension)
        {
            //使点集单调
            //Array.Sort(arrX);
            //Array.Reverse(arrX);
            //Array.Sort(arrY);
            //Array.Reverse(arrY);

            int n = dimension;                 //dimension次方程需要求 dimension个 系数
            double[,] Guass = new double[n, n + 1];     //高斯矩阵(增广矩阵) 例如：y=a1*x+a2*x*x
            for (int i = 0; i < n; i++)
            {
                int j;
                for (j = 0; j < n; j++)
                {
                    Guass[i, j] = SumArr(arrX, j + i + 2, length); //法矩阵
                }
                Guass[i, j] = SumArr(arrX, i + 1, arrY, 1, length);//法矩阵加一列生成增广矩阵
            }

            return ComputGauss(Guass, n);

        }
        /// <summary>
        /// 拟合一元二次曲线过定点（x,y）  A*x*x+B*x+C=y
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <param name="length"></param>
        /// <param name="dimension"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double[] MultiLineToFixedPoint(double[] arrX, double[] arrY, int length, int dimension, double x, double y)
        {
            int n = dimension;
            double[,] Guass = new double[n, n + 1];
            double x00 = 0, x01 = 0, x10 = 0, x11 = 0;
            double y0 = 0, y1 = 0;
            for (int i = 0; i < length; i++)
            {
                x00 += Math.Pow(arrX[i] - x, 2);
                x01 += (arrX[i] * arrX[i] - x * x) * (arrX[i] - x);
                x10 += (arrX[i] * arrX[i] - x * x) * (arrX[i] - x);
                x11 += Math.Pow((arrX[i] * arrX[i] - x * x), 2);

                y1 += (arrX[i] * arrX[i] - x * x) * (arrY[i] - y);
                y0 += (arrX[i] - x) * (arrY[i] - y);
            }
            Guass[0, 0] = x00;
            Guass[0, 1] = x01;
            Guass[1, 0] = x10;
            Guass[1, 1] = x11;

            Guass[0, 2] = y0;
            Guass[1, 2] = y1;

            double[] result = ComputGauss(Guass, n);//得到A,B

            //根据A,B已知算出C
            double B = result[0];
            double A = result[1];
            double C = y - x * x * A - x * B;

            double[] number = new double[3];
            number[0] = C;
            number[1] = B;
            number[2] = A;

            return number;
        }
        private static double SumArr(double[] arr, int n, int length) //求数组的元素的n次方的和
        {
            double s = 0;
            for (int i = 0; i < length; i++)
            {
                if (arr[i] != 0 || n != 0)
                    s = s + Math.Pow(arr[i], n);
                else
                    s = s + 1;
            }
            return s;
        }
        private static double SumArr(double[] arr1, int n1, double[] arr2, int n2, int length)
        {
            double s = 0;
            for (int i = 0; i < length; i++)
            {
                if ((arr1[i] != 0 || n1 != 0) && (arr2[i] != 0 || n2 != 0))
                    s = s + Math.Pow(arr1[i], n1) * Math.Pow(arr2[i], n2);
                else
                    s = s + 1;
            }
            return s;

        }

        /// <summary>
        /// 拟合一元多次曲线过定点（x,y）   例： A*x*x*x+B*x*x+C*x+D=y
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <param name="length"></param>
        /// <param name="dimension"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double[] MultiLineToFixedPoint2(double[] arrX, double[] arrY, int length, int dimension, double x, double y)
        {
            int n = dimension;
            double[,] Guass = new double[n, n + 1];
            for (int i = 0; i < n; i++)
            {
                int j;
                for (j = 0; j < n; j++)
                {
                    Guass[i, j] = SumArr(arrX, x, length, i, j);
                }
                Guass[i, j] = SumArr(arrX, arrY, x, y, length, i);
            }
            double[] result = ComputGauss(Guass, n);
            double D = 0;//常数项
            double A = 0;//被减项
            for (int i = 0; i < n; i++)
            {
                A += Math.Pow(x, n - i) * result[n - i - 1];
            }
            double[] number = new double[result.Length + 1];
            D = y - A;
            number[0] = D;
            for (int i = 0; i < result.Length; i++)
            {
                number[i + 1] = result[i];
            }
            return number;
        }

        /// <summary>
        /// 一元多次项 A的转置乘A的（i,j）项length个的和
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="x"></param>
        /// <param name="length"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private static double SumArr(double[] arrX, double x, int length, double i, double j) // i,j为行列式的横，纵坐标
        {
            double result = 0;
            for (int k = 0; k < length; k++)
            {
                result += (Math.Pow(arrX[k], i + 1) - Math.Pow(x, i + 1)) * (Math.Pow(arrX[k], j + 1) - Math.Pow(x, j + 1));
            }
            return result;
        }
        /// <summary>
        /// 一元多次项 A的转置乘y的length个的和
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="length"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static double SumArr(double[] arrX, double[] arrY, double x, double y, int length, double i)
        {
            double result = 0;
            for (int k = 0; k < length; k++)
            {
                result += (Math.Pow(arrX[k], i + 1) - Math.Pow(x, i + 1)) * (arrY[k] - y);
            }
            return result;
        }
        /// <summary>
        /// 计算高斯矩阵（增广矩阵）
        /// </summary>
        /// <param name="Guass"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static double[] ComputGauss(double[,] Guass, int n)
        {
            int i, j;
            int k, m;
            double temp;
            double max;
            double s;
            double[] x = new double[n];

            for (i = 0; i < n; i++) x[i] = 0.0;//初始化


            for (j = 0; j < n; j++)
            {
                max = 0;

                k = j;
                for (i = j; i < n; i++)
                {
                    if (Math.Abs(Guass[i, j]) > max)
                    {
                        max = Guass[i, j];
                        k = i;
                    }
                }



                if (k != j)
                {
                    for (m = j; m < n + 1; m++)
                    {
                        temp = Guass[j, m];
                        Guass[j, m] = Guass[k, m];
                        Guass[k, m] = temp;

                    }
                }

                if (0 == max)
                {
                    // "此线性方程为奇异线性方程" 

                    return x;
                }


                for (i = j + 1; i < n; i++)
                {

                    s = Guass[i, j];
                    for (m = j; m < n + 1; m++)
                    {
                        Guass[i, m] = Guass[i, m] - Guass[j, m] * s / (Guass[j, j]);

                    }
                }


            }//结束for (j=0;j<n;j++)


            for (i = n - 1; i >= 0; i--)
            {
                s = 0;
                for (j = i + 1; j < n; j++)
                {
                    s = s + Guass[i, j] * x[j];
                }

                x[i] = (Guass[i, n] - s) / Guass[i, i];

            }

            return x;
        } //返回值是函数的系数

        //例如：y=a0+a1* x 返回值则为a0 a1

        //例如：y=a0+a1* x+a2* x*x 返回值则为a0 a1 a2

        /// <summary>
        /// 计算R^2,R^2这个值越接近1，说明拟合出来的曲线跟原曲线就越接近
        /// </summary>
        /// <param name="Y">实际的Y</param>
        /// <param name="Ytest">代入拟合曲线方程得到的Y</param>
        /// <returns>返回R^2</returns>
        public static double CalculateRSquared(IEnumerable<double> Y, IEnumerable<double> Ytest)
        {
            int n = 0;
            double r = 0.0;

            double meanA = 0;
            double meanB = 0;
            double varA = 0;
            double varB = 0;
            int ii = 0;

            using (IEnumerator<double> ieA = Y.GetEnumerator())
            using (IEnumerator<double> ieB = Ytest.GetEnumerator())
            {
                while (ieA.MoveNext())
                {
                    if (!ieB.MoveNext())
                    {

                    }
                    ii++;
                    double currentA = ieA.Current;
                    double currentB = ieB.Current;

                    double deltaA = currentA - meanA;
                    double scaleDeltaA = deltaA / ++n;

                    double deltaB = currentB - meanB;
                    double scaleDeltaB = deltaA / n;

                    meanA += scaleDeltaA;
                    meanB += scaleDeltaB;

                    varA += scaleDeltaA * deltaA * (n - 1);
                    varB += scaleDeltaB * deltaB * (n - 1);
                    r += (deltaA * deltaB * (n - 1)) / n;
                }
                if (ieB.MoveNext())
                {

                }
            }
            return (r / Math.Sqrt(varA * varB)) * (r / Math.Sqrt(varA * varB));
        }

        /// <summary>
        /// 计算拟合曲线后的Y
        /// </summary>
        /// <param name="dataNumber">数据数</param>
        /// <param name="dimension">次数</param>
        /// <param name="arrX"></param>
        /// <param name="a">拟合曲线的系数数组</param>
        /// <returns></returns>
        public static double[] ComputResult(double[] arrX, double[] param)
        {
            double[] result = new double[arrX.Length];
            for (int i = 0; i < arrX.Length; i++)
            {
                double sum = 0;

                //for (int j = 0; j < dimension + 1; j++)
                //{
                //    sum += a[j] * Math.Pow(arrX[i], j);
                //}
                result[i] = Convert.ToDouble(sum.ToString("F9"));//保留一位
            }
            return result;
        }



        /// <summary>
        /// 计算拟合曲线后的Y
        /// </summary>
        /// <param name="dataNumber">数据数</param>
        /// <param name="dimension">次数</param>
        /// <param name="arrX"></param>
        /// <param name="a">拟合曲线的系数数组</param>
        /// <returns></returns>
        public static double[] ComputYtest(int dataNumber, int dimension, double[] arrX, double[] a)
        {
            double[] result = new double[dataNumber];
            for (int i = 0; i < dataNumber; i++)
            {
                double sum = 0;
                for (int j = 0; j < dimension + 1; j++)
                {
                    sum += a[j] * Math.Pow(arrX[i], j);
                }
                result[i] = Convert.ToDouble(sum.ToString("F9"));//保留一位
            }
            return result;
        }
        /// <summary>
        /// 指数函数拟合函数模型  y=c*m^x;输出为c，m
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double[] IndexEST(double[] x, double[] y)
        {
            double[] lnY = new double[y.Length];
            double[] ratio;
            for (int i = 0; i < y.Length; i++)
            {
                lnY[i] = Math.Log(y[i]);
            }
            ratio = MultiLine(x, lnY, y.Length, 1);
            for (int i = 0; i < ratio.Length; i++)
            {
                if (i == 0)
                {
                    ratio[i] = Math.Exp(ratio[i]);
                }
            }
            return ratio;
        }
        /// <summary>
        /// 幂函数拟合模型 y=c*x^b 输出为c，b
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double[] PowEST(double[] x, double[] y)
        {
            double[] lnY = new double[y.Length];
            double[] lnX = new double[x.Length];
            double[] dlinestRet;
            for (int i = 0; i < x.Length; i++)
            {
                lnY[i] = Math.Log(y[i]);
                lnX[i] = Math.Log(x[i]);
            }
            dlinestRet = MultiLine(lnX, lnY, y.Length, 1);
            dlinestRet[0] = Math.Exp(dlinestRet[0]);

            return dlinestRet;
        }
        /// <summary>
        /// 对数拟合函数 y=c*(lnx)+b 输出为b，c
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double[] LogEST(double[] x, double[] y)
        {
            double[] lnX = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] == 0 || x[i] < 0)
                {

                }
                lnX[i] = Math.Log(x[i]);
            }
            return MultiLine(lnX, y, y.Length, 1);
        }


        #region 根据给定曲线的系数数组求解曲线的驻点（导数为0的点） 原理：在[a,b]区间利用均分的方法，判断均分后点的单调性
        /// <summary>
        /// 根据给定曲线的系数数组求解曲线的驻点
        /// </summary>
        /// <param name="coefficient">曲线系数</param>
        /// <param name="count">均分数目</param>
        /// <param name="a">起始点</param>
        /// <param name="b">终止点</param>
        /// <param name="aValueY">拟合曲线第一个Y值</param>
        /// <returns></returns>
        public static ArrayList GetStationaryPoint(double[] coefficient, int count, double a, double b, double aValueY)
        {
            ArrayList result = new ArrayList();
            double proPointX = a;//上一个X值
            double proPointY = aValueY;//上一个Y值
            double currentPointX = 0;//当前点的X值
            double currentPointY = 0;//当前点的Y值
            double sectionValue = (b - a) / count;//区间间隔
            int tedium = -1;//单调性 1：增 0：减
            bool tediumChanged = false;//单调性是否改变 false:未改变  true:改变
            currentPointX = proPointX + sectionValue;

            while (currentPointX < b)
            {
                //计算当前点Y值
                for (int i = 0; i < coefficient.Length; i++)
                {
                    currentPointY += coefficient[i] * Math.Pow(currentPointX, i);
                }

                //判断单调性
                if (proPointY <= currentPointY)
                {
                    if (tedium == -1)//第一次进入不做单调性判断，直接赋值
                    {
                        tedium = 1;
                    }
                    else
                    {
                        if (tedium == 0)//单调性改变
                        {
                            tediumChanged = true;
                        }
                    }
                    tedium = 1;
                }
                else
                {
                    if (tedium == -1)//第一次进入不做单调性判断，直接赋值
                    {
                        tedium = 0;
                    }
                    else
                    {
                        if (tedium == 1)//单调性改变
                        {
                            tediumChanged = true;
                        }
                    }
                    tedium = 0;
                }
                //单调性改变
                if (tediumChanged)
                {
                    result.Add(proPointX);
                    tediumChanged = false;//重置，等待下一次单调性改变
                }
                proPointX = currentPointX;
                proPointY = currentPointY;
                currentPointX += sectionValue;
                currentPointY = 0;

            }
            return result;
        }

        #endregion


        #region 网上代码实现


        #endregion
    }
}
