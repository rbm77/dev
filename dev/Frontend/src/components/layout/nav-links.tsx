"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { HOME_ITEM, isActiveItem, isActivePath, type NavGroup } from "@/lib/navigation";
import { cn } from "@/lib/utils";

type NavLinksProps = {
  groups: readonly NavGroup[];
  /** Called after a link is chosen (closes the mobile menu). */
  onNavigate?: () => void;
};

const linkClass =
  "flex min-h-11 items-center gap-3 rounded-md px-3 text-sm font-medium text-sidebar-foreground/80 transition-colors hover:bg-sidebar-accent hover:text-sidebar-accent-foreground lg:min-h-9";
const activeClass = "bg-sidebar-accent text-sidebar-accent-foreground";

/** Grouped navigation shared by the desktop sidebar and the mobile menu. */
export function NavLinks({ groups, onNavigate }: NavLinksProps) {
  const pathname = usePathname();
  const homeActive = isActivePath(pathname, HOME_ITEM.href);

  return (
    <nav aria-label="Navegación principal" className="flex flex-col gap-5">
      <Link
        href={HOME_ITEM.href}
        onClick={onNavigate}
        aria-current={homeActive ? "page" : undefined}
        className={cn(linkClass, homeActive && activeClass)}
      >
        <HOME_ITEM.icon className="size-4 shrink-0" aria-hidden />
        {HOME_ITEM.label}
      </Link>

      {groups.map((group) => (
        <div key={group.id} className="flex flex-col gap-1">
          <p className="px-3 pb-1 text-xs font-semibold tracking-wide text-muted-foreground uppercase">
            {group.label}
          </p>
          {group.items.map((item) => {
            const active = isActiveItem(pathname, item);
            return (
              <Link
                key={item.href}
                href={item.href}
                onClick={onNavigate}
                aria-current={active ? "page" : undefined}
                className={cn(linkClass, active && activeClass)}
              >
                <item.icon className="size-4 shrink-0" aria-hidden />
                {item.label}
              </Link>
            );
          })}
        </div>
      ))}
    </nav>
  );
}
