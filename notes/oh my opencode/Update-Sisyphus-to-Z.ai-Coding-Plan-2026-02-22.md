# Update Sisyphus Agent to Use Z.ai Coding Plan

**Date:** 2026-02-22

## Implementation Plan

Update the Sisyphus agent configuration in oh-my-opencode.json to use Z.ai Coding Plan (GLM-4.7) as the primary model since it's the user's only paid subscription.

## Change Log

### Configuration Update

Modified `~/.config/opencode/oh-my-opencode.json`:

**Before:**
```json
"sisyphus": {
  "model": "github-copilot/claude-opus-4.6",
  "fallback": [
    "github-copilot/gpt-5-mini",
    "google/antigravity-claude-opus-4-5-thinking",
    "zai-coding-plan/glm-4.7"
  ],
  "variant": "max"
}
```

**After:**
```json
"sisyphus": {
  "model": "zai-coding-plan/glm-4.7",
  "fallback": [
    "github-copilot/claude-opus-4.6",
    "github-copilot/gpt-5-mini",
    "google/antigravity-claude-opus-4-5-thinking"
  ],
  "variant": "max"
}
```

## Rationale

**Why prioritize Z.ai Coding Plan:**
- It's the user's only paid subscription
- Paid models typically have better rate limits and reliability
- Ensures the main orchestration agent (Sisyphus) uses the most dependable model
- Free tier options (Copilot, Gemini Antigravity) are kept as fallbacks

**Sisyphus Agent Importance:**
- Main orchestration agent that breaks down complex tasks
- Delegates to specialist agents
- Critical for the overall agent system performance
- Benefits from the most reliable model available

## Verification

✅ JSON syntax validated
✅ OpenCode agent list command works correctly
✅ Sisyphus now uses Z.ai GLM-4.7 as primary model

## New Fallback Chain

**Sisyphus priority:**
1. **zai-coding-plan/glm-4.7** (paid, primary)
2. **github-copilot/claude-opus-4.6** (fallback 1)
3. **github-copilot/gpt-5-mini** (fallback 2)
4. **google/antigravity-claude-opus-4-5-thinking** (fallback 3)

## Files Modified

- `~/.config/opencode/oh-my-opencode.json` - Updated Sisyphus agent configuration
