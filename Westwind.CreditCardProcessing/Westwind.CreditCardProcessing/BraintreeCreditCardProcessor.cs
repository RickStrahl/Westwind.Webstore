using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Braintree;
using Westwind.Utilities;
using ValidationError = Braintree.ValidationError;

namespace Westwind.CreditCardProcessing
{

    /// <summary>
    /// BrainTree is a flat rate credit card processor meaning that each transaction
    /// is charged a flat rate of 2.9% (currently). There are no additional monthly
    /// or transaction fees - just the flat rate percentage. Although the rates
    /// may seem high, given that you don't pay per transaction and no gateway and 
    /// other processing fees, this rate works out very well especially if your 
    /// volume is relatively low or you're processing lots of high end business
    /// or reward or AMEX cards which tend to have higher merchant percentages per 
    /// charge anyway.
    /// 
    /// https://www.braintreepayments.com/
    /// </summary>
    public class BraintreeCreditCardProcessor : CreditCardProcessorBase
    {
        public string SubscriptionPlan { get; set; }


        /// <summary>
        /// Processes a simple Credit Card Transaction with Braintree
        /// </summary>
        /// <param name="AuthId">Optional Authentication Id used for auth capture or transaction lookups</param>
        /// <returns></returns>
        public override CreditCardProcessingResult Process(string preAuthCodeOrTransactionId = null)
        {
            var baseResult = base.Process();
            if (!baseResult.IsSuccess)
            {
                LogTransaction();
                return baseResult;
            }

            var gateway = new BraintreeGateway()
            {
                MerchantId = Merchant.MerchantId,
                PublicKey = Merchant.PublicKey,
                PrivateKey = Merchant.PrivateKey
            };

            if (Configuration.UseTestTransaction)
                gateway.Environment = Braintree.Environment.SANDBOX;
            else
                gateway.Environment = Braintree.Environment.PRODUCTION;

            var request = new TransactionRequest
            {
                Amount = Order.OrderAmount
            };

            if (!string.IsNullOrEmpty(preAuthCodeOrTransactionId))
            {
                // we're doing auth-capture
            }
            else if (!string.IsNullOrEmpty(Order.ClientNonce))
            {
                request.PaymentMethodNonce = Order.ClientNonce.Trim();                               
            }
            else
            {
                if (string.IsNullOrEmpty(Order.CardNumber))
                {
                    baseResult.IsSuccess = false;
                    baseResult.Message = "Missing credit card";
                    return baseResult;
                }

                request.CreditCard = new TransactionCreditCardRequest
                {
                    Number = Regex.Replace(Order.CardNumber, @"[ -/._#]", ""),
                    ExpirationMonth = Order.ExpirationMonth,
                    ExpirationYear = Order.ExpirationYear,
                    CVV = Order.SecurityCode,
                    CardholderName = BillingInfo.Name,
                };
            }

            // braintree only allows two decimals so round to
            request.Amount = Math.Round(request.Amount, 2);
            
            if (!string.IsNullOrEmpty(Order.OrderId))
                request.OrderId = Order.OrderId;
            
            request.BillingAddress = new AddressRequest()
            {
                LastName = BillingInfo.LastName,
                FirstName = BillingInfo.FirstName,
                StreetAddress = BillingInfo.Address,
                PostalCode = BillingInfo.PostalCode,
                CountryCodeAlpha2 = BillingInfo.CountryCode                
            };
            if (!string.IsNullOrEmpty(BillingInfo.Company))
                request.BillingAddress.Company = BillingInfo.Company;


            if (Order.ProcessType == ccProcessTypes.Sale || Order.ProcessType == ccProcessTypes.AuthCapture)
            {                
                request.Options = new TransactionOptionsRequest()
                {
                    SubmitForSettlement = true
                };
            }

            // assume failed 
            Result.ValidatedResult = ccProcessResults.Failed;
            Result.IsSuccess = false;

            Result<Transaction> braintreeResult;
            try
            {
                if (Order.ProcessType == ccProcessTypes.AuthCapture)
                    braintreeResult = gateway.Transaction.SubmitForSettlement(preAuthCodeOrTransactionId,Order.OrderAmount);
                else if (Order.OrderAmount >= 0)
                    braintreeResult = gateway.Transaction.Sale(request);
                else
                    braintreeResult = gateway.Transaction.Credit(request);
            }
            catch (Exception ex)
            {
                Result.ValidatedResult = ccProcessResults.Failed;
                Result.Message = ex.GetBaseException().Message;

                LogTransaction();
                return Result;
            }

            Result.ProcessorResultObject = braintreeResult;
            
            if (braintreeResult.IsSuccess())
            {
                var transaction = braintreeResult.Target;
                Result.TransactionId = transaction.Id;
                Result.AuthorizationCode = transaction.ProcessorAuthorizationCode;
                
                if (Order.ProcessType == ccProcessTypes.PreAuth)
                    Result.ValidatedResult = ccProcessResults.Authorized;
                else
                    Result.ValidatedResult = ccProcessResults.Approved;
                
                Result.ProcessedAmount = transaction.Amount ?? 0M;
                Result.CCLastFour = transaction.CreditCard.MaskedNumber;
                
                // update BillingInfor from BrainTree Processor where possible
                var resultObject = Result.ProcessorResultObject as Result<Transaction>;
                var postalCode = resultObject.Target?.BillingAddress?.PostalCode;
                var countryCode = resultObject.Target?.BillingAddress?.CountryCodeAlpha2;
                if (string.IsNullOrEmpty(BillingInfo.PostalCode))
                    BillingInfo.PostalCode = postalCode;
                if (string.IsNullOrEmpty(BillingInfo.CountryCode))
                {
                    BillingInfo.CountryCode = countryCode;
                    BillingInfo.Country = resultObject.Target?.BillingAddress?.CountryName;
                }

                Result.IsSuccess = true;
                Result.AvsResultCode = transaction.AvsErrorResponseCode + ": " + transaction.AvsPostalCodeResponseCode + " " + transaction.AvsStreetAddressResponseCode;
                Result.CvvResultCode = transaction.CvvResponseCode;

                Result.ExtendedMessage = braintreeResult.Message + "\r\n" +
                                         "Status: " + transaction.Status + "\r\n" +
                                         "Code: " + transaction.ProcessorResponseCode + "\r\n" +
                                         "Text: " + transaction.ProcessorResponseText + "\r\n" +
                                         " AVS: " + Result.AvsResultCode + "\r\n" +
                                         " CVV: " + Result.CvvResultCode;
            }
            else if (braintreeResult.Transaction != null)
            {
                Transaction transaction = braintreeResult.Transaction;
                Result.TransactionId = transaction.Id;

                /// TODO: Figure out additional statuses from message?
                Result.ValidatedResult = ccProcessResults.Declined;

                Result.CCLastFour = transaction.CreditCard?.MaskedNumber;
                
                Result.Message = braintreeResult.Message;                
                Result.AvsResultCode = transaction.AvsErrorResponseCode + ": " + transaction.AvsPostalCodeResponseCode + " " + transaction.AvsStreetAddressResponseCode;
                Result.CvvResultCode = transaction.CvvResponseCode;

                Result.ExtendedMessage = braintreeResult.Message + "\r\n" +
                                         "Status: " + transaction.Status + "\r\n" +
                                         "Code: " + transaction.ProcessorResponseCode + "\r\n" +
                                         "Text: " + transaction.ProcessorResponseText + "\r\n" +
                                         " AVS: " + Result.AvsResultCode + "\r\n" +
                                         " CVV: " + Result.CvvResultCode;

            }
            else
            {
                Result.ValidatedResult = ccProcessResults.Declined;
                Result.Message = braintreeResult.Message;                
                StringBuilder sb = new StringBuilder();

                foreach (ValidationError error in braintreeResult.Errors.DeepAll())
                {
                    sb.AppendLine("Attribute: " + error.Attribute);
                    sb.AppendLine("     Code: " + error.Code);
                    sb.AppendLine("  Message: " + error.Message);
                }
                Result.ExtendedMessage = sb.ToString();
            }


            try
            {
                Result.RawProcessorResult = JsonSerializationUtils.Serialize(braintreeResult, true, true);
                Result.RawProcessorRequest = JsonSerializationUtils.Serialize(request, true, true);
            }
            catch (Exception ex)
            {
                Result.RawProcessorResult = ex.Message;
            }

            LogTransaction();
            
            return Result;
        }

