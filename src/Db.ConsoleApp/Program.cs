using System;
using Db;
using Db.Application;
using Db.ConsoleApp.Commands;
using Microsoft.Extensions.DependencyInjection;

try
{
	Console.OutputEncoding = System.Text.Encoding.UTF8;

	var serviceCollection = new ServiceCollection()
		.AddApplicationUseCases()
		.AddServiceProviders()
		.BuildServiceProvider();

	var command = new CommandFactory(serviceCollection)
		.CreateCommandFromCommandLineArgs(args);

	Console.WriteLine(await command.Run());
}
catch (Exception _)
{
	Console.WriteLine(_.Message);
	Console.WriteLine(_.StackTrace);
}

    public class MyFirstClass
    {
        public string Option1 { get; set; }
        public int Option2 { get; set; }
    }

    public class MySecondClass
    {
        public string SettingOne { get; set; }
        public int SettingTwo { get; set; }
    }
