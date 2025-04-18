using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using HubDeburrSystem.DataAccess;
using HubDeburrSystem.Views.Pages;

namespace HubDeburrSystem.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<ILocalDataAccess, LocalDataAccess>();
            SimpleIoc.Default.Register<MonitorPageViewModel>();
            SimpleIoc.Default.Register<LocusPageViewModel>();
            SimpleIoc.Default.Register<TemplatePageViewModel>();
            SimpleIoc.Default.Register<SettingPageViewModel>();
            SimpleIoc.Default.Register<ReportPageViewModel>();
            SimpleIoc.Default.Register<EquipmentPageViewModel>();
        }

        public MainViewModel MainViewModelLocator => ServiceLocator.Current.GetInstance<MainViewModel>();

        public LoginViewModel LoginViewModelLocator => ServiceLocator.Current.GetInstance<LoginViewModel>();

        public MonitorPageViewModel MonitorPageViewModelLocator => ServiceLocator.Current.GetInstance<MonitorPageViewModel>();

        public LocusPageViewModel LocusPageViewModelLocator => ServiceLocator.Current.GetInstance<LocusPageViewModel>();

        public TemplatePageViewModel TemplatePageViewModelLocator => ServiceLocator.Current.GetInstance<TemplatePageViewModel>();

        public SettingPageViewModel SettingPageViewModelLocator => ServiceLocator.Current.GetInstance<SettingPageViewModel>();

        public ReportPageViewModel ReportPageViewModelLocator => ServiceLocator.Current.GetInstance<ReportPageViewModel>();

        public EquipmentPageViewModel EquipmentPageViewModelLocator => ServiceLocator.Current.GetInstance<EquipmentPageViewModel>();

        public static void Cleanup<T>() where T : ViewModelBase
        {
            // 统一的对象的释放
            ServiceLocator.Current.GetInstance<T>().Cleanup();
            SimpleIoc.Default.Unregister<T>();
            SimpleIoc.Default.Register<T>();
        }
    }
}