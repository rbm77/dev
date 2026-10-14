"use client";

import { useTransition } from "react";
import Link from "next/link";
import { ChevronDown, LogOut, UserCog } from "lucide-react";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { logout } from "@/features/auth/actions";

type UserMenuProps = {
  username: string;
  canViewProfile: boolean;
};

export function UserMenu({ username, canViewProfile }: UserMenuProps) {
  const [isPending, startTransition] = useTransition();
  const initials = (username.slice(0, 2) || "?").toUpperCase();

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="ghost" className="gap-2 px-1.5" aria-label="Menú de usuario">
          <Avatar className="size-8">
            <AvatarFallback className="bg-primary/10 text-xs font-semibold text-primary">{initials}</AvatarFallback>
          </Avatar>
          <span className="hidden max-w-40 truncate text-sm font-medium sm:inline">{username}</span>
          <ChevronDown className="hidden text-muted-foreground sm:block" aria-hidden />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-60">
        <DropdownMenuLabel className="font-normal">
          <span className="block text-xs text-muted-foreground">Sesión iniciada como</span>
          <span className="block truncate font-medium">{username}</span>
        </DropdownMenuLabel>
        <DropdownMenuSeparator />
        {canViewProfile && (
          <DropdownMenuItem asChild className="py-2.5 md:py-1.5">
            <Link href="/profile">
              <UserCog aria-hidden />
              Mi perfil
            </Link>
          </DropdownMenuItem>
        )}
        <DropdownMenuItem
          variant="destructive"
          className="py-2.5 md:py-1.5"
          disabled={isPending}
          onSelect={() => startTransition(() => logout())}
        >
          <LogOut aria-hidden />
          Cerrar sesión
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
