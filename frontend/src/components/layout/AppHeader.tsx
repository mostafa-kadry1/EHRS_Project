import { Globe, LogOut, Menu } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useProfile } from "@/contexts/ProfileContext";
import { useLanguage } from "@/contexts/LanguageContext";
import { useAuth } from "@/contexts/AuthContext";

interface AppHeaderProps {
  onMenuToggle?: () => void;
}

export function AppHeader({ onMenuToggle }: AppHeaderProps) {
  const { profileImage } = useProfile();
  const { t, toggleLang, lang } = useLanguage();
  const navigate = useNavigate();
  const { logout } = useAuth();

  return (
    <header className="h-14 md:h-16 bg-card border-b border-border flex items-center px-4 md:px-6 gap-3 md:gap-4 shrink-0">
      {/* Hamburger for mobile */}
      <button
        onClick={onMenuToggle}
        className="md:hidden w-9 h-9 flex items-center justify-center rounded-lg hover:bg-muted transition-colors"
      >
        <Menu className="w-5 h-5 text-foreground" />
      </button>

      {/* Mobile: show doctor name + image next to hamburger */}
      <div className="md:hidden flex items-center gap-2">
        <img
          src={profileImage}
          alt={t.drName}
          className="w-8 h-8 rounded-full object-cover ring-2 ring-primary/20 cursor-pointer"
          onClick={() => navigate("/doctor/profile")}
        />
        <p className="text-sm font-semibold text-foreground">{t.drName}</p>
      </div>

      <div className="flex-1" />

      <div className="flex items-center gap-2 md:gap-3 ml-auto rtl:ml-0 rtl:mr-auto">
        {/* Language toggle */}
        <button
          onClick={toggleLang}
          className="flex items-center gap-2 text-sm font-medium text-muted-foreground hover:text-foreground px-3 py-2 rounded-lg hover:bg-muted transition-colors"
        >
          <Globe size={16} />
          {lang === "en" ? "عربي" : "English"}
        </button>

        {/* Logout */}
        <button
          className="flex items-center gap-2 text-sm font-medium text-muted-foreground hover:text-foreground px-3 py-2 rounded-lg hover:bg-muted transition-colors"
          onClick={logout}
        >
          <LogOut size={16} />
          {lang === "ar" ? "تسجيل الخروج" : "Logout"}
        </button>

        {/* Desktop profile */}
        <div className="hidden md:flex items-center gap-2 md:gap-3 pl-2 md:pl-3 border-l border-border rtl:pl-0 rtl:pr-2 rtl:md:pr-3 rtl:border-l-0 rtl:border-r">
          <div className="text-right rtl:text-left">
            <p className="text-sm font-semibold text-foreground leading-none">
              {t.drName}
            </p>
            <p className="text-xs text-muted-foreground mt-0.5">
              {t.cardiologist}
            </p>
          </div>

          <img
            src={profileImage}
            alt={t.drName}
            className="w-9 h-9 rounded-full object-cover ring-2 ring-primary/20 cursor-pointer"
            onClick={() => navigate("/doctor/profile")}
          />
        </div>
      </div>
    </header>
  );
}