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
        List<CalculatedBill> print_Bill = new List<CalculatedBill>();
        string path_customer = @"C:\Users\tij\source\repos\Billing_System\CustomerList.txt";
        string path_call = @"C:\Users\tij\source\repos\Billing_System\CallList.txt";
        //private TimeSpan workdayStartTime;
        private TimeSpan workdayStopTime;
        TimeSpan startPeakTime = new TimeSpan(8, 0, 0);
        TimeSpan stopPeakTime = new TimeSpan(20, 0, 0);
        TimeSpan startDayTime = new TimeSpan(0, 0, 0);
        TimeSpan stoptDayTime = new TimeSpan(23, 59, 0);


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
            int i = 0;
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

                    /* Add some customer details into CallOfCustomer object */
                    //if (i == 0)
                    //{
                        foreach (CustomerDetails p in cutomer_Lines)
                        {
                            if (p.PNumber == input_val)
                            {
                                c.C_CallAddress = p.BillingAddress;
                                c.C_FullName = p.FullName;
                                c.C_PackageCode = p.PackageCode;
                                c.C_CustomerReg_Date = p.RegisteredDate;
                            }
                        }
                    //    i = i + 1;                       
                    ////}
                    

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
        public CalculatedBill Genarate(string inputval)
        {
            cutomer_Lines.Clear();
            call_Lines.Clear();
            customer_callLines.Clear();
            print_Bill.Clear();

            ScanCustomerCSV(path_customer);
            ScanCallCSV(path_call);
            CallDetailsOf_EachCustomer(inputval);

            CalculatedBill ret_obj = new CalculatedBill();            
            int i = 1;

            foreach (CallOfCustomer s in customer_callLines)
            {//

                ExitLable:
                CalculatedBill obj = new CalculatedBill();
                string[] call_header_dialed = s.C_CallPNumber.Split('-').Select(sValue => sValue.Trim()).ToArray();
                string header = call_header_dialed[0];
                string[] call_header_receved = s.C_EndPNumber.Split('-');
                obj.PNumber = s.C_CallPNumber;
                obj.BillingAdress = s.C_CallAddress;
                //obj.Rental = rental;
                obj.Package_Code = s.C_PackageCode;
                string f = call_header_receved[0];

                
                if(obj.Package_Code == "A")
                {
                    /* For Local Area Calls*/
                    if (header == call_header_receved[0])
                    {
                        /*In Peak Time*/
                        if (s.C_CallStartTime.Hour > startPeakTime.Hours && s.C_CallStartTime.Hour < stopPeakTime.Hours /*&& s.C_CallStartTime.AddMinutes(s.C_CallDuration / 60).Hour < stopPeakTime.Hours*/)
                        {
                            int PackA_Peak_Local_CallCarge = 3;
                            if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) <= stopPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackA_Peak_Local_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);
                            }
                            else if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) > stopPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                int a = s.C_CallDuration;
                                s.C_CallDuration = (stopPeakTime.Hours - s.C_CallStartTime.Hour) * 3600;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackA_Peak_Local_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);

                                s.C_CallDuration = a - s.C_CallDuration;
                                s.C_CallStartTime = Convert.ToDateTime(stopPeakTime.ToString());

                                //continue;//break;
                                goto ExitLable;
                            }

                        }
                        /*In Off Peak Time and before to 24*/
                        else if ((s.C_CallStartTime.Hour < stoptDayTime.Hours && s.C_CallStartTime.Hour >= stopPeakTime.Hours))
                        {
                            //Console.WriteLine("Call: " + i);
                            int PackA_OffPeak_Local_CallCarge = 2;

                            if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) <= 24)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackA_OffPeak_Local_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);
                            }
                            else if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) > 24)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                int a = s.C_CallDuration;
                                s.C_CallDuration = (24 - s.C_CallStartTime.Hour) * 3600;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackA_OffPeak_Local_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);

                                s.C_CallDuration = a - s.C_CallDuration;
                                s.C_CallStartTime = Convert.ToDateTime(startDayTime.ToString());

                                //continue;//break;
                                goto ExitLable;
                            }

                        }
                        /*In Off Peak Time and After to 24*/
                        else if ((s.C_CallStartTime.Hour >= startDayTime.Hours && s.C_CallStartTime.Hour < startPeakTime.Hours))
                        {
                            //Console.WriteLine("Call: " + i);
                            int PackA_OffPeak_Local_CallCarge = 2;

                            if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) <= startPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackA_OffPeak_Local_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);
                            }
                            else if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) > startPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                int a = s.C_CallDuration;
                                s.C_CallDuration = (startPeakTime.Hours - s.C_CallStartTime.Hour) * 3600;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackA_OffPeak_Local_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);

                                s.C_CallDuration = a - s.C_CallDuration;
                                s.C_CallStartTime = Convert.ToDateTime(startPeakTime.ToString());

                                //continue;//break;
                                goto ExitLable;
                            }

                        }

                    }
                    /* For Long Area Calls*/
                    else if (header != call_header_receved[0])
                    {

                        /*In Peak Time*/
                        if (s.C_CallStartTime.Hour > startPeakTime.Hours && s.C_CallStartTime.Hour < stopPeakTime.Hours /*&& s.C_CallStartTime.AddMinutes(s.C_CallDuration / 60).Hour < stopPeakTime.Hours*/)
                        {
                            int PackB_Peak_Long_CallCarge = 6;
                            if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) <= stopPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_Peak_Long_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);
                            }
                            else if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) > stopPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                int a = s.C_CallDuration;
                                s.C_CallDuration = (stopPeakTime.Hours - s.C_CallStartTime.Hour) * 3600;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_Peak_Long_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);

                                s.C_CallDuration = a - s.C_CallDuration;
                                s.C_CallStartTime = Convert.ToDateTime(stopPeakTime.ToString());

                                //continue;//break;
                                goto ExitLable;
                            }

                        }
                        /*In Off Peak Time and before to 24*/
                        else if ((s.C_CallStartTime.Hour < stoptDayTime.Hours && s.C_CallStartTime.Hour >= stopPeakTime.Hours))
                        {
                            //Console.WriteLine("Call: " + i);
                            int PackB_OffPeak_Long_CallCarge = 5;

                            if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) <= 24)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_OffPeak_Long_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);
                            }
                            else if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) > 24)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                int a = s.C_CallDuration;
                                s.C_CallDuration = (24 - s.C_CallStartTime.Hour) * 3600;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_OffPeak_Long_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);

                                s.C_CallDuration = a - s.C_CallDuration;
                                s.C_CallStartTime = Convert.ToDateTime(startDayTime.ToString());

                                //continue;//break;
                                goto ExitLable;
                            }

                        }
                        /*In Off Peak Time and After to 24*/
                        else if ((s.C_CallStartTime.Hour >= startDayTime.Hours && s.C_CallStartTime.Hour < startPeakTime.Hours))
                        {
                            //Console.WriteLine("Call: " + i);
                            int PackB_OffPeak_Long_CallCarge = 5;

                            if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) <= startPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_OffPeak_Long_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);
                            }
                            else if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) > startPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                int a = s.C_CallDuration;
                                s.C_CallDuration = (startPeakTime.Hours - s.C_CallStartTime.Hour) * 3600;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_OffPeak_Long_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);

                                s.C_CallDuration = a - s.C_CallDuration;
                                s.C_CallStartTime = Convert.ToDateTime(startPeakTime.ToString());

                                //continue;//break;
                                goto ExitLable;
                            }

                        }

                    }

                }
                
                else if(obj.Package_Code == "B")
                {
                    /* For Local Area Calls*/
                    if (header == call_header_receved[0])
                    {
                        /*In Peak Time*/
                        if (s.C_CallStartTime.Hour > startPeakTime.Hours && s.C_CallStartTime.Hour < stopPeakTime.Hours /*&& s.C_CallStartTime.AddMinutes(s.C_CallDuration / 60).Hour < stopPeakTime.Hours*/)
                        {
                            int PackB_Peak_Local_CallCarge = 4;
                            if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) <= stopPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_Peak_Local_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);
                            }
                            else if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) > stopPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                int a = s.C_CallDuration;
                                s.C_CallDuration = (stopPeakTime.Hours - s.C_CallStartTime.Hour) * 3600;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_Peak_Local_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);

                                s.C_CallDuration = a - s.C_CallDuration;
                                s.C_CallStartTime = Convert.ToDateTime(stopPeakTime.ToString());

                                //continue;//break;
                                goto ExitLable;
                            }

                        }
                        /*In Off Peak Time and before to 24*/
                        else if ((s.C_CallStartTime.Hour < stoptDayTime.Hours && s.C_CallStartTime.Hour >= stopPeakTime.Hours))
                        {
                            //Console.WriteLine("Call: " + i);
                            int PackB_OffPeak_Local_CallCarge = 3;

                            if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) <= 24)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_OffPeak_Local_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);
                            }
                            else if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) > 24)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                int a = s.C_CallDuration;
                                s.C_CallDuration = (24 - s.C_CallStartTime.Hour) * 3600;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_OffPeak_Local_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);

                                s.C_CallDuration = a - s.C_CallDuration;
                                s.C_CallStartTime = Convert.ToDateTime(startDayTime.ToString());

                                //continue;//break;
                                goto ExitLable;
                            }

                        }
                        /*In Off Peak Time and After to 24*/
                        else if ((s.C_CallStartTime.Hour >= startDayTime.Hours && s.C_CallStartTime.Hour < startPeakTime.Hours))
                        {
                            //Console.WriteLine("Call: " + i);
                            int PackB_OffPeak_Local_CallCarge = 3;

                            if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) <= startPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_OffPeak_Local_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);
                            }
                            else if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) > startPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                int a = s.C_CallDuration;
                                s.C_CallDuration = (startPeakTime.Hours - s.C_CallStartTime.Hour) * 3600;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_OffPeak_Local_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);

                                s.C_CallDuration = a - s.C_CallDuration;
                                s.C_CallStartTime = Convert.ToDateTime(startPeakTime.ToString());

                                //continue;//break;
                                goto ExitLable;
                            }

                        }

                    }
                    /* For Long Area Calls*/
                    else if (header != call_header_receved[0])
                    {
                       
                        /*Start In Peak Time*/
                        if (s.C_CallStartTime.Hour > startPeakTime.Hours && s.C_CallStartTime.Hour < stopPeakTime.Hours /*&& s.C_CallStartTime.AddMinutes(s.C_CallDuration / 60).Hour < stopPeakTime.Hours*/)
                        {
                            int PackB_Peak_Long_CallCarge = 6;
                            if ((s.C_CallStartTime.Hour + s.C_CallDuration/3600) <= stopPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_Peak_Long_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);
                            }
                            else if ((s.C_CallStartTime.Hour + s.C_CallDuration/3600) > stopPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                int a = s.C_CallDuration;
                                s.C_CallDuration = (stopPeakTime.Hours - s.C_CallStartTime.Hour) * 3600;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_Peak_Long_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);

                                s.C_CallDuration = a - s.C_CallDuration;                                
                                s.C_CallStartTime = Convert.ToDateTime(stopPeakTime.ToString());

                                //continue;//break;
                                goto ExitLable;
                            }
                            
                        }
                        /*In Off Peak Time and before to 24*/
                        else if ((s.C_CallStartTime.Hour < stoptDayTime.Hours && s.C_CallStartTime.Minute < stoptDayTime.Minutes && s.C_CallStartTime.Hour >= stopPeakTime.Hours))
                        {
                            //Console.WriteLine("Call: " + i);
                            int PackB_OffPeak_Long_CallCarge = 5;
                            
                            if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) <= 24)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_OffPeak_Long_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);
                            }
                            else if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) > 24)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                int a = s.C_CallDuration;
                                s.C_CallDuration = (24 - s.C_CallStartTime.Hour) * 3600;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_OffPeak_Long_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);

                                s.C_CallDuration = a - s.C_CallDuration;
                                s.C_CallStartTime = Convert.ToDateTime(startDayTime.ToString());

                                //continue;//break;
                                goto ExitLable;
                            }
                            
                        }
                        /*In Off Peak Time and After to 24*/
                        else if ((s.C_CallStartTime.Hour >= startDayTime.Hours && s.C_CallStartTime.Hour < startPeakTime.Hours))
                        {
                            //Console.WriteLine("Call: " + i);
                            int PackB_OffPeak_Long_CallCarge = 5;

                            if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) <= startPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_OffPeak_Long_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);
                            }
                            else if ((s.C_CallStartTime.Hour + s.C_CallDuration / 3600) > startPeakTime.Hours)
                            {
                                obj.CallNo = i;
                                obj.CallStartTime = s.C_CallStartTime;
                                int a = s.C_CallDuration;
                                s.C_CallDuration = (startPeakTime.Hours - s.C_CallStartTime.Hour) * 3600;
                                obj.CallDuration = s.C_CallDuration;
                                obj.Destination = s.C_EndPNumber;
                                obj.Each_CallCarge = PackB_OffPeak_Long_CallCarge * s.C_CallDuration / 60;

                                print_Bill.Add(obj);

                                s.C_CallDuration = a - s.C_CallDuration;
                                s.C_CallStartTime = Convert.ToDateTime(startPeakTime.ToString());

                                //continue;//break;
                                goto ExitLable;
                            }

                        }

                    }

                }
                i = i + 1;
                ret_obj = obj;              
                
            }//end foreach

            double temp_val = PrintBill();
            ret_obj.BillAmount = temp_val;

            return ret_obj;
        }
        /*+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++*/
        /* Print the calculated Bill*/
        public double PrintBill()
        {
            int n = 0;
            double totalCallCharge = 0;
            int rental = 100;
            double tax_pre = 20 / 100;
            double tax = 0;

            foreach (CalculatedBill res in print_Bill)
            {
                if (n == 0)
                {
                    Console.WriteLine("PhoneNumber: " + res.PNumber);
                    Console.WriteLine("BillingAdress: " + res.BillingAdress);
                    Console.WriteLine("totalCallCharge: " + totalCallCharge);
                    Console.WriteLine("Rental: " + res.Rental);
                    
                }
                
                Console.WriteLine("-- -- -- --");
                Console.WriteLine("Call: " + res.CallNo);
                Console.WriteLine("StartTime: " + res.CallStartTime);
                Console.WriteLine("CallDuration: "+ res.CallDuration);
                Console.WriteLine("DestinationNumber: "+ res.Destination);
                Console.WriteLine("This Call Charge: "+ res.Each_CallCarge);

                totalCallCharge = totalCallCharge + res.Each_CallCarge;

                n = n + 1;
            }
            Console.WriteLine("--------------------");
            Console.WriteLine("Totai Call Charges: " + totalCallCharge);
            tax = (totalCallCharge + rental) * 20/100;
            Console.WriteLine("Tax: " +  tax);
            Console.WriteLine("BillAmount: " + (totalCallCharge+tax+rental));

            //CalculatedBill ret_val = new CalculatedBill();
            //ret_val.Tax = tax;
            double BillAmount = totalCallCharge + tax + rental;

            return BillAmount;
        }









    }
}
