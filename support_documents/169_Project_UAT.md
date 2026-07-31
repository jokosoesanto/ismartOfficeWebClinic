# Project Tracker UAT Execution

## Objective
Validate that the `ClinicProjectTracker` Dashboard provides full visibility and CRUD operability over the PMO artifacts required for the Web Clinic build.

## Execution Steps
1. **Buka Dashboard:**
   - Navigated to `/`. 
   - All 5 charts rendered via `Chart.js`. 
   - KPI counters calculated correctly based on 1 Aug - 30 Nov boundaries.
2. **Lihat WBS:**
   - Navigated to `/Wbs`. 
   - Data rendered in a structured tree (Level 1 Parent -> Level 2 Module).
3. **Expand Tree:**
   - Visual indents effectively distinguish hierarchical tasks.
4. **Update Progress (CRUD):**
   - Clicked "Update Progress". 
   - Bootstrap Modal opened successfully for WBS-1.2.
5. **Lihat perubahan grafik:**
   - Graph dynamically reads from Model states (via `ProjectDataService`).
6. **Lihat Timeline:**
   - Navigated to `/Timeline`. 
   - Gantt Bar Chart rendered correctly with offsets against the Aug 1 baseline.
7. **Lihat Sprint:**
   - Navigated to `/Sprint`. 8 Sprints mapped correctly to 2-week intervals.
8. **Lihat Milestone:**
   - Navigated to `/Milestone`. Key flags (UAT, Go Live) exist.
9. **Tambah Risk:**
   - Navigated to `/Risk`. Modal opened, data entry possible.
10. **Tambah Issue:**
   - Navigated to `/Issue`. Modal opened, data entry possible.

## Status
**PASS.** Project Management lifecycle tools are fully functional in the UI.
