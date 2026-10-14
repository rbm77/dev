"use client";

import { useState } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { Menu } from "lucide-react";
import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle } from "@/components/ui/sheet";
import { HOME_ITEM, isActiveItem, isActivePath, visibleNavGroups } from "@/lib/navigation";
import type { Permission } from "@/lib/permissions";
import { cn } from "@/lib/utils";
import { NavLinks } from "./nav-links";

const MAX_GROUP_TABS = 3;

const tabClass =
  "flex min-h-16 flex-1 flex-col items-center justify-center gap-1 px-1 text-[0.6875rem] font-medium text-muted-foreground transition-colors";

/** Mobile/tablet bottom navigation: Inicio, the first sections and a full menu. */
export function BottomNav({ permissions }: { permissions: Permission[] }) {
  const pathname = usePathname();
  const [menuOpen, setMenuOpen] = useState(false);
  const groups = visibleNavGroups(permissions);
  const groupTabs = groups.slice(0, MAX_GROUP_TABS);
  const homeActive = isActivePath(pathname, HOME_ITEM.href);

  return (
    <>
      <nav
        aria-label="Navegación inferior"
        className="fixed inset-x-0 bottom-0 z-40 border-t bg-card print:hidden pb-[env(safe-area-inset-bottom)] lg:hidden"
      >
        <div className="mx-auto flex max-w-xl">
          <Link
            href={HOME_ITEM.href}
            aria-current={homeActive ? "page" : undefined}
            className={cn(tabClass, homeActive && "text-primary")}
          >
            <HOME_ITEM.icon className="size-5" aria-hidden />
            {HOME_ITEM.label}
          </Link>

          {groupTabs.map((group) => {
            const active = group.items.some((item) => isActiveItem(pathname, item));
            return (
              <Link
                key={group.id}
                href={group.items[0].href}
                aria-current={active ? "page" : undefined}
                className={cn(tabClass, active && "text-primary")}
              >
                <group.icon className="size-5" aria-hidden />
                {group.label}
              </Link>
            );
          })}

          <button
            type="button"
            onClick={() => setMenuOpen(true)}
            aria-haspopup="dialog"
            aria-expanded={menuOpen}
            className={cn(tabClass, menuOpen && "text-primary")}
          >
            <Menu className="size-5" aria-hidden />
            Menú
          </button>
        </div>
      </nav>

      <Sheet open={menuOpen} onOpenChange={setMenuOpen}>
        <SheetContent side="bottom" className="max-h-[85dvh] rounded-t-2xl pb-[env(safe-area-inset-bottom)]">
          <SheetHeader className="pb-0">
            <SheetTitle>Menú</SheetTitle>
            <SheetDescription className="sr-only">Todas las secciones disponibles</SheetDescription>
          </SheetHeader>
          <div className="overflow-y-auto px-2 pb-4">
            <NavLinks groups={groups} onNavigate={() => setMenuOpen(false)} />
          </div>
        </SheetContent>
      </Sheet>
    </>
  );
}
