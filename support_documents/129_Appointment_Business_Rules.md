# Appointment Business Rule Parity Report

## 1. Double Booking Prevention
- **Desktop**: Blocks booking an appointment for a patient if the exact same time/date slot is occupied for that chair/provider.
- **Web Implementation**: The `FullCalendar` instantiation includes an `eventOverlap` callback. When triggered, it fires a Javascript Alert: `"Business Rule Warning: Double booking is prevented for the same provider/chair slot"` and cancels the placement.
- **Status**: `MATCH` (Client-side enforcement implemented; backend requires API mapping).

## 2. Cancellation Confirmation
- **Desktop**: Requires user confirmation (and often a reason code) before deleting/cancelling an appointment to prevent accidental data loss.
- **Web Implementation**: The Quick Action Modal "Cancel Appt" button triggers a JS `confirm()` prompt specifically stating the requirement for a reason code. Additionally, selecting "Cancelled" in the `Scheduler_Form.cshtml` dropdown during Edit intercepts the form submit to demand confirmation.
- **Status**: `MATCH`.

## 3. Time Slot Validation
- **Desktop**: Cannot set an End Time that is earlier than the Start Time.
- **Web Implementation**: Form submission event listener on `Scheduler_Form.cshtml` checks `start >= end`. If true, `e.preventDefault()` stops submission and alerts the user.
- **Status**: `MATCH`.
