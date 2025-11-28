using JoakimsPlaygroundFunctions.Business.DTOs;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace JoakimsPlaygroundFunctions.Business.Interfaces
{
	public interface ILetterManager
	{
		LetterDto Get(Guid id);

		Task<Guid> CreateAsync(CreateLetterDto createLetterDto, string source);
	}
}
