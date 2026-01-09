# Team Members

| Full Name           | ID         |
|--------------------|------------|
| Bethelihem Wondimneh | UGR/7686/15 |
| Entisar Elias       | UGR/4241/15 |
| Hikma Oumer         | UGR/6192/15 |
| Teyiba Aman         | UGR/7407/15 |
| Tsedenya Bazezew    | UGR/9693/15 |



# Project Proposal: Smart Clinic Management System

─────────────────────────────────────────────
## 1. Project Overview
The Smart Clinic Management System (SCMS) aims to streamline healthcare operations, improve patient experience, and enable efficient clinic management. It addresses issues such as manual patient registration, appointment scheduling delays, and fragmented prescription management.

─────────────────────────────────────────────
## 2. Business Problem
Clinics often face: 
- Slow patient registration and appointment scheduling.
- Poor communication between doctors, patients, and administrative staff. 
- Manual prescription tracking leading to errors.
SCMS provides a digital platform to automate and unify these processes.

─────────────────────────────────────────────
## 3. Domain-Driven Design (DDD) Structure
**Core Domain:** Healthcare (patient management, appointments, prescriptions)

**Bounded Contexts:**  
1. Patient Management Context: Handles patient registration, medical history, and profile updates. 
2. Appointment Context: Manages booking, scheduling, and reminders. 
3. Doctor/Prescription Context: Manages treatment dashboards, prescriptions, and follow-ups.

**Supporting Domain:** Billing & Payment  
**Generic Domain:** Authentication & Notifications

─────────────────────────────────────────────
## 4. User Stories & Events
**Story 1: New Patient Registration**  
- Contexts: Patient Management, Appointment  
- Event: PatientRegistered  
- Effect: Appointment context auto-creates initial appointment schedule.

**Story 2: Appointment Booking**  
- Contexts: Appointment, Doctor/Prescription, Notification  
- Event: AppointmentBooked  
- Effect: Sends notifications to doctor and patient asynchronously.

**Story 3: Prescription Creation**  
- Contexts: Doctor/Prescription, Billing, Pharmacy  
- Event: PrescriptionCreated  
- Effect: Updates billing and pharmacy records asynchronously.

**Eventual Consistency:** Achieved via asynchronous messaging (RabbitMQ/Kafka).

─────────────────────────────────────────────
## 5. AI-Driven Feature
**RAG-based Patient FAQ Assistant:**  
- Provides instant answers to patient queries.  
- Suggests predictive appointment reminders.  
- Offers automated treatment/prescription recommendations to doctors.

─────────────────────────────────────────────
## 6. Technical Stack & Considerations
- Frontend: Angular (doctor, patient, admin dashboards)  
- Backend: ASP.NET Core (fully separated domain layer, POCOs)  
- Microservices Ready: Modular monolith structure for future extraction (e.g., AI service, Notifications service)  
- Event-Driven Architecture: Message broker for cross-context events  
- CI/CD & Tests: Automated deployment and testing pipeline  
- Observability: Logs, metrics per context  
- Security: Keycloak SSO and RBAC
