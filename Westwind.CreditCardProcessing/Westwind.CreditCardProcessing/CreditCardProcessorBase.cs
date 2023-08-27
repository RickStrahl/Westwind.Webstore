using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using Westwind.Utilities.InternetTools;

namespace Westwind.CreditCardProcessing

{
    /// <summary>
    /// The ccProcessing class provides a base class for provider processing of
    /// requests. The class aims at providing a simple, common interface to Credit
    /// Card Processing so it's easy to swap providers.
    ///
    /// This class acts as an abstract class that requires subclassing and use of a
    /// specific provider implementation class. However, because the functionality is
    /// so common amongst providers a single codebase can usually accomodate all requests.
    /// </summary>
    public abstract class CreditCardProcessorBase
    {
        protected CreditCardProcessorBase()
        {
            Result = new CreditCardProcessingResult();
            Merchant = new CreditCardMerchantInfo();
            BillingInfo = new CreditCardBillingInfo();
            Order = new CreditCardOrderInfo();
            Configuration = new CreditCardProcessingConfiguration();

            Order.ProcessType = ccProcessTypes.Sale;
            Configuration.UseLocalMod10Check = true;
            Configuration.Timeout = 50;
        }


        ///// <summary>
        ///// Transaction Id used for AuthCapture
        ///// </summary>
        //public string TransactionId { get; set;  }

        /// <summary>
        /// The result of processing for this request
        /// </summary>
        public CreditCardProcessingResult Result { get; set; }

        public CreditCardMerchantInfo Merchant { get; set; }

        public CreditCardBillingInfo BillingInfo { get; set;  }

        public CreditCardOrderInfo Order { get; set; }


        public CreditCardProcessingConfiguration Configuration { get; set;  }

        /// <summary>
        /// Error flag set after a call to ValidateCard if an error occurs.
        /// </summary>
        public bool Error { get; set; }

        /// <summary>
        /// Error message if error flag is set or negative result is returned.
        /// Generally this value will contain the value of ValidatedResult
        /// for processor failures or more general API/HTTP failure messages.
        /// </summary>
        public string ErrorMessage { get; set; }


        /// <summary>
        /// Factory method that creates an instance of the appropriate processor. Use
        /// this method to create the appropriate provider.
        ///
        /// &lt;&lt;code lang=&quot;C#&quot;&gt;&gt;
        /// ccProcessing CC = ccProcessing.CreateProcessor(CCType);
        /// if (CC == null)
        /// {
        ///     SetError(&quot;Invalid Credit Card Processor or Processor not
        /// supported&quot;);
        ///     return false;
        /// }
        ///
        /// CC.MerchantId = App.PaymentConfiguration.CCMerchantId;
        /// CC.MerchantPassword = App.PaymentConfiguration.CCMerchantPassword;
        ///
        /// // ...  set other properties
        ///
        /// bool Result = CC.ValidateCard();
        ///
        /// // *** deal with the results
        /// Inv.Ccresult = CC.ValidatedResult;
        /// if (!Result)
        /// {
        ///     ErrorMessage = CC.Message;
        ///     Inv.Ccerror = ErrorMessage;
        /// }
        ///
        /// // *** Always write out the raw response
        /// if (string.IsNullOrEmpty(CC.RawProcessorResult))
        ///     Inv.Ccresultx = CC.Message;
        /// else
        ///     Inv.Ccresultx = CC.RawProcessorResult;
        /// &lt;&lt;/code&gt;&gt;
        /// <seealso>Class ccProcessing</seealso>
        /// </summary>
        /// <param name="ccType">
        /// One of the supported Credit Card Processor Types
        /// </param>
        public static CreditCardProcessorBase CreateProcessor(ccProcessors ccType)
        {
            CreditCardProcessorBase cc = null;

            if (ccType == ccProcessors.Braintree)
            {
                cc = new BraintreeCreditCardProcessor();
            }
            //if (CCType == ccProcessors.AccessPoint)
            //{
            //    CC = new ccAccessPoint();
            //}
            //else if (CCType == ccProcessors.AuthorizeNetClassic)
            //{
            //    cc = new AuthorizeNetClassicCreditCardProcessor();
            //}
            //else if (CCType == ccProcessors.AuthorizeNet)
            //{
            //    cc = new AuthorizeNetCreditCardProcessor();
            //}
            //else if (CCType == ccProcessors.Navigate)
            //{
            //    cc = new NaviGateCreditCardProcessor();
            //}
            //else if (CCType == ccProcessors.PayFlowPro)
            //{
            //    CC = new ccPayFlowProHttp();
            //}
            //else if (CCType == ccProcessors.LinkPoint)
            //{
            //    CC = new ccLinkPoint();
            //}
            return cc;
        }


