---
name: GLM Token Usage Checker
description: Check GLM (Z.AI) token usage limits and remaining quota.
---

# GLM Token Usage Checker Skill

This skill allows you to check your GLM (Z.AI) token usage, remaining quota, and reset time for your coding plan.

## Capabilities
- Check remaining token quota in 5-hour window
- View current usage vs total limit
- Display time until quota reset
- Show historical usage (last 7 days)
- Check API key authentication status
- **Display in psmux split pane** for easy monitoring while working

## Requirements
- **API Key**: GLM API key from Z.AI
- **Environment Variable**: Set `ZAI_API_KEY` or `ZHIPUAI_API_KEY`
- **Optional**: [psmux](https://github.com/ronilaukkarinen/psmux) for split pane display

## Setup

### 1. Get Your API Key
1. Log in to [z.ai](https://z.ai)
2. Navigate to [API Keys](https://z.ai/manage-apikey)
3. Copy your API key

### 2. Set Environment Variable

**macOS/Linux (bash/zsh):**
```bash
export ZAI_API_KEY="your-api-key-here"
```

**For persistent settings (macOS/Linux):**
```bash
echo 'export ZAI_API_KEY="your-api-key-here"' >> ~/.zshrc  # or ~/.bashrc
source ~/.zshrc
```

**Windows (PowerShell):**
```powershell
$env:ZAI_API_KEY = "your-api-key-here"
```

**Windows (Command Prompt):**
```cmd
set ZAI_API_KEY=your-api-key-here
```

**For persistent settings (Windows):**
```powershell
[System.Environment]::SetEnvironmentVariable('ZAI_API_KEY', 'your-api-key-here', 'User')
```

## Usage

### Basic Check

Run the script to check your usage:

```bash
python3 .agent/skills/glm-usage-checker/scripts/glm_usage_checker.py
```

### Display in Psmux Split Pane

Open the usage table in a new psmux split pane:

```bash
python3 .agent/skills/glm-usage-checker/scripts/glm_usage_checker.py --split
# or
python3 .agent/skills/glm-usage-checker/scripts/glm_usage_checker.py -s
```

### Watch Mode (Auto-refresh)

Continuously monitor usage with auto-refresh every 30 seconds:

```bash
python3 .agent/skills/glm-usage-checker/scripts/glm_usage_checker.py --split --watch
```

### Example Output

```
GLM Token Usage Check
==================================================

[OK] Connected

[*] Plan Level: lite

[*] Token Quota (5-hour window):
  Used: 15%
  Remaining: 85%
  Resets in: 1h 30m

[*] Request Quota:
  Used: 6 out of 100
  Remaining: 94 (6%)

[*] Usage Details:
  - search-prime: 5 calls
  - web-reader: 1 call
  - zread: 0 calls

[->] Dashboard: https://z.ai/manage-apikey/billing
```

## API Endpoints Used

- **Quota Limit**: `https://api.z.ai/api/monitor/usage/quota/limit`
- **Model Usage**: `https://api.z.ai/api/monitor/usage/model-usage`

## Troubleshooting

### Error: No credentials found
- Make sure you've set the `ZAI_API_KEY` environment variable
- Verify the API key is correct

### Error: Invalid API key
- Check that your API key is valid and active
- Ensure your subscription is active at [z.ai/billing](https://z.ai/manage-apikey/billing)

### Error: SSL: CERTIFICATE_VERIFY_FAILED (macOS)
- This occurs on macOS when Python doesn't have SSL certificates properly configured
- The script has been updated to handle this by disabling certificate verification
- Make sure you're using `python3` not `python` (which may be Python 2.7)

### Token expired
- If using an OAuth token, you may need to refresh it
- Check your account status on the dashboard

## Related
- [Z.AI Billing](https://z.ai/manage-apikey/billing)
- [GLM API Documentation](https://docs.z.ai)
