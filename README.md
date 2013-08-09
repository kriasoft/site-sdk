## Web Application Starter Kit for .NET developers

![Solution](http://i.imgur.com/Gg8T6Yb.png)

This a solution (project) template of a typical single-page web application built on top of the following technologies:

 - [ASP.NET Web API](http://www.asp.net/web-api), [ASP.NET Web Pages](http://www.asp.net/web-pages) (Razor)
 - [AngularJS](http://www.angularjs.org) client side framework by Google
 - [SQL Server](http://www.windowsazure.com/en-us/services/data-management/) database project with [SSDT](http://msdn.microsoft.com/en-us/data/tools.aspx)
 - [Entity Framework](http://msdn.microsoft.com/en-us/data/ef.aspx) 6 with [Database First](http://msdn.microsoft.com/en-us/data/jj206878.aspx)
 - Real-time web functionality with [SignalR](http://www.asp.net/signalr)
 - [Enterprise Library 6.0](http://msdn.microsoft.com/en-us/library/ff648951.aspx)
 - [Windows Azure SDK](http://www.windowsazure.com/en-us/develop/net/)
 - [NuGet](http://nuget.org/) with [Package Restore](http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages)

You can use it to bootstrap your web and cloud app projects and dev environment. The main advantabe of building your app
on top of it - is that you'll be able to pull and merge updates at any time in the future by using Git functionality.

The starter kit contains multi-project solution structure, a set of 3rd party libraries, tools and a bunch of scripts
all preconfigured for instant web development gratification. Just clone the repo, open ```Source/Application.sln```
and you are ready to develop and test your application.

### Prerequisites

 - [Visual Studio 2012](http://www.visualstudio.com) with [Update 3](http://go.microsoft.com/fwlink/?LinkID=290979) and extensions:
   - [NuGet](http://www.nuget.org) package manager
   - [Web Tools 2012.2](http://go.microsoft.com/fwlink/?LinkId=282650)
   - [Windows Azure .NET SDK](http://www.windowsazure.com/en-us/downloads/?sdk=net)
   - [SQL Server Data Tools](http://msdn.microsoft.com/en-us/data/tools.aspx)
   - [Web Essentials 2012](http://visualstudiogallery.msdn.microsoft.com/07d54d12-7133-4e15-becb-6f451ea3bea6)
   - [TypeScript](http://www.typescriptlang.org)
   - [StyleCop](https://stylecop.codeplex.com/) (optional)

*Hint: make sure that you have the latest version and updates for Visual Studio and required extensions installed*

### Getting Started

To clone the repo run:

    git clone -o base git@github.com:kriasoft/site-sdk.git MyApp

Where ```MyApp``` is your project name. Next rename the included solution file:

    git mv Source/Application.sln Source/MyApp.sln
    git add .
    git commit -m 'Rename Application.sln file'

Open MyApp.sln file in Visual Studio and you are ready to go.

Later on you can always pull and merge the latest changes from [SiteSDK](https://github.com/kriasoft/site-sdk) repo into your project by running the following command:

    git pull base master

This way you will make sure that Membership Service, Email Service, Authorization via 3rd party OAuth providers and other Starter Kit's modules are all up to date, leaving your more time on developing real stuff specific to your application.

### Membership Entities

![Database Context / Entities](http://i.imgur.com/in7AgDB.png)

### Get Involved

Join our [discussion board](https://groups.google.com/forum/?fromgroups=#!forum/sitesdk) or [Skype](http://www.skype.com) chat. Sumbit [feature requests](https://github.com/kriasoft/site-sdk/issues/new?labels=enhancement) and [bug reports](https://github.com/kriasoft/site-sdk/issues/new?labels=bug).

Click `WIN+R`, copy and paste `skype:?chat&blob=-c-fREUqp9QPTRWgkVJIoX-wdAgmiPwrTF91u8d34_xC3gLO91Y` then hit `[OK]`

![SiteSDK Skype Chat](http://i.imgur.com/Nq9Q7.png)
