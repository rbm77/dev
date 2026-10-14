import Link from "next/link";
import { ChevronLeft } from "lucide-react";

type PageHeaderProps = {
  title: string;
  description?: string;
  /** Shows a back link above the title (detail and form pages). */
  backHref?: string;
  backLabel?: string;
  actions?: React.ReactNode;
};

export function PageHeader({ title, description, backHref, backLabel = "Volver", actions }: PageHeaderProps) {
  return (
    <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between lg:mb-6">
      <div className="min-w-0">
        {backHref && (
          <Link
            href={backHref}
            className="-ml-1 mb-1 inline-flex min-h-11 items-center gap-1 rounded-md px-1 text-sm text-muted-foreground hover:text-foreground md:min-h-8"
          >
            <ChevronLeft className="size-4" aria-hidden />
            {backLabel}
          </Link>
        )}
        <h1 className="text-xl font-semibold tracking-tight text-balance lg:text-2xl">{title}</h1>
        {description && <p className="mt-1 text-sm text-muted-foreground">{description}</p>}
      </div>
      {actions && <div className="flex flex-wrap items-center gap-2">{actions}</div>}
    </div>
  );
}
