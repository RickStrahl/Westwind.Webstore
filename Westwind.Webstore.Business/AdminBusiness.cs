using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Westwind.Utilities;
using Westwind.Utilities.Data;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Business
{
    public class AdminBusiness : WebStoreBusinessObject<Customer>
    {
        BusinessFactory BusinessFactory { get; }

        public AdminBusiness(WebStoreContext context, BusinessFactory businessFactory) : base(context)
        {
            BusinessFactory = businessFactory;
            var ctx = context;
        }

        #region Reports

        /// <summary>
        /// returns a list of email addresses for a given sku and a duration
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public List<EmailResult> EmailListFromSkus(string sku, int days = 90)
        {
            var sql = @"
select sku, email, invoiceDate, CreditCardResult_ProcessingResult
            from lineitems, invoices, customers
            where lineitems.InvoiceId = invoices.id and
                  invoices.CustomerId = customers.id and
                  invoices.InvoiceDate > @date and
                  (invoices.CreditCardResult_ProcessingResult = 'APPROVED' OR
                   invoices.CreditCardResult_ProcessingResult = 'PAID IN FULL')
order by Sku, InvoiceDate Desc
";

            var result = Db.Query<EmailResult>(sql, this.Db.CreateParameter("@date", DateTime.Now.AddDays((days+1) * -1)));
            if (result == null)
            {
                SetError(Db.ErrorMessage);
                return new List<EmailResult>();
            }

            if (!string.IsNullOrEmpty(sku))
            {
                return result.Where(res => res.Sku.Equals(sku,StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return result.ToList();
        }

        public class EmailResult
        {
            public string Sku { get; set; }
            public string Email { get; set; }
        }

        #endregion

        #region Database Backup and Cleanup

        /// <summary>
        /// Backs up the current Database to a specific path
        /// </summary>
        /// <param name="serverBackupFilename"></param>
        /// <param name="database">Name of the database to back up</param>
        /// <returns></returns>
        public string BackupDatabase(string serverBackupFilename, string database = "WestwindWebStore",
            bool zipDatabase = false)
        {
            SetError();

            if (string.IsNullOrEmpty(serverBackupFilename))
            {
                SetError("No backup file location specified.");
                return null;
            }

            if (File.Exists(serverBackupFilename))
            {
                File.Delete(serverBackupFilename);
            }

            var basePath = Path.GetDirectoryName(serverBackupFilename);
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            if (string.IsNullOrEmpty(database))
            {
                SetError("Couldn't determine database.");
                return null;
            }

            using (var sqlData = new SqlDataAccess(wsApp.Configuration.ConnectionString))
            {
                if (sqlData.ExecuteNonQuery("BACKUP database " + database + " to DISK='" + serverBackupFilename +
                                            "' WITH FORMAT") < 0)
                {
                    SetError(sqlData.ErrorMessage);
                    return null;
                }
            }

            if (!zipDatabase)
                return serverBackupFilename;


            string zipFile = Path.ChangeExtension(serverBackupFilename, "zip");

            string zipFileTemp = zipFile.Replace("\\temp\\", "\\temp2\\");
            var tpath = Path.GetDirectoryName(zipFileTemp);

            if (!Directory.Exists(tpath))
                Directory.CreateDirectory(tpath);
            if (File.Exists(zipFileTemp))
                File.Delete(zipFileTemp);
            if (File.Exists(zipFile))
                File.Delete(zipFile);

            var productImagesFolder = Path.Combine(basePath, "product-images");

            // TODO: Use Configured image path once we upload to a custom image path (not implemented yet)
            // also copy images folder
            FileUtils.CopyDirectory("./wwwroot/images/product-images", productImagesFolder, true, true);

            try
            {
                ZipFile.CreateFromDirectory(basePath, zipFileTemp);

                // delete .bak file and product-images files
                File.Delete(serverBackupFilename);
                Directory.Delete(productImagesFolder,true);

                File.Move(zipFileTemp, zipFile);

                Directory.Delete(tpath);

            }
            catch (Exception ex)
            {
                SetError("Backup failed to create Zip File: " + ex.Message);
                return null;
            }

            return zipFile;
        }


        /// <summary>
        /// Cleanup database and compact
        /// * Delete inactive Customers with no invoices
        /// * Delete Temporary LineItems
        /// * Delete Orphaned Invoices
        /// * Shrink Database
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public string DatabaseCleanup()
        {
            var sb = new StringBuilder();
            var customerBus = BusinessFactory.GetCustomerBusiness();

            // delete customers older than 5 years and no orders
            var sql = @"select id from customers where DATEDIFF(year, lastOrder, getUtcDate()) > 5 and id NOT IN (SELECT customerId FROM invoices)";
            var oldCustomerIds = Db.QueryList<IdListItem>(sql);
            foreach (var idItem in oldCustomerIds)
            {
                customerBus.Delete(idItem.Id, saveChanges: true);
            }

            if (oldCustomerIds == null)
            {
                sb.AppendLine("Unable to get old customer ids");
                return sb.ToString();
            }

            sb.AppendLine($"Deleted {oldCustomerIds.Count} old customers without orders.");

            sql = "select id from invoices where customerId not in (select id from customers)";
            var orphanedInvoiceIds = Db.QueryList<IdListItem>(sql);
            foreach (var idItem in oldCustomerIds)
            {
                Db.ExecuteNonQuery("delete from lineitems where invoiceid = @0; delete from invoices where id = @0");
            }
            sb.AppendLine($"Deleted {orphanedInvoiceIds.Count} orphaned invoices.");

            sql = "delete from lineItems where invoiceId not in (select id from invoices)";
            var count = Db.ExecuteNonQuery(sql);

            sb.AppendLine($"Deleted {count} orphaned line items.");

            var invoiceBus = BusinessFactory.GetInvoiceBusiness();
            invoiceBus.DeleteExpiredTemporaryInvoices();

            ShrinkDatabase();

            return sb.ToString();
        }




        public bool ShrinkDatabase(string databaseName = null)
        {
            if (string.IsNullOrEmpty(databaseName))
                databaseName = wsApp.Configuration.DatabaseName;
            if (string.IsNullOrEmpty(databaseName))
                databaseName = "WestwindWebStore";

            string sql = $@"DBCC SHRINKDATABASE('{databaseName}');";
            if (Db.ExecuteNonQuery(sql) < 0)
            {
                SetError(Db.ErrorMessage);
                return false;
            }

            return true;
        }

        #endregion
    }
}
