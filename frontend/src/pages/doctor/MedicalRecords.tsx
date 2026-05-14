import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Plus, FileText, Search, Image } from "lucide-react";
import { useLanguage } from "@/contexts/LanguageContext";
import { Dialog, DialogContent } from "@/components/ui/dialog";

const records = [
  {
    patient: "Linda Milton",
    date: "19 Oct 2025",
    diagnosis: "Hypertension",
    treatment: "Amlodipine 5mg",
    prescription: { date: "24 Apr 2024" },
    radiology: {
      date: "April 23, 2024",
      notes: "Chest X-ray review completed.",
    },
  },
  {
    patient: "Patrick Jane",
    date: "20 Oct 2025",
    diagnosis: "Asthma",
    treatment: "Albuterol Inhaler",
    prescription: { date: "12 May 2024" },
    radiology: null,
  },
  {
    patient: "Alice Morgan",
    date: "21 Oct 2025",
    diagnosis: "Type 2 Diabetes",
    treatment: "Metformin 500mg",
    prescription: { date: "03 Jun 2024" },
    radiology: {
      date: "June 1, 2024",
      notes: "Abdominal ultrasound normal.",
    },
  },
  {
    patient: "David Clark",
    date: "22 Oct 2025",
    diagnosis: "Migraine",
    treatment: "Sumatriptan",
    prescription: null,
    radiology: null,
  },
  {
    patient: "Grace Turner",
    date: "23 Oct 2025",
    diagnosis: "Hypothyroidism",
    treatment: "Levothyroxine",
    prescription: { date: "15 Jul 2024" },
    radiology: null,
  },
];

type Record = typeof records[number];

