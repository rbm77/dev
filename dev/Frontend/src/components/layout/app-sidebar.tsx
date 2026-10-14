"use client";

import Link from "next/link";
import { Brand } from "@/components/shared/brand";
import { visibleNavGroups } from "@/lib/navigation";
import type { Permission } from "@/lib/permissions";
import { NavLinks } from "./nav-links";

/** Fixed navigation column for large screens (lg and up). */
export function AppSidebar({ permissions }: { permissions: Permission[] }) {
  const groups = visibleNavGroups(permissions);

  return (
    <aside className="sticky top-0 hidden h-dvh print:hidden w-64 shrink-0 flex-col border-r border-sidebar-border bg-sidebar lg:flex">
      <div className="flex h-16 items-center px-5">
        <Link href="/" aria-label="Ir al inicio" className="rounded-md">
          <Brand />
        </Link>
      </div>
      <div className="flex-1 overflow-y-auto px-3 pt-2 pb-6 [scrollbar-width:thin]">
        <NavLinks groups={groups} />
      </div>
    </aside>
  );
}
