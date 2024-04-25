// <auto-generated/>
using Microsoft.Kiota.Abstractions;
using PubNet.Client.Generated.Packages.Dart.Item.Item.AnalysisJson;
using PubNet.Client.Generated.Packages.Dart.Item.Item.ArchiveTarGz;
using PubNet.Client.Generated.Packages.Dart.Item.Item.Docs;
using PubNet.Client.Generated.Packages.Dart.Item.Item.Retract;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
namespace PubNet.Client.Generated.Packages.Dart.Item.Item {
    /// <summary>
    /// Builds and executes requests for operations under \Packages\Dart\{name}\{version}
    /// </summary>
    public class WithVersionItemRequestBuilder : BaseRequestBuilder 
    {
        /// <summary>The analysisJson property</summary>
        public AnalysisJsonRequestBuilder AnalysisJson
        {
            get => new AnalysisJsonRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The archiveTarGz property</summary>
        public ArchiveTarGzRequestBuilder ArchiveTarGz
        {
            get => new ArchiveTarGzRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The Docs property</summary>
        public DocsRequestBuilder Docs
        {
            get => new DocsRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The Retract property</summary>
        public RetractRequestBuilder Retract
        {
            get => new RetractRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new <see cref="WithVersionItemRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WithVersionItemRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Dart/{name}/{version}", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="WithVersionItemRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WithVersionItemRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Dart/{name}/{version}", rawUrl)
        {
        }
    }
}