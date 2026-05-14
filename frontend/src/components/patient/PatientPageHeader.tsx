/**
 * PatientPageHeader
 *
 * Renders the patient name + ID block shown in the top-right of every patient
 * page. Clicking the avatar or name navigates to Personal Information.
 *
 * Props
 *  title       – page heading shown on the left
 *  name        – patient full name
 *  patientId   – numeric patient ID (or empty string while loading)
 *  imageUrl    – profile picture URL
 *  isAr        – true when language is Arabic
 *  patientIdLabel – translated "Patient ID" string
 */
import React from "react";
import { useNavigate } from "react-router-dom";
import defaultPhoto from "@/assets/patient_images/photo.jpg";

function resolveImageUrl(path: string | undefined | null): string | undefined {
  if (!path) return undefined;
  if (path.startsWith("http") || path.startsWith("data:") || path.startsWith("blob:")) return path;
  const base = (import.meta.env.VITE_API_BASE_URL || "http://localhost:5175/api").replace(/\/api$/, "");
  return base + (path.startsWith("/") ? path : "/" + path);
}

interface Props {
  title: string;
  name: string;
  patientId: string | number;
  imageUrl?: string;
  isAr: boolean;
  patientIdLabel: string;
}

const toAr = (s: string | number) =>
  String(s).replace(/\d/g, (d) => "٠١٢٣٤٥٦٧٨٩"[+d]);

export default function PatientPageHeader({
  title, name, patientId, imageUrl, isAr, patientIdLabel,
}: Props) {
  const navigate = useNavigate();
  const idDisplay = patientId
    ? `${patientIdLabel}: ${isAr ? toAr(String(patientId)) : patientId}`
    : "";

  return (
    <div
      className="patient-page-header"
      style={{
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        flexWrap: "wrap",
        gap: 12,
        marginBottom: 24,
        paddingBottom: 16,
        borderBottom: "1px solid hsl(var(--border))",
      }}
    >
      <h2 style={{ margin: 0, fontSize: 26, fontWeight: 800, color: "hsl(var(--foreground))", letterSpacing: -0.4 }}>
        {title}
      </h2>

      <div
        onClick={() => navigate("/patient/personal-information")}
        title={isAr ? "المعلومات الشخصية" : "Personal Information"}
        style={{ display: "flex", alignItems: "center", gap: 10, cursor: "pointer" }}
      >
        <div style={{ textAlign: isAr ? "left" : "right" }}>
          <p style={{ margin: 0, fontWeight: 700, fontSize: 14, color: "hsl(var(--foreground))" }}>{name}</p>
          {idDisplay && (
            <span style={{ fontSize: 12, color: "hsl(var(--muted-foreground))" }}>{idDisplay}</span>
          )}
        </div>
        <img
          src={resolveImageUrl(imageUrl) || defaultPhoto}
          alt={name}
          onError={(e) => { (e.target as HTMLImageElement).src = defaultPhoto; }}
          style={{
            width: 44, height: 44, borderRadius: "50%", objectFit: "cover",
            border: "2px solid hsl(var(--primary) / 0.25)",
            boxShadow: "0 2px 8px rgba(0,0,0,0.08)",
            flexShrink: 0,
          }}
        />
      </div>
    </div>
  );
}
