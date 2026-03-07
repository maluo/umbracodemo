#!/bin/bash

echo "=== Tailscale Status Check ==="
echo ""

# Check if Tailscale is installed
if ! command -v tailscale &> /dev/null; then
    echo "✗ Tailscale not installed"
    echo "  Install: brew install --cask tailscale"
    exit 1
fi

echo "✓ Tailscale is installed"
echo ""

# Check if Tailscale is running
if ! tailscale status &> /dev/null; then
    echo "✗ Tailscale is not running"
    echo "  Start it from the menu bar icon"
    exit 1
fi

echo "✓ Tailscale is running"
echo ""

# Get Tailscale IP
TS_IP=$(tailscale ip -4)
echo "Tailscale IPv4: $TS_IP"
echo ""

# Get status details
echo "Connected Devices:"
tailscale status --json | jq -r '.Peer[] | select(.Online == true) | "\(.HostName) - \(.TailscaleIPs[0] // "N/A")"' 2>/dev/null || tailscale status | grep -v "^\s*#"
echo ""

# Check SSH status
echo "SSH Status:"
tailscale status --json | jq -r '.Self.Caps' 2>/dev/null | grep -q "tailscale-ssh" && echo "  ✓ SSH enabled" || echo "  ✗ SSH not enabled"
echo ""

# Check if server is running
echo "OpenCode/Umbraco Server:"
if lsof -ti:7269 > /dev/null 2>&1; then
    echo "  ✓ HTTP server running on port 7269"
    echo "  Access via Tailscale: http://$TS_IP:7269"
else
    echo "  ✗ HTTP server not running"
fi

if lsof -ti:44376 > /dev/null 2>&1; then
    echo "  ✓ HTTPS server running on port 44376"
    echo "  Access via Tailscale: https://$TS_IP:44376"
else
    echo "  ✗ HTTPS server not running"
fi
