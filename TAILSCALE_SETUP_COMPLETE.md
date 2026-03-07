# Tailscale Setup for OpenCode - Complete Guide

**Branch:** `feat/tailscale-opencv-access`
**Date:** 2026-03-06

## What Has Been Configured

✅ **Documentation Created**
- Comprehensive setup guide (349 lines)
- Quick reference for 5-minute setup (78 lines)
- Setup instructions summary (142 lines)
- Helper scripts directory with 6 scripts
- SSH configuration example

✅ **Automated Scripts Created**
- `.tailscale/install_tailscale.sh` - Automated installation
- `.tailscale/enable_tailscale_ssh.sh` - Enable SSH access
- `.tailscale/start_opencode_server.sh` - Start OpenCode server
- `.tailscale/tailscale_status.sh` - Check status
- `.tailscale/ssh_config_example` - SSH config template

✅ **Everything Committed & Pushed**
- All documentation in `notes/` folder
- All helper scripts in `.tailscale/` folder
- Pushed to GitHub branch

## Quick Start (5 minutes)

### Step 1: Install Tailscale (2 minutes)

```bash
# Make script executable
chmod +x .tailscale/install_tailscale.sh

# Run installer (will prompt for password)
sudo .tailscale/install_tailscale.sh
```

**OR** install manually:
```bash
brew install --cask tailscale
```

### Step 2: Login to Tailscale (1 minute)

1. Click Tailscale icon in menu bar (top right)
2. Click "Log in..."
3. Choose Google/GitHub/Microsoft (Google recommended)
4. Wait for "Connected" status (green indicator)

### Step 3: Enable SSH Access (1 minute)

```bash
# Make script executable
chmod +x .tailscale/enable_tailscale_ssh.sh

# Enable SSH
sudo .tailscale/enable_tailscale_ssh.sh
```

### Step 4: Get Your Tailscale IP

```bash
tailscale ip -4
```
Note this IP (e.g., `100.x.x.x`)

### Step 5: Install on Phone (1 minute)

**iOS:**
1. Open App Store
2. Search "Tailscale"
3. Install and open
4. Log in with SAME account as Mac

**Android:**
1. Open Play Store
2. Search "Tailscale"
3. Install and open
4. Log in with SAME account as Mac

### Step 6: Test Remote Access

**Option A: SSH from Phone**
1. Install SSH app on phone:
   - iOS: Termius, Prompt, Blink Shell
   - Android: Termux, JuiceSSH
2. Connect:
   ```bash
   ssh luoma@100.x.x.x
   ```
   Replace `100.x.x.x` with your Tailscale IP

**Option B: Remote Desktop**
1. Open Tailscale app on phone
2. Tap your Mac device
3. Select "Remote Desktop" or "Share Screen"

## Accessing OpenCode from Phone

### Method 1: Direct Browser Access

1. Start OpenCode server on Mac:
   ```bash
   chmod +x .tailscale/start_opencode_server.sh
   .tailscale/start_opencode_server.sh
   ```

2. On phone browser:
   ```
   http://100.x.x.x:7269
   ```
   Replace with your Tailscale IP

### Method 2: SSH + Port Forwarding

1. SSH from phone to Mac:
   ```bash
   ssh -L 8080:localhost:7269 luoma@100.x.x.x
   ```

2. On phone browser:
   ```
   http://localhost:8080
   ```

### Method 3: Remote Desktop

1. Connect via Tailscale Remote Desktop
2. Use Mac's browser normally

## Helper Scripts Reference

| Script | Purpose | Usage |
|--------|---------|-------|
| `install_tailscale.sh` | Install Tailscale | `sudo ./.tailscale/install_tailscale.sh` |
| `enable_tailscale_ssh.sh` | Enable SSH access | `sudo ./.tailscale/enable_tailscale_ssh.sh` |
| `start_opencode_server.sh` | Start OpenCode server | `./.tailscale/start_opencode_server.sh` |
| `tailscale_status.sh` | Check status | `./.tailscale/tailscale_status.sh` |

