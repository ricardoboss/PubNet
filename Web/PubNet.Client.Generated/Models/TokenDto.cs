// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace PubNet.Client.Generated.Models {
    public class TokenDto : IParsable 
    {
        /// <summary>The browser property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Browser { get; set; }
#nullable restore
#else
        public string Browser { get; set; }
#endif
        /// <summary>The createdAtUtc property</summary>
        public DateTimeOffset? CreatedAtUtc { get; set; }
        /// <summary>The deviceType property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? DeviceType { get; set; }
#nullable restore
#else
        public string DeviceType { get; set; }
#endif
        /// <summary>The expiresAtUtc property</summary>
        public DateTimeOffset? ExpiresAtUtc { get; set; }
        /// <summary>The id property</summary>
        public Guid? Id { get; set; }
        /// <summary>The ipAddress property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? IpAddress { get; set; }
#nullable restore
#else
        public string IpAddress { get; set; }
#endif
        /// <summary>The name property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Name { get; set; }
#nullable restore
#else
        public string Name { get; set; }
#endif
        /// <summary>The platform property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Platform { get; set; }
#nullable restore
#else
        public string Platform { get; set; }
#endif
        /// <summary>The revokedAtUtc property</summary>
        public DateTimeOffset? RevokedAtUtc { get; set; }
        /// <summary>The scopes property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<string>? Scopes { get; set; }
#nullable restore
#else
        public List<string> Scopes { get; set; }
#endif
        /// <summary>The userAgent property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? UserAgent { get; set; }
#nullable restore
#else
        public string UserAgent { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="TokenDto"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static TokenDto CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new TokenDto();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                {"browser", n => { Browser = n.GetStringValue(); } },
                {"createdAtUtc", n => { CreatedAtUtc = n.GetDateTimeOffsetValue(); } },
                {"deviceType", n => { DeviceType = n.GetStringValue(); } },
                {"expiresAtUtc", n => { ExpiresAtUtc = n.GetDateTimeOffsetValue(); } },
                {"id", n => { Id = n.GetGuidValue(); } },
                {"ipAddress", n => { IpAddress = n.GetStringValue(); } },
                {"name", n => { Name = n.GetStringValue(); } },
                {"platform", n => { Platform = n.GetStringValue(); } },
                {"revokedAtUtc", n => { RevokedAtUtc = n.GetDateTimeOffsetValue(); } },
                {"scopes", n => { Scopes = n.GetCollectionOfPrimitiveValues<string>()?.ToList(); } },
                {"userAgent", n => { UserAgent = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("browser", Browser);
            writer.WriteDateTimeOffsetValue("createdAtUtc", CreatedAtUtc);
            writer.WriteStringValue("deviceType", DeviceType);
            writer.WriteDateTimeOffsetValue("expiresAtUtc", ExpiresAtUtc);
            writer.WriteGuidValue("id", Id);
            writer.WriteStringValue("ipAddress", IpAddress);
            writer.WriteStringValue("name", Name);
            writer.WriteStringValue("platform", Platform);
            writer.WriteDateTimeOffsetValue("revokedAtUtc", RevokedAtUtc);
            writer.WriteCollectionOfPrimitiveValues<string>("scopes", Scopes);
            writer.WriteStringValue("userAgent", UserAgent);
        }
    }
}
