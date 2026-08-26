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
            'Default': '#ffffff',
            'Hover': '#e9ecef',
            'Selected': '#0d6efd'
        };

        if (window.ConditionColors) {
            Object.assign(this.colors, window.ConditionColors);
        }
        if (window.TreatmentColors) {
            Object.assign(this.colors, window.TreatmentColors);
        }

        this.init();
    }

    init() {
        this.render();
    }

    setMode(isAdult) {
        this.isAdult = isAdult;
        this.render();
    }

    getToothType(id) {
        const molars = [1,2,3,14,15,16,17,18,19,30,31,32, 'A','B','I','J','K','L','S','T'];
        const premolars = [4,5,12,13,20,21,28,29];
        const canines = [6,11,22,27, 'C','H','M','R'];
        if (molars.includes(id)) return 'Molar';
        if (premolars.includes(id)) return 'Premolar';
        if (canines.includes(id)) return 'Canine';
        return 'Incisor';
    }

    getClinicalSurface(toothId, svgSurface) {
        const ur = [1,2,3,4,5,6,7,8, 'A','B','C','D','E'];
        const ul = [9,10,11,12,13,14,15,16, 'F','G','H','I','J'];
        const lr = [32,31,30,29,28,27,26,25, 'T','S','R','Q','P'];
        
        let isUpper = ur.includes(toothId) || ul.includes(toothId);
        let isRight = ur.includes(toothId) || lr.includes(toothId);

        if (svgSurface === 'C') return 'O'; // Occlusal/Incisal
        if (svgSurface === 'T') return isUpper ? 'B' : 'P'; // Top is Buccal for Upper, Palatal for Lower
        if (svgSurface === 'B') return isUpper ? 'P' : 'B'; // Bottom is Palatal for Upper, Buccal for Lower
        if (svgSurface === 'L') return isRight ? 'D' : 'M'; // Left is Distal for Right jaw, Mesial for Left jaw
        if (svgSurface === 'R') return isRight ? 'M' : 'D'; // Right is Mesial for Right jaw, Distal for Left jaw
        return svgSurface;
    }

    createToothSVG(id, title) {
        const type = this.getToothType(id);
        const size = 60;
        
        let W, H;
        if (type === 'Molar') { W = 42; H = 42; }
        else if (type === 'Premolar') { W = 32; H = 38; }
        else if (type === 'Canine') { W = 28; H = 38; }
        else { W = 24; H = 36; }

        const cx = 30, cy = 30;
        const dx = W/2, dy = H/2;
        const idx = dx * 0.45, idy = dy * 0.45;

        const tl = `${cx - dx},${cy - dy}`;
        const tr = `${cx + dx},${cy - dy}`;
        const bl = `${cx - dx},${cy + dy}`;
        const br = `${cx + dx},${cy + dy}`;

        const itl = `${cx - idx},${cy - idy}`;
        const itr = `${cx + idx},${cy - idy}`;
        const ibl = `${cx - idx},${cy + idy}`;
        const ibr = `${cx + idx},${cy + idy}`;

        const topQ = `${cx},${cy - dy - 6}`;
        const botQ = `${cx},${cy + dy + 6}`;
        const leftQ = `${cx - dx - 6},${cy}`;
        const rightQ = `${cx + dx + 6},${cy}`;

        const surfaces = {
            'T': `M ${tl} Q ${topQ} ${tr} L ${itr} L ${itl} Z`,
            'B': `M ${bl} Q ${botQ} ${br} L ${ibr} L ${ibl} Z`,
            'L': `M ${tl} Q ${leftQ} ${bl} L ${ibl} L ${itl} Z`,
            'R': `M ${tr} Q ${rightQ} ${br} L ${ibr} L ${itr} Z`,
            'C': `M ${itl} L ${itr} L ${ibr} L ${ibl} Z`
        };

        const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
        svg.setAttribute("width", size);
        svg.setAttribute("height", size);
        svg.setAttribute("viewBox", `0 0 ${size} ${size}`);
        svg.style.cursor = "pointer";
        svg.style.margin = "4px";
        svg.setAttribute("data-tooth", id);

        const g = document.createElementNS("http://www.w3.org/2000/svg", "g");
        
        for (const [key, pathData] of Object.entries(surfaces)) {
            const path = document.createElementNS("http://www.w3.org/2000/svg", "path");
            path.setAttribute("d", pathData);
            path.setAttribute("fill", this.getSurfaceColor(id, key));
            path.setAttribute("stroke", "#adb5bd");
            path.setAttribute("stroke-width", "1.5");
            path.setAttribute("stroke-linejoin", "round");
            path.setAttribute("data-surface", key);
            
            path.addEventListener("mouseover", (e) => this.handleHover(e, true));
            path.addEventListener("mouseout", (e) => this.handleHover(e, false));
            path.addEventListener("click", (e) => this.handleClick(id, key));
            g.appendChild(path);
        }



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
            const clinicalSurface = this.getClinicalSurface(toothId, surfaceId);
            this.onSelectSurface(toothId, clinicalSurface);
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

    getSvgSurface(toothId, clinicalSurface) {
        if (!clinicalSurface) return 'C';
        const ur = [1,2,3,4,5,6,7,8, 'A','B','C','D','E'];
        const ul = [9,10,11,12,13,14,15,16, 'F','G','H','I','J'];
        const lr = [32,31,30,29,28,27,26,25, 'T','S','R','Q','P'];
        
        // Use loose equality to support both string and int toothIds
        let isUpper = ur.some(id => id == toothId) || ul.some(id => id == toothId);
        let isRight = ur.some(id => id == toothId) || lr.some(id => id == toothId);

        if (clinicalSurface === 'O' || clinicalSurface === 'I') return 'C';
        if (clinicalSurface === 'B') return isUpper ? 'T' : 'B';
        if (clinicalSurface === 'P' || clinicalSurface === 'L') return isUpper ? 'B' : 'T';
        if (clinicalSurface === 'D') return isRight ? 'L' : 'R';
        if (clinicalSurface === 'M') return isRight ? 'R' : 'L';
        return 'C';
    }

    loadState(treatments) {
        if (!treatments || !Array.isArray(treatments)) return;
        
        treatments.forEach(t => {
            if (t.siteNumber && t.siteDetail) {
                // Determine if tooth is int or string
                const toothId = isNaN(t.siteNumber) ? t.siteNumber : parseInt(t.siteNumber);
                const svgSurface = this.getSvgSurface(toothId, t.siteDetail);
                const key = `${toothId}_${svgSurface}`;
                
                let resolvedColor = this.colors['Missing']; // default
                if (t.treatmentItemName) {
                    // Look up from historical metadata dictionary FIRST
                    if (window.TreatmentColors && window.TreatmentColors[t.treatmentItemName]) {
                        resolvedColor = window.TreatmentColors[t.treatmentItemName];
                    } else {
                        // Fallback to legacy colors if not found in dictionary
                        for (const c in this.colors) {
                            if (t.treatmentItemName.toLowerCase().includes(c.toLowerCase())) {
                                resolvedColor = this.colors[c];
                                break;
                            }
                        }
                    }
                }
                
                this.state[key] = {
                    type: 'entry',
                    code: t.treatmentItemName || 'Treatment',
                    color: resolvedColor
                };
            }
        });
        
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
