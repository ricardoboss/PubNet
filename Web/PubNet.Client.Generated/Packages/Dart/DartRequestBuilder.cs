// <auto-generated/>
using Microsoft.Kiota.Abstractions;
using PubNet.Client.Generated.Packages.Dart.Item;
using PubNet.Client.Generated.Packages.Dart.Storage;
using PubNet.Client.Generated.Packages.Dart.Versions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
namespace PubNet.Client.Generated.Packages.Dart {
    /// <summary>
    /// Builds and executes requests for operations under \Packages\Dart
    /// </summary>
    public class DartRequestBuilder : BaseRequestBuilder 
    {
        /// <summary>The Storage property</summary>
        public StorageRequestBuilder Storage
        {
            get => new StorageRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The Versions property</summary>
        public VersionsRequestBuilder Versions
        {
            get => new VersionsRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>Gets an item from the PubNet.Client.Generated.Packages.Dart.item collection</summary>
        /// <param name="position">Unique identifier of the item</param>
        /// <returns>A <see cref="WithNameItemRequestBuilder"/></returns>
        public WithNameItemRequestBuilder this[string position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("name", position);
                return new WithNameItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="DartRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public DartRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Dart", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="DartRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public DartRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Dart", rawUrl)
        {
        }
    }
}