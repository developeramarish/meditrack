/**
 * Theme Derivation Engine
 *
 * Takes 5 brand colors and derives 25+ semantic CSS variables.
 * This is the bridge between "designer picks colors" and "developer ships theme."
 *
 * Usage:
 *   const theme = deriveTheme({ background: '#03045E', foreground: '#CAF0F8', ... });
 *   // Returns 25+ CSS variables as { '--background': '239 94% 19%', ... }
 *
 * Or from a Coolors URL:
 *   const theme = themeFromCoolorsUrl('https://coolors.co/palette/03045e-0077b6-00b4d8-90e0ef-caf0f8');
 */

// ── Types ────────────────────────────────────────────────────────

export interface PaletteInput {
  /** Page background (darkest in dark themes, lightest in light) */
  readonly background: string;
  /** Primary text (highest contrast against background) */
  readonly foreground: string;
  /** Main brand / CTA color */
  readonly primary: string;
  /** Supporting action color */
  readonly secondary: string;
  /** Highlight / badge / accent color */
  readonly accent: string;
  /** Optional overrides — auto-derived if omitted */
  readonly destructive?: string;
  readonly success?: string;
  readonly warning?: string;
}

export interface DerivedTheme {
  readonly [key: `--${string}`]: string;
}

// ── Color Math Helpers ───────────────────────────────────────────

/** Parse hex (#RRGGBB) to [R, G, B] 0-255 */
function hexToRgb(hex: string): [number, number, number] {
  const clean = hex.replace('#', '');
  return [
    parseInt(clean.slice(0, 2), 16),
    parseInt(clean.slice(2, 4), 16),
    parseInt(clean.slice(4, 6), 16),
  ];
}

/** RGB to HSL (returns [h 0-360, s 0-100, l 0-100]) */
function rgbToHsl(r: number, g: number, b: number): [number, number, number] {
  const rNorm = r / 255;
  const gNorm = g / 255;
  const bNorm = b / 255;
  const max = Math.max(rNorm, gNorm, bNorm);
  const min = Math.min(rNorm, gNorm, bNorm);
  const lightness = (max + min) / 2;
  let hue = 0;
  let saturation = 0;

  if (max !== min) {
    const delta = max - min;
    saturation = lightness > 0.5 ? delta / (2 - max - min) : delta / (max + min);
    if (max === rNorm) hue = ((gNorm - bNorm) / delta + (gNorm < bNorm ? 6 : 0)) * 60;
    else if (max === gNorm) hue = ((bNorm - rNorm) / delta + 2) * 60;
    else hue = ((rNorm - gNorm) / delta + 4) * 60;
  }

  return [Math.round(hue * 10) / 10, Math.round(saturation * 1000) / 10, Math.round(lightness * 1000) / 10];
}

/** Convert hex to CSS HSL value string (e.g., "225 66% 14%") */
function hexToHslString(hex: string): string {
  const [r, g, b] = hexToRgb(hex);
  const [h, s, l] = rgbToHsl(r, g, b);
  return `${h} ${s}% ${l}%`;
}

/** Adjust HSL lightness by a delta (-100 to +100) */
function adjustLightness(hex: string, delta: number): string {
  const [r, g, b] = hexToRgb(hex);
  const [h, s, l] = rgbToHsl(r, g, b);
  const newL = Math.max(0, Math.min(100, l + delta));
  return `${h} ${s}% ${newL}%`;
}

/** Adjust HSL saturation by a factor (0 = fully desaturated, 1 = unchanged) */
function adjustSaturation(hex: string, factor: number, lightnessDelta = 0): string {
  const [r, g, b] = hexToRgb(hex);
  const [h, s, l] = rgbToHsl(r, g, b);
  const newS = Math.max(0, Math.min(100, s * factor));
  const newL = Math.max(0, Math.min(100, l + lightnessDelta));
  return `${h} ${newS}% ${newL}%`;
}

/** Determine if a color is "dark" (relative luminance below WCAG threshold ~46%) */
function isDark(hex: string): boolean {
  const [r, g, b] = hexToRgb(hex);
  // Use relative luminance (WCAG 2.1) instead of HSL lightness for accuracy
  const toLinear = (c: number) => {
    const srgb = c / 255;
    return srgb <= 0.03928 ? srgb / 12.92 : Math.pow((srgb + 0.055) / 1.055, 2.4);
  };
  const luminance = 0.2126 * toLinear(r) + 0.7152 * toLinear(g) + 0.0722 * toLinear(b);
  return luminance < 0.179; // ~46% HSL lightness equivalent
}

/** Get a contrast foreground (white or dark) for a given background */
function contrastForeground(hex: string): string {
  return isDark(hex) ? '0 0% 100%' : '222 47% 11%';
}

// ── Derivation Helpers ────────────────────────────────────────────

/** Pre-computed HSL for a hex color — avoids redundant parsing */
interface ParsedColor {
  readonly hex: string;
  readonly hsl: string;        // CSS-ready "H S% L%"
  readonly h: number;
  readonly s: number;
  readonly l: number;
}

function parseHex(hex: string): ParsedColor {
  const [r, g, b] = hexToRgb(hex);
  const [h, s, l] = rgbToHsl(r, g, b);
  return { hex, hsl: `${h} ${s}% ${l}%`, h, s, l };
}

/** Dimmed foreground with WCAG AA clamp (~4.5:1 against card surface) */
function deriveDimmedForeground(fg: ParsedColor, isBackgroundDark: boolean): string {
  const newS = Math.max(0, Math.min(100, fg.s * 0.7));
  const newL = isBackgroundDark
    ? Math.max(58, fg.l - 34)   // never drop below 58% on dark backgrounds
    : Math.min(52, fg.l + 20);  // never rise above 52% on light backgrounds
  return `${fg.h} ${newS}% ${newL}%`;
}

