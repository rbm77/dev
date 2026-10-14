import type { Metadata } from "next";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { ForgotPasswordForm } from "@/features/auth/components/forgot-password-form";

export const metadata: Metadata = { title: "Restablecer contraseña" };

export default function ForgotPasswordPage() {
  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle className="text-xl">Restablecer contraseña</CardTitle>
        <CardDescription>Se generará una contraseña temporal para tu usuario.</CardDescription>
      </CardHeader>
      <CardContent>
        <ForgotPasswordForm />
      </CardContent>
    </Card>
  );
}
