# Add Z.ai Fallback to oh-my-opencode Secondary Agents - 2026-02-22

## Task Checklist
- [x] Identify secondary/legacy agents in oh-my-opencode.json
- [x] Add zai-coding-plan/glm-4.7 as fallback for all secondary agents
- [x] Verify configuration file is valid JSON
- [x] Log task completion

## Implementation Details

### Configuration File Location
- Path: `/Users/luoma/.config/opencode/oh-my-opencode.json`

### Secondary/Legacy Agents (lowercase names)
The oh-my-opencode configuration contains both primary (uppercase) and secondary (lowercase) agent definitions:

1. **sisyphus** - Already using zai-coding-plan/glm-4.7 (no fallback needed)
2. **oracle** - Using google/gemini-3-pro-preview (variant: high) - NEEDED fallback
3. **librarian** - Already using zai-coding-plan/glm-4.7 (no fallback needed)
4. **explore** - Using opencode/gpt-5-nano - NEEDED fallback
5. **multimodal-looker** - Using google/gemini-3-flash-preview - NEEDED fallback
6. **prometheus** - Using google/gemini-3-pro-preview - NEEDED fallback
7. **metis** - Using google/gemini-3-pro-preview (variant: high) - NEEDED fallback
8. **momus** - Using google/gemini-3-pro-preview (variant: high) - NEEDED fallback
9. **atlas** - Using google/gemini-3-pro-preview - NEEDED fallback

### Fallback Configuration Added
Added `zai-coding-plan/glm-4.7` as fallback for the following agents:

**Without Variants:**
- `explore`
- `multimodal-looker`
- `prometheus`
- `atlas`

**With Variants (high):**
- `oracle` (variant: high)
- `metis` (variant: high)
- `momus` (variant: high)

**No Fallback Needed (already using Z.ai):**
- `sisyphus` - Primary: zai-coding-plan/glm-4.7
- `librarian` - Primary: zai-coding-plan/glm-4.7

### Technical Approach
Each secondary agent was updated to include a fallback array with Z.ai GLM-4.7:

```json
"agent_name": {
  "model": "primary-model",
  "variant": "high",  // where applicable
  "fallback": [
    {
      "model": "zai-coding-plan/glm-4.7"
    }
  ]
}
```

### Why Z.ai GLM-4.7 as Fallback
- **Reliability**: Z.ai is a paid subscription ($10/mo) with guaranteed availability
- **Cost-effective**: Z.ai tokens are affordable and no additional setup required
- **Authentication Working**: Unlike GitHub Copilot and Google Antigravity (which have authentication issues with OpenCode), Z.ai is confirmed working
- **Proven**: Primary agent (Sisyphus) already uses Z.ai GLM-4.7 successfully

### Fallback Chain
For secondary agents, the fallback hierarchy is:
1. **Primary**: Gemini 3 Pro/Flash Preview or OpenCode GPT-5 Nano
2. **Fallback**: Z.ai GLM-4.7 (guaranteed working)

## Change Log

### Files Modified

**File:** `/Users/luoma/.config/opencode/oh-my-opencode.json`

### Changes Made

**Agent: oracle**
- Before: Only `model` and `variant` properties
- After: Added `fallback` array with `zai-coding-plan/glm-4.7`

**Agent: explore**
- Before: Only `model` property (`opencode/gpt-5-nano`)
- After: Added `fallback` array with `zai-coding-plan/glm-4.7`

**Agent: multimodal-looker**
- Before: Only `model` property (`google/gemini-3-flash-preview`)
- After: Added `fallback` array with `zai-coding-plan/glm-4.7`

**Agent: prometheus**
- Before: Only `model` property (`google/gemini-3-pro-preview`)
- After: Added `fallback` array with `zai-coding-plan/glm-4.7`

**Agent: metis**
- Before: Only `model` and `variant` properties
- After: Added `fallback` array with `zai-coding-plan/glm-4.7`

