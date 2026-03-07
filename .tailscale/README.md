# Tailscale Helper Scripts

This directory contains helper scripts for setting up and using Tailscale with your OpenCode environment.

## Installation

Run the installation script:

```bash
chmod +x .tailscale/install_tailscale.sh
sudo .tailscale/install_tailscale.sh
```

Or install manually:

```bash
brew install --cask tailscale
```

## Quick Setup (3 steps)

### 1. Install and Login

```bash
# Install
brew install --cask tailscale

# Login via menu bar (click Tailscale icon > Log in...)
```

### 2. Enable SSH Access

```bash
chmod +x .tailscale/enable_tailscale_ssh.sh
sudo .tailscale/enable_tailscale_ssh.sh
```

### 3. Start OpenCode Server

```bash
chmod +x .tailscale/start_opencode_server.sh
.tailscale/start_opencode_server.sh
```

## Scripts

### install_tailscale.sh
Installs Tailscale via Homebrew on macOS.

**Usage:**
```bash
./.tailscale/install_tailscale.sh
```

### enable_tailscale_ssh.sh
Enables SSH access through Tailscale.

**Usage:**
```bash
sudo .tailscale/enable_tailscale_ssh.sh
```

This will:
- Check if Tailscale is installed and running
- Enable SSH access
- Display your Tailscale IP
- Show connection instructions

### start_opencode_server.sh
Starts the OpenCode/Umbraco server.

**Usage:**
```bash
./.tailscale/start_opencode_server.sh
```

This will:
- Check if server is already running
- Display Tailscale IP (if available)
- Start the dotnet server
- Show access URLs

### tailscale_status.sh
Shows current Tailscale and server status.

**Usage:**
```bash
./.tailscale/tailscale_status.sh
```

This displays:
- Tailscale installation status
- Tailscale IP address
- Connected devices
- SSH status
- Server port status (7269/44376)

## SSH Configuration

See `ssh_config_example` for sample SSH configuration.

To use it:

1. Copy to your SSH config:
```bash
cat .tailscale/ssh_config_example >> ~/.ssh/config
```

2. Edit the IP address:
```bash
vim ~/.ssh/config
# Change 100.x.x.x to your actual Tailscale IP
```

3. Connect easily:
```bash
ssh opencode-mac
```

## Port Forwarding

To access OpenCode from your phone browser:

```bash
# From your phone's SSH connection or local terminal
ssh -L 8080:localhost:7269 luoma@100.x.x.x
```

Then open `http://localhost:8080` in your browser.

## Accessing from Phone

### Via SSH

1. Install Tailscale app on your phone
2. Log in with same account as Mac
3. Install an SSH app:
   - iOS: Termius, Prompt, Blink Shell
   - Android: Termux, JuiceSSH
4. Connect:
   ```bash
   ssh luoma@100.x.x.x
   ```
   Replace `100.x.x.x` with your Tailscale IP

### Via Remote Desktop

1. Open Tailscale app on phone
2. Tap your Mac device
3. Select "Remote Desktop" or "Share Screen"

## Common Commands

```bash
# Check Tailscale status
tailscale status

# Get Tailscale IP
tailscale ip -4

# Restart Tailscale
sudo tailscale down && sudo tailscale up

# List connected devices
tailscale status --peers
```

## Troubleshooting

### Tailscale not connected

```bash
# Check status
./.tailscale/tailscale_status.sh

# Restart Tailscale
sudo tailscale down && sudo tailscale up
```

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
.tailscale/
├── README.md                  # This file
├── install_tailscale.sh       # Installation script
├── enable_tailscale_ssh.sh    # Enable SSH script
├── start_opencode_server.sh    # Start server script
├── tailscale_status.sh         # Status check script
└── ssh_config_example         # SSH config example
```

## Documentation

For complete documentation, see:
- `notes/Tailscale-Configuration-for-OpenCode-Remote-Access-2026-03-06.md`
- `notes/Tailscale-Quick-Reference-2026-03-06.md`
- `notes/Tailscale-Setup-Instructions-2026-03-06.md`

## Support

- Tailscale Docs: https://tailscale.com/kb/
- Tailscale Support: https://tailscale.com/support
