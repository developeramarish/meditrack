# MediTrack Competitive Analysis & Improvement Plan

**Date:** 2026-03-15
**Status:** Approved for planning
**Scope:** Feature gaps, UI/UX improvements, competitive positioning

---

## Competitive Positioning

| Area | MediTrack | Industry Leaders | Gap Level |
|------|-----------|-----------------|-----------|
| Architecture | Microservices + event-driven + DDD | Same (Epic, Canvas, Medplum) | None |
| AI Scribe | Clara (ambient + batch suggestions) | DAX Copilot, Abridge, Suki, Canvas Hyperscribe | Feature gaps |
| Frontend | React 19 + Tailwind + shadcn/ui | Canvas (React), Medplum (React 19) | Stack is modern, UX gaps |
| FHIR | Internal MCP tools, not yet external API | Medplum (FHIR-native), Epic (required by law) | Major — Phase 14 |
| Patient Portal | Not built | MyChart (Epic), athenahealth, Jane App | Major — Phase 10 |
| Billing | Not built | Kareo, AdvancedMD, athenahealth | Major — Phase 12 |
| Mobile | Responsive web only | DrChrono (iPad-native), Jane App | Moderate |
| Open Source | Yes (educational) | OpenEMR, OpenMRS, Medplum, Canvas Hyperscribe | Aligned |

## Unique Differentiators (Keep & Amplify)

1. **MCP-native AI** — No other open-source EHR has this. LLM-agnostic via MCP is genuinely novel
2. **Open-source AI copilot** — Only Canvas Hyperscribe competes here
3. **Full DDD for medical records** — Proper domain modeling most open-source EHRs skip
4. **Event-driven microservices** — OpenEMR is monolithic PHP, OpenMRS is monolithic Java
5. **Aspire orchestration** — No other open-source EHR uses .NET Aspire

---

## Top 10 Features to Add (Prioritized by Impact)

### 1. Pre-Charting Automation (Clara)
- Before patient arrives, Clara pulls recent labs, meds, allergies, last visit notes → draft chart summary
- DAX Copilot, Abridge, Nabla all do this. #1 feature that reduces "pajama time"
- **Effort:** Medium — PatientContextService already exists, add pre-visit trigger

### 2. Composable Task Chaining (Clara)
- Clara doesn't just suggest — it *acts*. "Order CBC" → auto-creates lab order draft
- Canvas Hyperscribe's killer differentiator. Current Clara is read-only suggestions
- **Effort:** High — requires MCP tool execution pipeline + confirmation UI

### 3. After-Visit Summary Generation
- Clara auto-generates patient-friendly summary: what was discussed, next steps, meds changed, follow-up
- Microsoft DAX Copilot's most-loved feature. Fewer phone callbacks
- **Effort:** Low — LLM call on session end, new summary template

### 4. Patient Portal MVP
- Read-only portal: appointments, lab results, medications, visit summaries, secure messaging
- 50%+ of healthcare interactions start on mobile portals. Table stakes
- **Effort:** High — already in roadmap (Phase 10)

### 5. Clinical Decision Support (CDS) Alerts
- Real-time: drug-drug interactions, allergy warnings, abnormal vitals, overdue screenings
- Every certified EHR requires this. Clara's suggestion engine can power it
- **Effort:** Medium — rules engine + alert UI component

### 6. Automated ICD-10/CPT Coding
- Clara suggests diagnosis codes (ICD-10) and procedure codes (CPT) from session transcript
- Ambience and DeepScribe built entire businesses on this. Reduces coding errors by 40%
- **Effort:** Medium — LLM prompt engineering + code lookup database

### 7. Voice Commands for Clara
- "Clara, show me latest labs" / "Clara, order a CBC" / "Clara, schedule follow-up in 2 weeks"
- Suki AI's core differentiator. `useVoiceCommands` hook already scaffolded
- **Effort:** Medium — Deepgram already captures audio, add command detection layer

### 8. Problem List & Allergy Management
- SNOMED CT-coded problem list, allergy recording, drug-allergy interaction checking
- Core clinical safety feature. Already in roadmap (Phase 9)
- **Effort:** Medium

### 9. Lab Orders & Results
- Create lab orders, track status, view results with abnormal value highlighting
- Second most-used EHR feature after charting. Already in roadmap (Phase 8a)
- **Effort:** Medium-High

