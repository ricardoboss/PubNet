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
    public partial class NugetPackageVersionDto3 : IAdditionalDataHolder, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The authors property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Authors { get; set; }
#nullable restore
#else
        public string Authors { get; set; }
#endif
        /// <summary>The copyright property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Copyright { get; set; }
#nullable restore
#else
        public string Copyright { get; set; }
#endif
        /// <summary>The dependencyGroups property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDependencyGroupDto>? DependencyGroups { get; set; }
#nullable restore
#else
        public List<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDependencyGroupDto> DependencyGroups { get; set; }
#endif
        /// <summary>The description property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Description { get; set; }
#nullable restore
#else
        public string Description { get; set; }
#endif
        /// <summary>The iconFile property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? IconFile { get; set; }
#nullable restore
#else
        public string IconFile { get; set; }
#endif
        /// <summary>The iconUrl property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? IconUrl { get; set; }
#nullable restore
#else
        public string IconUrl { get; set; }
#endif
        /// <summary>The packageId property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? PackageId { get; set; }
#nullable restore
#else
        public string PackageId { get; set; }
#endif
        /// <summary>The projectUrl property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ProjectUrl { get; set; }
#nullable restore
#else
        public string ProjectUrl { get; set; }
#endif
        /// <summary>The publishedAt property</summary>
        public DateTimeOffset? PublishedAt { get; set; }
        /// <summary>The readmeFile property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ReadmeFile { get; set; }
#nullable restore
#else
        public string ReadmeFile { get; set; }
#endif
        /// <summary>The repositoryMetadata property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::PubNet.Client.ApiClient.Generated.Models.NugetRepositoryMetadataDto? RepositoryMetadata { get; set; }
#nullable restore
#else
        public global::PubNet.Client.ApiClient.Generated.Models.NugetRepositoryMetadataDto RepositoryMetadata { get; set; }
#endif
        /// <summary>The tags property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Tags { get; set; }
#nullable restore
#else
        public string Tags { get; set; }
#endif
        /// <summary>The title property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Title { get; set; }
#nullable restore
#else
        public string Title { get; set; }
#endif
        /// <summary>The version property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Version { get; set; }
#nullable restore
#else
        public string Version { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto3"/> and sets the default values.
        /// </summary>
        public NugetPackageVersionDto3()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto3"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto3 CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVersionDto3();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "authors", n => { Authors = n.GetStringValue(); } },
                { "copyright", n => { Copyright = n.GetStringValue(); } },
                { "dependencyGroups", n => { DependencyGroups = n.GetCollectionOfObjectValues<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDependencyGroupDto>(global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDependencyGroupDto.CreateFromDiscriminatorValue)?.AsList(); } },
                { "description", n => { Description = n.GetStringValue(); } },
                { "iconFile", n => { IconFile = n.GetStringValue(); } },
                { "iconUrl", n => { IconUrl = n.GetStringValue(); } },
                { "packageId", n => { PackageId = n.GetStringValue(); } },
                { "projectUrl", n => { ProjectUrl = n.GetStringValue(); } },
                { "publishedAt", n => { PublishedAt = n.GetDateTimeOffsetValue(); } },
                { "readmeFile", n => { ReadmeFile = n.GetStringValue(); } },
                { "repositoryMetadata", n => { RepositoryMetadata = n.GetObjectValue<global::PubNet.Client.ApiClient.Generated.Models.NugetRepositoryMetadataDto>(global::PubNet.Client.ApiClient.Generated.Models.NugetRepositoryMetadataDto.CreateFromDiscriminatorValue); } },
                { "tags", n => { Tags = n.GetStringValue(); } },
                { "title", n => { Title = n.GetStringValue(); } },
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
            writer.WriteStringValue("authors", Authors);
            writer.WriteStringValue("copyright", Copyright);
            writer.WriteCollectionOfObjectValues<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDependencyGroupDto>("dependencyGroups", DependencyGroups);
            writer.WriteStringValue("description", Description);
            writer.WriteStringValue("iconFile", IconFile);
            writer.WriteStringValue("iconUrl", IconUrl);
            writer.WriteStringValue("packageId", PackageId);
            writer.WriteStringValue("projectUrl", ProjectUrl);
            writer.WriteDateTimeOffsetValue("publishedAt", PublishedAt);
            writer.WriteStringValue("readmeFile", ReadmeFile);
            writer.WriteObjectValue<global::PubNet.Client.ApiClient.Generated.Models.NugetRepositoryMetadataDto>("repositoryMetadata", RepositoryMetadata);
            writer.WriteStringValue("tags", Tags);
            writer.WriteStringValue("title", Title);
            writer.WriteStringValue("version", Version);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
#pragma warning restore CS0618
