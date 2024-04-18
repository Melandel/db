using System.Threading.Tasks;

namespace Db.Application.UseCases;

public interface IUseCase<TInput, TOutput>
	where TInput : UseCaseInput
	where TOutput : UseCaseOutput
{
	Task<TOutput> Process(TInput input);
}

