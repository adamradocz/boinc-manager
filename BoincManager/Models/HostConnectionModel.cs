using System.ComponentModel;

namespace BoincManager.Models
{
    public class HostConnectionModel
    {
        [Category("Data")]
        public string Name { get; set; }

        [Category("Data")]
        public string IpAddress { get; set; }

        [Category("Data")]
        public int Port { get; set; }

        [Category("Security")]
        public string Password { get; set; }
    }
}
