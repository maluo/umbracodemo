# Fix OpenCode SQLite Disk I/O Error

**Date:** 2026-02-22

## Problem

OpenCode was throwing a SQLite disk I/O error:
```
SQLiteError: disk I/O error
    at run (unknown)
    at run (bun:sqlite:322:21)
    at <anonymous> (src/storage/db.ts:73:12)
```

## Root Cause

The SQLite database at `~/.local/share/opencode/opencode.db` was corrupted. When attempting to run `PRAGMA integrity_check`, it returned:
```
Error: in prepare, disk I/O error (10)
```

## Solution

1. **Backed up the corrupted database:**
   ```bash
   cp ~/.local/share/opencode/opencode.db ~/.local/share/opencode/opencode.db.backup.20260222_210218
   ```

2. **Removed corrupted database files:**
   ```bash
   rm -f ~/.local/share/opencode/opencode.db
   rm -f ~/.local/share/opencode/opencode.db-shm
   rm -f ~/.local/share/opencode/opencode.db-wal
   ```

3. **Let OpenCode recreate the database:**
   Running `opencode --version` triggered OpenCode to automatically recreate a fresh database.

## Verification

✅ Database integrity check passes: `PRAGMA integrity_check` returns `ok`
✅ OpenCode version command works: `opencode --version` returns `1.2.10`
✅ Agent list works: `opencode agent list` returns agent configurations
✅ Authentication preserved: Both Z.AI Coding Plan and GitHub Copilot credentials intact

## Files Modified

- `~/.local/share/opencode/opencode.db` - Recreated (corrupted file backed up)
- `~/.local/share/opencode/opencode.db-shm` - Removed (recreated automatically)
- `~/.local/share/opencode/opencode.db-wal` - Removed (recreated automatically)
- `~/.local/share/opencode/opencode.db.backup.20260222_210218` - Backup of corrupted database

## Prevention

To avoid future corruption:
- Ensure proper shutdown of OpenCode (don't force quit)
- Don't run OpenCode on network drives or external storage with unreliable connections
- Keep adequate disk space (currently at 46% usage - OK)

## System Information

- Disk space: 220GB available (46% used)
- Database location: `~/.local/share/opencode/`
- OpenCode version: 1.2.10
- Platform: macOS (darwin)
