import Link from "next/link";
import { cn } from "@/lib/utils";

export type TabLink = { value: string; label: string; href: string };

/** Tabs that are real links, so the active tab lives in the URL (shareable, back button works). */
export function TabLinks({ tabs, active, label }: { tabs: readonly TabLink[]; active: string; label: string }) {
  return (
    <nav aria-label={label} className="mb-5 -mx-4 overflow-x-auto px-4 lg:mx-0 lg:px-0">
      <ul className="inline-flex min-w-full gap-1 rounded-lg bg-muted p-1 sm:min-w-0">
        {tabs.map((tab) => {
          const isActive = tab.value === active;
          return (
            <li key={tab.value} className="flex-1 sm:flex-none">
              <Link
                href={tab.href}
                aria-current={isActive ? "page" : undefined}
                scroll={false}
                className={cn(
                  "flex min-h-10 items-center justify-center rounded-md px-3 text-sm font-medium whitespace-nowrap text-muted-foreground transition-colors hover:text-foreground md:min-h-8",
                  isActive && "bg-card text-foreground shadow-xs",
                )}
              >
                {tab.label}
              </Link>
            </li>
          );
        })}
      </ul>
    </nav>
  );
}
