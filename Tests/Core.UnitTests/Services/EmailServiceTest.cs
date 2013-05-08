namespace App.UnitTests.Services
{
    using System;
    using System.Net;
    using App.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EmailServiceTest
    {
        [TestMethod]
        public void Sends_an_Email()
        {
            // Arrange
            var credentials = new NetworkCredential();
            var service = new EmailService(credentials);
            var data = new
            {
                SiteName = "Site Name",
                EmailVerificationUrl = "http://www.sample.com/verify/1234567890"
            };

            // Act
            service.Send("user@example.com", "WelcomeNewUser", "Welcome to Site", data);
        }
    }
}
