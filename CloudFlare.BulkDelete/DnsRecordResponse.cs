using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudFlare.BulkDelete
{
    public class DnsRecordResponse
    {
        public DnsRecord[] Result { get; set; } = null!;
    }
}
