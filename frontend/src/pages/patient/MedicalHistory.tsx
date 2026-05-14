import React, { useEffect, useState } from "react";
import "./medicalhistory.css";
import { FaCalendar } from "react-icons/fa";
import defaultDoctor from "@/assets/patient_images/doctor.jpg";
import { useLanguage as useLang } from "@/contexts/LanguageContext";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getPatientMedicalHistory, updatePatientMedicalHistory } from "@/api/medical.api";
import { Loader2 } from "lucide-react";
import { toast } from "sonner";
import { useAuth } from "@/contexts/AuthContext";
import { useProfile } from "@/contexts/ProfileContext";
import { getPatientDashboard } from "@/api/patient.api";
import PatientPageHeader from "@/components/patient/PatientPageHeader";
import EmptyState from "@/components/patient/EmptyState";

const chronicDiseasesList = [
  "Diabetes - السكري","Hypertension - ارتفاع ضغط الدم","Heart Disease - أمراض القلب",
  "Asthma - الربو","Cancer - السرطان","Depression - الاكتئاب",
  "Coronary Artery Disease - مرض الشرايين التاجية","Heart Failure - فشل القلب",
  "Chronic Obstructive Pulmonary Disease - الانسداد الرئوي المزمن",
  "Chronic Kidney Disease - أمراض الكلى المزمنة","Chronic Liver Disease - أمراض الكبد المزمنة",
  "Liver Cirrhosis - تليف الكبد","Epilepsy - الصرع",
  "Thyroid Disorder - اضطرابات الغدة الدرقية","Hyperthyroidism - فرط نشاط الغدة الدرقية",
  "Hypothyroidism - قصور الغدة الدرقية","Hyperlipidemia - ارتفاع الكوليسترول",
  "Rheumatoid Arthritis - التهاب المفاصل الروماتويدي","Osteoporosis - هشاشة العظام",
  "Anxiety Disorder - اضطراب القلق","Systemic Lupus Erythematosus - الذئبة الحمراء",
  "Chronic Anemia - فقر الدم المزمن","Parkinson's Disease - مرض باركنسون",
  "Alzheimer's Disease - مرض الزهايمر","Other - أخرى",
];

const allergiesList = [
  "Penicillin - البنسلين","Aspirin - الأسبرين","Dust - الغبار","Milk - الحليب",
  "Peanuts - الفول السوداني","Perfume - العطور",
  "Antibiotics Allergy - حساسية المضادات الحيوية","Ibuprofen Allergy - حساسية الإيبوبروفين",
  "Sulfa Drugs Allergy - حساسية أدوية السلفا","Tree Nuts Allergy - حساسية المكسرات الشجرية",
  "Egg Allergy - حساسية البيض","Fish Allergy - حساسية السمك",
  "Shellfish Allergy - حساسية المحار","Wheat Allergy - حساسية القمح",
  "Soy Allergy - حساسية الصويا","Pollen Allergy - حساسية حبوب اللقاح",
  "Pet Dander Allergy - حساسية وبر الحيوانات","Mold Allergy - حساسية العفن",
  "Latex Allergy - حساسية اللاتكس","Insect Sting Allergy - حساسية لسعات الحشرات",
  "Sun Allergy - حساسية الشمس","Other - أخرى",
];

interface MedicalHistoryData {
  chronicDiseases: string[];
  allergies: string[];
  surgeries: any[];
}

