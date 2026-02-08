# Claude Code Skills and Instincts

This folder contains Claude Code skills and instincts specifically configured for the Umbraco project.

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
│   └── task_logger/                          # Task logging skill
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

## Notes

- Skills and instincts in this folder override global Claude Code settings
- Project-specific configurations take precedence over global configurations
- All task logs are stored in the `notes/` folder at project root
- These skills ensure consistent development practices across the project

## Related Documentation

- [Git Workflow](../README.md)
- [Project Notes](../notes/)
- [Development Guidelines](../docs/)
