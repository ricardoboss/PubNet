// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace PubNet.Client.Generated.Models
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.18.0")]
    #pragma warning disable CS1591
    public partial class NugetSearchResultDto : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The data property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::PubNet.Client.Generated.Models.NugetSearchResultHitDto>? Data { get; set; }
#nullable restore
#else
        public List<global::PubNet.Client.Generated.Models.NugetSearchResultHitDto> Data { get; set; }
#endif
        /// <summary>The totalHits property</summary>
        public long? TotalHits { get; set; }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::PubNet.Client.Generated.Models.NugetSearchResultDto"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::PubNet.Client.Generated.Models.NugetSearchResultDto CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::PubNet.Client.Generated.Models.NugetSearchResultDto();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "data", n => { Data = n.GetCollectionOfObjectValues<global::PubNet.Client.Generated.Models.NugetSearchResultHitDto>(global::PubNet.Client.Generated.Models.NugetSearchResultHitDto.CreateFromDiscriminatorValue)?.AsList(); } },
                { "totalHits", n => { TotalHits = n.GetLongValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteCollectionOfObjectValues<global::PubNet.Client.Generated.Models.NugetSearchResultHitDto>("data", Data);
            writer.WriteLongValue("totalHits", TotalHits);
        }
    }
}
#pragma warning restore CS0618
