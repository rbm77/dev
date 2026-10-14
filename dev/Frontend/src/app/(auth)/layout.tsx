import { Brand } from "@/components/shared/brand";

export default function AuthLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <main className="flex min-h-dvh flex-col items-center justify-center px-4 py-10">
      <div className="flex w-full max-w-sm flex-col items-center gap-8">
        <Brand />
        {children}
        <p className="text-center text-xs text-muted-foreground">Transporte escolar · Gestión y cobros</p>
      </div>
    </main>
  );
}
