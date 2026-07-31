# Medical Record UAT Plan & Execution

## UAT Scenario: End-to-End Medical Record & Charting Workflow

### Pre-conditions
- Web Clinic Prototype is running.
- User is logged in and navigates to the Medical Record menu.

### Steps
1. **Buka Medical Record:** Navigate to `/MedicalRecord`. 
   - *Expected:* The MR Dashboard loads. A list of patients with search and filter capabilities is displayed.
2. **Pilih Patient:** Locate John Smith and click the `View Chart` button.
   - *Expected:* System navigates to `/MedicalRecord/Chart/1`.
3. **Verifikasi Dashboard Patient:** Check the top summary panel.
   - *Expected:* Patient age, last visit date, blood type, insurance, and allergy alerts (Penicillin - Severe) are prominently displayed.
4. **Verifikasi Diagnosis & Treatment:** Click through the `Odontogram` tab.
   - *Expected:* Existing conditions (Caries) and Completed Treatments (Root Canal) are listed clearly below the chart placeholder.
5. **Verifikasi Timeline:** Click the `Visit & Treatment History` tab.
   - *Expected:* A chronological timeline displays past visits grouped by dates and providers, with embedded clinical notes.
6. **Verifikasi Modals:** Click the `Clinical Notes` tab and select `New Note`.
   - *Expected:* A Bootstrap modal appears to capture new Note details without leaving the page.
7. **Input Validation:** Click `Add Treatment / Note` in the top right to reach the treatment form. Leave the procedure dropdown empty (if possible, or change its value via DOM) and attempt to save.
   - *Expected:* JavaScript validation intercept warns the user.
8. **Transisi ke Billing:** Use the `Go to Billing` button in the chart header.
   - *Expected:* Seamless transition to the Billing capability dashboard.

### Conclusion
**Status:** PASS. The prototype workflow correctly replicates the Desktop user journey across all sub-features of the Medical Record capability.
