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
namespace PubNet.Client.ApiClient.Generated.Packages.Dart.Versions.New
{
    /// <summary>
    /// Builds and executes requests for operations under \Packages\Dart\Versions\New
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.18.0")]
    public partial class NewRequestBuilder : BaseRequestBuilder
    {
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.ApiClient.Generated.Packages.Dart.Versions.New.NewRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public NewRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Dart/Versions/New", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.ApiClient.Generated.Packages.Dart.Versions.New.NewRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public NewRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Dart/Versions/New", rawUrl)
        {
        }
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Models.DartArchiveUploadInformationDto"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::PubNet.Client.ApiClient.Generated.Models.GenericErrorDto">When receiving a 401 status code</exception>
        /// <exception cref="global::PubNet.Client.ApiClient.Generated.Models.MissingScopeErrorDto">When receiving a 460 status code</exception>
        /// <exception cref="global::PubNet.Client.ApiClient.Generated.Models.InvalidRoleErrorDto">When receiving a 461 status code</exception>
        /// <exception cref="global::PubNet.Client.ApiClient.Generated.Models.InternalServerErrorDto">When receiving a 500 status code</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::PubNet.Client.ApiClient.Generated.Models.DartArchiveUploadInformationDto?> GetAsync(Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::PubNet.Client.ApiClient.Generated.Models.DartArchiveUploadInformationDto> GetAsync(Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "401", global::PubNet.Client.ApiClient.Generated.Models.GenericErrorDto.CreateFromDiscriminatorValue },
                { "460", global::PubNet.Client.ApiClient.Generated.Models.MissingScopeErrorDto.CreateFromDiscriminatorValue },
                { "461", global::PubNet.Client.ApiClient.Generated.Models.InvalidRoleErrorDto.CreateFromDiscriminatorValue },
                { "500", global::PubNet.Client.ApiClient.Generated.Models.InternalServerErrorDto.CreateFromDiscriminatorValue },
            };
            return await RequestAdapter.SendAsync<global::PubNet.Client.ApiClient.Generated.Models.DartArchiveUploadInformationDto>(requestInfo, global::PubNet.Client.ApiClient.Generated.Models.DartArchiveUploadInformationDto.CreateFromDiscriminatorValue, errorMapping, cancellationToken).ConfigureAwait(false);
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
            requestInfo.Headers.TryAdd("Accept", "application/json, text/plain;q=0.9");
            return requestInfo;
        }
        /// <summary>
        /// Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        /// </summary>
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Packages.Dart.Versions.New.NewRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::PubNet.Client.ApiClient.Generated.Packages.Dart.Versions.New.NewRequestBuilder WithUrl(string rawUrl)
        {
            return new global::PubNet.Client.ApiClient.Generated.Packages.Dart.Versions.New.NewRequestBuilder(rawUrl, RequestAdapter);
        }
    }
}
#pragma warning restore CS0618
