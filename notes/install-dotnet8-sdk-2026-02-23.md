# Task: Install .NET 8.0 SDK

**Date:** 2026-02-23  
**Status:** ✅ Completed

---

## Problem Summary

The Umbraco project (`d:\Courses\umbracodemo`) has a `global.json` that pinned the SDK version to `8.0.416`. When running `dotnet` commands, the following errors appeared:

- `dotnet --version` failed with "No .NET SDKs were found"
- The `dotnet` on PATH resolved to `C:\Program Files (x86)\dotnet\dotnet.exe` (32-bit, no SDKs)
- SDKs (including `8.0.418`) were installed under `C:\Program Files\dotnet\` (64-bit) but shadowed by the x86 PATH entry

---

## Root Causes

1. **PATH conflict**: `C:\Program Files (x86)\dotnet\` appeared before `C:\Program Files\dotnet\` in the system PATH, causing the 32-bit (runtime-only) dotnet to be resolved instead of the 64-bit SDK installation.

2. **`global.json` version mismatch**: The project required SDK `8.0.416`, but the installed versions were `8.0.121` and `8.0.418`.

---

## Implementation Plan & Changes

### 1. Updated `global.json`
- **File**: `d:\Courses\umbracodemo\global.json`
- **Change**: Updated SDK version from `8.0.416` → `8.0.418` (already installed)

```json
{
  "sdk": {
    "version": "8.0.418"
  }
}
```

### 2. Fixed User PATH
- Added `C:\Program Files\dotnet\` to the **beginning** of the User-level PATH environment variable
- This ensures the 64-bit SDK installation takes precedence over the 32-bit x86 installation

**Command used:**
```powershell
$userPath = [System.Environment]::GetEnvironmentVariable("PATH", "User")
$newUserPath = "C:\Program Files\dotnet\;" + $userPath
[System.Environment]::SetEnvironmentVariable("PATH", $newUserPath, "User")
```

---

## Verification

After the fix:
```
$ dotnet --version
8.0.418
```

---

## Installed SDKs on Machine (as of this date)

| Version | Location |
|---------|----------|
| 3.1.426 | C:\Program Files\dotnet\sdk |
| 5.0.103 | C:\Program Files\dotnet\sdk |
| 5.0.104 | C:\Program Files\dotnet\sdk |
| 7.0.101 | C:\Program Files\dotnet\sdk |
| 7.0.410 | C:\Program Files\dotnet\sdk |
| 8.0.121 | C:\Program Files\dotnet\sdk |
| **8.0.418** ✅ | C:\Program Files\dotnet\sdk |
| 9.0.306 | C:\Program Files\dotnet\sdk |
| 10.0.200-preview | C:\Program Files\dotnet\sdk |

---

## Action Required

> ⚠️ **Restart your terminal / IDE** for the PATH changes to take full effect in new sessions.

---

## Follow-up Fix (same session)

After the User PATH fix, `dotnet --version` still failed because the **Machine-level PATH** had `C:\Program Files (x86)\dotnet\` listed **before** `C:\Program Files\dotnet\`, and Machine PATH is evaluated first.

### Fix: Reordered Machine PATH via elevated PowerShell

Ran an elevated (Admin) PowerShell process to swap the order of the dotnet entries in the Machine PATH:

```powershell
Start-Process powershell -Verb RunAs -ArgumentList @"
-Command `$machinePath = [System.Environment]::GetEnvironmentVariable('PATH', 'Machine');
# ... swap x86 and x64 dotnet entries so x64 comes first ...
[System.Environment]::SetEnvironmentVariable('PATH', `$newPath, 'Machine')
"@ -Wait
```

### After fix:
- **Machine PATH order**: `C:\Program Files\dotnet\` → `C:\Program Files (x86)\dotnet\` ✅
- **`dotnet --version`**: `8.0.418` ✅
- **Status**: Permanently fixed for all new terminal sessions ✅
