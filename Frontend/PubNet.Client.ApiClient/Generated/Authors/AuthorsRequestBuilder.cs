// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using PubNet.Client.ApiClient.Generated.Authors.Item;
using PubNet.Client.ApiClient.Generated.Authors.Search;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace PubNet.Client.ApiClient.Generated.Authors
{
    /// <summary>
    /// Builds and executes requests for operations under \Authors
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class AuthorsRequestBuilder : BaseRequestBuilder
    {
        /// <summary>The Search property</summary>
        public global::PubNet.Client.ApiClient.Generated.Authors.Search.SearchRequestBuilder Search
        {
            get => new global::PubNet.Client.ApiClient.Generated.Authors.Search.SearchRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>Gets an item from the PubNet.Client.ApiClient.Generated.Authors.item collection</summary>
        /// <param name="position">Unique identifier of the item</param>
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Authors.Item.WithUsernameItemRequestBuilder"/></returns>
        public global::PubNet.Client.ApiClient.Generated.Authors.Item.WithUsernameItemRequestBuilder this[string position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("username", position);
                return new global::PubNet.Client.ApiClient.Generated.Authors.Item.WithUsernameItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.ApiClient.Generated.Authors.AuthorsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public AuthorsRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Authors", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.ApiClient.Generated.Authors.AuthorsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public AuthorsRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Authors", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618
