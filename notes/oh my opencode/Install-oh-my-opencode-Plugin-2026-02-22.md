# Task: Install oh-my-opencode Plugin - 2026-02-22

## Implementation Plan

1. Check user's subscription status
2. Install oh-my-opencode plugin with appropriate flags
3. Configure agent models to use antigravity-auth plugin
4. Test the installation

## Change Log

### Files Modified

- `C:\Users\james\.config\opencode\opencode.json` - Added oh-my-opencode plugin
- `C:\Users\james\.config\opencode\oh-my-opencode.json` - Updated agent models to use antigravity

### Changes Made

1. **Plugin Installation**
   - Added `oh-my-opencode` to plugin array in opencode.json
   - User already had `opencode-antigravity-auth@latest` installed

2. **User Subscriptions**
   - Claude: No
   - OpenAI/ChatGPT: No
   - Gemini: Yes (via antigravity-auth)
   - GitHub Copilot: No
   - OpenCode Zen: No
   - Z.ai Coding Plan: Yes

3. **Agent Model Configuration (oh-my-opencode.json)**
   - **Sisyphus**: `zai-coding-plan/glm-4.7` (orchestrator, no Claude fallback)
   - **Oracle**: `google/antigravity-gemini-3-pro` (variant: high)
   - **Librarian**: `zai-coding-plan/glm-4.7`
   - **Explore**: `google/antigravity-gemini-3-flash`
   - **Multimodal-looker**: `google/antigravity-gemini-3-flash` (variant: medium)
   - **Prometheus**: `google/antigravity-gemini-3-pro`
   - **Metis**: `google/antigravity-gemini-3-pro` (variant: high)
   - **Momus**: `google/antigravity-gemini-3-pro` (variant: high)
   - **Atlas**: `google/antigravity-gemini-3-pro`

4. **Category Model Configuration**
   - All categories updated to use antigravity models instead of CLI preview models
   - `visual-engineering`, `ultrabrain`, `artistry`: `google/antigravity-gemini-3-pro` (variant: high)
   - `quick`, `unspecified-low`, `unspecified-high`, `writing`: `google/antigravity-gemini-3-flash`

5. **Installation Command**
   ```bash
   npx oh-my-opencode install --no-tui --claude=no --openai=no --gemini=yes --copilot=no --opencode-zen=no --zai-coding-plan=yes
   ```

## Usage Examples

### Basic Usage

```bash
# Start oh-my-opencode
opencode

# Use ultrawork mode (activates all agents)
ultrawork

# Or short form
ulw
```

### Agent-Specific Usage

```bash
# Use Sisyphus (main orchestrator)
opencode run "fix the bug in user auth"

# Use Prometheus (planner)
/start-work

# Use Explore (fast codebase search)
opencode run "find all references to UserRepository"
```

## Technical Details

### Available Agents

| Agent | Model | Purpose |
|-------|-------|---------|
| **Sisyphus** | zai-coding-plan/glm-4.7 | Main orchestrator, delegates tasks |
| **Oracle** | google/antigravity-gemini-3-pro | Architecture, debugging, reasoning |
| **Librarian** | zai-coding-plan/glm-4.7 | Documentation, code search |
| **Explore** | google/antigravity-gemini-3-flash | Fast codebase grep |
| **Multimodal-looker** | google/antigravity-gemini-3-flash | Image analysis, visual tasks |
| **Prometheus** | google/antigravity-gemini-3-pro | Strategic planner, interview mode |
| **Metis** | google/antigravity-gemini-3-pro | Plan consultant |
| **Momus** | google/antigravity-gemini-3-pro | Critique agent |
| **Atlas** | google/antigravity-gemini-3-pro | Architecture specialist |

### Key Features

1. **ultrawork / ulw** - One-word command that activates all agents, executes tasks relentlessly until done

2. **Discipline Agents** - Sisyphus orchestrates Hephaestus, Oracle, Librarian, Explore as a full AI dev team in parallel

3. **IntentGate** - Analyzes true user intent before classifying or acting

4. **Hash-Anchored Edit Tool** - LINE#ID content hash validates every change, zero stale-line errors

5. **LSP + AST-Grep** - Workspace rename, pre-build diagnostics, AST-aware rewrites

6. **Background Agents** - Fire 5+ specialists in parallel, context stays lean

7. **Built-in MCPs** - Exa (web search), Context7 (official docs), Grep.app (GitHub search)

8. **Ralph Loop / /ulw-loop** - Self-referential loop, doesn't stop until 100% done

9. **Todo Enforcer** - Agent goes idle? System yanks it back, task gets done

10. **Prometheus Planner** - Interview-mode strategic planning before any execution

### Configuration Files

- Main config: `C:\Users\james\.config\opencode\opencode.json`
- Oh-my-opencode config: `C:\Users\james\.config\opencode\oh-my-opencode.json`
- Antigravity config: `C:\Users\james\.config\opencode\antigravity.json`

## Notes

- ⚠️ **Critical Warning**: Sisyphus agent is STRONGLY optimized for Claude Opus 4.5. Without Claude, you may experience significantly degraded performance

- The installation automatically configured agent models based on provider priority: Native (anthropic/, openai/, google/) > GitHub Copilot > OpenCode Zen > Z.ai Coding Plan

- All Gemini-based agents now use Antigravity models instead of Gemini CLI preview models for better performance

- Full Claude Code compatibility: hooks, commands, skills, MCPs, and plugins all work unchanged

## Free Advertising

**ELESTYLE** - Making elepay (multi-mobile payment gateway) and OneQR (mobile application SaaS for cashless solutions)
- Website: https://elestyle.jp

Get your company featured for free by contributing to oh-my-opencode: https://github.com/code-yeongyu/oh-my-opencode/compare

## Tutorial

### Key Tips

1. **Sisyphus agent strongly recommends Opus 4.5 model**. Using other models may result in significantly degraded experience.

2. **Feeling lazy?** Just include `ultrawork` (or `ulw`) in your prompt. The agent figures out the rest.

3. **Need precision?** Press **Tab** to enter Prometheus (Planner) mode, create a work plan through an interview process, then run `/start-work` to execute it with full orchestration.

4. **Want more info?** Checkout: https://github.com/code-yeongyu/oh-my-opencode/blob/master/docs/guide/overview.md

### Useful Commands

```bash
# Start with all agents activated
ultrawork

# Create a strategic plan before execution
/start-work

# Deep initialization - generates AGENTS.md files
/init-deep

# Use Ralph Loop for relentless execution
/ulw-loop

# List available commands
/help
```

## Verification

Installation verified successfully:
- ✅ OpenCode 1.2.10 detected
- ✅ oh-my-opencode plugin added
- ✅ Agent models configured for antigravity
- ✅ Test command successful

## References

- Repository: https://github.com/code-yeongyu/oh-my-opencode
- Installation Guide: https://github.com/code-yeongyu/oh-my-opencode/blob/master/docs/guide/installation.md
- Overview: https://github.com/code-yeongyu/oh-my-opencode/blob/master/docs/guide/overview.md
- Documentation: https://github.com/code-yeongyu/oh-my-opencode/blob/master/docs/reference/features.md

---

**Task completed on**: 2026-02-22
