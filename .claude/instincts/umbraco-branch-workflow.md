---
id: umbraco-branch-workflow
trigger: "when starting feature development or about to modify code"
confidence: 0.95
domain: git-workflow
source: umbraco-project-standards
project: umbracodemo
---

# Always Create Feature Branch Before Code Changes

## Context
This is an Umbraco project where all feature development must follow GitHub branch workflow. The project is hosted at https://github.com/maluo/umbracodemo

## Action

### BEFORE making any code changes:

1. **Check current branch**
   ```bash
   git branch --show-current
   ```

2. **If on `main` branch:**
   - ✅ Create a feature branch FIRST
   - ❌ DO NOT make any code changes on `main`

3. **Create feature branch with proper naming:**
   ```bash
   git checkout -b {type}/{short-description}
   ```

   **Types:**
   - `feat/` - New features
   - `fix/` - Bug fixes
   - `refactor/` - Code refactoring
   - `docs/` - Documentation
   - `test/` - Test improvements
   - `chore/` - Maintenance

4. **Examples of good branch names:**
   - `feat/excel-partial-rich-text`
   - `fix/pdf-header-alignment`
   - `refactor/export-services-cleanup`

## After Development

1. **Build and test:**
   ```bash
   dotnet build Umbraco13/Umbraco13.csproj
   dotnet build API/FundsApi/FundsApi.csproj
   ```

2. **Log task completion (MANDATORY):**
   ```bash
   /task_logger "Feature Name - YYYY-MM-DD"
   ```
   - Creates comprehensive task log in `notes/` folder
   - Documents implementation, changes, and technical details
   - Provides project history and reference

3. **Commit with conventional format:**
   ```bash
   git commit -m "feat: add partial rich text formatting"
   ```

4. **Push to remote:**
   ```bash
   git push -u origin feat/your-feature
   ```

5. **Create pull request for review**

## Evidence

- Project requires code review before merging
- Prevents breaking `main` branch
- Enables parallel development
- Standard Git/GitHub workflow
- All previous commits follow branch-based workflow

## Consequences

**❌ WRONG (Violating this instinct):**
```bash
git checkout main
# Make changes directly
git commit -am "quick fix"
git push origin main
```

**✅ RIGHT (Following this instinct):**
```bash
git checkout main
git pull origin main
git checkout -b fix/quick-fix
# Make changes
git commit -m "fix: resolve issue"
git push -u origin fix/quick-fix
gh pr create
```

## Related

- Skill: `umbraco-github-branch-workflow`
- Domain: Git, GitHub, CI/CD
