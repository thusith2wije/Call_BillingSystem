using System;
using System.Collections.Generic;
using System.Text;

namespace Billing_System
{
    public class CalculatedBill
    {
        public string PNumber { get; set; }
        public string BillingAdress { get; set; }
        //public int TotalCallCharge { get; set; }
        public double Tax { get; set; }
        public int Rental { get; set; }
        public double Each_CallCarge { get; set; }
        public double BillAmount { get; set; }
        public string Package_Code { get; set; }
        public int CallNo { get; set; }
        public DateTime CallStartTime { get; set; }
        public int CallDuration { get; set; }
        public string Destination { get; set; }

    }
}
