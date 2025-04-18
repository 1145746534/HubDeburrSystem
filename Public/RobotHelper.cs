using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Public
{
    public class RobotHelper
    {
        /// <summary>
        /// 发送多少个坐标给机器人后启动加工
        /// </summary>
        public static int NumberOfCoordinates {  get; set; }
        /// <summary>
        /// 给机器人发送点的编号
        /// </summary>
        public static int SendNumber { get; set; } = 1;

        /// <summary>
        /// 在当前角度上旋转指定角度后的角度计算
        /// </summary>
        /// <param name="currentAngle">当前角度</param>
        /// <param name="rotationAngle">旋转角度</param>
        /// <returns></returns>
        public static double AngleCalculationAfterRotation(double currentAngle, double rotationAngle)
        {
            double angle;
            //角度计算:180 -> -180
            if (currentAngle >= 0 && currentAngle + rotationAngle > 181)
            {
                angle = -180 + (currentAngle + rotationAngle - 180);
            }
            //角度计算:-180 -> 180
            else if (currentAngle < 0 && currentAngle + rotationAngle < -181)
            {
                angle = 180 - (Math.Abs(currentAngle + rotationAngle + 180));
            }
            else if(currentAngle + rotationAngle == 180)
            {
                angle = 180;
            }
            else if (currentAngle + rotationAngle == -180)
            {
                angle = -180;
            }
            else if (currentAngle + rotationAngle == -181)
            {
                angle = 180;
            }
            else if (currentAngle + rotationAngle == 181)
            {
                angle = -180;
            }
            else
            {
                angle = currentAngle + rotationAngle;
            }
            return angle;
        }
    }
}
