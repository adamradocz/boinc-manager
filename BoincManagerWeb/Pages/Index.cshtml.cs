using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace BoincManagerWeb.Pages
{
    public class IndexModel : PageModel
    {
        public string MachineName => Environment.MachineName;
        public string WorkingSet => BoincManager.Utils.ConvertBytesToFileSize(Environment.WorkingSet);
        public string OSDescription => System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        public string OSArchitecture => System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString();
        public string FrameworkDescription => System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;

        public void OnGet()
        {
            
        }
    }
}
