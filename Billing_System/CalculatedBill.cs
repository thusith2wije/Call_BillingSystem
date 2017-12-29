using System;
using System.Collections.Generic;
using System.Text;

namespace Billing_System
{
    public class CalculatedBill
    {
        public string PNumber { get; set; }
        public string BillingAdress { get; set; }
        public int TotalCallCharge { get; set; }
        public int Tax { get; set; }
        public int Rental { get; set; }
        public double Each_CallCarge { get; set; }
        public double BillAmount { get; set; }
        
    }
}