        public string GenerateClientToken()
        {
            var env = Braintree.Environment.PRODUCTION;
            if (Configuration.UseTestTransaction)
                env = Braintree.Environment.SANDBOX;

            var gateway = new BraintreeGateway(){
                Environment = env,
                MerchantId = Merchant.MerchantId,
                PublicKey = Merchant.PublicKey,
                PrivateKey = Merchant.PrivateKey
            };
            
            try
            {
                return gateway.ClientToken.Generate();
            }
            catch(Exception ex)
            {                
                this.ErrorMessage = ex.Message;
                return null;
            }
        }


        /// <summary>
        /// Creates a a new subscription and customer if it doesn't exist
        /// </summary>
        /// <param name="customerId">Optional - pass in a processor customer id if you have one and it will remove existing credit cards and replace with new data.</param>
        /// <returns></returns>
        public CreditCardProcessingResult CreateSubscription(string customerId = null)
        {
            var customer = CreateOrUpdateCustomer(customerId);
            if (customer == null)
            {
                SetError(ErrorMessage);
                return null;
            }

            var gateway = CreateGateway();

            var card = customer.CreditCards.FirstOrDefault();

            Result.CustomerId = customer.Id;
            Result.CreditCardToken = card.Token;
            
            var subscriptionRequest = new SubscriptionRequest
            {
                PaymentMethodToken =  Result.CreditCardToken,
                PlanId = SubscriptionPlan
            };

            Result<Subscription> subResult;
            try
            {
                subResult = gateway.Subscription.Create(subscriptionRequest);
            }
            catch (Exception ex)
            {
                Result.ValidatedResult = ccProcessResults.Failed;
                Result.Message = ex.GetBaseException().Message;                
                return Result;            
            }
            
            Result.ProcessorResultObject = subResult;

            if (!subResult.IsSuccess())
            {
                Result.ValidatedResult = ccProcessResults.Failed;
                Result.Message = subResult.Message;
                return Result;
            }

            Result.ValidatedResult = ccProcessResults.Approved;
            Result.SubscriptionId = subResult.Target.Id;
            Result.ProcessorResultObject = subResult;

            return Result;
        }

