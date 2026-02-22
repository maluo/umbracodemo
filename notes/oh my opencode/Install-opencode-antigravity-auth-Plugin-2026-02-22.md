# Task: Install opencode-antigravity-auth Plugin - 2026-02-22

## Implementation Plan

1. Verify current opencode.json configuration
2. Add Antigravity model definitions to the Google provider configuration
3. Test the configuration

## Change Log

### Files Modified

- `C:\Users\james\.config\opencode\opencode.json`

### Changes Made

1. **Added model definitions** to `provider.google.models`:
   - `antigravity-gemini-3-pro` (with variants: low, high)
   - `antigravity-gemini-3.1-pro` (with variants: low, high)
   - `antigravity-gemini-3-flash` (with variants: minimal, low, medium, high)
   - `antigravity-claude-sonnet-4-6`
   - `antigravity-claude-opus-4-6-thinking` (with variants: low, max)
   - `gemini-2.5-flash`
   - `gemini-2.5-pro`
   - `gemini-3-flash-preview`
   - `gemini-3-pro-preview`
   - `gemini-3.1-pro-preview`
   - `gemini-3.1-pro-preview-customtools`

2. Each model includes:
   - Display name
   - Context and output limits
   - Input/output modalities (text, image, pdf)
   - Variant configurations (where applicable)

### Plugin Status

The plugin `opencode-antigravity-auth@latest` was already installed.

## Usage Examples

### Basic Usage
```bash
# Use Claude Opus with thinking
opencode run "Hello" --model=google/antigravity-claude-opus-4-6-thinking --variant=max

# Use Gemini 3 Pro
opencode run "Hello" --model=google/antigravity-gemini-3-pro --variant=high

# Use Gemini 3 Flash
opencode run "Hello" --model=google/antigravity-gemini-3-flash --variant=medium
```

### Authentication
```bash
# Login with Google account
opencode auth login

# Add multiple accounts for higher quotas
opencode auth login
```

## Technical Details

### Configuration Location
- Main config: `C:\Users\james\.config\opencode\opencode.json`
- Accounts: `~/.config/opencode/antigravity-accounts.json`
- Plugin config: `~/.config/opencode/antigravity.json`

### Models Available

**Antigravity Quota (default):**
- `google/antigravity-gemini-3-pro`
- `google/antigravity-gemini-3.1-pro`
- `google/antigravity-gemini-3-flash`
- `google/antigravity-claude-sonnet-4-6`
- `google/antigravity-claude-opus-4-6-thinking`

**Gemini CLI Quota:**
- `google/gemini-2.5-flash`
- `google/gemini-2.5-pro`
- `google/gemini-3-flash-preview`
- `google/gemini-3-pro-preview`
- `google/gemini-3.1-pro-preview`
- `google/gemini-3.1-pro-preview-customtools`

### Features Enabled
- Multi-account support with auto-rotation
- Dual quota system (Antigravity + Gemini CLI)
- Extended thinking for Claude and Gemini 3
- Google Search grounding
- Auto-recovery from session errors

## Notes

- Plugin provides access to Claude Opus 4.6, Sonnet 4.6, and Gemini 3 Pro/Flash via Google OAuth
- Supports multiple Google accounts for higher combined quotas
- Automatically handles rate limit errors and account rotation
- Compatible with other OpenCode plugins

## Next Steps

1. Run `opencode auth login` to authenticate with Google
2. Optionally add multiple accounts for higher quotas
3. Test with a simple command to verify configuration

## References

- Plugin repository: https://github.com/NoeFabris/opencode-antigravity-auth
- Documentation: https://raw.githubusercontent.com/NoeFabris/opencode-antigravity-auth/dev/README.md

---

**Task completed on**: 2026-02-22
