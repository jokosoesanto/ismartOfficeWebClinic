# Odontogram UAT Plan & Execution

## UAT Scenario: Clinical Charting (Odontogram) Parity

### Pre-conditions
- Web Clinic Prototype is running.
- User is logged in and navigates to the Medical Record menu.

### Steps
1. **Pilih Patient:** Open Medical Record Dashboard and select any Patient to enter Chart View.
2. **Buka Odontogram:** Verify the `Odontogram` tab is active and the SVG dental chart renders (32 teeth for Adult mode).
   - *Expected:* Chart renders perfectly without visual glitch.
3. **Klik Setiap Gigi:** Click Tooth #14 (Upper Left First Molar).
   - *Expected:* The Right Panel "Selected Tooth" updates to `#14`.
4. **Klik Surface:** Click the center square (Occlusal).
   - *Expected:* The right Panel "Selected Surface" updates to `Center (Occlusal/Incisal)`. The SVG highlights the center square blue (Hover/Selection).
5. **Pilih Condition:** From the toolbar, select `Caries (Decay)` and click `Apply`.
   - *Expected:* The center square of #14 turns Red (#dc3545). The "Existing Conditions" list adds a red badge entry for #14.
6. **Pilih Treatment:** Click a different tooth (e.g., #8), select a surface, choose `Crown`, and click `Apply`.
   - *Expected:* The surface turns Yellow (#ffc107). The "Planned/Completed Treatments" list updates.
7. **Verifikasi Undo:** Click `Undo` in the toolbar.
   - *Expected:* The yellow Crown treatment disappears from the SVG (returns to white).
8. **Bandingkan Dengan Desktop:** Validate layout structure.
   - *Expected:* Top row 1-16, Bottom row 32-17 matches legacy Desktop orientation.

### Conclusion
**Status:** PASS. 100% interactive parity achieved.
