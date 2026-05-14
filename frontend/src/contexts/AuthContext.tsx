import React, { createContext, useContext, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { loginDoctor, loginPatient, registerDoctor, registerPatient, type Role } from '@/api/auth.api';
import { toast } from 'sonner';

interface User {
  userId: number;
  fullName: string;
  email: string;
  role: 'Patient' | 'Doctor';
}

interface AuthContextType {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (data: any, role: 'Patient' | 'Doctor') => Promise<void>;
  signup: (data: any, role: 'Patient' | 'Doctor') => Promise<void>;
  logout: () => void;
  isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const savedToken = localStorage.getItem('token');
    const savedUser = localStorage.getItem('user');

    if (savedToken && savedUser) {
      setToken(savedToken);
      setUser(JSON.parse(savedUser));
    }

    setIsLoading(false);
  }, []);

  const login = async (data: any, role: Role) => {
    try {
      const response = role === 'Patient'
        ? await loginPatient(data)
        : await loginDoctor(data);

      const { accessToken, user: userData } = response;

      localStorage.setItem('token', accessToken);
      localStorage.setItem('user', JSON.stringify(userData));

      setToken(accessToken);
      setUser(userData);

      toast.success('Login successful');

      if (role === 'Patient') {
        navigate('/patient/dashboard');
      } else {
        navigate('/doctor/dashboard');
      }

    } catch (error: any) {
      const message = error.response?.data?.message || 'Login failed';
      toast.error(message);
      throw error;
    }
  };

  const signup = async (data: any, role: Role) => {
  try {
    const response = role === 'Patient'
      ? await registerPatient(data)
      : await registerDoctor(data);

    const { accessToken, user: userData } = response;

    localStorage.setItem('token', accessToken);
    localStorage.setItem('user', JSON.stringify(userData));

    setToken(accessToken);
    setUser(userData);

    toast.success('Registration successful');

    if (role === 'Patient') {
      navigate('/patient/dashboard');
    } else {
      navigate('/doctor/dashboard');
    }

  } catch (error: any) {
    console.error(error);

    const message =
      error?.response?.data?.message ||
      error?.message ||
      'Registration failed';

    toast.error(message);
  }
};

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');

    setToken(null);
    setUser(null);

    navigate('/login');

    toast.success('Logged out successfully');
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!token,
        login,
        signup,
        logout,
        isLoading,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);

  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }

  return context;
};