using System;

namespace Db.ConsoleApp
{
	internal class BadConsoleAppArgumentException : Exception
	{
		public BadConsoleAppArgumentException(Exception innerException = null) : this(null, innerException)
		{
		}
		public BadConsoleAppArgumentException(string messageAddendum, Exception innerException = null) : base("The arguments passed to the console application are not valid. " + messageAddendum, innerException)
		{
		}
	}
}
