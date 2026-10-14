"use client";

import Link from "next/link";
import { Plus } from "lucide-react";
import { Button } from "@/components/ui/button";

type CreateButtonProps = { label: string } & ({ href: string; onClick?: never } | { onClick: () => void; href?: never });

/**
 * Primary "create" action: a regular button in the page header on larger
 * screens and a floating action button above the bottom navigation on phones.
 */
export function CreateButton({ label, href, onClick }: CreateButtonProps) {
  const fabClass =
    "fixed right-4 bottom-[calc(5rem+env(safe-area-inset-bottom))] z-30 size-14 rounded-full shadow-lg sm:hidden print:hidden";

  if (href) {
    return (
      <>
        <Button asChild className="hidden sm:inline-flex">
          <Link href={href}>
            <Plus aria-hidden />
            {label}
          </Link>
        </Button>
        <Button asChild size="icon" className={fabClass}>
          <Link href={href} aria-label={label}>
            <Plus className="size-6" aria-hidden />
          </Link>
        </Button>
      </>
    );
  }

  return (
    <>
      <Button className="hidden sm:inline-flex" onClick={onClick}>
        <Plus aria-hidden />
        {label}
      </Button>
      <Button size="icon" className={fabClass} onClick={onClick} aria-label={label}>
        <Plus className="size-6" aria-hidden />
      </Button>
    </>
  );
}
