---
name: captions
description: Use when the user wants to transcribe a video or a folder of videos into text. Captions is a .NET console tool that uses OpenAI Whisper (speech-to-text) to produce a single combined Markdown transcript and/or one plain-text file per video. Triggers include requests like "transcribe these videos", "get the text of this recording", or "generate a transcript from this footage".
---

# Captions

Captions transcribes one or more video files into text using a local Whisper model. It is a
.NET 10 console application. ffmpeg and the Whisper model are downloaded automatically on first
run, so the only hard prerequisite is the .NET SDK.

## When to use this skill

Use it whenever the goal is to turn the audio of a video (or a batch of videos in a folder)
into a written transcript. After running it, the produced `transcriptions.md` is a good input
for the `notary` agent, which converts transcripts into detailed software requirements.

## Prerequisites

- .NET SDK 10 or newer (`dotnet --version`).
- Internet access on the first run (to download ffmpeg and the Whisper model into the `tools/`
  and `models/` folders next to the executable). Subsequent runs are fully offline.
- The Captions repository available locally (this project).

## How to run it

From the repository root, invoke the project with `dotnet run`. Everything after `--` is passed
to Captions:

```bash
dotnet run --project Captions -- --dir <videos-folder> [options]
```

The input folder is scanned (non-recursively) for these extensions:
`.mp4 .mov .mkv .avi .webm .m4v .flv .wmv .mpg .mpeg`.

## Options

| Option               | Required | Description                                                                                 |
|----------------------|----------|---------------------------------------------------------------------------------------------|
| `--dir <folder>`     | Yes      | Folder containing the videos to transcribe.                                                 |
| `--out-dir <folder>` | No       | Where output is written. Defaults to the current working directory.                         |
| `--model <name>`     | No       | Whisper model: `tiny`, `base`, `small`, `medium`, `large`, `large-turbo`. Default `base`.   |
| `--out-each`         | No       | Also write one `.txt` transcript per video.                                                 |
| `--no-main`          | No       | Do not create the combined Markdown file (by default it is created).                        |
| `-h`, `--help`       | No       | Show usage and exit.                                                                         |

Notes:
- Larger models are more accurate but slower and larger to download.
- You must produce at least one kind of output: combining `--no-main` without `--out-each`
  is rejected.

## Output

- **Combined file (default):** `<out-dir>/transcriptions.md` — one `##` heading per video title
  followed by its transcription. This is the canonical artifact to hand to the `notary` agent.
- **Per-video files (`--out-each`):** `<out-dir>/<video-name>.txt` — the plain transcription of
  each video.

## Examples

Transcribe a folder into a single Markdown file in the current directory:

```bash
dotnet run --project Captions -- --dir ./videos
```

Transcribe into a chosen folder, also emitting one text file per video, using a more accurate
model:

```bash
dotnet run --project Captions -- --dir ./videos --out-dir ./out --out-each --model small
```

Only per-video text files, no combined Markdown:

```bash
dotnet run --project Captions -- --dir ./videos --out-each --no-main
```

## After running

1. Read `transcriptions.md` (or the `.txt` files) to confirm the transcription looks correct.
2. If the user wants requirements, user stories, or a spec derived from the transcript, hand the
   transcript to the `notary` agent.

## Troubleshooting

- "Input directory does not exist" — check the `--dir` path.
- First run is slow or appears to hang — it is downloading ffmpeg and/or the Whisper model into
  `models/` and `tools/`. Let it finish; later runs are fast and offline.
- Poor accuracy — try a larger `--model` (e.g. `small` or `medium`).
