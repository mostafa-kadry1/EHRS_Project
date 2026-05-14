import { NavLink } from "react-router-dom";
import { LayoutDashboard, Calendar, FileText, User, Search, Scissors } from "lucide-react";
import { cn } from "@/lib/utils";
import { useLanguage } from "@/contexts/LanguageContext";

export function MobileBottomNav() {
  const { t } = useLanguage();

  const navItems = [
    { title: t.dashboard, url: "/doctor/dashboard", icon: LayoutDashboard },
    { title: t.appointments, url: "/doctor/appointments", icon: Calendar },
    { title: t.medicalRecords, url: "/doctor/medical-records", icon: FileText },
    { title: t.surgeriesNav, url: "/doctor/surgeries", icon: Scissors },
    { title: t.searchPatientNav, url: "/doctor/search-patient", icon: Search },
    { title: t.doctorProfile, url: "/doctor/profile", icon: User },
  ];

  return (
    <nav className="md:hidden fixed bottom-0 left-0 right-0 z-50 bg-card border-t border-border safe-area-bottom">
      <div className="flex items-center justify-around px-2 py-1">
        {navItems.map((item) => (
          <NavLink
            key={item.url}
            to={item.url}
            end={item.url === "/"}
            className={({ isActive }) =>
              cn(
                "flex flex-col items-center gap-0.5 px-3 py-2 rounded-lg text-[10px] font-medium transition-colors min-w-[60px]",
                isActive
                  ? "text-primary"
                  : "text-muted-foreground"
              )
            }
          >
            <item.icon className="w-5 h-5" />
            <span className="truncate">{item.title}</span>
          </NavLink>
        ))}
      </div>
    </nav>
  );
}
