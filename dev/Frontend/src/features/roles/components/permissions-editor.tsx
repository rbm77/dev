"use client";

import { useMemo, useState, useTransition } from "react";
import { Info, Loader2 } from "lucide-react";
import { toast } from "sonner";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { ToggleGroup, ToggleGroupItem } from "@/components/ui/toggle-group";
import type { RolePermission } from "@/features/catalogs/types";
import { availableModes, RESOURCE_LABELS, RESOURCES, type PermissionMode, type Resource } from "@/lib/permissions";
import { saveRolePermissions } from "../actions";

type Level = "NONE" | PermissionMode;

const LEVEL_LABELS: Record<Level, string> = { NONE: "Sin acceso", READ: "Lectura", WRITE: "Escritura" };

function toResource(description: string | undefined): Resource | null {
  return RESOURCES.find((resource) => resource === description) ?? null;
}

function initialLevels(permissions: readonly RolePermission[]): Record<number, Level> {
  return Object.fromEntries(
    permissions.map((permission) => [
      permission.resourceId,
      permission.isAssigned ? (permission.isEditable ? "WRITE" : "READ") : "NONE",
    ]),
  );
}

type PermissionsEditorProps = {
  roleId: number;
  permissions: RolePermission[];
  readOnly: boolean;
};

export function PermissionsEditor({ roleId, permissions, readOnly }: PermissionsEditorProps) {
  const [saved, setSaved] = useState(() => initialLevels(permissions));
  const [levels, setLevels] = useState(saved);
  const [pending, startTransition] = useTransition();

  const dirty = useMemo(() => permissions.some((p) => levels[p.resourceId] !== saved[p.resourceId]), [permissions, levels, saved]);

  const save = () => {
    const payload = permissions
      .filter((permission) => levels[permission.resourceId] !== "NONE")
      .map((permission) => ({ resourceId: permission.resourceId, isEditable: levels[permission.resourceId] === "WRITE" }));

    startTransition(async () => {
      const result = await saveRolePermissions(roleId, payload);
      if (result.ok) {
        setSaved(levels);
        toast.success("Permisos guardados. Se aplican cuando los usuarios vuelvan a iniciar sesión.");
      } else {
        toast.error(result.error);
      }
    });
  };

  return (
    <div className="flex flex-col gap-4">
      <Alert>
        <Info aria-hidden />
        <AlertDescription>
          «Escritura» incluye «Lectura». Los cambios se aplican cuando los usuarios de este rol vuelven a iniciar
          sesión.
        </AlertDescription>
      </Alert>

      <Card className="gap-0 py-0">
        <ul className="divide-y">
          {permissions.map((permission) => {
            const resource = toResource(permission.description);
            const label = resource ? RESOURCE_LABELS[resource] : (permission.description ?? `Recurso ${permission.resourceId}`);
            const options: Level[] = ["NONE", ...(resource ? availableModes(resource) : (["READ", "WRITE"] as const))];
            const labelId = `resource-${permission.resourceId}`;

            return (
              <li
                key={permission.resourceId}
                className="flex flex-col gap-2 px-4 py-3 sm:flex-row sm:items-center sm:justify-between"
              >
                <span id={labelId} className="text-sm font-medium">
                  {label}
                </span>
                <ToggleGroup
                  type="single"
                  variant="outline"
                  spacing={0}
                  value={levels[permission.resourceId]}
                  onValueChange={(value) =>
                    value && setLevels((current) => ({ ...current, [permission.resourceId]: value as Level }))
                  }
                  disabled={readOnly || pending}
                  aria-labelledby={labelId}
                  className="w-full sm:w-auto"
                >
                  {options.map((level) => (
                    <ToggleGroupItem
                      key={level}
                      value={level}
                      className="h-10 flex-1 px-3 data-[state=on]:bg-primary data-[state=on]:text-primary-foreground sm:h-8 sm:flex-none"
                    >
                      {LEVEL_LABELS[level]}
                    </ToggleGroupItem>
                  ))}
                </ToggleGroup>
              </li>
            );
          })}
        </ul>
      </Card>

      {!readOnly && (
        <div className="sticky bottom-[calc(4rem+env(safe-area-inset-bottom))] z-20 -mx-4 flex flex-col-reverse gap-2 border-t bg-background/95 px-4 py-3 backdrop-blur sm:flex-row sm:justify-end lg:static lg:mx-0 lg:border-0 lg:bg-transparent lg:p-0">
          <Button variant="outline" disabled={!dirty || pending} onClick={() => setLevels(saved)}>
            Descartar cambios
          </Button>
          <Button disabled={!dirty || pending} onClick={save}>
            {pending && <Loader2 className="animate-spin" aria-hidden />}
            {pending ? "Guardando…" : "Guardar permisos"}
          </Button>
        </div>
      )}
    </div>
  );
}
