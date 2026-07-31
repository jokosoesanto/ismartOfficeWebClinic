# Business Rule Gap Report

## Overview
This report evaluates implicit and explicit business rules hardcoded or implemented within the Desktop Application (`iSmartOffice_Desktop`) compared to the current Web Prototype framework.

## 1. Appointment Concurrency Rules
- **Desktop**: Blocks booking an appointment for a patient if they already have an active appointment on the exact same time/date slot (Double-booking rule). It prompts a warning dialog `frmWarning`.
- **Web Gap**: The prototype currently lacks the API integration layer to perform pre-booking validation checks. Rule is MISSING.

## 2. Global State "Active Patient"
- **Desktop**: Maintaining an "Active Patient" is an intrinsic part of the application state. Opening a new Billing form automatically anchors it to the active patient context.
- **Web Gap**: The prototype mimics this via the top Header (showing a mocked active patient). However, navigating directly to `/Billing` does not yet hydrate data contextually based on that header. Rule is PARTIAL.

## 3. Data Deletion Protection
- **Desktop**: Prevents deleting a Patient if they have existing Medical Records or Financial Balances.
- **Web Gap**: Delete operations on datagrids currently lack confirmation dialogues (`SweetAlert` or Bootstrap Modals) and cascading rule checks. Rule is MISSING.

## 4. Role-Based Access Control (RBAC)
- **Desktop**: Modules map directly to boolean flags retrieved upon login, selectively hiding Ribbon elements.
- **Web Gap**: The `NavigationProvider` has a `RequiredPermission` string parameter mapped out. The rendering engine must be updated to securely filter the navigation tree and authorize controller actions. Rule is PARTIAL.
