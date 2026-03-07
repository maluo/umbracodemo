#!/bin/bash

# Tailscale Complete Setup Wizard
# This script guides you through the entire Tailscale setup process

set -e

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}════════════════════════════════════════════════════════════════${NC}"
echo -e "${BLUE}       TAILSCALE SETUP WIZARD FOR OPENCODE${NC}"
echo -e "${BLUE}════════════════════════════════════════════════════════════════${NC}"
echo ""

# Phase 1: Check Prerequisites
echo -e "${YELLOW}Phase 1: Checking Prerequisites...${NC}"
echo ""

if ! command -v brew &> /dev/null; then
    echo -e "${RED}✗ Homebrew not found. Please install it first:${NC}"
    echo "  /bin/bash -c \"\$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)\""
    exit 1
fi
echo -e "${GREEN}✓ Homebrew found${NC}"

if command -v tailscale &> /dev/null; then
    echo -e "${GREEN}✓ Tailscale already installed${NC}"
    TAILSCALE_INSTALLED=true
else
    echo -e "${YELLOW}⚠ Tailscale not installed${NC}"
    TAILSCALE_INSTALLED=false
fi

echo ""

# Phase 2: Install Tailscale
if [ "$TAILSCALE_INSTALLED" = false ]; then
    echo -e "${YELLOW}Phase 2: Installing Tailscale...${NC}"
    echo ""

    echo "Installing Tailscale via Homebrew..."
    brew install --cask tailscale

    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ Tailscale installed successfully!${NC}"
    else
        echo -e "${RED}✗ Installation failed. Please try manually:${NC}"
        echo "  Visit: https://tailscale.com/download/macos"
        exit 1
    fi
    echo ""
fi

# Phase 3: Check Tailscale Status
echo -e "${YELLOW}Phase 3: Checking Tailscale Status...${NC}"
echo ""

if tailscale status &> /dev/null; then
    echo -e "${GREEN}✓ Tailscale is running${NC}"
    TS_IP=$(tailscale ip -4)
    echo -e "${GREEN}✓ Tailscale IP: $TS_IP${NC}"
    echo ""
else
    echo -e "${YELLOW}⚠ Tailscale is not running${NC}"
    echo ""
    echo "Please start Tailscale:"
    echo "  1. Click Tailscale icon in menu bar (top right)"
    echo "  2. Click 'Log in...'"
    echo "  3. Choose Google/GitHub/Microsoft"
    echo "  4. Wait for 'Connected' status (green)"
    echo ""
    echo "After logging in, run this script again."
    exit 0
fi

# Phase 4: Enable SSH
echo -e "${YELLOW}Phase 4: Enabling SSH Access...${NC}"
echo ""

echo "Enabling SSH access through Tailscale..."
sudo tailscale up --ssh=on

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ SSH access enabled!${NC}"
else
    echo -e "${RED}✗ Failed to enable SSH. Please try manually:${NC}"
    echo "  sudo tailscale up --ssh=on"
    exit 1
fi
echo ""

# Phase 5: Show Connection Info
echo -e "${YELLOW}Phase 5: Connection Information${NC}"
echo ""

TS_IP=$(tailscale ip -4)
TS_STATUS=$(tailscale status --json | jq -r '.BackendState // "unknown" 2>/dev/null || echo "Running"')

echo -e "${GREEN}✓ Tailscale IP: $TS_IP${NC}"
echo -e "${GREEN}✓ Status: $TS_STATUS${NC}"
echo ""

# Phase 6: Phone Setup Instructions
echo -e "${YELLOW}Phase 6: Phone Setup Instructions${NC}"
echo ""
echo "To access your OpenCode from your phone, follow these steps:"
echo ""
echo "1. Install Tailscale app on your phone:"
echo "   - iOS: App Store → Search 'Tailscale' → Install"
echo "   - Android: Play Store → Search 'Tailscale' → Install"
echo ""
echo "2. Login to Tailscale app:"
echo "   - Open Tailscale app"
echo "   - Tap 'Sign in' / 'Log in'"
echo "   - Use the SAME account as your Mac"
echo ""
echo "3. Test connection from phone:"
echo "   - Install SSH app (Termius/Prompt/Termux/JuiceSSH)"
echo "   - Connect: ssh luoma@$TS_IP"
echo ""

# Phase 7: Access Methods
echo -e "${YELLOW}Phase 7: Access Methods${NC}"
echo ""

echo "You can access OpenCode from your phone in several ways:"
echo ""
echo "${GREEN}Method 1: SSH Access${NC}"
echo "  ssh luoma@$TS_IP"
echo "  Use: SSH app on phone"
echo ""

echo "${GREEN}Method 2: Direct Browser Access${NC}"
echo "  http://$TS_IP:7269"
echo "  https://$TS_IP:44376"
echo "  Use: Phone browser"
echo ""

echo "${GREEN}Method 3: SSH Port Forwarding${NC}"
echo "  ssh -L 8080:localhost:7269 luoma@$TS_IP"
echo "  Then: http://localhost:8080"
echo "  Use: SSH app + browser"
echo ""

echo "${GREEN}Method 4: Remote Desktop${NC}"
echo "  Open Tailscale app → Tap Mac → Remote Desktop"
echo "  Use: Tailscale app only"
echo ""

# Phase 8: Start Server Option
echo -e "${YELLOW}Phase 8: Start OpenCode Server?${NC}"
echo ""
read -p "Do you want to start the OpenCode server now? (y/N) " -n 1 -r
echo ""

if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo "Starting OpenCode server..."
    echo ""

    PROJECT_DIR="/Users/luoma/Downloads/backup Nov 22 2025/PVE/Umbraco/umbracodemo/Umbraco13"

    if [ ! -d "$PROJECT_DIR" ]; then
        echo -e "${RED}✗ Project directory not found${NC}"
        exit 1
    fi

    cd "$PROJECT_DIR"
    echo -e "${GREEN}✓ Starting server...${NC}"
    echo -e "${GREEN}✓ Access at: http://$TS_IP:7269${NC}"
    echo ""
    echo "Press Ctrl+C to stop the server"
    echo ""

    dotnet run
fi

# Summary
echo ""
echo -e "${BLUE}════════════════════════════════════════════════════════════════${NC}"
echo -e "${GREEN}✅ SETUP COMPLETE!${NC}"
echo -e "${BLUE}════════════════════════════════════════════════════════════════${NC}"
echo ""
echo "Quick Reference:"
echo "  Tailscale IP: $TS_IP"
echo "  SSH: ssh luoma@$TS_IP"
echo "  Browser: http://$TS_IP:7269"
echo ""
echo "For detailed guides, see:"
echo "  - notes/TAILSCALE_SETUP_COMPLETE.md"
echo "  - notes/SETUP_CHECKLIST.md"
echo ""
