# Captions

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4.svg)](https://dotnet.microsoft.com/)

**Captions** is a small, clean .NET console tool that transcribes a folder of videos or audio
files into text using a local [Whisper](https://github.com/openai/whisper) model. It produces a
single combined Markdown transcript and, optionally, one plain-text file per source file.

ffmpeg and the Whisper model are downloaded automatically on first run — the only hard
prerequisite is the .NET SDK.

The repository also ships with AI tooling for [Claude Code](https://claude.com/claude-code):

- a **`captions` skill** so any AI agent knows how to drive the tool, and
- a **`notary` agent** that turns a transcript into detailed, product-owner-quality requirements.

---

## Features

- 🎬 Batch-transcribes a whole folder of videos and/or audio files.
- 📝 One combined `transcriptions.md`, and/or one `.txt` per source file.
- 🤖 Local Whisper inference (no API keys, no cloud) via [Whisper.net](https://github.com/sandrohanea/whisper.net).
- ⬇️ Auto-downloads ffmpeg and the selected model on first run; offline afterwards.
- 🎚️ Selectable model size (`tiny` … `large-turbo`) to trade speed for accuracy.
- 🧱 Clean architecture (SOLID), each concern behind its own interface.

## Requirements

- [.NET SDK 10+](https://dotnet.microsoft.com/download) (`dotnet --version`).
- Internet access on the **first** run only (to fetch ffmpeg and the model).
- Supported input extensions:
  - Video: `.mp4 .mov .mkv .avi .webm .m4v .flv .wmv .mpg .mpeg`
  - Audio: `.mp3 .wav .m4a .flac .ogg .oga .opus .aac .wma .aiff .aif`

## Installation

Clone and build:

```bash
git clone https://github.com/<your-org>/Captions.git
cd Captions
dotnet build
```

Run it directly with `dotnet run` (recommended for development):

```bash
dotnet run --project Captions -- --dir ./videos
```

### Install as a global tool (recommended)

Captions ships as a [.NET global tool](https://learn.microsoft.com/dotnet/core/tools/global-tools),
so you can install the `captions` command once and run it from any folder:

```bash
dotnet pack Captions -c Release -o ./nupkg
dotnet tool install --global --add-source ./nupkg Captions
```

> **Important — PATH:** .NET installs global tools into `~/.dotnet/tools`, which is **not** on the
> `PATH` by default. If `captions` reports "command not found", add that folder to your `PATH`.
> For zsh (macOS default):
>
> ```bash
> echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.zshrc
> source ~/.zshrc
> ```
>
> For bash, use `~/.bashrc` (or `~/.bash_profile`) instead. On Windows the installer adds the
> folder to `PATH` automatically.

Then, from anywhere:

```bash
captions --dir ./videos
```

Update or remove it later with:

```bash
dotnet tool update --global --add-source ./nupkg Captions
dotnet tool uninstall --global Captions
```

### Other options

Produce a self-contained executable and call it directly:

```bash
dotnet publish Captions -c Release -o ./publish
./publish/Captions --dir ./videos
```

On the first run the tool creates two folders next to the executable:
`models/` (the Whisper model) and `tools/` (ffmpeg).

## Usage

```text
captions --dir <folder> [--out-dir <folder>] [--model <name>] [--out-each] [--no-main]
```

| Option               | Required | Description                                                                               |
|----------------------|----------|-------------------------------------------------------------------------------------------|
| `--dir <folder>`     | Yes      | Folder containing the videos or audio files to transcribe.                                |
| `--out-dir <folder>` | No       | Where output is written. Defaults to the current working directory.                       |
| `--model <name>`     | No       | `tiny`, `base`, `small`, `medium`, `large`, `large-turbo`. Default `base`.                |
| `--out-each`         | No       | Also write one `.txt` transcript per source file.                                         |
| `--no-main`          | No       | Do not create the combined Markdown file (created by default).                            |
| `-h`, `--help`       | No       | Show usage and exit.                                                                       |

### Examples

These use the global `captions` command. If you are running from the repository instead, replace
`captions` with `dotnet run --project Captions --`.

```bash
# Combined transcriptions.md in the current directory
captions --dir ./videos

# Choose an output folder, also emit one .txt per source file, use a more accurate model
captions --dir ./media --out-dir ./out --out-each --model small

# Only per-file text output, no combined Markdown
captions --dir ./podcasts --out-each --no-main
```

### Output

- **`<out-dir>/transcriptions.md`** — one `##` heading per source title followed by its transcription.
- **`<out-dir>/<source-name>.txt`** — only when `--out-each` is used.

## AI tooling (Claude Code)

This repo includes a skill and an agent under `.claude/`:

```
.claude/
├── skills/captions/SKILL.md   # teaches an AI agent how to run Captions
└── agents/notary.md           # turns a transcript into detailed requirements
```

### Installing the skill and agent

They are **project-scoped**: when you open this repository in
[Claude Code](https://claude.com/claude-code), the `captions` skill and the `notary` agent are
picked up automatically from `.claude/`. No extra steps needed.

To make them available in **every** project, copy them to your user-level Claude directory:

```bash
mkdir -p ~/.claude/skills ~/.claude/agents
cp -r .claude/skills/captions ~/.claude/skills/
cp .claude/agents/notary.md ~/.claude/agents/
```

### The `captions` skill

Lets any AI agent transcribe videos with this tool. In Claude Code, simply ask in natural
language and the skill is triggered:

> "Use captions to transcribe the videos in ./videos"

The agent will run the CLI and report where `transcriptions.md` was written.

### The `notary` agent

Takes a transcription (e.g. `transcriptions.md`) and turns it into a detailed requirements
document written as a Product Owner would: scope, functional requirements with user stories and
testable acceptance criteria, non-functional requirements, data/reports, and integrations.

**It asks you to clarify any ambiguity before finalizing**, so the requirements contain no silent
guesses. Invoke it like:

> "Use the notary agent to turn transcriptions.md into requirements"

### End-to-end workflow

```text
videos/  ──(captions skill)──▶  transcriptions.md  ──(notary agent)──▶  requirements.md
```

1. Ask the AI to transcribe a folder of videos with the **captions** skill.
2. Hand the resulting `transcriptions.md` to the **notary** agent.
3. Answer any clarifying questions it asks; it then writes detailed requirements.

## Project structure

```
Captions/
├── Program.cs                 # Composition root (wiring, exit codes, Ctrl+C)
├── Cli/                       # Argument parsing and help text
├── Logging/                   # IAppLogger / ConsoleLogger
├── Transcription/             # Media discovery, ffmpeg, Whisper model + transcriber
└── Output/                    # Combined Markdown and per-file writers
```

## Contributing

Issues and pull requests are welcome. Code and comments are written in English and follow clean
code and SOLID principles.

## License

[MIT](LICENSE) © 2026 Romeo de León.

Captions builds on the excellent [Whisper.net](https://github.com/sandrohanea/whisper.net),
[OpenAI Whisper](https://github.com/openai/whisper) models, and [ffmpeg](https://ffmpeg.org/).
