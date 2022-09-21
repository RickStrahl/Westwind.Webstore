
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Westwind.Globalization;
using Westwind.Utilities;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Business.Properties;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Westwind.Data.EfCore;

namespace Westwind.Webstore.Business
{
    public class CustomerBusiness : WebStoreBusinessObject<Customer>
    {
        public CustomerBusiness(WebStoreContext context) : base(context)
        {
        }





        /// <summary>
        /// Returns a user by email address
        /// </summary>
        /// <param name="email">Email address</param>
        /// <returns></returns>
        public Customer GetCustomerByEmailAddress(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            return LoadBase(c => c.Email == email);
        }

        /// <summary>
        /// Returns a user by validation key
        /// </summary>
        /// <param name="validationKey">Validation key assigned</param>
        /// <returns></returns>
        public Customer GetCustomerByValidationKey(string validationKey)
        {
            if (string.IsNullOrEmpty(validationKey))
                return null;

            return LoadBase(c => c.ValidationKey == validationKey);
        }

        /// <summary>
        /// Resets a users password based on an email address and validation key
        /// </summary>
        /// <param name="validationId"></param>
        /// <param name="email"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool RecoverPassword(string validationId, string newPassword)
        {
            var user = GetCustomerByValidationKey(validationId);

            if (user == null || user.ValidationKey != validationId)
            {
                SetError("Invalid validation code provided");
                return false;
            }

            user.Password = newPassword;

            if(!ValidatePassword(user.Password))
                return false;

            user.ValidationKey = null;
            user.IsActive = true;

            return Save();
        }


        /// <summary>
        /// Returns the billing address from the address list.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>The billing address or an empty address</returns>
        public static Address GetBillingAddress(Customer customer)
        {
            if (customer == null)
                throw new ArgumentException("Can't get billing address for a missing (null) customer.");

            Address address;

            if (customer.Addresses == null || customer.Addresses.Count == 0)
            {
                address = new Address()
                {
                    CustomerId = customer.Id,
                    AddressName = customer.Fullname,
                    AddressCompany = customer.Company,
                    AddressType = AddressTypes.Billing
                };
                customer.Addresses.Add(address);
                return address;
            }

            if (customer.Addresses.Count == 1)
            {
                address = customer.Addresses[0];
                address.AddressType = AddressTypes.Billing;
            }
            else
                address = customer.Addresses.FirstOrDefault(c => c.AddressType == AddressTypes.Billing);

            if (string.IsNullOrEmpty(address.AddressName))
            {
                address.AddressFullname = customer.Fullname;
            }

            if (address.AddressCompany is null)
            {
                address.AddressCompany = customer.Company;
            }

            return address;
        }

        /// <summary>
        /// Returns the first shipping address from the address list.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Shipping address or null if no shipping address is defined</returns>
        public static Address GetShippingAddress(Customer customer)
        {
            if (customer.Addresses.Count == 0)
                return null;

            var addr = customer.Addresses.FirstOrDefault(c => c.AddressType == AddressTypes.Shipping);
            if (addr == null)
                return null;

            if (string.IsNullOrEmpty(addr.AddressName))
            {
                addr.AddressFullname = customer.Fullname;
            }

            if (addr.AddressCompany is null)
            {
                addr.AddressCompany = customer.Company;
            }

            return addr;
        }

        public List<CustomerListItem> GetCustomerList(string search = null, int maxItems = 1000)
        {
            IQueryable<Customer> invBase = Context.Customers
                .Include(c => c.Addresses);


            if (!string.IsNullOrEmpty(search))
            {
                var lsearch = search.ToLower();
                if (lsearch == "all")
                {
                    // everything
                }
                else
                {
                   invBase = invBase.Where(c => c.Lastname.Contains(search) ||
                                                 c.Company.Contains(search) ||
                                                 c.Email.Contains(search) ||
                                                 c.Id.Contains(search));
                }
            }
            var custList = invBase.ToList();

            return custList.Select(c => new CustomerListItem
            {
                Id= c.Id,
                Name = c.Fullname,
                Company = c.Company,
                Email = c.Email,
                City = GetBillingAddress(c)?.City
            }).ToList();
        }

