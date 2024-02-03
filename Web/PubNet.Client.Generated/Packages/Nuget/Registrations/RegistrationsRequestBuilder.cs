// <auto-generated/>
using ApiSdk.Packages.Nuget.Registrations.Item;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
namespace ApiSdk.Packages.Nuget.Registrations {
    /// <summary>
    /// Builds and executes requests for operations under \Packages\Nuget\Registrations
    /// </summary>
    public class RegistrationsRequestBuilder : BaseRequestBuilder {
        /// <summary>Gets an item from the ApiSdk.Packages.Nuget.Registrations.item collection</summary>
        /// <param name="position">Unique identifier of the item</param>
        public RegistrationsItemRequestBuilder this[string position] { get {
            var urlTplParams = new Dictionary<string, object>(PathParameters);
            urlTplParams.Add("id", position);
            return new RegistrationsItemRequestBuilder(urlTplParams, RequestAdapter);
        } }
        /// <summary>
        /// Instantiates a new RegistrationsRequestBuilder and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public RegistrationsRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Nuget/Registrations", pathParameters) {
        }
        /// <summary>
        /// Instantiates a new RegistrationsRequestBuilder and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public RegistrationsRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Packages/Nuget/Registrations", rawUrl) {
        }
    }
}
