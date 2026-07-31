# Workflow Parity Report

## 1. Patient Check-In Workflow
**Desktop**: 
`Receptionist clicks 'Check-In' on Schedule -> Patient Status changes to 'Waiting' -> Dialog pops up to verify demographics -> Moves to Waitlist Panel.`

**Web Prototype**: 
- The concept of the 'Waitlist Panel' is modeled on the Dashboard.
- **Gap**: The transactional flow (triggering Demographics confirmation from a Schedule action) is missing. The prototype requires manual navigation between the Schedule and Patient modules to achieve this.

## 2. Clinical Charting Workflow
**Desktop**: 
`Select Patient -> Go to 'Dental Chart' -> Click Tooth -> Select Condition/Treatment -> Treatment is added to Grid -> Charges are auto-calculated.`

**Web Prototype**:
- Only the "Tx History" (Treatment History) datagrid is mocked out via Lazy Components.
- **Gap**: The interactive Odontogram (Graphic Tooth Charting) and the auto-calculation flow to Billing is completely missing. This is a highly complex workflow that needs heavy Javascript UI work.

## 3. Reporting & Export Workflow
**Desktop**: 
`Open frmExportReport -> Select Report -> Select Format (PDF, Excel, Word) -> Click Save -> System prompts File Dialog.`

**Web Prototype**:
- Reports module is scaffolded as a Dashboard view.
- **Gap**: Missing the Export pipeline. Web will need an API-driven document generator (e.g., SSRS or Rotativa) and a browser download response stream.

## 4. Payment Collection Workflow
**Desktop**: 
`Patient Checkout -> Open frmPayment -> Enter Amount -> Select Method -> Print Receipt.`

**Web Prototype**:
- Scaffolded Billing module.
- **Gap**: The popup/dialog for rapid payment entry (`frmPayment`) is missing. Needs a Modal Component architecture to replicate this seamless UX without leaving the context of the Patient page.
