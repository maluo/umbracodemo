---
name: umbraco-github-branch-workflow
description: Enforces GitHub branch workflow for feature development in Umbraco projects. Every feature must be developed on a separate branch before merging. Includes task logging for documentation.
version: 1.1.0
author: Claude Code
---

# Umbraco GitHub Branch Workflow

## Overview

This skill enforces a strict GitHub branch workflow for all feature development in the Umbraco project. **Every feature, bug fix, or enhancement must be developed on a separate branch** - never directly on `main`.

**Key Requirements:**
1. ✅ Always create a feature branch before making changes
2. ✅ Use conventional commit format
3. ✅ Build and test before pushing
4. ✅ **Log task completion after each feature** (MANDATORY)
5. ✅ Create pull request for review

## Workflow Steps

### 1. Pre-Development Checklist

Before starting any work:

```bash
# Ensure you're on main and it's up to date
git checkout main
git pull origin main

# Check for uncommitted changes
git status
```

**IF you have uncommitted changes:**
- Commit them first, OR
- Stash them with `git stash`

### 2. Create Feature Branch

Create a new branch with a descriptive name following this pattern:

```
{type}/{short-description}
```

**Branch types:**
- `feat/` - New features
- `fix/` - Bug fixes
- `refactor/` - Code refactoring
- `docs/` - Documentation updates
- `test/` - Test additions or improvements
- `chore/` - Maintenance tasks

**Examples:**
```bash
# Good branch names
git checkout -b feat/excel-partial-rich-text
git checkout -b fix/pdf-header-alignment
git checkout -b refactor/export-services-cleanup
git checkout -b docs/api-documentation

# Bad branch names (too vague)
git checkout -b feature-branch
git checkout -b stuff
git checkout -b test
```

### 3. Development Phase

Work on your feature in the created branch:

```bash
# Make your changes
# ... edit files ...

# Stage changes
git add .

# Commit with conventional commit format
git commit -m "feat: add partial rich text formatting for Excel export"
```

**Commit message format:**
```
{type}: {description}

{optional detailed body}
```

**Types:** `feat:`, `fix:`, `refactor:`, `docs:`, `test:`, `chore:`

### 4. Build and Test

Before pushing, verify everything works:

```bash
# Build Umbraco project
dotnet build Umbraco13/Umbraco13.csproj

# Build API project (if applicable)
dotnet build API/FundsApi/FundsApi.csproj

# Run tests (if available)
dotnet test
```

**IF build fails:**
- Fix the errors
- Commit the fixes
- Rebuild until successful

### 5. Log Task Completion

**MANDATORY**: After each feature is complete, log the task using the task_logger skill:

```bash
# Use task_logger skill to create a comprehensive log
# Format: task_logger "Feature Name - YYYY-MM-DD"
```

**The log includes:**
- Task checklist with completion status
- Implementation details and technical approach
- Code changes summary
- Build verification results
- Usage examples
- Git workflow followed

**Why this is important:**
- Creates a record of what was done and why
- Documents technical decisions and trade-offs
- Provides reference for future maintenance
- Tracks project history and patterns

**Example:**
```bash
# After completing a feature
/task_logger "PDF Metadata Feature - 2026-02-08"
```

**Log location:** `notes/FeatureName-YYYY-MM-DD.md`

### 6. Push to Remote

Push your branch to GitHub:

```bash
git push -u origin feat/excel-partial-rich-text
```

**The `-u` flag sets up tracking for the first push only.**

### 7. Create Pull Request

After pushing, create a pull request:

```bash
# Using GitHub CLI (if installed)
gh pr create --title "feat: Add partial rich text formatting for Excel" --body "Implements partial bold formatting with ** markers"

# Or open GitHub and create PR via web interface
open "https://github.com/maluo/umbracodemo/compare/main...feat-excel-partial-rich-text"
```

### 8. Code Review & Merge

**Wait for:**
- Code review approval
- CI/CD checks to pass
- Any requested changes to be addressed

**After approval:**
```bash
# Merge via GitHub web UI (preferred)
# Or via command line:
git checkout main
git merge feat/excel-partial-rich-text
git push origin main
```

### 9. Cleanup

After merging, delete the feature branch:

```bash
# Delete local branch
git branch -d feat/excel-partial-rich-text

# Delete remote branch
git push origin --delete feat-excel-partial-rich-text
```

## Rules & Enforcements

### ❌ FORBIDDEN

1. **NEVER commit directly to `main`**
   ```bash
   # WRONG - Don't do this!
   git checkout main
   git commit -am "quick fix"
   ```

2. **NEVER push uncommitted work to `main`**
   ```bash
   # WRONG - Don't do this!
   git push origin main
   ```

3. **NEVER create PR from `main`**
   ```bash
   # WRONG - Don't do this!
   gh pr create --base main --head main
   ```

### ✅ REQUIRED

1. **ALWAYS create a branch for any work**
   ```bash
   # RIGHT - Do this!
   git checkout -b feat/my-feature
   ```

