# 0.3.1 - 0.3.3
- Homebrew & standalone executable builds by release automation.

# 0.3.0 (2024-07-27)

- Companion file support
- Resilience in reserve geocoding requests
- Upgraded to .NET 8
- Configurable supported photos

# 0.2.1 (2024-03-06)

- Fixing unable to set non-string setting values [@RecepCil](https://github.com/RecepCil)

# 0.2.0 (2024-02-21)

- Archive verb & small fixes & test improvements
- Cache reverse geocode results by [@RecepCil](https://github.com/RecepCil)
- Verifying address responses with regex

# 0.1.0 (2023-06-13)

- HEIC & JPEG file support [@RecepCil](https://github.com/RecepCil)
- Verify file hash
- Invalid photo format prevention, error handling

# 0.0.1 (2023-02-23)

- Getting address without giving reverse geocoding specific properties
- Reverse geocode provider MapQuest removed, service deprecated

# 0.0.0 (2022-08-01)

- First public release.
`copy`, `info`, `address` and `settings` verbs developed
  with this essential options;

- FolderProcessType: Single, SubFoldersPreserveFolderHierarchy,FlattenAllSubFolders

- NamingStyle: Numeric, Day, DateTimeWithMinutes, DateTimeWithSeconds, Address, DayAddress, DateTimeWithMinutesAddress, DateTimeWithSecondsAddress, AddressDay, AddressDateTimeWithMinutes, AddressDateTimeWithSeconds

- FolderAppendType: FirstYearMonthDay, FirstYearMonth, FirstYear, DayRange, MatchingMinimumAddress

- GroupByFolderType: YearMonthDay, YearMonth, Year, AddressFlat, AddressHierarchy

- ReverseGeocodeProvider: BigDataCloud, OpenStreetMapFoundation, GoogleMaps, MapQuest, LocationIq
