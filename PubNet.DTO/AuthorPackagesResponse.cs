using PubNet.Models;

namespace PubNet.API.DTO;

public record AuthorPackagesResponse(IEnumerable<Package> Packages);
