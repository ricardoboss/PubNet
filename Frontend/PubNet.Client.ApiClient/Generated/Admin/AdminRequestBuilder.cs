// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using PubNet.Client.ApiClient.Generated.Admin.Identities;
using PubNet.Client.ApiClient.Generated.Admin.Identity;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace PubNet.Client.ApiClient.Generated.Admin
{
    /// <summary>
    /// Builds and executes requests for operations under \Admin
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class AdminRequestBuilder : BaseRequestBuilder
    {
        /// <summary>The Identities property</summary>
        public global::PubNet.Client.ApiClient.Generated.Admin.Identities.IdentitiesRequestBuilder Identities
        {
            get => new global::PubNet.Client.ApiClient.Generated.Admin.Identities.IdentitiesRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The Identity property</summary>
        public global::PubNet.Client.ApiClient.Generated.Admin.Identity.IdentityRequestBuilder Identity
        {
            get => new global::PubNet.Client.ApiClient.Generated.Admin.Identity.IdentityRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.ApiClient.Generated.Admin.AdminRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public AdminRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Admin", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.ApiClient.Generated.Admin.AdminRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public AdminRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Admin", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618
