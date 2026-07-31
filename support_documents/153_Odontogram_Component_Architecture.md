# Odontogram Component Architecture

## Architectural Decisions
To strictly adhere to the technical rules defined in the sprint, the Odontogram was designed from the ground up as a native Javascript Component rendering Scalable Vector Graphics (SVG).

### 1. Reusability & Independence
- The `Odontogram` class in `odontogram.js` is entirely detached from backend logic, meaning it can be instantiated anywhere within the frontend by supplying a container ID (e.g., `new Odontogram("odontogramContainer")`).

### 2. SVG Programmatic Generation
- Instead of using a giant hard-coded `.svg` image with 160 paths (32 teeth * 5 surfaces), the Javascript loops through Universal Numbering arrays (`1-16`, `32-17` for Adults, `A-J`, `T-K` for Children).
- Each tooth generates a standard 5-surface configuration (`T, B, L, R, C`) dynamically using `document.createElementNS`.
- **Benefit:** If the layout or size requirements change (e.g., switching to FDI numbering), it requires updating only a single array in Javascript rather than re-drawing an entire image file.

### 3. Data Structure (State Management)
- The chart maintains an internal dictionary `this.state` linking keys (e.g., `"14_C"`) to applied treatments.
- A `history` array stores stringified JSON snapshots of this state dictionary before every change, enabling the `undo()` stack.

### 4. DOM Integration
- The class exposes an `onSelectSurface(tooth, surface)` callback. This is intercepted in `MR_Chart.cshtml` to link the isolated SVG interactions with the HTML input fields, ensuring the rest of the page (like Treatment Forms and Billing links) reacts appropriately.
