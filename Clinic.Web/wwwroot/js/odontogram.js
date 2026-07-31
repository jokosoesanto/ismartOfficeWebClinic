class Odontogram {
    constructor(containerId) {
        this.container = document.getElementById(containerId);
        this.isAdult = true;
        this.selectedTooth = null;
        this.selectedSurface = null;
        this.state = {}; // format: { '14_O': { type: 'condition', code: 'Caries', color: 'red' } }
        this.history = []; // for undo
        this.onSelectSurface = null; // callback

        this.colors = {
            'Caries': '#dc3545',
            'Filling': '#0dcaf0',
            'Crown': '#ffc107',
            'Missing': '#6c757d',
            'RCT': '#198754',
            'Default': '#ffffff',
            'Hover': '#e9ecef',
            'Selected': '#0d6efd'
        };

        this.init();
    }

    init() {
        this.render();
    }

    setMode(isAdult) {
        this.isAdult = isAdult;
        this.render();
    }

    createToothSVG(id, title) {
        const size = 60;
        const o = 15; // offset for inner square
        const innerSize = size - 2 * o;
        
        const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
        svg.setAttribute("width", size);
        svg.setAttribute("height", size);
        svg.setAttribute("viewBox", `0 0 ${size} ${size}`);
        svg.style.cursor = "pointer";
        svg.style.margin = "4px";
        svg.setAttribute("data-tooth", id);

        // Define surfaces: T (Top), B (Bottom), L (Left), R (Right), C (Center)
        const surfaces = {
            'T': `0,0 ${size},0 ${size-o},${o} ${o},${o}`,
            'B': `0,${size} ${size},${size} ${size-o},${size-o} ${o},${size-o}`,
            'L': `0,0 ${o},${o} ${o},${size-o} 0,${size}`,
            'R': `${size},0 ${size-o},${o} ${size-o},${size-o} ${size},${size}`
        };

        const g = document.createElementNS("http://www.w3.org/2000/svg", "g");
        
        for (const [key, points] of Object.entries(surfaces)) {
            const polygon = document.createElementNS("http://www.w3.org/2000/svg", "polygon");
            polygon.setAttribute("points", points);
            polygon.setAttribute("fill", this.getSurfaceColor(id, key));
            polygon.setAttribute("stroke", "#6c757d");
            polygon.setAttribute("stroke-width", "1");
            polygon.setAttribute("data-surface", key);
            
            polygon.addEventListener("mouseover", (e) => this.handleHover(e, true));
            polygon.addEventListener("mouseout", (e) => this.handleHover(e, false));
            polygon.addEventListener("click", (e) => this.handleClick(id, key));
            g.appendChild(polygon);
        }

        // Center square
        const rect = document.createElementNS("http://www.w3.org/2000/svg", "rect");
        rect.setAttribute("x", o);
        rect.setAttribute("y", o);
        rect.setAttribute("width", innerSize);
        rect.setAttribute("height", innerSize);
        rect.setAttribute("fill", this.getSurfaceColor(id, 'C'));
        rect.setAttribute("stroke", "#6c757d");
        rect.setAttribute("stroke-width", "1");
        rect.setAttribute("data-surface", 'C');

        rect.addEventListener("mouseover", (e) => this.handleHover(e, true));
        rect.addEventListener("mouseout", (e) => this.handleHover(e, false));
        rect.addEventListener("click", (e) => this.handleClick(id, 'C'));
        g.appendChild(rect);

        // Add text label below or above based on jaw
        svg.appendChild(g);
        
        const container = document.createElement("div");
        container.style.display = "flex";
        container.style.flexDirection = "column";
        container.style.alignItems = "center";
        
        const label = document.createElement("small");
        label.className = "fw-bold text-muted";
        label.innerText = title || id;
        
        if (id <= 16 || (id >= 'A' && id <= 'J')) {
            container.appendChild(svg);
            container.appendChild(label);
        } else {
            container.appendChild(label);
            container.appendChild(svg);
        }
        
        return container;
    }

    getSurfaceColor(toothId, surfaceId) {
        const key = `${toothId}_${surfaceId}`;
        if (this.selectedTooth === toothId && this.selectedSurface === surfaceId) return this.colors['Selected'];
        if (this.state[key]) return this.colors[this.state[key].code] || this.state[key].color;
        return this.colors['Default'];
    }

    handleHover(e, isEnter) {
        const el = e.target;
        const currentFill = el.getAttribute("fill");
        if (currentFill !== this.colors['Selected'] && !this.state[`${el.parentNode.parentNode.getAttribute('data-tooth')}_${el.getAttribute('data-surface')}`]) {
            el.setAttribute("fill", isEnter ? this.colors['Hover'] : this.colors['Default']);
        }
    }

    handleClick(toothId, surfaceId) {
        this.selectedTooth = toothId;
        this.selectedSurface = surfaceId;
        this.render(); // Re-render to show selection
        if (this.onSelectSurface) {
            this.onSelectSurface(toothId, surfaceId);
        }
    }

    applyTreatment(conditionOrTreatment, color) {
        if (!this.selectedTooth || !this.selectedSurface) return;
        
        const key = `${this.selectedTooth}_${this.selectedSurface}`;
        
        // Save history for undo
        this.history.push(JSON.stringify(this.state));
        
        this.state[key] = {
            type: 'entry',
            code: conditionOrTreatment,
            color: color
        };
        
        this.selectedTooth = null;
        this.selectedSurface = null;
        this.render();
    }

    undo() {
        if (this.history.length > 0) {
            const prevState = this.history.pop();
            this.state = JSON.parse(prevState);
            this.render();
        }
    }

    clearSelection() {
        this.selectedTooth = null;
        this.selectedSurface = null;
        this.render();
    }

    render() {
        this.container.innerHTML = "";
        
        const wrapper = document.createElement("div");
        wrapper.className = "d-flex flex-column align-items-center gap-4 w-100";
        
        const upperRow = document.createElement("div");
        upperRow.className = "d-flex flex-wrap justify-content-center gap-1";
        
        const lowerRow = document.createElement("div");
        lowerRow.className = "d-flex flex-wrap justify-content-center gap-1";

        if (this.isAdult) {
            // Adult 1-16
            for (let i = 1; i <= 16; i++) {
                upperRow.appendChild(this.createToothSVG(i));
            }
            // Adult 32-17
            for (let i = 32; i >= 17; i--) {
                lowerRow.appendChild(this.createToothSVG(i));
            }
        } else {
            // Child A-J
            const upperChild = ['A','B','C','D','E','F','G','H','I','J'];
            upperChild.forEach(id => upperRow.appendChild(this.createToothSVG(id)));
            // Child T-K
            const lowerChild = ['T','S','R','Q','P','O','N','M','L','K'];
            lowerChild.forEach(id => lowerRow.appendChild(this.createToothSVG(id)));
        }

        wrapper.appendChild(upperRow);
        
        // Add a visual separator line (midline) if wanted, or just gap
        const midline = document.createElement("div");
        midline.style.width = "80%";
        midline.style.height = "2px";
        midline.style.backgroundColor = "#dee2e6";
        midline.style.margin = "10px 0";
        wrapper.appendChild(midline);
        
        wrapper.appendChild(lowerRow);

        this.container.appendChild(wrapper);
    }
}
