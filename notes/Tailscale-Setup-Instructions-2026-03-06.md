# Tailscale Configuration - Next Steps

## ✅ What I've Done

1. Created feature branch: `feat/tailscale-opencv-access`
2. Created comprehensive documentation with step-by-step instructions
3. Created quick reference guide for 5-minute setup
4. Pushed all documentation to GitHub

## 📋 Your Manual Steps Required

Tailscale installation requires admin privileges, so you'll need to complete these steps manually:

### Step 1: Install Tailscale (5 minutes)
Open Terminal and run:
```bash
brew install --cask tailscale
```
Enter your password when prompted.

### Step 2: Create Account (2 minutes)
1. Click Tailscale icon in menu bar (top right)
2. Click "Log in..."
3. Choose Google/GitHub/Microsoft (use Google for easiest setup)
4. Wait until it shows "Connected" (green)

### Step 3: Install on Phone (2 minutes)
- **iPhone:** Search "Tailscale" in App Store → Install → Open → Log in
- **Android:** Search "Tailscale" in Play Store → Install → Open → Log in

### Step 4: Enable SSH Access (1 minute)
In Terminal:
```bash
sudo tailscale up --ssh=on
```

### Step 5: Get Your Tailscale IP
```bash
tailscale ip -4
```
Note this IP (e.g., 100.x.x.x)

### Step 6: Test from Phone
Open a terminal/SSH app on your phone:
```bash
ssh luoma@YOUR_TAILSCALE_IP
```
Replace `YOUR_TAILSCALE_IP` with the IP from step 5.

### Step 7: Access OpenCode
Once connected via SSH, run:
```bash
cd /Users/luoma/Downloads/backup\ Nov\ 22\ 2025/PVE/Umbraco/umbracodemo/Umbraco13
dotnet run
```
Then open `http://YOUR_TAILSCALE_IP:7269` in your phone's browser.

## 📚 Documentation Files

Both files are in the `notes/` folder and pushed to GitHub:

1. **Full Guide:** `Tailscale-Configuration-for-OpenCode-Remote-Access-2026-03-06.md`
   - Detailed instructions (349 lines)
   - Multiple access methods
   - Troubleshooting section
   - Security best practices

2. **Quick Reference:** `Tailscale-Quick-Reference-2026-03-06.md`
   - 5-minute quick start
   - Essential commands
   - Key information

## 🔗 GitHub Repository

View the documentation online:
https://github.com/maluo/umbracodemo/tree/feat/tailscale-opencv-access/notes

## ⚡ Alternative: Remote Desktop (No SSH needed)

If you prefer not to use SSH:

1. Install Tailscale on both Mac and phone (steps 1-3)
2. On phone, open Tailscale app
3. Tap your Mac device name
4. Select "Remote Desktop" or "Share Screen"
5. Use your Mac's desktop from your phone

## 🎯 What This Gives You

After setup, you'll be able to:

✅ Access your OpenCode environment from anywhere
✅ SSH into your Mac from your phone
✅ Run code and commands from your phone
✅ View the OpenCode web interface on your phone
✅ Access files and resources on your Mac remotely
✅ All traffic encrypted and secure

## 📱 Phone Apps Needed

**iOS:**
- Tailscale (required)
- Prompt, Termius, or Blink Shell (for SSH)

**Android:**
- Tailscale (required)
- Termux or JuiceSSH (for SSH)

## 🚨 Common Issues & Solutions

**Issue:** Can't connect from phone
- Solution: Check Tailscale app on Mac shows "Connected" and phone shows Mac as "Online"

**Issue:** SSH connection refused
- Solution: Run `sudo tailscale up --ssh=on` again on Mac

**Issue:** Can't access OpenCode in browser
- Solution: Ensure `dotnet run` is running on Mac and use Tailscale IP, not localhost

## 📞 Need Help?

Check the full documentation for detailed troubleshooting:
- `notes/Tailscale-Configuration-for-OpenCode-Remote-Access-2026-03-06.md`

Or visit:
- Tailscale Docs: https://tailscale.com/kb/
- Tailscale Support: https://tailscale.com/support

## ✨ After Setup

Once everything is working, you can:

1. Work on your code from anywhere
2. Test your OpenCode environment remotely
3. Access your development server from coffee shops, travel, etc.
4. No need for port forwarding or router configuration

---

**Ready to start?** Follow the 7 steps above and you'll be done in 10-15 minutes!

**Questions?** The full documentation has everything you need.
