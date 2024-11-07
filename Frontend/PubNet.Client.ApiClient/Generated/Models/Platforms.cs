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
    public partial class Platforms : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The android property</summary>
        public bool? Android { get; set; }
        /// <summary>The iOS property</summary>
        public bool? IOS { get; set; }
        /// <summary>The linux property</summary>
        public bool? Linux { get; set; }
        /// <summary>The macOS property</summary>
        public bool? MacOS { get; set; }
        /// <summary>The web property</summary>
        public bool? Web { get; set; }
        /// <summary>The windows property</summary>
        public bool? Windows { get; set; }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::PubNet.Client.ApiClient.Generated.Models.Platforms"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::PubNet.Client.ApiClient.Generated.Models.Platforms CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::PubNet.Client.ApiClient.Generated.Models.Platforms();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "android", n => { Android = n.GetBoolValue(); } },
                { "iOS", n => { IOS = n.GetBoolValue(); } },
                { "linux", n => { Linux = n.GetBoolValue(); } },
                { "macOS", n => { MacOS = n.GetBoolValue(); } },
                { "web", n => { Web = n.GetBoolValue(); } },
                { "windows", n => { Windows = n.GetBoolValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteBoolValue("android", Android);
            writer.WriteBoolValue("iOS", IOS);
            writer.WriteBoolValue("linux", Linux);
            writer.WriteBoolValue("macOS", MacOS);
            writer.WriteBoolValue("web", Web);
            writer.WriteBoolValue("windows", Windows);
        }
    }
}
#pragma warning restore CS0618
