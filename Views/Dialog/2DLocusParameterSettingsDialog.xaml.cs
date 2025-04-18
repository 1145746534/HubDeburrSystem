using HubDeburrSystem.Models;
using HubDeburrSystem.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static HubDeburrSystem.Models.LocusParameterSettingModel;

namespace HubDeburrSystem.Views.Dialog
{
    /// <summary>
    /// LocusParameterSettingsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class LocusParameterSettingsDialog : Window
    {
        public LocusParameterSettingsDialog()
        {
            InitializeComponent();
            this.Loaded += LocusParameterSettingsDialog_Loaded;
        }

        private void LocusParameterSettingsDialog_Loaded(object sender, RoutedEventArgs e)
        {
            InnerCaliperLength_tbx.Text = InnerCaliperLength.ToString();
            InnerRadius_tbx.Text = InnerRadius.ToString();
            CalipersDevExpand_tbx.Text = CalipersDevExpand.ToString();
            CalipersMeaLength_tbx.Text = CalipersMeaLength.ToString();
            CalipersMeaWidth_tbx.Text = CalipersMeaWidth.ToString();
            CalipersAmpThreshold_tbx.Text = CalipersAmpThreshold.ToString();
            CalipersSmooth_tbx.Text = CalipersSmooth.ToString();
            OuterMinThreshold_tbx.Text = OuterMinThreshold.ToString();
            MinSimilarity_tbx.Text = MinSimilarity.ToString();

            DarkMinArea_tbx.Text = DarkMinArea.ToString();
            BrightMinArea_tbx.Text = BrightMinArea.ToString();
            SingleXldDilation_tbx.Text = SingleXldDilation.ToString();
            DarkMaxThreshold_tbx.Text = DarkMaxThreshold.ToString();
            BrightMinThreshold_tbx.Text = BrightMinThreshold.ToString();
            UnionDilationErosion_tbx.Text = UnionDilationErosion.ToString();
            MachiningLocusOffset_tbx.Text = MachiningLocusOffset.ToString();
            MaxDistance_tbx.Text = MaxDistance.ToString();

            CannyAlpha_tbx.Text = CannyAlpha.ToString();
            CannyLowThresold_tbx.Text = CannyLowThresold.ToString();
            CannyHighThresold_tbx.Text = CannyHighThresold.ToString();
            XldMinLength_tbx.Text = XldMinLength.ToString();
            MaskWidthHeight_tbx.Text = MaskWidthHeight.ToString();
        }

        private void Confirm_btn_Click(object sender, RoutedEventArgs e)
        {
            if (InnerCaliperLength_tbx.Text != InnerCaliperLength.ToString())
            {
                try
                {
                    int value = int.Parse(InnerCaliperLength_tbx.Text);
                    if (value >= 1 && value <= 255)
                    {
                        InnerCaliperLength = value;
                        ConfigEdit.SetAppSettings("InnerCaliperLength", InnerCaliperLength_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("内圆卡尺长度输入错误，必须大于0且小于等于255，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("内圆卡尺长度输入错误，请重新输入", MessageType.Error);
                    return;
                }

            }
            if (InnerRadius_tbx.Text != InnerRadius.ToString())
            {
                try
                {
                    var value = int.Parse(InnerRadius_tbx.Text);
                    if (value >= 1 && value < CalipersDevExpand)
                    {
                        InnerRadius = value;
                        ConfigEdit.SetAppSettings("InnerRadius", InnerRadius_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("内圆半径输入错误，必须大于等于1，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("内圆最小面积输入错误，请重新输入", MessageType.Error);
                    return;
                }

            }
            if (CalipersDevExpand_tbx.Text != CalipersDevExpand.ToString())
            {
                try
                {
                    int value = int.Parse(CalipersDevExpand_tbx.Text);
                    if (value >= 1)
                    {
                        CalipersDevExpand = value;
                        ConfigEdit.SetAppSettings("CalipersDevExpand", CalipersDevExpand_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("偏差倍数输入错误，必须大于等于1，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("偏差倍数输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (CalipersMeaLength_tbx.Text != CalipersMeaLength.ToString())
            {
                try
                {
                    int value = int.Parse(CalipersMeaLength_tbx.Text);
                    if (value >= 1)
                    {
                        CalipersMeaLength = value;
                        ConfigEdit.SetAppSettings("CalipersMeaLength", CalipersMeaLength_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("测量卡尺长度输入错误，必须大于等于1，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("测量卡尺长度输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (CalipersMeaWidth_tbx.Text != CalipersMeaWidth.ToString())
            {
                try
                {
                    int value = int.Parse(CalipersMeaWidth_tbx.Text);
                    if (value >= 1)
                    {
                        CalipersMeaWidth = value;
                        ConfigEdit.SetAppSettings("CalipersMeaWidth", CalipersMeaWidth_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("测量卡尺宽度输入错误，必须大于等于1，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("测量卡尺宽度输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (CalipersAmpThreshold_tbx.Text != CalipersAmpThreshold.ToString())
            {
                try
                {
                    int value = int.Parse(CalipersAmpThreshold_tbx.Text);
                    if (value >= 1)
                    {
                        CalipersAmpThreshold = value;
                        ConfigEdit.SetAppSettings("CalipersAmpThreshold", CalipersAmpThreshold_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("分割阈值输入错误，必须大于等于1，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("分割阈值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (CalipersSmooth_tbx.Text != CalipersSmooth.ToString())
            {
                try
                {
                    int value = int.Parse(CalipersSmooth_tbx.Text);
                    if (value >= 1)
                    {
                        CalipersSmooth = value;
                        ConfigEdit.SetAppSettings("CalipersSmooth", CalipersSmooth_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("平滑参数输入错误，必须大于等于1，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("平滑参数输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (OuterMinThreshold_tbx.Text != OuterMinThreshold.ToString())
            {
                try
                {
                    var value = int.Parse(OuterMinThreshold_tbx.Text);
                    if (value > 0 && value < 250)
                    {
                        OuterMinThreshold = value;
                        ConfigEdit.SetAppSettings("OuterMinThreshold", OuterMinThreshold_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("外圆最小阈值必须大于0或小于250，请重新输入", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("外圆最小阈值输入错误，请重新输入", MessageType.Error);
                    return;
                }

            }
            if (MinSimilarity_tbx.Text != MinSimilarity.ToString())
            {
                try
                {
                    var value = double.Parse(MinSimilarity_tbx.Text);
                    if (value >= 0.5 && value < 1)
                    {
                        MinSimilarity = value;
                        ConfigEdit.SetAppSettings("MinSimilarity", MinSimilarity_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("最小相似度必须大于0.5，且小于1，请重新输入", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("最小相似度输入错误，请重新输入", MessageType.Error);
                    return;
                }

            }

            if (DarkMinArea_tbx.Text != DarkMinArea.ToString())
            {
                try
                {
                    var value = double.Parse(DarkMinArea_tbx.Text);
                    if (value >= 100)
                    {
                        LocusParameterSettingModel.DarkMinArea = value;
                        ConfigEdit.SetAppSettings("DarkMinArea", DarkMinArea_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("窗口暗部最小面积值输入错误，必须大于等于100，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("窗口暗部最小面积值输入错误，请重新输入", MessageType.Error);
                    return;
                }

            }
            if (BrightMinArea_tbx.Text != BrightMinArea.ToString())
            {
                try
                {
                    var value = double.Parse(BrightMinArea_tbx.Text);
                    if (value >= 100)
                    {
                        LocusParameterSettingModel.BrightMinArea = value;
                        ConfigEdit.SetAppSettings("BrightMinArea", BrightMinArea_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("窗口亮部最小面积值输入错误，必须大于等于100，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("窗口亮部最小面积值输入错误，请重新输入", MessageType.Error);
                    return;
                }

            }
            if (SingleXldDilation_tbx.Text != SingleXldDilation.ToString())
            {
                try
                {
                    var value = double.Parse(SingleXldDilation_tbx.Text);
                    if (value > 0)
                    {
                        SingleXldDilation = value;
                        ConfigEdit.SetAppSettings("SingleXldDilation", SingleXldDilation_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("单轮廓膨胀值必须大于0，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("单轮廓膨胀值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (DarkMaxThreshold_tbx.Text != DarkMaxThreshold.ToString())
            {
                try
                {
                    var value = int.Parse(DarkMaxThreshold_tbx.Text);
                    if (value >= 1 && value <= 255)
                    {
                        LocusParameterSettingModel.DarkMaxThreshold = value;
                        ConfigEdit.SetAppSettings("DarkMaxThreshold", DarkMaxThreshold_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("暗部最大阈值必须大于等于1小于等于254，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("暗部最大阈值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (BrightMinThreshold_tbx.Text != BrightMinThreshold.ToString())
            {
                try
                {
                    var value = int.Parse(BrightMinThreshold_tbx.Text);
                    if (value >= 1 && value < 255)
                    {
                        LocusParameterSettingModel.BrightMinThreshold = value;
                        ConfigEdit.SetAppSettings("BrightMinThreshold", BrightMinThreshold_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("亮部最小阈值必须大于等于1，并且小于255，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("亮部最小阈值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (UnionDilationErosion_tbx.Text != UnionDilationErosion.ToString())
            {
                try
                {
                    var value = double.Parse(UnionDilationErosion_tbx.Text);
                    if (value >= 1 )
                    {
                        UnionDilationErosion = value;
                        ConfigEdit.SetAppSettings("UnionDilationErosion", UnionDilationErosion_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("合并膨胀值必须大于等于1，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("合并膨胀值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (MachiningLocusOffset_tbx.Text != MachiningLocusOffset.ToString())
            {
                try
                {
                    var value = int.Parse(MachiningLocusOffset_tbx.Text);
                    if (value >= -20 && value <= 20)
                    {
                        MachiningLocusOffset = value;
                        ConfigEdit.SetAppSettings("MachiningLocusOffset", MachiningLocusOffset_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("加工路径偏移值必须大于等于-20，小于等于20，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("加工路径偏移输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (MaxDistance_tbx.Text != MaxDistance.ToString())
            {
                try
                {
                    var value = double.Parse(MaxDistance_tbx.Text);
                    if(value > 0)
                    {
                        LocusParameterSettingModel.MaxDistance = value;
                        ConfigEdit.SetAppSettings("MaxDistance", MaxDistance_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("最大距离必须大于0，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("最大距离值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }

            if (CannyAlpha_tbx.Text != CannyAlpha.ToString())
            {
                try
                {
                    var value = double.Parse(CannyAlpha_tbx.Text);
                    if (value > 0)
                    {
                        CannyAlpha = value;
                        ConfigEdit.SetAppSettings("CannyAlpha", CannyAlpha_tbx.Text);
                    }
                    else
                    {
                        //#FFED6656
                        UMessageBox.Show("边缘算子平滑值必须大于0，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("边缘算子平滑值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (CannyLowThresold_tbx.Text != CannyLowThresold.ToString())
            {
                try
                {
                    int value = int.Parse(CannyLowThresold_tbx.Text);
                    if (value >= 1 && value < CannyHighThresold)
                    {
                        CannyLowThresold = value;
                        ConfigEdit.SetAppSettings("CannyLowThresold", CannyLowThresold_tbx.Text);
                    }
                    else
                    {
                        //#FFED6656
                        UMessageBox.Show("边缘算法低阈值必须大于1，且小于边缘算法高阈值，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("边缘算法低阈值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (CannyHighThresold_tbx.Text != CannyHighThresold.ToString())
            {
                try
                {
                    int value = int.Parse(CannyHighThresold_tbx.Text);
                    if (value > 0 && value > LocusParameterSettingModel.CannyLowThresold)
                    {
                        LocusParameterSettingModel.CannyHighThresold = value;
                        ConfigEdit.SetAppSettings("CannyHighThresold", CannyHighThresold_tbx.Text);
                    }
                    else
                    {
                        //#FFED6656
                        UMessageBox.Show("边缘算法高阈值必须大于0，且大于边缘算法低阈值，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("边缘算法高阈值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (XldMinLength_tbx.Text != XldMinLength.ToString())
            {
                try
                {
                    int value = int.Parse(XldMinLength_tbx.Text);
                    if (value > 0)
                    {
                        LocusParameterSettingModel.XldMinLength = value;
                        ConfigEdit.SetAppSettings("XldMinLength", XldMinLength_tbx.Text);
                    }
                    else
                    {
                        //#FFED6656
                        UMessageBox.Show("边缘算法轮廓最小长度必须大于0，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("边缘算法轮廓最小长度输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (MaskWidthHeight_tbx.Text != MaskWidthHeight.ToString())
            {
                try
                {
                    int value = int.Parse(MaskWidthHeight_tbx.Text);
                    if (value > 0)
                    {
                        MaskWidthHeight = value;
                        ConfigEdit.SetAppSettings("MaskWidthHeight", MaskWidthHeight_tbx.Text);
                    }
                    else
                    {
                        //#FFED6656
                        UMessageBox.Show("比度增强掩模的宽高必须大于等于3且小于等于201，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("比度增强掩模的宽高输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }

            this.Close();
        }


        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region==================TextBox输入限制====================
        private void InnerMaxThreshold_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void InnerMinArea_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void Tbx_PreviewTextInput_Int(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void OuterMinThreshold_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void MinSimilarity_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void DarkMinArea_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void BrightMinArea_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void SingleXldDilation_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void DarkMinThreshold_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void DarkMaxThreshold_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void BrightMinThreshold_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void BrightMaxThreshold_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void UnionDilationErosion_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void MaxDistance_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void MachiningLocusOffset_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9-]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void CannyAlpha_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void CannyLowThresold_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void CannyHighThresold_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void XldMinLength_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void MaskWidthHeight_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }
        #endregion


    }
}
