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
    public partial class PackageListCollectionDto : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The dart property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::PubNet.Client.Generated.Models.DartPackageListDto? Dart { get; set; }
#nullable restore
#else
        public global::PubNet.Client.Generated.Models.DartPackageListDto Dart { get; set; }
#endif
        /// <summary>The nuget property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::PubNet.Client.Generated.Models.NugetPackageListDto? Nuget { get; set; }
#nullable restore
#else
        public global::PubNet.Client.Generated.Models.NugetPackageListDto Nuget { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::PubNet.Client.Generated.Models.PackageListCollectionDto"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::PubNet.Client.Generated.Models.PackageListCollectionDto CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::PubNet.Client.Generated.Models.PackageListCollectionDto();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "dart", n => { Dart = n.GetObjectValue<global::PubNet.Client.Generated.Models.DartPackageListDto>(global::PubNet.Client.Generated.Models.DartPackageListDto.CreateFromDiscriminatorValue); } },
                { "nuget", n => { Nuget = n.GetObjectValue<global::PubNet.Client.Generated.Models.NugetPackageListDto>(global::PubNet.Client.Generated.Models.NugetPackageListDto.CreateFromDiscriminatorValue); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<global::PubNet.Client.Generated.Models.DartPackageListDto>("dart", Dart);
            writer.WriteObjectValue<global::PubNet.Client.Generated.Models.NugetPackageListDto>("nuget", Nuget);
        }
    }
}
#pragma warning restore CS0618