        /// <summary>
        /// Creates or updates an existing customer and 
        /// removes all but the current card.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="creditCardToken"></param>
        /// <returns></returns>
        public Customer CreateOrUpdateCustomer(string customerId = null)
        {
            var gateway = CreateGateway();

            var request = new CustomerRequest
            {
                FirstName = BillingInfo.FirstName,
                LastName = BillingInfo.LastName,
                Email = BillingInfo.Email,
                CreditCard = new CreditCardRequest
                {
                    CardholderName = BillingInfo.Name,
                    Number = Order.CardNumber,                    
                    ExpirationMonth = Order.ExpirationMonth,
                    ExpirationYear = Order.ExpirationYear,
                    BillingAddress = new CreditCardAddressRequest()
                    {
                        FirstName = BillingInfo.FirstName,
                        LastName = BillingInfo.LastName,
                        StreetAddress = BillingInfo.Address,
                        PostalCode = BillingInfo.PostalCode,
                        CountryCodeAlpha2 = BillingInfo.CountryCode
                    }
                }
            };

            Result<Customer> custResult;
            try
            {
                if (!string.IsNullOrEmpty(customerId))
                {
                    var cust = GetCustomer(customerId);
                    foreach (var ccCard in cust.CreditCards)
                    {
                        gateway.CreditCard.Delete(ccCard.Token);
                    }
                    foreach (var addr in cust.Addresses)
                    {
                        gateway.Address.Delete(customerId,addr.Id);
                    }                              
                    custResult = gateway.Customer.Update(customerId, request);
                }
                    
                else
                    custResult = gateway.Customer.Create(request);
            }
            catch (Exception ex)
            {
                SetError(ex.GetBaseException().Message);
                return null;
            }

            if (!custResult.IsSuccess())
            {
                SetError(custResult.Message);
                return null;
            }

            var customer = custResult.Target;
            var card = customer.CreditCards.FirstOrDefault();
            if (card == null)
            {
                SetError("Failed to create new credit card.");
                return null;
            }

            return custResult.Target;            
        }

        public Customer GetCustomer(string custId)
        {
            var gateway = CreateGateway();

            Customer customer;
            try
            {
               customer = gateway.Customer.Find(custId);
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
                return null;
            }

            return customer;
        }        

        public Subscription GetSubscription(string id)
        {
            var gateway = CreateGateway();

            Subscription subscription;
            try
            {
                subscription = gateway.Subscription.Find(id);
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
                return null;                
            }

            return subscription;
        }

        /// <summary>
        /// Cancels a subscription
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CancelSubscription(string id)
        {
            var gateway = CreateGateway();

            Result<Subscription> result;
            try
            {
                result = gateway.Subscription.Cancel(id); 
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
                return false;
            }

            if (result.IsSuccess())
                return true;
            
            
            if (!result.IsSuccess())
            {
                SetError(result.Message);
            }

            return false;
        }

        public CreditCardProcessingResult UpdateSubscription(string id)
        {
            throw new NotSupportedException();
        }

        public CreditCardProcessingResult CreateCreditCard(string customerId)
        {
            var gateway = CreateGateway();

            var request = new CreditCardRequest
            {
                CustomerId = customerId,
                Number = Order.CardNumber,
                ExpirationMonth = Order.ExpirationMonth,
                ExpirationYear = Order.ExpirationYear,                                
                BillingAddress = new CreditCardAddressRequest
                {
                    FirstName = BillingInfo.FirstName,
                    LastName = BillingInfo.LastName,
                    Company = BillingInfo.Company,
                    StreetAddress = BillingInfo.Address,
                    //ExtendedAddress = "Suite 403",
                    Locality = BillingInfo.City,
                    Region = BillingInfo.State,
                    PostalCode = BillingInfo.PostalCode,
                    CountryCodeAlpha2 = BillingInfo.CountryCode
                }
            };


            Result = new CreditCardProcessingResult();

            try
            {
                var result = gateway.CreditCard.Create(request);
            }
            catch (Exception ex)
            {
                Result.ValidatedResult = ccProcessResults.Failed;
                Result.Message = ex.GetBaseException().Message;
                return Result;
            }

            var pr = new CreditCardProcessingResult();

            return Result;

        }

        /// <summary>
        /// Creates an instance of the gateway that's configured
        /// with ids from the current object
        /// </summary>
        private BraintreeGateway CreateGateway()
        {
            var gateway = new BraintreeGateway
            {
                MerchantId = Merchant.MerchantId,
                PublicKey = Merchant.PublicKey,
                PrivateKey = Merchant.PrivateKey
            };

            if (Configuration.UseTestTransaction)
                gateway.Environment = Braintree.Environment.SANDBOX;

            return gateway;
        }
    }
}
