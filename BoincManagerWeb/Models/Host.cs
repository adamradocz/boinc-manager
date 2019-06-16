using BoincManager.Interfaces;
using BoincManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BoincManagerWeb.Models
{
    public class Host : IHost, IFilterable
    {
        public int Id { get; }
        public string Name { get; set; }
        public string IpAddress { get; set; }

        [Range(0, 65535, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Port { get; set; }
        public string Password { get; set; }
        public bool AutoConnect { get; set; }
        public string BoincVersion { get; set; }
        public string OperatingSystem { get; set; }
        public bool Connected { get; set; }
        public string Status { get; set; }

        public Host(HostState hostState)
        {
            Id = hostState.Id;

            Name = hostState.Name;
            IpAddress = hostState.IpAddress;
            Port = hostState.Port;
            Password = hostState.Password;
            AutoConnect = hostState.AutoConnect;
            BoincVersion = hostState.BoincVersion;
            OperatingSystem = hostState.OperatingSystem;
            Connected = hostState.Connected;
            Status = hostState.Status;
        }

        public IEnumerable<string> GetContentsForFiltering()
        {
            yield return Name;
            yield return IpAddress;
            yield return BoincVersion;
            yield return OperatingSystem;
            yield return Status;
        }
    }
}
