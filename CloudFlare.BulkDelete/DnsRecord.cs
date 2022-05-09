using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudFlare.BulkDelete
{
    public class DnsRecord
    {
        public string Id { get; set; } = null!;
        public string Zone_Id { get; set; } = null!;
        public string Zone_Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
