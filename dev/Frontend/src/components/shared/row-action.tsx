"use client";

import type { LucideIcon } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";

type RowActionProps = {
  icon: LucideIcon;
  label: string;
  onClick: () => void;
};

/** Icon button for list rows, with a tooltip on desktop and an accessible label. */
export function RowAction({ icon: Icon, label, onClick }: RowActionProps) {
  return (
    <Tooltip>
      <TooltipTrigger asChild>
        <Button variant="ghost" size="icon" aria-label={label} onClick={onClick}>
          <Icon aria-hidden />
        </Button>
      </TooltipTrigger>
      <TooltipContent>{label}</TooltipContent>
    </Tooltip>
  );
}
