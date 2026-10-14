import { Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";

export function SubmitButton({ pending, label, className }: { pending: boolean; label: string; className?: string }) {
  return (
    <Button type="submit" disabled={pending} className={className}>
      {pending && <Loader2 className="animate-spin" aria-hidden />}
      {pending ? "Guardando…" : label}
    </Button>
  );
}
