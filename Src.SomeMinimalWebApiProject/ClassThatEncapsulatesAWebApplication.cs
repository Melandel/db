class ClassThatEncapsulatesAWebApplication
{
	readonly WebApplication _wa;
	public ClassThatEncapsulatesAWebApplication(WebApplication wa)
	{
		_wa = wa;
	}

	public static ClassThatEncapsulatesAWebApplication Create()
	{
		var builder = WebApplication.CreateBuilder();
		var app = builder.Build();

		app.MapGet("/", () => "Hello World!");

		return new(app);
	}

	public void Run()
	{
		_wa.Run();
	}
}
