# Clara AI Enhancements — Feature Specs

**Date:** 2026-03-15
**Status:** Approved for planning
**Phase:** 7+ (post-Clara MVP, pre-Phase 8)
**Priority:** High — these differentiate MediTrack from all open-source EHRs

---

## 1. Pre-Charting Automation

### Problem
Doctors spend ~16 min/patient on pre-charting manually. DAX Copilot, Abridge, and Nabla all automate this.

### Solution
Before the patient arrives, Clara pulls recent labs, meds, allergies, last visit notes → generates a draft chart summary.

### Implementation
- **Trigger:** Cron job or event-driven — 30 min before scheduled appointment
- **Data sources:** PatientContextService (already exists) + MedicalRecords.API (last 3 visits) + Lab results (Phase 8a)
- **LLM call:** Summarize into structured pre-chart (chief complaint history, active meds, allergies, recent labs, last visit notes)
- **Output:** Pre-chart stored as `PreChartSummary` entity, linked to appointment
- **Frontend:** "Pre-Chart Ready" badge on appointment card, expandable summary panel

### Effort: Medium (2-3 days)
PatientContextService exists. Main work is the LLM prompt + new entity + UI badge.

---

## 2. After-Visit Summary Generation

### Problem
Patients forget 40-80% of what their doctor tells them. After-visit summaries improve adherence.

### Solution
Clara auto-generates a patient-friendly summary after each session: what was discussed, next steps, medications changed, follow-up date.

### Implementation
- **Trigger:** On session end (`POST /api/sessions/{id}/end`)
- **Input:** Full transcript + suggestions accepted + patient context
- **LLM call:** Generate plain-language summary (no medical jargon, 6th-grade reading level)
- **Output:** `AfterVisitSummary` entity linked to session, printable/emailable
- **Frontend:** Summary tab in SessionSummary page, "Print" and "Email to Patient" buttons

### Effort: Low (1-2 days)
Transcript and suggestions already available at session end.

---

## 3. Composable Task Chaining

### Problem
Clara currently suggests actions but can't execute them. Doctor must manually create orders, prescriptions, referrals.

### Solution
Suggestions become actionable: "Order CBC" → pre-fills lab order draft. "Prescribe Amoxicillin 500mg" → pre-fills prescription.

### Implementation
- **Suggestion types expanded:** `action_lab_order`, `action_prescription`, `action_referral`, `action_follow_up`
- **Action payload:** Structured data attached to suggestion (e.g., `{ drug: "Amoxicillin", dose: "500mg", frequency: "TID", duration: "10 days" }`)
- **Frontend:** "Accept & Create" button on actionable suggestions → opens pre-filled form
- **MCP tools:** `fhir_create` tool execution with doctor confirmation gate
- **Safety:** Never auto-execute. Always show pre-filled form for doctor review + explicit confirm

### Effort: High (1-2 weeks)
Requires structured output from LLM, new suggestion subtypes, pre-fill UI for each action type.

---

## 4. Voice Commands

### Problem
Doctors can't use keyboard/mouse while examining patients. Suki AI's core differentiator is voice-first workflow.

### Solution
"Clara, show me latest labs" / "Clara, order a CBC" / "Clara, schedule follow-up in 2 weeks"

### Implementation
- **`useVoiceCommands` hook already scaffolded** — needs command detection layer
- **Command detection:** Keyword prefix "Clara" triggers command mode (not transcription mode)
- **Command types:** Navigation ("show labs"), Action ("order CBC"), Query ("what medications is patient on?")
- **Deepgram:** Already capturing audio — add intent classification layer (regex for MVP, LLM for complex)
- **Feedback:** Audio chime + visual indicator when command is recognized

### Effort: Medium (3-5 days)
Audio capture exists. Main work is command parsing + action routing.

---

## 5. Automated ICD-10/CPT Coding

### Problem
Manual coding is error-prone and time-consuming. Ambience and DeepScribe built businesses on this.

### Solution
Clara suggests diagnosis codes (ICD-10) and procedure codes (CPT) from the session transcript.

### Implementation
- **Trigger:** On session end, alongside after-visit summary
- **LLM prompt:** "Based on this clinical encounter, suggest ICD-10 diagnosis codes and CPT procedure codes"
- **Code validation:** Cross-reference against seeded ICD-10/CPT database (reject hallucinated codes)
- **Output:** `CodingSuggestion` entity (code, description, confidence, source_text)
- **Frontend:** Coding panel in SessionSummary, accept/reject per code, feeds into billing (Phase 12)

### Effort: Medium (3-5 days)
Requires ICD-10/CPT seed data + LLM prompt + validation layer.

---

## 6. Clinical Decision Support (CDS) Alerts

### Problem
Drug-allergy and drug-drug interactions cause 1.3M+ ER visits annually. Every certified EHR requires CDS.

### Solution
Real-time alerts when prescribing conflicts with allergies, active medications, or clinical guidelines.

### Implementation
- **Depends on:** Phase 9 (Problem List & Allergies)
- **Rules engine:** Simple rule matching first (drug-allergy pairs, drug-drug pairs), LLM-enhanced later
- **Alert types:** `critical` (blocks action), `warning` (requires acknowledge), `info` (dismissible)
- **Frontend:** Red modal for critical (must click "Override with reason"), yellow banner for warning
- **Integration:** Clara can reference CDS rules during suggestions

### Effort: Medium (depends on Phase 9 completion)

---

## Priority Order

| # | Feature | Effort | Depends On | Impact |
|---|---------|--------|------------|--------|
| 1 | After-Visit Summary | Low (1-2 days) | Clara MVP | High — patients love it |
| 2 | Pre-Charting Automation | Medium (2-3 days) | Clara MVP + appointments | High — saves 16 min/patient |
| 3 | Voice Commands | Medium (3-5 days) | Clara MVP | High — hands-free workflow |
| 4 | Automated ICD-10/CPT Coding | Medium (3-5 days) | Clara MVP + seed data | High — billing enabler |
| 5 | Composable Task Chaining | High (1-2 weeks) | Clara MVP + MCP tools | Very High — killer differentiator |
| 6 | CDS Alerts | Medium | Phase 9 | Very High — patient safety |
