# Billing Business Rules Implementation

## Overview
This document details the client-side Business Rule implementations added to the Billing Capability to prevent invalid transactions, ensuring parity with the Desktop validation layer.

## Implemented Rules
### 1. Payment Cannot Be Negative
**Implementation:** The HTML5 input `min="0"` attribute was added to the Payment Amount field, and an explicit Javascript check triggers an `alert()` if a negative value bypasses the UI constraints.
**Status:** Parity Achieved.

### 2. Outstanding Calculation Logic
**Implementation:** Javascript recalculates the balances in real-time when inputs change (`change` and `input` event listeners).
**Formula:** `Outstanding = (Total Bill - Discount) - Insurance - Payment`. 
**Constraint:** If `Outstanding` goes negative, it is clamped to `$0.00` for display purposes (overpayment/credit logic is deferred to backend).
**Status:** Parity Achieved.

### 3. Insurance Constraint
**Implementation:** Javascript intercepts the `submit` event to verify `Insurance Coverage <= Net Bill`. If violated, execution is blocked and the user is warned.
**Status:** Parity Achieved.

### 4. Final Confirmation
**Implementation:** A Javascript `confirm()` dialog intercepts the `submit` event before navigating away to the Receipt view, asking "Are you sure you want to finalize this payment?".
**Status:** Parity Achieved.
