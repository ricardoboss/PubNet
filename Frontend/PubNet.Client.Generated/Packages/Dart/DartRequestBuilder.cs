// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using PubNet.Client.Generated.Packages.Dart.Item;
using PubNet.Client.Generated.Packages.Dart.Storage;
using PubNet.Client.Generated.Packages.Dart.Versions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace PubNet.Client.Generated.Packages.Dart
{
    /// <summary>
    /// Builds and executes requests for operations under \Packages\Dart
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.18.0")]
    public partial class DartRequestBuilder : BaseRequestBuilder
    {
        /// <summary>The Storage property</summary>
        public global::PubNet.Client.Generated.Packages.Dart.Storage.StorageRequestBuilder Storage
        {
            get => new global::PubNet.Client.Generated.Packages.Dart.Storage.StorageRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The Versions property</summary>
        public global::PubNet.Client.Generated.Packages.Dart.Versions.VersionsRequestBuilder Versions
        {
            get => new global::PubNet.Client.Generated.Packages.Dart.Versions.VersionsRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>Gets an item from the PubNet.Client.Generated.Packages.Dart.item collection</summary>
        /// <param name="position">Unique identifier of the item</param>
        /// <returns>A <see cref="global::PubNet.Client.Generated.Packages.Dart.Item.WithNameItemRequestBuilder"/></returns>
        public global::PubNet.Client.Generated.Packages.Dart.Item.WithNameItemRequestBuilder this[string position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("name", position);
                return new global::PubNet.Client.Generated.Packages.Dart.Item.WithNameItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.Generated.Packages.Dart.DartRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public DartRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Dart", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.Generated.Packages.Dart.DartRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public DartRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Dart", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618