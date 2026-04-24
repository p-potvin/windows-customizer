# 🛡️ VaultWares
**Privacy-First Digital Asset & Hardware Marketplace**

VaultWares builds consumer-facing products with a simple order of priorities: **individuals' privacy first**, **security second**, and **functionality third**. We use modern, type-safe tools to deliver a smooth experience for high-value digital and physical goods, but we don’t treat “security” as a substitute for privacy. Privacy is about limiting and controlling data; security is how we protect what exists. We aim to strike a practical balance and avoid fear-driven choices that quietly erode privacy.

## 🚀 Tech Stack
- **Frontend:** [Next.js 15](https://nextjs.org/) (App Router), [TypeScript](https://www.typescriptlang.org/)
- **Styling:** [Tailwind CSS](https://tailwindcss.com/), [Shadcn UI](https://ui.shadcn.com/)
- **Backend/Auth:** [Supabase](https://supabase.com/) (PostgreSQL + RLS), Either [Node.js](https://nodejs.org/), [Django](https://django.com/) or [.Net](https://dotnet.microsoft.com) depending on the project
- **State Management:** [TanStack Query](https://tanstack.com/query) & [Zustand](https://docs.pmnd.rs/zustand)
- **Validation:** [Zod](https://zod.dev/)

## 🛠️ Getting Started
1. **Pull the latest version:** `git fetch; git pull`
2. **Install dependencies:** `npm install`
3. **Set up Environment:** Create a `.env.local` with your Supabase keys.
4. **Run Development:** `npm run dev`

## 🧭 Principles (Privacy → Security → Functionality)
- **Privacy First:** Default to minimal data collection, clear consent, and no hidden tracking. Keep personal data out of logs and analytics unless it’s truly necessary.
- **Security Second:** Use proven defenses (RLS/least-privilege, input validation, safe storage) to protect privacy and prevent misuse.
- **Functionality Third:** Keep the product simple, understandable, and accessible for non-technical users.

## 🎨 Design Language
- **Theme:** Industrial-Cyber (Dark Mode Only).
- **Core Colors:** Slate-950 (BG), Emerald-400 (Accents), White/10 (Glassmorphism).
