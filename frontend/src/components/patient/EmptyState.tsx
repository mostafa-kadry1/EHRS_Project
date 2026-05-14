import React from "react";

interface Props {
  icon?: string;
  message: string;
}

/**
 * Professional empty-state block used across patient pages.
 */
export default function EmptyState({ icon = "📋", message }: Props) {
  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        padding: "56px 24px",
        background: "hsl(var(--card))",
        borderRadius: 16,
        border: "1px solid hsl(var(--border))",
        gap: 12,
        color: "hsl(var(--muted-foreground))",
        textAlign: "center",
      }}
    >
      <span style={{ fontSize: 40, lineHeight: 1 }}>{icon}</span>
      <p style={{ margin: 0, fontSize: 15, fontWeight: 600 }}>{message}</p>
    </div>
  );
}
