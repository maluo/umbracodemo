#!/bin/bash

echo "=== Start OpenCode/Umbraco Server ==="
echo ""

PROJECT_DIR="/Users/luoma/Downloads/backup Nov 22 2025/PVE/Umbraco/umbracodemo/Umbraco13"

# Check if project directory exists
if [ ! -d "$PROJECT_DIR" ]; then
    echo "✗ Project directory not found: $PROJECT_DIR"
    exit 1
fi

# Check if already running
if lsof -ti:7269 > /dev/null 2>&1 || lsof -ti:44376 > /dev/null 2>&1; then
    echo "⚠ Server appears to be running on ports 7269 or 44376"
    echo "  Check: lsof -i :7269"
    echo ""
    read -p "Kill existing server and restart? (y/N) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        lsof -ti:7269 | xargs kill -9 2>/dev/null
        lsof -ti:44376 | xargs kill -9 2>/dev/null
        echo "✓ Existing server killed"
    else
        echo "Exiting."
        exit 0
    fi
fi

# Get Tailscale IP
if command -v tailscale &> /dev/null && tailscale status &> /dev/null; then
    TS_IP=$(tailscale ip -4)
    echo "✓ Tailscale IP: $TS_IP"
    echo ""
    echo "After starting, access from Tailscale network:"
    echo "  HTTP:  http://$TS_IP:7269"
    echo "  HTTPS: https://$TS_IP:44376"
    echo ""
fi

# Start the server
echo "Starting OpenCode/Umbraco server..."
echo ""
cd "$PROJECT_DIR" && dotnet run
