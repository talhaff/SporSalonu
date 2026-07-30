import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import QueryProvider from "@/providers/QueryProvider";
import CheckInToastListener from "@/components/CheckInToastListener";

const inter = Inter({
  subsets: ["latin"],
  variable: "--font-inter",
});

export const metadata: Metadata = {
  title: "VipGym Yönetim | Mini-ERP",
  description: "Spor salonu yönetim sistemi",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="tr" className={`${inter.variable}`}>
      <body className="antialiased min-h-screen bg-[var(--color-background)] text-white relative">
        {/* Arka plan animasyon / dekorasyon (Premium hissiyat için) */}
        <div className="fixed inset-0 -z-10 overflow-hidden">
          <div className="absolute top-[-20%] left-[-10%] w-[50%] h-[50%] rounded-full bg-blue-600/20 blur-[120px]" />
          <div className="absolute bottom-[-20%] right-[-10%] w-[50%] h-[50%] rounded-full bg-emerald-600/10 blur-[120px]" />
        </div>
        
        <QueryProvider>
          {children}
          <CheckInToastListener />
        </QueryProvider>
      </body>
    </html>
  );
}
