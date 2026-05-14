import { useEffect, useRef, useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Mail, ArrowRight, Lock, Eye, EyeOff, ArrowLeft, ShieldCheck, KeyRound } from "lucide-react";
import { useNavigate } from "react-router-dom";
import LanguageToggle from "@/components/navigation/LanguageToggle";
import MobileBrandHeader from "@/components/layout/MobileBrandHeader";
import HeroPanel from "@/components/layout/HeroPanel";
import FieldError from "@/components/shared/FieldError";
import { useLanguage } from "@/contexts/LanguageContext";
import { toast } from "@/hooks/use-toast";

const isEmail = (v: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v);

const RESEND_SECONDS = 30;

const ForgotPassword = () => {
  const { t } = useLanguage();
  const navigate = useNavigate();
  const [step, setStep] = useState<1 | 2 | 3>(1);

  // Step 1
  const [email, setEmail] = useState("");
  const [emailError, setEmailError] = useState<string | null>(null);

  // Step 2
  const [code, setCode] = useState("");
  const [codeError, setCodeError] = useState<string | null>(null);
  const [resendUntil, setResendUntil] = useState<number>(0);
  const [now, setNow] = useState(Date.now());
  const tickRef = useRef<number | null>(null);

  // Step 3
  const [newPassword, setNewPassword] = useState("");
  const [confirmNew, setConfirmNew] = useState("");
  const [showPwd, setShowPwd] = useState(false);
  const [showConfirmPwd, setShowConfirmPwd] = useState(false);
  const [pwdError, setPwdError] = useState<string | null>(null);
  const [confirmError, setConfirmError] = useState<string | null>(null);

  useEffect(() => {
    if (resendUntil) {
      tickRef.current = window.setInterval(() => setNow(Date.now()), 500);
      return () => {
        if (tickRef.current) window.clearInterval(tickRef.current);
      };
    }
  }, [resendUntil]);

  const remaining = Math.max(0, Math.ceil((resendUntil - now) / 1000));

  const startResendTimer = () => setResendUntil(Date.now() + RESEND_SECONDS * 1000);

  // ---- Live validation handlers ----
  const onEmailChange = (v: string) => {
    const trimmed = v.trim();
    setEmail(trimmed);
    if (!trimmed) setEmailError(null);
    else if (!isEmail(trimmed)) setEmailError(t.emailInvalidStrict);
    else setEmailError(null);
  };

  const onCodeChange = (v: string) => {
    const digits = v.replace(/\D/g, "").slice(0, 6);
    setCode(digits);
    if (!digits) setCodeError(null);
    else if (digits.length < 6) setCodeError(t.codeInvalid);
    else setCodeError(null);
  };

  const onNewPasswordChange = (v: string) => {
    setNewPassword(v);
    if (!v) setPwdError(null);
    else if (v.length < 8) setPwdError(t.passwordMinChars);
    else setPwdError(null);
    if (confirmNew && v !== confirmNew) setConfirmError(t.confirmMismatch);
    else setConfirmError(null);
  };

  const onConfirmChange = (v: string) => {
    setConfirmNew(v);
    if (!v) setConfirmError(null);
    else if (v !== newPassword) setConfirmError(t.confirmMismatch);
    else setConfirmError(null);
  };

  // ---- Step submits ----
  const submitEmail = (e: React.FormEvent) => {
    e.preventDefault();
    if (!email.trim()) {
      setEmailError(t.emailRequired);
      return;
    }
    if (!isEmail(email.trim())) {
      setEmailError(t.emailInvalidStrict);
      return;
    }
    startResendTimer();
    toast({ title: t.successTitle, description: t.codeSent });
    setStep(2);
  };

  const submitCode = (e: React.FormEvent) => {
    e.preventDefault();
    if (!code) {
      setCodeError(t.codeRequired);
      return;
    }
    if (code.length !== 6) {
      setCodeError(t.codeInvalid);
      return;
    }
    toast({ title: t.successTitle, description: t.codeVerified });
    setStep(3);
  };

  const submitReset = (e: React.FormEvent) => {
    e.preventDefault();
    if (newPassword.length < 8) {
      setPwdError(t.passwordMinChars);
      return;
    }
    if (newPassword !== confirmNew) {
      setConfirmError(t.confirmMismatch);
      return;
    }
    toast({ title: t.successTitle, description: t.passwordChanged });
    navigate("/login");
  };

  const handleResend = () => {
    if (remaining > 0) return;
    startResendTimer();
    toast({ title: t.successTitle, description: t.codeSent });
  };

  return (
    <div className="min-h-screen bg-background flex">
      <aside className="hidden lg:block w-1/2 p-4 sticky top-0 h-screen">
        <HeroPanel />
      </aside>

      <div className="w-full lg:w-1/2 flex items-start lg:items-center justify-center pt-6 pb-10 lg:py-12 min-h-screen">
        <div className="w-full max-w-md mx-auto px-6 lg:px-0">
          <MobileBrandHeader />
          <div className="hidden lg:flex items-center justify-between mb-6">
            <span className="text-xs font-bold tracking-widest text-muted-foreground">EHRS</span>
            <LanguageToggle />
          </div>

          <button
            type="button"
            onClick={() => (step === 1 ? navigate("/login") : setStep((s) => (s - 1) as 1 | 2 | 3))}
            className="flex items-center gap-1.5 text-xs font-medium text-muted-foreground hover:text-foreground transition-colors mb-6"
          >
            <ArrowLeft size={14} className="rtl:rotate-180" />
            {t.backToSignIn}
          </button>

          {/* Stepper */}
          <div className="flex items-center gap-2 mb-6" aria-label="progress">
            {[1, 2, 3].map((n) => (
              <div
                key={n}
                className={`h-1.5 flex-1 rounded-full transition-colors duration-300 ${
                  n <= step ? "bg-primary" : "bg-muted"
                }`}
              />
            ))}
          </div>
          <p className="text-xs font-medium text-muted-foreground mb-2">
            {t.step} {step} {t.of} 3
          </p>

          <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.4 }}
          >
            <h2 className="text-3xl font-display font-bold text-foreground mb-1">{t.forgotTitle}</h2>
            <p className="text-muted-foreground mb-8 text-sm">
              {step === 1 ? t.forgotStepEmailDesc : step === 2 ? t.forgotStepCodeDesc : t.forgotStepResetDesc}
            </p>

            <AnimatePresence mode="wait">
              {step === 1 && (
                <motion.form
                  key="step-1"
                  initial={{ opacity: 0, x: 20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: -20 }}
                  transition={{ duration: 0.25 }}
                  onSubmit={submitEmail}
                  className="space-y-5"
                >
                  <div className="space-y-1.5">
                    <label htmlFor="fp-email" className="text-sm font-medium text-foreground">
                      {t.emailAddress}
                    </label>
                    <div className="relative">
                      <Mail size={18} className="absolute left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground rtl:left-auto rtl:right-3.5" />
                      <input
                        id="fp-email"
                        type="email"
                        autoComplete="email"
                        inputMode="email"
                        value={email}
                        onChange={(e) => onEmailChange(e.target.value)}
                        placeholder={t.emailAddressPlaceholder}
                        className={`w-full pl-11 rtl:pl-4 rtl:pr-11 pr-4 py-3 rounded-lg border bg-card text-foreground placeholder:text-muted-foreground/60 transition-all duration-200 focus:outline-none focus:ring-2 focus:border-transparent text-sm ${
                          emailError ? "border-destructive ring-destructive/40" : "border-border ring-primary"
                        }`}
                      />
                    </div>
                    <FieldError message={emailError} />
                  </div>

                  <button
                    type="submit"
                    className="w-full py-3.5 rounded-lg font-semibold text-sm flex items-center justify-center gap-2 bg-primary text-primary-foreground shadow-lg transition-transform active:scale-[0.99]"
                    style={{ boxShadow: "0 8px 24px -6px hsl(var(--primary) / 0.45)" }}
                  >
                    {t.sendCode}
                    <ArrowRight size={16} className="rtl:rotate-180" />
                  </button>
                </motion.form>
              )}

              {step === 2 && (
                <motion.form
                  key="step-2"
                  initial={{ opacity: 0, x: 20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: -20 }}
                  transition={{ duration: 0.25 }}
                  onSubmit={submitCode}
                  className="space-y-5"
                >
                  <div className="space-y-1.5">
                    <label htmlFor="fp-code" className="text-sm font-medium text-foreground">
                      {t.verificationCode}
                    </label>
                    <div className="relative">
                      <ShieldCheck size={18} className="absolute left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground rtl:left-auto rtl:right-3.5" />
                      <input
                        id="fp-code"
                        type="text"
                        inputMode="numeric"
                        autoComplete="one-time-code"
                        maxLength={6}
                        value={code}
                        onChange={(e) => onCodeChange(e.target.value)}
                        placeholder={t.codePlaceholder}
                        className={`w-full pl-11 rtl:pl-4 rtl:pr-11 pr-4 py-3 rounded-lg border bg-card text-foreground placeholder:text-muted-foreground/60 transition-all duration-200 focus:outline-none focus:ring-2 focus:border-transparent text-sm tracking-[0.5em] font-mono text-center ${
                          codeError ? "border-destructive ring-destructive/40" : "border-border ring-primary"
                        }`}
                      />
                    </div>
                    <FieldError message={codeError} />
                  </div>

                  <div className="text-center text-sm text-muted-foreground">
                    {t.didntReceiveCode}{" "}
                    {remaining > 0 ? (
                      <span className="font-semibold text-muted-foreground">
                        {t.resendIn} {remaining}s
                      </span>
                    ) : (
                      <button type="button" onClick={handleResend} className="font-semibold text-primary hover:underline">
                        {t.resendCode}
                      </button>
                    )}
                  </div>

                  <button
                    type="submit"
                    className="w-full py-3.5 rounded-lg font-semibold text-sm flex items-center justify-center gap-2 bg-primary text-primary-foreground shadow-lg transition-transform active:scale-[0.99]"
                    style={{ boxShadow: "0 8px 24px -6px hsl(var(--primary) / 0.45)" }}
                  >
                    {t.verifyCode}
                    <ArrowRight size={16} className="rtl:rotate-180" />
                  </button>
                </motion.form>
              )}

              {step === 3 && (
                <motion.form
                  key="step-3"
                  initial={{ opacity: 0, x: 20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: -20 }}
                  transition={{ duration: 0.25 }}
                  onSubmit={submitReset}
                  className="space-y-5"
                >
                  <div className="space-y-1.5">
                    <label htmlFor="fp-new" className="text-sm font-medium text-foreground">
                      {t.newPassword}
                    </label>
                    <div className="relative">
                      <KeyRound size={18} className="absolute left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground rtl:left-auto rtl:right-3.5" />
                      <input
                        id="fp-new"
                        type={showPwd ? "text" : "password"}
                        value={newPassword}
                        onChange={(e) => onNewPasswordChange(e.target.value)}
                        placeholder={t.newPasswordPlaceholder}
                        autoComplete="new-password"
                        className={`w-full pl-11 rtl:pl-4 rtl:pr-11 pr-11 py-3 rounded-lg border bg-card text-foreground placeholder:text-muted-foreground/60 transition-all duration-200 focus:outline-none focus:ring-2 focus:border-transparent text-sm ${
                          pwdError ? "border-destructive ring-destructive/40" : "border-border ring-primary"
                        }`}
                      />
                      <button
                        type="button"
                        onClick={() => setShowPwd(!showPwd)}
                        className="absolute right-3.5 rtl:right-auto rtl:left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors"
                      >
                        {showPwd ? <EyeOff size={18} /> : <Eye size={18} />}
                      </button>
                    </div>
                    <FieldError message={pwdError} />
                  </div>

                  <div className="space-y-1.5">
                    <label htmlFor="fp-confirm" className="text-sm font-medium text-foreground">
                      {t.confirmPassword}
                    </label>
                    <div className="relative">
                      <Lock size={18} className="absolute left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground rtl:left-auto rtl:right-3.5" />
                      <input
                        id="fp-confirm"
                        type={showConfirmPwd ? "text" : "password"}
                        value={confirmNew}
                        onChange={(e) => onConfirmChange(e.target.value)}
                        placeholder={t.confirmPlaceholder}
                        autoComplete="new-password"
                        className={`w-full pl-11 rtl:pl-4 rtl:pr-11 pr-11 py-3 rounded-lg border bg-card text-foreground placeholder:text-muted-foreground/60 transition-all duration-200 focus:outline-none focus:ring-2 focus:border-transparent text-sm ${
                          confirmError ? "border-destructive ring-destructive/40" : "border-border ring-primary"
                        }`}
                      />
                      <button
                        type="button"
                        onClick={() => setShowConfirmPwd(!showConfirmPwd)}
                        aria-label={showConfirmPwd ? "Hide password" : "Show password"}
                        className="absolute right-3.5 rtl:right-auto rtl:left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors"
                      >
                        {showConfirmPwd ? <EyeOff size={18} /> : <Eye size={18} />}
                      </button>
                    </div>
                    <FieldError message={confirmError} />
                  </div>

                  <button
                    type="submit"
                    className="w-full py-3.5 rounded-lg font-semibold text-sm flex items-center justify-center gap-2 bg-primary text-primary-foreground shadow-lg transition-transform active:scale-[0.99]"
                    style={{ boxShadow: "0 8px 24px -6px hsl(var(--primary) / 0.45)" }}
                  >
                    {t.saveNewPassword}
                    <ArrowRight size={16} className="rtl:rotate-180" />
                  </button>
                </motion.form>
              )}
            </AnimatePresence>
          </motion.div>
        </div>
      </div>
    </div>
  );
};

export default ForgotPassword;
