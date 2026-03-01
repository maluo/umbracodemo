# Git Rebase vs Merge: A Complete Guide

## Table of Contents
- [Overview](#overview)
- [Merge](#merge)
- [Rebase](#rebase)
- [Squash Merge](#squash-merge)
- [Visual Comparison](#visual-comparison)
- [When to Use Each](#when-to-use-each)
- [GitHub Pull Request Options](#github-pull-request-options)
- [Practical Examples](#practical-examples)
- [Best Practices](#best-practices)

---

## Overview

Both `merge` and `rebase` integrate changes from one branch into another, but they do so in fundamentally different ways:

| Aspect | Merge | Rebase |
|--------|-------|--------|
| **History** | Preserves complete history | Creates linear history |
| **Commits** | Creates merge commit | Rewrites commits |
| **Safety** | Non-destructive | Rewrites history |
| **Complexity** | Can get messy | Cleaner but risky |

---

## Merge

### What It Does

Merge combines two branches by creating a new **merge commit** that has two parents. This preserves the complete history of both branches.

### Before Merge
```
       A---B---C  (main)
             \
              D---E  (feature)
```

### After Merge
```
       A---B---C-----------F  (main)
             \           /
              D---E  (feature)
```

**Commit F is a merge commit with two parents** - C and E.

### Commands

```bash
# Basic merge
git checkout main
git merge feature

# Merge with a custom message
git merge feature -m "Merge feature branch"

# Merge --no-ff (always create merge commit)
git merge --no-ff feature
```

### Characteristics

**Pros:**
- ✅ Preserves true chronological history
- ✅ Shows exactly when and how branches were integrated
- ✅ Non-destructive - safe for public/shared branches
- ✅ Can easily see all commits from a feature branch together
- ✅ Easy to undo if something goes wrong

**Cons:**
- ❌ Creates "diamond" patterns in history with many merges
- ❌ History can become cluttered with merge commits
- ❌ `git log` output can be harder to follow
- ❌ Bisecting can be more difficult

---

## Rebase

### What It Does

Rebase takes all commits from one branch and **replays them** on top of another branch, creating a perfectly linear history.

### Before Rebase
```
       A---B---C  (main)
             \
              D---E  (feature)
```

### After Rebase (on feature branch)
```
       A---B---C---D'---E'  (feature)
                        ↑
                    Commits rewritten
```

Then a **fast-forward merge** brings main up to date:
```
       A---B---C---D'---E'  (main, feature)
```

### Commands

```bash
# Basic rebase
git checkout feature
git rebase main

# Interactive rebase (powerful!)
git rebase -i HEAD~3  # Edit last 3 commits

# Continue after resolving conflicts
git rebase --continue

# Abort rebase
git rebase --abort

# Skip a commit
git rebase --skip
```

### Interactive Rebase Commands

When you run `git rebase -i`, you'll see these options:

| Command | Action |
|---------|--------|
| `pick` | Use commit as-is |
| `reword` | Use commit but edit message |
| `edit` | Use commit but allow modifications |
| `squash` | Combine with previous commit |
| `fixup` | Like squash but discard this commit's log message |
| `exec` | Run shell command |
| `drop` | Remove commit |

### Characteristics

**Pros:**
- ✅ Clean, linear history - easier to read
- ✅ No unnecessary merge commits
- ✅ Easier to `git bisect` to find bugs
- ✅ Commits are in chronological order of changes
- ✅ Can clean up commits during rebase (squash, reword, etc.)

**Cons:**
- ❌ **Rewrites history** - dangerous for shared branches
- ❌ Loses context of when feature development started
- ❌ Must resolve conflicts for each conflicting commit
- ❌ Cannot easily undo once pushed
- ❌ Breaks commit hashes - creates new commit IDs

---

## Squash Merge

### What It Does

Squash merge combines all commits from a feature branch into **a single commit** on the target branch. This creates a clean history where each feature appears as one logical change.

### Before Squash Merge
```
       A---B---C  (main)
             \
              D---E---F  (feature)
```

### After Squash Merge
```
       A---B---C-----------S  (main)
             \
              D---E---F  (feature)

S = Single squashed commit containing changes from D, E, and F
```

### Commands

#### Local Squash Merge

```bash
# Squash merge a branch into current branch
git checkout main
git merge --squash feature

# This stages all changes but doesn't commit
# Commit with a custom message
git commit -m "Add complete user authentication feature"

# Alternative: One-liner
git merge --squash feature && git commit -m "Feature description"
```

#### Squash via Interactive Rebase

```bash
# Squash last N commits interactively
git rebase -i HEAD~3

# Editor opens:
pick abc1234 First commit
pick def5678 Second commit
pick ghi9012 Third commit

# Change to squash (keeps commit messages):
pick abc1234 First commit
squash def5678 Second commit
squash ghi9012 Third commit

# Or fixup (discards commit messages):
pick abc1234 First commit
fixup def5678 Second commit
fixup ghi9012 Third commit

# After saving, you'll edit the combined commit message
```

#### Squash Specific Commits

```bash
# Squash only commits D and E, keep F separate
git rebase -i HEAD~3

# Original:
pick abc1234 Commit D - Add feature
pick def5678 Commit E - Fix typo
pick ghi9012 Commit F - Add tests

# Squash D and E only:
pick abc1234 Commit D - Add feature
squash def5678 Commit E - Fix typo
pick ghi9012 Commit F - Add tests

# Result: Two commits (D+E combined, F separate)
```

#### Auto-Squash with Fixup

```bash
# Mark a commit as a fixup to automatically squash it
git commit --fixup=abc1234

# Then run:
git rebase -i --autosquash HEAD~3

# Git automatically arranges:
pick abc1234 Original commit
fixup def5678 Fixup commit (auto-moved)

# Great for quick fixes without manual rebase!
```

### Advanced Squash Techniques

#### Squash All Commits on a Branch

```bash
# Method 1: Reset and commit
git checkout main
git merge --squash feature
git commit -m "Feature: Complete implementation"

# Method 2: Soft reset
git checkout feature
git reset --soft main~1  # Go back but keep changes staged
git commit -m "Feature: Complete implementation"

# Method 3: Interactive rebase entire branch
git checkout feature
git rebase -i --root  # Rebase from the beginning
# Change all to squash except first
```

#### Squash with Custom Commit Message

```bash
# Squash and provide detailed message
git checkout main
git merge --squash feature

# View what will be committed
git diff --cached

# Commit with multi-line message
git commit -m "Add user authentication

- Implement login/logout functionality
- Add JWT token management
- Create auth middleware
- Add unit tests

Closes #123"
```

#### Selective Squash

```bash
# Squash only specific files from commits
git checkout feature
git log --oneline  # Find commit hashes

# Cherry-pick specific commits and squash
git checkout -b temp-branch main
git cherry-pick commit1
git cherry-pick commit2
git cherry-pick commit3

# Squash them all
git reset --soft HEAD~3
git commit -m "Combined feature changes"

# Replace original branch
git checkout feature
git reset --hard temp-branch
git branch -D temp-branch
```

### Characteristics

**Pros:**
- ✅ Clean, atomic history - one commit per feature
- ✅ Hides WIP commits and false starts
- ✅ Easier to revert entire features
- ✅ Reduces noise in git log
- ✅ Forces meaningful commit messages
- ✅ Great for code review (see feature as a whole)

**Cons:**
- ❌ Loses individual commit granularity
- ❌ Can't easily bisect within a feature
- ❌ Loses context of iterative development
- ❌ Harder to understand what changed within a feature
- ❌ Can't easily cherry-pick parts of a feature

### When to Use Squash Merge

1. **Before creating Pull Requests**
   - Combine multiple WIP commits into logical units
   - Make PRs easier to review

2. **Small, cohesive features**
   - When commits are part of one logical change
   - When individual commits don't add value

3. **Cleanup messy history**
   - After experimentation and refactoring
   - When "fix typo" commits litter your branch

4. **Feature branches**
   - Keep main branch clean with one commit per feature
   - Make git log more readable

### When NOT to Use Squash Merge

1. **Complex features with sub-components**
   - When individual commits have value
   - When you might need to revert parts

2. **Collaborative branches**
   - When multiple people commit to same branch
   - When commit attribution matters

3. **Release branches**
   - When you need to track individual changes
   - When merge conflict resolution matters

### Squash Merge Workflow Examples

#### Example 1: Clean Up Feature Before PR

```bash
# Feature branch with messy commits
git checkout feature
git log --oneline
# * xyz7890 Fix typo in README
# * abc1234 Add tests
# * def5678 Refactor function
# * ghi9012 Add new feature
# * jkl3456 WIP

# Interactive rebase to squash
git rebase -i HEAD~5

# Squash into logical commits:
pick ghi9012 Add new feature
squash def5678 Refactor function
pick abc1234 Add tests
fixup xyz7890 Fix typo in README
drop jkl3456 WIP

# Result: Clean commits ready for PR
```

#### Example 2: Squash Everything to One Commit

```bash
# Feature branch with many commits
git checkout main
git merge --squash feature

# Review changes
git status
git diff --staged

# Commit with descriptive message
git commit -m "Feature: User authentication

Implements complete user authentication system including:
- Login/logout with JWT tokens
- Password reset flow
- Email verification
- Auth middleware for protected routes

Resolves #123"

# Delete feature branch
git branch -d feature
```

#### Example 3: Semi-Linear History with Squash

```bash
# Combine rebase and squash for clean history
git checkout feature

# First, rebase onto latest main
git fetch origin
git rebase origin/main

# Then squash into logical commits
git rebase -i origin/main

# Example result:
# pick abc1234 Implement core feature
# pick def5678 Add tests
# pick ghi9012 Update documentation

# Push (force needed if remote exists)
git push origin feature --force-with-lease

# Create PR, use "Merge commit" to preserve these
```

#### Example 4: Fixup Earlier Commit

```bash
# Oops, forgot to add file to previous commit
git add forgotten-file.txt
git commit -m "Oops: Add forgotten file"

# Squash into previous commit
git rebase -i HEAD~2

# Change to:
pick abc1234 Original commit
fixup def5678 Oops: Add forgotten file

# Result: Only one commit with all changes
```

### GitHub Squash Merge

On GitHub, you can squash merge when completing a PR:

```bash
# Via GitHub UI
# 1. Go to PR page
# 2. Click "Merge pull request" dropdown
# 3. Select "Squash and merge"
# 4. Edit commit message if needed
# 5. Confirm

# Via GitHub CLI
gh pr merge --squash --delete-branch

# With custom message
gh pr merge --squash --message "Feature: Complete implementation"
```

---

## Visual Comparison

### Scenario: Two Features, Hotfix

```
Initial State:
       A---B---C  (main)
             \
              D---E  (feature-a)
             /
        F---G  (feature-b)
```

### Using Merge

```
       A---B---C---H---J  (main)
             \   \   \
              D---E   (feature-a)
             /
        F---G---I  (feature-b)

H = merge of feature-a
I = merge of feature-b
J = merge of hotfix
```

**Result:** You can see the complete branching structure, but it's complex.

### Using Rebase

```
       A---B---C---D'---E'---F'---G'  (main)

All commits linearly replayed in order
```

**Result:** Clean history, but you've lost the context that features were developed in parallel.

---

## When to Use Each

### Use Merge When:

1. **Working on shared/public branches**
   - Never rebase commits that exist outside your repository
   - Others may have based work on those commits

2. **Preserving history is important**
   - When the branch structure tells an important story
   - For release branches, tags, public history

3. **Team collaboration**
   - Pull requests in teams should typically be merged
   - Allows reviewers to see feature work as a unit

### Use Rebase When:

1. **Cleaning up local branches**
   - Before merging your feature into main
   - To incorporate latest changes from main

2. **Maintaining a linear history**
   - For personal projects or teams that prefer linear history
   - When branch structure isn't important

3. **Polishing commits before PR**
   - Squash "typo fix" commits
   - Reorder commits logically
   - Split large commits into smaller ones

### Quick Decision Tree

```
Are commits shared with others?
├── YES → Use MERGE
└── NO
    ├── Do you care about branch context?
    │   ├── YES → Use MERGE
    │   └── NO → Use REBASE
    └── Is this for a PR you're about to submit?
        └── YES → Use REBASE then merge
```

---

## GitHub Pull Request Options

When merging a PR on GitHub, you have three options:

### 1. Merge Commit

```
Before:          After:
A---B---C        A---B---C---M
     \                     /
      D---E               D---E
```

- Creates a merge commit `M`
- Preserves all branch history
- Default and safest option

### 2. Squash and Merge

```
Before:          After:
A---B---C        A---B---C---S
     \
      D---E

S = Combined commit of D and E
```

- Combines all commits into **one** commit
- Cleaner history
- Loses individual commit granularity

### 3. Rebase and Merge

```
Before:          After:
A---B---C        A---B---C---D'---E'
     \
      D---E
```

- Replays commits on top of base branch
- Linear history, no merge commit
- All individual commits preserved

---

## Practical Examples

### Example 1: Rebase Local Feature Branch

**Scenario:** You're working on `feature` while `main` has new commits.

```bash
# Current state
git checkout feature
git log --oneline --graph
# * E (feature)
# * D
# * C (main)
# * B
# * A

# Rebase onto main
git fetch origin
git rebase origin/main

# Result: linear history
git log --oneline --graph
# * E' (feature)
# * D'
# * C (origin/main)
# * B
# * A

# Now merge with fast-forward
git checkout main
git merge feature
```

### Example 2: Interactive Rebase to Clean Up

**Scenario:** You have messy commits you want to clean up before pushing.

```bash
git log --oneline
# * abc1234 Fix typo in function
# * def5678 Add user authentication
# * ghi9012 Add login function
# * jkl3456 WIP

# Interactive rebase last 4 commits
git rebase -i HEAD~4

# Editor opens with:
pick jkl3456 WIP
pick ghi9012 Add login function
pick def5678 Add user authentication
pick abc1234 Fix typo in function

# Edit to squash:
pick jkl3456 WIP
squash ghi9012 Add login function
squash def5678 Add user authentication
squash abc1234 Fix typo in function

# Result: One clean commit
git log --oneline
# * xyz7890 Add user authentication with login
```

### Example 3: Resolve Merge Conflicts

**Scenario:** Both branches changed the same lines.

```bash
git checkout feature
git rebase main
# CONFLICT: content of file.txt

# Edit the file to resolve conflicts
# ... edit file.txt ...

git add file.txt
git rebase --continue

# If you want to give up
git rebase --abort
```

---

## Best Practices

### Golden Rules

1. **NEVER rebase public history**
   ```bash
   # BAD: Rebase commits that are on remote
   git push origin main --force  # DON'T DO THIS!

   # GOOD: Only rebase local/private branches
   git rebase origin/main  # OK if you own the branch
   ```

2. **Force push with caution**
   ```bash
   # If you MUST force push (only on your own branch!)
   git push origin feature --force-with-lease  # Safer than --force
   ```

3. **Communicate with your team**
   - Agree on merge vs rebase workflow
   - Document your team's approach in CONTRIBUTING.md

### Recommended Workflow

For most teams, the **rebase-then-merge** workflow works well:

```bash
# 1. Update your feature branch
git checkout feature
git fetch origin
git rebase origin/main

# 2. Test and fix any issues

# 3. Push updated branch
git push origin feature --force-with-lease

# 4. Create/update PR on GitHub

# 5. Merge using "Merge commit" or "Squash and merge"
```

### Cleaning Up After Merge

```bash
# After merging, delete feature branch
git branch -d feature
git push origin --delete feature

# Or with GitHub CLI
gh pr merge
gh pr close
```

---

## Quick Reference

```bash
# Merge commands
git merge branch              # Basic merge
git merge --no-ff branch      # Always create merge commit
git merge --squash branch     # Squash all commits into one (staged, not committed)

# Squash merge workflow
git checkout main
git merge --squash feature    # Stage all changes
git commit -m "Feature done"  # Commit with message

# Rebase commands
git rebase main               # Rebase onto main
git rebase -i HEAD~3          # Interactive rebase last 3 commits
git rebase -i --autosquash    # Auto-squash with --fixup commits
git rebase --continue         # Continue after resolving conflicts
git rebase --abort            # Cancel rebase
git rebase --skip             # Skip current commit

# Interactive rebase actions
pick                          # Use commit as-is
reword                        # Use commit but edit message
edit                          # Use commit but allow modifications
squash                        # Combine with previous commit (keep messages)
fixup                         # Like squash but discard this commit's message
exec                          # Run shell command
drop                          # Remove commit

# Create fixup commit (for auto-squash)
git commit --fixup=abc1234    # Mark to squash into abc1234
git rebase -i --autosquash    # Auto-arrange fixups

# Pull = fetch + merge
git pull                      # Same as: git fetch; git merge
git pull --rebase             # Same as: git fetch; git rebase
git pull --squash             # Fetch and squash merge

# Configure default behavior
git config --global pull.rebase true    # Always rebase on pull
git config --global merge.ff false      # Always create merge commits
git config --global rebase.autoSquash true  # Enable auto-squash

# GitHub CLI
gh pr merge                   # Merge PR (default method)
gh pr merge --squash          # Squash and merge
gh pr merge --rebase          # Rebase and merge
gh pr merge --delete-branch   # Merge and delete branch

# View staged changes before squash commit
git diff --staged             # Show staged changes
git status                    # Show what will be committed
git log --oneline             # Show commit history

# Reset operations (use with caution!)
git reset --soft HEAD~3       # Reset 3 commits, keep changes staged
git reset --mixed HEAD~3      # Reset 3 commits, keep changes unstaged (default)
git reset --hard HEAD~3       # Reset 3 commits, discard all changes
```

---

## Summary

| Strategy | Best For | Result | Use When |
|----------|----------|--------|----------|
| **Merge** | Shared/public branches | Complete history preserved | Team collaboration, releases, preserving context |
| **Rebase** | Local/private cleanup | Linear history | Before PR submission, personal projects |
| **Squash Merge** | Feature branches | One commit per feature | Clean PRs, hiding WIP commits |
| **Squash + Rebase** | Perfectionists | Perfectly linear, atomic history | Personal workflows, strict standards |
| **Fast Forward** | Simple updates | Linear, no merge commit | No divergent work, simple updates |

### Quick Selection Guide

```
Is this a shared/public branch?
├── YES → Use MERGE (never rewrite history)
└── NO (your private branch)
    ├── Do you want to preserve commit history?
    │   ├── YES → Use MERGE
    │   └── NO → Continue
    ├── Do you want individual commits visible?
    │   ├── YES → Use REBASE
    │   └── NO → Use SQUASH
    └── About to submit PR?
        └── Use REBASE (for order) + SQUASH MERGE on GitHub
```

The best approach depends on your team's workflow and preferences. The most important thing is **consistency** - agree on a workflow and stick to it!

---

## Resources

- [Git Branching and Rebasing](https://git-scm.com/book/en/v2/Git-Branching-Rebasing)
- [GitHub: About pull request merges](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/incorporating-changes-from-a-pull-request/about-pull-request-merges)
- [Atlassian Git Tutorial](https://www.atlassian.com/git/tutorials/merging-vs-rebasing)
