# Abstract Project Template from West Wind Web Store Project

I need to create a project template based on the West Wind Web Store project. The template should create a blueprint template that can be used for a new project by copying an 'empty project' template project that provides the core features and uses the same base components that the West Wind project provides.

## Key Included Features

### Initial Project Creation

* Use West Wind Libraries
	* Westwind.Utilities  / Westwind.Utilities.Data
	* Westwind.AspNetCore
	* Westwind.AspNetCore.Markdown
	* Westwind.AspNetCore.LiveReload
	* Westwind.Data.EfCore
	* Westwind.Globalization
* Use Support Libs
	* MailKitLite
	* NewtonSoft.Json

* Logging
* Live Reload
* WestwindGlobalization
* Cors
* Authentication
	* Cookie 
	* Tokens (Json)
* User Login UI (don't use ASP.NET Identity system!)
	* Login
	* User Email Validation
	* Password Recovery
	* Same User Entity Model used Westwind.WebStore.Business
	* Account Password Hashing (`HashPassword()`)
* Business Objects Project
	* Based on Westwind.Data.EfCore
	* Sql Server mappings
* UI Libraries
	* Bring in Bootstrap, FontAwesome
	* Base styling from application.css and store.css (rename store.css to site.css)

### Based on program.cs
Pick up most of the generic functionality exposed throgh `program.cs` in the template

Template should be able to run and present a home page and allow for user login to a test authenticated page.

### Where to Create?
Create the Project in a new folder that's:

* At the same level as the Westwind.WebStore solution
* Project should be otherwise self contained
* Name it Westwind.Web.Template