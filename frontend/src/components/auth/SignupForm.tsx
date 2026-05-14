import { useState, useRef } from "react";
import { motion, AnimatePresence } from "framer-motion";
import {
  Mail, Eye, EyeOff, ArrowRight, UserIcon,
  Stethoscope, CreditCard, Calendar, Users,
} from "lucide-react";
import { useNavigate, useLocation } from "react-router-dom";
import { format } from "date-fns";
import { cn } from "@/lib/utils";
import RoleToggle from "./RoleToggle";
import LanguageToggle from "@/components/navigation/LanguageToggle";
import MobileBrandHeader from "@/components/layout/MobileBrandHeader";
import FieldError from "@/components/shared/FieldError";
import { useLanguage, SPECIALTIES } from "@/contexts/LanguageContext";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Calendar as CalendarPicker } from "@/components/ui/calendar";
import DateInput from "@/components/shared/DateInput";
import {
  Select, SelectContent, SelectItem,
  SelectTrigger, SelectValue,
} from "@/components/ui/select";
import { ChevronDown } from "lucide-react";
import { useAuth } from "@/contexts/AuthContext";
import { toast } from "sonner";

type Role = "doctor" | "patient";

// ── Helpers ──────────────────────────────────────────────────────────────────

const isEmail = (v: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v);

const TODAY = new Date();
TODAY.setHours(0, 0, 0, 0);

const MIN_DOB = new Date();
MIN_DOB.setFullYear(TODAY.getFullYear() - 120);
MIN_DOB.setHours(0, 0, 0, 0);

const CURRENT_YEAR = TODAY.getFullYear();

/**
 * Password complexity rules (applied to both Doctor and Patient):
 *  – minimum 8 characters
 *  – must NOT be purely numeric
 *  – must contain at least one letter
 *  – must contain at least one digit
 * Returns null when valid, or an error string.
 */
function validatePassword(v: string, t: ReturnType<typeof useLanguage>["t"]): string | null {
  if (!v) return null;
  if (v.length < 8) return t.passwordMinChars;
  if (/^\d+$/.test(v)) return t.passwordNumericOnly;
  if (!/[a-zA-Z]/.test(v)) return t.passwordNoLetters;
  if (!/\d/.test(v)) return t.passwordWeak; // needs at least one digit
  return null;
}

/**
 * Medical License Number validation.
 * Egyptian Medical Syndicate numbers are typically 5–7 digits.
 * We also accept alphanumeric formats (some hospitals prefix with letters).
 * Rules: 5–20 alphanumeric characters, no spaces or special chars.
 */
function validateLicense(v: string, t: ReturnType<typeof useLanguage>["t"]): string | null {
  if (!v) return null;
  if (!/^[a-zA-Z0-9]+$/.test(v)) return t.licenseOnlyAlnum;
  if (v.length < 5) return t.licenseTooShort;
  if (v.length > 20) return t.licenseTooLong;
  return null;
}

// ── Component ─────────────────────────────────────────────────────────────────

const SignupForm = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const initialRole = (location.state as { role?: Role })?.role || "doctor";

  const [role, setRole] = useState<Role>(initialRole);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);

  // Popover open state so we can close it on confirm
  const [dobOpen, setDobOpen] = useState(false);
  // Pending selection — we only commit when user clicks Confirm
  const [pendingDob, setPendingDob] = useState<Date | undefined>();

  const [fullName, setFullName] = useState("");
  const [fullNameError, setFullNameError] = useState<string | null>(null);

  const [email, setEmail] = useState("");
  const [emailError, setEmailError] = useState<string | null>(null);

  const [specialty, setSpecialty] = useState("");
  const [specialtyError, setSpecialtyError] = useState<string | null>(null);

  const [licenseNumber, setLicenseNumber] = useState("");
  const [licenseError, setLicenseError] = useState<string | null>(null);

  const [gender, setGender] = useState("");
  const [genderError, setGenderError] = useState<string | null>(null);

  // The committed DOB (sent to backend)
  const [dateOfBirth, setDateOfBirth] = useState<Date | undefined>();
  const [dobError, setDobError] = useState<string | null>(null);

  const [nationalId, setNationalId] = useState("");
  const [nationalIdError, setNationalIdError] = useState<string | null>(null);

  const [password, setPassword] = useState("");
  const [passwordError, setPasswordError] = useState<string | null>(null);

  const [confirmPassword, setConfirmPassword] = useState("");
  const [confirmError, setConfirmError] = useState<string | null>(null);

  const { t, lang } = useLanguage();
  const { signup } = useAuth();
  const [isLoading, setIsLoading] = useState(false);

  const isDoctor = role === "doctor";
  const ringClass = "ring-primary/40";
  const textClass = "text-primary";
  const specialties = SPECIALTIES;

