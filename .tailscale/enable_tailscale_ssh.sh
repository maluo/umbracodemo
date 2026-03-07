#!/bin/bash

echo "=== Enable Tailscale SSH Access ==="
echo ""
echo "This will enable SSH access through Tailscale."
echo "You'll need to enter your macOS password when prompted."
echo ""

# Check if Tailscale is installed
if ! command -v tailscale &> /dev/null; then
    echo "✗ Tailscale not found. Please install it first:"
    echo "  Run: /tmp/install_tailscale.sh"
    echo "  Or: brew install --cask tailscale"
    exit 1
fi

# Check if Tailscale is running
if ! tailscale status &> /dev/null; then
    echo "✗ Tailscale is not running. Please start Tailscale:"
    echo "  1. Click Tailscale icon in menu bar"
    echo "  2. Click 'Log in...' and complete setup"
    exit 1
fi

echo "✓ Tailscale is running"
echo ""

# Get current Tailscale IP
TS_IP=$(tailscale ip -4)
echo "✓ Your Tailscale IP: $TS_IP"
echo ""

# Enable SSH
echo "Enabling SSH access through Tailscale..."
sudo tailscale up --ssh=on

if [ $? -eq 0 ]; then
    echo ""
    echo "✓ SSH access enabled!"
    echo ""
    echo "Now you can SSH into this Mac from other Tailscale devices:"
    echo "  ssh luoma@$TS_IP"
    echo ""
    echo "From your phone:"
    echo "  1. Install Tailscale app and log in with same account"
    echo "  2. Use SSH app (Termius, Prompt, etc.)"
    echo "  3. Connect: ssh luoma@$TS_IP"
else
    echo ""
    echo "✗ Failed to enable SSH. Please try manually:"
    echo "  sudo tailscale up --ssh=on"
fi
