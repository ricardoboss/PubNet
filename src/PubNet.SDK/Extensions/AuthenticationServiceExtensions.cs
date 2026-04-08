using PubNet.SDK.Abstractions;

namespace PubNet.SDK.Extensions;

public static class AuthenticationServiceExtensions
{
	extension(IAuthenticationService authenticationService)
	{
		/// <summary>
		/// Determines if the currently authenticated author has the given <paramref name="username"/>.
		/// </summary>
		/// <param name="username">The username to check against</param>
		/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
		/// <returns><see langword="true"/> if the <paramref name="username"/> matches that of the currently authenticated user (case-sensitive), <see langword="false"/> if not</returns>
		public async Task<bool> IsSelfAsync(string username, CancellationToken cancellationToken = default)
		{
			try
			{
				var self = await authenticationService.GetSelfAsync(cancellationToken: cancellationToken);

				return self.UserName == username;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
