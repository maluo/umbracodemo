# Task: Add Block Content Rendering to HomePage.cshtml

**Date:** 2026-07-12

## Implementation Plan

1. **Investigate the HomePage Doc Type**: Checked `uSync/v9/ContentTypes/homepage.config` — the `blockContent` property uses `Umbraco.TinyMCE` (a Rich Text Editor), **not** a Block List or Block Grid.
2. **Check the generated model**: `HomePage.generated.cs` does not expose a typed `BlockContent` property, so it must be fetched dynamically via `Model.Value<IHtmlEncodedString>("blockContent")`.
3. **Render in the view**: Replace the `<!-- Finish render block content here -->` placeholder in `HomePage.cshtml` with the appropriate Razor code.

## Change Log

### [HomePage.cshtml](file:///Users/luoma/Downloads/backup%20Nov%2022%202025/PVE/Umbraco/umbracodemo/Umbraco13/Views/HomePage.cshtml)

Replaced the placeholder HTML comment with Razor code that:
- Fetches the `blockContent` value typed as `IHtmlEncodedString` (preserves HTML without double-encoding)
- Null-checks the value before rendering
- Wraps output in a `<section id="block-content" class="page-section">` + container div

```diff
-<!-- Finish render block content here -->
+@* Block Content from Home Page Doc Type *@
+@{
+    var blockContent = Model.Value<IHtmlEncodedString>("blockContent");
+}
+@if (blockContent != null && !string.IsNullOrWhiteSpace(blockContent.ToString()))
+{
+    <section id="block-content" class="page-section">
+        <div class="container">
+            @blockContent
+        </div>
+    </section>
+}
```
