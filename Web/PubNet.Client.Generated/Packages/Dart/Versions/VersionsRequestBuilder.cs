// <auto-generated/>
using Microsoft.Kiota.Abstractions;
using PubNet.Client.Generated.Packages.Dart.Versions.New;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
namespace PubNet.Client.Generated.Packages.Dart.Versions {
    /// <summary>
    /// Builds and executes requests for operations under \Packages\Dart\Versions
    /// </summary>
    public class VersionsRequestBuilder : BaseRequestBuilder 
    {
        /// <summary>The New property</summary>
        public NewRequestBuilder New
        {
            get => new NewRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new <see cref="VersionsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public VersionsRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Dart/Versions", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="VersionsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public VersionsRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Dart/Versions", rawUrl)
        {
        }
    }
}
