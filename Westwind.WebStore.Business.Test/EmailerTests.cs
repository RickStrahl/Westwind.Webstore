using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Westwind.Utilities;
using Westwind.Webstore.Business.Utilities;
using Westwind.Webstore.Web.Utilities;

namespace Westwind.Webstore.Business.Test
{
    [TestFixture]
    public class EmailerTests
    {
        [Test]
        public void EmailTest()
        {
            var emailer = new MailgunHttpEmailer();

            var result = emailer.SendEmail("Rick Strahl <rstrahl@west-wind.com>", "Test Message", "Test Content");

            Assert.IsTrue(result, emailer.ErrorMessage);
        }

        [Test]
        public void Email2Test()
        {
            var emailer = new MailgunHttpEmailer();

            var result = emailer.SendEmail("Rick Strahl <rstrahl@west-wind.com>", "Test Message", "Test Content with <b>html</b> embedded.", EmailModes.html);

            Assert.IsTrue(result, emailer.ErrorMessage);
        }


        [Test]
        public void AdminEmailTest()
        {
            var emailer = new MailgunHttpEmailer();

            var result = emailer.SendAdminEmail("Rick Strahl <rstrahl@west-wind.com>", "Test Message", "Test Content with <b>html</b> embedded.", EmailModes.html);

            Assert.IsTrue(result, emailer.ErrorMessage);
        }

        [Test]
        public void SendMailGunApiEmail()
        {
            var emailer = new MailgunHttpEmailer();
                // domain: wsApp.Configuration.Email.MailgunDomain,
                // apiKey: wsApp.Configuration.Email.MailgunApiKey,
                // from: "West Wind Technologies <admin@west-wind.com>");

            var res = emailer.SendEmail("rstrahl@west-wind.com",
                "Test Email " + DateTime.Now.ToString("t"),
                "Hello <b>cruel</b> world!",
                EmailModes.html);

            Assert.IsTrue(res);
            Console.WriteLine(emailer.ErrorMessage);

        }


    }
}
