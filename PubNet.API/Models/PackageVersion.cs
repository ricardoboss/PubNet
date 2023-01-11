using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PubNet.API.Models;

public class PackageVersion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Version { get; set; }

    public bool Retracted { get; set; }

    public string Archive_Url { get; set; }

    public string Archive_Sha256 { get; set; }

    [Column(TypeName = "json")]
    public string Pubspec { get; set; }
}