# MediTrack UI/UX Improvement Specs

**Date:** 2026-03-15
**Status:** Approved for planning
**Scope:** Cross-cutting UX improvements based on industry analysis

---

## Critical UX Improvements

### 1. Reduce Clicks Per Workflow

**Problem:** Epic's #1 complaint: 4,000+ clicks per ER shift. 85% of users rank workflow efficiency as top EHR satisfaction driver.

**Target:** 3 clicks or fewer for any common action.

**Audit needed:**
| Workflow | Current Clicks | Target | Fix |
|----------|---------------|--------|-----|
| Start Clara session from dashboard | ? | 2 | One-click "Start Session" on patient card |
| Create appointment | ? | 3 | Quick-add from calendar slot click |
| Find patient record | ? | 2 | CommandPalette search → direct to patient |
| View lab results | ? | 3 | Inline expand on patient timeline |

**Action:** Audit top 10 workflows, measure clicks, add shortcuts.

---

### 2. Role-Based Adaptive Dashboard

**Problem:** Everyone sees the same dashboard. Industry trend is personalized by role.

**Implementation:**
| Role | Primary Widgets | Secondary Widgets |
|------|----------------|-------------------|
| Doctor | Today's schedule, pending actions, Clara sessions, patient alerts | Recent records, analytics |
| Nurse | Vitals entry queue, medication admin, triage alerts | Today's schedule, patient list |
| Admin | Revenue metrics, scheduling gaps, compliance scores | User management, audit logs |
| Patient | Upcoming appointments, lab results, messages | Medications, visit summaries |

**Approach:** `DashboardLayout` component reads user role → renders role-specific widget grid. Widgets are the same components, just different arrangement/visibility.

**Effort:** Medium (3-5 days)

---

### 3. Patient Timeline View

**Problem:** Tabbed medical records require clicking between tabs to build a complete picture. Clinicians want one scrollable view.

**Implementation:**
- Replace tabs with vertical chronological timeline
- Each entry: date, type icon, title, provider, expandable detail
- Filter bar: type checkboxes, date range picker, provider dropdown
- Click to expand inline (no page navigation)
- Reference: Medplum's patient timeline, Epic's Chart Review

**Components:**
- `PatientTimeline` — main container
- `TimelineEntry` — polymorphic card (encounter, lab, medication, vital, imaging)
- `TimelineFilter` — filter bar
- `TimelineDetail` — inline expanded detail

**Effort:** Medium-High (1 week)

---

### 4. Clara Live Session UX

**Problem:** Compare against DAX Copilot and Abridge — missing confidence indicators, one-tap accept, dismiss feedback.

**Improvements:**
- **Confidence bars:** Visual progress bar on each suggestion (green/yellow/red based on score)
- **One-tap accept:** Tap suggestion → auto-added to session note draft with visual confirmation
- **Dismiss with reason:** Swipe left to dismiss, optional "why" dropdown (irrelevant, incorrect, already addressed)
- **Session timer:** Small countdown showing next batch trigger (e.g., "Clara thinking in 12s")
- **Floating Clara button:** Minimize to corner FAB, expand to full panel on tap

**Effort:** Medium (3-5 days)

---

### 5. Command Palette Enhancement

**Problem:** CommandPalette exists but limited to navigation. Power users want natural language.

**Improvements:**
- Natural language: "Show John Smith's last labs" → navigates to patient labs
- Quick actions: "New appointment for today 3pm" → opens pre-filled form
- Clara integration: "Start Clara session with patient 12345" → navigates to session start
- Recent items: Show last 5 accessed patients/records
- Keyboard-first: vim-style shortcuts (j/k navigation, Enter to select)

**Effort:** Medium (3-5 days)

---

### 6. Notification Center

**Current state:** Sonner toasts exist but no persistent notification center.

**Implementation:**
- **Bell icon** in header with unread count badge
- **Notification panel** (dropdown or slide-over):
  - Critical (red): drug interactions, abnormal labs, overdue results → sticky, requires action
  - Warning (yellow): appointment reminders, pending approvals → dismissible
  - Info (blue): Clara suggestions accepted, system updates → auto-dismiss after 7 days
