"use client";

import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { 
  ArrowUpRight, 
  ArrowDownRight, 
  Wallet,
  TrendingUp,
  Clock,
  Loader2
} from "lucide-react";

export default function FinansPage() {
  const { data: financeData, isLoading, error } = useQuery({
    queryKey: ["finance-islemler"],
    queryFn: async () => {
      const res = await api.get("/finance/islemler");
      return res.data.data;
    },
  });

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-full min-h-[60vh]">
        <Loader2 className="w-10 h-10 animate-spin text-primary" />
      </div>
    );
  }

  if (error) {
    return <div className="text-red-400 p-6 bg-red-500/10 rounded-xl">Finans verileri yüklenirken hata oluştu.</div>;
  }

  const toplamTahsilat = financeData?.toplamTahsilat || 0;
  const bekleyenAlacaklar = financeData?.bekleyenAlacaklar || 0;
  const islemler = financeData?.sonIslemler || [];

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-700">
      <header className="flex justify-between items-end">
        <div>
          <h1 className="text-4xl font-black mb-2">Kasa & Finans</h1>
          <p className="text-gray-400">Tüm gelir ve alacak durumları.</p>
        </div>
        <div className="flex gap-3">
          <button className="bg-white/5 hover:bg-white/10 border border-white/10 px-6 py-2.5 rounded-xl font-medium transition-all">
            Rapor İndir
          </button>
        </div>
      </header>

      {/* Özet Kartları */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="glass-panel p-6 relative overflow-hidden group">
          <div className="absolute top-0 right-0 p-6 opacity-10 group-hover:scale-110 transition-transform group-hover:opacity-20">
            <TrendingUp className="w-24 h-24 text-emerald-400" />
          </div>
          <div className="relative z-10">
            <div className="w-12 h-12 bg-emerald-500/20 text-emerald-400 rounded-2xl flex items-center justify-center mb-4 border border-emerald-500/20">
              <ArrowUpRight className="w-6 h-6" />
            </div>
            <p className="text-gray-400 font-medium mb-1">Aylık Toplam Tahsilat</p>
            <h3 className="text-3xl font-bold">₺{toplamTahsilat.toLocaleString('tr-TR')}</h3>
          </div>
        </div>

        <div className="glass-panel p-6 relative overflow-hidden group">
          <div className="absolute top-0 right-0 p-6 opacity-10 group-hover:scale-110 transition-transform group-hover:opacity-20">
            <Clock className="w-24 h-24 text-amber-400" />
          </div>
          <div className="relative z-10">
            <div className="w-12 h-12 bg-amber-500/20 text-amber-400 rounded-2xl flex items-center justify-center mb-4 border border-amber-500/20">
              <ArrowDownRight className="w-6 h-6" />
            </div>
            <p className="text-gray-400 font-medium mb-1">Bekleyen Alacaklar (Aktif)</p>
            <h3 className="text-3xl font-bold">₺{bekleyenAlacaklar.toLocaleString('tr-TR')}</h3>
          </div>
        </div>

        <div className="glass-panel p-6 relative overflow-hidden group border-primary/30">
          <div className="absolute top-0 right-0 p-6 opacity-10 group-hover:scale-110 transition-transform group-hover:opacity-20">
            <Wallet className="w-24 h-24 text-primary" />
          </div>
          <div className="relative z-10">
            <div className="w-12 h-12 bg-primary/20 text-primary rounded-2xl flex items-center justify-center mb-4 border border-primary/30">
              <Wallet className="w-6 h-6" />
            </div>
            <p className="text-gray-400 font-medium mb-1">Net Kasa (Örnek)</p>
            <h3 className="text-3xl font-bold">₺{toplamTahsilat.toLocaleString('tr-TR')}</h3>
          </div>
        </div>
      </div>

      {/* İşlem Geçmişi */}
      <div className="glass-panel">
        <div className="p-6 border-b border-white/5 flex justify-between items-center">
          <h3 className="text-xl font-bold">Son Finansal Hareketler</h3>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-white/5 border-b border-white/5 text-gray-400 text-sm">
                <th className="p-4 font-medium">Tarih</th>
                <th className="p-4 font-medium">İşlem Türü</th>
                <th className="p-4 font-medium">Üye</th>
                <th className="p-4 font-medium">Açıklama</th>
                <th className="p-4 font-medium text-right">Tutar</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-white/5">
              {islemler.length === 0 ? (
                <tr>
                  <td colSpan={5} className="p-8 text-center text-gray-500">
                    Henüz finansal işlem bulunmamaktadır.
                  </td>
                </tr>
              ) : (
                islemler.map((tx: any) => (
                  <tr key={tx.id} className="hover:bg-white/5 transition-colors">
                    <td className="p-4 text-sm text-gray-400">
                      {new Date(tx.islemTarihi).toLocaleDateString("tr-TR")}
                    </td>
                    <td className="p-4">
                      <span className={`px-3 py-1 rounded-full text-xs font-bold border ${
                        tx.islemTipi === "Odeme" 
                          ? "bg-emerald-500/10 text-emerald-400 border-emerald-500/20"
                          : "bg-red-500/10 text-red-400 border-red-500/20"
                      }`}>
                        {tx.islemTipi === "Odeme" ? "Tahsilat" : "Borçlandırma"}
                      </span>
                    </td>
                    <td className="p-4 font-medium text-white">
                      {tx.uyeAdSoyad}
                    </td>
                    <td className="p-4 text-sm text-gray-400">
                      {tx.aciklama || "-"}
                    </td>
                    <td className={`p-4 text-right font-bold ${
                      tx.islemTipi === "Odeme" ? "text-emerald-400" : "text-red-400"
                    }`}>
                      {tx.islemTipi === "Odeme" ? "+" : "-"}₺{tx.tutar.toLocaleString("tr-TR")}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