const inputBase =
  "w-full h-12 pl-11 rtl:pl-4 rtl:pr-11 pr-10 rounded-lg border bg-card text-foreground " +
  "placeholder:text-muted-foreground/60 transition-all duration-200 focus:outline-none " +
  "focus:ring-2 focus:border-transparent text-sm appearance-none";
  const inputCls = (err: string | null) =>
    `${inputBase} ${err ? "border-destructive ring-destructive/40" : `border-border ${ringClass}`}`;
  const iconClass =
    "absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground rtl:left-auto rtl:right-4";

  // ── Live validation handlers ──────────────────────────────────────────────

  const onFullName = (v: string) => {
    setFullName(v);
    if (!v) return setFullNameError(null);
    if (/\d/.test(v)) return setFullNameError(t.fullNameNoNumbers);
    if (/[^\p{L}\s'.-]/u.test(v)) return setFullNameError(t.fullNameNoSymbols);
    setFullNameError(null);
  };

  const onEmail = (v: string) => {
    const trimmed = v.trim();
    setEmail(trimmed);
    if (!trimmed) return setEmailError(null);
    if (!isEmail(trimmed)) return setEmailError(t.emailInvalidStrict);
    setEmailError(null);
  };

  const onLicense = (v: string) => {
    setLicenseNumber(v);
    setLicenseError(validateLicense(v, t));
  };

  const onNationalId = (raw: string) => {
    const hadLetters = /\D/.test(raw);
    const digits = raw.replace(/\D/g, "").slice(0, 14);
    setNationalId(digits);
    if (!digits) return setNationalIdError(hadLetters ? t.nationalIdOnlyDigits : null);
    if (hadLetters) return setNationalIdError(t.nationalIdOnlyDigits);
    if (digits.length !== 14) return setNationalIdError(t.nationalIdLength);
    setNationalIdError(null);
  };

  const onPassword = (v: string) => {
    setPassword(v);
    const err = validatePassword(v, t);
    setPasswordError(err);
    if (confirmPassword && v !== confirmPassword) setConfirmError(t.confirmMismatch);
    else setConfirmError(null);
  };

  const onConfirm = (v: string) => {
    setConfirmPassword(v);
    if (!v) return setConfirmError(null);
    if (v !== password) return setConfirmError(t.confirmMismatch);
    setConfirmError(null);
  };

  // DOB: user picks a day → stored in pendingDob until they click Confirm
  const onDaySelect = (d: Date | undefined) => {
    setPendingDob(d);
  };

  const onDobConfirm = () => {
    if (!pendingDob) return;
    setDateOfBirth(pendingDob);
    setDobError(null);
    setDobOpen(false);
  };

  // ── Submit ────────────────────────────────────────────────────────────────

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (isLoading) return;

    let firstError: string | null = null;

    if (!fullName.trim()) { setFullNameError(t.fullNameRequired); firstError ??= t.fullNameRequired; }
    else if (fullNameError) firstError ??= fullNameError;

    if (!email.trim()) { setEmailError(t.emailRequired); firstError ??= t.emailRequired; }
    else if (!isEmail(email.trim())) { setEmailError(t.emailInvalidStrict); firstError ??= t.emailInvalidStrict; }

    if (isDoctor) {
      if (!specialty) { setSpecialtyError(t.specialtyRequired); firstError ??= t.specialtyRequired; }
      else setSpecialtyError(null);

      const licErr = validateLicense(licenseNumber, t);
      if (!licenseNumber.trim()) {
        setLicenseError(t.licenseRequired); firstError ??= t.licenseRequired;
      } else if (licErr) {
        setLicenseError(licErr); firstError ??= licErr;
      }
    } else {
      if (!gender) { setGenderError(t.genderRequired); firstError ??= t.genderRequired; }
      else setGenderError(null);

      if (!dateOfBirth) { setDobError(t.dobRequired); firstError ??= t.dobRequired; }
      else setDobError(null);

      if (!nationalId.trim()) { setNationalIdError(t.nationalIdRequired); firstError ??= t.nationalIdRequired; }
      else if (nationalId.length !== 14) { setNationalIdError(t.nationalIdLength); firstError ??= t.nationalIdLength; }
    }

    // Password complexity
    const pwdErr = !password ? t.passwordRequired : validatePassword(password, t);
    if (pwdErr) { setPasswordError(pwdErr); firstError ??= pwdErr; }
    if (password !== confirmPassword) { setConfirmError(t.confirmMismatch); firstError ??= t.confirmMismatch; }

    if (firstError) { toast.error(firstError); return; }

    setIsLoading(true);
    try {
      const data = isDoctor
        ? { fullName, email, password, confirmPassword, specialization: specialty, medicalLicense: licenseNumber }
        : { fullName, email, password, confirmPassword, gender, dateOfBirth: dateOfBirth?.toISOString(), nationalId };

      await signup(data, isDoctor ? "Doctor" : "Patient");
    } catch {
      // Error handled in AuthContext
    } finally {
      setIsLoading(false);
    }
  };

  // ── Render ────────────────────────────────────────────────────────────────

  return (
    <div className="w-full max-w-md mx-auto px-6 lg:px-0">
      <MobileBrandHeader />
      <div className="hidden lg:flex items-center justify-between mb-6">
        <span className="text-xs font-bold tracking-widest text-muted-foreground">EHRS</span>
        <LanguageToggle />
      </div>

      <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.5 }}>
        <h2 className="text-3xl font-display font-bold text-foreground mb-1">{t.signUp}</h2>
        <p className="text-muted-foreground mb-8">
          {isDoctor ? t.createAccessDoctor : t.createAccessPatient}
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
            noValidate
          >
            {/* Full Name */}
            <div className="space-y-1.5">
              <label className="text-sm font-medium text-foreground">{t.fullName}</label>
              <div className="relative">
                <UserIcon size={18} className={iconClass} />
                <input
                  type="text"
                  value={fullName}
                  onChange={(e) => onFullName(e.target.value)}
                  placeholder={t.namePlaceholder}
                  className={inputCls(fullNameError)}
                />
              </div>
              <FieldError message={fullNameError} />
            </div>

            {/* Email */}
            <div className="space-y-1.5">
              <label htmlFor="signup-email" className="text-sm font-medium text-foreground">
                {t.emailAddress}
              </label>
              <div className="relative">
                <Mail size={18} className={iconClass} />
                <input
                  id="signup-email"
                  type="email"
                  name="email"
                  autoComplete="email"
                  inputMode="email"
                  spellCheck={false}
                  autoCapitalize="none"
                  value={email}
                  onChange={(e) => onEmail(e.target.value)}
                  placeholder={t.emailAddressPlaceholder}
                  className={inputCls(emailError)}
                />
              </div>
              <FieldError message={emailError} />
            </div>

            {/* ── Doctor-specific ── */}
            {isDoctor && (
              <>
                {/* Specialty */}
                <div className="space-y-1.5">
                  <label className="text-sm font-medium text-foreground">{t.specialty}</label>
                  <div className="relative">
                    <Stethoscope size={18} className={iconClass} />
                    <Select
                      value={specialty}
                      onValueChange={(v) => { setSpecialty(v); setSpecialtyError(null); }}
                    >
                      <SelectTrigger
                        dir={lang === "ar" ? "rtl" : "ltr"}
                        className={cn(
                          inputCls(specialtyError),
                          "h-12 cursor-pointer",
                          lang === "ar"
                            ? "pl-12 pr-4 text-right"
                            : "pl-12 pr-4 text-left"
                        )}
                      >
                        <SelectValue placeholder={t.selectSpecialty} />
                      </SelectTrigger>
                      <SelectContent className="max-h-60">
                        {specialties.map((s) => (
                          <SelectItem key={s.key} value={s.key}>
                            {lang === "ar" ? s.ar : s.en}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                  <FieldError message={specialtyError} />
                </div>

                {/* Medical License Number */}
                <div className="space-y-1.5">
                  <label className="text-sm font-medium text-foreground">{t.licenseNumber}</label>
                  <div className="relative">
                    <CreditCard size={18} className={iconClass} />
                    <input
                      type="text"
                      value={licenseNumber}
                      onChange={(e) => onLicense(e.target.value)}
                      placeholder={lang === "ar" ? "مثال: 12345 أو ABC123" : "e.g. 12345 or ABC123"}
                      maxLength={20}
                      className={inputCls(licenseError)}
                    />
                  </div>
                  {/* Helper hint */}
                  {!licenseError && (
                    <p className="text-[11px] text-muted-foreground">
                      {lang === "ar"
                        ? "5–20 حرفًا أبجديًا رقميًا (حروف أو أرقام فقط)"
                        : "5–20 alphanumeric characters (letters and/or digits)"}
                    </p>
                  )}
                  <FieldError message={licenseError} />
                </div>
              </>
            )}

            {/* ── Patient-specific ── */}
            {!isDoctor && (
              <>
                {/* Gender */}
                <div className="space-y-1.5">
                  <label className="text-sm font-medium text-foreground">
                    {t.gender}
                  </label>

                  <div className="relative">
                    <Users size={18} className={iconClass} />

                    <select
                      value={gender}
                      onChange={(e) => {
                        setGender(e.target.value);
                        setGenderError(null);
                      }}
                      className={inputCls(genderError)}
                    >
                      <option value="">{t.genderPlaceholder}</option>
                      <option value="male">{t.male}</option>
                      <option value="female">{t.female}</option>
                    </select>

                    <ChevronDown
                      size={18}
                      className="absolute right-4 rtl:right-auto rtl:left-4 top-1/2 -translate-y-1/2 text-muted-foreground pointer-events-none"
                    />
                  </div>

                  <FieldError message={genderError} />
                </div>

                {/* Date of Birth — shared DateInput (type + calendar icon) */}
                <div className="space-y-1.5">
                  <label className="text-sm font-medium text-foreground">{t.dateOfBirth}</label>
                  <DateInput
                    value={dateOfBirth}
                    onChange={(d) => { setDateOfBirth(d); if (d) setDobError(null); }}
                    minDate={MIN_DOB}
                    maxDate={TODAY}
                    fromYear={CURRENT_YEAR - 120}
                    toYear={CURRENT_YEAR}
                    placeholder={t.dobPlaceholder || "DD/MM/YYYY"}
                    confirmLabel={t.dobConfirm ?? "Confirm"}
                    error={!!dobError}
                  />
                  <FieldError message={dobError} />
                </div>

                {/* National ID */}
                <div className="space-y-1.5">
                  <label className="text-sm font-medium text-foreground">{t.nationalId}</label>
                  <div className="relative">
                    <CreditCard size={18} className={iconClass} />
                    <input
                      type="text"
                      inputMode="numeric"
                      maxLength={14}
                      value={nationalId}
                      onChange={(e) => onNationalId(e.target.value)}
                      onBeforeInput={(e: React.FormEvent<HTMLInputElement>) => {
                        const data = (e as any).data;
                        if (data && /\D/.test(data)) {
                          e.preventDefault();
                          setNationalIdError(t.nationalIdOnlyDigits);
                        }
                      }}
                      placeholder={t.nationalIdPlaceholder}
                      className={inputCls(nationalIdError)}
                    />
                  </div>
                  <FieldError message={nationalIdError} />
                </div>
              </>
            )}

            {/* Password */}
            <div className="space-y-1.5">
              <label className="text-sm font-medium text-foreground">{t.password}</label>
              <div className="relative">
                <input
                  type={showPassword ? "text" : "password"}
                  value={password}
                  onChange={(e) => onPassword(e.target.value)}
                  placeholder={t.passwordPlaceholder}
                  autoComplete="new-password"
                  className={`${inputCls(passwordError)} pl-4 rtl:pr-4 pr-11 rtl:pl-11`}
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3.5 rtl:right-auto rtl:left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors"
                >
                  {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                </button>
              </div>
              {/* Password hint */}
              {!passwordError && (
                <p className="text-[11px] text-muted-foreground">
                  {lang === "ar"
                    ? "8 أحرف على الأقل، تتضمن حروفًا وأرقامًا"
                    : "Min 8 characters — must include letters and numbers"}
                </p>
              )}
              <FieldError message={passwordError} />
            </div>

            {/* Confirm Password */}
            <div className="space-y-1.5">
              <label className="text-sm font-medium text-foreground">{t.confirmPassword}</label>
              <div className="relative">
                <input
                  type={showConfirm ? "text" : "password"}
                  value={confirmPassword}
                  onChange={(e) => onConfirm(e.target.value)}
                  placeholder={t.confirmPlaceholder}
                  autoComplete="new-password"
                  className={`${inputCls(confirmError)} pl-4 rtl:pr-4 pr-11 rtl:pl-11`}
                />
                <button
                  type="button"
                  onClick={() => setShowConfirm(!showConfirm)}
                  aria-label={showConfirm ? "Hide password" : "Show password"}
                  className="absolute right-3.5 rtl:right-auto rtl:left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors"
                >
                  {showConfirm ? <EyeOff size={18} /> : <Eye size={18} />}
                </button>
              </div>
              <FieldError message={confirmError} />
            </div>

            {/* Submit */}
            <motion.button
              whileHover={{ scale: 1.01 }}
              whileTap={{ scale: 0.99 }}
              type="submit"
              disabled={isLoading}
              className={`w-full py-3.5 rounded-lg font-semibold text-sm flex items-center justify-center gap-2 transition-all duration-200 shadow-lg bg-primary text-primary-foreground ${isLoading ? "opacity-70 cursor-not-allowed" : ""}`}
              style={{ boxShadow: "0 8px 24px -6px hsl(var(--primary) / 0.4)" }}
            >
              {isLoading ? (
                <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              ) : (
                <>
                  {t.signUp}
                  <ArrowRight size={16} className="rtl:rotate-180" />
                </>
              )}
            </motion.button>
          </motion.form>
        </AnimatePresence>

        <p className="text-center text-sm text-muted-foreground mt-8">
          {t.alreadyHaveAccount}{" "}
          <button
            onClick={() => navigate("/login")}
            className={`font-semibold ${textClass} hover:underline`}
          >
            {t.signInLink}
          </button>
        </p>
      </motion.div>
    </div>
  );
};

export default SignupForm;
