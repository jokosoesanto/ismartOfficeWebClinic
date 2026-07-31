# Appointment Validation Forensic Report

## Objective
Identify the root cause of the Appointment Form Validation failure reported during Manual UAT, where the "End time must be after Start time" validation did not fire.

## Root Cause Analysis (RCA)

### 1. Client Script & JavaScript Event Analysis
- **Finding:** The JavaScript validation logic is bound to the form's `submit` event:
  ```javascript
  document.querySelector('form').addEventListener('submit', function(e) { ... });
  ```
- **Finding:** The "Save Appointment" button is defined as:
  ```html
  <a href="/Appointment" class="btn btn-primary shadow-sm"><i class="bi bi-save me-2"></i>Save Appointment</a>
  ```
- **Analysis:** Because the "Save" action is implemented using an anchor tag (`<a>`) with an `href` attribute, clicking it triggers a standard browser navigation (HTTP GET to `/Appointment`). It **does not** trigger a form submission. Therefore, the `submit` event listener is never fired, bypassing both the custom JavaScript time validation and the native HTML5 `required` field validations.

### 2. Validation Message Rendering
- **Finding:** Since the event listener is bypassed, `alert('End time must be after Start time.')` is never reached.

### 3. Native HTML Validation
- **Finding:** HTML5 validations (e.g., `required` on Provider, Chair) are also completely bypassed because the form is never technically submitted.

## Conclusion
The defect is caused by incorrect HTML markup on the submit action. Using an `<a>` tag instead of a `<button type="submit">` circumvents the browser's form submission lifecycle, causing all client-side validations to be ignored.

## Recommended Fix
1. Change the "Save Appointment" anchor tag (`<a>`) to a `<button type="submit">`.
2. Ensure the custom validation script intercepts the submit, and if valid, manually handles the redirect to `/Appointment` since this is a prototype without a real POST endpoint, OR use a Javascript click handler on the button to run the validation. Given it's a prototype, we should handle the validation in JS and manually redirect on success.
