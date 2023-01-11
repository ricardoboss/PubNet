using PubNet.API.Models;

namespace PubNet.API.DTO;

public record PackagesResponse(IEnumerable<Package> Packages);