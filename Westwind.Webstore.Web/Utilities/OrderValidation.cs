using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Westwind.Utilities;
using Westwind.Utilities.InternetTools;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Utilities;

namespace Westwind.Webstore.Web.Utilities
{
    public class OrderValidation
    {

        #region PageTimeout Check

        static string Key = "WEBSTORE_991.13";

        private DateTime OriginalDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Encodes current date into an encrypted string that can be embedded into a request.
        /// You can then read this back on a POST to see if the date is still current
        /// and check for minimum and maximum time to process.
        /// </summary>
        /// <returns></returns>
        public static string EncodeCurrentDate()
        {
            return EncodeCurrentDate(DateTime.UtcNow);
        }


        /// <summary>
        /// Encodes current date into an encrypted string that can be embedded into a request.
        /// You can then read this back on a POST to see if the date is still current
        /// and check for minimum and maximum time to process.
        /// </summary>
        /// <returns></returns>
        public static string EncodeCurrentDate(DateTime time)
        {
            var dt = time.ToString("O");
            var encoded = Encryption.EncryptString(dt, Key, useBinHex: true);
            return encoded;
        }
        


        /// <summary> 
        /// checks to see if the passed date was done in more than than the timeoutSeconds timeframe.
        /// Fails if less time has passed.
        /// </summary>
        public static ErrorResult IsTimeEncodingValid(string encodedDate, int timeOutSeconds = 10, int timeoutTooLongMinutes = 10)
        {
            if (string.IsNullOrEmpty(encodedDate) || encodedDate.Length < 10)
                return new ErrorResult("Timeout validation failed: Invalid encoding format.");

            var now = DateTime.UtcNow;

            if (!DateTime.TryParse(Encryption.DecryptString(encodedDate, Key, useBinHex: true), out DateTime d))
            {
                return new ErrorResult("Timeout validation failed: Invalid validation code.");
            }
            d = d.ToUniversalTime();

            if (d > now.AddSeconds(timeOutSeconds * -1))
            {
                var secs = now.Subtract(d).TotalSeconds;
                return new ErrorResult( $"Time validation time out: Form filled in {secs:n0} seconds - faster than the  minimum {timeOutSeconds} seconds required.");
            }

            if (d < now.AddMinutes(timeoutTooLongMinutes * -1))
            {
                var mins = now.Subtract(d).TotalMinutes;
                return new ErrorResult($"Time validation time out: Form filled too slowly in {mins:n0} minutes - more than the max of {timeoutTooLongMinutes} minutes.");
            }

            return new ErrorResult();
        }
        #endregion

        #region Google ReCapture Verification

        /// <summary>
        /// Verifies a Recapture code captured on the Web Form 
        /// </summary>
        /// <param name="validationKey"></param>
        /// <param name="IpAddress"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static bool VerifyRecaptcha(string validationKey, string IpAddress = null, string secretKey = null)
        {
            if (string.IsNullOrEmpty(validationKey))
                return false;

            if (string.IsNullOrEmpty(secretKey))
                secretKey = wsApp.Configuration.Security.GoogleRecaptureSecret;
            
            var http = new HttpClient();

            http.AddPostKey("secret", secretKey);
            http.AddPostKey("response", validationKey);
            if (!string.IsNullOrEmpty(IpAddress))
                http.AddPostKey("remoteIp", IpAddress);
            http.HttpVerb = "POST";

            var json = http.DownloadString("https://www.google.com/recaptcha/api/siteverify");

            if (json.Contains("\"success\": true"))
                return true;
            
            return false;
        }

        #endregion
    }
}
