# Add Z.ai Coding Plan as Fallback for All Agents

**Date:** 2026-02-22

## Implementation Plan

Update oh-my-opencode.json configuration to add Z.ai Coding Plan (the user's only paid subscription) as the final fallback option for all agents and categories.

## Change Log

### Configuration Updates

Modified `~/.config/opencode/oh-my-opencode.json` to add `zai-coding-plan/glm-4.7` as the final fallback for:

**Agents:**
1. **sisyphus** - Added `zai-coding-plan/glm-4.7` as 3rd fallback (after gpt-5-mini, antigravity-claude-opus-4-5-thinking)
2. **oracle** - Reordered to add `google/antigravity-gemini-3-pro` as 2nd fallback, `zai-coding-plan/glm-4.7` as 3rd
3. **librarian** - Already uses Z.ai as primary, added `google/antigravity-gemini-3-flash` as fallback
4. **explore** - Already had Z.ai as fallback (unchanged)
5. **multimodal-looker** - Added `google/antigravity-gemini-3-flash` as fallback before Z.ai
6. **prometheus** - Added `google/antigravity-gemini-3-pro` as 2nd fallback, Z.ai as 3rd
7. **metis** - Added `google/antigravity-gemini-3-pro` as 2nd fallback, Z.ai as 3rd
8. **momus** - Added `google/antigravity-gemini-3-pro` as 2nd fallback, Z.ai as 3rd
9. **atlas** - Added `google/antigravity-gemini-3-pro` as 2nd fallback, Z.ai as 3rd
10. **hephaestus** - Added `zai-coding-plan/glm-4.7` as fallback

**Categories:**
1. **visual-engineering** - Added `google/antigravity-gemini-3-pro` as 2nd fallback, Z.ai as 3rd
2. **ultrabrain** - Added `google/antigravity-gemini-3-pro` as 2nd fallback, Z.ai as 3rd
3. **artistry** - Added `google/antigravity-gemini-3-pro` as 2nd fallback, Z.ai as 3rd
4. **quick** - Added `github-copilot/gpt-5-mini` as 2nd fallback, `zai-coding-plan/glm-4.7-flash` as 3rd
5. **unspecified-low** - Added `github-copilot/gpt-5-mini` as 2nd fallback, Z.ai as 3rd
6. **unspecified-high** - Changed to use `gemini-3-pro` instead of flash, added Copilot, then Z.ai
7. **writing** - Added `github-copilot/gpt-5-mini` as 2nd fallback, Z.ai as 3rd
8. **deep** - Added `zai-coding-plan/glm-4.7` as fallback

## Fallback Strategy

**Priority Order:**
1. **Primary model** (GitHub Copilot or Gemini Antigravity)
2. **Secondary free options** (Copilot mini, Gemini flash/pro)
3. **Z.ai Coding Plan** (user's only paid subscription) - final fallback

This ensures:
- Free tier options are tried first to conserve paid quota
- Z.ai is always available as a reliable fallback when free options fail
- All agents have a path to the paid model if needed

## Verification

✅ JSON syntax validated
✅ OpenCode agent list command works correctly
✅ All agents now have Z.ai as fallback option

## User's Subscriptions

- ✅ Z.ai Coding Plan (paid) - Primary fallback
- ✅ GitHub Copilot (included)
- ✅ Gemini Antigravity (free tier)
- ❌ Claude Pro/Max (not subscribed)
- ❌ OpenAI/ChatGPT (not subscribed)

## Files Modified

- `~/.config/opencode/oh-my-opencode.json` - Updated fallback chains for all agents and categories
