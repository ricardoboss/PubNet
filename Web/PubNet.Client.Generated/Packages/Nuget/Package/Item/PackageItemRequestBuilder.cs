// <auto-generated/>
using ApiSdk.Packages.Nuget.Package.Item.IndexJson;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
namespace ApiSdk.Packages.Nuget.Package.Item {
    /// <summary>
    /// Builds and executes requests for operations under \Packages\Nuget\Package\{id}
    /// </summary>
    public class PackageItemRequestBuilder : BaseRequestBuilder {
        /// <summary>The indexJson property</summary>
        public IndexJsonRequestBuilder IndexJson { get =>
            new IndexJsonRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new PackageItemRequestBuilder and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public PackageItemRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Nuget/Package/{id}", pathParameters) {
        }
        /// <summary>
        /// Instantiates a new PackageItemRequestBuilder and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public PackageItemRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Nuget/Package/{id}", rawUrl) {
        }
    }
}
