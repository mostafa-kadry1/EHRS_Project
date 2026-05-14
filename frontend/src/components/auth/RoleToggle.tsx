import { motion } from "framer-motion";
import { Stethoscope, User } from "lucide-react";
import { useLanguage } from "@/contexts/LanguageContext";

type Role = "doctor" | "patient";

interface RoleToggleProps {
  role: Role;
  onRoleChange: (role: Role) => void;
}

const RoleToggle = ({ role, onRoleChange }: RoleToggleProps) => {
  const { t, isRtl } = useLanguage();

  // In RTL the flex row is visually reversed, so "doctor" (first child) sits on the right.
  // Anchor the indicator to the start side and move it toward the end for "patient".
  const offset = role === "doctor" ? "0%" : isRtl ? "-100%" : "100%";

  return (
    <div className="relative flex rounded-xl bg-muted p-1 w-full overflow-hidden">
      <motion.div
        className="absolute top-1 bottom-1 rounded-lg shadow-lg"
        initial={false}
        animate={{
          x: offset,
          width: "calc(50% - 4px)",
        }}
        transition={{ type: "spring", stiffness: 400, damping: 30 }}
        style={{
          background: "hsl(var(--primary))",
          insetInlineStart: "4px",
        }}
      />
      {(["doctor", "patient"] as Role[]).map((r) => (
        <button
          key={r}
          onClick={() => onRoleChange(r)}
          className={`relative z-10 flex items-center justify-center gap-2 flex-1 py-2.5 rounded-lg text-sm font-semibold transition-colors duration-200 ${
            role === r ? "text-card" : "text-muted-foreground hover:text-foreground"
          }`}
        >
          {r === "doctor" ? <Stethoscope size={18} /> : <User size={18} />}
          {r === "doctor" ? t.doctor : t.patient}
        </button>
      ))}
    </div>
  );
};

export default RoleToggle;
