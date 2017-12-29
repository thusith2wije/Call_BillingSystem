using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Billing_System
{
    public class BillingEngine
    {
        bool val;           
        List<CustomerDetails> cutomer_Lines = new List<CustomerDetails>();
        List<CallDetails> call_Lines = new List<CallDetails>();
        List<CallOfCustomer>customer_callLines = new List<CallOfCustomer>();
        string path_customer = @"C:\Users\tij\source\repos\CustomerList.txt";
        string path_call = @"C:\Users\tij\source\repos\CallList.txt";
        

        /*Scan the CSV file of customer details*/
        public bool ScanCustomerCSV(string path_customer)
        {
            string customerLine;
            
            using (StreamReader sr = new StreamReader(path_customer))
            {
                while ((customerLine = sr.ReadLine()) != null)
                {                    
                    SplitCustomerDetails(customerLine);                    
                }
                //Console.WriteLine(i);
            }
            if (cutomer_Lines != null)
            {
               val = true;
            }
            else
            {
                val = false;
            }
            return val;
        }

        /*+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++*/
        /*Split scanned customer details stream lines and store in a list as objects*/
        public CustomerDetails SplitCustomerDetails(string line)
        {
            
            string[] col = line.Split(',').Select(sValue => sValue.Trim()).ToArray();
            CustomerDetails a = new CustomerDetails();
            a.PNumber = col[0];
            a.BillingAddress = col[1];
            a.FullName = col[2];
            a.PackageCode = col[3];
            a.RegisteredDate = col[4];

            cutomer_Lines.Add(a);
            //Console.WriteLine(a.FullName);
            return a;   
        }

        /*+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++*/
        /*Scan the CSV file of call details*/
        public bool ScanCallCSV(string path_call)
        {
            string callLine;
            call_Lines.Clear();

            using (StreamReader sr = new StreamReader(path_call))
            {
                while ((callLine = sr.ReadLine()) != null)
                {                    
                    SplitCallDetails(callLine);                    
                }
            }
            if (call_Lines != null)
            {
                val = true;
            }
            else
            {
                val = false;
            }
            return val;
        }

        /*+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++*/
        /*Split scanned call details stream lines and store in a list as objects*/
        public CallDetails SplitCallDetails(string line)
        {
            string[] col = line.Split(',').Select(sValue => sValue.Trim()).ToArray();
            CallDetails b = new CallDetails();
            b.CallPNumber = col[0];
            b.EndPNumber = col[1];            
            string iString = col[2];
            DateTime ret_Time = DateTime.Parse(iString);
            
            b.CallStartTime = ret_Time;
            b.CallDuration = col[3];

            call_Lines.Add(b);
            
            return b;
        }

        /*+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++*/
        /*Store each call details of considered customer in a list*/
        public CallDetails CallDetailsOf_EachCustomer(string input_val)
        {
            CallDetails obj = new CallDetails();

            foreach (CallDetails s in call_Lines)
            {                
                if(s.CallPNumber == input_val)
                {
                    obj = s;
                    CallOfCustomer c = new CallOfCustomer();                    

                    c.C_CallPNumber = s.CallPNumber;
                    c.C_EndPNumber = s.EndPNumber;
                    c.C_CallStartTime = s.CallStartTime;
                    
                    int ret_CallDuration = Int32.Parse(s.CallDuration);
                    c.C_CallDuration = ret_CallDuration;

                    /* Add some customer details into CallOfCustomer object*/
                    foreach (CustomerDetails p in cutomer_Lines)
                    {
                        if (p.PNumber == input_val)
                        {
                            c.C_CallAddress = p.BillingAddress;
                            c.C_FullName = p.FullName;
                            c.C_PackageCode = p.PackageCode;
                        }
                    }

                    customer_callLines.Add(c);
                }
                else
                {
                    Console.WriteLine("Obj Not found");
                }
            }
            
           
            return obj;
        }

        /*+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++*/
        /* Calculate the bill and put into a Object*/
        public CalculatedBill Genarate(string val)
        {
            ScanCallCSV(path_call);
            CallDetailsOf_EachCustomer(val);
            CalculatedBill obj = new CalculatedBill();
            //obj.BillAmount = 0.0;
            double callAmount = 0.0;
            

            foreach (CallOfCustomer s in customer_callLines)
            {
                int rental = 100;
                string[] call_header_dialed = s.C_CallPNumber.Split('-').Select(sValue => sValue.Trim()).ToArray();
                string header = call_header_dialed[0];
                string[] call_header_receved = s.C_EndPNumber.Split('-');
                obj.PNumber = s.C_CallPNumber;
                obj.BillingAdress = s.C_CallAddress;
                obj.Rental = rental;
                string f = call_header_receved[0];

                /* For Local Area Calls*/
                if (header == call_header_receved[0])
                {
                    /*In Peak Time*/
                    if (s.C_CallStartTime.Hour > 8 && s.C_CallStartTime.AddMinutes(s.C_CallDuration / 60).Hour < 20)
                    {
                        int Pack1_Peak_Local_CallCarge = 3;
                        obj.TotalCallCharge = Pack1_Peak_Local_CallCarge * s.C_CallDuration / 60;
                        obj.Tax = (obj.TotalCallCharge + 100) * 20 / 100;
                        callAmount = callAmount + obj.TotalCallCharge + obj.Tax + obj.Rental;
                    }
                    /*In Off Peak Time*/
                    else if (s.C_CallStartTime.Hour < 8 && s.C_CallStartTime.AddMinutes(s.C_CallDuration / 60).Hour > 20)
                    {
                        int Pack1_OffPeak_Local_CallCarge = 2;
                        obj.TotalCallCharge = Pack1_OffPeak_Local_CallCarge * s.C_CallDuration / 60;
                        obj.Tax = (obj.TotalCallCharge + 100) * 20 / 100;
                        callAmount = callAmount + obj.TotalCallCharge + obj.Tax + obj.Rental;
                    }                                        
                }

                /* For Long Area Calls*/
                else if (header != call_header_receved[0]/*Long*/ )
                {
                    /*In Peak Time*/
                    if (s.C_CallStartTime.Hour > 8 && s.C_CallStartTime.AddMinutes(s.C_CallDuration / 60).Hour < 20)
                    {
                        int Pack1_Peak_Long_CallCarge = 5;
                        obj.TotalCallCharge = Pack1_Peak_Long_CallCarge * s.C_CallDuration / 60;
                        obj.Tax = (obj.TotalCallCharge + 100) * 20 / 100;
                        callAmount = callAmount + obj.TotalCallCharge + obj.Tax + obj.Rental;
                    }
                    /*In Off Peak Time*/
                    else if ((s.C_CallStartTime.Hour < 8 && s.C_CallStartTime.AddMinutes(s.C_CallDuration / 60).Hour > 20))
                    {
                        int Pack1_OffPeak_Long_CallCarge = 4;
                        obj.TotalCallCharge = Pack1_OffPeak_Long_CallCarge * s.C_CallDuration / 60;
                        obj.Tax = (obj.TotalCallCharge + 100) * 20 / 100;
                        callAmount = callAmount + obj.TotalCallCharge + obj.Tax + obj.Rental;
                    }

                }

                obj.BillAmount = callAmount;
            }
            PrintBill(obj);
            return obj;
        }
        /*+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++*/
        /* Print the calculated Bill*/
        public void PrintBill(object obj)
        {
            CalculatedBill res = new CalculatedBill();
            Console.WriteLine("PhoneNumber: " + res.PNumber);
            Console.WriteLine("BillingAdress: " + res.BillingAdress);
            Console.WriteLine("TotalCallCharge: " + res.TotalCallCharge);
            Console.WriteLine("Tax: " + res.Tax);
            Console.WriteLine("Rental: " + res.Rental);
            Console.WriteLine("BillAmount: " + res.BillAmount);
            Console.WriteLine("Call_Bills");

        }
        
        







    }
}
