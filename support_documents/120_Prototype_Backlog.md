# Prototype Implementation Backlog

## Prioritization Criteria
1. **Critical Business Feature**: Prevents basic clinic operations if missing.
2. **Operational Feature**: Necessary for daily efficiency.
3. **Supporting Feature**: Backend, reporting, or data management.
4. **Nice to Have**: UX enhancements, non-critical visuals.

---

## 1. Critical Business Features
- **[PAT-001] Patient Advanced Demographics**: Map all missing fields (SSN, Occupation, Gender dropdown, BloodType) to the Patient Detail ViewComponent.
- **[SCH-001] Interactive Calendar Control**: Replace static Scheduler grid with a JS-based Calendar (FullCalendar) supporting Drag-and-Drop and context menus.
- **[MED-001] Clinical Odontogram (Charting)**: Research and implement an SVG/Canvas based tooth charting module to replicate Desktop functionality.
- **[BIL-001] Checkout & Payment Modal**: Implement a global modal accessible from Patient/Schedule to capture rapid payments.

## 2. Operational Features
- **[NAV-001] Keyboard Shortcuts Handler**: Implement JS listener to bind F2/F3 keys to global search and Add Patient functions.
- **[NAV-002] Global Toolbar 'Quick Add'**: Add a floating action button or Header dropdown for rapid creation of records.
- **[ADM-001] Insurance Setup Module**: Port `frmInsurance` to a new Razor template under Administration to allow configuration of Insurance providers.

## 3. Supporting Features
- **[SEC-001] RBAC Implementation**: Wire the `NavigationProvider`'s `RequiredPermission` string to actual ASP.NET Core Authorization Policies.
- **[REP-001] Dynamic Report Builder**: Migrate the static reporting view to a dynamic field-picker mimicking `frmCustomReport`.

## 4. Nice to Have
- **[UX-001] Patient Photo Upload Component**: Activate the placeholder avatar with a JS cropper and file upload mechanism.
- **[UX-002] Multi-Tab/Multi-Context State Management**: Enhance the Active Patient global state to support multiple patients across browser tabs using LocalStorage/SessionStorage.
