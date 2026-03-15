import { Badge } from "@/shared/components/ui/badge";
import type { BadgeProps } from "@/shared/components/ui/badge";
import { DiagnosisSeverity, PrescriptionStatus, RecordStatus } from "../types";

interface SeverityBadgeProps {
  readonly severity: DiagnosisSeverity;
  readonly size?: BadgeProps["size"];
}

const SEVERITY_VARIANTS: Record<DiagnosisSeverity, BadgeProps["variant"]> = {
  [DiagnosisSeverity.Mild]: "success-bordered",
  [DiagnosisSeverity.Moderate]: "warning-bordered",
  [DiagnosisSeverity.Severe]: "error-bordered",
  [DiagnosisSeverity.Critical]: "error-bordered",
};

export function SeverityBadge({ severity, size = "sm" }: SeverityBadgeProps) {
  return (
    <Badge
      variant={SEVERITY_VARIANTS[severity]}
      size={size}
      className={severity === DiagnosisSeverity.Critical ? "bg-error-100 text-error-900 border-error-300" : undefined}
    >
      {severity}
    </Badge>
  );
}

interface StatusBadgeProps {
  readonly status: RecordStatus;
  readonly size?: BadgeProps["size"];
}

const STATUS_LABELS: Record<RecordStatus, string> = {
  [RecordStatus.Active]: "Active",
  [RecordStatus.RequiresFollowUp]: "Follow-up",
  [RecordStatus.Resolved]: "Resolved",
  [RecordStatus.Archived]: "Archived",
};

const STATUS_VARIANTS: Record<RecordStatus, BadgeProps["variant"]> = {
  [RecordStatus.Active]: "info-bordered",
  [RecordStatus.RequiresFollowUp]: "warning-bordered",
  [RecordStatus.Resolved]: "success-bordered",
  [RecordStatus.Archived]: "neutral-bordered",
};

export function StatusBadge({ status, size = "sm" }: StatusBadgeProps) {
  return (
    <Badge variant={STATUS_VARIANTS[status]} size={size}>
      {STATUS_LABELS[status]}
    </Badge>
  );
}

interface PrescriptionStatusBadgeProps {
  readonly status: PrescriptionStatus;
}

const PRESCRIPTION_VARIANTS: Record<PrescriptionStatus, BadgeProps["variant"]> = {
  [PrescriptionStatus.Active]: "info-bordered",
  [PrescriptionStatus.Filled]: "success-bordered",
  [PrescriptionStatus.Completed]: "neutral-bordered",
  [PrescriptionStatus.Cancelled]: "error-bordered",
  [PrescriptionStatus.Expired]: "warning-bordered",
};

export function PrescriptionStatusBadge({ status }: PrescriptionStatusBadgeProps) {
  return (
    <Badge variant={PRESCRIPTION_VARIANTS[status]}>
      {status}
    </Badge>
  );
}
