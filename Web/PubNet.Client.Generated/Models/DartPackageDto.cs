// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace PubNet.Client.Generated.Models {
    public class DartPackageDto : IParsable 
    {
        /// <summary>The isDiscontinued property</summary>
        public bool? IsDiscontinued { get; set; }
        /// <summary>The latest property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public DartPackageVersionDto? Latest { get; set; }
#nullable restore
#else
        public DartPackageVersionDto Latest { get; set; }
#endif
        /// <summary>The name property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Name { get; set; }
#nullable restore
#else
        public string Name { get; set; }
#endif
        /// <summary>The replacedBy property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ReplacedBy { get; set; }
#nullable restore
#else
        public string ReplacedBy { get; set; }
#endif
        /// <summary>The versions property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<DartPackageVersionDto>? Versions { get; set; }
#nullable restore
#else
        public List<DartPackageVersionDto> Versions { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="DartPackageDto"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static DartPackageDto CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new DartPackageDto();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                {"isDiscontinued", n => { IsDiscontinued = n.GetBoolValue(); } },
                {"latest", n => { Latest = n.GetObjectValue<DartPackageVersionDto>(DartPackageVersionDto.CreateFromDiscriminatorValue); } },
                {"name", n => { Name = n.GetStringValue(); } },
                {"replacedBy", n => { ReplacedBy = n.GetStringValue(); } },
                {"versions", n => { Versions = n.GetCollectionOfObjectValues<DartPackageVersionDto>(DartPackageVersionDto.CreateFromDiscriminatorValue)?.ToList(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteBoolValue("isDiscontinued", IsDiscontinued);
            writer.WriteObjectValue<DartPackageVersionDto>("latest", Latest);
            writer.WriteStringValue("name", Name);
            writer.WriteStringValue("replacedBy", ReplacedBy);
            writer.WriteCollectionOfObjectValues<DartPackageVersionDto>("versions", Versions);
        }
    }
}
