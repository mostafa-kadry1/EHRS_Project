import { useEffect, useRef, useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Eye, EyeOff, Mail, ArrowRight, ShieldAlert } from "lucide-react";
import { useNavigate } from "react-router-dom";
import RoleToggle from "./RoleToggle";
import LanguageToggle from "@/components/navigation/LanguageToggle";
import MobileBrandHeader from "@/components/layout/MobileBrandHeader";
import FieldError from "@/components/shared/FieldError";
import { useLanguage } from "@/contexts/LanguageContext";
import { useAuth } from "@/contexts/AuthContext";
import { toast } from "sonner";

type Role = "doctor" | "patient";

const MAX_ATTEMPTS = 5;
const LOCKOUT_SECONDS = 60;

const isEmail = (v: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v);
const isUsername = (v: string) => /^[a-zA-Z0-9_.-]{3,}$/.test(v);


const LoginForm = () => {
  const [role, setRole] = useState<Role>("doctor");
  const [showPassword, setShowPassword] = useState(false);
  const [email, setEmail] = useState("");
  const [emailError, setEmailError] = useState<string | null>(null);
  const [password, setPassword] = useState("");
  const [passwordError, setPasswordError] = useState<string | null>(null);
  const [remember, setRemember] = useState(false);
  const [attempts, setAttempts] = useState(0);
  const [lockUntil, setLockUntil] = useState<number | null>(null);
  const [now, setNow] = useState(Date.now());
  const intervalRef = useRef<number | null>(null);
  const navigate = useNavigate();
  const { t } = useLanguage();
  const { login } = useAuth();
  const [isLoading, setIsLoading] = useState(false);

  const isDoctor = role === "doctor";
  const ringClass = "ring-primary/40";
  const textClass = "text-primary";

  const isLocked = lockUntil !== null && now < lockUntil;
  const remaining = isLocked ? Math.ceil((lockUntil! - now) / 1000) : 0;

  useEffect(() => {
    if (lockUntil) {
      intervalRef.current = window.setInterval(() => setNow(Date.now()), 500);
      return () => {
        if (intervalRef.current) window.clearInterval(intervalRef.current);
      };
    }
  }, [lockUntil]);

  useEffect(() => {
    if (lockUntil && now >= lockUntil) {
      setLockUntil(null);
      setAttempts(0);
    }
  }, [now, lockUntil]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (isLocked || isLoading) return;

    if (!email.trim()) {
      toast.error(t.emailRequired);
      return;
    }
    if (!isEmail(email.trim())) {
      toast.error(t.emailInvalidStrict);
      return;
    }
    if (!password) {
      toast.error(t.passwordRequired);
      return;
    }

    setIsLoading(true);
    try {
      await login({ email, password, rememberMe: remember }, isDoctor ? 'Doctor' : 'Patient');
    } catch (error) {
      // Error is handled in AuthContext
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="w-full max-w-md mx-auto px-6 lg:px-0">
      <MobileBrandHeader />
      <div className="hidden lg:flex items-center justify-between mb-6">
        <span className="text-xs font-bold tracking-widest text-muted-foreground uppercase">HealthCore EHRS</span>
        <LanguageToggle />
      </div>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
      >
        <h2 className="text-3xl font-display font-bold text-foreground mb-1">
          {t.signIn}
        </h2>
        <p className="text-muted-foreground mb-8">
          {isDoctor ? t.accessDoctor : t.accessPatient}
        </p>

        <RoleToggle role={role} onRoleChange={setRole} />

        <AnimatePresence mode="wait">
          <motion.form
            key={role}
            initial={{ opacity: 0, x: 10 }}
            animate={{ opacity: 1, x: 0 }}
            exit={{ opacity: 0, x: -10 }}
            transition={{ duration: 0.2 }}
            className="mt-8 space-y-5"
            onSubmit={handleSubmit}
          >
            {/* Email */}
            <div className="space-y-1.5">
              <label htmlFor="login-email" className="text-sm font-medium text-foreground">{t.emailAddress}</label>
              <div className="relative">
                <Mail size={18} className="absolute left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground rtl:left-auto rtl:right-3.5" />
                <input
                  id="login-email"
                  type="email"
                  name="email"
                  autoComplete="email"
                  inputMode="email"
                  spellCheck={false}
                  autoCapitalize="none"
                  required
                  value={email}
                  onChange={(e) => {
                    const v = e.target.value.trim();
                    setEmail(v);
                    if (!v) setEmailError(null);
                    else if (!isEmail(v)) setEmailError(t.emailInvalidStrict);
                    else setEmailError(null);
                  }}
                  placeholder={t.emailAddressPlaceholder}
                  className={`w-full pl-11 rtl:pl-4 rtl:pr-11 pr-4 py-3 rounded-lg border bg-card text-foreground placeholder:text-muted-foreground/60 transition-all duration-200 focus:outline-none focus:ring-2 focus:border-transparent text-sm ${
                    emailError ? "border-destructive ring-destructive/40" : `border-border ${ringClass}`
                  }`}
                />
              </div>
              <FieldError message={emailError} />
            </div>

            {/* Password */}
            <div className="space-y-1.5">
              <div className="flex items-center justify-between">
                <label className="text-sm font-medium text-foreground">{t.password}</label>
                <button
                  type="button"
                  onClick={() => navigate("/forgot-password")}
                  className={`text-xs font-medium ${textClass} hover:underline`}
                >
                  {t.forgotPassword}
                </button>
              </div>
              <div className="relative">
                <input
                  type={showPassword ? "text" : "password"}
                  autoComplete="current-password"
                  value={password}
                  onChange={(e) => {
                    const v = e.target.value;
                    setPassword(v);
                    if (!v) setPasswordError(null);
                    else if (isEmail(v)) setPasswordError(t.passwordIsEmail);
                    else if (v.length >= 8 && /^\d+$/.test(v)) setPasswordError(t.passwordNumericOnly);
                    else setPasswordError(null);
                  }}
                  placeholder={t.passwordPlaceholder}
                  className={`w-full pl-4 rtl:pr-4 pr-11 rtl:pl-11 py-3 rounded-lg border bg-card text-foreground placeholder:text-muted-foreground/60 transition-all duration-200 focus:outline-none focus:ring-2 focus:border-transparent text-sm ${
                    passwordError ? "border-destructive ring-destructive/40" : `border-border ${ringClass}`
                  }`}
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3.5 rtl:right-auto rtl:left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors"
                >
                  {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                </button>
              </div>
              <FieldError message={passwordError} />
            </div>

            {/* Remember Me */}
            <div className="flex items-center gap-2.5">
              <button
                type="button"
                onClick={() => setRemember(!remember)}
                className={`w-4.5 h-4.5 min-w-[18px] min-h-[18px] rounded border-2 flex items-center justify-center transition-all duration-200 ${
                  remember ? "bg-primary border-transparent" : "border-border bg-card"
                }`}
              >
                {remember && (
                  <svg width="10" height="8" viewBox="0 0 10 8" fill="none">
                    <path d="M1 4L3.5 6.5L9 1" stroke="white" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                  </svg>
                )}
              </button>
              <span className="text-sm text-muted-foreground">{t.rememberMe}</span>
            </div>

            {/* Lockout banner */}
            {isLocked && (
              <div className="flex items-center gap-2 rounded-lg border border-destructive/30 bg-destructive/10 px-3 py-2.5 text-sm text-destructive">
                <ShieldAlert size={16} className="shrink-0" />
                <span>
                  {t.lockedDesc} {remaining} {t.seconds}.
                </span>
              </div>
            )}

            {/* Submit - hidden during lockout */}
            {!isLocked && (
              <motion.button
                whileHover={{ scale: 1.01 }}
                whileTap={{ scale: 0.99 }}
                type="submit"
                disabled={isLoading}
                className={`w-full py-3.5 rounded-lg font-semibold text-sm flex items-center justify-center gap-2 transition-all duration-200 shadow-lg bg-primary text-primary-foreground ${isLoading ? 'opacity-70 cursor-not-allowed' : ''}`}
                style={{
                  boxShadow: "0 8px 24px -6px hsl(var(--primary) / 0.45)",
                }}
              >
                {isLoading ? (
                  <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                ) : (
                  <>
                    {t.signInBtn}
                    <ArrowRight size={16} />
                  </>
                )}
              </motion.button>
            )}
          </motion.form>
        </AnimatePresence>

        <p className="text-center text-sm text-muted-foreground mt-8">
          {t.noAccount}{" "}
          <button onClick={() => navigate("/signup", { state: { role } })} className={`font-semibold ${textClass} hover:underline`}>
            {t.createAccount}
          </button>
        </p>
      </motion.div>
    </div>
  );
};

export default LoginForm;
