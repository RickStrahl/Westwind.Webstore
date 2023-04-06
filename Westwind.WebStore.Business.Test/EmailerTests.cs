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
            var emailer = new Emailer();

            var result = emailer.SendEmail("Rick Strahl <rstrahl@west-wind.com>", "Test Message", "Test Content");

            Assert.IsTrue(result, emailer.ErrorMessage);
        }

        [Test]
        public void Email2Test()
        {
            var emailer = new Emailer();

            var result = emailer.SendEmail("Rick Strahl <rstrahl@west-wind.com>", "Test Message", "Test Content with <b>html</b> embedded.", EmailModes.html);

            Assert.IsTrue(result, emailer.ErrorMessage);
        }


        [Test]
        public void AdminEmailTest()
        {
            var emailer = new Emailer();

            var result = emailer.SendAdminEmail("Rick Strahl <rstrahl@west-wind.com>", "Test Message", "Test Content with <b>html</b> embedded.", EmailModes.html);

            Assert.IsTrue(result, emailer.ErrorMessage);
        }
    }
}
