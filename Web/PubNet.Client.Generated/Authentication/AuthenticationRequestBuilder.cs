// <auto-generated/>
using Microsoft.Kiota.Abstractions;
using PubNet.Client.Generated.Authentication.Account;
using PubNet.Client.Generated.Authentication.LoginToken;
using PubNet.Client.Generated.Authentication.PersonalAccessToken;
using PubNet.Client.Generated.Authentication.RegistrationsOpen;
using PubNet.Client.Generated.Authentication.Self;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
namespace PubNet.Client.Generated.Authentication {
    /// <summary>
    /// Builds and executes requests for operations under \Authentication
    /// </summary>
    public class AuthenticationRequestBuilder : BaseRequestBuilder {
        /// <summary>The Account property</summary>
        public AccountRequestBuilder Account { get =>
            new AccountRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The LoginToken property</summary>
        public LoginTokenRequestBuilder LoginToken { get =>
            new LoginTokenRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The PersonalAccessToken property</summary>
        public PersonalAccessTokenRequestBuilder PersonalAccessToken { get =>
            new PersonalAccessTokenRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The RegistrationsOpen property</summary>
        public RegistrationsOpenRequestBuilder RegistrationsOpen { get =>
            new RegistrationsOpenRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The Self property</summary>
        public SelfRequestBuilder Self { get =>
            new SelfRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new AuthenticationRequestBuilder and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public AuthenticationRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Authentication", pathParameters) {
        }
        /// <summary>
        /// Instantiates a new AuthenticationRequestBuilder and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public AuthenticationRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Authentication", rawUrl) {
        }
    }
}
