// Mirrors Backend/Common/Auth/PermissionMap.cs and PermissionHandler.cs.
// Permissions only drive what the UI shows; the API authorizes every request.

export const RESOURCES = [
  "COMPANY",
  "ROLE",
  "USER",
  "OWN_USER",
  "EMPLOYEE",
  "SALARY",
  "VEHICLE",
  "ROUTE",
  "GRADE",
  "STUDENT",
  "CONTACT",
  "MAINTENANCE",
  "INCIDENT",
  "CUSTOM_TRANSPORT",
  "EXPENSE",
  "PAYMENT_PERIOD",
  "VACATION",
  "SPECIFIC_EXEMPTION",
  "PERIODIC_EXEMPTION",
  "PAYMENT",
  "DEBTOR",
  "PAYMENT_REQUEST",
  "PAYMENT_APPROVAL",
  "EMAIL_ACCOUNT",
  "EMAIL_SENDER",
  "WHATSAPP_ACCOUNT",
] as const;

export type Resource = (typeof RESOURCES)[number];
export type PermissionMode = "READ" | "WRITE";
export type Permission = `${Resource}.${PermissionMode}`;

const CODE_TO_PERMISSION: Readonly<Record<string, Permission>> = {
  "1": "COMPANY.WRITE",
  "2": "COMPANY.READ",
  "3": "ROLE.WRITE",
  "4": "ROLE.READ",
  "5": "USER.WRITE",
  "6": "USER.READ",
  "7": "OWN_USER.WRITE",
  "8": "OWN_USER.READ",
  "9": "EMPLOYEE.WRITE",
  "10": "EMPLOYEE.READ",
  "11": "SALARY.WRITE",
  "12": "SALARY.READ",
  "13": "VEHICLE.WRITE",
  "14": "VEHICLE.READ",
  "15": "ROUTE.WRITE",
  "16": "ROUTE.READ",
  "17": "GRADE.WRITE",
  "18": "GRADE.READ",
  "19": "STUDENT.WRITE",
  "20": "STUDENT.READ",
  "21": "CONTACT.WRITE",
  "22": "CONTACT.READ",
  "23": "MAINTENANCE.WRITE",
  "24": "MAINTENANCE.READ",
  "25": "INCIDENT.WRITE",
  "26": "INCIDENT.READ",
  "27": "CUSTOM_TRANSPORT.WRITE",
  "28": "CUSTOM_TRANSPORT.READ",
  "29": "EXPENSE.WRITE",
  "30": "EXPENSE.READ",
  "31": "PAYMENT_PERIOD.WRITE",
  "32": "PAYMENT_PERIOD.READ",
  "33": "VACATION.WRITE",
  "34": "VACATION.READ",
  "35": "SPECIFIC_EXEMPTION.WRITE",
  "36": "SPECIFIC_EXEMPTION.READ",
  "37": "PERIODIC_EXEMPTION.WRITE",
  "38": "PERIODIC_EXEMPTION.READ",
  "39": "PAYMENT.WRITE",
  "40": "PAYMENT.READ",
  "41": "DEBTOR.READ",
  "43": "PAYMENT_REQUEST.READ",
  "44": "PAYMENT_APPROVAL.WRITE",
  "45": "EMAIL_ACCOUNT.WRITE",
  "46": "EMAIL_ACCOUNT.READ",
  "47": "EMAIL_SENDER.WRITE",
  "48": "EMAIL_SENDER.READ",
  "49": "WHATSAPP_ACCOUNT.WRITE",
  "50": "WHATSAPP_ACCOUNT.READ",
};

const KNOWN_PERMISSIONS: readonly Permission[] = Object.values(CODE_TO_PERMISSION);

/** Modes that exist in the backend map for a resource (some are read-only or write-only). */
export function availableModes(resource: Resource): PermissionMode[] {
  return (["READ", "WRITE"] as const).filter((mode) => KNOWN_PERMISSIONS.includes(`${resource}.${mode}`));
}

export const RESOURCE_LABELS: Readonly<Record<Resource, string>> = {
  COMPANY: "Empresa",
  ROLE: "Roles",
  USER: "Usuarios",
  OWN_USER: "Mi perfil",
  EMPLOYEE: "Empleados",
  SALARY: "Salarios",
  VEHICLE: "Vehículos",
  ROUTE: "Rutas",
  GRADE: "Grados",
  STUDENT: "Estudiantes",
  CONTACT: "Contactos",
  MAINTENANCE: "Mantenimientos",
  INCIDENT: "Incidentes",
  CUSTOM_TRANSPORT: "Transportes especiales",
  EXPENSE: "Gastos",
  PAYMENT_PERIOD: "Periodos de cobro",
  VACATION: "Vacaciones",
  SPECIFIC_EXEMPTION: "Exenciones específicas",
  PERIODIC_EXEMPTION: "Exenciones periódicas",
  PAYMENT: "Pagos",
  DEBTOR: "Deudores",
  PAYMENT_REQUEST: "Solicitudes de pago",
  PAYMENT_APPROVAL: "Aprobación de pagos",
  EMAIL_ACCOUNT: "Cuentas de correo",
  EMAIL_SENDER: "Remitentes de correo",
  WHATSAPP_ACCOUNT: "Cuentas de WhatsApp",
};

/** Converts the comma-separated `permissions` claim of the JWT into permission names. */
export function parsePermissionCodes(claim: string): Permission[] {
  return claim
    .split(",")
    .map((code) => CODE_TO_PERMISSION[code.trim()])
    .filter((permission): permission is Permission => permission !== undefined);
}

/** Same rule as PermissionHandler: WRITE on a resource also grants READ. */
export function can(
  permissions: readonly Permission[],
  resource: Resource,
  mode: PermissionMode,
): boolean {
  if (permissions.includes(`${resource}.WRITE`)) {
    return true;
  }
  return mode === "READ" && permissions.includes(`${resource}.READ`);
}

export function canAny(
  permissions: readonly Permission[],
  checks: ReadonlyArray<readonly [Resource, PermissionMode]>,
): boolean {
  return checks.some(([resource, mode]) => can(permissions, resource, mode));
}