## Common Commands

```bash
# Check Tailscale status
tailscale status

# Get Tailscale IP
tailscale ip -4

# List connected devices
tailscale status --peers

# Check if server is running
lsof -i :7269

# Check SSH status
sudo systemsetup -getremotelogin
```

## Troubleshooting

### Can't connect from phone

1. Check both devices show "Connected" in Tailscale app
2. Verify Tailscale IP: `tailscale ip -4`
3. Check firewall: System Settings > Network > Firewall

### SSH connection refused

```bash
# Re-enable SSH
sudo tailscale up --ssh=on

# Check SSH service
sudo systemsetup -getremotelogin
```

### Server not accessible

```bash
# Check if running
lsof -i :7269
lsof -i :44376

# Start server
./.tailscale/start_opencode_server.sh
```

## File Structure

```
umbracodemo/
├── .tailscale/
│   ├── README.md                    # This file
│   ├── install_tailscale.sh         # Installation script
│   ├── enable_tailscale_ssh.sh      # SSH enablement script
│   ├── start_opencode_server.sh      # Server startup script
│   ├── tailscale_status.sh           # Status check script
│   ├── ssh_config_example            # SSH config template
│   └── .tailscaleignore            # Git ignore for sensitive files
├── notes/
│   ├── Tailscale-Configuration-for-OpenCode-Remote-Access-2026-03-06.md
│   ├── Tailscale-Quick-Reference-2026-03-06.md
│   └── Tailscale-Setup-Instructions-2026-03-06.md
└── Umbraco13/                      # Your OpenCode project
```

## Phone Apps Required

### iOS
- **Tailscale** (required) - VPN/Network
- **Termius** or **Prompt** or **Blink Shell** - SSH client

### Android
- **Tailscale** (required) - VPN/Network
- **Termux** or **JuiceSSH** - SSH client

## Security Best Practices

✅ Enable 2FA on your Tailscale account
✅ Use strong SSH keys (don't use password auth)
✅ Monitor connected devices regularly
✅ Restrict access via ACLs if needed
✅ Keep Tailscale app updated

## What You Get

After setup, you can:

✅ Access OpenCode from anywhere
✅ SSH into your Mac from your phone
✅ Run commands and code remotely
✅ View OpenCode web interface on phone
✅ Access files on Mac remotely
✅ All traffic encrypted end-to-end
✅ No port forwarding needed
✅ Works on any network (home, office, public WiFi, cellular)

## Next Steps

1. **Complete installation** (5-10 minutes)
   - Run `./.tailscale/install_tailscale.sh`
   - Login to Tailscale
   - Install on phone

2. **Enable SSH access** (1 minute)
   - Run `sudo ./.tailscale/enable_tailscale_ssh.sh`

3. **Test connection** (2 minutes)
   - SSH from phone: `ssh luoma@100.x.x.x`
   - Or use Remote Desktop

4. **Start OpenCode** (1 minute)
   - Run `./.tailscale/start_opencode_server.sh`
   - Access at `http://100.x.x.x:7269`

## Support & Documentation

- **Full Guide:** `notes/Tailscale-Configuration-for-OpenCode-Remote-Access-2026-03-06.md`
- **Quick Reference:** `notes/Tailscale-Quick-Reference-2026-03-06.md`
- **Setup Summary:** `notes/Tailscale-Setup-Instructions-2026-03-06.md`
- **Tailscale Docs:** https://tailscale.com/kb/
- **Tailscale Support:** https://tailscale.com/support

## GitHub Repository

All files pushed to:
https://github.com/maluo/umbracodemo/tree/feat/tailscale-opencv-access

## Summary

✅ All documentation created
✅ All helper scripts created
✅ Everything committed and pushed
✅ Ready for you to complete setup in 10-15 minutes

**You just need to run the installation scripts and follow the quick start steps above!**
