import { NavLink } from "react-router-dom";
import {
  LayoutDashboard,
  Calendar,
  FileText,
  User,
 Stethoscope,
  Search,
  Scissors,
} from "lucide-react";

import { cn } from "@/lib/utils";
import { useLanguage } from "@/contexts/LanguageContext";

export function AppSidebar() {
  const { t } = useLanguage();

  const navItems = [
    {
      title: t.dashboard,
      url: "/doctor/dashboard",
      icon: LayoutDashboard,
    },
    {
      title: t.appointments,
      url: "/doctor/appointments",
      icon: Calendar,
    },
    {
      title: t.medicalRecords,
      url: "/doctor/medical-records",
      icon: FileText,
    },
    {
      title: t.surgeriesNav,
      url: "/doctor/surgeries",
      icon: Scissors,
    },
    {
      title: t.searchPatientNav,
      url: "/doctor/search-patient",
      icon: Search,
    },
    {
      title: t.doctorProfile,
      url: "/doctor/profile",
      icon: User,
    },
  ];

  return (
    <aside className="hidden md:flex w-64 lg:w-72 h-screen bg-sidebar flex-col shrink-0 sticky top-0 ltr:rounded-r-2xl rtl:rounded-l-2xl">
      
      {/* Logo */}
      <div className="flex items-center px-6 py-6 border-b border-sidebar-border">
        <div className="flex items-center gap-3">
          <div className="w-9 h-9 rounded-lg bg-sidebar-primary flex items-center justify-center">
            <Stethoscope className="w-5 h-5 text-sidebar-primary-foreground" />
          </div>

          <div>
            <p className="text-sm font-bold text-sidebar-foreground font-display">
              HealthCore EHRS
            </p>
          </div>
        </div>
      </div>

      {/* Navigation */}
      <nav className="flex-1 px-3 py-5 space-y-1">
        <p className="text-xs font-semibold uppercase tracking-widest text-sidebar-muted px-4 mb-3">
          {t.mainMenu}
        </p>

        {navItems.map((item) => (
          <NavLink
            key={item.url}
            to={item.url}
            className={({ isActive }) =>
              cn("sidebar-nav-item", isActive && "active")
            }
          >
            <item.icon className="w-4.5 h-4.5 shrink-0" size={18} />
            <span>{item.title}</span>
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}