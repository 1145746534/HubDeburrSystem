using Adapters;
using CommonServiceLocator;
using HubDeburrSystem.DataAccess;
using HubDeburrSystem.Models;
using HubDeburrSystem.ViewModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HubDeburrSystem.Views.Dialog
{
    /// <summary>
    /// TemplateDataEditDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateDataEditDialog : Window
    {
        private List<TemplateDataModel> datas;
        public TemplateDataEditDialog()
        {
            InitializeComponent();
            datas = new List<TemplateDataModel>();
            this.Loaded += AddWheelTypeDialog_Loaded;
        }

        private void AddWheelTypeDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var sDB = new SqlAccess().SystemDataAccess;
            datas = ServiceLocator.Current.GetInstance<TemplatePageViewModel>().TemplateDatas.Where(x => x.WheelType == LocusParameterSettingModel.SelectWheelModel).ToList();
            Index_tbx.Text = datas[0].Index.ToString();
            WheelType_tbx.Text = datas[0].WheelType;
            SpokeQuantity_tbx.Text = datas[0].SpokeQuantity.ToString();
            LocusScale_tbx.Text = datas[0].LocusScale.ToString();
            DarkMaxThreshold_tbx.Text = datas[0].DarkMaxThreshold.ToString();
            LightMinThreshold_tbx.Text = datas[0].LightMinThreshold.ToString();
            InnerCircleCaliperLength_tbx.Text = datas[0].InnerCircleCaliperLength.ToString();
            InnerCircleRadius_tbx.Text = datas[0].InnerCircleRadius.ToString();
            AngularCompensation_tbx.Text = datas[0].AngularCompensation.ToString();
            ProcessingPressure_tbx.Text = datas[0].ProcessingPressure.ToString();
        }

        private void Confirm_btn_Click(object sender, RoutedEventArgs e)
        {
            bool isChanged = false;
            if (WheelType_tbx.Text != datas[0].WheelType)
            {
                datas[0].WheelType = WheelType_tbx.Text;
                isChanged = true;
            }
            if (SpokeQuantity_tbx.Text != datas[0].SpokeQuantity.ToString())
            {
                try
                {
                    int value = int.Parse(SpokeQuantity_tbx.Text);
                    if (value > 0)
                    {
                        datas[0].SpokeQuantity = value;
                        isChanged = true;
                    }
                    else
                    {
                        UMessageBox.Show("轮辐数量必须大于0，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("轮辐数量输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (LocusScale_tbx.Text != datas[0].LocusScale.ToString())
            {
                try
                {
                    int value = int.Parse(LocusScale_tbx.Text);
                    if (value > -20 && value < 30)
                    {
                        datas[0].LocusScale = value;
                        isChanged = true;
                    }
                    else
                    {
                        UMessageBox.Show("轨迹缩放值必须大于-20或小于30，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("轨迹缩放值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (DarkMaxThreshold_tbx.Text != datas[0].DarkMaxThreshold.ToString())
            {
                try
                {
                    int value = int.Parse(DarkMaxThreshold_tbx.Text);
                    if (value > 1 && value < 200)
                    {
                        datas[0].DarkMaxThreshold = value;
                        isChanged = true;
                    }
                    else
                    {
                        UMessageBox.Show("窗口暗部最大阈值必须大于1或小于200，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("窗口暗部最大阈值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (LightMinThreshold_tbx.Text != datas[0].LightMinThreshold.ToString())
            {
                try
                {
                    int value = int.Parse(LightMinThreshold_tbx.Text);
                    if (value > 1 && value < 254)
                    {
                        datas[0].LightMinThreshold = value;
                        isChanged = true;
                    }
                    else
                    {
                        UMessageBox.Show("窗口亮部最大阈值必须大于1或小于254，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("窗口亮部最大阈值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (InnerCircleCaliperLength_tbx.Text != datas[0].InnerCircleCaliperLength.ToString())
            {
                try
                {
                    int value = int.Parse(InnerCircleCaliperLength_tbx.Text);
                    if (value > 1 && value < 254)
                    {
                        datas[0].InnerCircleCaliperLength = value;
                        isChanged = true;
                    }
                    else
                    {
                        UMessageBox.Show("内圆卡尺长度必须大于1或小于254，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("内圆最大阈值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }

            if (InnerCircleRadius_tbx.Text != datas[0].InnerCircleRadius.ToString())
            {
                try
                {
                    int value = int.Parse(InnerCircleRadius_tbx.Text);
                    if (value > 0)
                    {
                        datas[0].InnerCircleRadius = value;
                        isChanged = true;
                    }
                    else
                    {
                        UMessageBox.Show("内圆半径必须大于0，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("内圆半径输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (AngularCompensation_tbx.Text != datas[0].AngularCompensation.ToString())
            {
                try
                {
                    double value = double.Parse(AngularCompensation_tbx.Text);
                    if (value > -10 && value < 10)
                    {
                        datas[0].AngularCompensation = value;
                        isChanged = true;
                    }
                    else
                    {
                        UMessageBox.Show("角度补偿值必须大于-10或小于10，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("角度补偿值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (ProcessingPressure_tbx.Text != datas[0].ProcessingPressure.ToString())
            {
                try
                {
                    float value = float.Parse(ProcessingPressure_tbx.Text);
                    if (value > 0 && value < 0.3)
                    {
                        datas[0].ProcessingPressure = value;
                        isChanged = true;
                    }
                    else
                    {
                        UMessageBox.Show("加工压力值必须大于0或小于0.3，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("加工压力值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            if (isChanged)
            {
                var sDB = new SqlAccess().SystemDataAccess;
                sDB.Updateable(datas[0]).ExecuteCommand();
                ServiceLocator.Current.GetInstance<TemplatePageViewModel>().UpdateWheelDatas(WheelType_tbx.Text);
                ServiceLocator.Current.GetInstance<TemplatePageViewModel>().UpdateProcessingWheelDatas();
            }
            this.Close();
        }

        #region=======输入限制=======
        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SpokeQuantity_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void LocusScale_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private void LightMinThreshold_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void InnerCircleMaxThreshold_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void AngularCompensation_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]");
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void WheelType_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9a-z_]");//只允许数字、小写字母和下划线
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }
        #endregion
    }
}
