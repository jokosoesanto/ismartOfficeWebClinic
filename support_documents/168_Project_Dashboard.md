# Project PMO Dashboard

## Design Strategy
The Dashboard module acts as the nerve center for the Chief Enterprise Architect to monitor project health in real-time.

## Analytics Grid
The UI relies heavily on `Chart.js` to render the 9 requested metrics:
1. **Burndown Chart:** Tracks remaining Story Points across Sprints.
2. **Burnup Chart / Planned vs Actual:** Line chart overlaying expected progress against actual completions.
3. **Velocity Chart:** Bar chart showing completed story points per sprint.
4. **Pie Progress:** Doughnut chart splitting Complete vs Remaining overall percentages.
5. **WBS Progress:** Reflected in table form and Gantt duration (Timeline).
6. **Sprint Progress:** Visualized in active Sprint cards.
7. **KPI Counters:** Top level cards for "Remaining Days", "Open Risks", "Overall %", and "Active Sprint".

This provides a single pane of glass without overwhelming the user.
