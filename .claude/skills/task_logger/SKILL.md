---
name: task_logger
description: Logs task list, implementation details, and change logs to a file with the task name and today's date in the workspace root.
---

# Task Logger Skill

This skill allows the agent to create a consolidated log of a completed task.

## Usage

When a task is completed, or when requested by the user, follow these steps:

1.  **Gather Information**: Collect the following details from the task's lifecycle:
    -   **Task Name**: The high-level name of the task.
    -   **Today's Date**: The current date in format YYYY-MM-DD.
    -   **Task Checklist**: The list of tasks from `task.md`.
    -   **Implementation Details**: Key technical changes from `implementation_plan.md` or code edits.
    -   **Change Log**: A summary of what was accomplished (from `walkthrough.md`).
2.  **Format the Log**: Create a Markdown file with the content.
3.  **Naming Convention**: The log file should be named `[Task_Name]_[YYYY-MM-DD].md`.
4.  **Placement**: Save the file to the root directory of the current workspace.

## Template

```markdown
# [Task Name] - [YYYY-MM-DD]

## Task Checklist
[Compiled list of tasks with [x] status]

## Implementation Details
[Technical summary of changes]

## Change Log
[Description of what was modified/added/removed]
```
