// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace PubNet.Client.ApiClient.Generated.Models
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.18.0")]
    #pragma warning disable CS1591
    public partial class DartPackageListDto : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The packages property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::PubNet.Client.ApiClient.Generated.Models.DartPackageDto>? Packages { get; set; }
#nullable restore
#else
        public List<global::PubNet.Client.ApiClient.Generated.Models.DartPackageDto> Packages { get; set; }
#endif
        /// <summary>The totalHits property</summary>
        public int? TotalHits { get; set; }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Models.DartPackageListDto"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::PubNet.Client.ApiClient.Generated.Models.DartPackageListDto CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::PubNet.Client.ApiClient.Generated.Models.DartPackageListDto();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "packages", n => { Packages = n.GetCollectionOfObjectValues<global::PubNet.Client.ApiClient.Generated.Models.DartPackageDto>(global::PubNet.Client.ApiClient.Generated.Models.DartPackageDto.CreateFromDiscriminatorValue)?.AsList(); } },
                { "totalHits", n => { TotalHits = n.GetIntValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteCollectionOfObjectValues<global::PubNet.Client.ApiClient.Generated.Models.DartPackageDto>("packages", Packages);
            writer.WriteIntValue("totalHits", TotalHits);
        }
    }
}
#pragma warning restore CS0618