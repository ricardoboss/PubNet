// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using PubNet.Client.ApiClient.Generated.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.IndexJson
{
    /// <summary>
    /// Builds and executes requests for operations under \Packages\Nuget\{id}\index.json
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class IndexJsonRequestBuilder : BaseRequestBuilder
    {
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.IndexJson.IndexJsonRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public IndexJsonRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Nuget/{id}/index.json", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.IndexJson.IndexJsonRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public IndexJsonRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Nuget/{id}/index.json", rawUrl)
        {
        }
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Models.NugetPackageIndexDto"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::PubNet.Client.ApiClient.Generated.Models.InternalServerErrorDto">When receiving a 500 status code</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageIndexDto?> GetAsync(Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageIndexDto> GetAsync(Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "500", global::PubNet.Client.ApiClient.Generated.Models.InternalServerErrorDto.CreateFromDiscriminatorValue },
            };
            return await RequestAdapter.SendAsync<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageIndexDto>(requestInfo, global::PubNet.Client.ApiClient.Generated.Models.NugetPackageIndexDto.CreateFromDiscriminatorValue, errorMapping, cancellationToken).ConfigureAwait(false);
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
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.IndexJson.IndexJsonRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.IndexJson.IndexJsonRequestBuilder WithUrl(string rawUrl)
        {
            return new global::PubNet.Client.ApiClient.Generated.Packages.Nuget.Item.IndexJson.IndexJsonRequestBuilder(rawUrl, RequestAdapter);
        }
    }
}
#pragma warning restore CS0618
