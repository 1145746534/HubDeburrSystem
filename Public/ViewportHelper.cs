using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Media3D;
using HalconDotNet;
using HubDeburrSystem.Models;

namespace HubDeburrSystem.Public
{
    public class ViewportHelper
    {
        /// <summary>
        /// 基础四元数W
        /// </summary>
        public static double W { get; set; }
        /// <summary>
        /// 基础四元数X
        /// </summary>
        public static double X { get; set; }
        /// <summary>
        /// 基础四元数Y
        /// </summary>
        public static double Y { get; set; }
        /// <summary>
        /// 基础四元数Z
        /// </summary>
        public static double Z { get; set; }
        /// <summary>
        /// 欧拉角X或roll
        /// </summary>
        public static double EX { get; set; }
        /// <summary>
        /// 欧拉角Y或pitch
        /// </summary>
        public static double EY { get; set; }
        /// <summary>
        /// 欧拉角Z或yaw
        /// </summary>
        public static double EZ { get; set; }
        /// <summary>
        /// 欧拉角转四元数
        /// </summary>
        /// <param name="roll"></param>
        /// <param name="pitch"></param>
        /// <param name="yaw"></param>
        /// <returns></returns>
        public static Quaternion EulerToQuaternion(double roll, double pitch, double yaw)
        {
            roll = roll * Math.PI / 180;
            pitch = pitch * Math.PI / 180;
            yaw = yaw * Math.PI / 180;

            double cr = Math.Cos(roll * 0.5);
            double sr = Math.Sin(roll * 0.5);
            double cp = Math.Cos(pitch * 0.5);
            double sp = Math.Sin(pitch * 0.5);
            double cy = Math.Cos(yaw * 0.5);
            double sy = Math.Sin(yaw * 0.5);

            double w = cy * cp * cr + sy * sp * sr;
            double x = cy * cp * sr - sy * sp * cr;
            double y = sy * cp * sr + cy * sp * cr;
            double z = sy * cp * cr - cy * sp * sr;

            Quaternion q = new Quaternion(x, y, z, w);

            return q;
        }

        /// <summary>
        /// 四元素转欧拉角
        /// </summary>
        /// <param name="w">q1</param>
        /// <param name="x">q2</param>
        /// <param name="y">q3</param>
        /// <param name="z">q4</param>
        /// <param name="roll">x</param>
        /// <param name="pitch">y</param>
        /// <param name="yaw">z</param>
        public static void QuaternionToEuler(double w, double x, double y, double z, out double roll, out double pitch, out double yaw)
        {
            //转换为弧度
            double sinr_cosp = 2.0 * (w * x + y * z);
            double cosr_cosp = 1.0 - 2.0 * (x * x + y * y);
            var r = Math.Atan2(sinr_cosp, cosr_cosp);

            //处理pitch奇点问题
            double p;
            double sinp = 2.0 * (w * y - x * z);
            if (Math.Abs(sinp) >= 1)
                p = (sinp >= 0 ? 1 : -1) * Math.PI / 2; // 如果sinp = 1，pitch = 90，如果sinp = -1，pitch = -90
            else
                p = Math.Asin(sinp);

            double siny_cosp = 2.0 * (w * z + x * y);
            double cosy_cosp = 1.0 - 2.0 * (y * y + z * z);
            var ya = Math.Atan2(siny_cosp, cosy_cosp);
            //弧度转角度
            roll = (double)(r * 180 / Math.PI);
            pitch = (double)(p * 180 / Math.PI);
            yaw = (double)(ya * 180 / Math.PI);
        }

        public static void QuaternionToEuler(Quaternion q, out double ex, out double ey, out double ez)
        {
            var q0 = q.W;
            var q1 = q.X;
            var q2 = q.Y;
            var q3 = q.Z;
            var x = Math.Atan2(2 * (q0 * q1 + q2 * q3), q0 * q0 - q1 * q1 - q2 * q2 + q3 * q3);

            var y = Math.Asin(2 * (q0 * q2 - q1 * q3));

            var z = Math.Atan2(2 * (q1 * q2 + q0 * q3), q0 * q0 + q1 * q1 - q2 * q2 - q3 * q3);

            ex = x * 180.0 / Math.PI;
            ey = y * 180.0 / Math.PI; 
            ez = z * 180.0 / Math.PI;
        }
    }
}
