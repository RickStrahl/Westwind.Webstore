using System;
using System.Net;
using Westwind.Webstore.Business;
using Westwind.Webstore.Web.Models;

namespace Westwind.Webstore.Web.Controllers
{
    public class ErrorViewModel : WebStoreBaseViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public Exception Error { get; set; }

        public int StatusCode { get; set; } = 500;
        public string HttpVerb { get; set; }

        public string PostData { get; set;  }

        public string Path { get; set; }
    }
}
