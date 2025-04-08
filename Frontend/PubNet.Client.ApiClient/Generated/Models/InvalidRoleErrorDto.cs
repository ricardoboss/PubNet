// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System;
namespace PubNet.Client.ApiClient.Generated.Models
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    #pragma warning disable CS1591
    public partial class InvalidRoleErrorDto : ApiException, IAdditionalDataHolder, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The givenRole property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? GivenRole { get; set; }
#nullable restore
#else
        public string GivenRole { get; set; }
#endif
        /// <summary>The primary error message.</summary>
        public override string Message { get => base.Message; }
        /// <summary>The requiredRole property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? RequiredRole { get; set; }
#nullable restore
#else
        public string RequiredRole { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.ApiClient.Generated.Models.InvalidRoleErrorDto"/> and sets the default values.
        /// </summary>
        public InvalidRoleErrorDto()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Models.InvalidRoleErrorDto"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::PubNet.Client.ApiClient.Generated.Models.InvalidRoleErrorDto CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::PubNet.Client.ApiClient.Generated.Models.InvalidRoleErrorDto();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "givenRole", n => { GivenRole = n.GetStringValue(); } },
                { "requiredRole", n => { RequiredRole = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("givenRole", GivenRole);
            writer.WriteStringValue("requiredRole", RequiredRole);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
#pragma warning restore CS0618
