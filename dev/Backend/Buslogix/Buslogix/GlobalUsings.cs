// Global usings for this project's own namespaces.
//
// Reorganizing from technical layers (Controllers/Services/Repositories/Interfaces/Models)
// into Feature Folders turned a handful of shared namespaces into ~30 small, per-feature
// ones. Rather than scatter dozens of narrow `using Buslogix.Features.X;` lines across every
// file that happens to reference another feature's repository/model (e.g. EmailIngestion
// reading IEmailAccountRepository, or a controller referencing a shared DTO), they're made
// globally visible here once.
//
// EmailIngestion, MessageExtraction and Matching are feature folders too (they just have
// more internal structure - Abstractions/, Persistence/, etc. - than a plain CRUD entity),
// but their sub-namespaces (e.g. Buslogix.Features.MessageExtraction.Llm) aren't globalized
// here: only files that already need that specific piece reference it, via an explicit
// `using`, same as any third-party dependency. Buslogix.Common.BackgroundJobs (the generic
// TriggerQueue/TriggerWorker engine) follows the same rule - explicit `using` where used.

global using Buslogix.Common.Auth;
global using Buslogix.Common.Data;
global using Buslogix.Common.Logging;
global using Buslogix.Common.Middlewares;
global using Buslogix.Common.Shared;

global using Buslogix.Features.Companies;
global using Buslogix.Features.Contacts;
global using Buslogix.Features.Roles;
global using Buslogix.Features.Employees;
global using Buslogix.Features.Salaries;
global using Buslogix.Features.SalaryExpenses;
global using Buslogix.Features.Vehicles;
global using Buslogix.Features.Routes;
global using Buslogix.Features.Grades;
global using Buslogix.Features.Vacations;
global using Buslogix.Features.Expenses;
global using Buslogix.Features.FuelExpenses;
global using Buslogix.Features.MaintenanceExpenses;
global using Buslogix.Features.IncidentExpenses;
global using Buslogix.Features.Incidents;
global using Buslogix.Features.Maintenances;
global using Buslogix.Features.CustomTransports;
global using Buslogix.Features.PeriodicExemptions;
global using Buslogix.Features.SpecificExemptions;
global using Buslogix.Features.PaymentPeriods;
global using Buslogix.Features.PaymentPeriodRequests;
global using Buslogix.Features.Payments;
global using Buslogix.Features.Reports;
global using Buslogix.Features.EmailAccounts;
global using Buslogix.Features.EmailSenders;
global using Buslogix.Features.Users;
global using Buslogix.Features.Students;
global using Buslogix.Features.PaymentRequests;
