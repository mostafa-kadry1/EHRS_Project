import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "@/contexts/AuthContext";

interface ProtectedRouteProps {
  /** If provided, only users with this role can access the route. */
  role?: "Doctor" | "Patient";
}

/**
 * Wraps a group of routes. Redirects to /login when not authenticated.
 * Optionally also checks the user's role and redirects to /login on mismatch.
 */
export function ProtectedRoute({ role }: ProtectedRouteProps) {
  const { isAuthenticated, isLoading, user } = useAuth();

  // While restoring session from localStorage, don't flash a redirect.
  if (isLoading) {
    return (
      <div className="flex h-screen items-center justify-center">
        <div className="w-8 h-8 border-4 border-primary/30 border-t-primary rounded-full animate-spin" />
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (role && user?.role !== role) {
    // Wrong role — send them back to login instead of showing a blank page.
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
}
