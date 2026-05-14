import { useState } from "react";
import { useLanguage } from "@/contexts/LanguageContext";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from "@/components/ui/tabs";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Dialog, DialogContent } from "@/components/ui/dialog";
import { Search, Image, AlertTriangle } from "lucide-react";
import { motion, AnimatePresence } from "framer-motion";
import {
  Alert,
  AlertDescription,
} from "@/components/ui/alert";

interface MedicalRecord {
  date: string;
  diagnosis: string;
  treatment: string;
  doctor: string;
  prescription?: { date: string } | null;
  radiology?: { date: string; notes: string } | null;
}

interface PatientData {
  name: string;
  nationalId: string;
  patientId: string;
  bloodType: string;
  age: number;
  weight: number;
  height: number;
  chronicDiseases: string[];
  allergies: string[];
  medicalHistory: MedicalRecord[];
  surgicalHistory: {
    operation: string;
    doctor: string;
    date: string;
    notes: string;
  }[];
}

const mockPatients: Record<string, PatientData> = {
  "29901011234567": {
    name: "Jana MO",
    nationalId: "29901011234567",
    patientId: "PT-1042",
    bloodType: "A+",
    age: 27,
    weight: 65,
    height: 170,
    chronicDiseases: ["Hypertension", "Type 2 diabetes"],
    allergies: ["Penicillin"],
    medicalHistory: [
      {
        date: "2025-12-01",
        diagnosis: "Upper respiratory infection",
        treatment: "Antibiotics course",
        doctor: "Dr. Ahmed",
        prescription: { date: "2025-12-01" },
        radiology: {
          date: "Dec 1, 2025",
          notes: "Chest X-ray clear.",
        },
      },
      {
        date: "2025-08-15",
        diagnosis: "Hypertension follow-up",
        treatment: "Medication adjustment",
        doctor: "Dr. Sara",
        prescription: { date: "2025-08-15" },
        radiology: null,
      },
    ],
    surgicalHistory: [
      {
        operation: "Appendectomy",
        doctor: "Dr. Ahmed",
        date: "2023",
        notes: "Post-operative check scheduled in 7 days.",
      },
      {
        operation: "Knee Surgery",
        doctor: "Dr. Omar",
        date: "2020",
        notes: "Physical therapy recommended for 6 weeks.",
      },
    ],
  },

  "30005151234567": {
    name: "Ahmed Ali",
    nationalId: "30005151234567",
    patientId: "PT-2087",
    bloodType: "O-",
    age: 26,
    weight: 80,
    height: 175,
    chronicDiseases: ["Asthma"],
    allergies: ["Aspirin", "Sulfa drugs"],
    medicalHistory: [
      {
        date: "2025-11-20",
        diagnosis: "Asthma exacerbation",
        treatment: "Inhaler + steroids",
        doctor: "Dr. Fatima",
        prescription: { date: "2025-11-20" },
        radiology: null,
      },
    ],
    surgicalHistory: [
      {
        operation: "Tonsillectomy",
        doctor: "Dr. Hassan",
        date: "2019",
        notes: "Recovery was smooth, no complications.",
      },
    ],
  },
};

