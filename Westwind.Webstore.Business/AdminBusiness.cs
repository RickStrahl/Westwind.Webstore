using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Westwind.Utilities.Data;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Business
{
    public class AdminBusiness : WebStoreBusinessObject<Customer>
    {


        public AdminBusiness(WebStoreContext context) : base(context)
        {
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
                  (invoices.CreditCardResult_ProcessingResult = 'APPROVED' OR invoices.CreditCardResult_ProcessingResult = 'PAID IN FULL')
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

        #region Database Cleanup

        /// <summary>
        /// Backs up the current Database to a specific path
        /// </summary>
        /// <param name="serverBackupFilename"></param>
        /// <param name="database">Name of the database to back up</param>
        /// <returns></returns>
        public string BackupDatabase(string serverBackupFilename, string database = "WebStoreNew",
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

            try
            {

                ZipFile.CreateFromDirectory(basePath, zipFileTemp);
                File.Delete(serverBackupFilename);
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


        #endregion
    }
}