**Agent: momus**
- Before: Only `model` and `variant` properties
- After: Added `fallback` array with `zai-coding-plan/glm-4.7`

**Agent: atlas**
- Before: Only `model` property (`google/gemini-3-pro-preview`)
- After: Added `fallback` array with `zai-coding-plan/glm-4.7`

### Summary

**Total Agents Updated:** 7 out of 9 secondary agents

**Agents Already Using Z.ai (no changes needed):**
- `sisyphus` - Primary model is zai-coding-plan/glm-4.7
- `librarian` - Primary model is zai-coding-plan/glm-4.7

**Agents with New Fallback:**
1. `oracle` - Gemini 3 Pro Preview (high) → Z.ai GLM-4.7
2. `explore` - OpenCode GPT-5 Nano → Z.ai GLM-4.7
3. `multimodal-looker` - Gemini 3 Flash Preview → Z.ai GLM-4.7
4. `prometheus` - Gemini 3 Pro Preview → Z.ai GLM-4.7
5. `metis` - Gemini 3 Pro Preview (high) → Z.ai GLM-4.7
6. `momus` - Gemini 3 Pro Preview (high) → Z.ai GLM-4.7
7. `atlas` - Gemini 3 Pro Preview → Z.ai GLM-4.7

## Usage Examples

### Testing Fallback Configuration

```bash
# Test with primary model (should work)
opencode run "hello" --model=google/gemini-3-pro-preview

# If primary fails, should automatically fall back to Z.ai
# No manual intervention required - automatic fallback

# Test Z.ai directly (confirmed working)
opencode run "hello" --model=zai-coding-plan/glm-4.7
```

### Agent-Specific Usage

```bash
# Explore agent - primary: GPT-5 Nano, fallback: Z.ai GLM-4.7
opencode run "find all database queries"

# Oracle agent - primary: Gemini 3 Pro, fallback: Z.ai GLM-4.7
opencode run "debug this architecture issue"

# Prometheus agent - primary: Gemini 3 Pro, fallback: Z.ai GLM-4.7
/start-work
```

## Testing

- ✅ Configuration file is valid JSON
- ✅ All secondary agents now have fallback configured
- ✅ Z.ai GLM-4.7 is confirmed working (tested earlier)
- ✅ Fallback chain: Primary → Z.ai GLM-4.7

## Notes

### Primary vs Secondary Agents
- **Primary Agents (uppercase):** Sisyphus, Oracle, Librarian, Explore, Multimodal-looker, Prometheus, Metis, Momus, Atlas
  - These are the main agents used by oh-my-opencode
  - Already have proper fallback configurations from the installation

- **Secondary Agents (lowercase):** sisyphus, oracle, librarian, explore, multimodal-looker, prometheus, metis, momus, atlas
  - These appear to be legacy or alternative definitions
  - Now all have Z.ai GLM-4.7 as fallback

### Fallback Strategy
- Ensures reliability even if primary models fail
- All fallbacks point to Z.ai GLM-4.7 which is confirmed working
- No dependencies on GitHub Copilot or Google Antigravity (both have authentication issues with OpenCode)
- Maintains functionality even with the abandoned OpenCode project limitations

### Related Tasks
- Previous: `Add-Copilot-Antigravity-OpenCode-Config-2026-02-22.md`
- Previous: `Install-oh-my-opencode-Plugin-2026-02-22.md`
- Current: `Add-Zai-Fallback-oh-my-opencode-Secondary-Agents-2026-02-22.md`

## References

- Oh-My-OpenCode Repository: https://github.com/code-yeongyu/oh-my-opencode
- Configuration Schema: https://raw.githubusercontent.com/code-yeongyu/oh-my-opencode/master/assets/oh-my-opencode.schema.json
- Z.ai: https://z.ai

---

**Task completed on**: 2026-02-22
**Status**: Complete - All secondary agents now have Z.ai fallback configured
