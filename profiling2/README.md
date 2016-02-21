Profiling2 Overview
==============================================

This project contains a human rights perpetrator web application, including a full-text indexed document repository.  It is also integrated with a screening workflow system.

This README is for developers setting up the project for the first time.


Required Environment / Minimum Setup
----------------------------------------------

- MSSQL Server 2008 R2
- Visual Studio 2012
    - .NET 4.5
- Update to [ASP.NET MVC 3](http://www.asp.net/mvc/mvc3)
- Update [NuGet extension](http://nuget.codeplex.com/releases/view/64974) via Extension Manager
- In Visual Studio, do 
    - Project -> [Enable NuGet Package Restore](http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages)
    - Tools -> Manage NuGet Packages -> Restore
- Install [Bundler Run on Save extension](https://github.com/ServiceStack/Bundler) (if it works on Visual Studio 2012 by the time you read this)
- [Aspose.Total](http://www.aspose.com/.net/total-component.aspx) - this requires a valid license
- [Tesseract language training data](https://code.google.com/p/tesseract-ocr/wiki/ReadMe), English and French

Dependencies
----------------------------------------------

The following are nuget dependencies that need to be packaged and added to a local nuget repository.

- [HrdbWebServiceClient](https://bitbucket.org/jundir/hrdbwebserviceclient)
- [ScorePhrase](https://bitbucket.org/jundir/scorephrase)

Configuration
----------------------------------------------

Locally, all configuration files are in the root directory of Profiling2.Web.Mvc.

### Selected configuration keys

In `Web.config`, set `membership`'s `defaultProvider` to `LocalMembershipProvider`.  In production, `ActiveDirectoryMembershipProvider` is also possible (custom made, located in Profiling2.Infrastructure.Security - not the one included in ASP.NET).  

`roleManager`'s `defaultProvider` is `LocalRoleProvider` both in production and  development.

### Database

A new database may be setup by importing an initial dump, then running database migrations over it.  This has the advantage of having real data to develop on (at the same time managing the risk of having sensitive data at your location).

Alternatively, you can setup the database from scratch.  To import initial dump:

- In SSMS, create new database
- create file 'ftrow\_Ft\_sources' with filegroup 'ftfg\_Ft\_sources'
- create file 'SourceDocuments' with filegroup 'SourceDocuments'
- new query
- open Profiling2.Migrations/201301221832_schema.sql
- execute

Create database user:

- Security->Login->Create new login...
- Databases->(database name)->Security->Users->Create new user...
- Check 'db_owner' under 'database role membership'

Run migrations:

- Build Profiling2.Migrations
- Edit the properties in Build/Migrate.proj with your local machine settings
- Run Build/Migrate.cmd from the command line

### Adding First User

See [Creating A New Admin User](https://bitbucket.org/jundir/profiling2/wiki/Creating%20A%20New%20Admin%20User)

### Aspose license

An [Aspose.Total](http://www.aspose.com/.net/total-component.aspx) license is required for the Aspose libraries, which are used when previewing sources and generating reports.  Renewing enables continual access to updates to the software; however the Aspose libraries will continue to function as long as the date of each Aspose library is less than the date of the license.  The path to it can be configured in Profiling2.Web.Mvc/Local.config.  You can find the expiry date in the license file.

### Tesseract for OCR

Tesseract data must be in the directory configured under the parameter `TesseractDataDirectory`, or in the same directory as the Profiling2.Ocr.dll.  See the Tesseract link under Extended Resources for more info.


Walkthrough / Smoke Test
----------------------------------------------

OCR - to test that Tesseract is functioning properly, login and go to Sources -> Feeding -> Upload, and upload an image or a PDF containing scanned images.  Once logged in, log in as another user with 'CanApproveAndRejectSources' permission, and approve the source.  The application log (configured in log4net.config) should not throw any exceptions.

Lucene search - login as a user with `CanChangePersons` permission, and create a person.  Then try to search for that person.  The person search requires the `LuceneIndexDirectory` parameter to be configured.


Testing
----------------------------------------------

MSpec tests exist in the MSpecTests.Profiling2 project.  Use the mspec executable included with the Machine.Specifications NuGet package to run the tests from the command line.  A convenience script, mspec.bat, exists in the solution root folder for this purpose.


Build Process
----------------------------------------------

- cd /path/to/Profiling2/Build
- BuildAndPackage.cmd 0 3 4 (where 0.3.4 is the new version number)
- Copy /path/to/Profiling2/Drops/0.3.4/Profiling2.v0.3.4.zip to production
- Unzip and merge production config files with any new changes
- Stop the Profiling2AppPool
- Backup D:\Apps\Profiling2 and replace with new dir
- Run database migrations
- Start Profiling2AppPool
- Commit the updated Common/AssemblyVersion.cs, and run 'git tag -a deployed-yyyyMMddxx -m 0.3.4'


Design
----------------------------------------------

Spot for designers to put any information they need.


Known Issues / Gotcha
----------------------------------------------

- File upload size limits are in the Web.config (i.e. `<httpRuntime executionTimeout="600" maxRequestLength="51200" />` for IIS6), but look out for other parameters that need to be tweaked in IIS.
- A [customised version](https://github.com/jliew/Sharp-Architecture/commits/2.1.2-envers) of SharpArch.NHibernate.2.1.2 is used at the moment, located at ReferencedAssemblies/MONUSCO/SharpArch.NHibernate.dll.  It allows NHibernate.Envers to be configured.  SharpArch issue [85](https://github.com/sharparchitecture/Sharp-Architecture/issues/85).
- Build process still has an outstanding issue where Tesseract dependencies are not always included in the Drops archive (`libtesseract302.dll` and `liblept168.dll` in the bin/x86 and bin/x64 folders).  Running the build script again fixes the problem.
- An IOException may be thrown regarding access to a Lucene write.lock file.  This may be because a background job is trying to access the Lucene index at the same time as a user does.  The workaround at the moment for this is to ensure that Hangfire is always running in order that background jobs are triggered at their scheduled time (see [New Server Worklog](https://bitbucket.org/jundir/profiling2/wiki/New%20Server%20Worklog) for details); keeping in mind that recycling an application pool in IIS may not leave any workers running if done after hours (even with Idle Timeout=0).
- Scrolling through source search results (with automated paging) may result in a deadlock exception.  See [this page](https://confluence.atlassian.com/display/CONFKB/Database+deadlock+on+Microsoft+SQL+Server) to set the database's read_committed_snapshot_on flag.


Extended Resources
----------------------------------------------

[Tesseract ReadMe](https://code.google.com/p/tesseract-ocr/wiki/ReadMe)