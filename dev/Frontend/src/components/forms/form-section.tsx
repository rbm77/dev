import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { cn } from "@/lib/utils";

type FormSectionProps = {
  title: string;
  description?: string;
  children: React.ReactNode;
  /** Actions shown at the bottom of the card (forms that save independently). */
  footer?: React.ReactNode;
  /** Single column even on large screens. */
  narrow?: boolean;
  className?: string;
};

/** Card that groups related fields; two columns from md screens up. */
export function FormSection({ title, description, children, footer, narrow = false, className }: FormSectionProps) {
  return (
    <Card className={cn("gap-4", className)}>
      <CardHeader>
        <CardTitle className="text-base">{title}</CardTitle>
        {description && <CardDescription>{description}</CardDescription>}
      </CardHeader>
      <CardContent className={cn("grid gap-5", !narrow && "md:grid-cols-2")}>{children}</CardContent>
      {footer && <CardFooter className="justify-end border-t pt-4">{footer}</CardFooter>}
    </Card>
  );
}
