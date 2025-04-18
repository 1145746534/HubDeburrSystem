using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HalconDotNet;
using HubDeburrSystem.DataAccess;
using HubDeburrSystem.Models;
using HubDeburrSystem.Public;
using HubDeburrSystem.Views.Dialog;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.ViewModel
{
    public class SettingPageViewModel : ViewModelBase
    {
        #region=========九点标定输入框绑定字段=========
        private double _robotX1;
        public double RobotX1
        {
            get { return _robotX1; }
            set { Set(ref _robotX1, value); }
        }

        private double _robotY1;
        public double RobotY1
        {
            get { return _robotY1; }
            set { Set(ref _robotY1, value); }
        }

        private double _robotX2;
        public double RobotX2
        {
            get { return _robotX2; }
            set { Set(ref _robotX2, value); }
        }

        private double _robotY2;
        public double RobotY2
        {
            get { return _robotY2; }
            set { Set(ref _robotY2, value); }
        }

        private double _robotX3;
        public double RobotX3
        {
            get { return _robotX3; }
            set { Set(ref _robotX3, value); }
        }

        private double _robotY3;
        public double RobotY3
        {
            get { return _robotY3; }
            set { Set(ref _robotY3, value); }
        }

        private double _robotX4;
        public double RobotX4
        {
            get { return _robotX4; }
            set { Set(ref _robotX4, value); }
        }

        private double _robotY4;
        public double RobotY4
        {
            get { return _robotY4; }
            set { Set(ref _robotY4, value); }
        }

        private double _robotX5;
        public double RobotX5
        {
            get { return _robotX5; }
            set { Set(ref _robotX5, value); }
        }

        private double _robotY5;
        public double RobotY5
        {
            get { return _robotY5; }
            set { Set(ref _robotY5, value); }
        }

        private double _robotX6;
        public double RobotX6
        {
            get { return _robotX6; }
            set { Set(ref _robotX6, value); }
        }

        private double _robotY6;
        public double RobotY6
        {
            get { return _robotY6; }
            set { Set(ref _robotY6, value); }
        }

        private double _robotX7;
        public double RobotX7
        {
            get { return _robotX7; }
            set { Set(ref _robotX7, value); }
        }

        private double _robotY7;
        public double RobotY7
        {
            get { return _robotY7; }
            set { Set(ref _robotY7, value); }
        }

        private double _robotX8;
        public double RobotX8
        {
            get { return _robotX8; }
            set { Set(ref _robotX8, value); }
        }

        private double _robotY8;
        public double RobotY8
        {
            get { return _robotY8; }
            set { Set(ref _robotY8, value); }
        }

        private double _robotX9;
        public double RobotX9
        {
            get { return _robotX9; }
            set { Set(ref _robotX9, value); }
        }

        private double _robotY9;
        public double RobotY9
        {
            get { return _robotY9; }
            set { Set(ref _robotY9, value); }
        }

        private double _imageRow1;
        public double ImageRow1
        {
            get { return _imageRow1; }
            set { Set(ref _imageRow1, value); }
        }

        private double _imageCol1;
        public double ImageCol1
        {
            get { return _imageCol1; }
            set { Set(ref _imageCol1, value); }
        }

        private double _imageRow2;
        public double ImageRow2
        {
            get { return _imageRow2; }
            set { Set(ref _imageRow2, value); }
        }

        private double _imageCol2;
        public double ImageCol2
        {
            get { return _imageCol2; }
            set { Set(ref _imageCol2, value); }
        }

        private double _imageRow3;
        public double ImageRow3
        {
            get { return _imageRow3; }
            set { Set(ref _imageRow3, value); }
        }

        private double _imageCol3;
        public double ImageCol3
        {
            get { return _imageCol3; }
            set { Set(ref _imageCol3, value); }
        }

        private double _imageRow4;
        public double ImageRow4
        {
            get { return _imageRow4; }
            set { Set(ref _imageRow4, value); }
        }

        private double _imageCol4;
        public double ImageCol4
        {
            get { return _imageCol4; }
            set { Set(ref _imageCol4, value); }
        }

        private double _imageRow5;
        public double ImageRow5
        {
            get { return _imageRow5; }
            set { Set(ref _imageRow5, value); }
        }

        private double _imageCol5;
        public double ImageCol5
        {
            get { return _imageCol5; }
            set { Set(ref _imageCol5, value); }
        }

        private double _imageRow6;
        public double ImageRow6
        {
            get { return _imageRow6; }
            set { Set(ref _imageRow6, value); }
        }

        private double _imageCol6;
        public double ImageCol6
        {
            get { return _imageCol6; }
            set { Set(ref _imageCol6, value); }
        }

        private double _imageRow7;
        public double ImageRow7
        {
            get { return _imageRow7; }
            set { Set(ref _imageRow7, value); }
        }

        private double _imageCol7;
        public double ImageCol7
        {
            get { return _imageCol7; }
            set { Set(ref _imageCol7, value); }
        }

        private double _imageRow8;
        public double ImageRow8
        {
            get { return _imageRow8; }
            set { Set(ref _imageRow8, value); }
        }

        private double _imageCol8;
        public double ImageCol8
        {
            get { return _imageCol8; }
            set { Set(ref _imageCol8, value); }
        }

        private double _imageRow9;
        public double ImageRow9
        {
            get { return _imageRow9; }
            set { Set(ref _imageRow9, value); }
        }

        private double _imageCol9;
        public double ImageCol9
        {
            get { return _imageCol9; }
            set { Set(ref _imageCol9, value); }
        }
        #endregion
        #region=========标定和验证字段=========
        private double _xScale;
        /// <summary>
        /// 沿X方向缩放的比例因子
        /// </summary>
        public double XScale
        {
            get { return _xScale; }
            set { Set(ref _xScale, value); }
        }

        private double _yScale;
        /// <summary>
        /// 沿Y方向缩放的比例因子
        /// </summary>
        public double YScale
        {
            get { return _yScale; }
            set { Set(ref _yScale, value); }
        }

        private double _rotationAngle;
        /// <summary>
        /// 旋转角度
        /// </summary>
        public double RotationAngle
        {
            get { return _rotationAngle; }
            set { Set(ref _rotationAngle, value); }
        }

        private double _slantAngle;
        /// <summary>
        /// 倾斜角度
        /// </summary>
        public double SlantAngle
        {
            get { return _slantAngle; }
            set { Set(ref _slantAngle, value); }
        }

        private double _xTranslation;
        /// <summary>
        /// 沿X方向平移
        /// </summary>
        public double XTranslation
        {
            get { return _xTranslation; }
            set { Set(ref _xTranslation, value); }
        }

        private double _yTranslation;
        /// <summary>
        /// 沿Y方向平移
        /// </summary>
        public double YTranslation
        {
            get { return _yTranslation; }
            set { Set(ref _yTranslation, value); }
        }

        private double _rMSE;
        /// <summary>
        /// 均方根误差
        /// </summary>
        public double RMSE
        {
            get { return _rMSE; }
            set { Set(ref _rMSE, value); }
        }

        private double _mAXE;
        /// <summary>
        /// 最大误差
        /// </summary>
        public double MAXE
        {
            get { return _mAXE; }
            set { Set(ref _mAXE, value); }
        }

        private double _rowCoordinate;
        /// <summary>
        /// 行坐标
        /// </summary>
        public double RowCoordinate
        {
            get { return _rowCoordinate; }
            set { Set(ref _rowCoordinate, value); }
        }

        private double _colCoordinate;
        /// <summary>
        /// 列坐标
        /// </summary>
        public double ColCoordinate
        {
            get { return _colCoordinate; }
            set { Set(ref _colCoordinate, value); }
        }

        private double _xCoordinate;
        /// <summary>
        /// X坐标
        /// </summary>
        public double XCoordinate
        {
            get { return _xCoordinate; }
            set { Set(ref _xCoordinate, value); }
        }

        private double _yCoordinate;
        /// <summary>
        /// Y坐标
        /// </summary>
        public double YCoordinate
        {
            get { return _yCoordinate; }
            set { Set(ref _yCoordinate, value); }
        }

        private double _hubHeight;
        /// <summary>
        /// 轮毂高度
        /// </summary>
        public double HubHeight
        {
            get { return _hubHeight; }
            set {Set(ref _hubHeight, value); }
        }

        #endregion
        #region=========比例尺和补偿字段=========
        private double _calibrationPlateHeight;
        /// <summary>
        /// 标定板高度
        /// </summary>
        public double CalibrationPlateHeight
        {
            get { return _calibrationPlateHeight; }
            set
            {
                Set(ref _calibrationPlateHeight, value);
                ConfigEdit.SystemDatasWrite("CalibrationPlateHeight", CalibrationPlateHeight.ToString());
            }
        }

        private double _calibrationHeight1;
        /// <summary>
        /// 标定高度1
        /// </summary>
        public double CalibrationHeight1
        {
            get { return _calibrationHeight1; }
            set
            {
                Set(ref _calibrationHeight1, value);
                ConfigEdit.SystemDatasWrite("CalibrationHeight1", CalibrationHeight1.ToString());
            }
        }

        private double _calibrationHeight2;
        /// <summary>
        /// 标定高度2
        /// </summary>
        public double CalibrationHeight2
        {
            get { return _calibrationHeight2; }
            set
            {
                Set(ref _calibrationHeight2, value);
                ConfigEdit.SystemDatasWrite("CalibrationHeight2", CalibrationHeight2.ToString());
            }
        }

        private double _physicsDistance1;
        /// <summary>
        /// 物理距离1
        /// </summary>
        public double PhysicsDistance1
        {
            get { return _physicsDistance1; }
            set
            {
                Set(ref _physicsDistance1, value);
                ConfigEdit.SystemDatasWrite("PhysicsDistance1", PhysicsDistance1.ToString());
            }
        }

        private double _physicsDistance2;
        /// <summary>
        /// 物理距离2
        /// </summary>
        public double PhysicsDistance2
        {
            get { return _physicsDistance2; }
            set
            {
                Set(ref _physicsDistance2, value);
                ConfigEdit.SystemDatasWrite("PhysicsDistance2", PhysicsDistance2.ToString());
            }
        }

        private double _pixelDistance1;
        /// <summary>
        /// 像素距离1
        /// </summary>
        public double PixelDistance1
        {
            get { return _pixelDistance1; }
            set
            {
                Set(ref _pixelDistance1, value);
                ConfigEdit.SystemDatasWrite("PixelDistance1", PixelDistance1.ToString());
            }
        }

        private double _pixelDistance2;
        /// <summary>
        /// 像素距离2
        /// </summary>
        public double PixelDistance2
        {
            get { return _pixelDistance2; }
            set
            {
                Set(ref _pixelDistance2, value);
                ConfigEdit.SystemDatasWrite("PixelDistance2", PixelDistance2.ToString());
            }
        }

        private double _robotAxisCompensateX;
        /// <summary>
        /// 机器人X轴补偿
        /// </summary>
        public double RobotAxisCompensateX
        {
            get { return _robotAxisCompensateX; }
            set
            {
                Set(ref _robotAxisCompensateX, value);
                ConfigEdit.SystemDatasWrite("RobotAxisCompensateX", RobotAxisCompensateX.ToString());
            }
        }

        private double _robotAxisCompensateY;
        /// <summary>
        /// 机器人Y轴补偿
        /// </summary>
        public double RobotAxisCompensateY
        {
            get { return _robotAxisCompensateY; }
            set
            {
                Set(ref _robotAxisCompensateY, value);
                ConfigEdit.SystemDatasWrite("RobotAxisCompensateY", RobotAxisCompensateY.ToString());
            }
        }

        private double _robotAxisCompensateZ;
        /// <summary>
        /// 机器人Z轴补偿
        /// </summary>
        public double RobotAxisCompensateZ
        {
            get { return _robotAxisCompensateZ; }
            set
            {
                Set(ref _robotAxisCompensateZ, value);
                ConfigEdit.SystemDatasWrite("RobotAxisCompensateZ", RobotAxisCompensateZ.ToString());
            }
        }

        private double _robotAngleCompensate;
        /// <summary>
        /// 机器人角度补偿
        /// </summary>
        public double RobotAngleCompensate
        {
            get { return _robotAngleCompensate; }
            set
            {
                Set(ref _robotAngleCompensate, value);
                ConfigEdit.SystemDatasWrite("RobotAngleCompensate", RobotAngleCompensate.ToString());
            }
        }
        #endregion
        #region=========按钮命令=========
        /// <summary>
        /// 标定结果命令
        /// </summary>
        public RelayCommand CalibrationResultCommand { get; set; }

        /// <summary>
        /// 图像坐标转物理坐标命令
        /// </summary>
        public RelayCommand PixelToPhysicsCommand { get; set; }

        /// <summary>
        /// 物理坐标转图像坐标命令
        /// </summary>
        public RelayCommand PhysicsToPixelCommand { get; set; }

        #endregion

        public SettingPageViewModel()
        {
            CalibrationResultCommand = new RelayCommand(CalibrationResult);
            PixelToPhysicsCommand = new RelayCommand(PixelToPhysics);
            PhysicsToPixelCommand = new RelayCommand(PhysicsToPixel);
        }

        /// <summary>
        /// 像素坐标转物理坐标
        /// </summary>
        private void PixelToPhysics()
        {
            if (TemplateHelper.PixelToPhysicsHomMat2D == null)
            {
                return;
            }
            //获取指定高度下的新齐次变换矩阵
            var homMat2D = GetPixelToPhysicsHomMat2D(HubHeight, TemplateHelper.PixelToPhysicsHomMat2D);
            HOperatorSet.AffineTransPoint2d(homMat2D, RowCoordinate, ColCoordinate, out HTuple qx, out HTuple qy);
            XCoordinate = Math.Round(qx.D, 2) + RobotAxisCompensateX;
            YCoordinate = Math.Round(qy.D, 2) + RobotAxisCompensateY;

        }

        /// <summary>
        /// 物理坐标转像素坐标
        /// </summary>
        private void PhysicsToPixel()
        {
            //获取指定高度下的新齐次变换矩阵
            var homMat2D = GetPixelToPhysicsHomMat2D(HubHeight, TemplateHelper.PixelToPhysicsHomMat2D);
            HOperatorSet.HomMat2dInvert(homMat2D, out HTuple homMat2DInvert);

            HOperatorSet.AffineTransPoint2d(homMat2DInvert, XCoordinate, YCoordinate, out HTuple qRow, out HTuple qCol);
            //
            var r =  RobotAxisCompensateX / homMat2D[0].D;
            var c = RobotAxisCompensateY / homMat2D[0].D;
            RowCoordinate = Math.Round(qRow.D - r, 2);
            ColCoordinate = Math.Round(qCol.D - c, 2);
        }

        /// <summary>
        /// 标定结果
        /// </summary>
        private void CalibrationResult()
        {
            var r = UMessageBox.Show("执行标定", "请确认输入正确后点击确认！");
            if (r)
            {
                HTuple robotXs = new HTuple();
                robotXs[0] = RobotX1; robotXs[1] = RobotX2; robotXs[2] = RobotX3; robotXs[3] = RobotX4; robotXs[4] = RobotX5;
                robotXs[5] = RobotX6; robotXs[6] = RobotX7; robotXs[7] = RobotX8; robotXs[8] = RobotX9;
                HTuple robotYs = new HTuple();
                robotYs[0] = RobotY1; robotYs[1] = RobotY2; robotYs[2] = RobotY3; robotYs[3] = RobotY4; robotYs[4] = RobotY5; 
                robotYs[5] = RobotY6; robotYs[6] = RobotY7; robotYs[7] = RobotY8; robotYs[8] = RobotY9;
                HTuple imageRows = new HTuple();
                imageRows[0] = ImageRow1; imageRows[1] = ImageRow2; imageRows[2] = ImageRow3; imageRows[3] = ImageRow4; imageRows[4] = ImageRow5;
                imageRows[5] = ImageRow6; imageRows[6] = ImageRow7; imageRows[7] = ImageRow8; imageRows[8] = ImageRow9;
                HTuple imageCols = new HTuple();
                imageCols[0] = ImageCol1; imageCols[1] = ImageCol2; imageCols[2] = ImageCol3; imageCols[3] = ImageCol4; imageCols[4] = ImageCol5; 
                imageCols[5] = ImageCol6; imageCols[6] = ImageCol7; imageCols[7] = ImageCol8; imageCols[8] = ImageCol9;
                try
                {
                    #region=========生成并保存齐次变换矩阵=========
                    //生成像素坐标到物理坐标的齐次变换矩阵
                    HOperatorSet.VectorToHomMat2d(imageRows, imageCols, robotXs, robotYs, out HTuple homMat2D);
                    string path = Environment.CurrentDirectory + @"\PixelToPhysics1.tup";
                    HOperatorSet.WriteTuple(homMat2D, path);
                    TemplateHelper.PixelToPhysicsHomMat2D = homMat2D;
                    //获取矩阵的参数
                    HOperatorSet.HomMat2dToAffinePar(homMat2D, out HTuple sx, out HTuple sy, out HTuple phi, out HTuple theta, out HTuple tx, out HTuple ty);
                    //显示参数
                    XScale = Math.Round(sx.D, 5); 
                    YScale = Math.Round(sy.D, 5);
                    RotationAngle = Math.Round(phi.D, 4); 
                    SlantAngle = Math.Round(theta.D, 4);
                    XTranslation = Math.Round(tx.D, 2); 
                    YTranslation = Math.Round(ty.D, 2);
                    //构建计算RMSE和MAXE的数据
                    HOperatorSet.AffineTransPoint2d(homMat2D, imageRows, imageCols, out HTuple qxs, out HTuple qys);
                    List<double> sourceData = new List<double>();
                    List<double> newData = new List<double>();
                    for (int i = 0; i < robotXs.Length; i++)
                    {
                        sourceData.Add(robotXs[i].D);
                    }
                    for (int i = 0; i < robotYs.Length; i++)
                    {
                        sourceData.Add(robotYs[i].D);
                    }
                    for (int i = 0; i < qxs.Length; i++)
                    {
                        newData.Add(qxs[i].D);
                    }
                    for (int i = 0; i < qys.Length; i++)
                    {
                        newData.Add(qys[i].D);
                    }
                    //计算RMSE和MAXE
                    ComputeRMSEAndMaxe(sourceData, newData, out double rmse, out double maxe);
                    RMSE = rmse; MAXE = maxe;
                    #endregion

                    #region=========存储9点标定数据=========
                    var sDB = new SqlAccess().SystemDataAccess;
                    //判断表是否存在
                    var r1 = sDB.DbMaintenance.IsAnyTable("NinePointCalibrationData", false);
                    //如果数据存在，则清空表
                    if (r1) sDB.DbMaintenance.TruncateTable("NinePointCalibrationData");
                    //如果不存在，则创建表
                    else sDB.CodeFirst.As<NinePointCalibrationData>("NinePointCalibrationData").InitTables<NinePointCalibrationData>();
                    //构建数据
                    List<NinePointCalibrationData> datas = new List<NinePointCalibrationData>();
                    for (int i = 0; i < robotXs.Length; i++)
                    {
                        NinePointCalibrationData data = new NinePointCalibrationData();
                        data.Id = i + 1;
                        data.Row = imageRows[i].D;
                        data.Column = imageCols[i].D;
                        data.X = robotXs[i].D;
                        data.Y = robotYs[i].D;
                        datas.Add(data);
                    }
                    sDB.Insertable(datas).ExecuteCommand();
                    #endregion
                    if (rmse > 0.4 || maxe > 0.4) UMessageBox.Show($"标定误差过大，RMSE：{rmse}，MAXE：{maxe}", MessageType.Warning);
                    else UMessageBox.Show("标定成功！", MessageType.Success);
                }
                catch (Exception ex)
                {
                    UMessageBox.Show("标定错误：" + ex.Message, MessageType.Error);
                }
            }
        }

        /// <summary>
        /// 计算均方根误差和最大误差
        /// </summary>
        /// <param name="actualValues">实际值</param>
        /// <param name="predictedValues">预测值</param>
        /// <param name="rmse">均方根误差</param>
        /// <param name="maxe">最大误差</param>
        /// <exception cref="ArgumentException"></exception>
        public void ComputeRMSEAndMaxe(List<double> actualValues, List<double> predictedValues, out double rmse, out double maxe)
        {
            if (actualValues.Count != predictedValues.Count)
            {
                throw new ArgumentException("实际值和预测值的数量不匹配");
            }
            var errors = actualValues.Zip(predictedValues, (a, p) => a - p).ToList();
            var squaredErrors = errors.Select(e => e * e);
            var meanSquaredError = squaredErrors.Average();
            var rms = Math.Sqrt(meanSquaredError);
            double max = 0;
            for (int i = 0; i < errors.Count; i++)
            {
                if (Math.Abs(errors[i]) > max) max = Math.Abs(errors[i]);
            }
            rmse = Math.Round(rms, 4);
            maxe = Math.Round(max, 4);
        }

        /// <summary>
        ///加载设置页面数据
        /// </summary>
        public void LoadSettingPageData()
        {
            string path = Environment.CurrentDirectory + @"\PixelToPhysics1.tup";
            if (File.Exists(path))
            {
                HOperatorSet.ReadTuple(path, out HTuple homMat2D);
                TemplateHelper.PixelToPhysicsHomMat2D = homMat2D;
                HOperatorSet.HomMat2dToAffinePar(homMat2D, out HTuple sx, out HTuple sy, out HTuple phi, out HTuple theta, out HTuple tx, out HTuple ty);

                Console.WriteLine($"{homMat2D[0]} ,{homMat2D[1]},{homMat2D[2]},{homMat2D[3]},{homMat2D[4]}");
                XScale = Math.Round(sx.D, 5); 
                YScale = Math.Round(sy.D, 5);
                RotationAngle = Math.Round(phi.D, 4); 
                SlantAngle = Math.Round(theta.D, 4);
                XTranslation = Math.Round(tx.D, 2); 
                YTranslation = Math.Round(ty.D, 2);
            }
            else TemplateHelper.PixelToPhysicsHomMat2D = null;
            //显示九点标定数据
            var sDB = new SqlAccess().SystemDataAccess;
            var datas = sDB.Queryable<NinePointCalibrationData>().ToList();
            if (datas.Count >= 9)
            {
                RobotX1 = datas[0].X; RobotY1 = datas[0].Y; ImageRow1 = datas[0].Row; ImageCol1 = datas[0].Column;
                RobotX2 = datas[1].X; RobotY2 = datas[1].Y; ImageRow2 = datas[1].Row; ImageCol2 = datas[1].Column;
                RobotX3 = datas[2].X; RobotY3 = datas[2].Y; ImageRow3 = datas[2].Row; ImageCol3 = datas[2].Column;
                RobotX4 = datas[3].X; RobotY4 = datas[3].Y; ImageRow4 = datas[3].Row; ImageCol4 = datas[3].Column;
                RobotX5 = datas[4].X; RobotY5 = datas[4].Y; ImageRow5 = datas[4].Row; ImageCol5 = datas[4].Column;
                RobotX6 = datas[5].X; RobotY6 = datas[5].Y; ImageRow6 = datas[5].Row; ImageCol6 = datas[5].Column;
                RobotX7 = datas[6].X; RobotY7 = datas[6].Y; ImageRow7 = datas[6].Row; ImageCol7 = datas[6].Column;
                RobotX8 = datas[7].X; RobotY8 = datas[7].Y; ImageRow8 = datas[7].Row; ImageCol8 = datas[7].Column;
                RobotX9 = datas[8].X; RobotY9 = datas[8].Y; ImageRow9 = datas[8].Row; ImageCol9 = datas[8].Column;
            }

            HTuple rows = new HTuple();
            HTuple cols = new HTuple();
            for (int i = 0; i < datas.Count; i++)
            {
                HOperatorSet.TupleConcat(rows, datas[i].Row, out rows);
                HOperatorSet.TupleConcat(cols, datas[i].Column, out cols);
            }
            //构建计算RMSE和MAXE的数据
            HOperatorSet.AffineTransPoint2d(TemplateHelper.PixelToPhysicsHomMat2D, rows, cols, out HTuple qxs, out HTuple qys);
            List<double> sourceData = new List<double>();
            List<double> newData = new List<double>();
            for (int i = 0; i < datas.Count; i++)
            {
                sourceData.Add(datas[i].X);
            }
            for (int i = 0; i < datas.Count; i++)
            {
                sourceData.Add(datas[i].Y);
            }
            for (int i = 0; i < qxs.Length; i++)
            {
                newData.Add(qxs[i].D);
            }
            for (int i = 0; i < qys.Length; i++)
            {
                newData.Add(qys[i].D);
            }
            //计算RMSE和MAXE
            ComputeRMSEAndMaxe(sourceData, newData, out double rmse, out double maxe);
            RMSE = rmse; MAXE = maxe;
        }
        
        //0-X缩放
        //1-
        //2-X位移
        //3-
        //4-Y缩放
        //5-Y位移

        /// <summary>
        /// 获取指定高度下像素坐标转物理坐标的齐次变换矩阵
        /// </summary>
        /// <param name="hubHeight">轮毂高度</param>
        /// <param name="sourcePixelToPhysicsHomMat2D">源像素转物理齐次变换矩阵</param>
        /// <returns></returns>
        public HTuple GetPixelToPhysicsHomMat2D(double hubHeight, HTuple sourcePixelToPhysicsHomMat2D)
        {
            HTuple homMat2D = new HTuple();
            //获取固定物理长度PhysicsDistance1下每1mm高度对应的图像像素
            double pixel = (PixelDistance2 - PixelDistance1) / (CalibrationHeight2 - CalibrationHeight1);

            double scale = PhysicsDistance1 / ((hubHeight - CalibrationPlateHeight) * pixel + PixelDistance1);
            //获取新高度下像素到物理的缩放比例
            double scaleX = 80 / ((hubHeight - CalibrationPlateHeight) * 2.45283 + 650);
            double scaleY = 80 / ((hubHeight - CalibrationPlateHeight) * 2.45283 + 650);

            homMat2D = sourcePixelToPhysicsHomMat2D.Clone();
            homMat2D[0] = scaleX;
            homMat2D[2] = -scaleX * 2560;
            homMat2D[4] = scaleY;
            homMat2D[5] = -scaleY * 2560;
            return sourcePixelToPhysicsHomMat2D;
        }
    }
}
