# Project Timeline

## Execution Baseline
- **Kickoff:** 1 Aug 2026
- **Deadline:** 30 Nov 2026
- **Total Duration:** 121 Days

## Milestone Map
| Milestone | Target Date | Status |
|-----------|-------------|--------|
| Prototype Freeze | 31 Jul 2026 | Achieved |
| Backend Integration | 30 Sep 2026 | Pending |
| Testing Phase | 15 Oct 2026 | Pending |
| UAT | 15 Nov 2026 | Pending |
| Release Candidate | 25 Nov 2026 | Pending |
| Go Live | 30 Nov 2026 | Pending |

## Gantt Logic
The Timeline Module in `ClinicProjectTracker` dynamically reads the WBS planned dates (Level 1 and 2 tasks) to plot a horizontal bar chart representing the execution path. Offset dates are calculated against the baseline `1 Aug 2026`. Delay tracking will compare `ActualStart` against `PlannedStart`.
