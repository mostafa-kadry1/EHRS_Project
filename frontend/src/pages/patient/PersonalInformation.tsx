import React, { useEffect, useState } from "react";
import "./personalinformation.css";
import defaultUser from "@/assets/patient_images/photo.jpg";
import { useLanguage as useLang } from "@/contexts/LanguageContext";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getPatientProfile, updatePatientProfile } from "@/api/profile.api";
import { useProfile } from "@/contexts/ProfileContext";
import { Loader2 } from "lucide-react";
import DateInput from "@/components/shared/DateInput";
import { toast } from "sonner";

/** Resolve a relative path from the API server into an absolute URL.
 *  The backend returns paths like /uploads/patients/5/profile/x.png.
 *  We prefix with the API base URL (without the /api suffix). */
function resolveImageUrl(path: string | undefined | null): string | undefined {
  if (!path) return undefined;
  if (path.startsWith("http") || path.startsWith("data:") || path.startsWith("blob:")) return path;
  const base = (import.meta.env.VITE_API_BASE_URL || "http://localhost:5175/api").replace(/\/api$/, "");
  return base + (path.startsWith("/") ? path : "/" + path);
}

const toAr = (s: any) => String(s || "").replace(/\d/g, (d) => "٠١٢٣٤٥٦٧٨٩"[+d]);

const genderAr = (v: string) => {
  if (!v) return "";
  if (v.toLowerCase() === "female") return "أنثى";
  if (v.toLowerCase() === "male") return "ذكر";
  return v;
};

const KEEP_ORIGINAL = ["email", "fullName"];

const BLOOD_TYPES = ["A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"];

const ACCEPTED_IMAGE_TYPES = ["image/jpeg", "image/jpg", "image/png", "image/webp"];

interface ProfileData {
  patientId: string;
  fullName: string;
  gender: string;
  birthDate: string;
  email: string;
  contactNumber: string;
  address: string;
  bloodType: string;
  heightCm: string;
  weightKg: string;
  ssn: string;
  age: string;
  profilePicture?: string;
}

const MIN_DOB = new Date();
MIN_DOB.setFullYear(MIN_DOB.getFullYear() - 120);
const TODAY_STR = new Date().toISOString().slice(0, 10);
const MIN_DOB_STR = MIN_DOB.toISOString().slice(0, 10);

