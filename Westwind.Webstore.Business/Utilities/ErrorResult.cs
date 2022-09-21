using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Westwind.Webstore.Business.Utilities
{
    /// <summary>
    /// A generic structure that returns error information
    /// </summary>
    public class ErrorResult
    {
        public ErrorResult()
        {
                
        }

        public ErrorResult(string message, int errorCode = 0)
        {
            if (!string.IsNullOrEmpty(message))
            {
                IsError = true;
                Message = message;
            }
            ErrorCode = errorCode;
        }

        public bool IsError { get; set; }

        public string Message { get; set; }

        public int ErrorCode { get; set; }

        public object ExtraData { get; set;  }
        
        public override string ToString()
        {
            return (IsError ? "Error: " : "No Errors ") + Message;
        }
    }
}
