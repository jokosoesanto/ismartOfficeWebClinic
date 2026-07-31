# Implementation Roadmap

This roadmap organizes the Prioritized Backlog into logical Execution Sprints designed to bring the Web Prototype to 100% Feature and Workflow Parity with the Desktop application by the November 30, 2026 deadline.

## Sprint 1: Security & Master Data Parity (Aug 1 - Aug 14)
- **Goal**: Lock down the framework and prepare all dropdowns/lookups for the clinical forms.
- **Deliverables**:
  - Implement `[ADM-001]` Insurance Setup Module.
  - Execute `[SEC-001]` RBAC Implementation on Sidebar.
  - Finalize all basic Master Data CRUD (Doctors, Procedures).

## Sprint 2: Core Clinical Data Parity (Aug 15 - Aug 28)
- **Goal**: Achieve 100% Field-Level parity on Patient and Demographics.
- **Deliverables**:
  - Implement `[PAT-001]` Patient Advanced Demographics.
  - Integrate Client-side Validation (jQuery Unobtrusive) mirroring Desktop rules.
  - Add `[UX-001]` Patient Photo Upload Component.

## Sprint 3: Advanced UX & Operational Parity (Sep 1 - Sep 15)
- **Goal**: Restore the high-speed operational workflows from the Desktop.
- **Deliverables**:
  - Implement `[SCH-001]` Interactive Calendar Control (Drag & Drop Scheduler).
  - Add `[NAV-001]` Keyboard Shortcuts Handler.
  - Deploy `[NAV-002]` Global Toolbar 'Quick Add'.

## Sprint 4: Clinical Charting R&D (Sep 16 - Oct 15)
- **Goal**: The most technically challenging gap: Visual Dental Charting.
- **Deliverables**:
  - Research/Evaluate Odontogram architecture.
  - Implement `[MED-001]` Clinical Odontogram (Charting).
  - Bind chart selections to the `Tx History` data grid dynamically.

## Sprint 5: Billing & Reporting (Oct 16 - Nov 10)
- **Goal**: Finalize transactional flows and outputs.
- **Deliverables**:
  - Implement `[BIL-001]` Checkout & Payment Modal.
  - Deploy `[REP-001]` Dynamic Report Builder.
  - Setup Document Export Pipeline (PDF/Excel generation).

## Sprint 6: Final UAT & Hardening (Nov 11 - Nov 30)
- **Goal**: Ensure absolute stability and address edge case bugs before deadline.
- **Deliverables**:
  - Address `[UX-002]` Multi-Tab State Management edge cases.
  - Performance Tuning.
  - Final Visual Contract signoff.
