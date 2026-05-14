import { useState } from "react";
import { useLanguage } from "@/contexts/LanguageContext";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  DialogClose,
} from "@/components/ui/dialog";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Search, Pencil } from "lucide-react";
import { motion, AnimatePresence } from "framer-motion";

interface Surgery {
  type: string;
  date: string;
  notes: string;
}

interface PatientSurgery {
  name: string;
  id: string;
  surgeries: Surgery[];
}

const mockData: PatientSurgery[] = [
  {
    name: "Ahmed Mohamed",
    id: "P123456",
    surgeries: [
      {
        type: "Appendectomy",
        date: "2023-11-15",
        notes:
          "Post-operative check scheduled in 7 days.",
      },
    ],
  },

  {
    name: "Fatima Ali",
    id: "P789012",
    surgeries: [
      {
        type: "Hernia Repair",
        date: "2023-11-20",
        notes: "Monitor for any swelling.",
      },
    ],
  },
];

export default function Surgeries() {
  const { t } = useLanguage();

  const [searchName, setSearchName] = useState("");

  const [dialogOpen, setDialogOpen] =
    useState(false);

  const [editTarget, setEditTarget] =
    useState<{
      patientIdx: number;
      surgeryIdx: number;
    } | null>(null);

  const [form, setForm] = useState({
    type: "",
    date: "",
    notes: "",
  });

  const [patients, setPatients] =
    useState<PatientSurgery[]>(mockData);

  const filtered = patients.filter((p) =>
    p.name
      .toLowerCase()
      .includes(searchName.toLowerCase())
  );

  const openAdd = () => {
    setEditTarget(null);

    setForm({
      type: "",
      date: "",
      notes: "",
    });

    setDialogOpen(true);
  };

  const openEdit = (
    pIdx: number,
    sIdx: number
  ) => {
    setEditTarget({
      patientIdx: pIdx,
      surgeryIdx: sIdx,
    });

    setForm({
      ...filtered[pIdx].surgeries[sIdx],
    });

    setDialogOpen(true);
  };

  const handleSave = () => {
    if (editTarget !== null) {
      const realPatient = patients.find(
        (p) =>
          p.id ===
          filtered[editTarget.patientIdx].id
      );

      if (realPatient) {
        realPatient.surgeries[
          editTarget.surgeryIdx
        ] = form;

        setPatients([...patients]);
      }
    }

    setDialogOpen(false);
  };

  const getInitials = (name: string) => {
    return name
      .split(" ")
      .map((w) => w[0])
      .join("")
      .toUpperCase()
      .slice(0, 2);
  };

  return (
    <>
      <div className="max-w-4xl mx-auto space-y-6">
        <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3">
          <h1 className="text-xl sm:text-2xl font-bold text-foreground font-display">
            {t.surgeriesPage}
          </h1>

          <div className="flex items-center gap-3 w-full sm:w-auto">
            <div className="relative flex-1 sm:w-56">
              <Search className="absolute start-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />

              <Input
                placeholder={t.searchPatient}
                value={searchName}
                onChange={(e) =>
                  setSearchName(e.target.value)
                }
                className="ps-9"
              />
            </div>

            <Button
              onClick={openAdd}
              className="gap-2 shrink-0"
            >
              {t.addSurgery}
            </Button>
          </div>
        </div>

        <AnimatePresence>
          {filtered.map((patient, pIdx) => (
            <motion.div
              key={patient.id}
              initial={{
                opacity: 0,
                y: 15,
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
                duration: 0.3,
                delay: pIdx * 0.08,
              }}
            >
              <Card className="overflow-hidden">
                <CardContent className="p-5 space-y-4">
                  <div className="flex items-center gap-3">
                    <div className="w-10 h-10 rounded-full bg-muted flex items-center justify-center text-sm font-semibold text-muted-foreground shrink-0">
                      {getInitials(patient.name)}
                    </div>

                    <div>
                      <p className="text-sm font-semibold text-foreground">
                        {t.patientName}: {patient.name}
                      </p>

                      <p className="text-xs text-muted-foreground">
                        {t.patientId}: {patient.id}
                      </p>
                    </div>
                  </div>

                  <div className="hidden sm:block">
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>
                            {t.surgeryType}
                          </TableHead>

                          <TableHead>
                            {t.date}
                          </TableHead>

                          <TableHead>
                            {t.notes}
                          </TableHead>
                        </TableRow>
                      </TableHeader>

                      <TableBody>
                        {patient.surgeries.map(
                          (s, sIdx) => (
                            <TableRow key={sIdx}>
                              <TableCell className="font-medium">
                                {s.type}
                              </TableCell>

                              <TableCell className="text-muted-foreground">
                                {s.date}
                              </TableCell>

                              <TableCell className="text-muted-foreground">
                                {s.notes}
                              </TableCell>
                            </TableRow>
                          )
                        )}
                      </TableBody>
                    </Table>
                  </div>

                  <div className="sm:hidden space-y-2">
                    {patient.surgeries.map(
                      (s, sIdx) => (
                        <div
                          key={sIdx}
                          className="bg-muted/40 rounded-lg p-3 space-y-1"
                        >
                          <p className="text-sm font-semibold text-foreground">
                            {s.type}
                          </p>

                          <p className="text-xs text-muted-foreground">
                            {s.date}
                          </p>

                          <p className="text-xs text-muted-foreground">
                            {s.notes}
                          </p>
                        </div>
                      )
                    )}
                  </div>

                  <div className="flex justify-end">
                    <Button
                      variant="outline"
                      size="sm"
                      className="gap-1.5"
                      onClick={() =>
                        openEdit(pIdx, 0)
                      }
                    >
                      <Pencil className="w-3.5 h-3.5" />

                      {t.edit}
                    </Button>
                  </div>
                </CardContent>
              </Card>
            </motion.div>
          ))}
        </AnimatePresence>
      </div>

      <Dialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
      >
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>
              {editTarget !== null
                ? t.editSurgery
                : t.addSurgery}
            </DialogTitle>
          </DialogHeader>

          <div className="space-y-4">
            <div className="space-y-2">
              <Label>{t.surgeryType}</Label>

              <Input
                placeholder={
                  t.surgeryTypePlaceholder
                }
                value={form.type}
                onChange={(e) =>
                  setForm({
                    ...form,
                    type: e.target.value,
                  })
                }
              />
            </div>

            <div className="space-y-2">
              <Label>{t.surgeryDate}</Label>

              <Input
                type="date"
                value={form.date}
                onChange={(e) =>
                  setForm({
                    ...form,
                    date: e.target.value,
                  })
                }
              />
            </div>

            <div className="space-y-2">
              <Label>{t.notes}</Label>

              <Textarea
                placeholder={t.notesPlaceholder}
                value={form.notes}
                onChange={(e) =>
                  setForm({
                    ...form,
                    notes: e.target.value,
                  })
                }
                rows={3}
              />
            </div>
          </div>

          <DialogFooter className="gap-2">
            <DialogClose asChild>
              <Button variant="outline">
                {t.cancel}
              </Button>
            </DialogClose>

            <Button onClick={handleSave}>
              {editTarget !== null
                ? t.saveProfile
                : t.add}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  );
}