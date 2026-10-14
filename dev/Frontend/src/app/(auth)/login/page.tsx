import type { Metadata } from "next";
import { Clock } from "lucide-react";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { LoginForm } from "@/features/auth/components/login-form";
import { getParam } from "@/lib/search-params";

export const metadata: Metadata = { title: "Iniciar sesión" };

export default async function LoginPage({ searchParams }: PageProps<"/login">) {
  const params = await searchParams;
  const next = getParam(params, "next", 500);
  const expired = getParam(params, "expired") === "1";

  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle className="text-xl">Iniciar sesión</CardTitle>
        <CardDescription>Ingresa con tu usuario y contraseña.</CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-5">
        {expired && (
          <Alert>
            <Clock aria-hidden />
            <AlertDescription>Tu sesión expiró. Inicia sesión nuevamente.</AlertDescription>
          </Alert>
        )}
        <LoginForm next={next} />
      </CardContent>
    </Card>
  );
}