2. **ALWAYS commit with conventional commit format**
   ```bash
   # RIGHT - Do this!
   git commit -m "feat: add user authentication"
   git commit -m "fix: resolve null reference exception"
   git commit -m "docs: update API documentation"
   ```

3. **ALWAYS build and test before pushing**
   ```bash
   # RIGHT - Do this!
   dotnet build Umbraco13/Umbraco13.csproj
   dotnet build API/FundsApi/FundsApi.csproj
   ```

4. **ALWAYS create PR for review before merging**
   ```bash
   # RIGHT - Do this!
   gh pr create --title "feat: description" --body "details..."
   ```

## Pre-Commit Hook (Optional)

To enforce this workflow, create a `.git/hooks/pre-commit` file:

```bash
#!/bin/bash

# Get current branch name
BRANCH=$(git rev-parse --abbrev-ref HEAD)

# Prevent commits to main
if [ "$BRANCH" = "main" ]; then
    echo "❌ ERROR: Cannot commit directly to 'main' branch!"
    echo "Please create a feature branch first:"
    echo "  git checkout -b feat/your-feature"
    exit 1
fi

echo "✅ Commit allowed on branch: $BRANCH"
exit 0
```

Make it executable:
```bash
chmod +x .git/hooks/pre-commit
```

## Examples

### Example 1: Adding a New Feature

```bash
# Step 1: Start from main
git checkout main
git pull origin main

# Step 2: Create feature branch
git checkout -b feat/excel-partial-rich-text

# Step 3: Make changes
# ... edit files ...

# Step 4: Commit changes
git add .
git commit -m "feat: add partial rich text formatting for Excel disclaimer"

# Step 5: Build and test
dotnet build Umbraco13/Umbraco13.csproj

# Step 6: Log task completion (MANDATORY)
/task_logger "Excel Partial Rich Text Feature - 2026-02-08"

# Step 7: Push branch
git push -u origin feat/excel-partial-rich-text

# Step 8: Create PR
gh pr create --title "feat: Add partial rich text formatting for Excel" \
  --body "Implements partial bold formatting with ** markers in disclaimer text"
```

### Example 2: Fixing a Bug

```bash
# Step 1: Start from main
git checkout main
git pull origin main

# Step 2: Create fix branch
git checkout -b fix/pdf-header-alignment

# Step 3: Make changes
# ... edit files ...

# Step 4: Commit changes
git add .
git commit -m "fix: correct PDF header text alignment"

# Step 5: Build and test
dotnet build Umbraco13/Umbraco13.csproj

# Step 6: Log task completion (MANDATORY)
/task_logger "PDF Header Alignment Fix - 2026-02-08"

# Step 7: Push branch
git push -u origin fix/pdf-header-alignment

# Step 7: Create PR
gh pr create --title "fix: Correct PDF header text alignment" \
  --body "Resolves issue where header text was not properly aligned"
```

### Example 3: Refactoring Code

```bash
# Step 1: Start from main
git checkout main
git pull origin main

# Step 2: Create refactor branch
git checkout -b refactor/export-services-cleanup

# Step 3: Make changes
# ... edit files ...

# Step 4: Commit changes
git add .
git commit -m "refactor: simplify export service architecture"

# Step 5: Build and test
dotnet build Umbraco13/Umbraco13.csproj

# Step 6: Push branch
git push -u origin refactor/export-services-cleanup

# Step 7: Create PR
gh pr create --title "refactor: Simplify export service architecture" \
  --body "Cleans up duplicate code and improves maintainability"
```

## Troubleshooting

### Issue: "Failed to push some refs"

```bash
# Someone else pushed to main, you need to update your feature branch
git checkout main
git pull origin main
git checkout feat/your-feature
git merge main
# Resolve conflicts if any
git push origin feat/your-feature
```

### Issue: "Branch already exists"

```bash
# The remote branch already exists, check it out locally
git checkout feat/existing-branch
git pull origin feat/existing-branch
```

### Issue: Committed to main by mistake

```bash
# If you haven't pushed yet
git reset --soft HEAD~1
git checkout -b feat/your-feature
git commit -m "feat: your changes"

# If you already pushed
git revert HEAD
git push origin main
git checkout -b feat/your-feature
# Make your changes again properly
```

## Quick Reference

| Command | Purpose |
|---------|---------|
| `git checkout -b feat/name` | Create feature branch |
| `git status` | Check current branch and changes |
| `git add . && git commit -m "type: desc"` | Commit changes |
| `git push -u origin branch-name` | Push branch to remote |
| `gh pr create` | Create pull request |
| `git branch -d branch-name` | Delete local branch |
| `git push origin --delete branch-name` | Delete remote branch |

## Related Skills

- `everything-claude-code:git-workflow` - General Git workflow patterns
- `everything-claude-code:security-review` - Security review before merging
- `everything-claude-code:code-reviewer` - Automated code review

## Metadata

- **Repository:** https://github.com/maluo/umbracodemo
- **Project:** Umbraco 13 Demo
- **Language:** C# / .NET 8
- **Target Framework:** Umbraco CMS 13.x

---

*Generated for Umbraco project development workflow enforcement*
