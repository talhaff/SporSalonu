"use client";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { useState } from "react";

export default function QueryProvider({ children }: { children: React.ReactNode }) {
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            staleTime: 0, // Veriler her zaman eski kabul edilecek ve sayfa geçişlerinde otomatik güncellenecek
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
