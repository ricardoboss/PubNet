// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace PubNet.Client.Generated.Models {
    public class PackageListDto : IParsable 
    {
        /// <summary>The packages property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<PackageVersionDtoPackageDto>? Packages { get; set; }
#nullable restore
#else
        public List<PackageVersionDtoPackageDto> Packages { get; set; }
#endif
        /// <summary>The totalHits property</summary>
        public int? TotalHits { get; set; }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="PackageListDto"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static PackageListDto CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new PackageListDto();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                {"packages", n => { Packages = n.GetCollectionOfObjectValues<PackageVersionDtoPackageDto>(PackageVersionDtoPackageDto.CreateFromDiscriminatorValue)?.ToList(); } },
                {"totalHits", n => { TotalHits = n.GetIntValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteCollectionOfObjectValues<PackageVersionDtoPackageDto>("packages", Packages);
            writer.WriteIntValue("totalHits", TotalHits);
        }
    }
}
