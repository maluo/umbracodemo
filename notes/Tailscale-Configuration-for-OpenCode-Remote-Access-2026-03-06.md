# Tailscale Configuration for OpenCode Remote Access
**Date:** 2026-03-06
**Branch:** feat/tailscale-opencv-access

## Overview
This guide configures Tailscale VPN to enable remote access to your OpenCode environment from your phone.

## Prerequisites
- macOS (your development machine)
- iOS or Android phone
- Administrator privileges on macOS

## Installation Steps

### Step 1: Install Tailscale on macOS

**Option A: Using Homebrew (Recommended)**
```bash
brew install --cask tailscale
```
*Note: You'll need to enter your macOS password when prompted.*

**Option B: Manual Download**
1. Visit https://tailscale.com/download/macos
2. Download the Tailscale macOS package
3. Double-click the downloaded `.pkg` file
4. Follow the installation prompts

**Option C: From App Store**
1. Open App Store
2. Search for "Tailscale"
3. Click "Get" to install

### Step 2: Set Up Tailscale Account

1. After installation, Tailscale will appear in your menu bar (top right of screen)
2. Click the Tailscale icon
3. Select "Log in..."
4. Choose an authentication method:
   - **Google** (recommended for personal use)
   - **GitHub**
   - **Microsoft**
   - **Email magic link**
5. Complete the login process in your browser
6. After successful login, Tailscale will start connecting

### Step 3: Configure Tailscale on macOS

1. Click the Tailscale menu bar icon
2. Verify it shows "Connected" with a green indicator
3. Click on your device name to see your Tailscale IP address
4. Note your Tailscale IP (e.g., `100.x.x.x`)

### Step 4: Install Tailscale on Your Phone

**For iOS (iPhone/iPad):**
1. Open App Store
2. Search for "Tailscale"
3. Tap "Get" to download and install
4. Open the Tailscale app
5. Tap "Log in"
6. Use the same authentication method as your Mac
7. After login, you'll see your phone in the device list

**For Android:**
1. Open Google Play Store
2. Search for "Tailscale"
3. Tap "Install"
4. Open the Tailscale app
5. Tap "Sign in"
6. Use the same authentication method as your Mac
7. After login, you'll see your phone in the device list

### Step 5: Configure Access to OpenCode Environment

Your OpenCode environment runs on localhost ports:
- HTTP: `localhost:7269`
- HTTPS: `localhost:44376`

#### Option A: Access via Tailscale SSH (Recommended)

**On your Mac:**

1. Enable SSH access through Tailscale:
   ```bash
   # Generate SSH keys if you don't have them
   ssh-keygen -t ed25519

   # Enable tailscale SSH (this will prompt for your Mac password)
   sudo tailscale up --ssh=on
   ```

2. Find your Mac's Tailscale hostname:
   ```bash
   tailscale ip -4
   # Note the IP address, e.g., 100.x.x.x
   ```

**On your phone:**

**For iOS:**
1. Install "Tailscale SSH" app from App Store OR use the built-in terminal
2. Connect to your Mac:
   ```bash
   ssh username@100.x.x.x
   ```
   Replace:
   - `username` with your macOS username
   - `100.x.x.x` with your Mac's Tailscale IP

**For Android:**
1. Install Termux or similar terminal app
2. Connect to your Mac:
   ```bash
   ssh username@100.x.x.x
   ```

#### Option B: Port Forwarding (Alternative)

**On your Mac:**

1. Start your OpenCode/Umbraco server:
   ```bash
   cd /Users/luoma/Downloads/backup\ Nov\ 22\ 2025/PVE/Umbraco/umbracodemo/Umbraco13
   dotnet run
   ```

2. In a separate terminal, create a Tailscale funnel (requires Tailscale Plus):
   ```bash
   # Make localhost accessible via Tailscale
   tailscale funnel 7269
   # Or for HTTPS
   tailscale funnel 44376
   ```

**Note:** Tailscale Funnel is a paid feature. If you don't have Tailscale Plus, use Option A (SSH).

#### Option C: Use Tailscale Remote Access (Free)

Tailscale Remote Access allows you to access your Mac remotely:

1. On your Mac, click the Tailscale menu bar icon
2. Go to Preferences > General
3. Enable "Allow remote access to this Mac"
4. On your phone, open Tailscale app
5. Tap on your Mac device name
6. Select "Remote Desktop" or "Share Screen"
7. This will open Screen Sharing on your phone

### Step 6: Test Remote Access

**From your phone:**

1. Open Tailscale app
2. You should see your Mac listed as "Online"
3. Tap on your Mac device
4. Try one of these access methods:
   - SSH: `ssh username@100.x.x.x`
   - Remote Desktop/Screen Sharing
   - Port forwarding (if configured)

**Verify OpenCode access:**

Once connected to your Mac via SSH or Remote Desktop:
```bash
# From your phone's SSH connection to Mac
curl http://localhost:7269
# OR
curl https://localhost:44376
```

