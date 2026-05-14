import { AlertCircle } from "lucide-react";
import { motion, AnimatePresence } from "framer-motion";

interface FieldErrorProps {
  message?: string | null;
}

const FieldError = ({ message }: FieldErrorProps) => {
  return (
    <AnimatePresence mode="wait">
      {message ? (
        <motion.p
          key={message}
          initial={{ opacity: 0, y: -4 }}
          animate={{ opacity: 1, y: 0 }}
          exit={{ opacity: 0, y: -4 }}
          transition={{ duration: 0.15 }}
          className="flex items-center gap-1.5 text-xs font-medium text-destructive mt-1"
          role="alert"
        >
          <AlertCircle size={13} className="shrink-0" />
          <span>{message}</span>
        </motion.p>
      ) : null}
    </AnimatePresence>
  );
};

export default FieldError;