        public IEnumerable<CustomerListResult> GetCustomerList(CustomerListFilter filter )
        {
            if (filter == null)
                filter = new CustomerListFilter();

            IQueryable<Customer> custList = Context.Customers;

            if (filter.ActiveOnly)
                custList = custList.Where(c => c.IsActive);

            if (!string.IsNullOrEmpty(filter.Lastname))
                custList = custList.Where(c =>
                    c.Lastname.Contains(filter.Lastname, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(filter.Company))
                custList = custList.Where(c =>
                    c.Lastname.Contains(filter.Company, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(filter.Email))
                custList = custList.Where(c =>
                    c.Lastname.Contains(filter.Email, StringComparison.OrdinalIgnoreCase));

            IOrderedQueryable<Customer> cl;
            if (filter.SortOrder == CustomerListSortOrder.Lastname)
                cl = custList.OrderBy(c => c.Lastname);
            if (filter.SortOrder == CustomerListSortOrder.Company)
                cl = custList.OrderBy(c => c.Company);
            else
                cl = custList.OrderByDescending(c => c.Entered);

            var cl2 = cl.Skip(filter.MaxItems * (filter.ItemPage - 1)).Take(filter.MaxItems);


            var result = cl2.Select(c => new CustomerListResult
            {
                Id = c.Id,
                Firstname = c.Firstname,
                Lastname = c.Lastname,
                Company = c.Company,
                Entered = c.Entered
            });

            return result;
        }

        #region Account Operations and Data

        /// <summary>
        /// Authenticates a user by username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool AuthenticateUser(string username, string password)
        {
            var user = AuthenticateAndRetrieveUser(username, password);
            if (user == null)
                return false;

            return true;
        }

        /// <summary>
        /// Authenticates a user by username and password and returns the
        /// user instance
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Customer AuthenticateAndRetrieveUser(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    SetError("Invalid username or password.");
                    return null;
                }

                // assumes only no dupe email addresses
                var user = GetCustomerByEmailAddress(username);
                if (user == null)
                {
                    SetError("Invalid username or password.");
                    return null;
                }

                if (!user.IsActive)
                {
                    SetError("This account is not activated yet. Please activate the account, or reset your password");
                    return null;
                }



                string passwordHash = HashPassword(password, user.Id.ToString());
                if (user.Password != passwordHash && user.Password != password)
                {
                    SetError("Invalid username or password.");
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                SetError(ex);
                return null;
            }
        }

        /// <summary>
        /// Checks if an Email address is valid
        /// </summary>
        /// <param name="email">Email address</param>
        /// <returns></returns>
        public bool IsValidEmailAddress(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        const string HashPostFix = "|~|";

        /// <summary>
        /// Returns an hashed and salted password.
        ///
        /// Encoded Passwords end in || to indicate that they are
        /// encoded so that bus objects can validate values.
        /// </summary>
        /// <param name="password">The password to convert</param>
        /// <param name="customerIdSalt">
        /// Unique per instance salt - use user id</param>
        /// <param name="appSalt">Salt to apply to the password</param>
        /// <returns>Hashed password. If password passed is already a hash
        /// the existing hash is returned
        /// </returns>
        public static string HashPassword(string password, string customerIdSalt)
        {
            var appSalt = "@19a2sx1-$38%5#92#";

            // don't allow empty password
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            // already encoded
            if (password.EndsWith(HashPostFix))
                return password;

            string saltedPassword = customerIdSalt + password + appSalt;

            // pre-hash
            var sha = SHA1.Create();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(saltedPassword));

            // hash again
            var sha2 = SHA256.Create();
            hash = sha2.ComputeHash(hash);

            return StringUtils.BinaryToBinHex(hash) + HashPostFix;
        }

        #endregion

        #region Password Recovery

        /// <summary>
        /// Creates a validation link which is used for initial account validation
        /// and password recovery. Basically a compressed Guid.
        /// </summary>
        /// <returns></returns>
        public string CreateValidationKey() => DataUtils.GenerateUniqueId(20);

        /// <summary>
        /// Creates a new Recovery Validation Id and store it on the user's
        /// validationKey field and save
        /// a password
        /// </summary>
        /// <param name="email"></param>
        /// <returns>id or null on error</returns>
        public string CreateValidationKeyForUser(string email)
        {
            string validationLink = CreateValidationKey();
            var user = GetCustomerByEmailAddress(email);
            if (user == null)
                return null;

            user.ValidationKey = validationLink;

            if (!Save())
                return null;

            return validationLink;
        }

        /// <summary>
        /// Validates an email address and makes the account active.
        ///
        /// Does not save the customer (call SaveChanges())
        /// </summary>
        /// <param name="email">users email address</param>
        /// <param name="validationId">a validation key from the url</param>
        /// <returns></returns>
        public bool ValidateEmail(string validationId, string email)
        {
            var user = GetCustomerByValidationKey(validationId);
            if (user == null || user.ValidationKey != validationId || user.Email != email)
            {
                SetError("Invalid email or validation code provided " + validationId + " " + user?.ValidationKey);
                return false;
            }

            // clear the validation key
            user.ValidationKey = null;
            user.IsActive = true;

            return true;
        }

        #endregion


        #region Bus Object Overrides

        protected override bool OnValidate(Customer customer)
        {
            if (!base.OnValidate(customer))
                return false;

            if (string.IsNullOrEmpty(customer.Lastname) && string.IsNullOrEmpty(customer.Company))
                ValidationErrors.Add(WebStoreBusinessResources.NameOrCompanyMustBeEntered, "Lastname");

            if (string.IsNullOrEmpty(customer.Email))
                ValidationErrors.Add(WebStoreBusinessResources.EmailAddressRequired, "Email");

            var address = CustomerBusiness.GetBillingAddress(customer);

            if (string.IsNullOrEmpty(address.PostalCode))
                ValidationErrors.Add(WebStoreBusinessResources.YouMustProvideAValidPostalCode, "PostalCode");

            bool isUsOrder = (address.CountryCode == "US" || address.CountryCode == "CA");

            ValidateEmailAddress(customer.IsNew, customer);

            if (ValidationErrors.Count > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Validate password rules:
        /// * min 8 chars
        /// * upper and lower
        /// * min 1 number
        /// </summary>
        /// <param name="password">Password to compare</param>
        /// <returns></returns>
        public bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8) return false;

            bool hasLower = false,
                hasUpper = false,
                hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsDigit(c))
                    hasDigit = true;
                if (char.IsLetter(c) && (int)c >= 'A' && (int)c <= 'Z')
                    hasUpper = true;
                if (char.IsLetter(c) && (int)c >= 'a' && (int)c <= 'z')
                    hasLower = true;
            }