        /// <summary>
        /// Base ValidateCard method that provides the core CreditCard checking. Should
        /// always be called at the beginning of the subclassed overridden method.
        /// </summary>
        /// <param name="preAuthCodeOrTransactionId">PreAuth code if you are doing an AuthCapture</param>
        /// <returns>bool</returns>
        public virtual CreditCardProcessingResult Process(string preAuthCodeOrTransactionId = null)
        {
            if (!string.IsNullOrEmpty(Order.CardNumber) &&
                Configuration.UseLocalMod10Check && !Mod10Check(Order.CardNumber))
            {
                ErrorMessage = "Invalid Credit Card Number";
                Result.Message = ErrorMessage;
                Result.ValidatedResult = ccProcessResults.Declined;
                LogTransaction();
                return Result;
            }

            Result.IsSuccess = true;
            return Result;
        }


        /// <summary>
        /// Logs the information of the current request into the log file specified.
        /// If the log file is empty no logging occurs.
        /// </summary>
        protected virtual void LogTransaction()
        {
            if (!string.IsNullOrEmpty(Configuration.LogFile))
            {
                byte[] binLogString = Encoding.Default.GetBytes(DateTime.Now.ToString() + " - " +
                                                                BillingInfo.Name + " - " +
                                                                GetHiddenCc(Order.CardNumber ?? Result.CCLastFour, true) + " - " +
                                                                Result.ValidatedResult + " - " +
                                                                Result.Message + " - " +
                                                                Order.OrderAmount.ToString(CultureInfo.InstalledUICulture.NumberFormat) + "\r\n");

                lock (this)
                {
                    try
                    {
                        FileStream loFile = new FileStream(Configuration.LogFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
                        loFile.Seek(0, SeekOrigin.End);
                        loFile.Write(binLogString, 0, binLogString.Length);
                        loFile.Close();
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Strips spaces and other characters out of the credit card
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        public string CleanupCreditCard(string cardNumber = null)
        {
            if (cardNumber == null)
                cardNumber = Order?.CardNumber;

            if (string.IsNullOrEmpty(cardNumber))
                return string.Empty;

            return Regex.Replace(Order.CardNumber.Trim(), @"[ -/._#]", "");

        }


        /// <summary>
        /// Returns a string for a single AVS code value
        /// Supported codes:
        /// ANSUWXYZER_
        /// </summary>
        /// <param name="avsCode"></param>
        /// <returns></returns>
        public string AvsCodeToString(string avsCode)
        {
            if (avsCode == "A")
                return "Street Address Matched, PostalCode not Matched";
            if (avsCode == "N")
                return "No AVS Match";
            if (avsCode == "U")
                return "AVS not supported for this card type";
            if (avsCode == "W")
                return "PostalCode 9 matched, no street match";
            if (avsCode == "X")
                return "PostalCode 9 matched, street matched";
            if (avsCode == "Y")
                return "PostalCode 5 matched, street matched";
            if (avsCode == "Z")
                return "PostalCode 5 matched, street not matched";
            if (avsCode == "E")
                return "Not eligible for AVS";
            if (avsCode == "R")
                return "System unavailable";
            if (avsCode == "_")
                return "AVS supported on this network or transaction type";

            return "";
        }

        /// <summary>
        /// Determines whether given string passes standard Mod10 check.
        /// </summary>
        /// <param name="validString">String to be validated</param>
        /// <returns>True if valid, otherwise false</returns>
        public static bool Mod10Check(string StringToValidate)
        {
            if (string.IsNullOrEmpty(StringToValidate))
                return false;

            string trimString = StringToValidate.Trim();
            char lastChar = trimString[trimString.Length - 1];

            char checkSumChar = Mod10Worker(trimString.Substring(0, trimString.Length - 1), false);
            return (lastChar == checkSumChar);
        }

        /// <summary>
        /// Worker Mod10 check method that figures out the Mod10 checksum value for a string
        /// </summary>
        /// <param name="chkString">String to be validated</param>
        /// <param name="startLeft">Specifies if check starts from left</param>
        /// <returns>Checksum character</returns>
        private static char Mod10Worker(string chkString, bool startLeft)
        {
            // Remove any non-alphanumeric characters (The list can be expanded by adding characters
            // to RegEx.Replace pattern parameter's character set)
            string chkValid = chkString;
            chkValid = Regex.Replace(chkValid, @"[\s-\.]*", "").ToUpper();

            // Calculate the MOD 10 check digit
            int mod10 = 0;
            int digit;
            char curChar;

            for (int pos = 0; pos <= chkValid.Length - 1; pos++)
            {
                // Take each char starting from the right unless startLeft is true
                if (startLeft)
                {
                    digit = pos;
                }
                else
                {
                    digit = chkValid.Length - pos - 1;
                }

                curChar = chkValid[digit];

                // If the character is a digit, take its numeric value.
                if (Char.IsDigit(curChar))
                {
                    digit = (int)Char.GetNumericValue(curChar);
                }
                else
                {
                    // Otherwise, take the base16 value of the letter.
                    // NOTE: The .... is a place holder to force the / to equal 15.
                    // The USPS does not assign a value for a period. Periods were removed from
                    // string at beginning of the function
                    string base16String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ..../";
                    int foundPos = base16String.IndexOf(curChar) + 1;
                    digit = (foundPos >= 0 ? foundPos % 16 : 0);
                }

                // Multiply by 2 if the position is odd
                digit = digit * ((pos + 1) % 2 == 0 ? 1 : 2);

                // If the result is larger than 10, the two digits have to be added. This can be done
                // by adding the results of the integer division by 10 and then mod by 10.
                digit = (digit / 10) + (digit % 10);

                // If the result is 10 (which occurs when N is an odd position), add the two digits together again
                // (this becomes 1).
                digit = (digit == 10 ? 1 : digit);

                // Sum all the digits
                mod10 = mod10 + digit;
            }

            // Subtract the MOD 10 from 10
            mod10 = 10 - (mod10 % 10);

            // If the result is 10, then the check digit is 0.
            // Else, it is the result. Return it as a character
            mod10 = (mod10 == 10 ? 0 : mod10);

            return mod10.ToString()[0];
        }

        /// <summary>
        /// Returns a CC number as a partially hidden CC number.
        /// </summary>
        /// <param name="ccNumber">Credit Card Number to display</param>
        /// <returns>String like **** **** **** 1231</returns>
        public string GetHiddenCc(string ccNumber, bool showFirst = false)
        {
            if (string.IsNullOrEmpty(ccNumber))
                return ccNumber;

            ccNumber = ccNumber.Trim();

            // obfuscate the whole thing
            if (ccNumber.Length < 10)
                return "**** **** **** ****";

            string last = ccNumber.Substring(ccNumber.Length - 4, 4);
            string first = ccNumber.Substring(0, 4);

            if (showFirst)
                return first + " **** **** " + last;

            return "**** **** **** " + last;
        }

        /// <summary>
        /// Error setting method that sets the Error flag and message.
        /// </summary>
        /// <param name="lcErrorMessage"></param>
        protected void SetError(string errorMessage)
        {
            if (errorMessage == null || errorMessage.Length == 0)
            {
                errorMessage = "";
                Error = false;
                return;
            }

            ErrorMessage = errorMessage;
            Error = true;
        }
    }

    /// <summary>
    /// The ccProcessing class provides a base class for provider processing of
    /// requests. The class aims at providing a simple, common interface to Credit
    /// Card Processing so it's easy to swap providers.
    ///
    /// This class acts as an abstract class that requires subclassing and use of a
    /// specific provider implementation class. However, because the functionality is
    /// so common amongst providers a single codebase can usually accomodate all requests.
    /// </summary>
    [DebuggerDisplay("{ValidatedResult} - {Message}")]
    public class CreditCardProcessingResult
    {
        public CreditCardProcessingResult()
        {
            ValidatedResult = ccProcessResults.None;
        }


        /// <summary>
        /// High level property that lets you quickly see if the
        /// request is a success or not.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The parsed short form response. APPROVED, DECLINED, FAILED, FRAUD
        /// </summary>
        public ccProcessResults ValidatedResult { get; set; }

        /// <summary>
        /// Holds raw instance to the processors result object
        /// where available.
        /// </summary>
        public object ProcessorResultObject { get; set; }

        /// <summary>
        /// The raw response from the Credit Card Processor Server.
        ///
        /// You can use this result to manually parse out error codes
        /// and messages beyond the default parsing done by this class.
        /// </summary>
        public string RawProcessorResult { get; set; }

        /// <summary>
        /// This is a string that contains the format that's sent to the
        /// processor. Not used with all providers, but this property can
        /// be used for debugging and seeing what exactly gets sent to the
        /// server.
        /// </summary>
        public string RawProcessorRequest { get; set; }


        /// <summary>
        /// The parsed error message from the server if result is not APPROVED
        /// This message generally is a string regarding the failure like 'Invalid Card'
        /// 'AVS Error' etc. This info may or may not be appropriate for your customers
        /// to see - that's up to you.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// More detailed message from the processor on failure
        /// </summary>
        public string ExtendedMessage { get; set; }

        /// <summary>
        /// The Transaction ID returned from the server. Use to match
        /// transactions against the gateway for reporting.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Last 4 digists of Credit Card for storage
        /// </summary>
        public string CCLastFour { get; set; }

        /// <summary>
        /// Authorization Code returned for Approved transactions from the gateway
        /// </summary>
        public string AuthorizationCode { get; set; }


        /// <summary>
        /// The AVS Result code from the gateway if available
        /// </summary>
        public string AvsResultCode { get; set; }

        /// <summary>
        /// If available a Cvv Result code from the gateway
        /// </summary>
        public string CvvResultCode { get; set; }

        /// <summary>
        /// Subscription Id when setting up a subscription
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string CreditCardToken { get; set; }

        public string CustomerId { get; set; }


        public decimal ProcessedAmount { get; set;  }

    }


    public class CreditCardMerchantInfo
    {
        /// <summary>
        /// The merchant Id or store name or other mechanism used to identify your account.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// The merchant password for your merchant account. Not always used.
        /// </summary>
        public string MerchantPassword { get; set; }

        /// <summary>
        /// Public key for the merchant account. Not always used
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Private key for the merchant account. Not always used
        /// </summary>
        public string PrivateKey { get; set; }



        /// <summary>
        /// The link to hit on the server. Depending on the interface this can be a
        /// URL or domainname or domainname:Port combination.
        ///
        /// Applies only to providers that use HTTP POST operations directly.
        /// </summary>
        public string HttpLink { get; set; }


        /// <summary>
        /// Optional Url that determines a test server url
        /// </summary>
        public string TestLink { get; set; }
    }

    public class CreditCardProcessingConfiguration
    {
        /// <summary>
        /// Timeout in seconds for the connection against the remote processor
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Optional flag that determines whether to send a test transaction
        /// Not supported for AccessPoint
        /// </summary>
        public bool UseTestTransaction { get; set; }


        /// <summary>
        /// If available for the processor use client token processing
        /// for the credit card. Authorize.NET, BrainTree
        /// </summary>
        public bool UseClientTokenProcessing { get; set; }

        /// <summary>
        /// Determines whether a Mod10Check is performed before sending the
        /// credit card to the processor. Turn this off for testing so you
        /// can at least get to the provider.
        /// </summary>
        public bool UseLocalMod10Check { get; set; }

        /// <summary>
        /// Referring Url used with certain providers
        /// </summary>
        public string ReferringUrl { get; set; }


        /// <summary>
        /// Optional path to the log file used to write out request results.
        /// If this filename is blank no logging occurs. The filename specified
        /// here needs to be a fully qualified operating system path and the
        /// application has to be able to write to this path.
        /// </summary>
        public string LogFile
        {
            get { return _LogFile; }
            set { _LogFile = value; }
        }
        private string _LogFile = "";
    }

    public class CreditCardBillingInfo
    {
        /// <summary>
        /// First name and last name on the card
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Name))
                    return (FirstName + " " + LastName).Trim();

                return _Name;
            }
            set { _Name = value; }
        }
        string _Name = "";

        /// <summary>
        /// First Name of customer's name on the card. Can be used in lieu of Name
        /// property. If FirstName and LastName are used they get combined into a name
        /// IF Name is blank.
        /// <seealso>Class ccProcessing</seealso>
        /// </summary>
        public string FirstName { get; set; }


        /// <summary>
        /// Last Name of customer's name on the card. Can be used in lieu of Name
        /// property. If FirstName and LastName are used they get combined into a name
        /// IF Name is blank.
        ///
        /// AuthorizeNet requires both first and last names, while the other providers
        /// only use a single Name property.
        /// <seealso>Class ccProcessing</seealso>
        /// </summary>
        public string LastName { get; set; }


        /// <summary>
        /// Billing Company
        /// </summary>
        public string Company { get; set; }


        /// <summary>
        /// Billing Street Address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Billing City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Billing State (2 letter code or empty for foreign)
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Postal or PostalCode code
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Two letter CountryCode Id -  US, DE, CH etc.
        /// </summary>
        public string CountryCode { get; set; }

        public string Country { get; set; }

        /// <summary>
        /// Billing Phone Number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; }

        // Client IP Address
        public string IpAddress { get; set; }

    }

