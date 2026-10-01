# Folder Synchronization

A simple C#/.NET console application that periodically synchronizes a source folder with a backup folder.

## Features

* Copies new files from the source to the backup.
* Configures source and backup folders paths with console arguments
* Updates files that have changed.
* Creates missing directories.
* Removes files and directories from the backup that no longer exist in the source.
* Uses SHA-256 hashes to detect changed files.
* Logs synchronization activity to the console and a log file.
* Runs synchronization at a configurable interval.
* Stops the application with key-stroke `Ctrl+C`.

## Requirements

* .NET 10 SDK

## Usage

```bash
dotnet run -- <source-folder> <backup-folder> <interval-seconds> <log-file>
```

### Example

```bash
dotnet run -- ./sourceFolder ./destFolder 10 ./sync.log
```

This synchronizes `sourceFolder` with `destFolder` every 10 seconds and writes the log to `sync.log`.

## Notes

* The source folder must already exist.
* The source and backup folders cannot be the same.
* Both folders can not be inside one another.
* The backup folder is created automatically if it does not exist.