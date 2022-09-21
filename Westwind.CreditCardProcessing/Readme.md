# West Wind Credit Card Processing Helpers
##### Credit Card Processing Abstractions for a number of common Credit Card Providers

This library is a small wrapper around various Credit Card processing providers
that makes it easy to integrate with multiple providers more seamlessly using
the same front end class interface. This makes it easy to swap any of the 
supported providers.

This library supports only the actual sales process by handling Sales, PreAuth, 
AuthCapture and Credit Operations. Additional features such as Subscriptions,
or Sales Reporting is not provided and for those you can need to fall
back on the base providers. However, this library should provide a good
starting point to extend functionality for additional provider features.


Supported Gateways:
* BrainTree
* Authorize.NET
* NaviGate (MerchantPlus)

Requirements:
* .NET 4.5+

# Under Construction

## Resources


## License
The Westwind.CreditCardProcessing library is licensed under the
[MIT License](http://opensource.org/licenses/MIT) and there's no charge to use, 
integrate or modify the code for this project. You are free to use it in personal, 
commercial, government and any other type of application. 

[Commercial Licenses](http://west-wind.com/Westwind.Globalization/docs/?page=_2lp0u0i9b.htm) 
are also available as an option. If you are using these tools in a commercial application
that provides revenue, please consider purchasing one of our reasonably priced commercial
licenses that help support this project's development.

All source code is copyright West Wind Technologies, regardless of changes made to them. 
Any source code modifications must leave the original copyright code headers intact.


### Warranty Disclaimer: No Warranty!
IN NO EVENT SHALL THE AUTHOR, OR ANY OTHER PARTY WHO MAY MODIFY AND/OR REDISTRIBUTE 
THIS PROGRAM AND DOCUMENTATION, BE LIABLE FOR ANY COMMERCIAL, SPECIAL, INCIDENTAL, 
OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM 
INCLUDING, BUT NOT LIMITED TO, LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR 
LOSSES SUSTAINED BY YOU OR LOSSES SUSTAINED BY THIRD PARTIES OR A FAILURE OF THE 
PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS, EVEN IF YOU OR OTHER PARTIES HAVE 
BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.