    public class CreditCardOrderInfo
    {
        /// <summary>
        /// The credit card number. Number can contain spaces and other markup
        /// characters which are stripped for processing later.
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// The 3 or 4 letter digit that is on the back of the card
        /// </summary>
        public string SecurityCode { get; set; }

        /// <summary>
        /// Full expiration date in the format 01/2003
        /// </summary>
        public string CardExpiration
        {
            get { return _cardExpiration; }
            set
            {
                string Exp = value?.Trim();
                if (string.IsNullOrEmpty(Exp))
                {
                    ExpirationMonth = "";
                    ExpirationYear = "";
                    _cardExpiration = "";
                    return;
                }

                string[] Split = Exp.Split("/-.\\,".ToCharArray(), 2);
                ExpirationMonth = Split[0].PadLeft(2, '0');
                ExpirationYear = Split[1];

                // turn into 2 digit year
                if (ExpirationYear.Length == 4)
                    ExpirationYear = ExpirationYear.Substring(2, 2);
                _cardExpiration = Exp;
            }
        }
        string _cardExpiration = "";

        /// <summary>
        /// Credit Card Expiration Month as a string (2 digits ie. 08)
        /// </summary>
        public string ExpirationMonth { get; set; }


        /// <summary>
        /// Credit Card Expiration Year as a 2 digit string
        /// </summary>
        public string ExpirationYear { get; set; }

