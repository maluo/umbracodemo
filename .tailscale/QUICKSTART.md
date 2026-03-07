# Tailscale Quick Start - 5 Minute Setup

**Read this to get started in 5 minutes!**

---

## 🚀 Run the Setup Wizard (Recommended)

The easiest way to set up everything:

```bash
chmod +x .tailscale/setup_wizard.sh
./.tailscale/setup_wizard.sh
```

This wizard will:
- ✓ Install Tailscale if needed
- ✓ Check your Tailscale status
- ✓ Enable SSH access
- ✓ Show your connection information
- ✓ Guide you through phone setup
- ✓ Offer to start OpenCode server

---

## 📋 Manual Quick Start

If you prefer manual setup, follow these 5 steps:

### Step 1: Install Tailscale (2 minutes)

```bash
# Option A: Use the installer script
chmod +x .tailscale/install_tailscale.sh
sudo .tailscale/install_tailscale.sh

# Option B: Direct Homebrew
brew install --cask tailscale
```

**What happens:**
- Downloads and installs Tailscale for macOS
- Adds Tailscale icon to menu bar
- No restart required

### Step 2: Login to Tailscale (1 minute)

**Manual actions:**
1. Click the Tailscale icon in your menu bar (top right of screen)
2. Click "Log in..."
3. Choose a login method:
   - **Google** (recommended)
   - GitHub
   - Microsoft
   - Email magic link
4. Complete login in your browser
5. Wait for "Connected" status (green indicator)

**What happens:**
- Creates your private Tailscale network
- Registers your Mac on the network
- Gets your Tailscale IP address

### Step 3: Enable SSH Access (1 minute)

```bash
# Make script executable
chmod +x .tailscale/enable_tailscale_ssh.sh

# Enable SSH
sudo .tailscale/enable_tailscale_ssh.sh
```

**What happens:**
- Enables SSH connections through Tailscale
- Validates SSH is working
- Shows your Tailscale IP

### Step 4: Get Your Tailscale IP (10 seconds)

```bash
tailscale ip -4
```

**Write down this IP address:** `_______________`

**Example:** `100.64.123.45`

### Step 5: Install on Phone (2 minutes)

**For iOS (iPhone/iPad):**
1. Open App Store
2. Search: "Tailscale"
3. Tap "Get" to install
4. Open the app
5. Tap "Sign in" or "Log in"
6. Use the **SAME** account as your Mac

**For Android:**
1. Open Play Store
2. Search: "Tailscale"
3. Tap "Install"
4. Open the app
5. Tap "Sign in"
6. Use the **SAME** account as your Mac

**What happens:**
- Your phone joins the same private network
- Both devices can see each other
- No configuration needed

---

## ✅ You're Ready!

Now you can access your OpenCode from anywhere:

### Method 1: SSH from Phone (Command Line)

**iOS:** Install Termius, Prompt, or Blink Shell
**Android:** Install Termux or JuiceSSH

**Connect:**
```bash
ssh luoma@100.x.x.x
```
Replace `100.x.x.x` with your Tailscale IP

### Method 2: Direct Browser Access

Open your phone browser and go to:
```
http://100.x.x.x:7269
https://100.x.x.x:44376
```
Replace with your Tailscale IP

### Method 3: Remote Desktop

1. Open Tailscale app on phone
2. Tap on your Mac device
3. Select "Remote Desktop" or "Share Screen"
4. Use your Mac from your phone

---

## 🔧 Start OpenCode Server

To access OpenCode, start the server:

```bash
# Option A: Use the server script
chmod +x .tailscale/start_opencode_server.sh
./.tailscale/start_opencode_server.sh

# Option B: Manual start
cd Umbraco13
dotnet run
```

Then access via: `http://100.x.x.x:7269`

---

## 📊 Check Status

```bash
# See all status information
chmod +x .tailscale/tailscale_status.sh
./.tailscale/tailscale_status.sh
```

Shows:
- ✓ Tailscale installation status
- ✓ Tailscale IP address
- ✓ Connected devices
- ✓ SSH status
- ✓ Server port status

---

## 🚨 Troubleshooting

**Tailscale won't connect:**
```bash
sudo tailscale down && sudo tailscale up
```
Check firewall: System Settings > Network > Firewall

**SSH connection refused:**
```bash
sudo tailscale up --ssh=on
```

**Can't see Mac in phone Tailscale app:**
- Make sure you're logged into the SAME account on both devices
- Check both show "Connected" / "Online"
- Try refreshing the Tailscale app

**OpenCode not loading:**
```bash
# Check if server is running
lsof -i :7269

# Start server
cd Umbraco13
dotnet run
```

---

## 📚 Full Documentation

For complete details, see:
- `notes/TAILSCALE_SETUP_COMPLETE.md` - Full guide
- `notes/SETUP_CHECKLIST.md` - 11-step checklist
- `notes/Tailscale-Configuration-for-OpenCode-Remote-Access-2026-03-06.md` - Technical details

---

## 🎉 You're Done!

After following these 5 steps:

✅ Access OpenCode from anywhere
✅ SSH into your Mac from phone
✅ Use remote desktop from phone
✅ All traffic encrypted
✅ Works on any network (WiFi, cellular, public hotspots)
✅ No port forwarding needed
✅ Free for personal use

---

**Total time: 5-10 minutes**
**Difficulty: Easy** (just copy/paste commands and follow prompts)

**Ready to start?** Run the setup wizard!

```bash
chmod +x .tailscale/setup_wizard.sh
./.tailscale/setup_wizard.sh
```
