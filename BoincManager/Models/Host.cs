using System.ComponentModel.DataAnnotations;

namespace BoincManager.Models
{
    public class Host
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string IpAddress { get; set; }

        [Required]
        [Range(0, 65535)]
        public int Port { get; set; } = Constants.BoincDefaultPort;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public bool AutoConnect { get; set; } = true;

        // To make model binding possible in ASP.NET, the class must have a public default constructor.
        public Host()
        {

        }

        public Host(string name, string ipAddress, string password, bool autoConnect = true, int port = Constants.BoincDefaultPort)
        {
            Name = name;
            IpAddress = ipAddress;
            Password = password;
            AutoConnect = autoConnect;
            Port = port;
        }

    }
}
