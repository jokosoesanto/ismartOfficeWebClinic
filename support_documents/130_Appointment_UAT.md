# Appointment Module UAT (User Acceptance Testing)

## Capability: Appointment Complete Parity

### Scenario 1: Interactive Scheduler Navigation
**Steps:**
1. Navigate to `Web Prototype -> Appointment`.
2. **Verify** the main calendar uses `FullCalendar` and defaults to Weekly view (`timeGridWeek`).
3. Click the `Day` and `Month` buttons in the top-right toolbar.
4. **Verify** the calendar switches views accordingly.
5. **Verify** the Top Toolbar contains the "Filter" dropdown. Click it.
6. **Verify** Providers (Dr. Alan, Dr. Emily) and "Show Cancelled" switch are present.

### Scenario 2: Double Booking / Reschedule (Drag & Drop)
**Steps:**
1. Locate the existing appointment block for "John Smith" (Blue, 09:00 - 10:00).
2. Click and Drag the block to 14:00.
3. **Verify** a JS Confirmation prompts: *"Reschedule appointment to [Date/Time]?"*. Click OK.
4. Drag the block directly on top of another existing appointment block.
5. **Verify** a JS Alert blocks the action: *"Business Rule Warning: Double booking is prevented..."* and the block returns to its original slot.

### Scenario 3: Context Menu Replacement (Detail Modal)
**Steps:**
1. Click the "Jane Doe" appointment block (Green).
2. **Verify** a Modal pops up titled "Appointment Detail".
3. **Verify** the Modal header is Green (matching the Arrived status).
4. **Verify** Patient Name, Time, Provider, and Reason are populated.
5. Click the `Check-In` button in the quick actions area.
6. **Verify** an alert says "Appointment status changed to: Arrived" and the modal closes.

### Scenario 4: Waiting List Drag-and-Drop
**Steps:**
1. Observe the "Waiting List" panel on the right side of the screen.
2. Locate the draggable item "Maria Doe (Walk-in)".
3. Drag the item into an empty time slot on the Calendar.
4. **Verify** the calendar accepts the event (FullCalendar drop handling).

### Scenario 5: Appointment Form Validation
**Steps:**
1. Click **New Appt** on the top toolbar.
2. Observe the form. **Verify** `Chair`, `Room`, and `Status` dropdowns exist.
3. Set **Start Time** to `10:00` and **End Time** to `09:00`.
4. Click **Save Appointment**.
5. **Verify** the browser prevents submission with an alert: *"End time must be after Start time."*
6. Change **Status** to `Cancelled` and click **Save Appointment**.
7. **Verify** a JS confirmation asks: *"Are you sure you want to cancel this appointment?"*
