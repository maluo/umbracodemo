# Phone Setup Guide for Tailscale Access

**Setup your phone to access OpenCode remotely**

---

## 📱 Choose Your Phone Type

### iOS (iPhone/iPad)
[Jump to iOS Setup](#ios-setup)

### Android
[Jump to Android Setup](#android-setup)

---

## 🍎 iOS Setup

### Step 1: Install Tailscale App

1. Open the **App Store** on your iPhone/iPad
2. Tap the **Search** tab (bottom right)
3. Search for: **Tailscale**
4. Find the app by **Tailscale Inc.**
5. Tap **Get** or download button
6. Wait for installation to complete
7. Open the app

### Step 2: Log In to Tailscale

1. Open the Tailscale app
2. Tap **Sign in** or **Log in**
3. Choose the **SAME** login method you used on your Mac:
   - **Google** (if you used Google on Mac)
   - **GitHub** (if you used GitHub on Mac)
   - **Microsoft** (if you used Microsoft on Mac)
   - Email (if you used email on Mac)
4. Complete the login in your browser
5. Tap **Done** or return to the app

### Step 3: Verify Connection

1. In the Tailscale app, you should see your Mac listed
2. Look for a green dot or "Online" status
3. Your Mac's name will appear (same as your Mac's hostname)

### Step 4: Install SSH App (Optional)

**Choose one of these apps:**

**Termius** (Recommended)
- Search: "Termius" in App Store
- Install and open
- Easy SSH key management
- Free for basic use

**Prompt**
- Search: "Prompt" in App Store
- Install and open
- Modern interface
- Free tier available

**Blink Shell**
- Search: "Blink Shell" in App Store
- Install and open
- Very user-friendly
- Free for basic use

### Step 5: Test SSH Connection

1. Open your SSH app (Termius/Prompt/Blink)
2. Add a new host/connection
3. Configure:
   - **Host:** Your Tailscale IP (e.g., `100.64.123.45`)
   - **Username:** `luoma`
   - **Port:** `22`
4. Tap **Connect**
5. Accept the SSH key fingerprint (first time only)
6. You should see a terminal connected to your Mac

---

## 🤖 Android Setup

### Step 1: Install Tailscale App

1. Open the **Play Store** on your Android device
2. Tap the **Search** bar (top)
3. Search for: **Tailscale**
4. Find the app by **Tailscale Inc.**
5. Tap **Install**
6. Wait for installation to complete
7. Tap **Open** or launch the app

### Step 2: Log In to Tailscale

1. Open the Tailscale app
2. Tap **Sign in** or **Log in**
3. Choose the **SAME** login method you used on your Mac:
   - **Google** (if you used Google on Mac)
   - **GitHub** (if you used GitHub on Mac)
   - **Microsoft** (if you used Microsoft on Mac)
   - Email (if you used email on Mac)
4. Complete the login in your browser
5. Tap **Done** or return to the app

### Step 3: Verify Connection

1. In the Tailscale app, you should see your Mac listed
2. Look for a green dot or "Online" status
3. Your Mac's name will appear (same as your Mac's hostname)

### Step 4: Install SSH App (Optional)

**Choose one of these apps:**

**Termux** (Recommended)
- Search: "Termux" in Play Store
- Install and open
- Full Linux terminal on Android
- Completely free

**JuiceSSH**
- Search: "JuiceSSH" in Play Store
- Install and open
- Easy interface
- Free for basic use

### Step 5: Test SSH Connection

1. Open your SSH app (Termux/JuiceSSH)

**If using Termux:**
```bash
ssh luoma@100.x.x.x
```
Replace `100.x.x.x` with your Tailscale IP

**If using JuiceSSH:**
1. Tap **Identities** → Create new identity
   - **Nickname:** Mac
   - **Username:** luoma
   - **Password:** (your Mac password)
2. Tap **Connections** → Create new connection
   - **Nickname:** Mac SSH
   - **Type:** SSH
   - **Host:** Your Tailscale IP (e.g., `100.64.123.45`)
   - **Port:** `22`
   - **Identity:** Select the identity you created
3. Tap **Connect**
4. Enter password when prompted
5. You should see a terminal connected to your Mac

---

## 🌐 Access OpenCode via Browser (No SSH Needed!)

If you don't want to use SSH, you can access OpenCode directly in your browser:

1. Make sure OpenCode server is running on your Mac
2. Open Safari (iOS) or Chrome (Android)
3. Go to: `http://100.x.x.x:7269`
   - Replace `100.x.x.x` with your Tailscale IP
4. You should see the OpenCode interface!

---

## 💻 Remote Desktop Access (No SSH or Browser!)

Use your Mac's full desktop from your phone:

1. Open the Tailscale app on your phone
2. Find your Mac in the device list
3. Tap on your Mac device
4. Select **Remote Desktop** or **Share Screen**
5. Your Mac's desktop will appear on your phone
6. Control your Mac just like you were sitting in front of it

---

## 🔑 Getting Your Tailscale IP

On your Mac, run:

```bash
tailscale ip -4
```

Example output: `100.64.123.45`

Write this down - you'll need it on your phone!

---

## ✅ Verification Checklist

After setup, verify each method:

### SSH Access
- [ ] SSH app installed on phone
- [ ] Can connect to `luoma@100.x.x.x`
- [ ] Terminal shows Mac's command prompt

### Browser Access
- [ ] OpenCode server running on Mac
- [ ] Phone browser can access `http://100.x.x.x:7269`
- [ ] OpenCode interface loads and works

### Remote Desktop
- [ ] Mac visible in Tailscale phone app
- [ ] Can open Remote Desktop
- [ ] Can control Mac's desktop

---

## 🚨 Troubleshooting

### Can't see Mac in Tailscale app

**On Mac:**
- Check Tailscale is connected (menu bar icon should be green)
- Try: `tailscale status`

**On Phone:**
- Make sure you're logged into the SAME account as Mac
- Force close and reopen the Tailscale app
- Try logging out and logging back in

### SSH connection fails

**Check on Mac:**
```bash
# Is SSH enabled?
sudo tailscale up --ssh=on

# Check SSH service
sudo systemsetup -getremotelogin
# Should show "Remote Login: On"
```

**Common SSH errors:**
- `Connection refused`: SSH not enabled
- `Permission denied`: Wrong username/password
- `Host key verification failed`: First time - accept the key

### Browser won't load

**Check on Mac:**
```bash
# Is server running?
lsof -i :7269

# Start server
cd Umbraco13
dotnet run
```

**On phone:**
- Make sure you're using the Tailscale IP, not `localhost`
- Try both `http://100.x.x.x:7269` and `https://100.x.x.x:44376`

---

## 📞 Need Help?

**Mac Setup:**
- See `notes/TAILSCALE_SETUP_COMPLETE.md`
- See `.tailscale/QUICKSTART.md`

**Phone Issues:**
- Tailscale Docs: https://tailscale.com/kb/
- Tailscale Support: https://tailscale.com/support

---

## 🎉 You're All Set!

Once you complete phone setup, you can:

✅ Work on OpenCode from anywhere
✅ Access files on your Mac
✅ Run commands from your phone
✅ Use your Mac's desktop remotely
✅ All traffic encrypted and secure

**Total setup time:** 5-10 minutes
**Difficulty:** Easy

---

**Happy coding from anywhere!** 🚀
