import {
  Banknote,
  BusFront,
  CalendarDays,
  CalendarOff,
  CarFront,
  ClipboardCheck,
  Building2,
  GraduationCap,
  HandCoins,
  House,
  Inbox,
  KeyRound,
  MapIcon,
  Percent,
  Plug,
  Receipt,
  Route,
  ShieldCheck,
  TriangleAlert,
  UserCog,
  Users,
  UsersRound,
  Wallet,
  Wrench,
  type LucideIcon,
} from "lucide-react";
import { canAny, type Permission, type PermissionMode, type Resource } from "@/lib/permissions";

export type NavItem = {
  href: string;
  label: string;
  icon: LucideIcon;
  /** The item is shown when the user has ANY of these permissions. */
  requires: ReadonlyArray<readonly [Resource, PermissionMode]>;
};

export type NavGroup = {
  id: "billing" | "students" | "fleet" | "staff" | "expenses" | "admin";
  label: string;
  icon: LucideIcon;
  items: readonly NavItem[];
};

export const HOME_ITEM = { href: "/", label: "Inicio", icon: House } as const;

export const NAV_GROUPS: readonly NavGroup[] = [
  {
    id: "billing",
    label: "Cobros",
    icon: Wallet,
    items: [
      { href: "/payment-requests", label: "Solicitudes de pago", icon: Inbox, requires: [["PAYMENT_REQUEST", "READ"]] },
      { href: "/payments", label: "Pagos", icon: Banknote, requires: [["PAYMENT", "READ"]] },
      { href: "/debtors", label: "Deudores", icon: HandCoins, requires: [["DEBTOR", "READ"]] },
      { href: "/payment-periods", label: "Periodos de cobro", icon: CalendarDays, requires: [["PAYMENT_PERIOD", "READ"]] },
      { href: "/exemptions/specific", label: "Exenciones específicas", icon: Percent, requires: [["SPECIFIC_EXEMPTION", "READ"]] },
      { href: "/exemptions/periodic", label: "Exenciones periódicas", icon: Percent, requires: [["PERIODIC_EXEMPTION", "READ"]] },
      { href: "/vacations", label: "Vacaciones", icon: CalendarOff, requires: [["VACATION", "READ"]] },
    ],
  },
  {
    id: "students",
    label: "Estudiantes",
    icon: GraduationCap,
    items: [
      { href: "/students", label: "Estudiantes", icon: GraduationCap, requires: [["STUDENT", "READ"]] },
      { href: "/routes", label: "Rutas", icon: MapIcon, requires: [["ROUTE", "READ"]] },
      { href: "/grades", label: "Grados", icon: ClipboardCheck, requires: [["GRADE", "READ"]] },
    ],
  },
  {
    id: "fleet",
    label: "Flota",
    icon: BusFront,
    items: [
      { href: "/vehicles", label: "Vehículos", icon: CarFront, requires: [["VEHICLE", "READ"]] },
      { href: "/maintenances", label: "Mantenimientos", icon: Wrench, requires: [["MAINTENANCE", "READ"]] },
      { href: "/incidents", label: "Incidentes", icon: TriangleAlert, requires: [["INCIDENT", "READ"]] },
      { href: "/custom-transports", label: "Transportes especiales", icon: Route, requires: [["CUSTOM_TRANSPORT", "READ"]] },
    ],
  },
  {
    id: "staff",
    label: "Personal",
    icon: UsersRound,
    items: [{ href: "/employees", label: "Empleados", icon: UsersRound, requires: [["EMPLOYEE", "READ"]] }],
  },
  {
    id: "expenses",
    label: "Gastos",
    icon: Receipt,
    items: [{ href: "/expenses/general", label: "Gastos", icon: Receipt, requires: [["EXPENSE", "READ"]] }],
  },
  {
    id: "admin",
    label: "Administración",
    icon: Building2,
    items: [
      { href: "/settings/company", label: "Empresa", icon: Building2, requires: [["COMPANY", "READ"]] },
      { href: "/settings/users", label: "Usuarios", icon: Users, requires: [["USER", "READ"]] },
      { href: "/settings/roles", label: "Roles y permisos", icon: KeyRound, requires: [["ROLE", "READ"]] },
      { href: "/settings/critical-users", label: "Proceso crítico", icon: ShieldCheck, requires: [["USER", "READ"]] },
      {
        href: "/settings/integrations",
        label: "Integraciones",
        icon: Plug,
        requires: [
          ["EMAIL_ACCOUNT", "READ"],
          ["EMAIL_SENDER", "READ"],
          ["WHATSAPP_ACCOUNT", "READ"],
        ],
      },
    ],
  },
];

export const PROFILE_ITEM: NavItem = {
  href: "/profile",
  label: "Mi perfil",
  icon: UserCog,
  requires: [["OWN_USER", "READ"]],
};

/** Groups with only the items the user can open; empty groups are dropped. */
export function visibleNavGroups(permissions: readonly Permission[]): NavGroup[] {
  return NAV_GROUPS.map((group) => ({
    ...group,
    items: group.items.filter((item) => canAny(permissions, item.requires)),
  })).filter((group) => group.items.length > 0);
}

export function isActivePath(pathname: string, href: string): boolean {
  return href === "/" ? pathname === "/" : pathname === href || pathname.startsWith(`${href}/`);
}

/** `/expenses/general` should also highlight `/expenses/fuel`, etc. */
export function isActiveItem(pathname: string, item: Pick<NavItem, "href">): boolean {
  const section = item.href.startsWith("/expenses/") ? "/expenses" : item.href;
  return isActivePath(pathname, section);
}
