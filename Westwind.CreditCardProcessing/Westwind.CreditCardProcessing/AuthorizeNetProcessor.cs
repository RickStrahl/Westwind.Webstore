#if INCLUDE_AUTHORIZENET

using System;
using System.Linq;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using AuthorizeNet.Api.Controllers;
using Westwind.CreditCardProcessing;
using Westwind.Utilities;

namespace Westwind.CreditCardProcessing
{ 
    /// <summary>
    /// Processes a credit card transaction using Authorize.Net
    ///with a nonce and descriptor.
    ///
    /// Use:
    ///
    /// Merchant Id: ApiLoginId
    /// Merchant PublicKey: TransactionKey 
    /// </summary>
    public class AuthorizeNetProcessor : CreditCardProcessorBase
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public override CreditCardProcessingResult Process(string transactionId = null)
        {

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = Merchant.MerchantId,
                Item = Merchant.MerchantPassword,
                //// ApiLoginId
                //name = Merchant.MerchantId,
                ItemElementName = ItemChoiceType.transactionKey,
                //// TransactionKey
                //Item = Merchant.PublicKey,
            };


            opaqueDataType opaqueData = null;
            creditCardType creditCard = null;
            customerAddressType billingAddress= null;

            if (!string.IsNullOrEmpty(transactionId))
            {
                // we're doing auth-capture                
            }
            else if (!string.IsNullOrEmpty(Order.ClientNonce))
            {
                opaqueData = new opaqueDataType
                {
                    dataDescriptor = Order.ClientDescriptor,
                    dataValue = Order.ClientNonce
                };

            }
            //else
            {
                creditCard = new creditCardType
                {
                    cardNumber = this.Order.CardNumber,
                    expirationDate = this.Order.CardExpiration,
                    cardCode = Order.SecurityCode,

                };
                
                billingAddress = new customerAddressType
                {
                    firstName = this.BillingInfo.FirstName,
                    lastName = BillingInfo.LastName,
                    address = BillingInfo.Address,
                    city = BillingInfo.City,
                    zip = BillingInfo.PostalCode
                };
            }

            //standard api call to retrieve response
            object processItem = opaqueData;
            if (processItem == null)
                processItem = creditCard;
            var payment = new paymentType {
                Item = processItem
            };

            var authType = transactionTypeEnum.authOnlyTransaction;
            if (Order.ProcessType == ccProcessTypes.Sale)
                authType = transactionTypeEnum.authCaptureTransaction;
            else if (Order.ProcessType == ccProcessTypes.PreAuth)
                authType = transactionTypeEnum.authOnlyTransaction;
            else if (Order.ProcessType == ccProcessTypes.AuthCapture)
                authType = transactionTypeEnum.priorAuthCaptureTransaction;
            
            transactionRequestType transactionRequest;
            if (!string.IsNullOrEmpty(transactionId) && Order.ProcessType == ccProcessTypes.AuthCapture)
            {
                transactionRequest = new transactionRequestType
                {
                    transactionType = authType.ToString(),
                    refTransId = transactionId
                };
            }
            else
            {
                transactionRequest = new transactionRequestType
                {
                    transactionType = authType.ToString(),
                    amount = Order.OrderAmount, 
                    payment = payment,
                    refTransId = transactionId,
                    billTo = billingAddress
                };
            }
            
            var request = new createTransactionRequest { transactionRequest = transactionRequest, refId = Order.OrderId };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            // validate response
            if (response != null)
            {
                try
                {
                    Result.RawProcessorResult = JsonSerializationUtils.Serialize(response, true, true);
                }
                catch (Exception ex)
                {
                    Result.RawProcessorResult = "Failed: "  +  ex.Message;
                }
               
                if (response.messages.resultCode == messageTypeEnum.Error)
                {
                    Result.IsSuccess = false;
                    Result.ValidatedResult = ccProcessResults.Failed;
                    Result.Message = response.messages?.message?.FirstOrDefault()?.text;
                    return Result;
                }

                var transaction = response.transactionResponse;
                var responseCode = transaction.responseCode;                

                if (response.messages.resultCode == messageTypeEnum.Ok)
                {              
                    Result.TransactionId = transaction.transId;
                    Result.AuthorizationCode = transaction.authCode;

                    if (Order.ProcessType == ccProcessTypes.PreAuth)
                        Result.ValidatedResult = ccProcessResults.Authorized;
                    else
                        Result.ValidatedResult = ccProcessResults.Approved;
                                        
                    Result.IsSuccess = true;
                    Result.CCLastFour = transaction.accountNumber;
                    Result.AvsResultCode = transaction.avsResultCode ;
                    Result.CvvResultCode = transaction.cvvResultCode;

                    var msg = transaction.messages?.FirstOrDefault()?.description;
                    Result.ExtendedMessage = msg + "\r\n" +                                             
                                             "Code: " + transaction.responseCode + "\r\n" +                                             
                                             " AVS: " + Result.AvsResultCode + "\r\n" +
                                             " CVV: " + Result.CvvResultCode;
                }
                else
                {
                    // declined
                    if (responseCode == "2" || responseCode == "4")
                    {
                        Result.ValidatedResult = ccProcessResults.Declined;
                        Result.CCLastFour = transaction.accountNumber;
                        Result.AvsResultCode = transaction.avsResultCode;
                        Result.CvvResultCode = transaction.cvvResultCode;
                        Result.Message = transaction.errors[0].errorText;
                        if (string.IsNullOrEmpty(Result.Message))
                            Result.Message = Result.RawProcessorResult;
                        SetError(Result.Message);
                    }
                    else
                    {
                        if (transaction?.errors != null &&
                            transaction.errors.Length > 0)
                            Result.Message = transaction.errors[0].errorText;
                        else if (response.messages != null && response.messages.message.Length > 0)
                            Result.Message = response.messages.message[0].text;
                        else
                            Result.Message = "Unknown error";

                        Result.IsSuccess = false;
                        Result.ValidatedResult = ccProcessResults.Failed;
                        Result.AvsResultCode = transaction.avsResultCode;
                        Result.CvvResultCode = transaction.cvvResultCode;

                        Result.ExtendedMessage = Result.Message + "\n" +
                                                  "Code: " + transaction.responseCode + "\r\n" +
                                                  " AVS: " + Result.AvsResultCode + "\r\n" +
                                                  " CVV: " + Result.CvvResultCode;
                    }
                }
            }
            else
            {
                Result.ValidatedResult = ccProcessResults.Failed;
                Result.Message = "Processor request failed.";
                SetError(Result.Message);             
            }

            try
            {
                Result.RawProcessorRequest = JsonSerializationUtils.Serialize(request, true, true);
            }
            catch (Exception ex)
            {
                Result.RawProcessorRequest = ex.Message;
            }

            LogTransaction();

            return Result;            
        } 
    }
}

#endif
