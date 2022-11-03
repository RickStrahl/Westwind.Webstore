using System;
using NUnit.Framework;
using Westwind.CreditCardProcessing;
using Westwind.Webstore.Business;

namespace Westwind.WebStore.Business.Tests
{
    [TestFixture]
    public class BrainTreeTests
    {
        private readonly string MerchantId = wsApp.Configuration.Payment.MerchantId;
        private readonly string PublicKey = wsApp.Configuration.Payment.PublicKey;
        private readonly string PrivateKey = wsApp.Configuration.Payment.PrivateKey;

        public BrainTreeTests()
        {

        }

        [Test]
        public void GenerateClientTokenTest()
        {
            var cc = new BraintreeCreditCardProcessor();
            cc.Configuration.UseTestTransaction = true;

            cc.Merchant.MerchantId = MerchantId;
            cc.Merchant.PrivateKey = PrivateKey;
            cc.Merchant.PublicKey = PublicKey;

            Console.WriteLine("Creds: " + MerchantId + " " + PrivateKey + " " + PublicKey);

            var clientToken = cc.GenerateClientToken();
            Assert.IsNotNull(clientToken, $"client Token should not be null: {cc.ErrorMessage}");
            Console.WriteLine(clientToken);
        }


        [Test]
        public void ProcessSuccessSale()
        {
            var cc = new BraintreeCreditCardProcessor();
            cc.Configuration.UseTestTransaction = true;


            cc.Merchant.MerchantId = MerchantId;
            cc.Merchant.PublicKey = PublicKey;
            cc.Merchant.PrivateKey = PrivateKey;


            cc.Order.CardNumber = "4111111111111111";
            cc.Order.ExpirationMonth = "12";
            cc.Order.ExpirationYear = "2015";
            cc.Order.SecurityCode = "129";

            cc.Order.OrderAmount = 1.10M;
            cc.Order.ProcessType = CreditCardProcessing.ccProcessTypes.Sale;

            cc.BillingInfo.LastName = "Doe";
            cc.BillingInfo.FirstName = "Jane";
            cc.BillingInfo.Address = "123 N. 31st st";
            cc.BillingInfo.PostalCode = "97031";

            var result = cc.Process();

            Assert.IsNotNull(result);

            Console.WriteLine(result.ValidatedResult);
            Console.WriteLine(result.Message);
            Console.WriteLine(result.TransactionId);

            Assert.IsTrue(result.ValidatedResult == ccProcessResults.Approved);

        }

        [Test]
        public void ProcessDeclinedSale()
        {
            var cc = new BraintreeCreditCardProcessor();
            cc.Configuration.UseTestTransaction = true;

            cc.Merchant.MerchantId = MerchantId;
            cc.Merchant.PublicKey = PublicKey;
            cc.Merchant.PrivateKey = PrivateKey;


            cc.Order.CardNumber = "4111111111111111";
            cc.Order.ExpirationMonth = "12";
            cc.Order.ExpirationYear = "2015";
            cc.Order.SecurityCode = "129";

            //cc.BillingInfo.PostalCode = "97031";

            cc.Order.OrderAmount = 1.10M;
            cc.Order.ProcessType = CreditCardProcessing.ccProcessTypes.Sale;

            var result = cc.Process();

            Console.WriteLine(result.ValidatedResult);
            Console.WriteLine(result.Message);
            Console.WriteLine(result.TransactionId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ValidatedResult == ccProcessResults.Declined);
        }

        [Test]
        public void ProcessDeclinedCVVSale()
        {
            var cc = new BraintreeCreditCardProcessor();

            cc.Merchant.MerchantId = MerchantId;
            cc.Merchant.PublicKey = PublicKey;
            cc.Merchant.PrivateKey = PrivateKey;

            cc.Configuration.UseTestTransaction = true;

            cc.Order.CardNumber = "4000111111111115";
            cc.Order.ExpirationMonth = "12";
            cc.Order.ExpirationYear = "2015";
            cc.Order.SecurityCode = "200";

            cc.Order.OrderAmount = 1.10M;
            cc.Order.ProcessType = CreditCardProcessing.ccProcessTypes.Sale;

            cc.BillingInfo.PostalCode = "97031";

            var result = cc.Process();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ValidatedResult == ccProcessResults.Declined);
            Console.WriteLine(result.ValidatedResult);
            Console.WriteLine(result.Message);

            Console.WriteLine(result.TransactionId);
        }


        [Test]
        public void ProcessDeclinedAvsPostalCodeSale()
        {
            var cc = new BraintreeCreditCardProcessor();

            cc.Merchant.MerchantId = MerchantId;
            cc.Merchant.PublicKey = PublicKey;
            cc.Merchant.PrivateKey = PrivateKey;

            cc.Configuration.UseTestTransaction = true;

            cc.Order.CardNumber = "4000111111111115";
            cc.Order.ExpirationMonth = "12";
            cc.Order.ExpirationYear = "2015";
            cc.Order.SecurityCode = "200";

            cc.Order.OrderAmount = 1.10M;
            cc.Order.ProcessType = CreditCardProcessing.ccProcessTypes.Sale;
            cc.BillingInfo.Address = "200 Kalla Road";
            cc.BillingInfo.PostalCode = "20000";

            var result = cc.Process();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ValidatedResult == ccProcessResults.Declined);
            Console.WriteLine(result.ValidatedResult);
            Console.WriteLine(result.Message);

            Console.WriteLine(result.TransactionId);
        }


        [Test]
        public void ProcessPreAuthAndSale()
        {
            var cc = new BraintreeCreditCardProcessor();

            cc.Merchant.MerchantId = MerchantId;
            cc.Merchant.PublicKey = PublicKey;
            cc.Merchant.PrivateKey = PrivateKey;

            cc.Configuration.UseTestTransaction = true;

            cc.Order.CardNumber = "4000111111111115";
            cc.Order.ExpirationMonth = "12";
            cc.Order.ExpirationYear = "2015";
            cc.Order.SecurityCode = "129";

            cc.Order.OrderAmount = 1.10M;
            cc.Order.ProcessType = CreditCardProcessing.ccProcessTypes.PreAuth;

            cc.BillingInfo.LastName = "Doe";
            cc.BillingInfo.FirstName = "Jane";
            cc.BillingInfo.Address = "123 N. 31st st";
            cc.BillingInfo.PostalCode = "97031";


            var result = cc.Process();

            Assert.IsNotNull(result);
            Console.WriteLine(result.ValidatedResult);
            Console.WriteLine(result.Message);
            Console.WriteLine(result.TransactionId);
            Assert.IsTrue(result.ValidatedResult == ccProcessResults.Authorized, result.ValidatedResult.ToString());



            string authId = result.TransactionId;
            cc.Order.ProcessType = CreditCardProcessing.ccProcessTypes.AuthCapture;

            result = cc.Process(authId);

            Assert.IsNotNull(result);

            Console.WriteLine(result.ValidatedResult);
            Console.WriteLine(result.Message);
            Console.WriteLine(result.TransactionId);

            Assert.IsTrue(result.ValidatedResult == ccProcessResults.Approved, result.ValidatedResult.ToString());

            Console.WriteLine(result.ValidatedResult);
            Console.WriteLine(result.Message);
            Console.WriteLine(result.TransactionId);

        }
    }
}
