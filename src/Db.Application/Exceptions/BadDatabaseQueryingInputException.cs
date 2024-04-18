using System;

namespace Db.Application.Exceptions;

class BadDatabaseQueryingInputException : Exception
{
	public BadDatabaseQueryingInputException(string message) : base(message)
	{
	}
}
