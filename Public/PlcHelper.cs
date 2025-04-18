using CommonServiceLocator;
using HubDeburrSystem.ViewModel;
using Sharp7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HubDeburrSystem.Public
{
    public class PlcHelper
    {
        /// <summary>
        /// PLC连接
        /// </summary>
        public static S7Client PlcCilent { get; set; } = new S7Client();

        /// <summary>
        /// PLC IP地址
        /// </summary>
        public static string PlcIP { get; set; }

        /// <summary>
        /// 读写PLC线程控制
        /// </summary>
        public static bool ReadWritePlcThreadControl { get; set; } = false;

        /// <summary>
        /// PLC数据交换读缓冲区
        /// </summary>
        public static byte[] PlcReadDataBuffer { get; set; }

        /// <summary>
        /// PLC数据交换写缓冲区
        /// </summary>
        public static byte[] PlcWriteDataBuffer { get; set; }

        /// <summary>
        /// 读Plc数据的DB块号
        /// </summary>
        public static int ReadPlcDB {  get; set; }

        /// <summary>
        /// 写Plc数据的DB块号
        /// </summary>
        public static int WritePlcDB { get; set; }

        /// <summary>
        /// 读Plc数据的长度
        /// </summary>
        public static int ReadPlcDataLength { get; set; }

        /// <summary>
        /// 写Plc数据的长度
        /// </summary>
        public static int WritePlcDataLength { get; set; }
    }
}
