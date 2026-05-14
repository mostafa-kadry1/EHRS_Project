/**
 * DateInput — Shared hybrid date-input used everywhere in the app.
 *
 * Behaviour:
 *  - Shows a text input so the user can type manually (DD/MM/YYYY)
 *  - A calendar icon button opens the shared Calendar popover
 *  - Clicking a day + pressing "Confirm" commits the date and closes the popover
 *  - RTL-safe: cursor/typing is always LTR for date digits
 *  - Consistent style with the rest of the form inputs
 *
 * Used by: SignupForm DOB, Booking date, PersonalInformation birthDate
 */

import React, { useState, useRef } from "react";
import { Calendar as CalendarIcon } from "lucide-react";
import { format, parse, isValid } from "date-fns";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Calendar } from "@/components/ui/calendar";
import { cn } from "@/lib/utils";
import { useLanguage } from "@/contexts/LanguageContext";

interface DateInputProps {
  /** Committed date value */
  value: Date | undefined;
  onChange: (date: Date | undefined) => void;
  /** Min selectable date */
  minDate?: Date;
  /** Max selectable date */
  maxDate?: Date;
  placeholder?: string;
  /** Error state — adds red border */
  error?: boolean;
  disabled?: boolean;
  /** Extra CSS classes on the wrapper */
  className?: string;
  /** Override confirm button label */
  confirmLabel?: string;
  /** fromYear passed to Calendar */
  fromYear?: number;
  /** toYear passed to Calendar */
  toYear?: number;
  id?: string;
}

const TODAY = new Date();
TODAY.setHours(0, 0, 0, 0);

/** Parse a DD/MM/YYYY typed string into a Date. Returns null if invalid. */
function parseDMY(str: string): Date | null {
  const clean = str.replace(/[^\d/]/g, "");
  const match = clean.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})$/);
  if (!match) return null;
  const [, d, m, y] = match;
  const parsed = parse(`${d.padStart(2, "0")}/${m.padStart(2, "0")}/${y}`, "dd/MM/yyyy", new Date());
  return isValid(parsed) ? parsed : null;
}

export default function DateInput({
  value,
  onChange,
  minDate,
  maxDate,
  placeholder = "DD/MM/YYYY",
  error = false,
  disabled = false,
  className,
  confirmLabel = "Confirm",
  fromYear,
  toYear,
  id,
}: DateInputProps) {
  const [open, setOpen] = useState(false);
  const { lang } = useLanguage();
  const isRTL = lang === "ar";
  const [pending, setPending] = useState<Date | undefined>(value);
  const [typedText, setTypedText] = useState(value ? format(value, "dd/MM/yyyy") : "");
  const inputRef = useRef<HTMLInputElement>(null);

  // Sync text field when external value changes
  React.useEffect(() => {
    setTypedText(value ? format(value, "dd/MM/yyyy") : "");
    setPending(value);
  }, [value]);

  const handleTextChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    let v = e.target.value;
    // Auto-insert slashes for convenience
    const digits = v.replace(/\D/g, "");
    let formatted = digits;
    if (digits.length > 2) formatted = digits.slice(0, 2) + "/" + digits.slice(2);
    if (digits.length > 4) formatted = digits.slice(0, 2) + "/" + digits.slice(2, 4) + "/" + digits.slice(4, 8);
    setTypedText(formatted);

    const parsed = parseDMY(formatted);
    if (parsed) {
      // Clamp to min/max
      if (minDate && parsed < minDate) return;
      if (maxDate && parsed > maxDate) return;
      onChange(parsed);
    } else if (formatted === "") {
      onChange(undefined);
    }
  };

  const handleCalendarSelect = (d: Date | undefined) => {
    setPending(d);
  };

  const handleConfirm = () => {
    if (!pending) return;
    onChange(pending);
    setTypedText(format(pending, "dd/MM/yyyy"));
    setOpen(false);
  };

  const handleOpenChange = (o: boolean) => {
    setOpen(o);
    if (!o) setPending(value); // cancel pending if closed without confirm
  };

  const ringCls = error
    ? "border-destructive ring-destructive/40 focus:ring-destructive/40"
    : "border-border ring-primary/40 focus:border-transparent";

  return (
    <div className={cn("relative h-12 flex items-center", className)}>
      {/* Text input — always LTR for digit readability */}
      <input
        ref={inputRef}
        id={id}
        type="text"
        inputMode="numeric"
        dir={isRTL ? "rtl" : "ltr"}
        value={typedText}
        onChange={handleTextChange}
        placeholder={placeholder}
        disabled={disabled}
        maxLength={10}
        className={cn(
          "w-full h-11 rounded-lg border bg-card text-foreground",
          isRTL
            ? "h-12 pl-10 pr-3 text-right direction-rtl"
            : "h-12 pr-10 pl-3 text-left direction-ltr",
          "[unicode-bidi:plaintext]",
          "placeholder:text-muted-foreground transition-all duration-200",
          "focus:outline-none focus:ring-2 text-sm",
          ringCls,
          disabled && "opacity-50 cursor-not-allowed",
        )}
      />

      {/* Calendar icon button */}
      <Popover open={open} onOpenChange={handleOpenChange}>
        <PopoverTrigger asChild>
          <button
            type="button"
            disabled={disabled}
            tabIndex={-1}
            aria-label="Open calendar"
            className={cn(
              `absolute top-1/2 -translate-y-1/2 ${isRTL ? "left-3" : "right-3"
              }`,
              "text-muted-foreground hover:text-primary transition-colors",
              disabled && "opacity-40 cursor-not-allowed pointer-events-none",
            )}
          >
            <CalendarIcon size={17} />
          </button>
        </PopoverTrigger>

        <PopoverContent
          className="w-auto p-0"
          align="start"
          // RTL: flip to end so it doesn't overflow the screen
          side="bottom"
        >
          <Calendar
            mode="single"
            selected={pending}
            onSelect={handleCalendarSelect}
            defaultMonth={pending ?? (maxDate ? new Date(maxDate.getFullYear() - 25, 0, 1) : TODAY)}
            fromYear={fromYear}
            toYear={toYear}
            disabled={(d) => {
              if (minDate && d < minDate) return true;
              if (maxDate && d > maxDate) return true;
              return false;
            }}
            className="pointer-events-auto"
          />
          <div className="border-t border-border px-3 py-2 flex justify-end">
            <button
              type="button"
              disabled={!pending}
              onClick={handleConfirm}
              className="text-sm font-semibold text-primary hover:underline disabled:opacity-40 disabled:cursor-not-allowed"
            >
              {confirmLabel}
            </button>
          </div>
        </PopoverContent>
      </Popover>
    </div>
  );
}
