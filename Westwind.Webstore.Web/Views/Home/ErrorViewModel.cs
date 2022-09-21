﻿using System;
using System.Net;
using Westwind.Webstore.Web.Models;

namespace Westwind.Webstore.Web.Controllers
{
    public class ErrorViewModel : WebStoreBaseViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public Exception Error { get; set; }

        public int StatusCode { get; set; } = 500;
    }
}
