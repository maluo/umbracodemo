# Tailscale Setup Checklist

## ✅ Configuration Completed by AI

All setup materials, documentation, and automation scripts have been created:

- [x] Feature branch created: `feat/tailscale-opencv-access`
- [x] Comprehensive documentation (3 guides, 528 lines total)
- [x] Helper scripts (6 scripts for automation)
- [x] SSH configuration examples
- [x] All files committed and pushed to GitHub

## 📋 Your Manual Steps Required

Follow these steps in order. Each should take 1-5 minutes.

### Phase 1: Mac Setup (5 minutes)

- [ ] **1. Install Tailscale**
  ```bash
  chmod +x .tailscale/install_tailscale.sh
  sudo .tailscale/install_tailscale.sh
  ```
  *Time: 2 minutes*

- [ ] **2. Login to Tailscale**
  1. Click Tailscale icon in menu bar (top right)
  2. Click "Log in..."
  3. Choose Google/GitHub/Microsoft
  4. Wait for "Connected" (green indicator)
  *Time: 1 minute*

- [ ] **3. Enable SSH Access**
  ```bash
  chmod +x .tailscale/enable_tailscale_ssh.sh
  sudo .tailscale/enable_tailscale_ssh.sh
  ```
  *Time: 1 minute*

- [ ] **4. Note Your Tailscale IP**
  ```bash
  tailscale ip -4
  ```
  Write down this IP: ___________
  *Time: 10 seconds*

### Phase 2: Phone Setup (2 minutes)

- [ ] **5. Install Tailscale App**
  - **iOS:** App Store → Search "Tailscale" → Install
  - **Android:** Play Store → Search "Tailscale" → Install
  *Time: 1 minute*

- [ ] **6. Login on Phone**
  1. Open Tailscale app
  2. Tap "Sign in" / "Log in"
  3. Use SAME account as Mac
  4. Wait for "Online" status
  *Time: 1 minute*

- [ ] **7. Install SSH App** (if using SSH method)
  - **iOS:** Termius, Prompt, or Blink Shell
  - **Android:** Termux or JuiceSSH
  *Time: 1 minute*

### Phase 3: Test Connection (3 minutes)

- [ ] **8. Check Connection Status**
  ```bash
  ./tailscale/tailscale_status.sh
  ```
  Verify both Mac and phone show "Online"
  *Time: 10 seconds*

- [ ] **9. Test SSH from Phone**
  ```bash
  ssh luoma@100.x.x.x
  ```
  Replace `100.x.x.x` with your Tailscale IP
  *Time: 1 minute*

- [ ] **10. Start OpenCode Server**
  ```bash
  chmod +x .tailscale/start_opencode_server.sh
  .tailscale/start_opencode_server.sh
  ```
  *Time: 1 minute*

- [ ] **11. Test OpenCode Access**
  - On phone browser: `http://100.x.x.x:7269`
  - Verify you can see OpenCode interface
  *Time: 1 minute*

## ✨ Success Criteria

You'll know it's working when:

- ✅ Tailscale shows "Connected" on Mac
- ✅ Tailscale app shows Mac as "Online" on phone
- ✅ You can SSH from phone to Mac
- ✅ OpenCode loads in phone browser
- ✅ You can access from anywhere with internet

## 🚨 Troubleshooting

**If Tailscale won't connect:**
- Check firewall: System Settings > Network > Firewall
- Try restarting: `sudo tailscale down && sudo tailscale up`

**If SSH fails:**
- Re-enable: `sudo tailscale up --ssh=on`
- Check SSH service: `sudo systemsetup -getremotelogin`

**If OpenCode won't load:**
- Check if running: `lsof -i :7269`
- Start it: `.tailscale/start_opencode_server.sh`

## 📊 Progress Tracking

| Phase | Tasks | Completed | Time |
|-------|--------|-----------|-------|
| Mac Setup | 4 | 0/4 | 5 min |
| Phone Setup | 3 | 0/3 | 2 min |
| Testing | 4 | 0/4 | 3 min |
| **Total** | **11** | **0/11** | **10 min** |

## 📚 Documentation Files

Reference these files for help:

- `TAILSCALE_SETUP_COMPLETE.md` - Complete setup guide
- `notes/Tailscale-Quick-Reference-2026-03-06.md` - Quick start
- `notes/Tailscale-Configuration-for-OpenCode-Remote-Access-2026-03-06.md` - Full details
- `.tailscale/README.md` - Script usage guide

## 🔗 Online Access

All files available at:
https://github.com/maluo/umbracodemo/tree/feat/tailscale-opencv-access

## 🎯 After Setup

Once complete, you can:

- Access OpenCode from anywhere
- Work on code from your phone
- SSH into Mac remotely
- Use remote desktop
- All traffic encrypted

---

**Total time to complete: 10-15 minutes**
**Difficulty: Easy** (copy/paste commands, follow UI prompts)
