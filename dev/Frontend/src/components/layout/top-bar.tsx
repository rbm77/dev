import Link from "next/link";
import { Brand } from "@/components/shared/brand";
import { can, type Permission } from "@/lib/permissions";
import { UserMenu } from "./user-menu";

type TopBarProps = {
  username: string;
  permissions: Permission[];
};

export function TopBar({ username, permissions }: TopBarProps) {
  return (
    <header className="sticky top-0 z-30 print:hidden border-b bg-card/95 pt-[env(safe-area-inset-top)] backdrop-blur supports-backdrop-filter:bg-card/80">
      <div className="flex h-14 items-center justify-between gap-3 px-4 lg:h-16 lg:justify-end lg:px-8">
        <Link href="/" aria-label="Ir al inicio" className="rounded-md lg:hidden">
          <Brand />
        </Link>
        <UserMenu username={username} canViewProfile={can(permissions, "OWN_USER", "READ")} />
      </div>
    </header>
  );
}
