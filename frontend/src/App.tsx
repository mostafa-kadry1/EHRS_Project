import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

import { ProfileProvider } from "@/contexts/ProfileContext";
import { LanguageProvider } from "@/contexts/LanguageContext";
import { AuthProvider } from "@/contexts/AuthContext";
import { ProtectedRoute } from "@/components/auth/ProtectedRoute";

import Login from "./pages/auth/Login";
import Signup from "./pages/auth/Signup";
import ForgotPassword from "./pages/auth/ForgotPassword";

import Dashboard from "./pages/doctor/Dashboard";
import Appointments from "./pages/doctor/Appointments";
import MedicalRecords from "./pages/doctor/MedicalRecords";
import NewMedicalRecord from "./pages/doctor/NewMedicalRecord";
import DoctorProfile from "./pages/doctor/DoctorProfile";
import SearchPatient from "./pages/doctor/SearchPatient";
import Surgeries from "./pages/doctor/Surgeries";

import NotFound from "./pages/NotFound";

import PatientLayout from "./pages/patient/PatientLayout";
import PatientDashboard from "./pages/patient/Dashboard";
import PatientAppointments from "./pages/patient/Appointments";
import PatientBooking from "./pages/patient/Booking";
import PatientImaging from "./pages/patient/Imaging";
import PatientMedicalHistory from "./pages/patient/MedicalHistory";
import PatientPersonalInformation from "./pages/patient/PersonalInformation";
import PatientPrescriptions from "./pages/patient/Prescriptions";

import DoctorLayout from "./pages/doctor/DoctorLayout";


const queryClient = new QueryClient();

const App = () => (
  <QueryClientProvider client={queryClient}>
    <TooltipProvider>
      <Toaster />
      <Sonner />

      <LanguageProvider>
        <ProfileProvider>

          <BrowserRouter>

            <AuthProvider>

              <Routes>

                {/* ================= AUTH ================= */}

                <Route
                  path="/"
                  element={<Navigate to="/login" replace />}
                />

                <Route
                  path="/login"
                  element={<Login />}
                />

                <Route
                  path="/signup"
                  element={<Signup />}
                />

                <Route
                  path="/forgot-password"
                  element={<ForgotPassword />}
                />



                {/* ================= DOCTOR (protected) ================= */}

                <Route element={<ProtectedRoute role="Doctor" />}>
                  <Route
                    path="/doctor"
                    element={<DoctorLayout />}
                  >

                    <Route
                      index
                      element={<Navigate to="dashboard" replace />}
                    />

                    <Route
                      path="dashboard"
                      element={<Dashboard />}
                    />

                    <Route
                      path="appointments"
                      element={<Appointments />}
                    />

                    <Route
                      path="medical-records"
                      element={<MedicalRecords />}
                    />

                    <Route
                      path="medical-records/new"
                      element={<NewMedicalRecord />}
                    />

                    <Route
                      path="search-patient"
                      element={<SearchPatient />}
                    />

                    <Route
                      path="surgeries"
                      element={<Surgeries />}
                    />

                    <Route
                      path="profile"
                      element={<DoctorProfile />}
                    />

                  </Route>
                </Route>



                {/* ================= PATIENT (protected) ================= */}

                <Route element={<ProtectedRoute role="Patient" />}>
                  <Route
                    path="/patient"
                    element={<PatientLayout />}
                  >

                    <Route
                      index
                      element={<Navigate to="dashboard" replace />}
                    />

                    <Route
                      path="dashboard"
                      element={<PatientDashboard />}
                    />

                    <Route
                      path="appointments"
                      element={<PatientAppointments />}
                    />

                    <Route
                      path="booking"
                      element={<PatientBooking />}
                    />

                    <Route
                      path="imaging"
                      element={<PatientImaging />}
                    />

                    <Route
                      path="medical-history"
                      element={<PatientMedicalHistory />}
                    />

                    <Route
                      path="personal-information"
                      element={<PatientPersonalInformation />}
                    />

                    <Route
                      path="prescriptions"
                      element={<PatientPrescriptions />}
                    />

                  </Route>
                </Route>



                {/* ================= 404 ================= */}

                <Route
                  path="*"
                  element={<NotFound />}
                />

              </Routes>

            </AuthProvider>

          </BrowserRouter>

        </ProfileProvider>
      </LanguageProvider>

    </TooltipProvider>
  </QueryClientProvider>
);

export default App;