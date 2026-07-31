# End-to-End System UAT Plan

## UAT Scenario: Full Clinic Patient Lifecycle

### Pre-conditions
- Prototype is hosted and accessible.
- Default Medical Blue theme is active.

### Execution Steps
1. **Login:** Simulate Authentication.
   - *Result:* Routes to Main Dashboard successfully.
2. **Patient Registration:** Navigate to `/Patient/Create`. Fill mock data and Submit.
   - *Result:* Returns to Patient List.
3. **Scheduling:** Navigate to `/Appointment`. Select a calendar slot.
   - *Result:* Quick Schedule Modal opens. Links back to the patient.
4. **Clinical Examination:** Navigate to `/MedicalRecord`. Open John Smith's chart.
   - *Result:* Odontogram and timelines render correctly.
5. **Charting:** Add a Caries diagnosis to Tooth #4. Add a Crown treatment to Tooth #11.
   - *Result:* Visual updates occur immediately on the SVG chart. Lists populate.
6. **Billing Transfer:** Click "Go to Billing".
   - *Result:* Financial module activates.
7. **Payment:** Execute a dummy payment in `/Billing`.
   - *Result:* Invoice status visual changes (e.g., Unpaid to Paid).
8. **Reporting:** Navigate to `/Report`. Select "Daily Production" and click View.
   - *Result:* Parameter panels work. Simulated Reportviewer displays.

### Conclusion
**Status:** PASS. The prototype effectively communicates the business logic, workflow, and visual aesthetic required by the development team to commence backend wiring.
