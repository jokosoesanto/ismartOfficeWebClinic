# Clinical Workflow Map

## Overview
This document maps the end-to-end clinical workflow supported by the iSmart Office Web Clinic Prototype.

## 1. Patient Registration & Check-in
* **Walk-in / Phone Call**: 
  * Front desk checks the Schedule (`Dashboard` / `Appointment`)
  * New Patient: Front desk navigates to `Patient > Add Patient` (`Patient_Form`).
  * Existing Patient: Front desk searches for the patient in the `Patient List` (`Patient_List`).
* **Check-in**:
  * Front desk updates the Appointment status to "Waiting".
  * Patient appears in the **Waiting Room** on the `Dashboard`.

## 2. Clinical Evaluation (Dental Chart)
* **Chairside**:
  * Provider clicks on the patient in the Waiting Room to open the **Medical Record** (`MR_Chart`).
  * Provider reviews Patient Details, Medical Alerts, and Treatment History.
  * Provider uses the **Odontogram** to log findings (e.g. Caries on #14).
* **Treatment Planning & Execution**:
  * Provider clicks "Add Treatment" (`MR_TreatmentForm`).
  * Provider selects tooth, surfaces, procedure (e.g. Root Canal).
  * Provider sets status to "Completed" and saves.

## 3. Checkout & Billing
* **Post-Treatment**:
  * Patient proceeds to checkout.
  * Front desk navigates to `Billing > Make Payment` (`Payment_Form`) via Quick Actions or Patient Detail.
  * Front desk reviews the unbilled completed treatments (e.g. $450 Root Canal).
  * Front desk processes payment (Cash, Credit Card, Insurance).
* **Receipt**:
  * After payment, the system generates a receipt (`Payment_Preview`) which can be printed.
  * The transaction is recorded in the **Billing History** (`Payment_History`).

## 4. End of Day & Reporting
* **Inventory Management**:
  * Nurse checks inventory levels (`Inventory_List`). If Composite Resin is low, an alert is visible (`Inventory_Detail`).
* **Financial Reconciliation**:
  * Manager navigates to `Reports` (`ReportViewer`).
  * Manager generates the **Daily Revenue Report** to reconcile cash/credit payments against the till.
