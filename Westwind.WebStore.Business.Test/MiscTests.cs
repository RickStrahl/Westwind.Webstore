using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Westwind.Utilities;
using Westwind.Webstore.Web.Utilities;

namespace Westwind.Webstore.Business.Test
{
    [TestFixture]
    public class MiscTests
    {
        [Test]
        public void ExpandoTest()
        {
            var account = new Account();
            dynamic dynAccount = account;

            Console.WriteLine(account.Company);
            Console.WriteLine(dynAccount.Bogus);
        }
    }

    public class Account : LocalizationExpando
    {
        public string Name { get; set; } = "Rick";
        public string Company { get; set; } = "West Wind";
    }


    public class LocalizationExpando : Expando
    {
        /// <summary>
        /// Override the method so if a property is not found it returns the name of the property
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            // first check the Properties collection for member
            if (Properties.Keys.Contains(binder.Name))
            {
                result = Properties[binder.Name];
                return true;
            }

            // Next check for Public properties via Reflection
            if (Instance != null)
            {
                try
                {
                    if (GetProperty(Instance, binder.Name, out result))
                        return true;
                }
                catch { }
            }

            result = binder.Name;
            return true;
        }

        [Test]
        public async Task InvoiceValidation_TimeEncodingTest()
        {

            var dtstr = OrderValidation.EncodeCurrentDate();
            Console.WriteLine(dtstr);


            await Task.Delay(2000);

            var result =  OrderValidation.IsTimeEncodingValid(dtstr,10, 10);
            Console.WriteLine(result);

            dtstr = OrderValidation.EncodeCurrentDate(DateTime.UtcNow.AddMinutes(16 * -1));
            Console.WriteLine(dtstr);

            result = OrderValidation.IsTimeEncodingValid(dtstr, 10, 10);
            Console.WriteLine(result);

        }
    }
}
