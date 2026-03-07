# Tailscale iPhone Quick Start
**Access your development environment from iPhone in 3 minutes**

---

## 🚀 Quick Setup (3 commands)

### Step 1: Enable Tailscale on Your Mac

```bash
sudo tailscale up --ssh=on
```

### Step 2: Get Your Tailscale IP

```bash
tailscale ip -4
```

**Write down your IP:** `_______________`
Example: `100.84.108.68`

### Step 3: Install Tailscale on Your iPhone

1. Open **App Store** on your iPhone
2. Search for **Tailscale**
3. Tap **Get** to install
4. Open the app
5. Tap **Sign in** or **Log in**
6. Use the **SAME** account as your Mac (Google recommended)

---

## 🌐 Connect from Your iPhone

### Method 1: Remote Desktop (EASIEST)

No extra apps needed!

1. Open **Tailscale** app on your iPhone
2. Find and tap on your **Mac** in the device list
3. Tap **Remote Desktop** or **Share Screen**
4. Your Mac's desktop appears on your iPhone

**That's it!** You can now use your Mac from your iPhone.

---

### Method 2: Direct Browser Access

Open **Safari** on your iPhone and go to:

```
http://100.x.x.x:7269
```

Replace `100.x.x.x` with your Tailscale IP

**Works for:** Any web-based access to your development environment

---

### Method 3: SSH Access (Optional)

If you want command-line access:

1. Install an SSH app from App Store:
   - **Termius** (recommended)
   - **Prompt**
   - **Blink Shell**

2. Open the SSH app and add a new connection:
   - **Host:** Your Tailscale IP (e.g., `100.84.108.68`)
   - **Username:** Your Mac username (e.g., `luoma`)
   - **Port:** `22`

3. Tap **Connect**

**First time only:** You'll be asked to verify the SSH fingerprint. Tap **Continue**.

---

## ✅ How to Know It's Working

You'll know Tailscale is working when:

- [ ] Tailscale icon in Mac menu bar shows **Connected** (green)
- [ ] iPhone Tailscale app shows your Mac as **Online** (green dot)
- [ ] You can connect via Remote Desktop OR SSH

---

## 🚨 Troubleshooting

### Can't see your Mac on iPhone?

**On Mac:**
- Check Tailscale is connected (menu bar icon should be green)
- Try: `tailscale status`

**On iPhone:**
- Make sure you're logged into the **SAME** account as your Mac
- Force close and reopen Tailscale app
- Try logging out and logging back in

### SSH connection refused?

```bash
# Re-enable SSH on Mac
sudo tailscale up --ssh=on
```

### Browser won't load?

- Make sure you're using your **Tailscale IP**, not `localhost`
- Check your development server is running on Mac
- Try the HTTP URL: `http://100.x.x.x:7269`

---

## 📱 What You Get

After setup, you can:

✅ **Access your Mac from anywhere** with internet
✅ **Use remote desktop** on your iPhone
✅ **SSH into your Mac** for command-line access
✅ **Open browser apps** on your Mac from iPhone
✅ **All traffic is encrypted** and secure
✅ **Works on any network** (WiFi, cellular, public hotspots)
✅ **No configuration needed** - no port forwarding, no router setup

---

## 🔑 Security

- **Private Network**: Only your devices can connect
- **End-to-End Encryption**: All traffic through Tailscale is encrypted
- **Authentication Required**: Must log in with your account
- **No Public Exposure**: Your Mac is not accessible from the internet
- **Safe**: Uses industry-standard VPN technology

---

## 📚 Need More Help?

**Tailscale Documentation:** https://tailscale.com/kb/
**Tailscale Support:** https://tailscale.com/support

**Skill Documentation:** `.claude/skills/tailscale_remote_access/SKILL.md`

---

## 🎉 You're Done!

**Total setup time: 3-5 minutes**

Just 3 steps:
1. Enable Tailscale on Mac: `sudo tailscale up --ssh=on`
2. Get your IP: `tailscale ip -4`
3. Install Tailscale on iPhone and log in

Then connect from iPhone via Remote Desktop, browser, or SSH!

---

**Happy remote development!** 🚀
