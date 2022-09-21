using Westwind.Utilities;
using Westwind.Utilities.Data;
using Westwind.Webstore.Business;

namespace Westwind.Webstore
{
    /// <summary>
    /// Class that can be used to validate
    /// </summary>
    public class EmailAddressValidator
    {

        public int TimeoutMinutes { get; set; } = 10;

        public string ConnectionString { get; set; } = wsApp.Configuration.ConnectionString;

        public string ErrorMessage { get; set; }

        public string Tablename = "wws_EmailValidationCodes";

        public static string GenerateValidationKey()
        {
            return DataUtils.GenerateUniqueId(15);
        }

        public string GenerateCode(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ErrorMessage = "You need to provide an email address";
                return null;
            }

            var id = GenerateValidationKey();
            var db = new SqlDataAccess(ConnectionString);

            var insertSql = $"insert into {Tablename} (ValidationCode, email) values (@0,@1)";

            if (db.ExecuteNonQuery(insertSql, id, email) == 1)
                return id;

            if (!DoesTableExist() &&
                CreateTable() &&
                db.ExecuteNonQuery(insertSql, id, email) == 1)
                return id;

            ErrorMessage = "Couldn't generate Validation Code: " + db.ErrorMessage;
            return null;
        }


        /// <summary>
        /// Validates a code that was generated with Generate Code
        /// Checks if it exists and sets its IsValidated flag so it can be rechecked
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool ValidateCode(string code, string email)
        {
            if (string.IsNullOrEmpty(code))
            {
                ErrorMessage = "Invalid or missing validation code.";
                return false;
            }

            if (string.IsNullOrEmpty(email))
            {
                ErrorMessage = "Invalid or missing email adddress for validation.";
                return false;
            }

            var db = new SqlDataAccess(ConnectionString);

            string result = db.ExecuteScalar(
                $@"
delete from {Tablename} where DateDiff( Minute,  Timestamp, getutcdate()) > @0;
select ValidationCode from {Tablename} where ValidationCode = @1 and Email = @2;
update {Tablename} set IsValidated = 1 where ValidationCode = @1 and Email = @2",
                TimeoutMinutes, code, email) as string;

            if (string.IsNullOrEmpty(result))
            {
                ErrorMessage = "Invalid validation code. " + db.ErrorMessage;
                return false;
            }

            return true;
        }


        /// <summary>
        /// checks to see if a validation code exists and has been previously validated
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsCodeValidated(string code, string email, bool remove = false)
        {
            if (string.IsNullOrEmpty(code))
                return false;

            var db = new SqlDataAccess(ConnectionString);

            string result = db.ExecuteScalar(
                $"select ValidationCode from {Tablename} where ValidationCode = @0 and Email = @1 and IsValidated = 1",
                code, email) as string;

            if (string.IsNullOrEmpty(result))
                return false;

            if (remove)
                db.ExecuteNonQuery($"delete from {Tablename} where ValidationCode = @0", code);

            return true;
        }


        /// <summary>
        /// Creates the Validation Codes table
        /// </summary>
        /// <returns></returns>
        public bool CreateTable(bool forceRecreation = false)
        {
            if (!forceRecreation && DoesTableExist())
                return true;


            var sql = $@"
CREATE TABLE {Tablename}(
	[Timestamp] [datetime] NOT NULL DEFAULT GetUtcDate(),
	[ValidationCode] [nvarchar](30) NULL,
	[Email] [nvarchar](120) NULL,
	[IsValidated] [bit] NOT NULL DEFAULT 0
)";

            if (forceRecreation)
                sql = $"DROP TABLE {Tablename};\r\n" + sql;

            var db = new SqlDataAccess(ConnectionString);
            if (db.ExecuteNonQuery(sql) < 0)
            {
                ErrorMessage = "Couldn't create table: " + db.ErrorMessage;
                return false;
            }

            return true;
        }


        /// <summary>
        /// Checks to see if the validation codes table exists
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public bool DoesTableExist()
        {
            var sql = @"SELECT TABLE_CATALOG FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @0";

            var db = new SqlDataAccess(ConnectionString);
            var cat = db.ExecuteScalar(sql, Tablename);

            return !(cat is null);
        }


    }
}
