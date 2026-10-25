# photo-cli options reference

Enum options accept the name or the number (e.g. `--naming-style DateTimeWithSeconds` or `-s 4`). Paths can be relative. `--input` defaults to the current folder.

## copy

Required: `--output`, `--naming-style`, `--process-type`, `--number-style`.

| Option | Values |
|---|---|
| `-f --process-type` | `Single` (1) only the root folder · `SubFoldersPreserveFolderHierarchy` (2) keep folder tree · `FlattenAllSubFolders` (3) all subfolders into one level |
| `-s --naming-style` | `Numeric` (1) · `Day` (2) · `DateTimeWithMinutes` (3) · `DateTimeWithSeconds` (4) · `Address` (5) · `DayAddress` (6) · `DateTimeWithMinutesAddress` (7) · `DateTimeWithSecondsAddress` (8) · `AddressDay` (9) · `AddressDateTimeWithMinutes` (10) · `AddressDateTimeWithSeconds` (11) |
| `-n --number-style` | `AllNamesAreSameLength` (1) 100,101 · `PaddingZeroCharacter` (2) 001,002 · `OnlySequentialNumbers` (3) 1,2,10 |
| `-g --group-by` | `YearMonthDay` (1) · `YearMonth` (2) · `Year` (3) · `AddressFlat` (4) one folder `Country-City-…` · `AddressHierarchy` (5) `Country/City/…` |
| `-a --folder-append` | `FirstYearMonthDay` (1) · `FirstYearMonth` (2) · `FirstYear` (3) · `DayRange` (4) · `MatchingMinimumAddress` (5) |
| `-p --folder-append-location` | `Prefix` (1) · `Suffix` (2) |
| `-t --no-taken-date` | `Continue` (0, default) · `PreventProcess` (1) · `DontCopyToOutput` (2) · `InSubFolder` (3) · `AppendToEndOrderByFileName` (4) · `InsertToBeginningOrderByFileName` (5) |
| `-c --no-coordinate` | `Continue` (0, default) · `PreventProcess` (1) · `DontCopyToOutput` (2) · `InSubFolder` (3) |
| `-x --invalid-format` | `Continue` (0, default) · `PreventProcess` (1) · `DontCopyToOutput` (2) · `InSubFolder` (3) |
| `-d --dry-run` | flag: report only, nothing copied |
| `-v --verify` | flag: verify copies by SHA1 hash |
| `-w --expected-day-range` | number: stop if photos span more days than this |

Rules (the validator rejects anything else):
- `Single` → no `--folder-append`, `--folder-append-location`.
- `FlattenAllSubFolders` → no `--folder-append`, `--folder-append-location`.
- `SubFoldersPreserveFolderHierarchy` → no `--group-by`.
- `--folder-append` and `--folder-append-location` must be used together.
- `--group-by` excludes `--folder-append`, `--folder-append-location`.
- Any naming style containing `Address`, `--group-by AddressFlat`/`AddressHierarchy`, or `--folder-append MatchingMinimumAddress` → requires `--reverse-geocode`.

## archive

Everything is optional. `--output` is required unless `ArchivePath` is saved in settings. The output layout is fixed: `year/month/day/<datetime>-<hash>.ext`, and metadata is indexed in `photo-cli.sqlite3`. Re-running on the same archive adds new photos and skips duplicates.

| Option | Values |
|---|---|
| `-o --output` / `-i --input` | archive folder / source folder |
| `-d --dry-run` | flag |
| `-t --no-taken-date`, `-c --no-coordinate`, `-x --invalid-format` | `Continue` (0, default) · `PreventProcess` (1) |
| `-w --expected-day-range` | number |
| `-y --album-type` | `NoAlbumLinking` (0) · `Individual` (1) · `DateRange` (2) |
| `-a --album-name` | new album name |
| `-p --update-album` | existing album id (`photo-cli list -t Albums`) |
| `-s --auto-reverse-geocode-album` | flag: one album per address part (requires `--reverse-geocode`) |
| `-f --delete-on-source` | flag: **deletes source files** after success |
| `-j --custom-database-path` | path to a SQLite file |

