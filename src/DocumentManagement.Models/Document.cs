using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DocumentManagement.Models;

/// <summary>
/// Represents a document in the document management system.
/// </summary>
public class Document
{
    /// <summary>
    /// Gets or sets the unique identifier for the document.
    /// </summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the document.
    /// </summary>
    [Required]
    [StringLength(200)]
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file name of the document.
    /// </summary>
    [Required]
    [StringLength(255)]
    [JsonProperty("fileName")]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    [JsonProperty("fileSize")]
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the MIME type of the document.
    /// </summary>
    [StringLength(100)]
    [JsonProperty("mimeType")]
    public string? MimeType { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the document was created.
    /// </summary>
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the document was last modified.
    /// </summary>
    [JsonProperty("modifiedAt")]
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}