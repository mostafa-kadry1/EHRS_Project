import { createContext, useContext, useState, ReactNode } from "react";
import doctorImg from "@/assets/doctor-profile.jpg";
import defaultPatient from "@/assets/patient_images/photo.jpg";

interface ProfileContextType {
  /** Doctor sidebar profile image */
  profileImage: string;
  setProfileImage: (img: string) => void;
  /** Patient top-bar profile image (synced from PersonalInfo) */
  patientImage: string;
  setPatientImage: (img: string) => void;
}

const ProfileContext = createContext<ProfileContextType>({
  profileImage: doctorImg,
  setProfileImage: () => {},
  patientImage: defaultPatient,
  setPatientImage: () => {},
});

export function ProfileProvider({ children }: { children: ReactNode }) {
  const [profileImage, setProfileImage] = useState<string>(doctorImg);
  const [patientImage, setPatientImage] = useState<string>(defaultPatient);
  return (
    <ProfileContext.Provider value={{ profileImage, setProfileImage, patientImage, setPatientImage }}>
      {children}
    </ProfileContext.Provider>
  );
}

export const useProfile = () => useContext(ProfileContext);
