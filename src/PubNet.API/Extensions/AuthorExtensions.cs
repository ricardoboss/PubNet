using PubNet.Database.Models;

namespace PubNet.API.Extensions;

public static class AuthorExtensions
{
	extension(Author author)
	{
		public bool IsAdmin => author.Role == Role.Admin;

		/// <summary>
		/// Whether this author may discontinue, retract or delete the given package.
		/// </summary>
		public bool CanManage(Package package)
		{
			return author.IsAdmin || author.Id == package.AuthorId;
		}
	}
}
