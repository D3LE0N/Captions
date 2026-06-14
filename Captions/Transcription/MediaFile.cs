namespace Captions.Transcription;

/// <summary>A discovered media file (video or audio) together with a human-friendly title.</summary>
/// <param name="Path">Absolute path to the media file.</param>
/// <param name="Title">File name without extension, used as the transcript heading.</param>
public readonly record struct MediaFile(string Path, string Title);