### 10. Clinician Burnout Dashboard
- Track documentation time per patient, after-hours charting, Clara acceptance rate, appointment density
- Novel differentiator. Alert when utilization exceeds 85%. Target: 70-85%
- **Effort:** Low — analytics data already exists, new dashboard widgets

---

## UI/UX Improvements

### Critical UX Gaps

#### 1. Reduce Clicks Per Workflow
- Epic's #1 complaint: 4,000+ clicks per ER shift
- Target: **3 clicks or fewer** for any common action
- Audit: start Clara session, create appointment, find patient record

#### 2. Role-Based Adaptive Dashboard
| Role | Dashboard Focus |
|------|----------------|
| Doctor | Today's schedule, pending actions, Clara sessions, patient alerts |
| Nurse | Vitals entry queue, medication administration, triage alerts |
| Admin | Revenue metrics, scheduling gaps, compliance scores |
| Patient | Upcoming appointments, lab results, messages, medications |

#### 3. Patient Timeline View
- Replace tabbed medical records with unified longitudinal timeline
- Vertical chronological: encounters, labs, meds, vitals, imaging
- Filter by type, date range, provider — click to expand inline
- Reference: Medplum's approach

#### 4. Mobile-First Patient Experience
- Touch-friendly tap targets (48px minimum)
- Swipe gestures, bottom navigation bar on mobile
- Offline-capable appointment viewing

#### 5. Clara Live Session UX Enhancements
- Confidence indicators: visual display per suggestion
- One-tap accept: tap suggestion → auto-added to note draft
- Dismiss with reason: swipe + optional "why" (trains model)
- Session timer: visible batch trigger countdown

#### 6. Command Palette Enhancement
- Natural language: "Show John Smith's last labs"
- Quick actions: "New appointment for today 3pm"
- Clara integration: "Start Clara session with patient 12345"

#### 7. Notification Center
- Critical alerts (drug interactions, abnormal labs) → red badge, sticky
- Informational (reminders, Clara suggestions) → dismissible
- Audit trail per patient

#### 8. Dark Mode
- Clinicians work long shifts in dimly lit rooms
- Tailwind `dark:` variants make this straightforward

### Design System Gaps

| Pattern | Current | Best Practice | Action |
|---------|---------|---------------|--------|
| Urgency colors | Status colors exist | Triage-level colors consistently across ALL views | Standardize triage palette |
| Data density | Standard spacing | Clinicians prefer dense layouts | Add "compact view" toggle |
| Keyboard shortcuts | CommandPalette | Full keyboard navigation | Add vim-style shortcuts |
| Loading states | Unknown | Skeleton screens, not spinners | Audit all loading states |
| Empty states | Unknown | Helpful with CTAs | Audit all empty states |
| Error recovery | Unknown | Inline errors with retry | Standardize error handling |

---

## Reference Projects

| Project | What to Learn | Priority |
|---------|--------------|----------|
| Canvas Hyperscribe | Composable task chaining, open-source AI copilot | High |
| Medplum | FHIR-native React components, patient timeline, MCP server | High |
| OpenEMR | Billing workflows, e-prescribing, ONC certification | Medium |
| Abridge | Note quality scoring, guideline alignment, pre-charting UX | Medium |
| Jane App | Beautiful intake forms, allied health scheduling UX | Low (UX inspiration) |

---

## Industry Context (2025-2026)

### AI Clinical Assistant Market
- "Big 4" ambient AI: Abridge (#1 KLAS 2025), Ambience (200+ specialties), Nabla (Epic-focused), Suki (voice-first)
- Epic launching native "Art for Clinicians" scribe → expected to consolidate market
- Burnout decreased 51.9% → 38.8% after 30 days with ambient AI
- 93% of doctors report giving patients "full attention" with ambient AI

### Healthcare UX Trends
- Ambient-first documentation (AI writes, clinician reviews)
- Pre-charting automation (AI prepares chart before patient arrives)
- Composable task chaining (outputs trigger next workflow steps)
- Mobile-first patient portals (50%+ access is mobile)
- WCAG accessibility mandatory
- FHIR R4 is global standard (78% of countries have regulations)

### Architecture Trends
- Microservices + event-driven is standard for healthcare SaaS
- Benchmark: 94,000 clinical events/second with 76ms latency
- DDD for service boundaries (align with business capabilities)
- Stream governance for schema validation + auditability
