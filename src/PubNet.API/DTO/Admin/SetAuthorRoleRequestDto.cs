using JetBrains.Annotations;
using PubNet.Database.Models;

namespace PubNet.API.DTO.Admin;

[PublicAPI]
public record SetAuthorRoleRequestDto(Role Role);