export default function SearchPatient() {
  const { t } = useLanguage();

  const [searchId, setSearchId] = useState("");

  const [patient, setPatient] = useState<PatientData | null>(null);

  const [notFound, setNotFound] = useState(false);

  const [validationError, setValidationError] = useState("");

  const [selectedRecord, setSelectedRecord] =
    useState<MedicalRecord | null>(null);

  const handleSearch = () => {
    const trimmed = searchId.trim();

    setValidationError("");

    setNotFound(false);

    if (!/^\d*$/.test(trimmed)) {
      setValidationError(t.idInvalid);

      setPatient(null);

      return;
    }

    if (trimmed.length < 14) {
      setValidationError(t.idTooShort);

      setPatient(null);

      return;
    }

    if (trimmed.length > 14) {
      setValidationError(t.idTooLong);

      setPatient(null);

      return;
    }

    const found = mockPatients[trimmed];

    if (found) {
      setPatient(found);
    } else {
      setPatient(null);

      setNotFound(true);
    }
  };

  const calculateBMI = (
    weight: number,
    heightCm: number
  ) => {
    const heightM = heightCm / 100;

    return (
      weight /
      (heightM * heightM)
    ).toFixed(1);
  };

  return (
    <>
      <div className="max-w-4xl mx-auto space-y-6">
        <h1 className="text-xl sm:text-2xl font-bold text-foreground font-display">
          {t.searchPatientPage}
        </h1>

        <Tabs
          defaultValue="medical"
          className="w-full"
        >
          <TabsList className="w-full grid grid-cols-2 h-14 bg-transparent gap-3 p-0">
            <TabsTrigger
              value="medical"
              className="h-full text-base font-bold rounded-lg border border-border data-[state=active]:bg-[hsl(var(--sidebar-background))] data-[state=active]:text-white data-[state=active]:border-transparent data-[state=active]:shadow-none bg-background text-foreground"
            >
              {t.medicalHistoryTab}
            </TabsTrigger>

            <TabsTrigger
              value="surgery"
              className="h-full text-base font-bold rounded-lg border border-border data-[state=active]:bg-[hsl(var(--sidebar-background))] data-[state=active]:text-white data-[state=active]:border-transparent data-[state=active]:shadow-none bg-background text-foreground"
            >
              {t.surgeryTab}
            </TabsTrigger>
          </TabsList>

          <div className="flex gap-2 mt-5">
            <div className="flex-1 flex border border-border rounded-lg overflow-hidden bg-background">
              <div className="flex items-center px-3 border-e border-border bg-muted/30">
                <span className="text-sm text-muted-foreground whitespace-nowrap">
                  {t.enterPatientId}
                </span>
              </div>

              <Input
                placeholder={t.enterPatientId}
                value={searchId}
                onChange={(e) =>
                  setSearchId(e.target.value)
                }
                onKeyDown={(e) =>
                  e.key === "Enter" &&
                  handleSearch()
                }
                className="border-0 focus-visible:ring-0 focus-visible:ring-offset-0"
              />
            </div>

            <Button
              onClick={handleSearch}
              className="gap-2 shrink-0 h-auto px-5"
            >
              <Search className="w-4 h-4" />

              {t.searchBtn}
            </Button>
          </div>

          {validationError && (
            <Alert
              variant="destructive"
              className="mt-3"
            >
              <AlertTriangle className="h-4 w-4" />

              <AlertDescription>
                {validationError}
              </AlertDescription>
            </Alert>
          )}

          {notFound && (
            <p className="text-sm text-destructive mt-3">
              {t.patientNotFound}
            </p>
          )}

          <AnimatePresence mode="wait">
            {patient && (
              <motion.div
                key={patient.patientId}
                initial={{
                  opacity: 0,
                  y: 20,
                }}
                animate={{
                  opacity: 1,
                  y: 0,
                }}
                exit={{
                  opacity: 0,
                  y: -10,
                }}
                transition={{
                  duration: 0.4,
                  ease: "easeOut",
                }}
              >
                <motion.div
                  initial={{
                    opacity: 0,
                    scale: 0.96,
                  }}
                  animate={{
                    opacity: 1,
                    scale: 1,
                  }}
                  transition={{
                    duration: 0.35,
                    delay: 0.1,
                  }}
                >
                  <Card className="mt-4 border-border">
                    <CardContent className="p-5 space-y-1.5">
                      <h2 className="text-lg font-bold text-foreground">
                        {patient.name}
                      </h2>

                      <p className="text-sm text-muted-foreground">
                        {t.patientId}:{" "}
                        <span className="text-xs font-mono text-muted-foreground bg-muted px-2 py-1 rounded">
                          {patient.patientId}
                        </span>
                      </p>

                      <p className="text-sm text-muted-foreground">
                        {t.bloodType}:{" "}
                        <span className="text-foreground">
                          {patient.bloodType}
                        </span>
                      </p>

                      <div className="grid grid-cols-2 sm:grid-cols-4 gap-3 pt-2">
                        <div className="bg-muted/40 rounded-lg p-2.5 text-center">
                          <p className="text-xs text-muted-foreground">
                            {t.age}
                          </p>

                          <p className="text-sm font-semibold text-foreground">
                            {patient.age} {t.years}
                          </p>
                        </div>

                        <div className="bg-muted/40 rounded-lg p-2.5 text-center">
                          <p className="text-xs text-muted-foreground">
                            {t.weight}
                          </p>

                          <p className="text-sm font-semibold text-foreground">
                            {patient.weight} {t.kg}
                          </p>
                        </div>

                        <div className="bg-muted/40 rounded-lg p-2.5 text-center">
                          <p className="text-xs text-muted-foreground">
                            {t.height}
                          </p>

                          <p className="text-sm font-semibold text-foreground">
                            {patient.height} {t.cm}
                          </p>
                        </div>

                        <div className="bg-muted/40 rounded-lg p-2.5 text-center">
                          <p className="text-xs text-muted-foreground">
                            {t.bmi}
                          </p>

                          <p className="text-sm font-semibold text-foreground">
                            {calculateBMI(
                              patient.weight,
                              patient.height
                            )}
                          </p>
                        </div>
                      </div>

                      <p className="text-sm">
                        <span className="font-semibold text-foreground">
                          {t.chronicDiseases}:
                        </span>{" "}
                        <span className="text-muted-foreground">
                          {patient.chronicDiseases.join(
                            " , "
                          )}
                        </span>
                      </p>

                      <p className="text-sm">
                        <span className="font-semibold text-foreground">
                          {t.allergiesLabel}:
                        </span>{" "}
                        <span className="text-muted-foreground">
                          {patient.allergies.join(", ")}
                        </span>
                      </p>
                    </CardContent>
                  </Card>
                </motion.div>
              </motion.div>
            )}
          </AnimatePresence>
        </Tabs>
      </div>

      <Dialog
        open={!!selectedRecord}
        onOpenChange={(open) =>
          !open &&
          setSelectedRecord(null)
        }
      >
        <DialogContent className="sm:max-w-lg max-h-[85vh] overflow-y-auto">
          {selectedRecord && (
            <div className="space-y-5">
              <div>
                <p className="text-lg font-bold text-foreground">
                  {t.patientName}: {patient?.name}
                </p>

                <p className="text-sm text-muted-foreground">
                  {t.date}: {selectedRecord.date}
                </p>
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>
    </>
  );
}