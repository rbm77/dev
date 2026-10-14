"use client";

import { DialogContent } from "@/components/ui/dialog";
import { cn } from "@/lib/utils";

/**
 * Dialog content that is a centered modal on larger screens and a bottom sheet
 * on phones. Use it inside <Dialog> with the regular DialogHeader/DialogFooter.
 */
export function ResponsiveDialogContent({ className, ...props }: React.ComponentProps<typeof DialogContent>) {
  return (
    <DialogContent
      className={cn(
        "max-h-[90dvh] overflow-y-auto",
        "max-sm:top-auto max-sm:bottom-0 max-sm:left-0 max-sm:max-w-none max-sm:translate-x-0 max-sm:translate-y-0",
        "max-sm:rounded-b-none max-sm:pb-[calc(1.5rem+env(safe-area-inset-bottom))] max-sm:data-open:slide-in-from-bottom-10",
        className,
      )}
      {...props}
    />
  );
}
