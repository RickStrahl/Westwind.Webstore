using System;
using Westwind.Licensing;
using Westwind.Utilities;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;
using Product = Westwind.Licensing.Product;

namespace Westwind.Webstore.Business
{
    public class InvoiceLicensing
    {
        private InvoiceBusiness Invoice { get; }


        public InvoiceLicensing(InvoiceBusiness invoice)
        {
            Invoice = invoice;
        }


        /// <summary>
        /// Retrieves server based license information that is on file.
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public LicenseInfo GetLicense(string serialNumber)
        {
            var config = wsApp.Configuration.Licensing;
            var client = new LicenseAdminServiceClient(config.ServerUrl);

            string token = client.Authenticate(config.Username, config.Password);
            if (token == null)
            {
                SetError("Authentication with the license server failed.");
                return null;
            }

            var regLicense = client.GetLicense(serialNumber);

            if (regLicense == null)
            {
                SetError(client.ErrorMessage);
                return null;
            }

            return regLicense;
        }


        /// <summary>
        /// Attempts to generate a license for the lineitem by making a call
        /// to the license server.
        /// </summary>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        public bool CreateLicenseForLineItem(LineItem lineItem, Westwind.Webstore.Business.Entities.Product item = null)
        {
            if (item == null)
            {
                var itemBus = BusinessFactory.Current.GetProductBusiness();
                item = itemBus.LoadBySku(lineItem.Sku);
                if (item == null)
                {
                    SetError("Invalid product sku.");
                    return false;
                }
            }

            var config = wsApp.Configuration.Licensing;
            var client = new LicenseAdminServiceClient(config.ServerUrl);

            string token = client.Authenticate(config.Username, config.Password);
            if (token == null)
            {
                SetError("Authentication with the license server failed.");
                return false;
            }

            var customer = Invoice.Entity.Customer;
            var invoice = Invoice.Entity;

            var license = new LicenseInfo()
            {
                Email = customer.Email,
                Name = (customer.Firstname + " " + customer.Lastname).Trim(),
                Company = customer.Company,
                ProductSku = lineItem.Sku,
                ProductName = lineItem.Description,
                ReferenceId = Invoice.Entity?.InvoiceNumber,
                ReferenceId2 = Invoice.Entity?.Id.ToString(),
                Version = item?.Version,
                MaxActivations = item?.LicenseCount ?? 3
            };

            // Make the HTTP call
            var regLicense = client.CreateLicense(license);

            if (regLicense == null)
            {
                SetError(client.ErrorMessage);
                return false;
            }

            lineItem.UseLicensing = true;
            lineItem.LicenseSerial = regLicense.SerialNumber;

            return true;
        }


        /// <summary>
        /// Reissues a license which will give a new Serial number. License
        /// is reissued by changing the registration date by a minute.
        ///
        /// Updates the passed in LineItem - make sure to save it after update.
        ///
        /// On failure, old license info is left intact. Only on success is the
        /// license info updated.
        /// </summary>
        /// <param name="lineItem">Lineitem to apply the license to. This line item is updated with the new license info.</param>
        /// <returns></returns>
        public bool ReissueLicense(LineItem lineItem)
        {
            if (lineItem == null)
            {
                SetError("Line item must be passed to reissues.");
                return false;
            }

            if (string.IsNullOrEmpty(lineItem.LicenseSerial))
            {
                SetError("Lineitem must have an existing serial number in order to reissue license.");
                return false;
            }

            var config = wsApp.Configuration.Licensing;
            var client = new LicenseAdminServiceClient(config.ServerUrl);
            string token = client.Authenticate(config.Username, config.Password);
            if (token == null)
            {
                SetError("Authentication with the license server failed.");
                return false;
            }

            var customer = Invoice.Entity.Customer;
            var invoice = Invoice.Entity;

            var license = new LicenseInfo()
            {
                Email = customer.Email,
                Name = (customer.Firstname + " " + customer.Lastname).Trim(),
                Company = customer.Company,
                ProductSku = lineItem.Sku,
                ProductName = lineItem.Description,
                SerialNumber = lineItem.LicenseSerial,
                RegistrationDate = DateTime.MinValue
            };

            // Make the HTTP call
            var regLicense = client.ReissueLicense(license);

            if (regLicense == null)
            {
                SetError(client.ErrorMessage);
                return false;
            }

            lineItem.UseLicensing = true;
            lineItem.LicenseSerial = regLicense.SerialNumber;

            return true;
        }

        /// <summary>
        /// Revokes or restores a license by marking the revocation status flag which
        /// if set to true doesn't allow the license to be validated.
        ///
        /// Note does not update the license in the DB!
        /// </summary>
        /// <param name="lineItem">line item to which to apply the license info</param>
        /// <param name="restoreLicense">if true restores a previously revoked license</param>
        /// <returns></returns>
        public bool RevokeLicense(LineItem lineItem, bool restoreLicense = false)
        {
            var config = wsApp.Configuration.Licensing;
            var client = new LicenseAdminServiceClient(config.ServerUrl);
            string token = client.Authenticate(config.Username, config.Password);
            if (token == null)
            {
                SetError("Authentication with the license server failed.");
                return false;
            }

            var customer = Invoice.Entity.Customer;
            var invoice = Invoice.Entity;

            var license = new LicenseRevokeRequest()
            {
                SerialNumber = lineItem.LicenseSerial,
                Email = customer.Email,
                ProductSku = lineItem.Sku,
                RestoreLicense = restoreLicense
            };

            // Make the HTTP call
            var result = client.RevokeLicense(license);

            if (!result)
            {
                SetError(client.ErrorMessage);
                return false;
            }

            return true;
        }


        public string ErrorMessage { get; set; }

        protected void SetError()
        {
            SetError("CLEAR");
        }

        protected void SetError(string message)
        {
            if (message == null || message == "CLEAR")
            {
                ErrorMessage = string.Empty;
                return;
            }
            ErrorMessage += message;
        }

        protected void SetError(Exception ex, bool checkInner = false)
        {
            if (ex == null)
            {
                ErrorMessage = string.Empty;
            }
            else
            {
                Exception e = ex;
                if (checkInner)
                    e = e.GetBaseException();

                ErrorMessage = e.Message;
            }
        }


    }
}
