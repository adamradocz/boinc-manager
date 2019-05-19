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
    }
}
