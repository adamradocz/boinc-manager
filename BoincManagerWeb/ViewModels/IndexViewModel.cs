using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoincManagerWeb.ViewModels
{
    public class IndexViewModel
    {
        public string MachineName => Environment.MachineName;
        public string WorkingSet => BoincManager.Utils.ConvertBytesToFileSize(Environment.WorkingSet);
        public string OSDescription => System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        public string OSArchitecture => System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString();
        public string FrameworkDescription => System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
    }
}
