---
name: tailscale_setup
description: Automates complete Tailscale configuration for OpenCode remote access. Installs Tailscale, enables SSH access, guides through phone setup, and tests remote connections.
---

# Tailscale Setup Skill

Automates the complete setup of Tailscale VPN for accessing OpenCode environment remotely from your phone.

## Capabilities

- **Automated Installation**: Installs Tailscale via Homebrew on macOS
- **SSH Configuration**: Enables SSH access through Tailscale with validation
- **Status Monitoring**: Checks Tailscale connection, IP, and server status
- **Phone Setup Guidance**: Provides detailed instructions for iOS and Android setup
- **Server Management**: Starts OpenCode/Umbraco server with port checking
- **Connection Testing**: Verifies remote access from phone to OpenCode

## Requirements

- **macOS**: Running on macOS (Darwin)
- **Homebrew**: Must have Homebrew installed (`which brew`)
- **OpenCode Project**: Project must exist at expected path
- **Internet Connection**: Required for Tailscale authentication

## Usage

Invoke with the skill name:

```
/skill tailscale_setup
```

The skill will guide you through:

1. Prerequisites check
2. Tailscale installation (if needed)
3. Account login guidance
4. SSH access enablement
5. Phone setup instructions
6. Server startup
7. Connection verification

## What the Skill Does

### Phase 1: Prerequisites Check
- Verifies Homebrew is installed
- Checks if Tailscale is already installed
- Validates OpenCode project path exists
- Checks internet connectivity

### Phase 2: Tailscale Installation
- Installs Tailscale via `brew install --cask tailscale`
- Verifies successful installation
- Adds Tailscale to macOS menu bar

### Phase 3: Account Setup
- Guides user through Tailscale login via menu bar
- Supports Google, GitHub, Microsoft, and email authentication
- Waits for "Connected" status

### Phase 4: SSH Configuration
- Enables SSH access with `tailscale up --ssh=on`
- Validates SSH is running
- Retrieves and displays Tailscale IP address
- Shows connection details for remote access

### Phase 5: Phone Setup Guidance
- Provides step-by-step instructions for iOS (iPhone/iPad)
- Provides step-by-step instructions for Android
- Lists recommended SSH apps for each platform
- Shows all access methods (SSH, Browser, Remote Desktop)

### Phase 6: Server Management
- Checks if OpenCode server is already running
- Offers to start server on ports 7269 (HTTP) and 44376 (HTTPS)
- Displays access URLs via Tailscale IP
- Provides commands for manual server management

### Phase 7: Connection Testing
- Displays test commands for each access method
- Shows troubleshooting steps
- Provides verification checklist

## Access Methods

After setup, you can access OpenCode via:

### Method 1: SSH Access
```bash
ssh luoma@100.x.x.x
```
**Best for**: Command-line access, file management, running commands

### Method 2: Direct Browser Access
```
http://100.x.x.x:7269
https://100.x.x.x:44376
```
**Best for**: Web interface, direct access without SSH

### Method 3: SSH Port Forwarding
```bash
ssh -L 8080:localhost:7269 luoma@100.x.x.x
# Then access: http://localhost:8080
```
**Best for**: Browser access through SSH tunnel

### Method 4: Remote Desktop
**Action**: Tailscale app → Tap Mac → Remote Desktop
**Best for**: Full desktop control, GUI access

## Project Paths

Default paths used by the skill:

- **OpenCode Project**: `/Users/luoma/Downloads/backup Nov 22 2025/PVE/Umbraco/umbracodemo`
- **Umbraco Directory**: `Umbraco13/`
- **Tailscale Scripts**: `.tailscale/`
- **Documentation**: `notes/`

## Phone App Recommendations

### iOS (iPhone/iPad)
- **Termius**: Recommended SSH app with key management
- **Prompt**: Modern SSH interface with free tier
- **Blink Shell**: User-friendly SSH client

### Android
- **Termux**: Full Linux terminal, completely free
- **JuiceSSH**: Easy interface, free basic use

## Helper Scripts

The skill uses these helper scripts from `.tailscale/` directory:

1. **setup_wizard.sh**: Comprehensive guided setup wizard
2. **install_tailscale.sh**: Automated Tailscale installation
3. **enable_tailscale_ssh.sh**: SSH access enablement
4. **start_opencode_server.sh**: OpenCode server startup
5. **tailscale_status.sh**: Status monitoring

## Troubleshooting

### Common Issues

**Issue**: Tailscale won't connect
**Solution**:
```bash
sudo tailscale down && sudo tailscale up
```

**Issue**: SSH connection refused
**Solution**:
```bash
sudo tailscale up --ssh=on
```

**Issue**: Can't see Mac on phone
**Solution**:
- Ensure both devices use SAME login account
- Check both show "Connected"/"Online"
- Refresh Tailscale app on phone

**Issue**: OpenCode won't load in browser
**Solution**:
```bash
# Check if running
lsof -i :7269

# Start server
cd Umbraco13
dotnet run
```

## Security Features

- **Private Network**: Isolated VPN accessible only to authenticated devices
- **End-to-End Encryption**: All traffic encrypted
- **Authentication Required**: Google/GitHub/Microsoft required
- **SSH Key Support**: Strong key authentication (password auth discouraged)
- **Device Isolation**: Each device requires separate authentication
- **ACL Configuration**: Advanced access control available

## Success Criteria

Setup is complete when:

- [ ] Tailscale installed on Mac
- [ ] Tailscale shows "Connected" status
- [ ] SSH access enabled (`tailscale status` shows SSH capability)
- [ ] Tailscale IP address known (`tailscale ip -4`)
- [ ] Tailscale app installed on phone
- [ ] Phone shows Mac as "Online"
- [ ] Can SSH from phone to Mac (or remote desktop works)
- [ ] OpenCode server running
- [ ] Can access OpenCode from phone browser

## Related Resources

- **Tailscale Documentation**: https://tailscale.com/kb/
- **Tailscale Support**: https://tailscale.com/support
- **Project Setup Guides**: See `notes/TAILSCALE_SETUP_COMPLETE.md`
- **Quick Start**: See `.tailscale/QUICKSTART.md`
- **Phone Setup Guide**: See `.tailscale/PHONE_SETUP_GUIDE.md`

## Advanced Configuration

For advanced users, the skill supports:

- **ACL Configuration**: Create custom access control lists at https://login.tailscale.com/admin/acls
- **Multiple Devices**: Add more phones, tablets, laptops to network
- **Custom DNS**: Configure custom DNS settings
- **Subnet Routers**: Route specific subnets through Tailscale
- **Tailscale Funnel**: Expose local services publicly (requires Tailscale Plus)

## Version Information

- **Skill Version**: 1.0.0
- **Created**: 2026-03-06
- **Tailscale Version**: Latest stable (automatically installed)
- **Platform**: macOS (Darwin)

## Notes

- This skill is designed for personal use (free tier up to 3 devices)
- Tailscale works across all network types: home, office, cellular, public WiFi
- No port forwarding or router configuration required
- All connections go through Tailscale's encrypted network
- Skill requires sudo privileges for installation and SSH configuration

---

**Ready to set up Tailscale for OpenCode remote access!**
