using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.API.DTO.Authors;

namespace PubNet.API.Controllers.Authors;

[Route("Authors")]
[Tags("Authors")]
public class AuthorsRootController(IAuthorDao authorDao) : AuthorsController
{
	[HttpGet("Search")]
	public Task<AuthorListDto> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		return authorDao.SearchAsync(q, skip, take, cancellationToken);
	}
}
