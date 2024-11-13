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
    public partial class NugetCatalogEntryDto : IAdditionalDataHolder, IParsable
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
        /// <summary>The dependencyGroups property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDependencyGroupDto>? DependencyGroups { get; set; }
#nullable restore
#else
        public List<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDependencyGroupDto> DependencyGroups { get; set; }
#endif
        /// <summary>The deprecation property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDeprecationDto? Deprecation { get; set; }
#nullable restore
#else
        public global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDeprecationDto Deprecation { get; set; }
#endif
        /// <summary>The description property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Description { get; set; }
#nullable restore
#else
        public string Description { get; set; }
#endif
        /// <summary>The iconUrl property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? IconUrl { get; set; }
#nullable restore
#else
        public string IconUrl { get; set; }
#endif
        /// <summary>The Id property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Id { get; set; }
#nullable restore
#else
        public string Id { get; set; }
#endif
        /// <summary>The language property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Language { get; set; }
#nullable restore
#else
        public string Language { get; set; }
#endif
        /// <summary>The licenseExpression property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? LicenseExpression { get; set; }
#nullable restore
#else
        public string LicenseExpression { get; set; }
#endif
        /// <summary>The licenseUrl property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? LicenseUrl { get; set; }
#nullable restore
#else
        public string LicenseUrl { get; set; }
#endif
        /// <summary>The listed property</summary>
        public bool? Listed { get; set; }
        /// <summary>The minClientVersion property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? MinClientVersion { get; set; }
#nullable restore
#else
        public string MinClientVersion { get; set; }
#endif
        /// <summary>The id property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? NugetCatalogEntryDtoId { get; set; }
#nullable restore
#else
        public string NugetCatalogEntryDtoId { get; set; }
#endif
        /// <summary>The packageContent property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? PackageContent { get; set; }
#nullable restore
#else
        public string PackageContent { get; set; }
#endif
        /// <summary>The projectUrl property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ProjectUrl { get; set; }
#nullable restore
#else
        public string ProjectUrl { get; set; }
#endif
        /// <summary>The published property</summary>
        public DateTimeOffset? Published { get; set; }
        /// <summary>The readmeUrl property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ReadmeUrl { get; set; }
#nullable restore
#else
        public string ReadmeUrl { get; set; }
#endif
        /// <summary>The requireLicenseAcceptance property</summary>
        public bool? RequireLicenseAcceptance { get; set; }
        /// <summary>The summary property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Summary { get; set; }
#nullable restore
#else
        public string Summary { get; set; }
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
        /// <summary>The vulnerabilities property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVulnerabilityDto>? Vulnerabilities { get; set; }
#nullable restore
#else
        public List<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVulnerabilityDto> Vulnerabilities { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::PubNet.Client.ApiClient.Generated.Models.NugetCatalogEntryDto"/> and sets the default values.
        /// </summary>
        public NugetCatalogEntryDto()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Models.NugetCatalogEntryDto"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::PubNet.Client.ApiClient.Generated.Models.NugetCatalogEntryDto CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::PubNet.Client.ApiClient.Generated.Models.NugetCatalogEntryDto();
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
                { "dependencyGroups", n => { DependencyGroups = n.GetCollectionOfObjectValues<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDependencyGroupDto>(global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDependencyGroupDto.CreateFromDiscriminatorValue)?.AsList(); } },
                { "deprecation", n => { Deprecation = n.GetObjectValue<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDeprecationDto>(global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDeprecationDto.CreateFromDiscriminatorValue); } },
                { "description", n => { Description = n.GetStringValue(); } },
                { "iconUrl", n => { IconUrl = n.GetStringValue(); } },
                { "@id", n => { Id = n.GetStringValue(); } },
                { "language", n => { Language = n.GetStringValue(); } },
                { "licenseExpression", n => { LicenseExpression = n.GetStringValue(); } },
                { "licenseUrl", n => { LicenseUrl = n.GetStringValue(); } },
                { "listed", n => { Listed = n.GetBoolValue(); } },
                { "minClientVersion", n => { MinClientVersion = n.GetStringValue(); } },
                { "id", n => { NugetCatalogEntryDtoId = n.GetStringValue(); } },
                { "packageContent", n => { PackageContent = n.GetStringValue(); } },
                { "projectUrl", n => { ProjectUrl = n.GetStringValue(); } },
                { "published", n => { Published = n.GetDateTimeOffsetValue(); } },
                { "readmeUrl", n => { ReadmeUrl = n.GetStringValue(); } },
                { "requireLicenseAcceptance", n => { RequireLicenseAcceptance = n.GetBoolValue(); } },
                { "summary", n => { Summary = n.GetStringValue(); } },
                { "tags", n => { Tags = n.GetStringValue(); } },
                { "title", n => { Title = n.GetStringValue(); } },
                { "version", n => { Version = n.GetStringValue(); } },
                { "vulnerabilities", n => { Vulnerabilities = n.GetCollectionOfObjectValues<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVulnerabilityDto>(global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVulnerabilityDto.CreateFromDiscriminatorValue)?.AsList(); } },
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
            writer.WriteCollectionOfObjectValues<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDependencyGroupDto>("dependencyGroups", DependencyGroups);
            writer.WriteObjectValue<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageDeprecationDto>("deprecation", Deprecation);
            writer.WriteStringValue("description", Description);
            writer.WriteStringValue("iconUrl", IconUrl);
            writer.WriteStringValue("@id", Id);
            writer.WriteStringValue("language", Language);
            writer.WriteStringValue("licenseExpression", LicenseExpression);
            writer.WriteStringValue("licenseUrl", LicenseUrl);
            writer.WriteBoolValue("listed", Listed);
            writer.WriteStringValue("minClientVersion", MinClientVersion);
            writer.WriteStringValue("id", NugetCatalogEntryDtoId);
            writer.WriteStringValue("packageContent", PackageContent);
            writer.WriteStringValue("projectUrl", ProjectUrl);
            writer.WriteDateTimeOffsetValue("published", Published);
            writer.WriteStringValue("readmeUrl", ReadmeUrl);
            writer.WriteBoolValue("requireLicenseAcceptance", RequireLicenseAcceptance);
            writer.WriteStringValue("summary", Summary);
            writer.WriteStringValue("tags", Tags);
            writer.WriteStringValue("title", Title);
            writer.WriteStringValue("version", Version);
            writer.WriteCollectionOfObjectValues<global::PubNet.Client.ApiClient.Generated.Models.NugetPackageVulnerabilityDto>("vulnerabilities", Vulnerabilities);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
#pragma warning restore CS0618