            var result = hasLower && hasUpper && hasDigit;

            if (!result)
                ValidationErrors.Add(WebStoreBusinessResources.InvalidPasswordFormat);

            return result;
        }

        /// <summary>
        /// Validates email address and ensures the email address isn't duplicated
        /// </summary>
        /// <param name="isNew"></param>
        /// <param name="email"></param>
        public bool ValidateEmailAddress(bool isNew, Customer customer)
        {
            var email = customer.Email;

            if (!IsValidEmailAddress(email))
            {
                ValidationErrors.Add(WebStoreBusinessResources.EmailAddressIsInvalid, "Email");
                return false;
            }

            email = email.ToLower();

            if (isNew && Context.Customers
                    .Any(c => c.Email == email))
            {
                ValidationErrors.Add(
                    WebStoreBusinessResources.EmailAddressAlreadyInUse, "Email");
                return false;
            }

            // changing email to an address that already exists for another customer?
            if (Context.Customers
                .Any(c => c.Id != customer.Id && c.Email.ToLower() == email))
            {
                ValidationErrors.Add(WebStoreBusinessResources.EmailAddressAlreadyInUse, "Email");
                return false;
            }

            return true;
        }


        /// <summary>
        /// Creates an empty customer that can be saved
        /// </summary>
        /// <returns></returns>
        public Customer CreateEmpty()
        {
            var customer  = Create();
            customer.Email = string.Empty;
            customer.Firstname = string.Empty;
            customer.Lastname = string.Empty;
            customer.Password = string.Empty;

            return customer;
        }

        protected override void OnAfterCreated(Customer entity)
        {
            // new records need to be validated
            entity.IsNew = true;
            entity.IsActive = false;
            entity.Addresses.Add(new Address()  { AddressType = AddressTypes.Billing });
        }


        protected override bool OnBeforeSave(Customer entity)
        {
            if (!base.OnBeforeSave(entity))
                return false;

            if (entity._extraProperties != null)
            {
                entity._extraPropertiesStorage = JsonSerializationUtils.Serialize(entity._extraProperties);
            }

            if (!string.IsNullOrEmpty(entity.Password) && !entity.Password.EndsWith(HashPostFix))
            {
                entity.Password = HashPassword(entity.Password, entity.Id);
            }

            entity.Updated = DateTime.Now;

            return true;
        }

        protected override bool OnAfterSave(Customer entity)
        {
            if (!base.OnAfterSave(entity))
                return false;

            entity.IsNew = false;
            return true;
        }

        #endregion


    }

    public class CustomerListResult
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public string Company { get; set; }

        public DateTime Entered { get; set; }

        public string LastOrder { get; set; }

        public int OrderCount { get; set; }
    }

    public class CustomerListFilter
    {
        public bool ActiveOnly { get; set; } = true;

        public string Company { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }


        public int MaxItems { get; set; } = 2000;

        public int ItemPage { get; set; } = 1;

        public CustomerListSortOrder SortOrder { get; set; } = CustomerListSortOrder.Lastname;
    }

    public enum CustomerListSortOrder
    {
        Lastname,
        Company,
        Entered
    }

    public class CustomerListItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string City { get; set; }

    }
}