Rules: `--album-type` needs exactly one of `--album-name` / `--update-album`, and each of those needs `--album-type`.

## info

Required: `--output <file>.csv`. Options: `-a --all-folders` (include subfolders), `-t`/`-c`/`-x` with `Continue` (0) or `PreventProcess` (1), plus reverse geocode options.

## address

Required: `--input <photo>`, `--reverse-geocode`, `--type`: `AllAvailableProperties` (0) · `SelectedProperties` (1) · `FullResponse` (2). Use `FullResponse` to discover which property names or levels to pass to the other verbs.

## list

`-i --input <archive>` and `-t --type`:
- `Summary` (0, default): album, photo and cache counts
- `Albums` (1)
- `PhotosByAlbumId` (2): needs `-n --id`
- `PhotosByAlbumName` (3): needs `-a --name`
- `PhotosByExactDate` (4): with `-y --year`, `-m --month`, `-d --day`
- `PhotosByDateRange` (5): needs `-s --start-date` and/or `-e --end-date` (`yyyy-MM-dd`)

`-r --raw` prints paths instead of opening them in macOS Preview.

## settings

- `photo-cli settings`: list all settings
- `-k --key K`: get one setting
- `-k K -v V`: set one setting
- `-r --reset`: reset all settings to defaults

Keys include `YearFormat`, `DateTimeFormatWithSeconds`, `AddressSeparator`, `ArchivePath`, `GoogleMapsApiKey`, `BigDataCloudApiKey`, `LocationIqApiKey`, `ConnectionLimit`, `SupportedExtensions`, `CompanionExtensions`.

## Reverse geocode (copy, archive, info, address)

| `-e --reverse-geocode` | Needs | Key |
|---|---|---|
| `Disabled` (0, default) | none | – |
| `BigDataCloud` (1) | `-u --bigdatacloud-levels 2 4 6 8` | `PHOTO_CLI_BIG_DATA_CLOUD_API_KEY` or `-b` |
| `OpenStreetMapFoundation` (2), free | `-r --openstreetmap-properties country city town suburb` | none |
| `GoogleMaps` (3) | `-m --googlemaps-types administrative_area_level_1 administrative_area_level_2` | `PHOTO_CLI_GOOGLE_MAPS_API_KEY` or `-k` |
| `LocationIq` (5) | `-r --openstreetmap-properties …` | `PHOTO_CLI_LOCATIONIQ_API_KEY` or `-q`; `-h` for a paid license |

Other options: `-l --language <culture>`; `-z --missing-reverse-geocode` `Continue` (0) / `PreventProcess` (1), which requires a provider.

## Exit codes

| Code | Meaning | Typical fix |
|---|---|---|
| 0 | Success | |
| 1 | Arguments not parsed | check spelling, see `photo-cli help <verb>` |
| 2 | Invalid `appsettings.json` | `photo-cli settings --reset` |
| 3 | Unexpected error | rerun with more logging |
| 10 | API key missing or invalid | set the env var or pass the key |
| 11–17 | Option validation failed (address, info, copy, settings, archive, mcp, skill) | read the printed message, fix the combination |
| 20 | Input folder doesn't exist | |
| 21 | No photos found | wrong folder, or `--all-folders` / a different process type needed |
| 22 | Output folder is not empty | pick a new or empty folder |
| 23 | Output file already exists | |
| 24/25 | No write or create permission on output | |
| 26 | Input file doesn't exist | |
| 27 | Hash verification failed | |
| 28 | User home folder not found (`skill --global`) | set `CLAUDE_CONFIG_DIR` |
| 30–32 | Stopped: photos without date and/or coordinate | use `Continue` / `InSubFolder` instead of `PreventProcess` |
| 33 | Stopped: invalid format | |
| 34 | Stopped: missing reverse geocode | |
| 35 | Stopped: day range exceeded | raise `--expected-day-range` |
| 40–42 | Settings key not found / invalid value / bad LogLevel format | |
| 50–57 | Album errors (exists, not found by id, name must be unique, no photos, no date range) | |
| 60 | No archive database found | wrong `--input` for list |
| 61 | No photos to list | widen the filter |
| 62 | Album not found by name | |
