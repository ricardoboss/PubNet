using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Authorization;
using PubNet.API.DTO.Admin;
using PubNet.API.DTO.Authors;
using PubNet.API.DTO.Authors.Errors;
using PubNet.API.DTO.Settings;
using PubNet.API.DTO.Settings.Errors;
using PubNet.API.Interfaces;
using PubNet.API.Services;
using PubNet.Database;

namespace PubNet.API.Controllers;

/// <summary>
/// Configures the instance and manages its authors. Packages are managed through the regular
/// <see cref="PackagesController"/> endpoints, which accept admins for packages they do not own.
/// </summary>
[ApiController]
[Route("admin")]
[Authorize(Policies.Admin)]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class AdminController(
	ISettingsService settingsService,
	IAuthorRoleService authorRoleService,
	PubNetContext db,
	ILogger<AdminController> logger
) : BaseController
{
	[HttpGet("settings")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SettingDescriptorDto>))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public IReadOnlyList<SettingDescriptorDto> GetSettings()
	{
		return settingsService.GetAll();
	}

	[HttpPatch("settings")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(PubNetStatusCodes.Status490UnknownSetting, Type = typeof(UnknownSettingErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status491InvalidSettingValue, Type = typeof(InvalidSettingValueErrorDto))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> UpdateSettingsAsync([FromBody] UpdateSettingsRequestDto dto,
		CancellationToken cancellationToken = default)
	{
		try
		{
			await settingsService.ApplyAsync(dto.Settings, cancellationToken);
		}
		catch (UnknownSettingException e)
		{
			return Error<UnknownSettingErrorDto>(PubNetStatusCodes.Status490UnknownSetting,
				$"'{e.Key}' is not a configurable setting");
		}
		catch (InvalidSettingValueException e)
		{
			return Error<InvalidSettingValueErrorDto>(PubNetStatusCodes.Status491InvalidSettingValue,
				$"The value for '{e.Key}' is invalid: {e.Reason}");
		}

		return NoContent();
	}

	[HttpGet("authors")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdminAuthorsResponseDto))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<AdminAuthorsResponseDto> GetAuthorsAsync(CancellationToken cancellationToken = default)
	{
		var authors = await db.Authors
			.OrderByDescending(a => a.RegisteredAtUtc)
			.ToListAsync(cancellationToken);

		return new(authors.Select(a => AuthorDto.FromAuthor(a, true, true)!));
	}

	[HttpPost("authors/{username}/role")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status404NotFound, Type = typeof(AuthorNotFoundErrorDto))]
	[ProducesResponseType(PubNetStatusCodes.Status480LastAdmin, Type = typeof(LastAdminErrorDto))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> SetRoleAsync(string username, [FromBody] SetAuthorRoleRequestDto dto,
		CancellationToken cancellationToken = default)
	{
		var author = await db.Authors.FirstOrDefaultAsync(a => a.UserName == username, cancellationToken);
		if (author is null)
			return Error<AuthorNotFoundErrorDto>(PubNetStatusCodes.Status404NotFound,
				"Author not found: " + username);

		try
		{
			await authorRoleService.SetRoleAsync(author, dto.Role, cancellationToken);
		}
		catch (LastAdminException)
		{
			logger.LogWarning("Refused to demote {Username} (author {AuthorId}), who is the last admin",
				author.UserName, author.Id);

			return Error<LastAdminErrorDto>(PubNetStatusCodes.Status480LastAdmin);
		}

		return Ok(AuthorDto.FromAuthor(author, true, true));
	}
}
