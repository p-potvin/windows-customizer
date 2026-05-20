## 2026-05-05 - VaultCoreBadge Accessibility and UX Improvements
**Learning:** Adding a native HTML title attribute and cursor: help is a simple, lightweight way to add tooltip-like hints without needing heavy components. And role="status" + aria-label ensures screen readers get the context they need for important status badges.
**Action:** Use this simple pattern for status badges instead of heavier tooltip wrappers when only simple text explanation is needed.
