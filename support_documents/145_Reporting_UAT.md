# Reporting UAT Plan & Execution

## UAT Scenario: End-to-End Reporting Workflow

### Pre-conditions
- Web Clinic Prototype is running.
- User is on the Reporting Dashboard (`/Report`).

### Steps
1. **Buka Report:** Navigate to `/Report`. 
   - *Expected:* The Report Viewer loads. An Empty State graphic is displayed in the main panel. The Parameter panel is greyed out.
2. **Cari Report:** Locate the Search box above the categories or browse the `Financial Reports` category. Click on `Daily Revenue`.
   - *Expected:* The sidebar highlights the selected report. The Parameter panel becomes active. The empty state text changes to "Report Configured".
3. **Filter Report:** Configure the parameters (e.g. Set Date Range to 'This Month', Group by 'Provider').
4. **Preview:** Click the `Generate Preview` button in the Parameter panel.
   - *Expected:* The empty state disappears and the actual A4 document mock is displayed on the screen.
5. **Export Placeholder:** Click the `Export PDF` or `Export Excel` buttons in the top header, or interact with the preview toolbar.
   - *Expected:* Placeholders exist to verify visual placement according to the Desktop layout.

### Conclusion
**Status:** PASS. The prototype workflow correctly replicates the Desktop user journey for report generation.
