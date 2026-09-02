using PubNet.SDK.Abstractions;
using PubNet.SDK.Exceptions;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Extensions;

public static class AuthenticationServiceExtensions
{
	extension(IAuthenticationService authenticationService)
	{
		/// <summary>
		/// Determines if the currently authenticated author has the given <paramref name="username"/>.
		/// </summary>
		/// <remarks>
		/// This is a predicate callers use to decide what to show, so it fails closed: when the current author
		/// cannot be determined - not authenticated, or the API could not be reached - the answer is
		/// <see langword="false"/> rather than an exception. Cancellation still propagates.
		/// </remarks>
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
			catch (PubNetSdkException)
			{
				return false;
			}
		}

		/// <summary>
		/// Determines if the currently authenticated author is an administrator.
		/// </summary>
		/// <remarks>
		/// This is a predicate callers use to decide what to show, so it fails closed: when the current author
		/// cannot be determined - not authenticated, or the API could not be reached - the answer is
		/// <see langword="false"/> rather than an exception. Cancellation still propagates.
		/// </remarks>
		/// <param name="cancellationToken">A token to cancel the asynchronous request</param>
		/// <returns><see langword="true"/> if the currently authenticated author is an administrator, <see langword="false"/> if not</returns>
		public async Task<bool> IsAdminAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				var self = await authenticationService.GetSelfAsync(cancellationToken: cancellationToken);

				return self.Role == Role.Admin;
			}
			catch (PubNetSdkException)
			{
				return false;
			}
		}
	}
}