export default function MedicalHistory() {
  const { t, lang } = useLang();
  const isAr = lang === "ar";
  const { user } = useAuth();
  const { patientImage } = useProfile();
  const queryClient = useQueryClient();

  const [selectedDiseases, setSelectedDiseases] = useState<string[]>([]);
  const [selectedAllergies, setSelectedAllergies] = useState<string[]>([]);
  const [showModal, setShowModal] = useState(false);
  const [pendingDelete, setPendingDelete] = useState<{ type: "disease" | "allergy"; value: string } | null>(null);

  const { data: dashboard } = useQuery({
    queryKey: ["patient-dashboard"],
    queryFn: getPatientDashboard,
    staleTime: 60_000,
  });

  const { data: history, isLoading } = useQuery({
    queryKey: ["patient-medical-history"],
    queryFn: getPatientMedicalHistory,
  });

  useEffect(() => {
    if (history) {
      setSelectedDiseases(history.chronicDiseases || []);
      setSelectedAllergies(history.allergies || []);
    }
  }, [history]);

  const updateMutation = useMutation({
    mutationFn: (updatedData: Partial<MedicalHistoryData>) => updatePatientMedicalHistory(updatedData),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["patient-medical-history"] });
      toast.success(isAr ? "تم تحديث السجل الطبي" : "Medical history updated");
    },
    onError: () => toast.error(isAr ? "فشل التحديث" : "Failed to update medical history"),
  });

  const handleSelect = (type: "disease" | "allergy") => (e: React.ChangeEvent<HTMLSelectElement>) => {
    const value = e.target.value;
    if (!value) return;
    if (type === "disease") {
      if (!selectedDiseases.includes(value)) {
        const newList = [...selectedDiseases, value];
        setSelectedDiseases(newList);
        updateMutation.mutate({ chronicDiseases: newList });
      }
    } else {
      if (!selectedAllergies.includes(value)) {
        const newList = [...selectedAllergies, value];
        setSelectedAllergies(newList);
        updateMutation.mutate({ allergies: newList });
      }
    }
    e.target.value = "";
  };

  const confirmRemove = (type: "disease" | "allergy", value: string) => {
    setPendingDelete({ type, value });
    setShowModal(true);
  };

  const handleConfirmDelete = () => {
    if (!pendingDelete) return;
    const { type, value } = pendingDelete;
    if (type === "disease") {
      const newList = selectedDiseases.filter((i) => i !== value);
      setSelectedDiseases(newList);
      updateMutation.mutate({ chronicDiseases: newList });
    } else {
      const newList = selectedAllergies.filter((i) => i !== value);
      setSelectedAllergies(newList);
      updateMutation.mutate({ allergies: newList });
    }
    setShowModal(false);
    setPendingDelete(null);
  };

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="w-8 h-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="mh-container" dir={isAr ? "rtl" : "ltr"}>

        {/* ── Unified header — same as every other patient page ── */}
        <PatientPageHeader
          title={t.medicalHistoryTitle}
          name={dashboard?.fullName || user?.fullName || ""}
          patientId={dashboard?.patientId ?? ""}
          imageUrl={dashboard?.profilePicture || patientImage}
          isAr={isAr}
          patientIdLabel={t.patientId}
        />

        {/* Diseases */}
        <div className="card">
          <h4>{t.chronicDiseases}</h4>
          <select onChange={handleSelect("disease")} defaultValue="">
            <option value="" disabled>{t.selectDisease}</option>
            {chronicDiseasesList.map((d, i) => <option key={i} value={d}>{d}</option>)}
          </select>
          <div className="tags">
            {selectedDiseases.map((d, i) => (
              <span key={i} className="tag">
                {d}
                <button className="tag-remove" onClick={() => confirmRemove("disease", d)}>×</button>
              </span>
            ))}
          </div>
        </div>

        {/* Allergies */}
        <div className="card">
          <h4>{t.allergies}</h4>
          <select onChange={handleSelect("allergy")} defaultValue="">
            <option value="" disabled>{t.selectAllergy}</option>
            {allergiesList.map((a, i) => <option key={i} value={a}>{a}</option>)}
          </select>
          <div className="tags">
            {selectedAllergies.map((a, i) => (
              <span key={i} className="tag">
                {a}
                <button className="tag-remove" onClick={() => confirmRemove("allergy", a)}>×</button>
              </span>
            ))}
          </div>
        </div>

        {/* Confirm delete modal */}
        {showModal && (
          <div className="modal-overlay">
            <div className="modal">
              <h3>{isAr ? "تأكيد الحذف" : "Confirm Delete"}</h3>
              <p>{isAr ? "هل أنت متأكد من أنك تريد حذف هذا العنصر؟" : "Are you sure you want to delete this item?"}</p>
              <div className="modal-actions">
                <button className="cancel-btn" onClick={() => { setShowModal(false); setPendingDelete(null); }}>
                  {isAr ? "إلغاء" : "Cancel"}
                </button>
                <button className="delete-btn" onClick={handleConfirmDelete}>
                  {isAr ? "حذف" : "Delete"}
                </button>
              </div>
            </div>
          </div>
        )}

        {/* Surgeries */}
        <div className="card surgery">
          <h4>{t.surgeries}</h4>

          {history?.surgeries && history.surgeries.length > 0 ? (
            history.surgeries.map((s: any, i: number) => (
              <div key={i} className="inner-card mb-4">
                <div className="surgery-doctor">
                  <img
                    src={s.doctorProfilePicture || defaultDoctor}
                    alt={s.doctorName}
                    onError={(e) => { (e.target as HTMLImageElement).src = defaultDoctor; }}
                  />
                  <div>
                    <h5>{s.doctorName}</h5>
                    <span>{s.doctorSpecialization}</span>
                  </div>
                </div>
                <h5>{s.surgeryName}</h5>
                <p className="date">
                  <FaCalendar />{" "}
                  {isAr
                    ? `تاريخ العملية: ${new Date(s.surgeryDate).toLocaleDateString("ar-EG")}`
                    : `Surgery Date: ${new Date(s.surgeryDate).toLocaleDateString()}`}
                </p>
                <p className="notes-title">{isAr ? "ملاحظات العملية" : "Surgery Notes"}</p>
                <p>{s.notes}</p>
              </div>
            ))
          ) : (
            // Professional empty state with surgery icon
            <EmptyState
              icon="🏥"
              message={isAr ? "لا توجد عمليات جراحية مسجلة" : "No surgeries recorded"}
            />
          )}
        </div>

    </div>
  );
}
