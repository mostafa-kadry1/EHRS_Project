import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Search, Upload, ArrowLeft } from "lucide-react";
import { useLanguage } from "@/contexts/LanguageContext";

export default function NewMedicalRecord() {
  const navigate = useNavigate();

  const { t } = useLanguage();

  const [prescriptionFile, setPrescriptionFile] = useState("");

  const [radiologyFile, setRadiologyFile] = useState("");

  const idFields = [
    {
      label: t.patientId,
      id: "patientId",
      placeholder: t.searchPatientId,
    },

    {
      label: t.appointmentId,
      id: "appointmentId",
      placeholder: t.searchAppointmentId,
    },
  ];

  const textFields = [
    {
      label: t.chiefComplaint,
      id: "complaint",
      placeholder: t.describeComplaint,
    },

    {
      label: t.diagnosis,
      id: "diagnosis",
      placeholder: t.enterDiagnosis,
    },

    {
      label: t.clinicalNotes,
      id: "notes",
      placeholder: t.addClinicalNotes,
    },

    {
      label: t.treatment,
      id: "treatment",
      placeholder: t.describeTreatment,
    },
  ];

  return (
    <>
      <div className="mb-6">
        <button
          onClick={() => navigate("/doctor/medical-records")}
          className="flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground mb-4 transition-colors"
        >
          <ArrowLeft className="w-4 h-4 rtl:rotate-180" />

          {t.backToMedicalRecords}
        </button>

        <h1 className="text-xl md:text-2xl font-bold text-foreground">
          {t.newMedicalRecord}
        </h1>

        <p className="text-sm text-muted-foreground mt-1">
          {t.fillDetails}
        </p>
      </div>

      <div className="max-w-3xl mx-auto">
        <div className="bg-card rounded-xl border border-border p-4 md:p-6 space-y-5 md:space-y-6">
          <div>
            <h3 className="text-sm font-semibold text-foreground mb-4 pb-2 border-b border-border">
              {t.referenceIds}
            </h3>

            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
              {idFields.map((field) => (
                <div key={field.id}>
                  <label className="block text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-1.5">
                    {field.label}
                  </label>

                  <div className="relative">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground rtl:left-auto rtl:right-3" />

                    <input
                      type="text"
                      id={field.id}
                      placeholder={field.placeholder}
                      className="w-full pl-9 pr-3 py-2.5 text-sm bg-background border border-border rounded-lg focus:outline-none focus:ring-2 focus:ring-ring transition-all placeholder:text-muted-foreground rtl:pl-3 rtl:pr-9"
                    />
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div>
            <h3 className="text-sm font-semibold text-foreground mb-4 pb-2 border-b border-border">
              {t.medicalInformation}
            </h3>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {textFields.map((field) => (
                <div
                  key={field.id}
                  className={field.id === "notes" ? "md:col-span-2" : ""}
                >
                  <label className="block text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-1.5">
                    {field.label}
                  </label>

                  <textarea
                    id={field.id}
                    placeholder={field.placeholder}
                    rows={field.id === "notes" ? 4 : 3}
                    className="w-full px-3 py-2.5 text-sm bg-background border border-border rounded-lg focus:outline-none focus:ring-2 focus:ring-ring transition-all placeholder:text-muted-foreground resize-none"
                  />
                </div>
              ))}
            </div>
          </div>

          <div>
            <h3 className="text-sm font-semibold text-foreground mb-4 pb-2 border-b border-border">
              {t.attachments}
            </h3>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label className="block text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-1.5">
                  {t.prescription}
                </label>

                <div className="border-2 border-dashed border-border rounded-lg p-4 text-center hover:border-primary/50 transition-colors">
                  <Upload className="w-6 h-6 text-muted-foreground mx-auto mb-2" />

                  <p className="text-xs text-muted-foreground mb-2">
                    {prescriptionFile || t.noFileChosen}
                  </p>

                  <label className="cursor-pointer">
                    <span className="text-xs font-semibold text-primary hover:underline">
                      {t.chooseFile}
                    </span>

                    <input
                      type="file"
                      className="hidden"
                      onChange={(e) =>
                        setPrescriptionFile(
                          e.target.files?.[0]?.name || ""
                        )
                      }
                    />
                  </label>
                </div>
              </div>

              <div>
                <label className="block text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-1.5">
                  {t.radiology}
                </label>

                <div className="border-2 border-dashed border-border rounded-lg p-4 text-center hover:border-primary/50 transition-colors">
                  <Upload className="w-6 h-6 text-muted-foreground mx-auto mb-2" />

                  <p className="text-xs text-muted-foreground mb-2">
                    {radiologyFile || t.noFileChosen}
                  </p>

                  <label className="cursor-pointer">
                    <span className="text-xs font-semibold text-primary hover:underline">
                      {t.chooseFile}
                    </span>

                    <input
                      type="file"
                      className="hidden"
                      onChange={(e) =>
                        setRadiologyFile(
                          e.target.files?.[0]?.name || ""
                        )
                      }
                    />
                  </label>
                </div>
              </div>
            </div>
          </div>

          <div className="flex flex-col-reverse sm:flex-row items-center justify-end gap-3 pt-2 border-t border-border">
            <button
              onClick={() => navigate("/doctor/medical-records")}
              className="px-5 py-2.5 text-sm font-semibold text-muted-foreground bg-muted rounded-lg hover:bg-secondary transition-colors w-full sm:w-auto"
            >
              {t.cancel}
            </button>

            <button className="px-6 py-2.5 text-sm font-semibold bg-primary text-primary-foreground rounded-lg hover:bg-primary-glow transition-colors shadow-sm flex items-center justify-center gap-2 w-full sm:w-auto">
              {t.addRecord}
            </button>
          </div>
        </div>
      </div>
    </>
  );
}