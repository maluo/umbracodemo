---
name: email_writer
description: Writes professional emails for various purposes (follow-ups, requests, apologies, thank you, etc.) without applying task logging or git operations afterward.
---

# Email Writer Skill

This skill specializes in crafting well-structured, professional emails for various business and personal contexts.

## Usage

When the user requests an email to be written:

1. **Gather Requirements**: Ask for (or infer) the following:
   - **Purpose**: What is the email's objective? (follow-up, request, apology, thank you, introduction, etc.)
   - **Recipient**: Who is receiving it? (colleague, client, manager, recruiter, etc.)
   - **Tone**: Formal, semi-formal, casual?
   - **Key Information**: Specific details to include
   - **Desired Outcome**: What action should the recipient take?

2. **Draft the Email**: Create a well-structured email with:
   - Clear, concise subject line
   - Professional greeting
   - Purpose statement (why you're writing)
   - Main body (key information)
   - Call to action (what's needed)
   - Professional closing
   - Appropriate signature

3. **Present to User**: Show the draft and offer to make adjustments

## Important Constraints

- **DO NOT** apply task_logger after writing an email
- **DO NOT** create git commits or PRs for email drafts
- Emails are typically created as drafts for user review, not permanent workspace changes
- Save emails only if user explicitly requests it

## Email Templates

### Follow-Up Email
```
Subject: Following up on [topic]

Hi [Name],

I wanted to follow up on [previous conversation/topic]. Have you had a chance to [review/consider]?

[Optional: Brief context or value add]

Please let me know if you need any additional information from my end.

Best regards,
[Your name]
```

### Request Email
```
Subject: Request for [what you need]

Dear [Name],

I hope this email finds you well. I am writing to request [specific request].

[Context/Reason for request]

[Optional: Timeline or urgency]

Thank you for your consideration. I appreciate your help with this.

Best regards,
[Your name]
```

### Thank You Email
```
Subject: Thank you for [what they did]

Hi [Name],

I wanted to express my sincere appreciation for [specific thing they helped with].

[Optional: Specific impact or why it mattered]

It was incredibly helpful and I'm grateful for your support.

Best,
[Your name]
```

### Apology Email
```
Subject: Apology regarding [situation]

Dear [Name],

I am writing to sincerely apologize for [mistake/issue].

[Acknowledge responsibility if applicable]

[How you'll fix it/prevent recurrence]

I understand this may have caused [impact], and I am committed to making it right.

Sincerely,
[Your name]
```

### Introduction Email
```
Subject: Introduction - [Your name] / [Their name]

Hi [Name],

I hope you're doing well. I wanted to introduce myself - I'm [your role] at [company].

[Reason for reaching out]

[Idea/proposal/value proposition]

I would welcome the opportunity to [discuss further/meet/call].

Best regards,
[Your name]
```

## Best Practices

- **Keep it concise**: Respect the recipient's time
- **Clear subject line**: Make the purpose immediately obvious
- **One main topic**: Avoid multiple unrelated topics
- **Call to action**: Be clear about what you want
- **Proofread**: Check for clarity, tone, and errors
- **Appropriate formality**: Match the context and relationship
