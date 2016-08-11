using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using zxm.MailKit;
using zxm.AspNetCore.ExceptionHandler;
using zxm.MailKit.Abstractions;

namespace zxm.AspNetCore.ExceptionHandler.Tests
{
    public class ExceptionHandlerMiddlewareTest
    {
        [Fact]
        public async Task TestExceptionLogger()
        {
            Mock<IMailSender> emailSenderMock = null;

            var hostBuilder = new WebHostBuilder();
            hostBuilder.ConfigureServices(collection =>
            {
                emailSenderMock = new Mock<IMailSender>();
                emailSenderMock.Setup(p => p.SendEmail(It.IsAny<IEnumerable<MailAddress>>(), It.IsAny<string>(), It.IsAny<string>())).Throws<NotImplementedException>();
                collection.AddSingleton<IMailSender>(provider => emailSenderMock.Object);
            });
            hostBuilder.Configure(app =>
            {
                app.UseExceptionHandler(new List<MailAddress> { new MailAddress {Address = "123@test.com" }, new MailAddress {Address = "246@test.com" } }, "Test Error");
                app.Run(context =>
                {
                    throw new Exception("Server Error");
                });
            });

            using (var testServer = new TestServer(hostBuilder))
            {
                await Assert.ThrowsAsync<Exception>(async () => await testServer.CreateRequest("/").GetAsync());

                emailSenderMock.Verify(
                        p => p.SendEmailAsync(It.IsAny<IEnumerable<MailAddress>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            }
        }
    }
}
