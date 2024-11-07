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
    public partial class DartPackageVersionDto : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The archiveSha256 property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ArchiveSha256 { get; set; }
#nullable restore
#else
        public string ArchiveSha256 { get; set; }
#endif
        /// <summary>The archiveUrl property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ArchiveUrl { get; set; }
#nullable restore
#else
        public string ArchiveUrl { get; set; }
#endif
        /// <summary>The publishedAt property</summary>
        public DateTimeOffset? PublishedAt { get; set; }
        /// <summary>The pubspec property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::PubNet.Client.ApiClient.Generated.Models.PubSpec? Pubspec { get; set; }
#nullable restore
#else
        public global::PubNet.Client.ApiClient.Generated.Models.PubSpec Pubspec { get; set; }
#endif
        /// <summary>The retracted property</summary>
        public bool? Retracted { get; set; }
        /// <summary>The version property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Version { get; set; }
#nullable restore
#else
        public string Version { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Models.DartPackageVersionDto"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::PubNet.Client.ApiClient.Generated.Models.DartPackageVersionDto CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::PubNet.Client.ApiClient.Generated.Models.DartPackageVersionDto();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "archiveSha256", n => { ArchiveSha256 = n.GetStringValue(); } },
                { "archiveUrl", n => { ArchiveUrl = n.GetStringValue(); } },
                { "publishedAt", n => { PublishedAt = n.GetDateTimeOffsetValue(); } },
                { "pubspec", n => { Pubspec = n.GetObjectValue<global::PubNet.Client.ApiClient.Generated.Models.PubSpec>(global::PubNet.Client.ApiClient.Generated.Models.PubSpec.CreateFromDiscriminatorValue); } },
                { "retracted", n => { Retracted = n.GetBoolValue(); } },
                { "version", n => { Version = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("archiveSha256", ArchiveSha256);
            writer.WriteStringValue("archiveUrl", ArchiveUrl);
            writer.WriteDateTimeOffsetValue("publishedAt", PublishedAt);
            writer.WriteObjectValue<global::PubNet.Client.ApiClient.Generated.Models.PubSpec>("pubspec", Pubspec);
            writer.WriteBoolValue("retracted", Retracted);
            writer.WriteStringValue("version", Version);
        }
    }
}
#pragma warning restore CS0618
