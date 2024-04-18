using System;
using Db.Standards.ErrorHandling;

namespace Db.Application.UseCases.DatabaseQuerying;

public record DatabaseName
{
	public override string ToString() => _encapsulated;
	public static implicit operator string(DatabaseName obj) => obj._encapsulated;

	readonly string _encapsulated;
	DatabaseName(string encapsulated)
	{
		_encapsulated = encapsulated switch
		{
			null => throw ObjectConstructionException.FromInvalidMemberValue(GetType(), nameof(_encapsulated), encapsulated),
			_ => encapsulated
		};
	}

	public static DatabaseName From(string encapsulated)
	{
		try
		{
			return new(encapsulated);
		}
		catch (ObjectConstructionException objectConstructionException)
		{
			AddMethodParametersValuesAsDebuggingInformation(objectConstructionException);
			throw;
		}
		catch (Exception developerMistake)
		{
			var objectConstructionException = ObjectConstructionException.FromDeveloperMistake(typeof(DatabaseName), developerMistake);
			AddMethodParametersValuesAsDebuggingInformation(objectConstructionException);
			throw objectConstructionException;
		}

		void AddMethodParametersValuesAsDebuggingInformation(ObjectConstructionException objectConstructionException)
		{
			objectConstructionException.AddDebuggingInformation(nameof(encapsulated), encapsulated);
		}
	}
}
