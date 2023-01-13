using PubNet.Models;

namespace PubNet.API.DTO;

public record PackagesResponse(IEnumerable<Package> Packages);