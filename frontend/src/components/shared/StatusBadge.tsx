import { cn } from "@/lib/utils";
import { useLanguage } from "@/contexts/LanguageContext";

type Status =
  | "Waiting"
  | "Completed"
  | "Cancelled"
  | "Scheduled"
  | "In Progress";

interface StatusBadgeProps {
  status: Status;
}

const statusMap: Record<Status, string> = {
  Waiting: "status-waiting",
  Completed: "status-completed",
  Cancelled: "status-cancelled",
  Scheduled: "status-completed",
  "In Progress": "status-waiting",
};

const statusKeyMap: Record<
  Status,
  "waiting" | "completed" | "cancelled" | "scheduled"
> = {
  Waiting: "waiting",
  Completed: "completed",
  Cancelled: "cancelled",
  Scheduled: "scheduled",
  "In Progress": "waiting",
};

export function StatusBadge({ status }: StatusBadgeProps) {
  const { t } = useLanguage();

  return (
    <span
      className={cn(
        "inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-semibold",
        statusMap[status] ?? "bg-muted text-muted-foreground"
      )}
    >
      <span className="w-1.5 h-1.5 rounded-full bg-current me-1.5 opacity-80" />
      {t[statusKeyMap[status]]}
    </span>
  );
}
