using System;
using Db.Standards.ErrorHandling;

namespace Db.Application.UseCases.DatabaseQuerying;

public record DeploymentEnvironment
{
	public override string ToString() => _encapsulated;
	public static implicit operator string(DeploymentEnvironment obj) => obj._encapsulated;

	readonly string _encapsulated;
	DeploymentEnvironment(string encapsulated)
	{
		_encapsulated = encapsulated switch
		{
			null => throw ObjectConstructionException.FromInvalidMemberValue(GetType(), nameof(_encapsulated), encapsulated),
			_ => encapsulated
		};
	}

	public static DeploymentEnvironment From(string encapsulated)
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
			var objectConstructionException = ObjectConstructionException.FromDeveloperMistake(typeof(DeploymentEnvironment), developerMistake);
			AddMethodParametersValuesAsDebuggingInformation(objectConstructionException);
			throw objectConstructionException;
		}

		void AddMethodParametersValuesAsDebuggingInformation(ObjectConstructionException objectConstructionException)
		{
			objectConstructionException.AddDebuggingInformation(nameof(encapsulated), encapsulated);
		}
	}
}
