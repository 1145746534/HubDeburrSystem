using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    public class WheelTypeChooseModel:ViewModelBase
    {
        private string _key;

        public string Key
        {
            get { return _key; }
            set {Set(ref _key, value); }
        }

        private string _wheelType;

        public string WheelType
        {
            get { return _wheelType; }
            set {Set(ref _wheelType, value); }
        }
    }
}