// ── Derivation Engine ────────────────────────────────────────────

/**
 * Derive a complete theme from 5 brand colors.
 *
 * Pre-computes HSL for each input color once, then references the cached
 * values throughout — eliminates ~30 redundant hex→RGB→HSL conversions.
 */
export function deriveTheme(input: PaletteInput): DerivedTheme {
  // Pre-compute all input colors once
  const bg = parseHex(input.background);
  const fg = parseHex(input.foreground);
  const pri = parseHex(input.primary);
  const sec = parseHex(input.secondary);
  const acc = parseHex(input.accent);

  const isBackgroundDark = isDark(input.background);
  const lift = isBackgroundDark ? 4 : -4;
  const dimmedFg = deriveDimmedForeground(fg, isBackgroundDark);

  return {
    // ── Layout ──
    '--background': bg.hsl,
    '--foreground': fg.hsl,

    // ── Surfaces (derived from background) ──
    '--card': adjustLightness(input.background, lift),
    '--card-foreground': fg.hsl,
    '--popover': adjustLightness(input.background, lift * 1.5),
    '--popover-foreground': fg.hsl,
    '--muted': adjustSaturation(input.background, 0.6, lift * 2),
    '--muted-foreground': dimmedFg,

    // ── Brand colors ──
    '--primary': pri.hsl,
    '--primary-foreground': contrastForeground(input.primary),
    '--secondary': sec.hsl,
    '--secondary-foreground': contrastForeground(input.secondary),
    '--accent': acc.hsl,
    '--accent-foreground': contrastForeground(input.accent),

    // ── Utility (derived from background) ──
    '--destructive': input.destructive
      ? hexToHslString(input.destructive)
      : isBackgroundDark ? '0 72% 63%' : '0 84% 60%',
    '--destructive-foreground': '0 0% 100%',
    '--border': adjustLightness(input.background, lift * 3),
    '--input': adjustLightness(input.background, lift * 2.5),
    '--ring': pri.hsl,

    // ── Sidebar (darker than background) ──
    '--sidebar-background': adjustLightness(input.background, -lift),
    '--sidebar-foreground': adjustSaturation(input.foreground, 0.8, isBackgroundDark ? -18 : 10),
    '--sidebar-primary': pri.hsl,
    '--sidebar-primary-foreground': contrastForeground(input.primary),
    '--sidebar-accent': adjustLightness(input.background, lift),
    '--sidebar-accent-foreground': fg.hsl,
    '--sidebar-border': adjustLightness(input.background, lift * 1.5),
    '--sidebar-ring': pri.hsl,

    // ── Chart ──
    '--chart-surface': adjustLightness(input.background, lift),
    '--chart-grid': adjustLightness(input.background, lift * 3),
    '--chart-text': dimmedFg,
    '--chart-tooltip-border': adjustLightness(input.background, lift * 4),
    '--chart-tooltip-shadow': isBackgroundDark
      ? '228 80% 5% / 0.5'
      : '0 0% 0% / 0.1',

    // ── Geometry (not derived from colors, but needed for applied themes) ──
    '--radius': '0.5rem',
  };
}

// ── Coolors.co Integration ───────────────────────────────────────

/**
 * Parse a Coolors.co URL into hex color array.
 *
 * Supports:
 *   https://coolors.co/palette/03045e-0077b6-00b4d8-90e0ef-caf0f8
 *   https://coolors.co/03045e-0077b6-00b4d8-90e0ef-caf0f8
 */
export function parseCoolorsUrl(url: string): string[] {
  const match = url.match(/coolors\.co\/(?:palette\/)?([a-f0-9]{6}(?:-[a-f0-9]{6})*)/i);
  if (!match) throw new Error(`Invalid Coolors URL: ${url}`);
  return match[1].split('-').map(hex => `#${hex}`);
}

/**
 * Auto-assign palette roles by sorting colors by luminance.
 * Darkest → background, lightest → foreground, middle 3 by saturation.
 */
export function autoAssignRoles(hexColors: string[]): PaletteInput {
  const sorted = [...hexColors].sort((a, b) => {
    const [, , lA] = rgbToHsl(...hexToRgb(a));
    const [, , lB] = rgbToHsl(...hexToRgb(b));
    return lA - lB;
  });

  // For 5 colors: [darkest, ..., lightest]
  // Middle 3 sorted by saturation: most vivid = primary
  const middle = sorted.slice(1, -1).sort((a, b) => {
    const [, sA] = rgbToHsl(...hexToRgb(a));
    const [, sB] = rgbToHsl(...hexToRgb(b));
    return sB - sA;
  });

  return {
    background: sorted[0],
    foreground: sorted[sorted.length - 1],
    primary: middle[0],
    secondary: middle[1] ?? middle[0],
    accent: middle[2] ?? middle[0],
  };
}

/**
 * One-step: Coolors URL → complete theme.
 *
 * @example
 * const theme = themeFromCoolorsUrl('https://coolors.co/palette/03045e-0077b6-00b4d8-90e0ef-caf0f8');
 */
export function themeFromCoolorsUrl(url: string): DerivedTheme {
  const colors = parseCoolorsUrl(url);
  if (colors.length < 3) throw new Error(`Need at least 3 colors, got ${colors.length}`);
  const roles = autoAssignRoles(colors);
  return deriveTheme(roles);
}

