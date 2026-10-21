# osu.Browser

Runs osu! in web browsers via WebAssembly, using osu.Framework.Browser (see its README for details on how the framework runs in browsers).

## Building

The browser framework package is not yet published, so a local checkout of osu-framework is required alongside this repository:

```bash
./UseLocalFramework.sh
```

Realm doesn't provide native libraries for browsers, so one must be built from a checkout of [realm-dotnet](https://github.com/realm/realm-dotnet) matching the `Realm` package version referenced by osu.Game (with submodules initialised):

```bash
./osu.Browser/native/build-realm.sh <path to realm-dotnet source>
```

This applies fixes to Realm's Emscripten support (see `native/realm/*.patch`).

Then publish, and serve the `bin/Release/net10.0-browser/publish/wwwroot` directory:

```bash
dotnet publish osu.Browser -c Release
```

## Limitations

- Online functionality is not available (see `OfflineAPIAccess`), as browsers block cross-origin requests to the osu! API.
- Beatmaps can be imported by dragging `.osz` files onto the page, or via the import button in settings.
- Tempo-changing mods (e.g. Double Time) also change pitch.
- Video playback and editor waveforms are not supported.
- The database is held in memory, and persisted shortly after changes are made. Changes may be lost if the page is closed immediately after making them.
