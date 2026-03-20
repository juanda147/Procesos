import { SvgIcon } from '@mui/material';
import type { SvgIconProps } from '@mui/material/SvgIcon';

export default function JusticeLadyIcon(props: SvgIconProps) {
  return (
    <SvgIcon {...props} viewBox="0 0 64 64">
      {/* Scales beam */}
      <line x1="12" y1="14" x2="52" y2="14" stroke="currentColor" strokeWidth="2" fill="none" />

      {/* Center post of scales */}
      <line x1="32" y1="10" x2="32" y2="18" stroke="currentColor" strokeWidth="2" fill="none" />

      {/* Left scale chains */}
      <line x1="12" y1="14" x2="8" y2="26" stroke="currentColor" strokeWidth="1.2" fill="none" />
      <line x1="12" y1="14" x2="16" y2="26" stroke="currentColor" strokeWidth="1.2" fill="none" />

      {/* Left scale pan */}
      <path d="M5 26 Q12 32 19 26" stroke="currentColor" strokeWidth="1.5" fill="none" />

      {/* Right scale chains */}
      <line x1="52" y1="14" x2="48" y2="24" stroke="currentColor" strokeWidth="1.2" fill="none" />
      <line x1="52" y1="14" x2="56" y2="24" stroke="currentColor" strokeWidth="1.2" fill="none" />

      {/* Right scale pan */}
      <path d="M45 24 Q52 30 59 24" stroke="currentColor" strokeWidth="1.5" fill="none" />

      {/* Head */}
      <circle cx="32" cy="8" r="4" stroke="currentColor" strokeWidth="1.5" fill="none" />

      {/* Blindfold */}
      <line x1="28" y1="8" x2="36" y2="8" stroke="currentColor" strokeWidth="1.8" fill="none" />

      {/* Body / dress */}
      <path
        d="M32 12 L32 34 M28 18 L32 14 L36 18 M26 44 L30 34 L32 34 L34 34 L38 44"
        stroke="currentColor" strokeWidth="1.8" fill="none"
        strokeLinecap="round" strokeLinejoin="round"
      />

      {/* Dress / robe shape */}
      <path
        d="M29 20 Q28 28 26 36 Q25 40 22 44 L42 44 Q39 40 38 36 Q36 28 35 20"
        stroke="currentColor" strokeWidth="1.5" fill="none"
        strokeLinecap="round" strokeLinejoin="round"
      />

      {/* Sword on the right hand side */}
      <line x1="38" y1="16" x2="42" y2="38" stroke="currentColor" strokeWidth="1.5" fill="none" />
      {/* Sword guard */}
      <line x1="39" y1="20" x2="43" y2="18" stroke="currentColor" strokeWidth="1.5" fill="none" />

      {/* Base pedestal */}
      <rect x="18" y="44" width="28" height="3" rx="1" stroke="currentColor" strokeWidth="1.5" fill="none" />
      <rect x="14" y="47" width="36" height="3" rx="1" stroke="currentColor" strokeWidth="1.5" fill="none" />

      {/* Arms holding scales and sword */}
      <path d="M28 18 L12 14" stroke="currentColor" strokeWidth="1.5" fill="none" strokeLinecap="round" />
      <path d="M36 18 L38 17" stroke="currentColor" strokeWidth="1.5" fill="none" strokeLinecap="round" />
    </SvgIcon>
  );
}
