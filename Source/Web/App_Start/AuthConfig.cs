// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthConfig.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Web
{
    using Microsoft.AspNet.Membership.OpenAuth;
    using Microsoft.AspNet.Membership.OpenAuth.Data;

    public static class AuthConfig
    {

        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            OpenAuth.UsersAccountsTableName = "[Membership].[UserOpenAuthAccount]";
            OpenAuth.UsersDataTableName = "[Membership].[UserOpenAuthData]";

            //OpenAuth.AuthenticationClients.AddMicrosoft(
            //    clientId: "",
            //    clientSecret: "");

            //OpenAuth.AuthenticationClients.AddTwitter(
            //    consumerKey: "",
            //    consumerSecret: "");

            //OpenAuth.AuthenticationClients.AddFacebook(
            //    appId: "",
            //    appSecret: "");

            OpenAuth.AuthenticationClients.AddGoogle();
        }
    }
}
