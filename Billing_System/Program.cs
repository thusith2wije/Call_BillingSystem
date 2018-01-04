using System;

namespace Billing_System
{
    public class Program
    {
        static void Main(string[] args)
        {
            for(int i = 0;i<999;i++)
            {

                Console.WriteLine("Enter Customer Number: XXX-XXXXXXX"); // Prompt
                string input_val = Console.ReadLine(); // Get string from user
                Console.WriteLine("-----------------------");
                BillingEngine bil = new BillingEngine();
                bil.Genarate(input_val);
            }
            


            Console.WriteLine();

        }
    }
}