        public string ClientNonce { get; set;  }

        public string ClientDescriptor { get; set;  }

        /// <summary>
        /// Determines what type of transaction is being processed (Sale, Credit, PreAuth)
        /// </summary>
        public ccProcessTypes ProcessType { get; set; }

        /// <summary>
        /// Transaction Id or AuthId of a previously authorized order
        /// </summary>
        public string TransactionId { get; set;  }

        /// <summary>
        /// The amount of the order.
        /// </summary>
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// The currency used - not used by all processors
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// The amount of Tax for this transaction
        /// </summary>
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// The Order Id as a string. This is mainly for reference but should be unique.
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Order Comment. Usually this comment shows up on the CC bill.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// IP Address associated with this order
        /// </summary>
        public string IpAddress { get; set; }
    }


    /// <summary>
    /// Credit Card Processors available
    /// </summary>
    public enum ccProcessors
    {
        AuthorizeNet,
        Braintree,
        PayFlowPro,
        AuthorizeNetClassic,
        AccessPoint,
        LinkPoint,
        Navigate,
        BluePay
    }

    /// <summary>
    /// Types of Credit Card Transactions that
    /// can be used against processors.
    /// </summary>
    public enum ccProcessTypes
    {
        Sale,
        Credit,
        PreAuth,
        AuthCapture,
        Void
    }

    public enum ccProcessResults
    {
        None,
        Approved,
        Authorized,
        Declined,
        Failed,
        Fraud
    }

}
