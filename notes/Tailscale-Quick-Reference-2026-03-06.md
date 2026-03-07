# Tailscale Setup Quick Reference
**Generated:** 2026-03-06

## Quick Start (5 minutes)

### 1. Install on Mac
```bash
brew install --cask tailscale
# OR download from https://tailscale.com/download/macos
```

### 2. Create Account & Login
- Click Tailscale icon in menu bar
- Log in with Google/GitHub/Microsoft
- Wait for "Connected" status (green)

### 3. Install on Phone
- **iOS:** Download from App Store
- **Android:** Download from Play Store
- Log in with SAME account as Mac

### 4. Enable SSH Access (Mac terminal)
```bash
sudo tailscale up --ssh=on
```

### 5. Get Mac's Tailscale IP
```bash
tailscale ip -4
# Note: 100.x.x.x
```

### 6. Connect from Phone
```bash
# From phone's terminal/SSH app
ssh luoma@100.x.x.x
# Replace 100.x.x.x with actual IP from step 5
```

### 7. Access OpenCode
Once SSH'd into Mac from phone:
```bash
cd /Users/luoma/Downloads/backup\ Nov\ 22\ 2025/PVE/Umbraco/umbracodemo/Umbraco13
dotnet run
# Access at http://100.x.x.x:7269 from phone browser
```

## Alternative: Remote Desktop

On phone Tailscale app:
1. Tap your Mac device
2. Select "Remote Desktop" or "Share Screen"
3. Use Mac's desktop from your phone

## Key Commands

| Command | Purpose |
|---------|---------|
| `tailscale status` | Check connection |
| `tailscale ip -4` | Get Tailscale IP |
| `sudo tailscale up --ssh=on` | Enable SSH |
| `lsof -i :7269` | Check if OpenCode running |

## Device Info

- **Mac Username:** `luoma`
- **Project Path:** `/Users/luoma/Downloads/backup Nov 22 2025/PVE/Umbraco/umbracodemo`
- **OpenCode Port:** 7269
- **Your Tailscale IP:** (run `tailscale ip -4`)

## Troubleshooting

**Can't connect?**
- Check Mac firewall: System Settings > Network > Firewall
- Verify Tailscale shows "Connected" on both devices
- Re-enable SSH: `sudo tailscale up --ssh=on`

**Full Documentation:** See `notes/Tailscale-Configuration-for-OpenCode-Remote-Access-2026-03-06.md`
