import { Outlet } from "react-router-dom";
import { AppLayout } from "@/components/layout/AppLayout";

const DoctorLayout = () => {
  return (
    <AppLayout showHeader>
      <Outlet />
    </AppLayout>
  );
};

export default DoctorLayout;