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
    public partial class NugetPackageDto : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The author property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Author { get; set; }
#nullable restore
#else
        public string Author { get; set; }
#endif
        /// <summary>The latest property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto? Latest { get; set; }
#nullable restore
#else
        public global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto Latest { get; set; }
#endif
        /// <summary>The name property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Name { get; set; }
#nullable restore
#else
        public string Name { get; set; }
#endif
        /// <summary>The type property</summary>
        public int? Type { get; set; }
        /// <summary>The versions property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto>? Versions { get; set; }
#nullable restore
#else
        public List<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto> Versions { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDto"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDto CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDto();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "author", n => { Author = n.GetStringValue(); } },
                { "latest", n => { Latest = n.GetObjectValue<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto>(global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto.CreateFromDiscriminatorValue); } },
                { "name", n => { Name = n.GetStringValue(); } },
                { "type", n => { Type = n.GetIntValue(); } },
                { "versions", n => { Versions = n.GetCollectionOfObjectValues<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto>(global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto.CreateFromDiscriminatorValue)?.AsList(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("author", Author);
            writer.WriteObjectValue<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto>("latest", Latest);
            writer.WriteStringValue("name", Name);
            writer.WriteIntValue("type", Type);
            writer.WriteCollectionOfObjectValues<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto>("versions", Versions);
        }
    }
}
#pragma warning restore CS0618
