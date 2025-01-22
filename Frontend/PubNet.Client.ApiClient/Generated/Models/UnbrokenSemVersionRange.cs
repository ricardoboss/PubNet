// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace PubNet.Client.ApiClient.Generated.Models
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    #pragma warning disable CS1591
    public partial class UnbrokenSemVersionRange : IAdditionalDataHolder, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The end property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::PubNet.Client.ApiClient.Generated.Models.SemVersion? End { get; set; }
#nullable restore
#else
        public global::PubNet.Client.ApiClient.Generated.Models.SemVersion End { get; set; }
#endif
        /// <summary>The endInclusive property</summary>
        public bool? EndInclusive { get; set; }
        /// <summary>The includeAllPrerelease property</summary>
        public bool? IncludeAllPrerelease { get; set; }
        /// <summary>The start property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::PubNet.Client.ApiClient.Generated.Models.SemVersion? Start { get; set; }
#nullable restore
#else
        public global::PubNet.Client.ApiClient.Generated.Models.SemVersion Start { get; set; }
#endif
        /// <summary>The startInclusive property</summary>
        public bool? StartInclusive { get; set; }
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.ApiClient.Generated.Models.UnbrokenSemVersionRange"/> and sets the default values.
        /// </summary>
        public UnbrokenSemVersionRange()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Models.UnbrokenSemVersionRange"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::PubNet.Client.ApiClient.Generated.Models.UnbrokenSemVersionRange CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::PubNet.Client.ApiClient.Generated.Models.UnbrokenSemVersionRange();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "end", n => { End = n.GetObjectValue<global::PubNet.Client.ApiClient.Generated.Models.SemVersion>(global::PubNet.Client.ApiClient.Generated.Models.SemVersion.CreateFromDiscriminatorValue); } },
                { "endInclusive", n => { EndInclusive = n.GetBoolValue(); } },
                { "includeAllPrerelease", n => { IncludeAllPrerelease = n.GetBoolValue(); } },
                { "start", n => { Start = n.GetObjectValue<global::PubNet.Client.ApiClient.Generated.Models.SemVersion>(global::PubNet.Client.ApiClient.Generated.Models.SemVersion.CreateFromDiscriminatorValue); } },
                { "startInclusive", n => { StartInclusive = n.GetBoolValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<global::PubNet.Client.ApiClient.Generated.Models.SemVersion>("end", End);
            writer.WriteBoolValue("endInclusive", EndInclusive);
            writer.WriteBoolValue("includeAllPrerelease", IncludeAllPrerelease);
            writer.WriteObjectValue<global::PubNet.Client.ApiClient.Generated.Models.SemVersion>("start", Start);
            writer.WriteBoolValue("startInclusive", StartInclusive);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
#pragma warning restore CS0618
