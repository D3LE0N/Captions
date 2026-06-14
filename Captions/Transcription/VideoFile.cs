namespace Captions.Transcription;

/// <summary>A discovered video file together with a human-friendly title.</summary>
/// <param name="Path">Absolute path to the video file.</param>
/// <param name="Title">File name without extension, used as the transcript heading.</param>
public readonly record struct VideoFile(string Path, string Title);
