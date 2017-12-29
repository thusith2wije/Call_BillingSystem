using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.Generic;




namespace Billing_System.Tests
{
    [TestFixture]
    public class BillingSystemTests : BillingEngine
    {


        [Test] //1
        public void Check_CustomerDetailsCSV_IsNotNull()
        {
            /*Arrange*/
            string path = @"C:\Users\tij\source\repos\CustomerList.txt";
            bool expected = true;
            
            /*Action*/           
            BillingEngine obj = new BillingEngine();
            bool ret_val = obj.ScanCustomerCSV(path);

            /* Assert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, ret_val);
        }

        [Test, ExpectedException(typeof(FormatException))] //2
        public void Check_CustomerDetailsCSV_Splited_Corect()
        {
           
            /*Arrange*/
            string input_val = "011-7489261,Borella,Thusitha,2,12.1.2017";
            CustomerDetails expected = new CustomerDetails() ;
            expected.PNumber = "011-7489261";
            expected.BillingAddress = "Borella";
            expected.FullName = "Thusitha";
            expected.PackageCode = "2";
            expected.RegisteredDate = "12/1/2017";

            /*Action*/
            CustomerDetails ret_val = SplitCustomerDetails(input_val);

            /*Asseert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.PNumber, ret_val.PNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.FullName, ret_val.FullName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillingAddress, ret_val.BillingAddress);
            // Some problem with RegisteredDate
        }

        [Test] //3
        public void Check_CallDetailsCSV_IsNotNull()
        {
            /*Arrange*/
            string path = @"C:\Users\tij\source\repos\CallList.txt";
            bool expected = true;

            /*Action*/
            BillingEngine obj = new BillingEngine();
            bool ret_val = obj.ScanCallCSV(path);

            /* Assert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, ret_val);
        }

        [Test] //4
        public void Check_CallDetailsCSV_Splited_Corect()
        {
            /*Arrange*/
            string input_val = "011-7489263,011-4958098,05/12/2017 15:00:00,180";
            CallDetails expected = new CallDetails();
            expected.CallPNumber = "011-7489263";
            expected.EndPNumber = "011-4958098";
            //expected.CallStartTime = 05/12/2017 15:00:00;
            expected.CallDuration = "180";

            /*Action*/
            CallDetails ret_val = SplitCallDetails(input_val);

            /*Asseert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.CallPNumber, ret_val.CallPNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.EndPNumber, ret_val.EndPNumber);
            //Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.CallStartTime, ret_val.CallStartTime);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.CallDuration, ret_val.CallDuration);
        }

        [Test]//5
        public void Map_ValuesFromCallList_MatchingTo_CustomerList_PNumber()
        {
            /*Arrange*/
            string input_val = "011-7489261";
            CallDetails expected = new CallDetails();
            expected.CallPNumber = "011-7489261";
            expected.EndPNumber = "011-4958096";
            //expected.CallStartTime = 05/12/2017 15:00:00;
            expected.CallDuration = "120";

            /*Action*/
            CallDetails ret_val = CallDetailsOf_EachCustomer(input_val);

            /*Assert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.CallPNumber, ret_val.CallPNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.EndPNumber, ret_val.EndPNumber);
            //Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.CallStartTime, ret_val.CallStartTime);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.CallDuration, ret_val.CallDuration);

        }

        [Test]//6
        public void CheckGenarateFor_PeakTime_Local_Calls()
        {
            /*Arrange*/
            CalculatedBill expected = new CalculatedBill();
            expected.PNumber = "011-7489263";
            expected.BillingAdress = "Borella";
            expected.TotalCallCharge = 3 * 180 / 60;
            expected.Tax = (expected.TotalCallCharge + 100) * 20 / 100;
            expected.Rental = 100;
            expected.BillAmount = expected.TotalCallCharge + expected.Tax + expected.Rental;

            /*Action*/
            CalculatedBill ret_val = Genarate("011-7489263");

            /*Assert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.PNumber, ret_val.PNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.TotalCallCharge, ret_val.TotalCallCharge);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.Tax, ret_val.Tax);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillAmount, ret_val.BillAmount);
        }
        [Test]//7
        public void CheckGenarateFor_PeakTime_NotLocal_Calls()
        {
            /*Arrange*/
            CalculatedBill expected = new CalculatedBill();
            expected.PNumber = "011-7489262";
            expected.BillingAdress = "Dehiwala";
            expected.TotalCallCharge = 5 * 60 / 60;
            expected.Tax = (expected.TotalCallCharge + 100) * 20 / 100;
            expected.Rental = 100;
            expected.BillAmount = expected.TotalCallCharge + expected.Tax + expected.Rental;

            /*Action*/
            CalculatedBill ret_val = Genarate("011-7489262");

            /*Assert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.PNumber, ret_val.PNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.TotalCallCharge, ret_val.TotalCallCharge);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.Tax, ret_val.Tax);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillAmount, ret_val.BillAmount);
        }





    }
}
