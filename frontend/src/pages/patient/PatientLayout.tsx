import React, { useState } from "react";
import { NavLink, Outlet, useNavigate } from "react-router-dom";
import {
  FaHome, FaCalendarAlt, FaBook, FaFileMedical,
  FaXRay, FaPrescriptionBottleAlt, FaUser, FaTimes, FaHeartbeat,
} from "react-icons/fa";
import { Globe, LogOut } from "lucide-react";
import { useLanguage } from "@/contexts/LanguageContext";
import { useAuth } from "@/contexts/AuthContext";
import { useProfile } from "@/contexts/ProfileContext";
import "./Sidebar.css";
import "./PatientApp.css";

const PatientLayout = () => {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const { lang, toggleLang, t } = useLanguage();
  const { logout } = useAuth();
  const { patientImage } = useProfile();
  const isAr = lang === "ar";

  const navItems = [
    { to: "/patient/dashboard",           label: t.dashboard,                  icon: <FaHome /> },
    { to: "/patient/appointments",        label: t.appointments,               icon: <FaCalendarAlt /> },
    { to: "/patient/booking",             label: isAr ? "الحجز" : "Booking",   icon: <FaBook /> },
    { to: "/patient/medical-history",     label: isAr ? "السجل الطبي" : "Medical History", icon: <FaFileMedical /> },
    { to: "/patient/imaging",             label: isAr ? "التصوير والأشعة" : "Imaging & Radiology", icon: <FaXRay /> },
    { to: "/patient/prescriptions",       label: isAr ? "الوصفات الطبية" : "Prescriptions", icon: <FaPrescriptionBottleAlt /> },
    { to: "/patient/personal-information",label: isAr ? "المعلومات الشخصية" : "Personal Info", icon: <FaUser /> },
  ];

  return (
    <div className="app-layout" dir={isAr ? "rtl" : "ltr"}>
      {sidebarOpen && (
        <div className="mobile-overlay" onClick={() => setSidebarOpen(false)} />
      )}

      {/* Sidebar */}
      <div className={`sidebar ${sidebarOpen ? "mobile-open" : ""}`}>
        <div className="top">
          <div className="logo">
            <div className="logo-icon"><FaHeartbeat /></div>
            <h3>HealthCore EHRS</h3>
          </div>
          <FaTimes className="toggle-btn mobile-close" onClick={() => setSidebarOpen(false)} />
        </div>

        <ul className="menu">
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className="link"
              onClick={() => setSidebarOpen(false)}
              end={item.to === "/patient/dashboard"}
            >
              <li>{item.icon}<span>{item.label}</span></li>
            </NavLink>
          ))}
        </ul>
      </div>

      {/* Main content */}
      <div className="main-content">
        {/* Topbar */}
        <div className="global-topbar">
          <button className="hamburger-btn-inline" onClick={() => setSidebarOpen(true)}>☰</button>
          <div className="topbar-right">
            <button
              onClick={toggleLang}
              className="flex items-center gap-2 text-sm font-medium text-muted-foreground hover:text-foreground px-3 py-2 rounded-lg transition-colors"
            >
              <Globe size={16} />
              {lang === "en" ? "عربي" : "English"}
            </button>
            <button
              className="flex items-center gap-2 text-sm font-medium text-muted-foreground hover:text-foreground px-3 py-2 rounded-lg hover:bg-muted transition-colors"
              onClick={logout}
            >
              <LogOut size={16} />
              {isAr ? "تسجيل الخروج" : "Logout"}
            </button>
          </div>
        </div>

        {/* Page content */}
        <Outlet />
      </div>
    </div>
  );
};

export default PatientLayout;
