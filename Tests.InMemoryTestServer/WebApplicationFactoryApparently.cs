using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Tests.FileSystem;

public class WebApplicationFactoryApparently
{
	class LogLessInMemoryServer : Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<ClassThatEncapsulatesAWebApplication>
	{
		protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
		{
			builder.ConfigureLogging(logging => logging.ClearProviders());
		}
	}
	[Test]
	public async Task Can_Use_Class_Encapsulating_WebApplication_As_GenericType()
	{
		var wa = new LogLessInMemoryServer();
		var client = wa.CreateClient();
		var r = await client.GetAsync("/");

		Assert.That((await r.Content.ReadAsStringAsync()), Is.EqualTo("Hello World!"));
	}
}
