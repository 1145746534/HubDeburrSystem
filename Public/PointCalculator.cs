using HalconDotNet;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.RootFinding;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HubDeburrSystem.Public
{
    public class PointCalculator
    {
        /// <summary>
        /// 计算在指定方向上延长指定长度后的点  
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="directionDegrees">角度</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static (double X, double Y) ExtendPoint(double x, double y, double directionDegrees, double length)
        {
            double directionRadians = directionDegrees * Math.PI / 180.0; // 将角度转换为弧度  

            double newX = x + length * Math.Cos(directionRadians); // 从(x, y)出发沿directionDegrees度方向  
            double newY = y + length * Math.Sin(directionRadians);

            return (newX, newY);
        }

        /// <summary>
        /// 计算在指定方向上延长指定长度后的点  
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="directionDegrees">弧度</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static (double X, double Y) ExtendPointRadian(double x, double y, double directionRadians, double length)
        {

            double newX = x + length * Math.Cos(directionRadians);
            double newY = y + length * Math.Sin(directionRadians);

            return (newX, newY);
        }
        /// <summary>
        /// 计算在指定方向上缩短指定长度后的点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="directionRadians"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static (double X, double Y) ShortenPointRadian(double x, double y, double directionRadians, double length)
        {
            double newX = x - length * Math.Cos(directionRadians); //   
            double newY = y - length * Math.Sin(directionRadians); //  

            return (newX, newY);
        }


        /// <summary>
        /// Generate starting and ending points
        /// </summary>
        /// <param name="centerX">中点X</param>
        /// <param name="centerY">中点Y</param>
        /// <param name="radians">弧度角</param>
        /// <param name="length">衍生长度</param>
        /// <returns></returns>
        public static (double StartX, double StartY, double EndX, double EndY) GenStartAndEndPoints(double centerX, double centerY, double radians, double length)
        {
            (double X, double Y) pointArrow1 = PointCalculator.ShortenPointRadian(centerX, centerY, radians, length);
            (double X, double Y) pointArrow2 = PointCalculator.ExtendPointRadian(centerX, centerY, radians, length);
            return (pointArrow1.X, pointArrow1.Y, pointArrow2.X, pointArrow2.Y);

        }

        /// <summary>
        /// 斜率的绝对值
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="startCol"></param>
        /// <param name="endRow"></param>
        /// <param name="endCol"></param>
        /// <returns></returns>
        public static double GetSlope(double startRow, double startCol, double endRow, double endCol)
        {
            double slope = (double)(endCol - startCol) / (endRow - startRow);
            //slope = Math.Abs(slope);
            return slope;
        }


        /// <summary>
        /// Generate angle
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <returns></returns>
        public static HTuple GenAngle(double startX, double startY, double endX, double endY)
        {
            HTuple hv_LineRowStart = startX;
            HTuple hv_LineColumnStart = startY;
            HTuple hv_LineRowEnd = endX;
            HTuple hv_LineColumnEnd = endY;
            HTuple hv_TmpCtrl_Dr = new HTuple(), hv_TmpCtrl_Dc = new HTuple();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TmpCtrl_Dr = hv_LineRowStart - hv_LineRowEnd;
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TmpCtrl_Dc = hv_LineColumnEnd - hv_LineColumnStart;
            }

            HTuple hv_TmpCtrl_Phi = hv_TmpCtrl_Dr.TupleAtan2(
                    hv_TmpCtrl_Dc);
            return hv_TmpCtrl_Phi;
        }

        public static HTuple GenMeasureRectangle2_(double centerX, double centerY, double phi, double length, double widthLen, HTuple threshold)
        {


            HTuple hv_LineRowStart_Measure_01_0;
            HTuple hv_LineColumnStart_Measure_01_0;
            HTuple hv_LineRowEnd_Measure_01_0;
            HTuple hv_LineColumnEnd_Measure_01_0;

            HTuple hv_AmplitudeThreshold = new HTuple();
            HTuple hv_RoiWidthLen2 = new HTuple();
            hv_AmplitudeThreshold.Dispose();
            hv_AmplitudeThreshold = 10;
            hv_RoiWidthLen2.Dispose();
            hv_RoiWidthLen2 = 5;

            HTuple hv_TmpCtrl_Row = new HTuple(), hv_TmpCtrl_Column = new HTuple();
            HTuple hv_TmpCtrl_Dr = new HTuple(), hv_TmpCtrl_Dc = new HTuple();
            HTuple hv_TmpCtrl_Phi = new HTuple(), hv_TmpCtrl_Len1 = new HTuple();
            HTuple hv_TmpCtrl_Len2 = new HTuple(), hv_MsrHandle_Measure_01_0 = new HTuple();
            HTuple hv_Row_Measure_01_0 = new HTuple(), hv_Column_Measure_01_0 = new HTuple();
            HTuple hv_Amplitude_Measure_01_0 = new HTuple(), hv_Distance_Measure_01_0 = new HTuple();

            (double X, double Y) pointArrow1 = PointCalculator.ShortenPointRadian(centerX, centerY, phi, length);
            hv_LineRowStart_Measure_01_0 = pointArrow1.X;
            hv_LineColumnStart_Measure_01_0 = pointArrow1.Y;
            (double X, double Y) pointArrow2 = PointCalculator.ExtendPointRadian(centerX, centerY, phi, length);
            hv_LineRowEnd_Measure_01_0 = pointArrow2.X;
            hv_LineColumnEnd_Measure_01_0 = pointArrow2.Y;

            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TmpCtrl_Row = 0.5 * (hv_LineRowStart_Measure_01_0 + hv_LineRowEnd_Measure_01_0);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TmpCtrl_Column = 0.5 * (hv_LineColumnStart_Measure_01_0 + hv_LineColumnEnd_Measure_01_0);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TmpCtrl_Dr = hv_LineRowStart_Measure_01_0 - hv_LineRowEnd_Measure_01_0;
            }

            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TmpCtrl_Dc = hv_LineColumnEnd_Measure_01_0 - hv_LineColumnStart_Measure_01_0;
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TmpCtrl_Phi = hv_TmpCtrl_Dr.TupleAtan2(
                    hv_TmpCtrl_Dc);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TmpCtrl_Len1 = 0.5 * ((((hv_TmpCtrl_Dr * hv_TmpCtrl_Dr) + (hv_TmpCtrl_Dc * hv_TmpCtrl_Dc))).TupleSqrt()
                    );
            }
            hv_TmpCtrl_Len2.Dispose();
            hv_TmpCtrl_Len2 = new HTuple(hv_RoiWidthLen2);
            //Measure 01: Create measure for line Measure 01 [0]
            //Measure 01: Attention: This assumes all images have the same size!
            hv_MsrHandle_Measure_01_0.Dispose();
            HOperatorSet.GenMeasureRectangle2(hv_TmpCtrl_Row, hv_TmpCtrl_Column, hv_TmpCtrl_Phi,
                hv_TmpCtrl_Len1, hv_TmpCtrl_Len2, 5472, 3648, "nearest_neighbor", out hv_MsrHandle_Measure_01_0);
            return hv_MsrHandle_Measure_01_0;
        }



        /// <summary>
        /// 计算在指定方向上缩短指定长度后的点  
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="directionDegrees"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static (double X, double Y) ShortenPoint(double x, double y, double directionDegrees, double length)
        {
            double newX = x - length * Math.Cos(directionDegrees * Math.PI / 180.0); // 从(x, y)出发沿directionDegrees度方向的反方向缩短  
            double newY = y - length * Math.Sin(directionDegrees * Math.PI / 180.0); //  

            return (newX, newY);
        }

        public void GenRectangle2(double centerX, double centerY, double phi, double length, double widthLen)
        {
            double startX;
            double startY;
            double endX;
            double endY;
            double length2 = length / 2;

            (double X, double Y) point1 = ExtendPoint(centerX, centerY, length2, phi);
            startX = point1.X;
            startY = point1.Y;
            (double X, double Y) point2 = ShortenPoint(centerX, centerY, length2, phi);
            endX = point2.X;
            endY = point2.Y;



        }

        public double GetAngle(double satrtX, double startY, double endX, double endY)
        {
            HOperatorSet.AngleLx(satrtX, startY, endX, endY, out HTuple angle);
            return angle.D;
        }




        public static void n()
        {
            double x = 5, y = 10;
            double direction = 30; // 方向为30度  
            double lengthToExtend = 50; // 延长长度  
            double lengthToShorten = 50; // 缩短长度  

            (double X, double Y) point1 = ExtendPoint(x, y, direction, lengthToExtend);
            (double X, double Y) point2 = ShortenPoint(x, y, direction, lengthToShorten);

            Console.WriteLine($"Extended Point: ({point1.X}, {point1.Y})");
            Console.WriteLine($"Shortened Point: ({point2.X}, {point2.Y})");
        }
    }
}
