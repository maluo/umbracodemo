# Claude Code Skills and Instincts

This folder contains Claude Code skills and instincts specifically configured for Umbraco project.

## Skills

Located in `.claude/skills/`:

### 1. umbraco-github-branch-workflow
**Enforces GitHub branch workflow** for all feature development in this project.

**Key Requirements:**
1. Always create a feature branch before making changes
2. Use conventional commit format (`feat:`, `fix:`, etc.)
3. Build and test before pushing
4. **Log task completion after each feature** (MANDATORY)
5. Create pull request for review

**Usage:**
The skill is automatically activated when Claude detects development work in this project.

### 2. task_logger
**Creates comprehensive task logs** for completed work.

**Output:**
- Creates markdown logs in `notes/` folder
- Format: `FeatureName-YYYY-MM-DD.md`
- Includes: task checklist, implementation details, code changes, build results

**Usage:**
Invoke with `/task_logger "Feature Name - YYYY-MM-DD"` after completing a feature.

### 3. tailscale_setup
**Automates Tailscale configuration** for OpenCode remote access from phone.

**Capabilities:**
- Automated Tailscale installation via Homebrew
- SSH access enablement with validation
- Phone setup guidance (iOS/Android)
- Connection testing and troubleshooting
- Server management for OpenCode

**Output:**
- Installs Tailscale on macOS
- Enables SSH access through Tailscale
- Displays Tailscale IP for remote connection
- Provides phone setup instructions
- Guides through connection testing

**Usage:**
Invoke with `/skill tailscale_setup` to start automated setup wizard.

## Instincts

Located in `.claude/instincts/`:

### umbraco-branch-workflow
**Automatically enforces branch workflow** when development is detected.

**Behavior:**
- Checks current branch before making changes
- Prevents commits to `main` branch
- Guides through proper feature branch creation
- Reminds about task logging, building, and PR creation

## Project Structure

```
.claude/
├── skills/
│   ├── umbraco-github-branch-workflow.md    # Branch workflow enforcement
│   ├── task_logger/                          # Task logging skill
│   │   └── SKILL.md
│   └── tailscale_setup/                      # Tailscale setup skill
│       └── SKILL.md
├── instincts/
│   └── umbraco-branch-workflow.md            # Branch workflow instinct
└── settings.local.json                       # Local settings (gitignored)
```

## Workflow Summary

For any feature development in this project:

1. ✅ Create feature branch: `git checkout -b feat/your-feature`
2. ✅ Make changes and commit: `git commit -m "feat: description"`
3. ✅ Build and test: `dotnet build Umbraco13/Umbraco13.csproj`
4. ✅ **Log task**: `/task_logger "Feature Name - 2026-02-08"`
5. ✅ Push to remote: `git push -u origin feat/your-feature`
6. ✅ Create pull request for review

For Tailscale setup:
1. ✅ Invoke skill: `/skill tailscale_setup`
2. ✅ Follow wizard prompts
3. ✅ Complete phone setup
4. ✅ Test remote access
5. ✅ **Log task**: `/task_logger "Tailscale Setup - 2026-03-06"`

## Notes

- Skills and instincts in this folder override global Claude Code settings
- Project-specific configurations take precedence over global configurations
- All task logs are stored in `notes/` folder at project root
- These skills ensure consistent development practices across the project
- Tailscale setup skill provides complete automation for remote access configuration

## Related Documentation

- [Git Workflow](../README.md)
- [Project Notes](../notes/)
- [Development Guidelines](../docs/)
- [Tailscale Setup](../.tailscale/)
