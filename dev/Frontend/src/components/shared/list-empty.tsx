import { SearchX, type LucideIcon } from "lucide-react";
import { EmptyState } from "./empty-state";

type ListEmptyProps = {
  /** True when the empty result comes from active filters. */
  filtered: boolean;
  icon: LucideIcon;
  /** Plural noun with article, e.g. "vehículos registrados". */
  noun: string;
  action?: React.ReactNode;
};

export function ListEmpty({ filtered, icon, noun, action }: ListEmptyProps) {
  if (filtered) {
    return (
      <EmptyState
        icon={SearchX}
        title="Sin resultados"
        description="No hay registros que coincidan con los filtros. Prueba con otros criterios."
      />
    );
  }
  return <EmptyState icon={icon} title={`Aún no hay ${noun}`} action={action} />;
}
