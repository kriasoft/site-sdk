<img src="http://i.imgur.com/mGy9OWv.png" alt="" style="float:right;padding:0 0 6px 12px;" />
# Web Application Starter Kit for .NET developers

This a solution (project) template of a typical web application built on top of the following technologies:

 - [ASP.NET MVC 4](http://www.asp.net/mvc/mvc4), [ASP.NET Web API](http://www.asp.net/web-api), [ASP.NET Web Pages](http://www.asp.net/web-pages) (Razor)
 - SQL Server Database Project with [SSDT](http://msdn.microsoft.com/en-us/data/tools.aspx)
 - [Entity Framework](http://msdn.microsoft.com/en-us/data/ef.aspx) 5 with [Database First](http://msdn.microsoft.com/en-us/data/jj206878.aspx)
 - Real-time web functionality with [SignalR](http://www.asp.net/signalr)
 - [Enterprise Library 6.0](http://msdn.microsoft.com/en-us/library/ff648951.aspx)
 - [AngularJS](http://angularjs.org) JavaScript MVC Framework
 - [Windows Azure SDK](http://www.windowsazure.com/en-us/develop/net/)
 - [NuGet](http://nuget.org/) with [Package Restore](http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages)

You can use it to bootstrap your web and cloud app projects and dev environment. The main advantabe of building your app
on top of it - is that you'll be able to pull and merge updates at any time in the future by using Git functionality.

The starter kit contains multi-project solution structure, a set of 3rd party libraries, tools and a bounch of scripts
all preconfigured for instant web development gratification. Just clone the repo, open ```Source/Application.sln```
and you are ready to develop and test your application.

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

### Database Context / Entities

![Database Context / Entities](http://i.imgur.com/in7AgDB.png)

### Get Involved

Join our [discussion board](https://groups.google.com/forum/?fromgroups=#!forum/sitesdk) or [Skype](http://www.skype.com) chat. Sumbit [feature requests](https://github.com/kriasoft/site-sdk/issues/new?labels=enhancement) and [bug reports](https://github.com/kriasoft/site-sdk/issues/new?labels=bug).

Click `WIN+R`, copy and paste `skype:?chat&blob=-c-fREUqp9QPTRWgkVJIoX-wdAgmiPwrTF91u8d34_xC3gLO91Y` then hit `[OK]`

![SiteSDK Skype Chat](http://i.imgur.com/Nq9Q7.png)
