import { AppSidebar } from "@/components/layout/app-sidebar";
import { BottomNav } from "@/components/layout/bottom-nav";
import { TopBar } from "@/components/layout/top-bar";
import { verifySession } from "@/lib/session";

export default async function AppLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  // Only the username and permissions reach client components, never the token.
  const { username, permissions } = await verifySession();

  return (
    <div className="flex min-h-dvh">
      <AppSidebar permissions={permissions} />
      <div className="flex min-w-0 flex-1 flex-col">
        <TopBar username={username} permissions={permissions} />
        <main className="mx-auto w-full max-w-6xl flex-1 px-4 pt-5 pb-[calc(5.5rem+env(safe-area-inset-bottom))] lg:px-8 lg:pt-8 lg:pb-10">
          {children}
        </main>
        <BottomNav permissions={permissions} />
      </div>
    </div>
  );
}
