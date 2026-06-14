namespace Captions.Cli;

/// <summary>
/// Builds the <c>--help</c> usage text. Centralised so the option documentation lives in
/// a single place next to the parser.
/// </summary>
public static class HelpText
{
    public static string Build() =>
        """
        captions - Transcribe a folder of videos or audio files into text using Whisper
        (speech-to-text).

        USAGE:
          captions --dir <folder> [--out-dir <folder>] [--model <name>] [--out-each] [--no-main]

        OPTIONS:
          --dir <folder>      (Required) Folder that contains the videos or audio files to
                              transcribe.
          --out-dir <folder>  Folder where the output is written. Defaults to the current
                              working directory when omitted.
          --model <name>      Whisper model to use. One of: tiny, base, small, medium, large,
                              large-turbo. Larger models are more accurate but slower and bigger
                              to download. Defaults to 'base'.
          --out-each          Also write one transcript file (.txt) per media file.
          --no-main           Do not create the combined Markdown file. By default a single
                              .md file is produced with each title and its transcription.
          -h, --help          Show this help and exit.

        NOTES:
          - Both video and audio files are supported (e.g. .mp4, .mov, .mkv as well as .mp3,
            .wav, .m4a, .flac, .ogg, .aac). The folder is scanned non-recursively.
          - The selected Whisper model is downloaded automatically on first run and cached under
            the 'models' folder next to the executable.
          - ffmpeg is downloaded automatically when missing and cached under the 'tools'
            folder next to the executable.

        EXAMPLES:
          captions --dir ./media
          captions --dir ./media --out-dir ./out --out-each
          captions --dir ./recordings --model small
          captions --dir ./podcasts --out-each --no-main
        """;
}
