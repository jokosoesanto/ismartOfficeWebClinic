# Billing UAT Plan & Execution

## UAT Scenario: End-to-End Billing Workflow

### Pre-conditions
- Web Clinic Prototype is running.
- User is on the Billing Dashboard (`/Billing`).

### Steps
1. **Buka Billing Dashboard:** Navigate to `/Billing`. 
   - *Expected:* Invoice list is displayed with Search and Filter tools.
2. **Pilih Patient:** Locate John Smith (Unpaid) and click the `Pay` button.
   - *Expected:* System navigates to `/Billing/Payment`.
3. **Verifikasi Treatment Summary:** Observe the 'Appointments to Pay' section.
   - *Expected:* Both 'Root Canal' ($450.00) and 'Panoramic X-Ray' ($150.00) are listed and checked. Total Bill shows $600.00.
4. **Terapkan Discount:** Enter `10` in the Discount (%) field.
   - *Expected:* Discount Applied updates to `-$60.00`. Net Bill updates to `$540.00`.
5. **Pilih Insurance:** Enter `100.00` in the Insurance Coverage field.
   - *Expected:* Remaining Outstanding updates calculations correctly.
6. **Input Payment:** Enter `-50` in Payment Amount and click 'Make Payment'.
   - *Expected:* Business Rule validation blocks it. Change it to `440.00`.
7. **Simpan Payment:** Click 'Make Payment'.
   - *Expected:* Confirmation dialog appears. Clicking OK proceeds.
8. **Lihat Receipt Preview:** User is navigated to `/Billing/Preview/1`.
   - *Expected:* The Receipt displays Subtotal ($600), Discount, Insurance, Total Payment, and Outstanding cleanly.
9. **Lihat Payment History:** Click Close on Receipt, navigate to `/Billing/History`.
   - *Expected:* Historical logs are present matching the transaction.

### Conclusion
**Status:** PASS. The prototype workflow correctly replicates the Desktop user journey.
