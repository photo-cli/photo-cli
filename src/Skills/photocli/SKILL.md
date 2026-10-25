---
name: photocli
description: Build, verify and run `photo-cli` commands that organize photos by EXIF taken date and GPS address (copy, archive, info, address, list, settings). Use when the user wants to sort, rename, archive, report on, or list photos, reverse-geocode photo locations, or asks how to use photo-cli. Guides through missing arguments step by step and always shows the final command for confirmation before running it.
allowed-tools: Bash(photo-cli:*)
---

# photo-cli

`photo-cli` reads when (EXIF taken date) and where (GPS → reverse geocoded address) photos were taken, then copies or archives them into an organized folder. It never modifies the source folder (except `archive --delete-on-source`).

Full option values, defaults and rules: [references/options.md](references/options.md). `photo-cli help <verb>` prints live examples.

## Workflow

1. **Understand the goal.** Map the request to one verb:

   | Goal | Verb |
   |---|---|
   | Copy into a new folder with custom names/folders | `copy` |
   | Long-term library: date folders, SQLite index, albums | `archive` |
   | CSV report only, no files moved | `info` |
   | Check what address one photo resolves to | `address` |
   | Browse/open an archive (albums, dates, counts) | `list` |
   | View/change saved defaults (API keys, formats) | `settings` |

2. **Fill in missing arguments by asking.** When the request is vague, don't guess the required options. Ask about one decision at a time, offering 2–4 concrete choices with a one-line effect each, in this order:
   - Verb (if unclear), then input folder and output folder.
   - `copy`: folder layout (`--process-type`) → file naming (`--naming-style`) → grouping (`--group-by`) or folder append (`--folder-append` + `--folder-append-location`) → number style.
   - Address wanted? Then provider (`--reverse-geocode`) and its address parts. Suggest `OpenStreetMapFoundation` with `country city town suburb` as the free default.
   - What to do with photos missing a date or coordinate (`--no-taken-date`, `--no-coordinate`).
   - `archive`: album (`--album-type` + `--album-name`/`--update-album`) or none.

   Skip a question when the user already answered it or the default is clearly fine. Use the rules in the reference to narrow the options, and never offer a combination the validator rejects.

3. **Always verify the command.** Before running anything, show the final command in a code block, with long option names, plus a short bullet list of what it will do:
   ```
   photo-cli copy --input ~/Pictures/Trip --output ~/Pictures/Trip-sorted --process-type FlattenAllSubFolders --naming-style DateTimeWithSeconds --number-style PaddingZeroCharacter --group-by YearMonthDay
   ```
   - Reads all subfolders, writes one flat set grouped into `year/month/day` folders
   - File names like `2024.06.01_14.22.05.jpg`; duplicates get `-1`, `-2`
   - Source folder is untouched

   Then ask: run it, run a `--dry-run` first, or change something? Never run without an explicit yes.

4. **Run and report (only if asked).** For `copy`/`archive`, offer `--dry-run` first: it writes a `photo-cli-dry-run.csv` report and copies nothing. After the run:
   - Report the exit code. `0` means success. For any other code, explain it using the exit code table in the reference and suggest a fix.
   - Summarize the **Statistics** table that `copy`, `archive` and `info` print: photos found and copied, how many have a date and coordinate and how many are missing one, reverse geocode requests sent and cache hits, albums created, file errors. Lead with anything surprising (for example, many photos without coordinates, or invalid formats).
   - Mention the generated report (`photo-cli-report.csv` in the output folder, or the `info` CSV path).

## Safety rules

- `archive --delete-on-source` deletes the source photos. Only add it when the user asks for it explicitly, and ask for confirmation again just before running.
- For `copy`, the output folder must be empty or not exist yet.
- Never print API keys in commands you show. Prefer the environment variables `PHOTO_CLI_GOOGLE_MAPS_API_KEY`, `PHOTO_CLI_BIG_DATA_CLOUD_API_KEY` and `PHOTO_CLI_LOCATIONIQ_API_KEY`, or `photo-cli settings --key GoogleMapsApiKey --value ...` (which the user runs themselves).
- Large folders with a reverse geocoding provider can take a long time and make many API requests. Say so before running.
- If `photo-cli` is not installed, suggest `brew tap photo-cli/homebrew-photo-cli && brew install photo-cli` or `dotnet tool install photo-cli -g`.

## Querying an archive

If the photo-cli MCP tools are available (`mcp__photo-cli__*`), use them to answer questions about an archived library (search by date, place or album, statistics). Otherwise use `photo-cli list --type ... --input <archive>`. Add `--raw` to print paths instead of opening Preview on macOS.
