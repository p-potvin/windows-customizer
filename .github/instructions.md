# 🛠 Project Context: VaultWares
##1. Project Overview

*	VaultWares builds consumer-facing software with **individuals' privacy as the first priority**. Security is a close second (it exists to protect privacy), and functionality is the third pillar (it must stay simple and useful for real people). The tech stack aims for a minimalist "industrial-cyber" aesthetic without sacrificing privacy-first defaults.
## 2. Core Tech Stack

*    Framework: Next.js 15+ (App Router)

*    Language: TypeScript (Strict mode)

*    Styling: Tailwind CSS with a dark-mode first approach.

*    State Management: TanStack Query (React Query) for server state; Zustand for local state.

*    Database/Backend: Supabase (PostgreSQL + Auth + Storage).

*    UI Components: Radix UI primitives / Shadcn UI.

## 3. Coding Standards & Patterns

*	When generating code for VaultWares, adhere to the following:

*   Component Architecture: Use Functional Components with Arrow Syntax. Favor Server Components for data fetching and Client Components only when interactivity is required.

*   Naming Conventions: 
** 		Components: PascalCase (e.g., ProductVault.tsx)**

**      Hooks: camelCase starting with 'use' (e.g., useVaultAuth.ts)**

**      Utilities: kebab-case (e.g., format-currency.ts)**

*    Type Safety: Avoid any at all costs. Use Zod for schema validation (especially for API responses and form inputs).

*    Performance: Implement React Suspense for loading states and utilize Next.js Image component for all assets.

## 4. Style Guide (Tailwind)

*    Primary Palette: * Background: bg-slate-950

**        Accents: text-emerald-400 (Success/Action), text-amber-500 (Warning).**

**        Borders: border-slate-800.**

*    Button Style: Default to a "Glassmorphism" effect: bg-white/10 backdrop-blur-md border border-white/20 hover:bg-white/20.

## 5. Specific Constraints for Gemini

*    Privacy First (Primary): Collect the minimum data needed, avoid hidden tracking/fingerprinting, and keep personal data out of logs and analytics by default. Prefer explicit consent and privacy-preserving defaults.

*    Security (Second): Always sanitize and validate user inputs. If writing SQL or Supabase queries, ensure Row Level Security (RLS) and least-privilege access are considered.

*    Functionality (Third): Keep flows understandable for non-technical users. Prefer clear labels, safe defaults, and guidance over complexity.

*    Error Handling: Use a centralized error-boundary pattern. Don't just console.log(error); provide user-friendly feedback using a Toast component.

*    No Redundancy: Check @/components/ui before creating new UI elements to avoid duplicating Shadcn components.
