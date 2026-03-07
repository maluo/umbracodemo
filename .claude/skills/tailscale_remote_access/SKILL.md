---
name: tailscale_remote_access
description: Configure Tailscale for remote access from iPhone. Enables SSH, browser, and remote desktop access to your development environment from anywhere.
---

# Tailscale Remote Access Skill

Simple, standalone skill to configure Tailscale for accessing your development environment from iPhone.

## What It Does

- **Installs Tailscale** on your Mac (if not installed)
- **Enables SSH access** for secure remote connections
- **Displays your Tailscale IP** for iPhone connection
- **Provides iPhone setup instructions** (App Store, login, testing)
- **Shows all access methods** (SSH, browser, remote desktop)

## Quick Start

**Invoke the skill:**
```bash
/skill tailscale_remote_access
```

Then from your iPhone:
1. Install Tailscale from App Store
2. Login with same account as Mac
3. Connect via SSH, browser, or remote desktop

## Your Tailscale IP

When configured, your Mac will have a Tailscale IP like:
```
100.x.x.x
```

Use this IP from your iPhone to connect.

## Access Methods

### Method 1: Remote Desktop (Easiest)
No SSH or browser needed - use Tailscale app on iPhone:

1. Open Tailscale app on iPhone
2. Tap on your Mac device
3. Select "Remote Desktop" or "Share Screen"
4. Your Mac's desktop appears on iPhone

**Best for**: Full desktop control, no extra apps needed

### Method 2: Direct Browser Access
Open Safari on iPhone and go to:
```
http://100.x.x.x:7269
https://100.x.x.x:44376
```

Replace `100.x.x.x` with your Tailscale IP

**Best for**: Web-based access, no SSH needed

### Method 3: SSH Access
Install SSH app on iPhone (Termius, Prompt, or Blink Shell from App Store), then:

```bash
ssh username@100.x.x.x
```

Replace `username` with your Mac username

**Best for**: Command-line access, running commands

## Requirements

- **macOS**: Your development machine
- **Homebrew**: For Tailscale installation
- **iPhone**: iOS device with App Store
- **Internet**: Required for Tailscale authentication

## What Happens

1. **Prerequisites Check**: Verifies Homebrew and network
2. **Installation**: Installs Tailscale via Homebrew (if needed)
3. **Status Check**: Verifies Tailscale is running
4. **SSH Setup**: Enables SSH access through Tailscale
5. **IP Display**: Shows your Tailscale IP address
6. **iPhone Setup**: Provides step-by-step instructions
7. **Connection Info**: Shows how to connect from iPhone

## Security

- **Private Network**: Only your devices can connect
- **End-to-End Encryption**: All traffic encrypted
- **Authentication Required**: Must log in with your account
- **No Public Exposure**: Mac not accessible from internet

## Troubleshooting

**Can't connect from iPhone?**
- Verify both devices use SAME login account
- Check Mac shows "Connected" in Tailscale menu bar
- Check iPhone shows Mac as "Online" in Tailscale app

**SSH connection refused?**
```bash
sudo tailscale up --ssh=on
```

**Tailscale not running?**
```bash
sudo tailscale down && sudo tailscale up
```

## Success Criteria

You're set up when:

- [ ] Tailscale installed on Mac
- [ ] Tailscale shows "Connected" status
- [ ] SSH access enabled
- [ ] You know your Tailscale IP
- [ ] Tailscale installed on iPhone
- [ ] iPhone logged in with same account
- [ ] Can see Mac in iPhone Tailscale app
- [ ] Can connect via remote desktop or SSH

## Quick Commands

```bash
# Check Tailscale status
tailscale status

# Get your Tailscale IP
tailscale ip -4

# Enable SSH access
sudo tailscale up --ssh=on

# Restart Tailscale
sudo tailscale down && sudo tailscale up
```

## Notes

- Works from ANYWHERE with internet (home, office, coffee shops, cellular)
- No port forwarding needed
- No router configuration required
- Free for personal use (up to 3 devices)
- All traffic goes through Tailscale's secure network

## Version

- **Skill Version**: 1.0.0
- **Tailscale**: Latest stable (automatically installed)
- **Platform**: macOS (Darwin) → iPhone (iOS)

---

**Ready to access your dev environment from iPhone!**
