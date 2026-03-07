#!/bin/bash

echo "=== Tailscale Installation Script ==="
echo ""
echo "This script will install Tailscale on macOS."
echo "You'll need to enter your macOS password when prompted."
echo ""

# Check if running with appropriate permissions
if [ "$EUID" -eq 0 ]; then 
    echo "Please don't run this script as root. Run it as your normal user."
    exit 1
fi

# Check if Homebrew is installed
if ! command -v brew &> /dev/null; then
    echo "Homebrew not found. Please install Homebrew first:"
    echo "  /bin/bash -c \"$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)\""
    exit 1
fi

echo "✓ Homebrew found"
echo ""

# Install Tailscale
echo "Installing Tailscale via Homebrew..."
brew install --cask tailscale

if [ $? -eq 0 ]; then
    echo ""
    echo "✓ Tailscale installed successfully!"
    echo ""
    echo "Next steps:"
    echo "1. Click the Tailscale icon in your menu bar (top right)"
    echo "2. Click 'Log in...' to create/connect your account"
    echo "3. Wait for it to show 'Connected'"
    echo ""
    echo "Then run: sudo tailscale up --ssh=on"
else
    echo ""
    echo "✗ Installation failed. Please try manual installation:"
    echo "  Visit https://tailscale.com/download/macos"
fi
