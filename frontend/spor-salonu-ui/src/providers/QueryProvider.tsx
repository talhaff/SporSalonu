"use client";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { useState } from "react";

export default function QueryProvider({ children }: { children: React.ReactNode }) {
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            staleTime: 60 * 1000, // 1 dakika (Veriler 1 dk taze kabul edilecek)
            refetchOnWindowFocus: false, // Pencereye odaklanıldığında tekrar çekilmesini kapat
            retry: 1, // Hata anında sadece 1 kez daha dene
          },
        },
      })
  );

  return (
    <QueryClientProvider client={queryClient}>
      {children}
    </QueryClientProvider>
  );
}