const PersonalInfo = () => {
  const { t, lang } = useLang();
  const isAr = lang === "ar";
  const queryClient = useQueryClient();
  const { setPatientImage } = useProfile();

  const [isEditing, setIsEditing] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);

  const { data: profile, isLoading } = useQuery({
    queryKey: ["patient-profile"],
    queryFn: getPatientProfile,
  });

  const [formData, setFormData] = useState<ProfileData>({
    patientId: "", fullName: "", gender: "", birthDate: "", email: "",
    contactNumber: "", address: "", bloodType: "", heightCm: "", weightKg: "",
    ssn: "", age: "",
  });

  useEffect(() => {
    if (profile) {

      const resolvedPic = resolveImageUrl(profile.profilePicture);

      const data: ProfileData = {
        patientId: String(profile.patientId || ""),
        fullName: profile.fullName || "",
        gender: profile.gender || "",
        birthDate: profile.birthDate || "",
        email: profile.email || "",
        contactNumber: profile.contactNumber || "",
        address: profile.address || "",
        bloodType: profile.bloodType || "",
        heightCm: String(profile.heightCm || ""),
        weightKg: String(profile.weightKg || ""),
        ssn: profile.ssn || "",
        age: String(profile.age || ""),
        profilePicture: resolvedPic,
      };

      setFormData(data);

      if (resolvedPic) {
        setPatientImage(resolvedPic);
      }
    }
  }, [profile, setPatientImage]);

  const updateMutation = useMutation({
    mutationFn: async (data: ProfileData) => {
      const form = new FormData();
      form.append("FullName", data.fullName);
      form.append("Gender", data.gender);
      form.append("BirthDate", data.birthDate);
      form.append("Email", data.email);
      form.append("ContactNumber", data.contactNumber);
      form.append("Address", data.address);
      form.append("BloodType", data.bloodType);
      form.append("HeightCm", data.heightCm);
      form.append("WeightKg", data.weightKg);
      form.append("Ssn", data.ssn);
      if (imageFile) form.append("ProfilePicture", imageFile);
      return updatePatientProfile(form);
    },
    onSuccess: (response) => {
      // Update formData with the new profile picture returned from server
      const serverPic = resolveImageUrl(response?.profilePicture);
      if (serverPic) {
        setFormData(prev => ({ ...prev, profilePicture: serverPic }));
        setPatientImage(serverPic);
      } else if (previewUrl) {
        // Optimistic: use the local preview until next query refresh
        setFormData(prev => ({ ...prev, profilePicture: previewUrl }));
        setPatientImage(previewUrl);
      }
      queryClient.invalidateQueries({ queryKey: ["patient-profile"] });
      queryClient.invalidateQueries({ queryKey: ["patient-dashboard"] });
      toast.success(isAr ? "تم تحديث الملف الشخصي بنجاح" : "Profile updated successfully");
      setIsEditing(false);
      setShowModal(false);
      setImageFile(null);
      setPreviewUrl(null);
    },
    onError: () => toast.error(isAr ? "فشل تحديث الملف الشخصي" : "Failed to update profile"),
  });

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    if (!ACCEPTED_IMAGE_TYPES.includes(file.type)) {
      toast.error((t as any).imageTypeError || "Only JPEG, PNG, and WebP images are supported.");
      e.target.value = "";
      return;
    }
    setImageFile(file);
    // Use FileReader so JPEG/WebP renders correctly regardless of browser blob URL quirks
    const reader = new FileReader();
    reader.onload = (ev) => setPreviewUrl(ev.target?.result as string);
    reader.readAsDataURL(file);
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => setFormData({ ...formData, [e.target.name]: e.target.value });

  const displayValue = (key: string, value: string) => {
    if (!isAr) return value;
    if (KEEP_ORIGINAL.includes(key)) return value;
    if (key === "gender") return genderAr(value);
    if (key === "birthDate") {
      if (!value) return "";
      const d = new Date(value);
      const day = String(d.getDate()).padStart(2, "0");
      const mon = String(d.getMonth() + 1).padStart(2, "0");
      const yr = String(d.getFullYear());
      return `${toAr(day)}/${toAr(mon)}/${toAr(yr)}`;
    }
    return toAr(value);
  };

  const labels = (t as any).fieldLabels ?? {};

  const fieldDef: Array<{ key: keyof ProfileData; icon: string; type?: string }> = [
    { key: "fullName", icon: "fa-user" },
    { key: "gender", icon: "fa-venus-mars" },
    { key: "birthDate", icon: "fa-birthday-cake", type: "date" },
    { key: "email", icon: "fa-envelope", type: "email" },
    { key: "contactNumber", icon: "fa-phone" },
    { key: "address", icon: "fa-map-marker-alt" },
    { key: "bloodType", icon: "fa-tint" },
    { key: "heightCm", icon: "fa-ruler-vertical", type: "number" },
    { key: "weightKg", icon: "fa-weight", type: "number" },
    { key: "ssn", icon: "fa-id-card" },
  ];

  const readOnlyField = (key: string, icon: string, label: string, value: string) => (
    <div className="info-row" key={key}>
      <label className="label-with-icon">
        <span className="label-icon"><i className={`fa ${icon}`} /></span>
        {label}
      </label>
      <input type="text" value={displayValue(key, value)} readOnly dir={isAr && !KEEP_ORIGINAL.includes(key) ? "rtl" : "ltr"} />
    </div>
  );

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="w-8 h-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="profile-container" dir={isAr ? "rtl" : "ltr"}>

      <div className="top-section">
        <h2>{t.personalInfoTitle}</h2>
        <button className="edit-btn" onClick={() => { if (isEditing) setShowModal(true); else setIsEditing(true); }} disabled={updateMutation.isPending}>
          {updateMutation.isPending ? <Loader2 className="w-4 h-4 animate-spin" /> : (isEditing ? t.save : t.edit)}
        </button>
      </div>

      <div className="profile-content">
        {/* Image section */}
        <div className="profile-image">
          <img
            src={previewUrl ?? formData.profilePicture ?? defaultUser}
            alt={formData.fullName}
            onError={(e) => { (e.target as HTMLImageElement).src = defaultUser; }}
          />
          {isEditing && (
            <label className="upload-photo">
              {t.changePhoto}
              <input
                type="file"
                accept=".jpg,.jpeg,.png,.webp,image/jpeg,image/png,image/webp"
                onChange={handleImageChange}
                hidden
              />
            </label>
          )}
          {/* Always show patient ID + age in the image card */}
          {!isEditing && (
            <div style={{ textAlign: "center", marginTop: 4 }}>
              <p style={{ margin: 0, fontWeight: 700, fontSize: 15 }}>{formData.fullName}</p>
              <p style={{ margin: "2px 0 0", fontSize: 12, color: "hsl(var(--muted-foreground))" }}>
                {labels.patientId || "Patient ID"}: {isAr ? toAr(formData.patientId) : formData.patientId}
              </p>
              {formData.age && (
                <p style={{ margin: "2px 0 0", fontSize: 12, color: "hsl(var(--muted-foreground))" }}>
                  {labels.age || "Age"}: {isAr ? toAr(formData.age) : formData.age}
                </p>
              )}
            </div>
          )}
        </div>

        {/* Fields */}
        <div className="profile-info">
          {fieldDef.map(({ key, icon, type }) => {
            const label = labels[key] || key;
            const value = formData[key];

            if (!isEditing) {
              return readOnlyField(key, icon, label, value);
            }

            // Blood type: dropdown
            if (key === "bloodType") {
              return (
                <div className="info-row" key={key}>
                  <label className="label-with-icon"><span className="label-icon"><i className={`fa ${icon}`} /></span>{label}</label>
                  <select name="bloodType" value={value} onChange={handleChange} className="info-select">
                    <option value="">{(t as any).selectBloodType || "Select blood type"}</option>
                    {BLOOD_TYPES.map((bt) => <option key={bt} value={bt}>{bt}</option>)}
                  </select>
                </div>
              );
            }

            // Gender: dropdown
            if (key === "gender") {
              return (
                <div className="info-row" key={key}>
                  <label className="label-with-icon"><span className="label-icon"><i className={`fa ${icon}`} /></span>{label}</label>
                  <select name="gender" value={value} onChange={handleChange} className="info-select">
                    <option value="">{isAr ? "اختر الجنس" : "Select gender"}</option>
                    <option value="male">{isAr ? "ذكر" : "Male"}</option>
                    <option value="female">{isAr ? "أنثى" : "Female"}</option>
                  </select>
                </div>
              );
            }

            // Date of birth: shared DateInput (type + calendar icon)
            if (key === "birthDate") {
              const dobVal = value ? new Date(value) : undefined;
              return (
                <div className="info-row" key={key}>
                  <label className="label-with-icon"><span className="label-icon"><i className={`fa ${icon}`} /></span>{label}</label>
                  <DateInput
                    value={dobVal}
                    onChange={(d) => {
                      if (d) setFormData(prev => ({ ...prev, birthDate: d.toISOString().slice(0, 10) }));
                      else setFormData(prev => ({ ...prev, birthDate: "" }));
                    }}
                    minDate={MIN_DOB}
                    maxDate={new Date()}
                    fromYear={new Date().getFullYear() - 120}
                    toYear={new Date().getFullYear()}
                    placeholder="DD/MM/YYYY"
                    confirmLabel={isAr ? "تأكيد" : "Confirm"}
                  />
                </div>
              );
            }

            // Phone: numeric, 11-digit validation hint
            if (key === "contactNumber") {
              return (
                <div className="info-row" key={key}>
                  <label className="label-with-icon"><span className="label-icon"><i className={`fa ${icon}`} /></span>{label}</label>
                  <input
                    type="tel"
                    name={key}
                    value={value}
                    onChange={(e) => {
                      const v = e.target.value.replace(/\D/g, "").slice(0, 11);
                      setFormData({ ...formData, contactNumber: v });
                    }}
                    inputMode="numeric"
                    maxLength={11}
                    placeholder={isAr ? "01xxxxxxxxx" : "01xxxxxxxxx"}
                  />
                  {value && value.length !== 11 && (
                    <p style={{ color: "hsl(var(--destructive))", fontSize: 11, margin: "2px 0 0" }}>
                      {(t as any).phoneInvalid || "Phone number must be 11 digits."}
                    </p>
                  )}
                </div>
              );
            }

            // SSN: digits only, 14 chars
            if (key === "ssn") {
              return (
                <div className="info-row" key={key}>
                  <label className="label-with-icon"><span className="label-icon"><i className={`fa ${icon}`} /></span>{label}</label>
                  <input
                    type="text"
                    name={key}
                    value={value}
                    onChange={(e) => {
                      const v = e.target.value.replace(/\D/g, "").slice(0, 14);
                      setFormData({ ...formData, ssn: v });
                    }}
                    inputMode="numeric"
                    maxLength={14}
                    placeholder={isAr ? "14 رقمًا" : "14 digits"}
                  />
                </div>
              );
            }

            return (
              <div className="info-row" key={key}>
                <label className="label-with-icon"><span className="label-icon"><i className={`fa ${icon}`} /></span>{label}</label>
                <input
                  type={type === "date" || type === "number" ? type : "text"}
                  name={key}
                  value={value}
                  onChange={handleChange}
                  dir={isAr && !KEEP_ORIGINAL.includes(key) ? "rtl" : "ltr"}
                />
              </div>
            );
          })}
        </div>
      </div>

      {showModal && (
        <div className="modal-overlay">
          <div className="modal">
            <p>{t.saveChanges}</p>
            <button className="save-btn" onClick={() => updateMutation.mutate(formData)} disabled={updateMutation.isPending}>
              {updateMutation.isPending ? <Loader2 className="w-4 h-4 animate-spin" /> : t.yes}
            </button>
            <button className="cancel-btn" onClick={() => setShowModal(false)} disabled={updateMutation.isPending}>{t.no}</button>
          </div>
        </div>
      )}
    </div>
  );
};

export default PersonalInfo;
