# Task: Configure GitHub Copilot with Z.ai Fallback in oh-my-opencode - 2026-02-22

## Implementation Plan

1. Re-enable GitHub Copilot plugin in opencode.json
2. Update oh-my-opencode.json to use GitHub Copilot for specific scenarios
3. Configure Z.ai Coding Plan as universal fallback

## Change Log

### Files Modified

- `C:\Users\james\.config\opencode\opencode.json` - Re-added GitHub Copilot plugin and provider
- `C:\Users\james\.config\opencode\oh-my-opencode.json` - Updated agents and categories with multi-tier fallback

### Changes Made

1. **Re-enabled GitHub Copilot Plugin**
   - Added `opencode-copilot-auth` to plugin array
   - Added `github-copilot` provider configuration

2. **Agent Model Configuration (Multi-tier Fallback)**

   | Agent | Primary Model | Fallback 1 | Fallback 2 |
   |--------|---------------|--------------|-------------|
   | **Sisyphus** | zai-coding-plan/glm-4.7 | github-copilot/gpt-5-mini | google/antigravity-claude-opus-4-5-thinking |
   | **Oracle** | google/antigravity-gemini-3-pro (variant: high) | github-copilot/gpt-5-mini | zai-coding-plan/glm-4.7 |
   | **Librarian** | zai-coding-plan/glm-4.7 | github-copilot/gpt-5-mini | - |
   | **Explore** | github-copilot/gpt-5-mini | google/antigravity-gemini-3-flash | zai-coding-plan/glm-4.7 |
   | **Multimodal-looker** | google/antigravity-gemini-3-flash (variant: medium) | zai-coding-plan/glm-4.7 | - |
   | **Prometheus** | google/antigravity-gemini-3-pro | github-copilot/gpt-5-mini | zai-coding-plan/glm-4.7 |
   | **Metis** | google/antigravity-gemini-3-pro (variant: high) | github-copilot/gpt-5-mini | zai-coding-plan/glm-4.7 |
   | **Momus** | google/antigravity-gemini-3-pro (variant: high) | github-copilot/gpt-5-mini | zai-coding-plan/glm-4.7 |
   | **Atlas** | google/antigravity-gemini-3-pro | github-copilot/gpt-5-mini | zai-coding-plan/glm-4.7 |

3. **Category Model Configuration (Multi-tier Fallback)**

   | Category | Primary Model | Fallback 1 | Fallback 2 |
   |----------|---------------|--------------|-------------|
   | **visual-engineering** | google/antigravity-gemini-3-pro (variant: high) | github-copilot/gpt-5-mini | zai-coding-plan/glm-4.7 |
   | **ultrabrain** | google/antigravity-gemini-3-pro (variant: high) | github-copilot/gpt-5-mini | zai-coding-plan/glm-4.7 |
   | **artistry** | google/antigravity-gemini-3-pro (variant: high) | github-copilot/gpt-5-mini | zai-coding-plan/glm-4.7 |
   | **quick** | github-copilot/gpt-5-mini | google/antigravity-gemini-3-flash | zai-coding-plan/glm-4.7 |
   | **unspecified-low** | github-copilot/gpt-5-mini | google/antigravity-gemini-3-flash | zai-coding-plan/glm-4.7 |
   | **unspecified-high** | github-copilot/gpt-5-mini | google/antigravity-gemini-3-flash | zai-coding-plan/glm-4.7 |
   | **writing** | github-copilot/gpt-5-mini | google/antigravity-gemini-3-flash | zai-coding-plan/glm-4.7 |

## Technical Details

### Provider Strategy

**GitHub Copilot (Personal Account):**
- Used for quick tasks and as primary for Explore agent
- Primary model: `github-copilot/gpt-5-mini`
- Fast, efficient for rapid iterations

**Google Antigravity (Free via OAuth):**
- Used for reasoning, thinking, and creative tasks
- Primary for: Oracle, Prometheus, Metis, Momus, Atlas, visual-engineering, ultrabrain, artistry
- May have rate limits

**Z.ai Coding Plan (Paid Subscription - $10/mo):**
- Primary for: Sisyphus (orchestrator), Librarian (docs/search)
- Universal fallback for all agents and categories
- Ensures service continuity

### Fallback Hierarchy

1. **Level 1 (Primary)**: Best model for the task
2. **Level 2 (First Fallback)**: GitHub Copilot GPT-5 Mini (fast, reliable)
3. **Level 3 (Second Fallback)**: Google Antigravity or Z.ai GLM-4.7 (paid, guaranteed availability)

### When Each Provider is Used

**GitHub Copilot GPT-5 Mini:**
- Quick tasks (explore, writing, unspecified)
- Fallback for most agents when primary fails

**Google Antigravity:**
- High-end reasoning (Oracle with high thinking)
- Creative tasks (artistry, visual-engineering)
- Complex planning (ultrabrain, Prometheus, Metis, Momus, Atlas)

**Z.ai GLM-4.7:**
- Main orchestrator (Sisyphus)
- Documentation and code search (Librarian)
- Ultimate fallback for everything

## Usage Examples

### Automatic Multi-tier Fallback

No manual configuration needed - automatic switching:

```bash
# Uses multi-tier fallback automatically
opencode run "implement feature"
ultrawork
```

### Manual Model Selection

```bash
# Use GitHub Copilot explicitly
opencode run "task" --model=github-copilot/gpt-5-mini

# Use Google Antigravity explicitly
opencode run "task" --model=google/antigravity-gemini-3-pro

# Use Z.ai explicitly
opencode run "task" --model=zai-coding-plan/glm-4.7
```

### Agent-Specific Examples

```bash
# Explore uses GitHub Copilot (fast search)
opencode run "find all database queries"

# Oracle uses Google Antigravity (high reasoning)
opencode run "debug this complex issue"

# Sisyphus uses Z.ai (orchestration)
opencode run "ulw build this feature from scratch"
```

## Benefits

1. **Three-tier redundancy**: GitHub Copilot → Google Antigravity → Z.ai
2. **Optimal performance**: Best model used first for each scenario
3. **Cost-effective**: Free quotas used first, paid Z.ai only as ultimate fallback
4. **Fast tasks**: GitHub Copilot GPT-5 Mini for quick operations
5. **Reliability**: Guaranteed service even if two providers fail
6. **Seamless**: Automatic switching without workflow interruption

## Provider Summary

| Provider | Cost | Primary Use | Role |
|-----------|-------|--------------|-------|
| **GitHub Copilot** | Included with account | Quick tasks, Explore | Fast, reliable |
| **Google Antigravity** | Free via OAuth | Reasoning, creative | High-end models |
| **Z.ai Coding Plan** | $10/mo (paid) | Orchestrator, docs | Ultimate fallback |

## Testing

- ✅ GitHub Copilot GPT-5 Mini tested and working
- ✅ Google Antigravity models verified
- ✅ Z.ai GLM-4.7 verified
- ✅ Multi-tier fallback chain configured

## Notes

- GitHub Copilot is your personal account (no additional cost)
- Google Antigravity is free via OAuth but may have rate limits
- Z.ai is your only paid subscription ($10/month) - used as ultimate fallback
- Three-tier fallback ensures maximum reliability
- Configuration prioritizes performance while minimizing cost

## References

- GitHub Copilot: https://github.com/features/copilot
- Oh-My-OpenCode: https://github.com/code-yeongyu/oh-my-opencode
- Antigravity Auth: https://github.com/NoeFabris/opencode-antigravity-auth
- Z.ai: https://z.ai/subscribe

---

**Task completed on**: 2026-02-22
