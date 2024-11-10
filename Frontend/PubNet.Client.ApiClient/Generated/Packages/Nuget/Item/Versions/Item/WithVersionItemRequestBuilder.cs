// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using PubNet.Client.ApiClient.Generated.Models;
using PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.Versions.Item.ArchiveNupkg;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.Versions.Item
{
    /// <summary>
    /// Builds and executes requests for operations under \Packages\Nuget\{id}\Versions\{version}
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class WithVersionItemRequestBuilder : BaseRequestBuilder
    {
        /// <summary>The archiveNupkg property</summary>
        public global::PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.Versions.Item.ArchiveNupkg.ArchiveNupkgRequestBuilder ArchiveNupkg
        {
            get => new global::PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.Versions.Item.ArchiveNupkg.ArchiveNupkgRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.Versions.Item.WithVersionItemRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WithVersionItemRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Nuget/{id}/Versions/{version}", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.Versions.Item.WithVersionItemRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WithVersionItemRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Nuget/{id}/Versions/{version}", rawUrl)
        {
        }
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::PubNet.Client.ApiClient.Generated.Models.AuthErrorDto">When receiving a 401 status code</exception>
        /// <exception cref="global::PubNet.Client.ApiClient.Generated.Models.MissingScopeErrorDto">When receiving a 460 status code</exception>
        /// <exception cref="global::PubNet.Client.ApiClient.Generated.Models.InvalidRoleErrorDto">When receiving a 461 status code</exception>
        /// <exception cref="global::PubNet.Client.ApiClient.Generated.Models.InternalServerErrorDto">When receiving a 500 status code</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task DeleteAsync(Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task DeleteAsync(Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToDeleteRequestInformation(requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "401", global::PubNet.Client.ApiClient.Generated.Models.AuthErrorDto.CreateFromDiscriminatorValue },
                { "460", global::PubNet.Client.ApiClient.Generated.Models.MissingScopeErrorDto.CreateFromDiscriminatorValue },
                { "461", global::PubNet.Client.ApiClient.Generated.Models.InvalidRoleErrorDto.CreateFromDiscriminatorValue },
                { "500", global::PubNet.Client.ApiClient.Generated.Models.InternalServerErrorDto.CreateFromDiscriminatorValue },
            };
            await RequestAdapter.SendNoContentAsync(requestInfo, errorMapping, cancellationToken).ConfigureAwait(false);
        }
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::PubNet.Client.ApiClient.Generated.Models.NotFoundErrorDto">When receiving a 404 status code</exception>
        /// <exception cref="global::PubNet.Client.ApiClient.Generated.Models.InternalServerErrorDto">When receiving a 500 status code</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto?> GetAsync(Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto> GetAsync(Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "404", global::PubNet.Client.ApiClient.Generated.Models.NotFoundErrorDto.CreateFromDiscriminatorValue },
                { "500", global::PubNet.Client.ApiClient.Generated.Models.InternalServerErrorDto.CreateFromDiscriminatorValue },
            };
            return await RequestAdapter.SendAsync<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto>(requestInfo, global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto.CreateFromDiscriminatorValue, errorMapping, cancellationToken).ConfigureAwait(false);
        }
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToDeleteRequestInformation(Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToDeleteRequestInformation(Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default)
        {
#endif
            var requestInfo = new RequestInformation(Method.DELETE, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            return requestInfo;
        }
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default)
        {
#endif
            var requestInfo = new RequestInformation(Method.GET, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            return requestInfo;
        }
        /// <summary>
        /// Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        /// </summary>
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.Versions.Item.WithVersionItemRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.Versions.Item.WithVersionItemRequestBuilder WithUrl(string rawUrl)
        {
            return new global::PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.Versions.Item.WithVersionItemRequestBuilder(rawUrl, RequestAdapter);
        }
    }
}
#pragma warning restore CS0618
