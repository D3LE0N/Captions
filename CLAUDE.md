# CLAUDE.md

Guidance for [Claude Code](https://claude.com/claude-code) (and other AI agents) when working in
this repository.

## What this project is

**Captions** is a .NET 10 console application that transcribes a folder of videos or audio files
into text using a local Whisper model. It outputs a combined Markdown file and/or one text file
per source file. ffmpeg and the Whisper model are downloaded automatically on first run.

## Common commands

```bash
dotnet build                                        # build the solution
dotnet run --project Captions -- --help             # show CLI usage
dotnet run --project Captions -- --dir ./media      # transcribe a folder
dotnet publish Captions -c Release -o ./publish     # self-contained build
```

There is no test project yet. Verify changes by building and running the CLI against a sample
folder of videos or audio files.

## Architecture

The app follows clean architecture and SOLID; each concern sits behind an interface and is wired
in the composition root (`Program.cs`).

- `Cli/` — argument parsing (`CliParser`), parsed options (`CliOptions`), help (`HelpText`).
  Parsing is side-effect free and returns a `CliParseResult` (success / help / error).
- `Logging/` — `IAppLogger` with a `ConsoleLogger` implementation.
- `Transcription/`
  - `IMediaFileLocator` → `MediaFileLocator` — discovers videos and audio files by extension.
  - `IFfmpegProvider` → `FfmpegProvider` — downloads/caches ffmpeg into `tools/`.
  - `IAudioExtractor` → `FfmpegAudioExtractor` — extracts 16 kHz mono WAV via ffmpeg.
  - `IWhisperModelProvider` → `WhisperModelProvider` — downloads/caches the model into `models/`.
  - `WhisperModelCatalog` — maps friendly model names to `GgmlType` (keeps the CLI free of
    Whisper types).
  - `ITranscriber` → `WhisperTranscriber` — transcribes a WAV with Whisper.net.
  - `TranscriptionService` — orchestrates locate → extract → transcribe; a single failure is
    logged and skipped, never aborting the batch.
- `Output/` — `ITranscriptionWriter` with `MainMarkdownWriter` (combined `transcriptions.md`) and
  `PerFileWriter` (one `.txt` per media file). The writer set is chosen from the options.

`models/` and `tools/` are created next to the executable at runtime and hold downloaded
artifacts (do not commit them).

## Conventions

- All code, comments, and user-facing strings are written in **English**.
- Prefer clean code and SOLID: new behavior goes behind an interface and is injected from
  `Program.cs`; do not reach for statics or service locators.
- Keep argument validation inside `CliParser`; keep wiring inside `Program.cs`.
- When adding a CLI option, update `CliOptions`, `CliParser`, and `HelpText` together.

## AI tooling shipped with the repo

- `.claude/skills/captions/SKILL.md` — teaches an AI agent how to run this tool.
- `.claude/agents/notary.md` — the `notary` agent that converts a transcript into detailed,
  product-owner-quality requirements and asks the human to resolve ambiguities first.

Keep these in sync with the CLI: if options or output paths change, update `SKILL.md`.

## License

MIT. New files should be compatible with MIT licensing.
