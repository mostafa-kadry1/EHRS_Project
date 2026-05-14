import { useEffect, useRef, useState } from "react";
import { Edit, Save, Camera, Loader2 } from "lucide-react";
import { useProfile } from "@/contexts/ProfileContext";
import { useLanguage } from "@/contexts/LanguageContext";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getDoctorProfile, updateDoctorProfile } from "@/api/doctor.api";
import { toast } from "sonner";

interface DoctorProfileData {
  fullName: string;
  specialization: string;
  medicalLicense: string;
  email: string;
  affiliatedHospital: string;
  about: string;
  profilePicture?: string;
}

export default function DoctorProfile() {
  const [editing, setEditing] = useState(false);

  const { profileImage, setProfileImage: setGlobalImage } = useProfile();

  const { t } = useLanguage();

  const [draftImage, setDraftImage] = useState<string | null>(null);

  const [imageFile, setImageFile] = useState<File | null>(null);

  const fileInputRef = useRef<HTMLInputElement>(null);

  const queryClient = useQueryClient();

  const { data: profile, isLoading } = useQuery({
    queryKey: ["doctor-profile"],
    queryFn: async () => {
      const response = await getDoctorProfile();

      return response as DoctorProfileData;
    },
  });

  const [draft, setDraft] = useState<DoctorProfileData>({
    fullName: "",
    specialization: "",
    medicalLicense: "",
    email: "",
    affiliatedHospital: "",
    about: "",
  });

  useEffect(() => {
    if (profile) {
      setDraft(profile);

      if (profile.profilePicture) {
        setGlobalImage(profile.profilePicture);
      }
    }
  }, [profile, setGlobalImage]);

  const updateMutation = useMutation({
    mutationFn: async (updatedData: DoctorProfileData) => {
      const formData = new FormData();

      formData.append("FullName", updatedData.fullName);

      formData.append("Specialization", updatedData.specialization);

      formData.append("MedicalLicense", updatedData.medicalLicense);

      formData.append("Email", updatedData.email);

      formData.append(
        "AffiliatedHospital",
        updatedData.affiliatedHospital
      );

      formData.append("About", updatedData.about);

      if (imageFile) {
        formData.append("ProfilePictureFile", imageFile);
      }

      return updateDoctorProfile(formData);
    },

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["doctor-profile"],
      });

      toast.success("Profile updated successfully");

      setEditing(false);

      setDraftImage(null);

      setImageFile(null);
    },

    onError: () => {
      toast.error("Failed to update profile");
    },
  });

  const handleSave = () => {
    updateMutation.mutate(draft);
  };

  const handleEdit = () => {
    if (profile) {
      setDraft(profile);
    }

    setDraftImage(null);

    setEditing(true);
  };

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="w-8 h-8 animate-spin text-primary" />
      </div>
    );
  }

  const infoRows = [
    {
      label: t.medicalLicense,
      key: "medicalLicense" as const,
    },

    {
      label: t.contact,
      key: "email" as const,
    },

    {
      label: t.affiliatedHospital,
      key: "affiliatedHospital" as const,
    },
  ];

  return (
    <>
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-xl md:text-2xl font-bold text-foreground">
          {t.doctorProfile}
        </h1>

        <button
          disabled={updateMutation.isPending}
          onClick={editing ? handleSave : handleEdit}
          className="flex items-center gap-2 px-5 py-2.5 rounded-lg text-sm font-semibold bg-foreground text-background hover:opacity-90 transition-opacity disabled:opacity-50"
        >
          {updateMutation.isPending ? (
            <Loader2 className="w-4 h-4 animate-spin" />
          ) : editing ? (
            <Save className="w-4 h-4" />
          ) : (
            <Edit className="w-4 h-4" />
          )}

          {editing ? t.saveProfile : t.editProfile}
        </button>
      </div>

      <div className="bg-card rounded-xl border border-border p-6 md:p-8 max-w-3xl">
        <div className="flex flex-col sm:flex-row gap-6 sm:gap-8 items-center sm:items-start">
          <div className="shrink-0 relative group">
            <div className="w-40 h-40 md:w-48 md:h-48 rounded-2xl overflow-hidden ring-4 ring-primary">
              <img
                src={draftImage || profileImage}
                alt={profile?.fullName}
                className="w-full h-full object-cover"
              />
            </div>

            {editing && (
              <>
                <input
                  ref={fileInputRef}
                  type="file"
                  accept="image/*"
                  className="hidden"
                  onChange={(e) => {
                    const file = e.target.files?.[0];

                    if (file) {
                      setImageFile(file);

                      setDraftImage(URL.createObjectURL(file));
                    }
                  }}
                />

                <button
                  onClick={() => fileInputRef.current?.click()}
                  className="absolute inset-0 rounded-2xl bg-black/40 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity cursor-pointer"
                >
                  <Camera className="w-8 h-8 text-white" />
                </button>
              </>
            )}
          </div>

          <div className="flex-1 text-center sm:text-left rtl:sm:text-right">
            {editing ? (
              <div className="space-y-2">
                <input
                  type="text"
                  value={draft.fullName}
                  onChange={(e) =>
                    setDraft({
                      ...draft,
                      fullName: e.target.value,
                    })
                  }
                  className="text-xl md:text-2xl font-bold text-foreground bg-background border border-border rounded-md px-2 py-1 focus:outline-none focus:ring-2 focus:ring-ring w-full"
                />

                <input
                  type="text"
                  value={draft.specialization}
                  onChange={(e) =>
                    setDraft({
                      ...draft,
                      specialization: e.target.value,
                    })
                  }
                  className="text-sm text-muted-foreground font-medium bg-background border border-border rounded-md px-2 py-1 focus:outline-none focus:ring-2 focus:ring-ring w-full"
                />
              </div>
            ) : (
              <>
                <h2 className="text-xl md:text-2xl font-bold text-foreground">
                  {profile?.fullName}
                </h2>

                <p className="text-sm text-muted-foreground font-medium mt-1">
                  {profile?.specialization}
                </p>
              </>
            )}

            <div className="mt-6 space-y-4">
              {infoRows.map((row) => (
                <div
                  key={row.key}
                  className="flex flex-col sm:flex-row sm:gap-8"
                >
                  <p className="text-sm font-semibold text-foreground sm:w-44 shrink-0">
                    {row.label}
                  </p>

                  {editing ? (
                    <input
                      type="text"
                      value={draft[row.key] || ""}
                      onChange={(e) =>
                        setDraft({
                          ...draft,
                          [row.key]: e.target.value,
                        })
                      }
                      className="text-sm text-foreground bg-background border border-border rounded-md px-2 py-1 focus:outline-none focus:ring-2 focus:ring-ring w-full max-w-xs"
                    />
                  ) : (
                    <p className="text-sm text-foreground">
                      {profile?.[row.key]}
                    </p>
                  )}
                </div>
              ))}

              <div className="flex flex-col sm:flex-row sm:gap-8">
                <p className="text-sm font-semibold text-foreground sm:w-44 shrink-0">
                  {t.about}
                </p>

                {editing ? (
                  <textarea
                    value={draft.about}
                    onChange={(e) =>
                      setDraft({
                        ...draft,
                        about: e.target.value,
                      })
                    }
                    rows={3}
                    className="flex-1 text-sm bg-background border border-border rounded-md px-2 py-1 focus:outline-none focus:ring-2 focus:ring-ring resize-none"
                  />
                ) : (
                  <p className="text-sm text-muted-foreground">
                    {profile?.about}
                  </p>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}