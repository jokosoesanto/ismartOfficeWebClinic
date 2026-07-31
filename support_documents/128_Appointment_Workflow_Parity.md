# Appointment Workflow Parity Report

## Create Appointment
- **Desktop**: Select time on timeline, double click, opens form.
- **Web**: Click "New Appt" on toolbar, opens `Scheduler_Form.cshtml`. Or drag patient from Waiting List onto the calendar. 
- **Parity Status**: `MATCH`.

## Edit / Reschedule Appointment
- **Desktop**: Drag and drop across the timeline.
- **Web**: Integrated `FullCalendar` drag-and-drop. Fires a JS confirmation dialog mimicking Desktop protection rules.
- **Parity Status**: `MATCH`.

## Check-In
- **Desktop**: Right click -> Check In -> Moves to Waiting List / Status changes to Arrived.
- **Web**: Click Appointment block -> Modal opens -> Click 'Check-In' button. Or click 'Check-In' directly from the Waiting List panel.
- **Parity Status**: `MATCH` (UX adapted for Web constraints).

## Open Patient from Appointment
- **Desktop**: Right click -> Go to Patient.
- **Web**: Click Appointment block -> Modal opens -> Click "View Full Chart" link.
- **Parity Status**: `MATCH`.

## Cancel Appointment
- **Desktop**: Delete/Cancel button requires a confirmation dialog.
- **Web**: Both the 'Cancel Appt' modal button and the form submission trigger a JS `confirm()` prompt before allowing cancellation.
- **Parity Status**: `MATCH`.