### Step 7: Accessing OpenCode Specifically

If you're using SSH access:

1. SSH into your Mac from your phone
2. Navigate to your project:
   ```bash
   cd /Users/luoma/Downloads/backup\ Nov\ 22\ 2025/PVE/Umbraco/umbracodemo
   ```
3. Start the development server if not running:
   ```bash
   cd Umbraco13
   dotnet run
   ```
4. Access the web interface:
   - Use your Mac's Tailscale IP: `http://100.x.x.x:7269`
   - Or use SSH port forwarding from your phone

**SSH Port Forwarding (Advanced):**

On your phone, when connecting:
```bash
# Forward local port 8080 to remote localhost:7269
ssh -L 8080:localhost:7269 username@100.x.x.x
```

Then on your phone's browser, open:
- `http://localhost:8080`

## Configuration Files

### Tailscale ACL Configuration (Optional)

For advanced control, you can configure ACLs at https://login.tailscale.com/admin/acls

Example ACL to allow your phone to access your Mac:
```json
{
  "acls": [
    {
      "action": "accept",
      "src": ["*"],
      "dst": ["100.x.x.x:*"]
    }
  ],
  "tagOwners": {
    "tag:dev": ["user:your@email.com"]
  }
}
```

Replace `your@email.com` with your actual Tailscale account email.

## Troubleshooting

### Tailscale won't connect

1. Check your Mac's firewall:
   ```bash
   sudo /usr/libexec/ApplicationFirewall/socketfilterfw --getglobalstate
   ```

2. Ensure Tailscale is allowed through firewall:
   - System Settings > Network > Firewall
   - Allow incoming connections for Tailscale

### Cannot access from phone

1. Verify both devices are on the same Tailscale network:
   - On Mac: Tailscale app should show "Connected"
   - On Phone: Tailscale app should show your Mac as "Online"

2. Check device list on phone:
   - Open Tailscale app
   - Your Mac should be visible and green (Online)

### SSH connection refused

1. Ensure SSH is enabled on Mac:
   ```bash
   sudo systemsetup -setremotelogin on
   ```

2. Verify Tailscale SSH is enabled:
   ```bash
   tailscale status --json | grep ssh
   ```

### Port not accessible

1. Check if your application is running:
   ```bash
   lsof -i :7269
   lsof -i :44376
   ```

2. Ensure no firewall rules block these ports

## Security Best Practices

1. **Enable 2FA** on your Tailscale account at https://login.tailscale.com/admin/settings
2. **Use strong SSH keys** - don't use password authentication
3. **Restrict ACL access** - only allow specific devices to connect
4. **Monitor connected devices** regularly at https://login.tailscale.com/admin/machines
5. **Rotate access tokens** if you suspect a compromise

## Useful Commands

```bash
# Check Tailscale status
tailscale status

# Check your Tailscale IP
tailscale ip -4

# Enable SSH access
sudo tailscale up --ssh=on

# Disable SSH access
sudo tailscale up --ssh=off

# Restart Tailscale daemon
sudo tailscale down && sudo tailscale up

# View connected machines
tailscale status --json | jq '.Peer[] | .HostName, .TailscaleIPs[0]'

# Check Tailscale logs
log stream --predicate 'process == "tailscale"' --level debug
```

## Quick Reference

| Task | Command/Action |
|------|---------------|
| Connect to Tailscale | Click menu bar icon > Log in |
| Get Tailscale IP | `tailscale ip -4` |
| SSH from phone | `ssh username@100.x.x.x` |
| List connected devices | `tailscale status` |
| Enable SSH | `sudo tailscale up --ssh=on` |
| Remote Desktop | Tailscale app > Tap device > Remote Desktop |

## Device Information

- **Mac Hostname:** (Run `hostname` to find out)
- **Mac Username:** `luoma`
- **Tailscale IP:** (Run `tailscale ip -4` to find out)
- **OpenCode Port:** 7269 (HTTP), 44376 (HTTPS)
- **Project Path:** `/Users/luoma/Downloads/backup Nov 22 2025/PVE/Umbraco/umbracodemo`

## Next Steps

1. Complete the manual installation steps above
2. Test SSH connection from your phone to Mac
3. Start your OpenCode/Umbraco server
4. Access via port forwarding or remote desktop
5. Enjoy remote access from anywhere!

## Notes

- Tailscale creates a private network that works from anywhere with internet
- All traffic is encrypted end-to-end
- No port forwarding or router configuration needed
- Works across different network types (home, office, cellular, public WiFi)
- Free for personal use (up to 3 devices on basic plan)

## Support

- Tailscale Documentation: https://tailscale.com/kb/
- Tailscale Support: https://tailscale.com/support
- OpenCode Documentation: Check your project's README.md

---

**Configuration completed:** Manual installation required
**Branch:** feat/tailscale-opencv-access
**Status:** Documentation created - follow steps above to complete setup
