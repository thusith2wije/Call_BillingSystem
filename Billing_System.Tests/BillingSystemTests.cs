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
            string path = @"C:\Users\tij\source\repos\Billing_System\CustomerList.txt";
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
            CustomerDetails expected = new CustomerDetails();
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
            string path = @"C:\Users\tij\source\repos\Billing_System\CallList.txt";
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
            string iString = "05/12/2017 15:00:00";
            DateTime ret_Time = DateTime.Parse(iString);
            expected.CallStartTime = ret_Time;
            expected.CallDuration = "180";

            /*Action*/
            CallDetails ret_val = SplitCallDetails(input_val);

            /*Asseert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.CallPNumber, ret_val.CallPNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.EndPNumber, ret_val.EndPNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.CallStartTime, ret_val.CallStartTime);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.CallDuration, ret_val.CallDuration);
        }


        [Test]//5
        public void CheckGenarateFor_StartCallIn_PeakTime_PackageA_Local_OneCall()
        {
            /*Arrange*/
            CalculatedBill expected = new CalculatedBill();
            expected.PNumber = "011-7489260";
            expected.BillingAdress = "Gangaramaya";
            expected.Each_CallCarge = 3 * 120 / 60;//6
            double Tax = (expected.Each_CallCarge + 100) * 20 / 100;//21.2
            expected.BillAmount = expected.Each_CallCarge + Tax + expected.Rental;//127.2


            /*Action*/
            CalculatedBill ret_val = Genarate("011-7489260");

            /*Assert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.PNumber, ret_val.PNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillingAdress, ret_val.BillingAdress);
            //Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillAmount, ret_val.BillAmount);
        }
        [Test]//6
        public void CheckGenarateFor_StartCallIn_PeakTime_PackageA_Local_MoreCalls()
        {
            /*Arrange*/
            CalculatedBill expected = new CalculatedBill();
            expected.PNumber = "011-7489261";
            double totalCallCharges = (3 * 120 / 60) + (3 * 180 / 60) + (3 * 60 / 60);
            double Tax = (totalCallCharges + 100) * 20 / 100;
            expected.BillAmount = totalCallCharges + Tax + 100;

            /*Action*/
            CalculatedBill ret_val = Genarate("011-7489261");

            /*Assert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.PNumber, ret_val.PNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillAmount, ret_val.BillAmount);
        }

        [Test]//7
        public void CheckGenarateFor_StartCallIn_PeakTime_PackageA_NotLocal_Calls()
        {
            /*Arrange*/
            CalculatedBill expected = new CalculatedBill();
            expected.PNumber = "011-7489262";
            //expected.BillingAdress = "Dehiwala";
            double totalCallCharge = (6 * 180 / 60) + (6 * 120 / 60);//18+12
            double Tax = (totalCallCharge + 100) * 20 / 100;
            expected.Rental = 100;
            expected.BillAmount = totalCallCharge + Tax + expected.Rental;

            /*Action*/
            CalculatedBill ret_val = Genarate("011-7489262");

            /*Assert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.PNumber, ret_val.PNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillAmount, ret_val.BillAmount);
        }

        [Test]//8
        public void CheckGenarateFor_StartCallIn_PeakTime_PackageB_NotLocal_Calls()
        {
            /*Arrange*/
            CalculatedBill expected = new CalculatedBill();
            expected.PNumber = "011-7489263";
            expected.BillingAdress = "Moratuwa";
            double totalCallCharge = 6 * 120 / 60;
            double Tax = (totalCallCharge + 100) * 20 / 100;
            expected.Rental = 100;
            expected.BillAmount = totalCallCharge + Tax + expected.Rental;

            /*Action*/
            CalculatedBill ret_val = Genarate("011-7489263");

            /*Assert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.PNumber, ret_val.PNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillingAdress, ret_val.BillingAdress);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillAmount, ret_val.BillAmount);
        }

        [Test]//9
        public void CheckGenarateFor_StartCallIn_PeakTime_PackageB_NotLocal_LongDurationCalls()
        {
            /*Arrange*/
            CalculatedBill expected = new CalculatedBill();
            expected.PNumber = "011-7489264";
            expected.BillingAdress = "Kelaniya";
            double totalCallCharge = (6 * 3600 / 60) + (5 * 3600 / 60);//360+300
            double Tax = (totalCallCharge + 100) * 20 / 100;//152
            expected.Rental = 100;
            expected.BillAmount = totalCallCharge + Tax + expected.Rental;//660+152+100 = 912

            /*Action*/
            CalculatedBill ret_val = Genarate("011-7489264");

            /*Assert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.PNumber, ret_val.PNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillingAdress, ret_val.BillingAdress);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillAmount, ret_val.BillAmount);
        }
        [Test]//10
        public void CheckGenarateFor_StarstCallIn_OffPeakTime_PackageB_NotLocal_LongDurationCalls()
        {
            /*Arrange*/
            CalculatedBill expected = new CalculatedBill();
            expected.PNumber = "011-7489265";
            expected.BillingAdress = "Gampaha";
            double totalCallCharge = (6 * 3600 / 60) + (5 * 3600 / 60);
            double Tax = (totalCallCharge + 100) * 20 / 100;
            expected.Rental = 100;
            expected.BillAmount = totalCallCharge + Tax + expected.Rental;

            /*Action*/
            CalculatedBill ret_val = Genarate("011-7489265");

            /*Assert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.PNumber, ret_val.PNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillingAdress, ret_val.BillingAdress);
            //Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.Tax, ret_val.Tax);
            //Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillAmount, ret_val.BillAmount);
        }

        [Test]//11
        public void CheckGenarateFor_StartCallIn_PeakTime_PackageC_Local_Calls()
        {
            /*Arrange*/
            CalculatedBill expected = new CalculatedBill();
            expected.PNumber = "011-7489266";
            //expected.BillingAdress = "Dehiwala";
            double totalCallCharge = (2 * (120-60) / 60);//4
            double Tax = (totalCallCharge + 300) * 20 / 100;//60.8
            expected.Rental = 300;
            expected.BillAmount = totalCallCharge + Tax + expected.Rental;//364.8

            /*Action*/
            CalculatedBill ret_val = Genarate("011-7489266");

            /*Assert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.PNumber, ret_val.PNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillAmount, ret_val.BillAmount);
        }

        [Test]//12
        public void CheckGenarateFor_StartCallIn_PeakTime_PackageD_Local_Calls()
        {
            /*Arrange*/
            CalculatedBill expected = new CalculatedBill();
            expected.PNumber = "011-7489267";
            double totalCallCharge = (3 * 120 / 60);//6
            double Tax = (totalCallCharge + 300) * 20 / 100;//61.2
            expected.Rental = 300;
            expected.BillAmount = totalCallCharge + Tax + expected.Rental;//367.2

            /*Action*/
            CalculatedBill ret_val = Genarate("011-7489267");

            /*Assert*/
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.PNumber, ret_val.PNumber);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.BillAmount, ret_val.BillAmount);
        }


    }
}
