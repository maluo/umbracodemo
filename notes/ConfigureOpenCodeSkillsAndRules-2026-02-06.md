# Configure OpenCode Skills and Rules

## Task Name
Configure OpenCode skills and rules

## Implementation Plan
1. Review the `.agent` folder structure to understand existing rules and skills
2. Check contents of `.agent/rules` and `.agent/skills` directories
3. Create `opencode.json` configuration file in project root
4. Configure instruction paths to include:
   - `.agent/rules/*` for all rules (including log-after-execution.md)
   - `.agent/skills/*/SKILL.md` for all skill definitions

## Change Log
- Created `opencode.json` in project root
- Added instruction path `.agent/rules/*` to load all rules
- Added instruction path `.agent/skills/*/SKILL.md` to load all skills (OCR and SVG conversion)
- OpenCode will now automatically load custom rules and skills when working in this project

## System Notification After Execution

To create system notifications after task execution, use the following approaches:

### Windows
```bash
powershell -Command "Add-Type -AssemblyName System.Windows.Forms; [System.Windows.Forms.MessageBox]::Show('Task completed successfully!')"
```

### macOS
```bash
osascript -e 'display notification "Task completed successfully!" with title "OpenCode"'
```

### Linux
```bash
notify-send "OpenCode" "Task completed successfully!"
```

### Cross-platform Node.js (if available)
```bash
node -e "const { exec } = require('child_process'); exec('powershell -Command [console]::beep(1000, 500)')"
```

### Integration with Log-After-Execution Rule
The system notification can be integrated into the existing `.agent/rules/log-after-execution.md` rule by adding a notification command after the logging step, ensuring users are immediately informed when tasks complete.

## JavaScript Notification in OpenCode Global Config

To configure JavaScript-based notifications globally across all OpenCode sessions, update the global opencode configuration:

### Global Config Location
- Windows: `C:\Users\<username>\.config\opencode\opencode.json`
- macOS/Linux: `~/.config/opencode/opencode.json`

### Example opencode.json with JavaScript Notification
```json
{
  "$schema": "https://opencode.ai/config.json",
  "instructions": ["AGENTS.md"],
  "postExecutionScript": "notify-completed.js"
}
```

### Notification JavaScript Script Example
Create a `notify-completed.js` file in your config directory:

```javascript
const { exec } = require('child_process');
const os = require('os');

const platform = os.platform();

function sendNotification(title, message) {
  switch (platform) {
    case 'win32':
      exec(`powershell -Command "Add-Type -AssemblyName System.Windows.Forms; [System.Windows.Forms.MessageBox]::Show('${message}', '${title}')"`);
      break;
    case 'darwin':
      exec(`osascript -e 'display notification "${message}" with title "${title}"'`);
      break;
    case 'linux':
      exec(`notify-send "${title}" "${message}"`);
      break;
    default:
      console.log(`[${title}] ${message}`);
  }
}

// Get task info from environment or command line args
const taskName = process.env.OPENCODE_TASK_NAME || 'Task';
const status = process.env.OPENCODE_STATUS || 'completed';

sendNotification('OpenCode', `${taskName} ${status}!`);
```

### Global AGENTS.md Integration
Add notification instructions to `~/.config/opencode/AGENTS.md`:

File: `~/.config/opencode/AGENTS.md`

```
# Global OpenCode Rules

## Post-Execution Notifications
After completing any task, trigger the notification script:

node ~/.config/opencode/notify-completed.js

Set environment variables before execution:
export OPENCODE_TASK_NAME="Task Name"
export OPENCODE_STATUS="completed"
```

### Benefits of Global JavaScript Notification
- Cross-platform compatibility using single script
- Centralized notification configuration
- Easy to customize notification style and content
- Can be enhanced with additional features like sound, icons, or desktop integration