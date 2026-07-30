"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { Search, Loader2, Clock, Calendar, CheckCircle2, XCircle, ArrowDownLeft, ArrowUpRight } from "lucide-react";

interface CheckInHistoryItem {
  id: number;
  uyeAdSoyad: string;
  telefon: string;
  girisTarihi: string;
  cikisTarihi: string | null;
  izinVerildi: boolean;
  redSebebi: string | null;
}

export default function CheckInHistoryPage() {
  const [searchQuery, setSearchQuery] = useState("");

  const { data: historyData, isLoading, error } = useQuery({
    queryKey: ["checkin-gecmisi"],
    queryFn: async () => {
      const res = await api.get("/checkin/gecmis");
      return res.data.data as CheckInHistoryItem[];
    },
    refetchInterval: 5000, // 5 saniyede bir otomatik canlı güncelleme
  });

  const filteredData = historyData?.filter(item =>
    item.uyeAdSoyad.toLowerCase().includes(searchQuery.toLowerCase()) ||
    item.telefon.includes(searchQuery)
  ) || [];

  const formatDuration = (startStr: string, endStr: string | null) => {
    if (!endStr) return <span className="text-amber-400 font-medium animate-pulse">İçeride (Devam Ediyor)</span>;
    const start = new Date(startStr).getTime();
    const end = new Date(endStr).getTime();
    const diffMins = Math.floor((end - start) / (1000 * 60));
    if (diffMins < 60) return `${diffMins} dk`;
    const hours = Math.floor(diffMins / 60);
    const mins = diffMins % 60;
    return `${hours} sa ${mins} dk`;
  };

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-700">
      <header className="flex justify-between items-end">
        <div>
          <h1 className="text-4xl font-black mb-2 flex items-center gap-3">
            <Clock className="w-9 h-9 text-primary" /> Geçiş & Turnike Geçmişi
          </h1>
          <p className="text-gray-400">Tüm üye giriş ve çıkışlarının detaylı zaman dökümü.</p>
        </div>
      </header>

      {/* Arama Alanı */}
      <div className="glass-panel p-4 flex items-center gap-4">
        <div className="relative flex-1">
          <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
          <input
            type="text"
            placeholder="Üye adı veya telefon no ile filtrele..."
            className="w-full bg-white/5 border border-white/10 rounded-xl py-3 pl-12 pr-4 text-sm focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all text-white placeholder-gray-500"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
        </div>
      </div>

      {/* Tablo Alanı */}
      <div className="glass-panel overflow-hidden">
        {isLoading ? (
          <div className="flex items-center justify-center p-12">
            <Loader2 className="w-8 h-8 animate-spin text-primary" />
          </div>
        ) : error ? (
          <div className="p-8 text-center text-red-400">Veriler yüklenirken bir sorun oluştu.</div>
        ) : filteredData.length === 0 ? (
          <div className="p-12 text-center text-gray-400 font-medium">Henüz kayıtlı bir geçiş logu bulunmuyor.</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="border-b border-white/10 bg-white/5 text-gray-400 text-xs font-semibold uppercase tracking-wider">
                  <th className="py-4 px-6">Üye Bilgisi</th>
                  <th className="py-4 px-6">Giriş Saati</th>
                  <th className="py-4 px-6">Çıkış Saati</th>
                  <th className="py-4 px-6">Toplam Süre</th>
                  <th className="py-4 px-6">Durum / Not</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-white/5 text-sm">
                {filteredData.map((item) => (
                  <tr key={item.id} className="hover:bg-white/[0.02] transition-colors">
                    <td className="py-4 px-6">
                      <div className="flex items-center gap-3">
                        <div className="w-9 h-9 rounded-full bg-gradient-to-tr from-blue-500 to-indigo-600 flex items-center justify-center font-bold text-xs">
                          {item.uyeAdSoyad.substring(0, 2).toUpperCase()}
                        </div>
                        <div>
                          <p className="font-semibold text-white">{item.uyeAdSoyad}</p>
                          <p className="text-xs text-gray-400">{item.telefon}</p>
                        </div>
                      </div>
                    </td>
                    <td className="py-4 px-6">
                      <div className="flex items-center gap-2 text-emerald-400 font-medium">
                        <ArrowDownLeft className="w-4 h-4" />
                        <div>
                          <span>{new Date(item.girisTarihi).toLocaleTimeString("tr-TR", { hour: "2-digit", minute: "2-digit", second: "2-digit" })}</span>
                          <p className="text-[11px] text-gray-400 font-normal">{new Date(item.girisTarihi).toLocaleDateString("tr-TR")}</p>
                        </div>
                      </div>
                    </td>
                    <td className="py-4 px-6">
                      {item.cikisTarihi ? (
                        <div className="flex items-center gap-2 text-orange-400 font-medium">
                          <ArrowUpRight className="w-4 h-4" />
                          <div>
                            <span>{new Date(item.cikisTarihi).toLocaleTimeString("tr-TR", { hour: "2-digit", minute: "2-digit", second: "2-digit" })}</span>
                            <p className="text-[11px] text-gray-400 font-normal">{new Date(item.cikisTarihi).toLocaleDateString("tr-TR")}</p>
                          </div>
                        </div>
                      ) : (
                        <span className="text-gray-500 italic text-xs">-</span>
                      )}
                    </td>
                    <td className="py-4 px-6 font-medium text-gray-300">
                      {item.izinVerildi ? formatDuration(item.girisTarihi, item.cikisTarihi) : "-"}
                    </td>
                    <td className="py-4 px-6">
                      <div className="flex items-center gap-2">
                        {item.izinVerildi ? (
                          <span className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-bold bg-emerald-500/10 text-emerald-400 border border-emerald-500/20">
                            <CheckCircle2 className="w-3.5 h-3.5" /> Giriş Onaylandı
                          </span>
                        ) : (
                          <span className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-bold bg-red-500/10 text-red-400 border border-red-500/20">
                            <XCircle className="w-3.5 h-3.5" /> {item.redSebebi || "Reddedildi"}
                          </span>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
