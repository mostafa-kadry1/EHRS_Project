import { NavLink, useNavigate } from "react-router-dom";
import { LayoutDashboard, Calendar, FileText, User, Stethoscope, Search, Scissors, X, LogOut } from "lucide-react";
import { cn } from "@/lib/utils";
import { useLanguage } from "@/contexts/LanguageContext";
import { useAuth } from "@/contexts/AuthContext";
import { Sheet, SheetContent } from "@/components/ui/sheet";

interface MobileSidebarDrawerProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function MobileSidebarDrawer({ open, onOpenChange }: MobileSidebarDrawerProps) {
  const { t } = useLanguage();
  const navigate = useNavigate();
  const { logout } = useAuth();

  const navItems = [
    { title: t.dashboard, url: "/doctor/dashboard", icon: LayoutDashboard },
    { title: t.appointments, url: "/doctor/appointments", icon: Calendar },
    { title: t.medicalRecords, url: "/doctor/medical-records", icon: FileText },
    { title: t.surgeriesNav, url: "/doctor/surgeries", icon: Scissors },
    { title: t.searchPatientNav, url: "/doctor/search-patient", icon: Search },
    { title: t.doctorProfile, url: "/doctor/profile", icon: User },
  ];

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent side="left" className="w-72 p-0 bg-sidebar border-none [&>button]:hidden">
        {/* Logo */}
        <div className="flex items-center justify-between px-6 py-6 border-b border-sidebar-border">
          <div className="flex items-center gap-3">
            <div className="w-9 h-9 rounded-lg bg-sidebar-primary flex items-center justify-center">
              <Stethoscope className="w-5 h-5 text-sidebar-primary-foreground" />
            </div>
            <div>
              <p className="text-sm font-bold text-sidebar-foreground font-display">HealthCore EHRS</p>
            </div>
          </div>
          <button
            onClick={() => onOpenChange(false)}
            className="w-8 h-8 flex items-center justify-center rounded-lg hover:bg-sidebar-accent transition-colors"
          >
            <X className="w-5 h-5 text-sidebar-foreground" />
          </button>
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
              end={item.url === "/"}
              onClick={() => onOpenChange(false)}
              className={({ isActive }) =>
                cn("sidebar-nav-item", isActive && "active")
              }
            >
              <item.icon className="w-4.5 h-4.5 shrink-0" size={18} />
              <span>{item.title}</span>
            </NavLink>
          ))}

          <button
            onClick={() => { onOpenChange(false); logout(); }}
            className="sidebar-nav-item w-full text-left mt-4"
          >
            <LogOut className="w-4.5 h-4.5 shrink-0" size={18} />
            <span>{t.logout ?? "Logout"}</span>
          </button>
        </nav>
      </SheetContent>
    </Sheet>
  );
}