export default function MedicalRecords() {
  const navigate = useNavigate();
  const { t } = useLanguage();

  const [search, setSearch] = useState("");
  const [selectedRecord, setSelectedRecord] = useState<Record | null>(null);

  const filtered = records.filter((r) =>
    r.patient.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <>
      <div className="mb-6 flex flex-col sm:flex-row sm:items-center justify-between gap-3">
        <div>
          <h1 className="text-xl md:text-2xl font-bold text-foreground">
            {t.medicalRecords}
          </h1>

          <p className="text-sm text-muted-foreground mt-1">
            {t.generalDocuments}
          </p>
        </div>

        <button
          onClick={() => navigate("/doctor/medical-records/new")}
          className="flex items-center justify-center gap-2 bg-primary text-primary-foreground px-4 py-2 rounded-lg text-sm font-semibold hover:bg-primary-glow transition-colors shadow-sm w-full sm:w-auto"
        >
          <Plus className="w-4 h-4" />

          {t.addRecord}
        </button>
      </div>

      <div className="bg-card rounded-xl border border-border overflow-hidden">
        <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between px-4 md:px-6 py-3 md:py-4 border-b border-border gap-3">
          <div className="flex items-center gap-2">
            <FileText className="w-4 h-4 text-primary" />

            <h2 className="text-sm md:text-base font-semibold text-foreground">
              {t.patientRecords}
            </h2>
          </div>

          <div className="relative w-full max-w-xs">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground rtl:left-auto rtl:right-3" />

            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder={t.searchPatientRecord}
              className="w-full pl-9 pr-3 py-2 text-sm bg-muted rounded-lg border border-border focus:outline-none focus:ring-2 focus:ring-ring focus:border-transparent transition-all placeholder:text-muted-foreground rtl:pl-3 rtl:pr-9"
            />
          </div>
        </div>

        <div className="hidden md:block overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="bg-muted/40">
                <th className="text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider px-6 py-3 rtl:text-right">
                  {t.patient}
                </th>

                <th className="text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider px-6 py-3 rtl:text-right">
                  {t.date}
                </th>

                <th className="text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider px-6 py-3 rtl:text-right">
                  {t.diagnosis}
                </th>

                <th className="text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider px-6 py-3 rtl:text-right">
                  {t.treatment}
                </th>

                <th className="text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider px-6 py-3 rtl:text-right">
                  {t.action}
                </th>
              </tr>
            </thead>

            <tbody className="divide-y divide-border">
              {filtered.map((rec, i) => (
                <tr
                  key={i}
                  className="hover:bg-muted/30 transition-colors"
                >
                  <td className="px-6 py-4">
                    <span className="text-sm font-medium text-foreground">
                      {rec.patient}
                    </span>
                  </td>

                  <td className="px-6 py-4 text-sm text-muted-foreground">
                    {rec.date}
                  </td>

                  <td className="px-6 py-4">
                    <span className="text-sm font-medium text-foreground">
                      {rec.diagnosis}
                    </span>
                  </td>

                  <td className="px-6 py-4 text-sm text-muted-foreground">
                    {rec.treatment}
                  </td>

                  <td className="px-6 py-4">
                    <button
                      onClick={() => setSelectedRecord(rec)}
                      className="text-xs text-primary font-semibold hover:underline"
                    >
                      {t.view}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <div className="md:hidden divide-y divide-border">
          {filtered.map((rec, i) => (
            <div key={i} className="px-4 py-3">
              <div className="flex items-center gap-3">
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-foreground truncate">
                    {rec.patient}
                  </p>

                  <p className="text-xs text-muted-foreground">
                    {rec.date}
                  </p>
                </div>

                <button
                  onClick={() => setSelectedRecord(rec)}
                  className="text-xs text-primary font-semibold"
                >
                  {t.view}
                </button>
              </div>

              <div className="mt-2 ps-12 space-y-0.5">
                <p className="text-xs">
                  <span className="text-muted-foreground">
                    {t.diagnosis}:
                  </span>{" "}
                  <span className="font-medium text-foreground">
                    {rec.diagnosis}
                  </span>
                </p>

                <p className="text-xs text-muted-foreground">
                  {t.treatment}: {rec.treatment}
                </p>
              </div>
            </div>
          ))}
        </div>
      </div>

      <Dialog
        open={!!selectedRecord}
        onOpenChange={(open) => !open && setSelectedRecord(null)}
      >
        <DialogContent className="sm:max-w-lg max-h-[85vh] overflow-y-auto">
          {selectedRecord && (
            <div className="space-y-5">
              <div>
                <p className="text-lg font-bold text-foreground">
                  {t.patientName}: {selectedRecord.patient}
                </p>

                <p className="text-sm text-muted-foreground">
                  {t.date}: {selectedRecord.date}
                </p>
              </div>

              <div>
                <div className="bg-primary/10 rounded-md px-3 py-1.5 mb-1.5">
                  <h3 className="text-sm font-bold text-foreground">
                    {t.diagnosis}
                  </h3>
                </div>

                <p className="text-sm font-semibold text-foreground px-1">
                  {selectedRecord.diagnosis}
                </p>
              </div>

              <div>
                <div className="bg-primary/10 rounded-md px-3 py-1.5 mb-1.5">
                  <h3 className="text-sm font-bold text-foreground">
                    {t.treatment}
                  </h3>
                </div>

                <p className="text-sm font-semibold text-foreground px-1">
                  {selectedRecord.treatment}
                </p>
              </div>

              <div>
                <h3 className="text-sm font-bold text-primary mb-2">
                  {t.prescription}
                </h3>

                {selectedRecord.prescription ? (
                  <div className="border border-border rounded-lg p-3">
                    <span className="inline-block bg-primary/10 text-primary text-xs font-medium px-2.5 py-1 rounded-full mb-2">
                      {selectedRecord.prescription.date}
                    </span>

                    <div className="bg-muted rounded-lg h-28 flex items-center justify-center">
                      <div className="text-center text-muted-foreground">
                        <Image className="w-8 h-8 mx-auto mb-1" />

                        <span className="text-sm font-medium">
                          Image
                        </span>
                      </div>
                    </div>
                  </div>
                ) : (
                  <p className="text-sm text-muted-foreground italic px-1">
                    {t.noFileChosen}
                  </p>
                )}
              </div>

              <div>
                <h3 className="text-sm font-bold text-primary mb-2">
                  {t.radiology}
                </h3>

                {selectedRecord.radiology ? (
                  <div className="border border-border rounded-lg p-3 flex gap-3">
                    <div className="w-28 h-28 bg-muted rounded-lg flex items-center justify-center shrink-0">
                      <div className="text-center text-muted-foreground">
                        <Image className="w-8 h-8 mx-auto mb-1" />

                        <span className="text-xs">X-Ray</span>
                      </div>
                    </div>

                    <div className="flex-1 min-w-0">
                      <div className="flex items-center justify-between mb-1">
                        <h4 className="text-sm font-bold text-foreground">
                          {t.clinicalNotes}
                        </h4>

                        <span className="text-xs text-muted-foreground">
                          {selectedRecord.radiology.date}
                        </span>
                      </div>

                      <p className="text-sm text-muted-foreground">
                        {selectedRecord.radiology.notes}
                      </p>
                    </div>
                  </div>
                ) : (
                  <p className="text-sm text-muted-foreground italic px-1">
                    {t.noFileChosen}
                  </p>
                )}
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>
    </>
  );
}