import * as React from "react";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { DayPicker, type DayPickerProps } from "react-day-picker";

import { cn } from "@/lib/utils";
import { buttonVariants } from "@/components/ui/button";

export type CalendarProps = DayPickerProps & {
  /** Minimum selectable year. Defaults to current year - 120. */
  fromYear?: number;
  /** Maximum selectable year. Defaults to current year. */
  toYear?: number;
};

/**
 * Calendar component built on react-day-picker v9.
 *
 * Fixes:
 *  1. Weekday labels correctly centred (w-9 flex justify-center).
 *  2. Nav arrows: prev always LEFT, next always RIGHT — never on same side.
 *  3. Month + Year dropdowns replace the old caption label so users jump
 *     to any year instantly without clicking month-by-month.
 *  4. fromYear / toYear bound the selectable range.
 */
function Calendar({
  className,
  classNames,
  showOutsideDays = true,
  fromYear,
  toYear,
  ...props
}: CalendarProps) {
  const currentYear = new Date().getFullYear();
  const minYear = fromYear ?? currentYear - 120;
  const maxYear = toYear ?? currentYear;
  const isRTL = document.documentElement.dir === "rtl";

  const [displayMonth, setDisplayMonth] = React.useState<Date>(() => {
    if ("selected" in props && props.selected instanceof Date) return new Date(props.selected);
    const today = new Date();
    today.setFullYear(Math.min(maxYear, Math.max(minYear, today.getFullYear())));
    return today;
  });

  const MONTHS = [
    "January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December",
  ];

  const years = React.useMemo(() => {
    const arr: number[] = [];
    for (let y = maxYear; y >= minYear; y--) arr.push(y);
    return arr;
  }, [minYear, maxYear]);

  const gotoPrev = () =>
    setDisplayMonth((p) =>
      isRTL
        ? new Date(p.getFullYear(), p.getMonth() + 1, 1)
        : new Date(p.getFullYear(), p.getMonth() - 1, 1)
    );

  const gotoNext = () =>
    setDisplayMonth((p) =>
      isRTL
        ? new Date(p.getFullYear(), p.getMonth() - 1, 1)
        : new Date(p.getFullYear(), p.getMonth() + 1, 1)
    );

  const isPrevDisabled = displayMonth.getFullYear() === minYear && displayMonth.getMonth() === 0;
  const isNextDisabled = displayMonth.getFullYear() === maxYear && displayMonth.getMonth() === 11;

  const selectCls =
    "text-sm font-medium bg-transparent border border-border rounded px-1.5 py-0.5 " +
    "focus:outline-none focus:ring-2 focus:ring-ring cursor-pointer";

  return (
    <div className={cn("p-3 select-none", className)}>
      {/* Custom caption: [←] [Month▾] [Year▾] [→] */}
      <div className="flex items-center justify-between mb-2 gap-1">
        <button
          type="button"
          onClick={gotoPrev}
          disabled={isPrevDisabled}
          aria-label="Previous month"
          className={cn(
            buttonVariants({ variant: "outline" }),
            "h-7 w-7 p-0 opacity-70 hover:opacity-100 disabled:opacity-30 disabled:cursor-not-allowed shrink-0",
          )}
        >
          {isRTL ? (
            <ChevronRight className="h-4 w-4" />
          ) : (
            <ChevronLeft className="h-4 w-4" />
          )}
        </button>

        <div className="flex items-center gap-1 justify-center flex-1">
          <select
            value={displayMonth.getMonth()}
            onChange={(e) =>
              setDisplayMonth(new Date(displayMonth.getFullYear(), +e.target.value, 1))
            }
            aria-label="Select month"
            className={selectCls}
          >
            {MONTHS.map((name, idx) => (
              <option key={name} value={idx}>{name}</option>
            ))}
          </select>

          <select
            value={displayMonth.getFullYear()}
            onChange={(e) =>
              setDisplayMonth(new Date(+e.target.value, displayMonth.getMonth(), 1))
            }
            aria-label="Select year"
            className={cn(selectCls, "max-w-[76px]")}
          >
            {years.map((y) => (
              <option key={y} value={y}>{y}</option>
            ))}
          </select>
        </div>

        <button
          type="button"
          onClick={gotoNext}
          disabled={isNextDisabled}
          aria-label="Next month"
          className={cn(
            buttonVariants({ variant: "outline" }),
            "h-7 w-7 p-0 opacity-70 hover:opacity-100 disabled:opacity-30 disabled:cursor-not-allowed shrink-0",
          )}
        >
          {isRTL ? (
            <ChevronLeft className="h-4 w-4" />
          ) : (
            <ChevronRight className="h-4 w-4" />
          )}
        </button>
      </div>

      {/* DayPicker grid — built-in caption/nav hidden; we use our own above */}
      <DayPicker
        showOutsideDays={showOutsideDays}
        month={displayMonth}
        onMonthChange={setDisplayMonth}
        classNames={{
          months: "flex flex-col",
          month: "space-y-1",
          month_caption: "hidden",
          caption_label: "hidden",
          nav: "hidden",
          /* ── Alignment fix: give every cell the same w-9 h-9 box ── */
          weekdays: "flex mb-1",
          weekday:
            "w-9 h-7 flex items-center justify-center text-muted-foreground text-[0.8rem] font-normal",
          weeks: "space-y-1",
          week: "flex w-full",
          day: "w-9 h-9 flex items-center justify-center p-0",
          day_button: cn(
            buttonVariants({ variant: "ghost" }),
            "h-9 w-9 p-0 font-normal aria-selected:opacity-100",
          ),
          selected:
            "bg-primary text-primary-foreground hover:bg-primary hover:text-primary-foreground " +
            "focus:bg-primary focus:text-primary-foreground rounded-md",
          today: "bg-accent text-accent-foreground rounded-md",
          outside:
            "text-muted-foreground opacity-50 aria-selected:bg-accent/50 " +
            "aria-selected:text-muted-foreground aria-selected:opacity-30",
          disabled: "text-muted-foreground opacity-30 cursor-not-allowed",
          range_end: "day-range-end",
          range_middle:
            "aria-selected:bg-accent aria-selected:text-accent-foreground",
          hidden: "invisible",
          ...classNames,
        }}
        {...props}
      />
    </div>
  );
}

Calendar.displayName = "Calendar";
export { Calendar };
