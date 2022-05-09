using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudFlare.BulkDelete
{
    public class AppConfig
    {
        public string AuthEmail { get; set; } = null!;
        public string AuthKey { get; set; } = null!;
        public string[] DomainNames { get; set; } = null!;
        public string IpToDelete { get; set; } = null!;
    }
}
