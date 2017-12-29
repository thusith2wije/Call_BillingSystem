using System;
using System.Collections.Generic;
using System.Text;

namespace Billing_System
{
    public class CallOfCustomer
    {
        public string C_CallPNumber { get; set; }
        public string C_EndPNumber { get; set; }
        public DateTime C_CallStartTime { get; set; }
        public int C_CallDuration { get; set; }
        public string C_CallAddress { get; set; }
        public string C_FullName { get; set; }
        public string C_PackageCode { get; set; }
        
    }
}
