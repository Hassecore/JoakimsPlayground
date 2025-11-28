using JoakimsPlaygroundFunctions.Business.DTOs;
using JoakimsPlaygroundFunctions.Business.Interfaces;
using JoakimsPlaygroundFunctions.Data.Entities;
using JoakimsPlaygroundFunctions.Data.Repositories;

namespace JoakimsPlaygroundFunctions.Business.Classes
{
	public class LetterManager : ILetterManager
	{
		private readonly IRepository _repository;

		public LetterManager(IRepository repository)
		{
			_repository = repository;
		}

		public LetterDto Get(Guid id)
		{
			var letter = _repository.Get<Letter>(l => l.Id == id);

			return new LetterDto
			{
				Id = letter.Id,
				Sender = letter.Sender,
				Content = letter.Content,
				PS = letter.PS
			};
		}

		public async Task<Guid> CreateAsync(CreateLetterDto createLetterDto, string source)
		{
			var letter = new Letter
			{
				Id = Guid.NewGuid(),
				Sender = createLetterDto.Sender,
				Content = createLetterDto.Content,
				PS = createLetterDto.PS,
				Source = source,
			};

			var id = await _repository.CreateAsync(letter);

			return id;
		}
	}
}
