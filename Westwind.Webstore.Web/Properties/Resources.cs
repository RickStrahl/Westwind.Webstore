using System;
using System.Web;
using System.Resources;
using Westwind.Globalization;

namespace AppResources
{
    public class GeneratedResourceSettings
    {
        // You can change the ResourceAccess Mode globally in Application_Start        
        public static ResourceAccessMode ResourceAccessMode = DbResourceConfiguration.Current.ResourceAccessMode;
    }


    [System.CodeDom.Compiler.GeneratedCodeAttribute("Westwind.Globalization.StronglyTypedResources", "3.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Account
    {
        public static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    var asmbly = typeof(Account).Assembly;
                    var nameSpace = asmbly.GetName().Name + ".Properties.Account";
                    var temp = new ResourceManager(nameSpace, asmbly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        private static ResourceManager resourceMan = null;

		public static System.String CouldntSaveYourProfile
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","CouldntSaveYourProfile",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Country
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","Country",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String CreateANewAccount
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","CreateANewAccount",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String DontHaveAnAccount
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","DontHaveAnAccount",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String EmailAddressRequired
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","EmailAddressRequired",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String EnterYourEmailAddress
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","EnterYourEmailAddress",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String EnterYourPassword
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","EnterYourPassword",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String IForgotMyPassword
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","IForgotMyPassword",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String NameOrCompanyMustBeEntered
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","NameOrCompanyMustBeEntered",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String PasswordMissingOrdontMatch
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","PasswordMissingOrdontMatch",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String PasswordRecovery
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","PasswordRecovery",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String PasswordRecoveryMessageWithEmail
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","PasswordRecoveryMessageWithEmail",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String PleaseEnterANewPassword
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","PleaseEnterANewPassword",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String PleaseSignInToYourAccount
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","PleaseSignInToYourAccount",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ProfileInformationSaved
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","ProfileInformationSaved",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String SendRecoveryEmail
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","SendRecoveryEmail",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String SignIn
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","SignIn",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String SignInButton
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","SignInButton",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String SignInToYourAccount
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","SignInToYourAccount",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String SignOut
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","SignOut",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ThankYouForVerifyingYourEmail
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","ThankYouForVerifyingYourEmail",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String VerifyYourPassword
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Account","VerifyYourPassword",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

	}


    [System.CodeDom.Compiler.GeneratedCodeAttribute("Westwind.Globalization.StronglyTypedResources", "3.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Home
    {
        public static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    var asmbly = typeof(Home).Assembly;
                    var nameSpace = asmbly.GetName().Name + ".Properties.Home";
                    var temp = new ResourceManager(nameSpace, asmbly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        private static ResourceManager resourceMan = null;

		public static System.String WhatsNewMessage
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Home","WhatsNewMessage",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String WhatsNewMessageHeader
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Home","WhatsNewMessageHeader",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

	}


    [System.CodeDom.Compiler.GeneratedCodeAttribute("Westwind.Globalization.StronglyTypedResources", "3.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class LocalizationForm
    {
        public static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    var asmbly = typeof(LocalizationForm).Assembly;
                    var nameSpace = asmbly.GetName().Name + ".Properties.LocalizationForm";
                    var temp = new ResourceManager(nameSpace, asmbly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        private static ResourceManager resourceMan = null;

		public static System.String Add
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Add",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String AllResourceSets
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","AllResourceSets",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String AreYouSureYouWantToDeleteThisResource
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","AreYouSureYouWantToDeleteThisResource",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String AreYouSureYouWantToDoThis
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","AreYouSureYouWantToDoThis",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Backup
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Backup",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Backup_Title
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Backup.Title",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String BackupComplete
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","BackupComplete",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String BackupFailed
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","BackupFailed",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Cancel
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Cancel",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Comment
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Comment",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String CreateClass
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","CreateClass",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String CreateClass_Title
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","CreateClass.Title",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String CreateClassInfo
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","CreateClassInfo",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String CreateStronglyTypedClasses
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","CreateStronglyTypedClasses",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String CreateTable
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","CreateTable",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String CreateTable_Title
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","CreateTable.Title",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Delete
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Delete",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Delete_ResourceSet_Title
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Delete.ResourceSet.Title",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Delete_Title
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Delete.Title",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String DisableResourceEditing
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","DisableResourceEditing",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String DynamicClasses
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","DynamicClasses",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Edit
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Edit",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String EnableResourceEditing
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","EnableResourceEditing",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Export
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Export",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ExportResx_Title
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ExportResx.Title",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String FeatureNotSupported
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","FeatureNotSupported",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Filename
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Filename",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String FileResourceUpload
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","FileResourceUpload",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Folder
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Folder",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String From
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","From",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String GridView
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","GridView",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String GridView_Title
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","GridView_Title",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String HelloWorld
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","HelloWorld",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Home_Title
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Home.Title",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ImportExportResx
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ImportExportResx",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ImportExportResx_Title
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ImportExportResx.Title",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ImportOrExportResxResources
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ImportOrExportResxResources",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ImportResx
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ImportResx",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ImportResx_Title
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ImportResx.Title",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String InvalidResourceId
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","InvalidResourceId",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Lang
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Lang",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String LocaleIdsFailedToLoad
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","LocaleIdsFailedToLoad",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String LocalizationAdministration
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","LocalizationAdministration",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String LocalizationTableHasBeenCreated
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","LocalizationTableHasBeenCreated",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String LocalizationTableNotCreated
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","LocalizationTableNotCreated",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String NoProviderConfigured
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","NoProviderConfigured",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String NoResourcePassedToAddOrUpdate
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","NoResourcePassedToAddOrUpdate",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Refresh
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Refresh",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ReloadResources
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ReloadResources",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ReloadResources_Title
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ReloadResources.Title",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Rename
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Rename",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Rename_Title
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Rename.Title",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResourceEditor
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResourceEditor",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResourceGenerationFailed
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResourceGenerationFailed",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResourceImportFailed
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResourceImportFailed",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResourceProviderInfo
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResourceProviderInfo",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Resources
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Resources",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResourceSaved
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResourceSaved",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResourceSetLoadingFailed
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResourceSetLoadingFailed",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResourceSetRenamed
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResourceSetRenamed",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResourceSetsToExport
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResourceSetsToExport",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResourcesHaveBeenBackedUp
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResourcesHaveBeenBackedUp",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResourcesHaveBeenReloaded
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResourcesHaveBeenReloaded",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResourceUpdateFailed
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResourceUpdateFailed",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResxDesignerClasses
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResxDesignerClasses",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResxExportInfo
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResxExportInfo",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResxExportInfo_Project
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResxExportInfo.Project",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResxExportInfo_WebForms
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResxExportInfo.WebForms",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResxImportInfo
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResxImportInfo",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResxImportInfo_Project
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResxImportInfo.Project",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResxImportInfo_WebForms
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResxImportInfo.WebForms",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResxResourcesHaveBeenCreated
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResxResourcesHaveBeenCreated",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ResxResourcesHaveBeenImported
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","ResxResourcesHaveBeenImported",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Save
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Save",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String SaveResource
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","SaveResource",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String SearchResources
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","SearchResources",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String StronglyTypedClassCreated
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","StronglyTypedClassCreated",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String StronglyTypedGlobalResourcesFailed
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","StronglyTypedGlobalResourcesFailed",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String TextToTranslate
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","TextToTranslate",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String To
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","To",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Translate
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Translate",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String TranslateResource
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","TranslateResource",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Translation
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Translation",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Use
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","Use",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String YouAreAboutToDeleteThisResourceSet
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("LocalizationForm","YouAreAboutToDeleteThisResourceSet",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

	}


    [System.CodeDom.Compiler.GeneratedCodeAttribute("Westwind.Globalization.StronglyTypedResources", "3.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources
    {
        public static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    var asmbly = typeof(Resources).Assembly;
                    var nameSpace = asmbly.GetName().Name + ".Properties.Resources";
                    var temp = new ResourceManager(nameSpace, asmbly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        private static ResourceManager resourceMan = null;

		public static System.String HelloWorld
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Resources","HelloWorld",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String PleaseFixTheFollowing
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Resources","PleaseFixTheFollowing",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Today
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Resources","Today",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Yesterday
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Resources","Yesterday",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

	}


    [System.CodeDom.Compiler.GeneratedCodeAttribute("Westwind.Globalization.StronglyTypedResources", "3.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ShoppingCart
    {
        public static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    var asmbly = typeof(ShoppingCart).Assembly;
                    var nameSpace = asmbly.GetName().Name + ".Properties.ShoppingCart";
                    var temp = new ResourceManager(nameSpace, asmbly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        private static ResourceManager resourceMan = null;

		public static System.String ContinueShoppingOrChangeOrder
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("ShoppingCart","ContinueShoppingOrChangeOrder",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String InvoiceCouldNotBeSaved
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("ShoppingCart","InvoiceCouldNotBeSaved",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String InvoiceSaved
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("ShoppingCart","InvoiceSaved",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Item
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("ShoppingCart","Item",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String NotesAndInformation
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("ShoppingCart","NotesAndInformation",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String PaymentInformation
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("ShoppingCart","PaymentInformation",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String PlaceOrder
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("ShoppingCart","PlaceOrder",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Price
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("ShoppingCart","Price",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Product
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("ShoppingCart","Product",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ShoppingCartIsEmpty
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("ShoppingCart","ShoppingCartIsEmpty",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String ShoppingCartTitle
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("ShoppingCart","ShoppingCartTitle",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

	}

}
