import { ReactNode, useState } from "react";
import { AppSidebar } from "./AppSidebar";
import { AppHeader } from "./AppHeader";
import { MobileSidebarDrawer } from "../navigation/MobileSidebarDrawer";

interface AppLayoutProps {
  children: ReactNode;
  showHeader?: boolean;
}

export function AppLayout({ children, showHeader = true }: AppLayoutProps) {
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  return (
    <div className="flex h-screen w-full bg-background overflow-hidden">
      <AppSidebar />
      <div className="flex flex-col flex-1 min-w-0 overflow-hidden relative">
        {showHeader && <AppHeader onMenuToggle={() => setMobileMenuOpen(true)} />}
        <main className="flex-1 p-3 sm:p-4 md:p-6 lg:p-8 overflow-auto">
          {children}
        </main>
      </div>
      <MobileSidebarDrawer open={mobileMenuOpen} onOpenChange={setMobileMenuOpen} />
    </div>
  );
}
