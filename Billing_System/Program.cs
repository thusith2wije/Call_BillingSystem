using System;

namespace Billing_System
{
    public class Program
    {
        static void Main(string[] args)
        {
            
            BillingEngine bil = new BillingEngine();            
            bil.Genarate("011-7489261");


            Console.WriteLine();

        }
    }
}
