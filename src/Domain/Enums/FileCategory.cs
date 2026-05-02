namespace Domain.Enums;

/// <summary>
/// Groups uploaded files by the media type used for admin reporting.
/// </summary>
public enum FileCategory
{
    /// <summary>
    /// Image files such as JPEG, PNG, GIF, or WebP.
    /// </summary>
    Image = 1,

    /// <summary>
    /// Video files such as MP4, WebM, or QuickTime.
    /// </summary>
    Video = 2,

    /// <summary>
    /// Audio files such as MP3, WAV, OGG, or AAC.
    /// </summary>
    Audio = 3,

    /// <summary>
    /// Document files such as PDF, Word, Excel, PowerPoint, or plain text.
    /// </summary>
    Document = 4
}
