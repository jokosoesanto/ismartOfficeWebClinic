# Project Work Breakdown Structure (WBS)

## Overview
This document defines the complete Work Breakdown Structure for the iSmartOffice Web Clinic Production Development Phase (1 Aug 2026 - 30 Nov 2026). It serves as the backlog source for the ClinicProjectTracker PMO application.

## 1. iSmartOffice Web Clinic Production (Level 1)
- **Status:** In Progress
- **Duration:** 1 Aug 2026 - 30 Nov 2026

### 1.1 Patient Module
- Implement CRUD operations
- Wire Entity Framework to Database
- **PIC:** Backend Team

### 1.2 Appointment Module
- Implement Calendar logic (FullCalendar.js backend sync)
- Handle double booking validation
- **PIC:** Fullstack Team

### 1.3 Medical Record Module
- Form validations and temporal logic
- **PIC:** Core Business Logic Team

#### 1.3.1 Odontogram Engine
- Wire SVG interactions to Patient condition history
- Handle historical tooth extraction states
- **PIC:** Frontend Specialists

### 1.4 Billing Module
- Generate Invoices
- Payment Gateway integration
- **PIC:** Financial Team

### 1.5 Reporting & Analytics
- Crystal Reports / SSRS porting to web
- Build dashboard endpoints
- **PIC:** Data Team

### 1.6 UAT & Deployment
- System Integration Testing
- User Acceptance Testing
- Go Live on Cloud
- **PIC:** QA & DevOps

*This WBS is seeded into the ClinicProjectTracker as the primary tree structure for monitoring.*