- **Per-patient audit:** "Show all notifications for this patient"
- **Persistence:** Store in DB, mark read/unread, filterable

**Effort:** Medium (3-5 days)

---

### 7. Dark Mode

**Problem:** Clinicians work 12+ hour shifts, often in dimly lit rooms. Dark mode reduces eye strain.

**Implementation:**
- Tailwind `dark:` variants on all components
- Toggle in header (sun/moon icon)
- Persist preference in localStorage + user profile
- Respect OS preference (`prefers-color-scheme: dark`)
- Audit all hardcoded colors → convert to CSS variables / Tailwind tokens

**Effort:** Medium (2-3 days) — Tailwind makes this straightforward

---

### 8. Compact View Toggle

**Problem:** Clinicians prefer dense layouts — more information per screen. Current spacing is generous.

**Implementation:**
- Toggle button: "Comfortable" / "Compact" in settings or header
- Compact: reduced padding/margins, smaller text, tighter table rows
- CSS class `.compact` on body → Tailwind `group-[.compact]:` variants
- Tables: row height 32px (compact) vs 48px (comfortable)
- Cards: padding 12px (compact) vs 24px (comfortable)

**Effort:** Low-Medium (2-3 days)

---

### 9. Loading & Empty States Audit

**Problem:** Unknown current state. Best practice: skeleton screens (not spinners), helpful empty states with CTAs.

**Implementation:**
- **Skeleton screens:** Replace all spinners with content-shaped skeleton animations
- **Empty states:** Every list/table has a friendly empty state with icon + message + CTA
  - "No appointments today — [Create one]"
  - "No lab results yet — [Order labs]"
  - "No Clara sessions — [Start your first session]"
- **Error states:** Inline retry button, not modal alerts

**Effort:** Low (1-2 days per area, ongoing)

---

### 10. Mobile Bottom Navigation

**Problem:** 50%+ of patient portal access is mobile. Sidebar doesn't work well on mobile.

**Implementation:**
- Detect mobile viewport → replace sidebar with bottom tab bar
- 4-5 tabs max: Home, Appointments, Records, Clara, More
- Touch targets: 48px minimum
- Swipe gestures: swipe between tabs
- Already have responsive layout — extend with `md:hidden` / `md:block`

**Effort:** Medium (3-5 days)

---

## Design System Gaps

| Gap | Current | Target | Effort |
|-----|---------|--------|--------|
| Triage colors | Ad-hoc status colors | Standardized 5-level triage palette across ALL views | Low |
| Keyboard shortcuts | CommandPalette only | Full keyboard nav, vim-style, shortcut cheatsheet modal | Medium |
| Loading states | Spinners | Skeleton screens everywhere | Low-Medium |
| Empty states | Missing or generic | Contextual CTAs per feature | Low |
| Error recovery | Unknown | Inline retry + error boundary fallbacks | Low-Medium |
| Accessibility | Partial | WCAG 2.1 AA audit, aria labels, focus management | Medium |

---

## Priority Order

| # | Improvement | Effort | Impact | Quick Win? |
|---|------------|--------|--------|------------|
| 1 | Dark Mode | 2-3 days | High (clinician comfort) | Yes |
| 2 | Clara Live Session UX | 3-5 days | High (core product) | Yes |
| 3 | Loading & Empty States | 1-2 days each | Medium (polish) | Yes |
| 4 | Role-Based Dashboard | 3-5 days | High (personalization) | No |
| 5 | Patient Timeline | 1 week | Very High (clinical workflow) | No |
| 6 | Notification Center | 3-5 days | High (patient safety) | No |
| 7 | Command Palette Enhancement | 3-5 days | Medium (power users) | No |
| 8 | Compact View Toggle | 2-3 days | Medium (clinician preference) | Yes |
| 9 | Mobile Bottom Navigation | 3-5 days | High (patient portal) | No |
| 10 | Reduce Clicks Audit | Ongoing | Very High (efficiency) | No |
