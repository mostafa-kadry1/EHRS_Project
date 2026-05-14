import React, { useState } from "react";
import "./Imaging.css";
import { FaSearch } from "react-icons/fa";
import { useLanguage as useLang } from "@/contexts/LanguageContext";
import { useProfile } from "@/contexts/ProfileContext";
import { useAuth } from "@/contexts/AuthContext";
import { useQuery } from "@tanstack/react-query";
import { getPatientDashboard } from "@/api/patient.api";
import { Loader2 } from "lucide-react";
import PatientPageHeader from "@/components/patient/PatientPageHeader";
import EmptyState from "@/components/patient/EmptyState";

interface ImagingRecord {
  id: number;
  imagePath?: string;
  date: string;
  doctorName: string;
  notes: string;
}

const Imaging = () => {
  const { t, lang } = useLang();
  const isAr = lang === "ar";
  const { user } = useAuth();
  const { patientImage } = useProfile();
  const [search, setSearch] = useState("");

  const { data: dashboard } = useQuery({
    queryKey: ["patient-dashboard"],
    queryFn: getPatientDashboard,
    staleTime: 60_000,
  });

  const imagingRecords: ImagingRecord[] = [];
  const isLoading = false;

  const formatDate = (date: string) =>
    new Date(date).toLocaleDateString(isAr ? "ar-EG" : "en-US", {
      year: "numeric", month: "long", day: "numeric",
    });

  const filteredData = imagingRecords.filter((item) =>
    formatDate(item.date).toLowerCase().includes(search.toLowerCase()) ||
    (item.doctorName || "").toLowerCase().includes(search.toLowerCase()) ||
    (item.notes || "").toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="imaging-container" dir={isAr ? "rtl" : "ltr"}>

      {/* ── Header row: title + patient info (full width, same as every page) ── */}
      <PatientPageHeader
        title={t.imagingTitle}
        name={dashboard?.fullName || user?.fullName || ""}
        patientId={dashboard?.patientId ?? ""}
        imageUrl={dashboard?.profilePicture || patientImage}
        isAr={isAr}
        patientIdLabel={t.patientId}
      />

      {/* ── Search bar: separate row below the header ── */}
      <div className="imaging-search-row">
        <div className="search-box">
          <input
            type="text"
            placeholder={t.search}
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
          <button className="search-btn" type="button"><FaSearch /></button>
        </div>
      </div>

      {isLoading ? (
        <div className="flex justify-center py-20">
          <Loader2 className="w-10 h-10 animate-spin text-primary" />
        </div>
      ) : filteredData.length === 0 ? (
        <EmptyState
          icon="🩻"
          message={(t as any).noImagingFound || (isAr ? "لا توجد سجلات تصوير." : "No imaging records found.")}
        />
      ) : (
        filteredData.map((item) => (
          <div className="imaging-card" key={item.id}>
            {item.imagePath && (
              <div className="image-box">
                <img src={item.imagePath} alt="scan" />
              </div>
            )}
            <div className="content-box">
              <div className="card-header-box">
                <div className="card-header">
                  <h5>{t.clinicalNotes}</h5>
                  <span>{formatDate(item.date)}</span>
                </div>
                {item.doctorName && (
                  <p style={{ margin: "4px 0 0", fontSize: 13, color: "hsl(var(--muted-foreground))" }}>
                    {item.doctorName}
                  </p>
                )}
              </div>
              <input type="text" value={item.notes} readOnly placeholder="—" className="notes-input" />
            </div>
          </div>
        ))
      )}
    </div>
  );
};

export default Imaging;
