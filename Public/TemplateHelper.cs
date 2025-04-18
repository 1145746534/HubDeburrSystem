using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using HubDeburrSystem.Models;
using System.Data;
using HubDeburrSystem.ViewModel;
using CommonServiceLocator;
using static HubDeburrSystem.Models.LocusParameterSettingModel;
using System.Runtime.InteropServices;
using NPOI.POIFS.FileSystem;
using NPOI.SS.Formula.Functions;
using HubDeburrSystem.Views.Pages;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using ABB.Robotics.Controllers.RapidDomain;
using static NPOI.SS.Formula.PTG.AttrPtg;


namespace HubDeburrSystem.Public
{
    public class TemplateHelper
    {
        /// <summary>
        /// 相机句柄
        /// </summary>
        public static HTuple CameraHandle = null;

        /// <summary>
        /// 相机标识符
        /// </summary>
        public static string CameraIdentifier = null;

        /// <summary>
        /// 图像坐标转物理坐标的齐次变换矩阵
        /// </summary>
        public static HTuple PixelToPhysicsHomMat2D { get; set; }

        /// <summary>
        /// 连接相机
        /// </summary>
        /// <param name="cameraIdentifier">相机标识符</param>
        /// <returns>相机句柄</returns>
        public static HTuple ConnectCamera(string cameraIdentifier)//启动相机
        {
            HOperatorSet.CloseAllFramegrabbers();//释放相机句柄  
            try
            {
                //HOperatorSet.OpenFramegrabber("GigEVision2", 0, 0, 0, 0, 0, 0, "progressive", -1, "default", -1, "false", "default", cameraIdentifier, 0, -1, out HTuple acqHandle);
                HOperatorSet.OpenFramegrabber("MVision", 1, 1, 0, 0, 0, 0, "progressive", 8, "default", -1, "false", "auto", cameraIdentifier, 0, -1, out HTuple acqHandle);
                HOperatorSet.SetFramegrabberParam(acqHandle, "ExposureTime", 11000.0);
                HOperatorSet.SetFramegrabberParam(acqHandle, "TriggerMode", "Off");
                return acqHandle;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 自动设置相机曝光时间
        /// </summary>
        /// <param name="handle">相机句柄</param>
        /// <param name="image">当前图像</param>
        /// <param name="minMean">最小平均亮度</param>
        /// <param name="maxMean">最大平均亮度</param>
        /// <param name="step">曝光值步长</param>
        /// <returns></returns>
        public static HTuple AutoSetCameraExposureTime(HTuple handle, HObject image, int minMean, int maxMean, int step)
        {
            HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
            HOperatorSet.GenRectangle1(out HObject region, 0, 0, height - 1, width - 1);
            HOperatorSet.Rgb1ToGray(image, out HObject grayImage);
            HOperatorSet.Intensity(region, grayImage, out HTuple mean, out _);
            if (mean > minMean && mean < maxMean) return new HTuple();
            HOperatorSet.GetFramegrabberParam(handle, "ExposureTime", out HTuple exposureTime);
            if (mean < minMean)
            {
                exposureTime += step;
                if (exposureTime > 100000) exposureTime = 100000;
                HOperatorSet.SetFramegrabberParam(handle, "ExposureTime", exposureTime);
            }
            else
            {
                exposureTime -= step;
                if (exposureTime < 10000) exposureTime = 10000;
                HOperatorSet.SetFramegrabberParam(handle, "ExposureTime", exposureTime);

            }
            return exposureTime;
        }

        /// <summary>
        /// 获取线性渐变画刷
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static LinearGradientBrush GetLinearGradientBrush(string color)//#88BDE09C
        {
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
            linearGradientBrush.StartPoint = new Point(0, 0);
            linearGradientBrush.EndPoint = new Point(0, 1);
            linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(color), 0.0));
            linearGradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1.0));
            return linearGradientBrush;
        }

        public static LinearGradientBrush GetLinearGradientBrush(Color color)//Colors.red
        {
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
            linearGradientBrush.StartPoint = new Point(0, 0);
            linearGradientBrush.EndPoint = new Point(0, 1);
            linearGradientBrush.GradientStops.Add(new GradientStop(color, 0.0));
            linearGradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1.0));
            return linearGradientBrush;
        }

        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="image">图像</param>
        /// <param name="hWindow">窗口</param>
        /// <param name="isNotStretch">是否不拉伸显示图像</param>
        /// <param name="isEnhancement">是否增强显示图像</param>
        public static void DisplayImage(HObject image, HWindow hWindow, bool isNotStretch, bool isEnhancement)
        {
            hWindow.ClearWindow();
            if (isNotStretch)
            {
                HTuple cwin_Width, cwin_Height;
                HOperatorSet.GetImageSize(image, out HTuple hv_Width, out HTuple hv_Height);//获取图片大小规格
                HOperatorSet.GetWindowExtents(hWindow, out HTuple win_Row, out HTuple win_Col, out HTuple win_Width, out HTuple win_Height);//获取窗体大小规格
                cwin_Height = 1.0 * win_Height / win_Width * hv_Width;//宽不变计算高          
                if (cwin_Height > hv_Height)//宽不变高能容纳
                {
                    cwin_Height = 1.0 * (cwin_Height - hv_Height) / 2;
                    HOperatorSet.SetPart(hWindow, -cwin_Height, 0, cwin_Height + hv_Height, hv_Width);//设置窗体的规格
                }
                else//高不变宽能容纳
                {
                    cwin_Width = 1.0 * win_Width / win_Height * hv_Height;//高不变计算宽
                    cwin_Width = 1.0 * (cwin_Width - hv_Width) / 2;
                    HOperatorSet.SetPart(hWindow, 0, -cwin_Width, hv_Height, cwin_Width + hv_Width);//设置窗体的规格
                }
            }
            if (isEnhancement)
            {
                HOperatorSet.ScaleImage(image, out HObject imageScaled, 2, 1);
                HOperatorSet.DispObj(imageScaled, hWindow);
            }
            else HOperatorSet.DispObj(image, hWindow);
        }

        /// <summary>
        /// 获取轮毂图像
        /// </summary>
        /// <param name="image">图像</param>
        /// <param name="reduceRadius">剪切半径(包含最大轮毂的半径)</param>
        /// <param name="minThreshold">轮毂最小阈值</param>
        /// <param name="radiusAdjust">半径调整（最终输出轮毂图像的半径大小调整）</param>
        /// <returns>轮毂图像</returns>
        public static HObject GetWheelImage(HObject image, int minThreshold, int radiusAdjust)
        {
            //根据设定的外圆大小从图像中剪切出轮毂图像
            HOperatorSet.Threshold(image, out HObject region1, minThreshold, 255);
            HOperatorSet.Connection(region1, out HObject connectedRegions1);
            HOperatorSet.FillUp(connectedRegions1, out HObject regionFillUp1);
            HOperatorSet.SelectShape(regionFillUp1, out HObject selectedRegions, "area", "and", 5000000, 99999999);
            HOperatorSet.SmallestCircle(selectedRegions, out HTuple row, out HTuple column, out HTuple radius);
            if (row.Length < 1) return null;
            //生成外圆轮廓
            HOperatorSet.GenCircleContourXld(out HObject contCircle, row, column, radius + radiusAdjust, 0, 6.28318, "positive", 1);
            //生成外圆区域
            HOperatorSet.GenRegionContourXld(contCircle, out HObject r, "filled");
            //剪切制作模板的区域
            HOperatorSet.ReduceDomain(image, r, out HObject outImage);
            return outImage;
        }

        /// <summary>
        /// 获取轮毂中心
        /// </summary>
        /// <param name="image">图像</param>
        /// <param name="maxThreshold">内圆最大阈值</param>
        /// <param name="centerRow">中心行坐标</param>
        /// <param name="centerColumn">中心列坐标</param>
        /// <returns>中心轮廓</returns>
        public static HObject GetWheelCenter(HObject image, HTuple radius, HTuple measureLength1, out HTuple centerRow, out HTuple centerColumn)
        {
            GetOuterCircle(image, OuterMinThreshold, out HTuple row, out HTuple column, out HTuple outRadius);

            if (row.Length > 0)
            {
                //获取内圆 通过找圆心的方式
                GenInnerCircle(image, row, column, radius, measureLength1, out centerRow, out centerColumn, out HTuple innerCircleRadius);
                if (centerRow.Length > 0)
                {
                    HOperatorSet.GenCircleContourXld(out HObject contCircle, centerRow, centerColumn, innerCircleRadius, 0, 6.28318, "positive", 1);
                    return contCircle;
                }
            }
            centerRow = new HTuple();
            centerColumn = new HTuple();
            return new HObject();



        }

        /// <summary>
        /// 获取外轮廓的中心和半径
        /// </summary>
        /// <param name="image"></param>
        /// <param name="minThreshold"></param>
        /// <param name="centerRow"></param>
        /// <param name="centerColumn"></param>
        /// <param name="OutRadius"></param>
        public static void GetOuterCircle(HObject image, int minThreshold, out HTuple centerRow, out HTuple centerColumn, out HTuple OutRadius)
        {
            HOperatorSet.Threshold(image, out HObject region1, minThreshold, 255);
            HOperatorSet.Connection(region1, out HObject connectedRegions1);
            HOperatorSet.FillUp(connectedRegions1, out HObject regionFillUp1);
            HOperatorSet.SelectShape(regionFillUp1, out HObject selectedRegions, "area", "and", 5000000, 99999999);
            HOperatorSet.InnerCircle(selectedRegions, out centerRow, out centerColumn, out OutRadius);

        }



        /// <summary>
        /// 获取内圆中心点和半径
        /// </summary>
        /// <param name="image"></param>
        /// <param name="OutCenterRow">外圆中心点Row</param>
        /// <param name="OutCenterColumn">外圆中心点Column</param>
        /// <param name="radius">查找内圆工具的半径</param>
        /// <param name="measureLength1">卡尺长度</param>
        /// <param name="InnerCenterRow"></param>
        /// <param name="InnerCenterColumn"></param>
        /// <param name="InnerCenterRadius">内圆半径</param>
        /// <returns></returns>
        public static HObject GenInnerCircle(HObject image, HTuple OutCenterRow, HTuple OutCenterColumn, HTuple radius, HTuple measureLength1, out HTuple InnerCenterRow, out HTuple InnerCenterColumn, out HTuple InnerCenterRadius)
        {
            InnerCenterRow = new HTuple();
            InnerCenterColumn = new HTuple();
            InnerCenterRadius = new HTuple();
            //获取内圆 通过找圆心的方式
            HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
            HOperatorSet.CreateMetrologyModel(out HTuple MetrologyCircleHandle);
            HOperatorSet.SetMetrologyModelImageSize(MetrologyCircleHandle, width, height);
            HOperatorSet.AddMetrologyObjectCircleMeasure(MetrologyCircleHandle, OutCenterRow, OutCenterColumn, radius, measureLength1, 3, 1, 30, new HTuple(), new HTuple(), out HTuple CircleIndex);
            HOperatorSet.SetMetrologyObjectParam(MetrologyCircleHandle, CircleIndex, "num_instances", 1);
            HOperatorSet.SetMetrologyObjectParam(MetrologyCircleHandle, CircleIndex, "min_score", 0.1);
            HOperatorSet.ApplyMetrologyModel(image, MetrologyCircleHandle);

            HOperatorSet.GetMetrologyObjectResult(MetrologyCircleHandle, CircleIndex, "all", "result_type", "all_param", out HTuple hv_Parameter);
            HOperatorSet.GetMetrologyObjectMeasures(out HObject ho_Contours, MetrologyCircleHandle, CircleIndex, "all", out HTuple hv_Row, out HTuple hv_Column); //工具环
            HOperatorSet.GetMetrologyObjectResultContour(out HObject ho_Contour, MetrologyCircleHandle, CircleIndex, "all", 1.5); //环
            if (hv_Parameter.Length > 0)
            {
                InnerCenterRow = hv_Parameter.TupleSelect(0);
                InnerCenterColumn = hv_Parameter.TupleSelect(1);
                InnerCenterRadius = hv_Parameter.TupleSelect(2);
            }                                                                                                                     //

            HOperatorSet.ClearMetrologyModel(MetrologyCircleHandle);
            return ho_Contours;
        }


        /// <summary>
        /// 轮型识别算法
        /// </summary>
        /// <param name="image">需要识别的图像</param>
        /// <param name="templateDatas">用于识别的模板数据</param>
        /// <param name="angleStart">模板起始角度</param>
        /// <param name="angleExtent">模板角度范围</param>
        /// <param name="similarity">系统设定判定识别成功的最小相似度</param>
        /// <returns>识别结果</returns>
        public static IdentifyDataModel IdentifyAlgorithm(HObject image, TemplateDataListModel templateDatas, double angleStart, double angleExtent, double similarity)
        {
            IdentifyDataModel activeIdentifyData = new IdentifyDataModel();
            IdentifyDataModel notActiveIdentifyData = new IdentifyDataModel();
            List<HTuple> activeAngles = new List<HTuple>();
            List<HTuple> notActiveAngles = new List<HTuple>();
            List<string> identifyWheels = new List<string>();

            //活跃模板匹配，并将结果放入对应的匹配结果列表
            if (templateDatas.ActiveTemplateList.Count > 0)
            {
                for (int i = 0; i < templateDatas.ActiveTemplateList.Count; i++)
                {
                    HOperatorSet.FindNccModel(image, templateDatas.ActiveTemplateList[i], angleStart, angleExtent, 0.5, 1, 0.5, "true", 0,
                        out HTuple row, out HTuple column, out HTuple angle, out HTuple score);
                    activeIdentifyData.WheelTypes.Add(templateDatas.ActiveWheelTypeList[i]);
                    activeAngles.Add(angle);

                    if (score < 0.55) activeIdentifyData.Similaritys.Add(0.0);
                    else
                    {
                        activeIdentifyData.Similaritys.Add(Math.Round(score.D, 3));
                        if (score.D >= similarity)
                        {
                            identifyWheels.Add(templateDatas.ActiveWheelTypeList[i]);
                        }
                    }
                }
                //获取活跃模板匹配中的相似度最大值
                activeIdentifyData.Similarity = activeIdentifyData.Similaritys.Max();
            }
            else activeIdentifyData.Similarity = 0.0;
            //如果活跃模板匹配相似度最大值大于等于（系统设定识别成功的最小相似度 + 0.05 ），认为匹配成功 
            if (activeIdentifyData.Similarity >= similarity + 0.05)
            {
                var index = activeIdentifyData.Similaritys.FindIndex(x => x == activeIdentifyData.Similarity);
                activeIdentifyData.IdentifyWheelType = activeIdentifyData.WheelTypes[index];
                activeIdentifyData.Radian = activeAngles[index];
                activeIdentifyData.Radians = activeAngles;
                activeIdentifyData.IdentifyWheels = identifyWheels;
                return activeIdentifyData;
            }
            else
            {
                if (templateDatas.NotActiveTemplateList.Count > 0)
                {
                    for (int i = 0; i < templateDatas.NotActiveTemplateList.Count; i++)
                    {
                        HOperatorSet.FindNccModel(image, templateDatas.NotActiveTemplateList[i], angleStart, angleExtent, 0.5, 1, 0.5, "true", 0,
                            out HTuple row, out HTuple column, out HTuple angle, out HTuple score);

                        notActiveIdentifyData.WheelTypes.Add(templateDatas.NotActiveWheelTypeList[i]);
                        activeIdentifyData.WheelTypes.Add(templateDatas.NotActiveWheelTypeList[i]);

                        notActiveAngles.Add(angle);
                        activeAngles.Add(angle);

                        if (score < 0.55)
                        {
                            notActiveIdentifyData.Similaritys.Add(0.0);
                            activeIdentifyData.Similaritys.Add(0.0);
                        }
                        else
                        {
                            notActiveIdentifyData.Similaritys.Add(Math.Round(score.D, 3));
                            activeIdentifyData.Similaritys.Add(Math.Round(score.D, 3));
                            if (score.D >= similarity)
                            {
                                identifyWheels.Add(templateDatas.NotActiveWheelTypeList[i]);
                            }
                        }
                    }
                    //获取不活跃模板匹配中的相似度最大值
                    notActiveIdentifyData.Similarity = notActiveIdentifyData.Similaritys.Max();
                    //如果活跃模板相似度最大值 大于等于 不活跃模板相似度最大值，且活跃模板相似度最大值 大于等于 设定值，则在活跃模板中识别成功
                    if (activeIdentifyData.Similarity >= notActiveIdentifyData.Similarity && activeIdentifyData.Similarity >= similarity)
                    {
                        var index = activeIdentifyData.Similaritys.FindIndex(x => x == activeIdentifyData.Similarity);
                        activeIdentifyData.IdentifyWheelType = activeIdentifyData.WheelTypes[index];
                        activeIdentifyData.Radian = activeAngles[index];
                        activeIdentifyData.Radians = activeAngles;
                        activeIdentifyData.IdentifyWheels = identifyWheels;
                        return activeIdentifyData;
                    }
                    //如果活跃模板相似度最大值 小于 不活跃模板相似度最大值，且不活跃模板相似度最大值 大于等于 设定值，则在不活跃模板中识别成功
                    else if (activeIdentifyData.Similarity < notActiveIdentifyData.Similarity && notActiveIdentifyData.Similarity >= similarity)
                    {
                        var index = notActiveIdentifyData.Similaritys.FindIndex(x => x == notActiveIdentifyData.Similarity);
                        activeIdentifyData.IdentifyWheelType = notActiveIdentifyData.WheelTypes[index];
                        activeIdentifyData.Similarity = notActiveIdentifyData.Similarity;
                        activeIdentifyData.Radian = notActiveAngles[index];
                        activeIdentifyData.Radians = activeAngles;
                        activeIdentifyData.IdentifyWheels = identifyWheels;
                        return activeIdentifyData;
                    }
                    //识别不成功
                    else return null;
                }
                else
                {
                    if (activeIdentifyData.Similarity >= similarity)
                    {
                        var index = activeIdentifyData.Similaritys.FindIndex(x => x == activeIdentifyData.Similarity);
                        activeIdentifyData.IdentifyWheelType = activeIdentifyData.WheelTypes[index];
                        activeIdentifyData.Radian = activeAngles[index];
                        activeIdentifyData.Radians = activeAngles;
                        activeIdentifyData.IdentifyWheels = identifyWheels;
                        return activeIdentifyData;
                    }
                    else return null;
                }
            }
        }

        /// <summary>
        /// 生成等边三角形
        /// </summary>
        /// <param name="row">中心行坐标</param>
        /// <param name="colmun">中心列坐标</param>
        /// <param name="sideLength">边长</param>
        /// <returns></returns>
        public static HObject GenEquilateralTriangle(HTuple row, HTuple colmun, HTuple sideLength)
        {

            double sideLength_ = sideLength.D;

            HTuple rows = new HTuple();
            HTuple cols = new HTuple();

            HTuple p1row = row;
            HTuple p1col = colmun + sideLength_ / Math.Sqrt(3);
            rows.Append(p1row);
            cols.Append(p1col);

            HTuple p2row = row - sideLength_ / 2;
            HTuple p2col = colmun - sideLength_ / (2 * Math.Sqrt(3));
            rows.Append(p2row);
            cols.Append(p2col);

            HTuple p3row = row + sideLength_ / 2;
            HTuple p3col = colmun - sideLength_ / (2 * Math.Sqrt(3));
            rows.Append(p3row);
            cols.Append(p3col);

            rows.Append(p1row);
            cols.Append(p1col);

            HOperatorSet.GenContourPolygonXld(out HObject contour, rows, cols);
            return contour;
        }

        /// <summary>
        ///生成并显示轨迹
        /// </summary>
        /// <param name="windowHandle">显示的窗口</param>
        /// <param name="locusDatas">轨迹数据</param>
        /// <param name="selectIndex">选中点的索引</param>
        public static void GenAndShowLocus(HWindow windowHandle, List<MachiningPathPosModel> locusDatas, int poseId, HObject image = null)
        {
            if (locusDatas.Count > 0)
            {
                //从最后一条数据的PoseId获取轮廓的总数
                var xldCount = locusDatas[locusDatas.Count - 1].PoseId / 1000;
                //存储十字
                List<HObject> crossList = new List<HObject>();
                //存储矩形 - 圆
                List<HObject> rectangleList = new List<HObject>();
                //存储矩形2 （卡尺）
                List<HObject> rectangle2List = new List<HObject>();
                //存储窗口XLD
                HOperatorSet.GenEmptyObj(out HObject conCatObject);
                //存储窗口箭头的集合
                HOperatorSet.GenEmptyObj(out HObject ArrowList);
                //存储OK的点位置
                HOperatorSet.GenEmptyObj(out HObject conOKXldObject);
                //存储NG的点位置
                HOperatorSet.GenEmptyObj(out HObject conNGXldObject);
                //存储轮廓的中心
                HTuple rowCenter = null;
                HTuple columnCenter = null;
                double expand = CalipersDevExpand; //偏差扩大倍数
                HTuple TmpCtrl_Len1 = CalipersMeaLength;
                HTuple TmpCtrl_Len2 = CalipersMeaWidth;
                HTuple AmplitudeThreshold = CalipersAmpThreshold;
                HTuple Smooth = CalipersSmooth;

                for (int i = 0; i < xldCount; i++)
                {
                    List<MachiningPathPosModel> points = locusDatas.Where(x => x.PoseId >= (i + 1) * 1000 && x.PoseId <= (i + 1) * 1000 + 999).ToList();
                    List<MachiningPathPosModel> ps = locusDatas.Where(x => x.PoseId > (i + 1) * 1000 + 900 && x.PoseId <= (i + 1) * 1000 + 999).ToList();
                    HOperatorSet.GenEmptyObj(out HObject crossObject);
                    HOperatorSet.GenEmptyObj(out HObject rectangleObject);
                    HOperatorSet.GenEmptyObj(out HObject rectangle2Object);
                    HTuple rows = null;
                    HTuple columns = null;
                    //箭头的起点and终点
                    HTuple StartPointRows = null;
                    HTuple StartPointColumns = null;
                    HTuple EndPointRows = null;
                    HTuple EndPointColumns = null;

                    double previousRow = 0; //上一个点的行号
                    double previousCol = 0; //上一个点的列号
                    double nowRow;  //行 - 
                    double nowCol;  //列
                    double nextRow; //下一个点的行号
                    double nextCol; //下一个点的列号
                    List<ActualPathPointModel> actualPathPointModels = new List<ActualPathPointModel>();
                    for (int k = 0; k < points.Count; k++)
                    {
                        //进刀点:生成长为8的等边三角形
                        if (points[k].PoseId % 1000 >= 901 && points[k].PoseId % 1000 <= 903)
                        {
                            HOperatorSet.GenCrossContourXld(out HObject cross1, points[k].Row, points[k].Column, 8, 0);
                            HOperatorSet.ConcatObj(crossObject, cross1, out crossObject);
                            HObject t = GenEquilateralTriangle(points[k].Row, points[k].Column, 2);
                            HOperatorSet.ConcatObj(rectangleObject, t, out rectangleObject);
                        }
                        //出刀点:生成边长为12的等边三角形
                        else if (points[k].PoseId % 1000 >= 904 && points[k].PoseId % 1000 <= 906)
                        {
                            HOperatorSet.GenCrossContourXld(out HObject cross1, points[k].Row, points[k].Column, 8, 0);
                            HOperatorSet.ConcatObj(crossObject, cross1, out crossObject);
                            //var t = GenEquilateralTriangle(points[k].Row, points[k].Column, 4);
                            //HOperatorSet.ConcatObj(rectangleObject, t, out rectangleObject);
                        }
                        //起点：生成半径为8的圆
                        else if (points[k].PoseId % 1000 == 0)
                        {
                            HOperatorSet.GenCrossContourXld(out HObject cross1, points[k].Row, points[k].Column, 8, 0);
                            HOperatorSet.ConcatObj(crossObject, cross1, out crossObject);
                            HOperatorSet.GenCircleContourXld(out HObject contCircle, points[k].Row, points[k].Column, 4, 0, 6.28318, "positive", 1);
                            HOperatorSet.ConcatObj(rectangleObject, contCircle, out rectangleObject);
                            previousRow = points[k].Row;
                            previousCol = points[k].Column;
                        }
                        //其他的生成小圆
                        else
                        {
                            HOperatorSet.GenCrossContourXld(out HObject cross1, points[k].Row, points[k].Column, 8, 0);
                            HOperatorSet.ConcatObj(crossObject, cross1, out crossObject);
                            HOperatorSet.GenCircleContourXld(out HObject contCircle, points[k].Row, points[k].Column, 2, 0, 6.28318, "positive", 1);
                            HOperatorSet.ConcatObj(rectangleObject, contCircle, out rectangleObject);

                            HTuple angle;
                            nowRow = points[k].Row;
                            nowCol = points[k].Column;
                            HTuple angle1;
                            HTuple angle2;
                            HTuple angle4 = new HTuple();

                            if (k < (points.Count - 1))
                            {
                                nextRow = points[k + 1].Row;
                                nextCol = points[k + 1].Column;
                                HOperatorSet.AngleLx(previousRow, previousCol, nextRow, nextCol, out angle); //与水平X轴夹角 = &列的实际夹角+90度
                                HOperatorSet.AngleLx(previousRow, previousCol, nextRow, nextCol, out angle4); //与水平X轴夹角 = &列的实际夹角+90度

                            }
                            else
                            {
                                HOperatorSet.AngleLx(previousRow, previousCol, nowRow, nowCol, out angle); //与水平X轴夹角 = &列的实际夹角+90度
                            }

                            if (image != null)
                            {


                                //将箭头的起点and终点添加到集合中 
                                (double StartX, double StartY, double EndX, double EndY) segment;
                                //基数跟偶数圈的箭头取向要相反
                                //if (i % 2 == 0) //偶数圈                               
                                //    segment = PointCalculator.GenStartAndEndPoints(nowRow, nowCol, (angle.D + 3.14), TmpCtrl_Len1);
                                //else
                                //    segment = PointCalculator.GenStartAndEndPoints(nowRow, nowCol, angle.D, TmpCtrl_Len1);

                                segment = PointCalculator.GenStartAndEndPoints(nowRow, nowCol, (angle.D + 3.14), TmpCtrl_Len1);

                                HOperatorSet.TupleConcat(StartPointRows, segment.StartX, out StartPointRows);
                                HOperatorSet.TupleConcat(StartPointColumns, segment.StartY, out StartPointColumns);
                                HOperatorSet.TupleConcat(EndPointRows, segment.EndX, out EndPointRows);
                                HOperatorSet.TupleConcat(EndPointColumns, segment.EndY, out EndPointColumns);

                                HTuple phi = PointCalculator.GenAngle(segment.StartX, segment.StartY, segment.EndX, segment.EndY);

                                HOperatorSet.GenRectangle2ContourXld(out HObject rect, nowRow, nowCol, phi, TmpCtrl_Len1, TmpCtrl_Len2);
                                HOperatorSet.GetImageSize(image, out HTuple imgWidth, out HTuple imgHeight);
                                //图像分辨率 5472*3648
                                //HOperatorSet.GenMeasureRectangle2(endRow, endCol, angleD, TmpCtrl_Len1, TmpCtrl_Len2, imgWidth, imgHeight, "nearest_neighbor", out HTuple MsrHandle_Measure);
                                HOperatorSet.GenMeasureRectangle2(nowRow, nowCol, phi, TmpCtrl_Len1, TmpCtrl_Len2, imgWidth, imgHeight, "nearest_neighbor", out HTuple MsrHandle_Measure);
                                //测量 平滑系数 3
                                HOperatorSet.MeasurePos(image, MsrHandle_Measure, Smooth, AmplitudeThreshold, "negative", "last", out HTuple rowEdge, out HTuple colEdge
                                    , out HTuple Amplitude_Measure, out HTuple Distance_Measure);
                                //positive

                                if (rowEdge.Length > 0)
                                {

                                    points[k].rowEdge = rowEdge;
                                    points[k].columnEdge = colEdge;
                                    points[k].rowDeviationAbs = Math.Abs(nowRow - rowEdge[0].D);
                                    points[k].columnDeviationAbs = Math.Abs(nowCol - colEdge[0].D);

                                    HOperatorSet.AngleLx(previousRow, previousCol, nowRow, nowCol, out angle1); //与水平X轴夹角 = &列的实际夹角+90度
                                    //实际路径的两点斜率
                                    HOperatorSet.AngleLx(points[k - 1].rowEdge, points[k - 1].columnEdge, rowEdge[0].D, colEdge[0].D, out angle2); //与水平X轴夹角 = &列的实际夹角+90度

                                    double slopeDifference = Math.Abs(angle2[0].D - angle1[0].D);

                                    HOperatorSet.AngleLl(previousRow, previousCol, nowRow, nowCol, points[k - 1].rowEdge, points[k - 1].columnEdge, rowEdge[0].D, colEdge[0].D, out HTuple angle3);
                                    //HOperatorSet.AngleLl(1654, 1198, 1647, 1156, 1643, 1200, 1612, 1162, out angle3);

                                    //算上一个点的实际点与轨迹点的偏差  -然后推算当前的实际点 - 再算推算点与当前实际点的偏差
                                    double devRow = points[k - 1].rowEdge - points[k - 1].Row;
                                    double devCol = points[k - 1].columnEdge - points[k - 1].Column;

                                    double reckonRow = points[k].Row + devRow;
                                    double reckonCol = points[k].Column + devCol;

                                    double diff_RowAbs = Math.Abs(rowEdge[0].D - reckonRow);
                                    double diff_ColAbs = Math.Abs(colEdge[0].D - reckonCol);

                                    if (Math.Abs(angle3.D) > 0.8)
                                    {
                                        //points[k].IsAccord = false;
                                        //Console.WriteLine($"{k}点，前：{angle1?.D} 后：{angle4?.D} 实际角度：{angle2.D} 前-实 差值：{angle3.D}");
                                    }
                                    if (diff_RowAbs > 20 || diff_ColAbs > 20)
                                    {
                                        //0points[k].IsAccord = false;
                                        Console.WriteLine($"当前实际点{rowEdge}，{colEdge} ：推算点：{reckonRow}，{reckonCol} ：R:{rowEdge - reckonRow} ,{colEdge - reckonCol} ");
                                    }

                                    actualPathPointModels.Add(new ActualPathPointModel()
                                    {
                                        IndexK = k,
                                        ActualRowAbs = points[k].rowDeviationAbs,
                                        ActualColumnAbs = points[k].columnDeviationAbs
                                    });

                                }



                                HOperatorSet.ConcatObj(rectangle2Object, rect, out rectangle2Object);
                            }

                            previousRow = nowRow;
                            previousCol = nowCol;

                        }
                        HOperatorSet.TupleConcat(rows, points[k].Row, out rows);
                        HOperatorSet.TupleConcat(columns, points[k].Column, out columns);



                    }
                    //每一个轮廓都筛选一次-
                    //绝对偏差的中位数
                    if (actualPathPointModels.Count > 0)
                    {
                        List<ActualPathPointModel> actualPathPointXs = actualPathPointModels.OrderBy(x => x.ActualRowAbs).ToList();
                        double medianX = actualPathPointXs[actualPathPointXs.Count / 2].ActualRowAbs; //中位数X
                        List<ActualPathPointModel> actualPathPointYs = actualPathPointModels.OrderBy(y => y.ActualColumnAbs).ToList();
                        double medianY = actualPathPointYs[actualPathPointYs.Count / 2].ActualColumnAbs; //中位数Y
                                                                                                         //得到数据之后做数据筛选

                        for (int n = 0; n < actualPathPointModels.Count; n++)
                        {
                            int k = actualPathPointModels[n].IndexK; //取出索引
                            double rowAbs = actualPathPointModels[n].ActualRowAbs; //取出绝对偏差Row
                            double colAbs = actualPathPointModels[n].ActualColumnAbs; //取出绝对偏差Column
                            if (points[k].IsAccord)
                            {
                                //判断偏差是否符合范围
                                if (0 < rowAbs && rowAbs < expand * medianX && 0 < colAbs && colAbs < expand * medianY)
                                {
                                    //判断一下当前点的偏差是不是大于前两个点偏差
                                    points[k].IsAccord = true;
                                    HOperatorSet.GenCrossContourXld(out HObject OKXld, points[k].rowEdge, points[k].columnEdge, 8, 0);
                                    HOperatorSet.ConcatObj(conOKXldObject, OKXld, out conOKXldObject);
                                }
                                else
                                {
                                    points[k].IsAccord = false;
                                    HOperatorSet.GenCrossContourXld(out HObject NgXld, points[k].rowEdge, points[k].columnEdge, 8, 0);
                                    HOperatorSet.ConcatObj(conNGXldObject, NgXld, out conNGXldObject);
                                }
                            }
                            else
                            {
                                HOperatorSet.GenCrossContourXld(out HObject NgXld, points[k].rowEdge, points[k].columnEdge, 8, 0);
                                HOperatorSet.ConcatObj(conNGXldObject, NgXld, out conNGXldObject);
                            }

                        }
                    }


                    HOperatorSet.GenContourPolygonXld(out HObject contour, rows, columns);
                    HOperatorSet.ConcatObj(conCatObject, contour, out conCatObject);
                    HOperatorSet.AreaCenterXld(contour, out HTuple area, out HTuple row, out HTuple column, out HTuple pointOrder);
                    HOperatorSet.TupleConcat(rowCenter, row, out rowCenter);
                    HOperatorSet.TupleConcat(columnCenter, column, out columnCenter);
                    crossList.Add(crossObject);
                    rectangleList.Add(rectangleObject);
                    rectangle2List.Add(rectangle2Object);

                    //箭头
                    if (EndPointRows != null && EndPointRows.Length > 0)
                    {
                        Gen_arrow_contour_xld(out HObject ho_Arrow, StartPointRows, StartPointColumns, EndPointRows, EndPointColumns, 10, 10);
                        HOperatorSet.ConcatObj(ArrowList, ho_Arrow, out ArrowList);
                    }

                }

                //========================显示=================================
                windowHandle.SetColor("magenta");
                windowHandle.DispObj(conCatObject);
                windowHandle.SetColor("red");
                windowHandle.DispObj(conNGXldObject);
                //显示箭头
                windowHandle.DispObj(ArrowList);
                windowHandle.SetColor("yellow");
                windowHandle.DispObj(conOKXldObject);

                //显示轨迹点
                for (int i = 0; i < crossList.Count; i++)
                {
                    //windowHandle.DispObj(crossList[i]);

                    windowHandle.DispObj(rectangleList[i]);
                    windowHandle.DispObj(rectangle2List[i]);
                }
                //显示轮廓序号和顶点总数
                for (int i = 0; i < rowCenter.Length; i++)
                {
                    HOperatorSet.SetTposition(windowHandle, rowCenter[i], columnCenter[i]);
                    HOperatorSet.WriteString(windowHandle, (i + 1).ToString() + " := " + crossList[i].CountObj().ToString());
                }
                //选中点显示
                int index = locusDatas.FindIndex(x => x.PoseId == poseId);
                if (index >= 0)
                {
                    windowHandle.SetColor("red");
                    HOperatorSet.GenCrossContourXld(out HObject cross, locusDatas[index].Row, locusDatas[index].Column, 8, 0);
                    HOperatorSet.GenCircleContourXld(out HObject circle, locusDatas[index].Row, locusDatas[index].Column, 2, 0, 6.28318, "positive", 1);
                    windowHandle.DispObj(cross);
                    windowHandle.DispObj(circle);
                }
            }
        }


        public static void Gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1,
    HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_TempArrow = null;

            // Local control variables 

            HTuple hv_Length = new HTuple(), hv_ZeroLengthIndices = new HTuple();
            HTuple hv_DR = new HTuple(), hv_DC = new HTuple(), hv_HalfHeadWidth = new HTuple();
            HTuple hv_RowP1 = new HTuple(), hv_ColP1 = new HTuple();
            HTuple hv_RowP2 = new HTuple(), hv_ColP2 = new HTuple();
            HTuple hv_Index = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            HOperatorSet.GenEmptyObj(out ho_TempArrow);
            //This procedure generates arrow shaped XLD contours,
            //pointing from (Row1, Column1) to (Row2, Column2).
            //If starting and end point are identical, a contour consisting
            //of a single point is returned.
            //
            //input parameteres:
            //Row1, Column1: Coordinates of the arrows' starting points
            //Row2, Column2: Coordinates of the arrows' end points
            //HeadLength, HeadWidth: Size of the arrow heads in pixels
            //
            //output parameter:
            //Arrow: The resulting XLD contour
            //
            //The input tuples Row1, Column1, Row2, and Column2 have to be of
            //the same length.
            //HeadLength and HeadWidth either have to be of the same length as
            //Row1, Column1, Row2, and Column2 or have to be a single element.
            //If one of the above restrictions is violated, an error will occur.
            //
            //
            //Init
            ho_Arrow.Dispose();
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            //
            //Calculate the arrow length
            hv_Length.Dispose();
            HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);
            //
            //Mark arrows with identical start and end point
            //(set Length to -1 to avoid division-by-zero exception)
            hv_ZeroLengthIndices.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ZeroLengthIndices = hv_Length.TupleFind(
                    0);
            }
            if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
            {
                if (hv_Length == null)
                    hv_Length = new HTuple();
                hv_Length[hv_ZeroLengthIndices] = -1;
            }
            //
            //Calculate auxiliary variables.
            hv_DR.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;
            }
            hv_DC.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;
            }
            hv_HalfHeadWidth.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_HalfHeadWidth = hv_HeadWidth / 2.0;
            }
            //
            //Calculate end points of the arrow head.
            hv_RowP1.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);
            }
            hv_ColP1.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);
            }
            hv_RowP2.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);
            }
            hv_ColP2.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);
            }
            //
            //Finally create output XLD contour for each input point pair
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {
                if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
                {
                    //Create_ single points for arrows with identical start and end point
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_TempArrow.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(hv_Index),
                            hv_Column1.TupleSelect(hv_Index));
                    }
                }
                else
                {
                    //Create arrow contour
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_TempArrow.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_TempArrow, ((((((((((hv_Row1.TupleSelect(
                            hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                            hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                            hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
                            ((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
                            hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
                            hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
                            hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));
                    }
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_Arrow, ho_TempArrow, out ExpTmpOutVar_0);
                    ho_Arrow.Dispose();
                    ho_Arrow = ExpTmpOutVar_0;
                }
            }
            ho_TempArrow.Dispose();

            hv_Length.Dispose();
            hv_ZeroLengthIndices.Dispose();
            hv_DR.Dispose();
            hv_DC.Dispose();
            hv_HalfHeadWidth.Dispose();
            hv_RowP1.Dispose();
            hv_ColP1.Dispose();
            hv_RowP2.Dispose();
            hv_ColP2.Dispose();
            hv_Index.Dispose();

            return;
        }

        /// <summary>
        /// 根据出刀点生成轨迹数据
        /// </summary>
        /// <param name="locusDatas">轨迹数据</param>
        /// <param name="outPoint">出刀点数据</param>
        /// <returns>根据出刀点生成的轨迹数据</returns>
        public static List<MachiningPathPosModel> GenLocusOfOutPoint(List<MachiningPathPosModel> locusDatas, string outPoint, int spokeType)
        {
            List<MachiningPathPosModel> ds = new List<MachiningPathPosModel>();
            int id = 1;
            //将字符串分割成数组
            string[] str = outPoint.Split(',');
            int xldCount = locusDatas[locusDatas.Count - 1].PoseId / 1000;
            for (int i = 0; i < xldCount; i++)
            {
                //进刀点数据
                var inPoints = locusDatas.Where(x => x.PoseId >= (i + 1) * 1000 + 901 && x.PoseId <= (i + 1) * 1000 + 903).ToList();
                for (int j = 0; j < inPoints.Count; j++)
                {
                    MachiningPathPosModel data = new MachiningPathPosModel
                    {
                        Id = id,
                        PoseId = inPoints[j].PoseId,
                        Row = inPoints[j].Row,
                        Column = inPoints[j].Column,
                        X = inPoints[j].X,
                        Y = inPoints[j].Y,
                        Z = inPoints[j].Z,
                        EX = inPoints[j].EX,
                        EY = inPoints[j].EY,
                        EZ = inPoints[j].EZ,
                        Q1 = inPoints[j].Q1,
                        Q2 = inPoints[j].Q2,
                        Q3 = inPoints[j].Q3,
                        Q4 = inPoints[j].Q4
                    };
                    ds.Add(data);
                    id++;
                }
                //轨迹数据
                List<MachiningPathPosModel> points = locusDatas.Where(x => x.PoseId >= (i + 1) * 1000 && x.PoseId <= (i + 1) * 1000 + 900).ToList();
                for (int j = 0; j < points.Count; j++)
                {
                    MachiningPathPosModel data = new MachiningPathPosModel
                    {
                        Id = id,
                        PoseId = points[j].PoseId,
                        Row = points[j].Row,
                        Column = points[j].Column,
                        X = points[j].X,
                        Y = points[j].Y,
                        Z = points[j].Z,
                        EX = points[j].EX,
                        EY = points[j].EY,
                        EZ = points[j].EZ,
                        Q1 = points[j].Q1,
                        Q2 = points[j].Q2,
                        Q3 = points[j].Q3,
                        Q4 = points[j].Q4
                    };
                    ds.Add(data);
                    id++;
                }
                //获取当前出刀点
                if (int.Parse(str[i % spokeType]) != 0)
                {
                    int n = int.Parse(str[i % spokeType]);
                    if (n != points.Count - 1)
                    {
                        int startPosid = points[points.Count - 1].PoseId + 1;
                        for (int j = 0; j <= n; j++)
                        {
                            MachiningPathPosModel data = new MachiningPathPosModel
                            {
                                Id = id,
                                PoseId = startPosid,
                                Row = points[j].Row,
                                Column = points[j].Column,
                                X = points[j].X,
                                Y = points[j].Y,
                                Z = points[j].Z,
                                EX = points[j].EX,
                                EY = points[j].EY,
                                EZ = points[j].EZ,
                                Q1 = points[j].Q1,
                                Q2 = points[j].Q2,
                                Q3 = points[j].Q3,
                                Q4 = points[j].Q4
                            };
                            ds.Add(data);
                            id++;
                            startPosid++;
                        }
                    }
                }

                //出刀的数据
                List<MachiningPathPosModel> outPoints = locusDatas.Where(x => x.PoseId >= (i + 1) * 1000 + 904 && x.PoseId <= (i + 1) * 1000 + 906).ToList();
                for (int j = 0; j < outPoints.Count; j++)
                {
                    MachiningPathPosModel data = new MachiningPathPosModel
                    {
                        Id = id,
                        PoseId = outPoints[j].PoseId,
                        Row = outPoints[j].Row,
                        Column = outPoints[j].Column,
                        X = outPoints[j].X,
                        Y = outPoints[j].Y,
                        Z = outPoints[j].Z,
                        EX = outPoints[j].EX,
                        EY = outPoints[j].EY,
                        EZ = outPoints[j].EZ,
                        Q1 = outPoints[j].Q1,
                        Q2 = outPoints[j].Q2,
                        Q3 = outPoints[j].Q3,
                        Q4 = outPoints[j].Q4
                    };
                    ds.Add(data);
                    id++;
                }
            }

            return ds;
        }

        /// <summary>
        /// 根据出刀点生成轨迹数据
        /// </summary>
        /// <param name="locusDatas">轨迹数据</param>
        /// <param name="outPoint">出刀点数据</param>
        /// <returns>根据出刀点生成的轨迹数据</returns>
        public static List<MachiningPathPosModel> GenLocusOfOutPoint1(List<MachiningPathPosModel> locusDatas, string outPoint, int spokeType)
        {
            List<MachiningPathPosModel> ds = new List<MachiningPathPosModel>();
            int id = 1;
            //将字符串分割成数组
            string[] str = outPoint.Split(',');
            int xldCount = locusDatas[locusDatas.Count - 1].PoseId / 1000;
            for (int i = 0; i < xldCount; i++)
            {
                //进刀点数据
                var inPoints = locusDatas.Where(x => x.PoseId >= (i + 1) * 1000 + 901 && x.PoseId <= (i + 1) * 1000 + 903).ToList();
                for (int j = 0; j < inPoints.Count; j++)
                {
                    MachiningPathPosModel data = new MachiningPathPosModel
                    {
                        Id = id,
                        PoseId = inPoints[j].PoseId,
                        Row = inPoints[j].Row,
                        Column = inPoints[j].Column,
                        X = inPoints[j].X,
                        Y = inPoints[j].Y,
                        Z = inPoints[j].Z,
                        EX = inPoints[j].EX,
                        EY = inPoints[j].EY,
                        EZ = inPoints[j].EZ,
                        Q1 = inPoints[j].Q1,
                        Q2 = inPoints[j].Q2,
                        Q3 = inPoints[j].Q3,
                        Q4 = inPoints[j].Q4
                    };
                    ds.Add(data);
                    id++;
                }
                //轨迹数据
                List<MachiningPathPosModel> points = locusDatas.Where(x => x.PoseId >= (i + 1) * 1000 && x.PoseId <= (i + 1) * 1000 + 900).ToList();
                for (int j = 0; j < points.Count; j++)
                {
                    MachiningPathPosModel data = new MachiningPathPosModel
                    {
                        Id = id,
                        PoseId = points[j].PoseId,
                        Row = points[j].Row,
                        Column = points[j].Column,
                        X = points[j].X,
                        Y = points[j].Y,
                        Z = points[j].Z,
                        EX = points[j].EX,
                        EY = points[j].EY,
                        EZ = points[j].EZ,
                        Q1 = points[j].Q1,
                        Q2 = points[j].Q2,
                        Q3 = points[j].Q3,
                        Q4 = points[j].Q4
                    };
                    ds.Add(data);
                    id++;
                }
                //获取当前出刀点

                int n = int.Parse(str[i]);
                if (n != 0 && n != points.Count - 1)
                {
                    int startPosid = points[points.Count - 1].PoseId + 1;
                    for (int j = 0; j <= n; j++)
                    {
                        MachiningPathPosModel data = new MachiningPathPosModel
                        {
                            Id = id,
                            PoseId = startPosid,
                            Row = points[j].Row,
                            Column = points[j].Column,
                            X = points[j].X,
                            Y = points[j].Y,
                            Z = points[j].Z,
                            EX = points[j].EX,
                            EY = points[j].EY,
                            EZ = points[j].EZ,
                            Q1 = points[j].Q1,
                            Q2 = points[j].Q2,
                            Q3 = points[j].Q3,
                            Q4 = points[j].Q4
                        };
                        ds.Add(data);
                        id++;
                        startPosid++;
                    }
                }


                //出刀的数据
                List<MachiningPathPosModel> outPoints = locusDatas.Where(x => x.PoseId >= (i + 1) * 1000 + 904 && x.PoseId <= (i + 1) * 1000 + 906).ToList();
                for (int j = 0; j < outPoints.Count; j++)
                {
                    MachiningPathPosModel data = new MachiningPathPosModel
                    {
                        Id = id,
                        PoseId = outPoints[j].PoseId,
                        Row = outPoints[j].Row,
                        Column = outPoints[j].Column,
                        X = outPoints[j].X,
                        Y = outPoints[j].Y,
                        Z = outPoints[j].Z,
                        EX = outPoints[j].EX,
                        EY = outPoints[j].EY,
                        EZ = outPoints[j].EZ,
                        Q1 = outPoints[j].Q1,
                        Q2 = outPoints[j].Q2,
                        Q3 = outPoints[j].Q3,
                        Q4 = outPoints[j].Q4
                    };
                    ds.Add(data);
                    id++;
                }
            }

            return ds;
        }


        /// <summary>
        /// 获取窗口轮廓（阈值算法）
        /// </summary>
        /// <param name="image">获取的源图像</param>
        /// <param name="contour">初始轮廓</param>
        /// <param name="darkMaxThreshold">暗部最大阈值</param>
        /// <param name="lightMinThreshold">亮部最小阈值</param>
        /// <returns></returns>
        public static HObject GetWindowContours(HObject image, HObject contour, int darkMaxThreshold, int lightMinThreshold, double xldDilation)
        {
            //=========获取窗口轮廓=========
            HOperatorSet.GenRegionContourXld(contour, out HObject sourceRegion, "filled");
            HOperatorSet.DilationRectangle1(sourceRegion, out HObject sourceDilation, xldDilation, xldDilation);
            HOperatorSet.ReduceDomain(image, sourceDilation, out HObject sourceImage);
            HOperatorSet.MeanImage(sourceImage, out HObject sourceImageMean, 5, 5);

            HOperatorSet.Threshold(sourceImageMean, out HObject sourceThreshold, 0, darkMaxThreshold);
            HOperatorSet.Connection(sourceThreshold, out HObject winRegionConnection);
            HOperatorSet.FillUp(winRegionConnection, out HObject winRegionFillUp);


            HOperatorSet.AreaCenter(winRegionFillUp, out HTuple area, out HTuple row, out HTuple column);
            HOperatorSet.TupleSortIndex(-area, out HTuple indices);
            HOperatorSet.SelectObj(winRegionFillUp, out HObject maxContour, indices[0] + 1);
            HOperatorSet.GenContourRegionXld(maxContour, out HObject contours, "border");
            HOperatorSet.SmoothContoursXld(contours, out HObject smoothedContours, 9);
            return smoothedContours;
        }

        /// <summary>
        /// 获取窗口轮廓(边缘算法)
        /// </summary>
        /// <param name="image"></param>
        /// <param name="contour"></param>
        /// <param name="filter"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="xldMinLenght"></param>
        /// <returns></returns>
        public static HObject GetWindowContours(HObject image, HObject contour, double filter, int low, int high, int xldMinLenght, double xldDilation, int mask)
        {
            //获取窗口轮廓
            HOperatorSet.GenRegionContourXld(contour, out HObject region, "filled");
            HOperatorSet.DilationRectangle1(region, out HObject regionDilation, xldDilation, xldDilation);
            HOperatorSet.ReduceDomain(image, regionDilation, out HObject imageReduced);
            //中值平均
            HOperatorSet.MeanImage(imageReduced, out HObject imageMean, 5, 5);
            //对比度增强
            HOperatorSet.Emphasize(imageMean, out HObject imageEmphasize, mask, mask, 29);
            //边缘算子
            HOperatorSet.EdgesSubPix(imageEmphasize, out HObject edges, "canny", filter, low, high);
            HOperatorSet.SelectShapeXld(edges, out HObject selectedXLD, "contlength", "and", xldMinLenght, 99999);
            HOperatorSet.UnionAdjacentContoursXld(selectedXLD, out HObject unionContours, 50, 1, "attr_keep");
            HOperatorSet.AreaCenterXld(unionContours, out HTuple area, out HTuple row, out HTuple col, out HTuple p);
            if (area.Length == 0) return null;
            HOperatorSet.TupleSortIndex(-area, out HTuple indices);
            HOperatorSet.SelectObj(unionContours, out HObject contourSelected, indices[0] + 1);
            HOperatorSet.CloseContoursXld(contourSelected, out HObject closedContours);
            HOperatorSet.GenRegionContourXld(closedContours, out HObject xldToRegion, "filled");
            HOperatorSet.DilationRectangle1(xldToRegion, out HObject xldToRegionDilation, xldDilation, xldDilation);
            HOperatorSet.ErosionRectangle1(xldToRegionDilation, out HObject xldToRegionErosion, xldDilation, xldDilation);
            HOperatorSet.GenContourRegionXld(xldToRegionErosion, out HObject contours, "border");
            HOperatorSet.SmoothContoursXld(contours, out HObject smoothedContours, 9);
            return smoothedContours;
        }

        /// <summary>
        /// 获取旋转后的点
        /// </summary>
        /// <param name="pRow">待旋转点的行坐标</param>
        /// <param name="pCol">待旋转点的列坐标</param>
        /// <param name="centerRow">旋转中心行坐标</param>
        /// <param name="centerCol">旋转中心列坐标</param>
        /// <param name="rad">旋转的弧度</param>
        /// <param name="newRow">旋转后新点的行坐标</param>
        /// <param name="newCol">旋转后新点的列坐标</param>
        public static void GetThePointAfterRotation(HTuple pRow, HTuple pCol, HTuple centerRow, HTuple centerCol, HTuple rad, out HTuple newRow, out HTuple newCol)
        {
            newRow = rad.TupleCos() * (pRow - centerRow) - rad.TupleSin() * (pCol - centerCol) + centerRow;
            newCol = rad.TupleCos() * (pCol - centerCol) + rad.TupleSin() * (pRow - centerRow) + centerCol;
        }

        /// <summary>
        /// 选择模板算法（当识别结果出现多个时判断哪个轨迹模板最适合当前轮毂）
        /// </summary>
        /// <param name="image">图像</param>
        /// <param name="datas">识别的结果</param>
        /// <param name="centerRow">图像中心行坐标</param>
        /// <param name="centerColumn">图像中心列坐标</param>
        /// <returns></returns>
        public static string SelectTemplateAlgorithm(HObject image, IdentifyDataModel datas, HTuple centerRow, HTuple centerColumn)
        {
            //存储识别出的轮毂原始窗口轨迹的最小外接矩形的长短边
            List<HTuple> longShortLengths = new List<HTuple>();
            //存储识别出的轮毂原始窗口轨迹的最小外接矩形的长短边之和
            HTuple sums = new HTuple();
            //存储识别出的轮毂窗口轨迹
            List<HObject> winContours = new List<HObject>();
            for (int i = 0; i < datas.IdentifyWheels.Count; i++)
            {
                var locusIndex = LocusPageViewModel.ProcessingLocusDatas.LocusName.FindIndex(x => x == datas.IdentifyWheels[i]);
                if (locusIndex >= 0 && LocusPageViewModel.ProcessingLocusDatas.LocusPoints[locusIndex].Count > 0)
                {
                    //==============获取当前轨迹的窗口种类
                    var locusDatas = LocusPageViewModel.ProcessingLocusDatas.LocusPoints[locusIndex];
                    //从最后一条数据的PoseId获取当前轮毂轮廓的总数
                    var xldCount1 = locusDatas[locusDatas.Count - 1].PoseId / 1000;
                    //获取模板数据
                    var templateData = ServiceLocator.Current.GetInstance<TemplatePageViewModel>().ProcessingTemplateDatas.First(x => x.WheelType == datas.IdentifyWheels[i]);
                    //窗口种类
                    var tt = xldCount1 / templateData.SpokeQuantity;
                    //存储原始窗口轮廓
                    HOperatorSet.GenEmptyObj(out HObject winContour);
                    HTuple longShortLength = null;
                    for (int j = 0; j < tt; j++)
                    {
                        //从轨迹数据中生成旧窗口轮廓
                        HTuple rows = null;
                        HTuple columns = null;
                        //获取第k个窗口的轨迹数据
                        var points = locusDatas.Where(x => x.PoseId >= (j + 1) * 1000 && x.PoseId <= (j + 1) * 1000 + 900).ToList();
                        for (int t = 0; t < points.Count; t++)
                        {
                            HOperatorSet.TupleConcat(rows, points[t].Row, out rows);
                            HOperatorSet.TupleConcat(columns, points[t].Column, out columns);
                        }
                        HOperatorSet.GenContourPolygonXld(out HObject contour, rows, columns);
                        //计算最小外接矩形:length1为长边边长，length2为短边边长
                        HOperatorSet.SmallestRectangle2Xld(contour, out HTuple row, out HTuple column, out HTuple phi, out HTuple length1, out HTuple length2);
                        HOperatorSet.TupleConcat(longShortLength, length1, out longShortLength);
                        HOperatorSet.TupleConcat(longShortLength, length2, out longShortLength);
                        HOperatorSet.ConcatObj(winContour, contour, out winContour);
                    }
                    winContours.Add(winContour);
                    longShortLengths.Add(longShortLength);
                    //计算最小外接矩形长短边之和
                    HOperatorSet.TupleSum(longShortLength, out HTuple sum);
                    HOperatorSet.TupleConcat(sums, sum, out sums);
                }
            }
            HOperatorSet.TupleMax(sums, out HTuple max);
            //最大矩形索引
            HOperatorSet.TupleFindFirst(sums, max, out HTuple index1);
            //模板数据
            var d = ServiceLocator.Current.GetInstance<TemplatePageViewModel>().ProcessingTemplateDatas.First(x => x.WheelType == datas.IdentifyWheels[index1.I]);
            var wheelIndex = datas.WheelTypes.FindIndex(x => x == datas.IdentifyWheels[index1.I]);
            HTuple length = null;
            //根据中心和角度生成仿射矩阵
            HOperatorSet.VectorAngleToRigid(d.CenterRow, d.CenterColumn, 0, centerRow, centerColumn, datas.Radians[wheelIndex], out HTuple homMat2D1);
            //旧XLD仿射变换到新位置
            HOperatorSet.AffineTransContourXld(winContours[index1.I], out HObject contoursAffineTran, homMat2D1);
            HOperatorSet.CountObj(contoursAffineTran, out HTuple number2);
            for (int i = 0; i < number2; i++)
            {
                HOperatorSet.SelectObj(contoursAffineTran, out HObject objectSelected, i + 1);
                HObject contours = GetWindowContours(image, objectSelected, d.DarkMaxThreshold, d.LightMinThreshold, SingleXldDilation);
                //获取区域的最小外接矩形
                HOperatorSet.SmallestRectangle2Xld(contours, out HTuple row, out HTuple column, out HTuple phi, out HTuple length1, out HTuple length2);
                //存储长短边
                HOperatorSet.TupleConcat(length, length1, out length);
                HOperatorSet.TupleConcat(length, length2, out length);
            }
            //计算窗口实际最小外接矩形长短边之和
            HOperatorSet.TupleSum(length, out HTuple sum1);
            //与原始窗口轨迹的最小外接矩形的长短边之和做比较，选差值最小的作为轨迹模板
            HTuple decData = new HTuple();
            for (int i = 0; i < sums.Length; i++)
            {
                HOperatorSet.TupleAbs(sums[i] - sum1, out HTuple abs);
                HOperatorSet.TupleConcat(decData, abs, out decData);
            }
            HOperatorSet.TupleMin(decData, out HTuple min);
            HOperatorSet.TupleFindFirst(decData, min, out HTuple index2);

            return datas.IdentifyWheels[index2];
        }

        /// <summary>
        /// 大毛刺判断
        /// </summary>
        /// <param name="affineImage">模板图像仿射后的图像</param>
        /// <param name="currentImage">当前图像</param>
        /// <param name="affineContours">模板轮廓仿射后的轮廓</param>
        /// <returns>毛刺轮廓</returns>
        public static HObject LargeBurrJudgment(HObject affineImage, HObject currentImage, HObject affineContours)
        {
            //存储毛刺轮廓
            HOperatorSet.GenEmptyObj(out HObject burrContours);
            int number = affineContours.CountObj();
            for (int i = 1; i <= number; i++)
            {
                HObject selectObj = affineContours.SelectObj(i);
                HOperatorSet.CloseContoursXld(selectObj, out HObject closedContours);
                //生成窗口区域
                HOperatorSet.GenRegionContourXld(closedContours, out HObject region, "filled");
                //=======判断有没有亮部区域======
                HOperatorSet.ReduceDomain(affineImage, region, out HObject imageReduced);
                //亮部区域阈值128-255
                //HOperatorSet.Threshold(imageReduced, out HObject tRegion, 128, 255);
                HOperatorSet.Threshold(imageReduced, out HObject tRegion, 188, 255);
                HOperatorSet.Connection(tRegion, out HObject connectedRegions);
                HOperatorSet.SelectShape(connectedRegions, out HObject selectedRegions, "area", "and", 5000, 999999);
                HOperatorSet.Union1(selectedRegions, out HObject regionUnion);
                HOperatorSet.AreaCenter(regionUnion, out HTuple area, out HTuple row1, out HTuple column1);
                //存储亮部区域
                HOperatorSet.GenEmptyObj(out HObject lightRegion);
                if (area > 10000)
                {
                    HOperatorSet.SmallestRectangle2(regionUnion, out HTuple row, out HTuple col, out HTuple phi, out HTuple length1, out HTuple length2);
                    HOperatorSet.GenRectangle2(out HObject rectangleRegion, row, col, phi, length1, length2);
                    HOperatorSet.DilationCircle(rectangleRegion, out lightRegion, 20);
                }
                //窗口区域减亮部区域
                HOperatorSet.Difference(region, lightRegion, out HObject regionDifference);
                //将相减后的区域腐蚀30
                HOperatorSet.ErosionCircle(regionDifference, out HObject regionErosion, 30);
                //得到毛刺区域
                HOperatorSet.Difference(regionDifference, regionErosion, out HObject deburrRegion);
                //获取原始图像中的毛刺区域图像
                HOperatorSet.ReduceDomain(affineImage, deburrRegion, out HObject sourceDeburrImage);
                //获取当前图像中的毛刺区域图像
                HOperatorSet.ReduceDomain(currentImage, deburrRegion, out HObject currentDeburrImage);
                HOperatorSet.SubImage(currentDeburrImage, sourceDeburrImage, out HObject imageSub, 1, 0);
                HOperatorSet.Threshold(imageSub, out HObject region1, 35, 250);
                HOperatorSet.Connection(region1, out HObject connectedRegions1);
                HOperatorSet.OpeningCircle(connectedRegions1, out HObject regionOpening, 3.5);
                //毛刺大小
                HOperatorSet.SelectShape(regionOpening, out HObject selectedRegions1, "area", "and", 400, 1200);
                HOperatorSet.Union1(selectedRegions1, out HObject regionUnion1);
                HOperatorSet.ClosingCircle(regionUnion1, out HObject regionClosing, 3.5);
                HOperatorSet.GenContourRegionXld(regionClosing, out HObject burrContour, "border");
                HOperatorSet.ConcatObj(burrContours, burrContour, out burrContours);
            }
            return burrContours;
        }

        /// <summary>
        /// 根据轨迹数据生成轮廓
        /// </summary>
        /// <param name="datas">加工路径数据</param>
        /// <param name="isInOut">是否包含进出刀点</param>
        /// <returns>轮廓</returns>
        public static HObject GenContours(List<MachiningPathPosModel> datas, bool isInOut)
        {
            //生成空对象，存储加工轮廓数据生成的窗口XLD
            HOperatorSet.GenEmptyObj(out HObject contours);
            //从最后一条数据的PoseId获取当前轮毂轮廓的总数
            var xldCount = datas[datas.Count - 1].PoseId / 1000;
            for (int k = 0; k < xldCount; k++)
            {
                HTuple rows = null;
                HTuple columns = null;
                //获取第k个窗口的轨迹数据
                List<MachiningPathPosModel> points = new List<MachiningPathPosModel>();
                if (isInOut)
                {
                    points = datas.Where(x => x.PoseId >= (k + 1) * 1000 && x.PoseId <= (k + 1) * 1000 + 999).ToList();
                }
                else
                {
                    points = datas.Where(x => x.PoseId >= (k + 1) * 1000 && x.PoseId <= (k + 1) * 1000 + 900).ToList();
                }
                for (int i = 0; i < points.Count; i++)
                {
                    HOperatorSet.TupleConcat(rows, points[i].Row, out rows);
                    HOperatorSet.TupleConcat(columns, points[i].Column, out columns);
                }
                HOperatorSet.GenContourPolygonXld(out HObject contour, rows, columns);
                HOperatorSet.ConcatObj(contours, contour, out contours);
            }
            return contours;
        }

        /// <summary>
        /// 仿射源图像和轮廓到新位置
        /// </summary>
        /// <param name="sourceImage">源图像</param>
        /// <param name="sourceContorus">源轮廓</param>
        /// <param name="sourceRow">源图像中心行坐标</param>
        /// <param name="sourceCol">源图像中心列坐标</param>
        /// <param name="centerRow">仿射中心行坐标</param>
        /// <param name="centerCol">仿射中心列坐标</param>
        /// <param name="radian">仿射弧度</param>
        /// <param name="affineImage">仿射后的源图像</param>
        /// <returns>仿射后的源轮廓</returns>
        public static HObject AffineImageAndContours(HObject sourceImage, HObject sourceContorus, HTuple sourceRow, HTuple sourceCol, HTuple centerRow, HTuple centerCol, HTuple radian, out HObject affineImage)
        {
            //根据中心和角度生成仿射矩阵
            HOperatorSet.VectorAngleToRigid(sourceRow, sourceCol, 0, centerRow, centerCol, radian, out HTuple homMat2D);
            //旧XLD仿射变换到新位置
            HOperatorSet.AffineTransContourXld(sourceContorus, out HObject contoursAffine, homMat2D);
            HOperatorSet.AffineTransImage(sourceImage, out affineImage, homMat2D, "constant", "false");
            return contoursAffine;
        }

        /// <summary>
        /// 轮廓点投影
        /// </summary>
        /// <param name="contourAffine">原始轮廓仿射后的轮廓</param>
        /// <param name="newContours">新轮廓</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="rowList">投影后的轮廓行数据</param>
        /// <param name="colList">投影后的轮廓列数据</param>
        public static HObject ContourProjection(HObject contourAffine, HObject newContours, double maxDistance, out List<HTuple> rowList, out List<HTuple> colList)
        {
            //存储投影的轨迹点
            List<HTuple> projectionRows = new List<HTuple>();
            List<HTuple> projectionCols = new List<HTuple>();
            HOperatorSet.GenEmptyObj(out HObject newXlds);
            HOperatorSet.CountObj(contourAffine, out HTuple number1);
            for (int i = 0; i < number1; i++)
            {
                HOperatorSet.SelectObj(contourAffine, out HObject OldProcessingContoursSelected, i + 1);
                HOperatorSet.SelectObj(newContours, out HObject thresholdContourSelected, i + 1);
                HOperatorSet.GenRegionContourXld(thresholdContourSelected, out HObject thresholdregion, "margin");//margin,filled
                HTuple rows = null, cols = null;
                HOperatorSet.GetContourXld(OldProcessingContoursSelected, out HTuple row, out HTuple col);
                for (int j = 0; j < row.Length; j++)
                {
                    //计算新XLD与旧XLD上的每个顶点的距离
                    HOperatorSet.DistancePc(thresholdContourSelected, row[j], col[j], out HTuple distanceMin, out HTuple distanceMax);
                    //如果距离小于等于一个像素，旧XLD上的顶点坐标就是新XLD上的坐标
                    if (distanceMin < 1 || distanceMin > maxDistance)
                    {
                        HOperatorSet.TupleConcat(rows, Math.Round(row[j].D, 2), out rows);
                        HOperatorSet.TupleConcat(cols, Math.Round(col[j].D, 2), out cols);
                    }
                    else
                    {
                        //生成去重轮廓偏移后以轮廓上顶点为圆心的圆
                        HOperatorSet.GenCircle(out HObject circle1, row[j], col[j], 1);
                        //获取圆与阈值出的区域的最近距离的点
                        HOperatorSet.DistanceRrMin(circle1, thresholdregion, out HTuple minDistance, out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);

                        HOperatorSet.TupleConcat(rows, Math.Round(row2[0].D, 2), out rows);
                        HOperatorSet.TupleConcat(cols, Math.Round(column2[0].D, 2), out cols);
                    }
                }
                projectionRows.Add(rows);
                projectionCols.Add(cols);
                HOperatorSet.GenContourPolygonXld(out HObject contour, rows, cols);
                HOperatorSet.ConcatObj(newXlds, contour, out newXlds);
            }
            rowList = projectionRows;
            colList = projectionCols;
            return newXlds;
        }

        /// <summary>
        /// 轨迹偏移
        /// </summary>
        /// <param name="rows">源轨迹行数据</param>
        /// <param name="cols">源轨迹列数据</param>
        /// <param name="offsetValue">偏移值（像素）</param>
        /// <param name="offsetRows">偏移后的行数据</param>
        /// <param name="offsetCols">偏移后的列数据</param>
        public static void LocusOffset(List<HTuple> rows, List<HTuple> cols, int offsetValue, out List<HTuple> offsetRows, out List<HTuple> offsetCols)
        {
            List<HTuple> offsetR = new List<HTuple>();
            List<HTuple> offsetC = new List<HTuple>();
            //生成投影后的轮廓
            for (int i = 0; i < rows.Count; i++)
            {
                HOperatorSet.GenContourPolygonXld(out HObject contour, rows[i], cols[i]);
                HOperatorSet.GenRegionContourXld(contour, out HObject region, "filled");//margin,filled
                HObject offsetContour = new HObject();
                if (offsetValue > 0)
                {
                    //膨胀设定的像素
                    HOperatorSet.DilationRectangle1(region, out offsetContour, offsetValue, offsetValue);
                }
                else
                {
                    HOperatorSet.TupleAbs(offsetValue, out HTuple abs);
                    //腐蚀设定的像素
                    HOperatorSet.ErosionRectangle1(region, out offsetContour, abs, abs);
                }
                //投影
                HTuple row = new HTuple();
                HTuple col = new HTuple();
                for (int j = 0; j < rows[i].Length; j++)
                {
                    HOperatorSet.GenCircle(out HObject circle, rows[i][j], cols[i][j], 1);
                    HOperatorSet.DistanceRrMin(circle, offsetContour, out HTuple minDistance, out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);
                    HOperatorSet.TupleConcat(row, Math.Round(row2[0].D, 2), out row);
                    HOperatorSet.TupleConcat(col, Math.Round(column2[0].D, 2), out col);
                }
                offsetR.Add(row);
                offsetC.Add(col);
            }
            offsetRows = offsetR;
            offsetCols = offsetC;
        }

        /// <summary>
        ///  轨迹缩放
        /// </summary>
        /// <param name="loucsDatas"></param>
        /// <param name="scaleValue"></param>
        /// <returns></returns>
        public static List<MachiningPathPosModel> LocusScale(List<MachiningPathPosModel> loucsDatas, int scaleValue, HTuple homMat2D)
        {
            List<MachiningPathPosModel> newDatas = new List<MachiningPathPosModel>();
            //轮廓总数
            int contoursCount = loucsDatas[loucsDatas.Count - 1].PoseId / 1000;
            //生成投影后的轮廓
            for (int i = 0; i < contoursCount; i++)
            {

                HTuple rows = new HTuple();
                HTuple columns = new HTuple();
                //获取不包含进出刀点的轮廓数据
                var contourData = loucsDatas.Where(x => x.PoseId >= (i + 1) * 1000 && x.PoseId <= (i + 1) * 1000 + 900).ToList();
                for (int j = 0; j < contourData.Count; j++)
                {
                    HOperatorSet.TupleConcat(rows, contourData[j].Row, out rows);
                    HOperatorSet.TupleConcat(columns, contourData[j].Column, out columns);
                }
                HOperatorSet.GenContourPolygonXld(out HObject contour, rows, columns);
                HOperatorSet.GenRegionContourXld(contour, out HObject region, "filled");//margin,filled
                HObject offsetContour = new HObject();
                if (scaleValue > 0)
                {
                    //膨胀设定的像素
                    HOperatorSet.DilationCircle(region, out offsetContour, scaleValue);
                }
                else
                {
                    HOperatorSet.TupleAbs(scaleValue, out HTuple abs);
                    //腐蚀设定的像素
                    HOperatorSet.ErosionCircle(region, out offsetContour, abs);
                }
                //投影
                for (int j = 0; j < rows.Length; j++)
                {
                    HOperatorSet.GenCircle(out HObject circle, rows[j], columns[j], 1);
                    HOperatorSet.DistanceRrMin(circle, offsetContour, out HTuple minDistance, out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);
                    contourData[j].Row = row2;
                    contourData[j].Column = column2;
                    //获取指定高度下的新齐次变换矩阵
                    var pixelToPhysicsHomMat2D = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(contourData[j].Z, homMat2D);
                    HOperatorSet.AffineTransPoint2d(pixelToPhysicsHomMat2D, contourData[j].Row, contourData[j].Column, out HTuple x, out HTuple y);
                    contourData[j].X = Math.Round(x.D, 2);
                    contourData[j].Y = Math.Round(y.D, 2);
                }
                //构建新数据
                var inDatas = loucsDatas.Where(x => x.PoseId >= (i + 1) * 1000 + 901 && x.PoseId <= (i + 1) * 1000 + 903).ToList();
                for (int j = 0; j < inDatas.Count; j++)
                {
                    newDatas.Add(inDatas[j]);
                }
                for (int j = 0; j < contourData.Count; j++)
                {
                    newDatas.Add(contourData[j]);
                }
                var outDatas = loucsDatas.Where(x => x.PoseId >= (i + 1) * 1000 + 904 && x.PoseId <= (i + 1) * 1000 + 906).ToList();
                for (int j = 0; j < outDatas.Count; j++)
                {
                    newDatas.Add(outDatas[j]);
                }
            }
            return newDatas;
        }

        /// <summary>
        /// 卡尺抓取真实边
        /// </summary>
        /// <param name="loucsDatas"></param>
        /// <param name="homMat2D"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public static List<MachiningPathPosModel> LocusScale1(List<MachiningPathPosModel> loucsDatas, HTuple homMat2D, HObject image, int spokeType, out string[] newOutPoseIDS, string outPoint = null)
        {
            if (image == null)
            {
                newOutPoseIDS = null;
                return loucsDatas;
            }
            HOperatorSet.GetImageSize(image, out HTuple imgWidth, out HTuple imgHeight);
            double expand = CalipersDevExpand; //偏差扩大倍数
            HTuple TmpCtrl_Len1 = CalipersMeaLength;//测量矩形1/2长度
            HTuple TmpCtrl_Len2 = CalipersMeaWidth; //测量矩形1/2宽度
            HTuple AmplitudeThreshold = CalipersAmpThreshold;//分割阈值
            HTuple Smooth = CalipersSmooth;  //平滑参数
            string[] str = outPoint.Split(',');

            List<MachiningPathPosModel> newDatas = new List<MachiningPathPosModel>();
            //轮廓总数
            int contoursCount = loucsDatas[loucsDatas.Count - 1].PoseId / 1000;
            newOutPoseIDS = new string[contoursCount]; //新的出刀点数据-对应每一个轮廓
            int id = 0;
            int index = 0;
            //生成投影后的轮廓
            for (int i = 0; i < contoursCount; i++)
            {
                index = int.Parse(str[i % spokeType]);  //得到出刀点的PoseID
                //设置默认值
                newOutPoseIDS[i] = Convert.ToString(index);
                double previousRow = 0; //上一个点的行
                double previousCol = 0; ///上一个点的列
                double nowRow;  //当前点的行
                double nowCol;  //当前点的列
                double nextRow; //下一个点的行
                double nextCol; //下一个点的列
                bool isFirstPoint = false;
                //获取不包含进出刀点的轮廓数据
                List<MachiningPathPosModel> contourData = loucsDatas.Where(x => x.PoseId >= (i + 1) * 1000 && x.PoseId <= (i + 1) * 1000 + 900).ToList();
                for (int j = 0; j < contourData.Count; j++)
                {
                    contourData[j].rowEdge = contourData[j].Row;
                    contourData[j].columnEdge = contourData[j].Column;
                    //起点
                    if (contourData[j].PoseId % 1000 == 0)
                    {
                        previousRow = contourData[j].Row;
                        previousCol = contourData[j].Column;
                        isFirstPoint = true;
                    }
                    else
                    {
                        HTuple angle;
                        nowRow = contourData[j].Row;
                        nowCol = contourData[j].Column;
                        if (j < (contourData.Count - 1))
                        {
                            nextRow = contourData[j + 1].Row;
                            nextCol = contourData[j + 1].Column;
                            HOperatorSet.AngleLx(previousRow, previousCol, nextRow, nextCol, out angle); //与水平X轴夹角 = &列的实际夹角+90度
                        }
                        else
                            HOperatorSet.AngleLx(previousRow, previousCol, nowRow, nowCol, out angle); //与水平X轴夹角 = &列的实际夹角+90度


                        (double StartX, double StartY, double EndX, double EndY) segment;
                        segment = PointCalculator.GenStartAndEndPoints(nowRow, nowCol, (angle.D + 3.14), TmpCtrl_Len1);

                        ////基数跟偶数圈的箭头取向要相反
                        //if (i % 2 == 0) //偶数圈                               
                        //    segment = PointCalculator.GenStartAndEndPoints(nowRow, nowCol, (angle.D + 3.14), TmpCtrl_Len1);
                        //else
                        //    segment = PointCalculator.GenStartAndEndPoints(nowRow, nowCol, angle.D, TmpCtrl_Len1);

                        HTuple phi = PointCalculator.GenAngle(segment.StartX, segment.StartY, segment.EndX, segment.EndY);
                        HOperatorSet.GenMeasureRectangle2(nowRow, nowCol, phi, TmpCtrl_Len1, TmpCtrl_Len2, imgWidth, imgHeight, "nearest_neighbor", out HTuple MsrHandle_Measure);
                        HOperatorSet.MeasurePos(image, MsrHandle_Measure, Smooth, AmplitudeThreshold, "negative", "last", out HTuple rowEdge, out HTuple colEdge
                         , out HTuple Amplitude_Measure, out HTuple Distance_Measure);

                        if (rowEdge.Length > 0)
                        {
                            contourData[j].rowDeviationAbs = Math.Abs(nowRow - rowEdge[0].D);
                            contourData[j].columnDeviationAbs = Math.Abs(nowCol - colEdge[0].D);
                            contourData[j].rowEdge = rowEdge;
                            contourData[j].columnEdge = colEdge;

                            HOperatorSet.AngleLl(previousRow, previousCol, nowRow, nowCol, contourData[j - 1].rowEdge, contourData[j - 1].columnEdge, rowEdge[0].D, colEdge[0].D, out HTuple angle3);

                            if (Math.Abs(angle3.D) > 0.8)
                            {
                                //Console.WriteLine($"{j}点，两线相差：{angle3.D}");
                                contourData[j].IsAccord = false;
                            }

                        }
                        //获取指定高度下的新齐次变换矩阵
                        HTuple pixelToPhysicsHomMat2D = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(contourData[j].Z, homMat2D);
                        HOperatorSet.AffineTransPoint2d(pixelToPhysicsHomMat2D, contourData[j].rowEdge, contourData[j].columnEdge, out HTuple x, out HTuple y);
                        contourData[j].X = Math.Round(x.D, 2);
                        contourData[j].Y = Math.Round(y.D, 2);

                        //第一个也需要添加卡尺，借用第二点的角度
                        if (isFirstPoint)
                        {
                            HOperatorSet.GenMeasureRectangle2(previousRow, previousCol, phi, TmpCtrl_Len1, TmpCtrl_Len2, imgWidth, imgHeight, "nearest_neighbor", out MsrHandle_Measure);
                            HOperatorSet.MeasurePos(image, MsrHandle_Measure, Smooth, AmplitudeThreshold, "negative", "last", out rowEdge, out colEdge
                             , out Amplitude_Measure, out Distance_Measure);

                            if (rowEdge.Length > 0)
                            {
                                contourData[j - 1].rowEdge = rowEdge;
                                contourData[j - 1].columnEdge = colEdge;
                            }

                            //获取指定高度下的新齐次变换矩阵
                            pixelToPhysicsHomMat2D = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(contourData[j].Z, homMat2D);
                            HOperatorSet.AffineTransPoint2d(pixelToPhysicsHomMat2D, contourData[j - 1].rowEdge, contourData[j - 1].columnEdge, out x, out y);
                            contourData[j - 1].X = Math.Round(x.D, 2);
                            contourData[j - 1].Y = Math.Round(y.D, 2);

                        }

                        previousRow = nowRow;
                        previousCol = nowCol;
                        isFirstPoint = false;
                    }
                }
                //ID  PoseID  出刀位置PoseID

                //构建新数据
                //进刀点
                List<MachiningPathPosModel> inDatas = loucsDatas.Where(x => x.PoseId >= (i + 1) * 1000 + 901 && x.PoseId <= (i + 1) * 1000 + 903).ToList();
                for (int j = 0; j < inDatas.Count; j++)
                {
                    id = id + 1;
                    inDatas[j].Id = id;
                    newDatas.Add(inDatas[j]);
                }

                //偏差
                List<MachiningPathPosModel> actualPathPointXs = contourData.OrderBy(x => x.rowDeviationAbs).ToList();
                double medianX = actualPathPointXs[actualPathPointXs.Count / 2].rowDeviationAbs; //中位数X
                List<MachiningPathPosModel> actualPathPointYs = contourData.OrderBy(y => y.columnDeviationAbs).ToList();
                double medianY = actualPathPointYs[actualPathPointYs.Count / 2].columnDeviationAbs; //中位数Y

                for (int j = 0; j < contourData.Count; j++)
                {
                    contourData[j].Row = contourData[j].rowEdge;
                    contourData[j].Column = contourData[j].columnEdge;
                    if (contourData[j].PoseId % 1000 == 0)
                    {
                        continue;
                    }
                    double rowAbs = contourData[j].rowDeviationAbs; //取出绝对偏差Row
                    double colAbs = contourData[j].columnDeviationAbs; //取出绝对偏差Column
                    //判断偏差是否符合范围
                    if (0 < rowAbs && rowAbs < expand * medianX && 0 < colAbs && colAbs < expand * medianY && contourData[j].IsAccord)
                    {

                    }
                    else
                    {
                        contourData[j].IsAccord = false;
                        //contourData[j].Row = contourData[j - 1].Row;
                        //contourData[j].Column = contourData[j - 1].Column;
                        //contourData[j].X = contourData[j - 1].X;
                        //contourData[j].Y = contourData[j - 1].Y;

                    }
                    //newDatas.Add(contourData[j]);
                }

                int posdID = contourData[0].PoseId;

                //构建轮毂点
                for (int j = 0; j < contourData.Count; j++)
                {
                    if (contourData[j].IsAccord)
                    {

                        id = id + 1;
                        contourData[j].PoseId = posdID;
                        contourData[j].Id = id;
                        newDatas.Add(contourData[j]);
                        posdID = posdID + 1;


                        //int.Parse(str[i % spokeType])
                        //if (posdID % 1000 <= int.Parse(outPointPoseIds[WinNum]))
                        //{
                        //    outPointPoseIds[WinNum] = (int.Parse(outPointPoseIds[WinNum]) - 1).ToString();
                        //    string str = string.Join(",", outPointPoseIds);
                        //    CurrentTemplateData.OutPointPoseId = str;
                        //}

                    }
                    else
                    {

                        if (posdID % 1000 <= index)  //删除轮毂前面的点
                        {
                            index = index - 1;
                            newOutPoseIDS[i] = index.ToString();
                            Console.WriteLine(index);
                        }

                    }
                }

                //出刀点
                var outDatas = loucsDatas.Where(x => x.PoseId >= (i + 1) * 1000 + 904 && x.PoseId <= (i + 1) * 1000 + 906).ToList();
                for (int j = 0; j < outDatas.Count; j++)
                {
                    id = id + 1;
                    outDatas[j].Id = id;
                    newDatas.Add(outDatas[j]);
                }
            }

            //foreach (MachiningPathPosModel data in newDatas)
            //{
            //    Console.WriteLine( $"输出结果：ID：{data.Id} - PoseId:{data.PoseId}");
            //}

            return newDatas;
        }

        /// <summary>
        /// 旋转进出刀点
        /// </summary>
        /// <param name="datas">路径点数据</param>
        /// <param name="data">轮型模板数据</param>
        /// <param name="centerRow">旋转中心行坐标</param>
        /// <param name="centerCol">旋转中心列坐标</param>
        /// <param name="radian">旋转弧度</param>
        /// <param name="inOutRows">旋转后的进出刀点行坐标</param>
        /// <param name="inOutCols">旋转后的进出刀点列坐标</param>
        public static void RotateInOutPoints(List<MachiningPathPosModel> datas, TemplateDataModel data, HTuple centerRow, HTuple centerCol, HTuple radian, out List<HTuple> inOutRows, out List<HTuple> inOutCols)
        {
            var xldCount = datas[datas.Count - 1].PoseId / 1000;
            //存储新的进出刀点
            List<HTuple> inOutRs = new List<HTuple>();
            List<HTuple> inOutCs = new List<HTuple>();
            for (int k = 0; k < xldCount; k++)
            {
                //获取进出刀点数据
                var ps = datas.Where(x => x.PoseId > (k + 1) * 1000 + 900 && x.PoseId <= (k + 1) * 1000 + 999).ToList();
                //生成旋转后的新进出刀点数据
                HTuple ioR = null;
                HTuple ioC = null;
                for (int i = 0; i < ps.Count; i++)
                {
                    HTuple r = (ps[i].Row - data.CenterRow) * Math.Cos(radian) - (ps[i].Column - data.CenterColumn) * Math.Sin(radian) + centerRow;
                    HTuple c = (ps[i].Row - data.CenterRow) * Math.Sin(radian) + (ps[i].Column - data.CenterColumn) * Math.Cos(radian) + centerCol;
                    HOperatorSet.TupleConcat(ioR, Math.Round(r.D, 2), out ioR);
                    HOperatorSet.TupleConcat(ioC, Math.Round(c.D, 2), out ioC);
                }
                inOutRs.Add(ioR);
                inOutCs.Add(ioC);
            }
            inOutRows = inOutRs;
            inOutCols = inOutCs;
        }

        /// <summary>
        /// 合并轨迹点
        /// </summary>
        /// <param name="inOutRows">进出刀点行坐标</param>
        /// <param name="inOutCols">进出刀点列坐标</param>
        /// <param name="locusRows">轨迹点行坐标</param>
        /// <param name="locusCols">轨迹点列坐标</param>
        /// <param name="newRows">合并后的轨迹点行坐标</param>
        /// <param name="newCols">合并后的轨迹点列坐标</param>
        public static void MergeLocusPoints(List<HTuple> inOutRows, List<HTuple> inOutCols, List<HTuple> locusRows, List<HTuple> locusCols, out List<HTuple> newRows, out List<HTuple> newCols)
        {
            //存储合并后所有点的行列坐标
            List<HTuple> newRs = new List<HTuple>();
            List<HTuple> newCs = new List<HTuple>();
            for (int i = 0; i < locusRows.Count; i++)
            {
                HTuple unionRow = new HTuple();
                HTuple unionColumn = new HTuple();
                if (inOutRows[i] != null)
                {
                    for (int j = 0; j < inOutRows[i].Length / 2; j++)//入刀点
                    {
                        HOperatorSet.TupleConcat(unionRow, inOutRows[i][j], out unionRow);
                        HOperatorSet.TupleConcat(unionColumn, inOutCols[i][j], out unionColumn);
                    }
                }

                for (int k = 0; k < locusRows[i].Length; k++)//加工轨迹点
                {
                    HOperatorSet.TupleConcat(unionRow, locusRows[i][k], out unionRow);
                    HOperatorSet.TupleConcat(unionColumn, locusCols[i][k], out unionColumn);
                }

                if (inOutRows[i] != null)
                {
                    for (int l = inOutRows[i].Length / 2; l < inOutRows[i].Length; l++)//出刀点
                    {
                        HOperatorSet.TupleConcat(unionRow, inOutRows[i][l], out unionRow);
                        HOperatorSet.TupleConcat(unionColumn, inOutCols[i][l], out unionColumn);
                    }
                }
                newRs.Add(unionRow);
                newCs.Add(unionColumn);
            }
            newRows = newRs;
            newCols = newCs;
        }

        /// <summary>
        /// 生成发送给机器人的数据
        /// </summary>
        /// <param name="datas">原始数据</param>
        /// <param name="newRows"></param>
        /// <param name="newCols"></param>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static List<MachiningPathPosModel> GenToRobotDatas(List<MachiningPathPosModel> datas, List<HTuple> rows, List<HTuple> cols, HTuple radian)
        {
            List<MachiningPathPosModel> toRobotDatas = new List<MachiningPathPosModel>();
            int id = 0;
            //模板匹配出的弧度转角度
            var angle = radian.D * 180.0 / Math.PI;
            for (int i = 0; i < rows.Count; i++)
            {
                // 获取第i个窗口的轨迹数据
                var points = datas.Where(x => x.PoseId >= (i + 1) * 1000 && x.PoseId <= (i + 1) * 1000 + 999).ToList();
                for (int j = 0; j < rows[i].Length; j++)
                {
                    MachiningPathPosModel point = new MachiningPathPosModel();
                    point.Id = id + 1;
                    point.PoseId = points[j].PoseId;
                    point.Row = rows[i][j];
                    point.Column = cols[i][j];
                    //获取指定高度下的新齐次变换矩阵
                    var pixelToPhysicsHomMat2D = ServiceLocator.Current.GetInstance<SettingPageViewModel>().GetPixelToPhysicsHomMat2D(points[j].Z, PixelToPhysicsHomMat2D);
                    HOperatorSet.AffineTransPoint2d(pixelToPhysicsHomMat2D, rows[i][j], cols[i][j], out HTuple x, out HTuple y);
                    point.X = Math.Round(x.D, 2) + ServiceLocator.Current.GetInstance<SettingPageViewModel>().RobotAxisCompensateX;
                    point.Y = Math.Round(y.D, 2) + ServiceLocator.Current.GetInstance<SettingPageViewModel>().RobotAxisCompensateY;
                    point.Z = points[j].Z + ServiceLocator.Current.GetInstance<SettingPageViewModel>().RobotAxisCompensateZ;
                    point.EX = points[j].EX;
                    point.EY = points[j].EY;
                    //旋转模板位姿
                    point.EZ = RobotHelper.AngleCalculationAfterRotation(points[j].EZ, angle);
                    var q = ViewportHelper.EulerToQuaternion(points[j].EX, points[j].EY, point.EZ);
                    point.Q1 = q.W;
                    point.Q2 = q.X;
                    point.Q3 = q.Y;
                    point.Q4 = q.Z;
                    toRobotDatas.Add(point);
                    id++;
                }
            }
            return toRobotDatas;
        }

        /// <summary>  
        /// 计算点P绕新旋转中心Pn逆时针旋转指定角度后的新位置。  
        /// </summary>  
        /// <param name="x">点P的x坐标。</param>  
        /// <param name="y">点P的y坐标。</param>   
        /// <param name="xn">新旋转中心Pn的x坐标。</param>  
        /// <param name="yn">新旋转中心Pn的y坐标。</param>  
        /// <param name="rad">旋转角度，以弧度为单位。</param>  
        /// <returns>返回旋转后的新位置P'。</returns>  
        public static (double x, double y) RotatePointAroundNewCenter(double x, double y, double xn, double yn, double rad)
        {
            // 计算点P相对于新旋转中心Pn的位置  
            double x_rel = x - xn;
            double y_rel = y - yn;

            // 执行逆时针旋转  
            double x_rotated = x_rel * Math.Cos(rad) - y_rel * Math.Sin(rad);
            double y_rotated = x_rel * Math.Sin(rad) + y_rel * Math.Cos(rad);

            // 将旋转后的点转换回原始坐标系  
            double x_prime = x_rotated + xn;
            double y_prime = y_rotated + yn;

            return (x_prime, y_prime);
        }

        /// <summary>
        /// 窗口轮廓的缩放、旋转、平移
        /// </summary>
        /// <param name="oldXld">旧轮廓</param>
        /// <param name="operateType">操作类型：Scale，Rotate </param>
        /// <param name="xldNum">待处理轮廓的排序值，0为所有轮廓</param>
        /// <param name="x">缩放旋转的值或平移时的x坐标</param>
        /// <param name="y">平移时的y坐标</param>
        /// <param name="centerRow">缩放或旋转的中心行</param>
        /// <param name="centerColumn">缩放或旋转的中心列</param>
        /// <returns>新轮廓</returns>
        public static HObject ContourScaleRotateOrTranslate(HObject oldXld, string operateType, int xldNum, double x, double y, HTuple centerRow, HTuple centerColumn)
        {
            HOperatorSet.CountObj(oldXld, out HTuple number);
            if (xldNum > number) return null;
            if (xldNum == 0)
            {
                //生成空的仿射变换矩阵
                HOperatorSet.HomMat2dIdentity(out HTuple hM2D);
                HTuple homMat2D;
                if (operateType == "Scale")
                {
                    HOperatorSet.HomMat2dScale(hM2D, 1 + x / 100, 1 + x / 100, centerColumn, centerRow, out homMat2D);
                }
                else if (operateType == "Rotate")
                {
                    //将输入的角度转换成弧度
                    HOperatorSet.TupleRad(x, out HTuple rad);
                    //添加旋转
                    HOperatorSet.HomMat2dRotate(hM2D, rad, centerColumn, centerRow, out homMat2D);
                }
                else if (operateType == "Translate")
                {
                    HOperatorSet.HomMat2dTranslate(hM2D, x, y, out homMat2D);
                }
                else return null;
                //仿射变换
                HOperatorSet.AffineTransContourXld(oldXld, out HObject contoursAffineTrans, homMat2D);
                return contoursAffineTrans;
            }
            else
            {
                HOperatorSet.GenEmptyObj(out HObject contours);
                for (int i = 0; i < number; i++)
                {
                    HOperatorSet.SelectObj(oldXld, out HObject contourSelected, i + 1);
                    if (i + 1 == xldNum)
                    {
                        //生成空的仿射变换矩阵
                        HOperatorSet.HomMat2dIdentity(out HTuple hM2D);
                        HTuple homMat2D;
                        if (operateType == "Scale")
                        {
                            HOperatorSet.HomMat2dScale(hM2D, 1 + x / 100, 1 + x / 100, centerColumn, centerRow, out homMat2D);
                        }
                        else if (operateType == "Rotate")
                        {
                            //将输入的角度转换成弧度
                            HOperatorSet.TupleRad(x, out HTuple rad);
                            //添加旋转
                            HOperatorSet.HomMat2dRotate(hM2D, rad, centerColumn, centerRow, out homMat2D);
                        }
                        else if (operateType == "Translate")
                        {
                            HOperatorSet.HomMat2dTranslate(hM2D, x, y, out homMat2D);
                        }
                        else return null;
                        //仿射变换
                        HOperatorSet.AffineTransContourXld(contourSelected, out HObject contoursAffineTrans, homMat2D);
                        HOperatorSet.ConcatObj(contours, contoursAffineTrans, out contours);
                    }
                    else HOperatorSet.ConcatObj(contours, contourSelected, out contours);
                }
                return contours;
            }
        }

        /// <summary>
        /// 模板窗口显示
        /// </summary>
        /// <param name="hWindow"></param>
        /// <param name="image"></param>
        /// <param name="windowXld">原始轮廓</param>
        /// <param name="innerCircleXld">内圆轮廓</param>
        /// <param name="actualXld">实际轮廓</param>
        /// <param name="newXld">新轮廓</param>
        /// <param name="oldCenterCross">旧中心十字</param>
        /// <param name="newCenterCross">新中心十字</param>
        /// <param name="rowList"></param>
        /// <param name="colList"></param>
        /// <param name="isImageEnhancement">是否增强显示的图像</param>
        public static void TemplateWindowDisplay(HWindow hWindow, HObject image, HObject windowXld, HObject innerCircleXld, HObject actualXld, HObject newXld, HObject oldCenterCross, HObject newCenterCross, bool isImageEnhancement)
        {
            hWindow.ClearWindow();
            if (isImageEnhancement)
            {
                HOperatorSet.ScaleImage(image, out HObject imageScaled, 2, 1);
                hWindow.DispObj(imageScaled);
            }
            else hWindow.DispObj(image);

            hWindow.SetColor("orange red");
            if (windowXld != null) hWindow.DispObj(windowXld);
            if (innerCircleXld != null) hWindow.DispObj(innerCircleXld);
            if (oldCenterCross != null) hWindow.DispObj(oldCenterCross);
            hWindow.SetColor("magenta");
            if (actualXld != null)
                hWindow.DispObj(actualXld);
            hWindow.SetColor("green");
            if (newXld != null)
                hWindow.DispObj(newXld);
            if (newCenterCross != null) hWindow.DispObj(newCenterCross);
            hWindow.SetColor("yellow");
            if (newXld != null)
            {
                HOperatorSet.CountObj(newXld, out HTuple number);
                for (int i = 0; i < number; i++)
                {
                    HOperatorSet.SelectObj(newXld, out HObject objectSelected, i + 1);
                    HOperatorSet.AreaCenterXld(objectSelected, out HTuple area, out HTuple row, out HTuple column, out HTuple pointOrder);
                    HOperatorSet.GetContourXld(objectSelected, out HTuple row1, out HTuple column1);
                    HOperatorSet.GenCrossContourXld(out HObject cross, row1, column1, 8, 0);
                    hWindow.DispObj(cross);
                    HOperatorSet.SetTposition(hWindow, row, column);
                    HOperatorSet.WriteString(hWindow, (i + 1).ToString() + " := " + row1.Length.ToString());
                }
            }
        }
    }
}
