/**
 * Shared badge style constants for semantic color variants.
 *
 * These use the `dark:` variant pattern because status/semantic colors
 * (success, error, warning, info) are fixed scales — not derived by the
 * theme engine. Centralizing them here prevents repetition across pages.
 */

/** Semantic color badge — light bg + dark text with dark mode override */
export const BADGE_VARIANT = {
  success:
    "border border-success-500/30 bg-success-50 dark:bg-success-900/30 text-success-700 dark:text-success-300",
  error:
    "border border-error-500/30 bg-error-50 dark:bg-error-900/30 text-error-700 dark:text-error-300",
  warning:
    "border border-warning-500/30 bg-warning-50 dark:bg-warning-900/30 text-warning-700 dark:text-warning-300",
  info:
    "border border-info-500/30 bg-info-50 dark:bg-info-900/30 text-info-700 dark:text-info-300",
  primary:
    "border border-primary-200 dark:border-primary-700 bg-primary-50 dark:bg-primary-900/30 text-primary-700 dark:text-primary-300",
  accent:
    "border border-accent-200 dark:border-accent-700 bg-accent-50 dark:bg-accent-900/30 text-accent-700 dark:text-accent-300",
  secondary:
    "border border-secondary-200 dark:border-secondary-700 bg-secondary-50 dark:bg-secondary-900/30 text-secondary-700 dark:text-secondary-300",
  neutral:
    "border border-border bg-muted text-muted-foreground",
} as const;

export type BadgeVariant = keyof typeof BADGE_VARIANT;
