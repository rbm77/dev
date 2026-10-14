// Common/Shared/Enums.cs — the API sends and receives the numeric values.

export const MAINTENANCE_TYPES = [
  { value: "1", label: "Preventivo" },
  { value: "2", label: "Correctivo" },
  { value: "3", label: "Inspección técnica" },
] as const;

export const INCIDENT_TYPES = [
  { value: "1", label: "Accidente" },
  { value: "2", label: "Avería" },
  { value: "3", label: "Retraso" },
  { value: "4", label: "Infracción de tránsito" },
  { value: "5", label: "Evento de seguridad" },
] as const;

/** "Estado" filter; the adjective agrees with "estado" for every module. */
export const ACTIVE_FILTER_OPTIONS = [
  { value: "true", label: "Activo" },
  { value: "false", label: "Inactivo" },
] as const;

export function maintenanceTypeLabel(type: number): string {
  return MAINTENANCE_TYPES.find((option) => option.value === String(type))?.label ?? "—";
}

export function incidentTypeLabel(type: number): string {
  return INCIDENT_TYPES.find((option) => option.value === String(type))?.label ?? "—";
}
