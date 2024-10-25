using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.API.Attributes;
using PubNet.API.DTO.Authors;
using PubNet.Auth;

namespace PubNet.API.Controllers.Authors;

[Route("Authors")]
[Authorize]
[Tags("Authors")]
public class AuthorsRootController(IAuthorDao authorDao) : AuthorsController
{
	[HttpGet("Search")]
	[RequireScope(Scopes.Authors.Search)]
	public Task<AuthorListDto> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		return authorDao.SearchAsync(q, skip, take, cancellationToken);
	}
}
