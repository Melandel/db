using System.Threading.Tasks;
using Db.Application.UseCases;

namespace Db.ConsoleApp.Commands;

abstract class Command
{
	public abstract Task<string> Run();
}

abstract class Command<TUseCase, TIn, TOut>
: Command
	where TIn : UseCaseInput
	where TOut : UseCaseOutput
	where TUseCase : IUseCase<TIn, TOut>
{
	protected TUseCase UseCase;
	protected TIn UseCaseInput;

	protected Command(TUseCase useCase, TIn useCaseInput)
	{
		UseCase = useCase;
		UseCaseInput = useCaseInput;
	}

	public override async Task<string> Run()
	{
		TOut useCaseOutput = await UseCase.Process(UseCaseInput);
		return Format(useCaseOutput);
	}
	protected abstract string Format(TOut executionOutput);
}
