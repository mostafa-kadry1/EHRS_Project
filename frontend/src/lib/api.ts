import axios, { AxiosError, AxiosRequestConfig, AxiosResponse } from 'axios';

const DEFAULT_TIMEOUT_MS = 15000;

function getStoredToken(): string | null {
  try {
    return localStorage.getItem('token');
  } catch {
    return null;
  }
}

function clearAuthStorage() {
  try {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  } catch {
    // ignore
  }
}

function redirectToLogin() {
  if (window.location.pathname.includes('/login')) return;
  window.location.href = '/login';
}

export type ApiErrorCode =
  | 'NETWORK_ERROR'
  | 'UNAUTHORIZED'
  | 'TIMEOUT'
  | 'BAD_REQUEST'
  | 'NOT_FOUND'
  | 'SERVER_ERROR'
  | 'UNKNOWN_ERROR';

export interface ApiErrorPayload {
  code: ApiErrorCode;
  message: string;
  status?: number;
  data?: unknown;
}

function mapAxiosError(error: AxiosError): ApiErrorPayload {
  if (error.code === 'ECONNABORTED') {
    return { code: 'TIMEOUT', message: 'Request timeout' };
  }

  if (!error.response) {
    return { code: 'NETWORK_ERROR', message: 'Network error. Please check your connection.' };
  }

  const status = error.response.status;
  if (status === 401) {
    return { code: 'UNAUTHORIZED', message: 'Unauthorized' };
  }
  if (status === 400) return { code: 'BAD_REQUEST', message: 'Bad request', status };
  if (status === 404) return { code: 'NOT_FOUND', message: 'Not found', status };
  if (status >= 500) return { code: 'SERVER_ERROR', message: 'Server error', status };

  return {
    code: 'UNKNOWN_ERROR',
    message: error.message || 'Unknown error',
    status,
    data: error.response.data,
  };
}

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5175/api',
  timeout: DEFAULT_TIMEOUT_MS,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor: auto token injection
api.interceptors.request.use((config) => {
  const token = getStoredToken();
  if (token) {
    // Axios v1 headers type is strict; avoid typing issues by using plain assignment.
    (config.headers as any) = config.headers ?? {};
    (config.headers as any)['Authorization'] = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor: centralized error mapping + graceful 401 handling
api.interceptors.response.use(
  (response: AxiosResponse) => response,
  (error: AxiosError) => {
    const payload = mapAxiosError(error);

    if (payload.code === 'UNAUTHORIZED') {
      clearAuthStorage();
      redirectToLogin();
    }

    return Promise.reject({ ...payload, original: error });
  }
);

export default api;

