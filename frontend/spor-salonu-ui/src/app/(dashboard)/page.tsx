"use client";

import Link from "next/link";
import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { Users, CreditCard, Activity, ArrowUpRight, Loader2, DoorOpen, LogOut } from "lucide-react";

export default function DashboardPage() {
  const { data: dashboardData, isLoading, error } = useQuery({
    queryKey: ["dashboard-ozet"],
    queryFn: async () => {
      const res = await api.get("/dashboard/ozet");
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
    return <div className="text-red-400 p-6 bg-red-500/10 rounded-xl">Veriler yüklenirken hata oluştu.</div>;
  }

  const handleManualOverride = async (isEntry: boolean) => {
    try {
      await api.post("/hardware/manual-override", { isEntry });
      // Toast gösterilebilir (SignalR zaten OnCheckIn veya success atarsa o da kullanılabilir ama override'da toast iyi olur)
      alert(`Turnike ${isEntry ? "Giriş" : "Çıkış"} kapısı açıldı.`);
    } catch (err) {
      alert("Turnike açılamadı.");
    }
  };

  const stats = [
    { name: "Aktif Üyeler", value: dashboardData?.stats?.aktifUyeler || 0, icon: Users, color: "text-blue-400" },
    { name: "Aylık Gelir", value: `₺${dashboardData?.stats?.aylikGelir?.toLocaleString('tr-TR') || 0}`, icon: CreditCard, color: "text-emerald-400" },
    { name: "Bugünkü Girişler", value: dashboardData?.stats?.bugunkuGirisler || 0, icon: Activity, color: "text-purple-400" },
  ];

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-700">
      <header className="flex justify-between items-end">
        <div>
          <h1 className="text-4xl font-black mb-2">Genel Bakış</h1>
          <p className="text-gray-400">Hoş geldiniz. Salonunuzun anlık durumu.</p>
        </div>
        <div className="flex items-center gap-3">
          <button 
            onClick={() => handleManualOverride(true)}
            className="bg-emerald-500/20 text-emerald-400 border border-emerald-500/50 hover:bg-emerald-500/40 px-5 py-2.5 rounded-xl font-medium transition-all flex items-center gap-2"
          >
            <DoorOpen className="w-4 h-4" /> Turnike Giriş
          </button>
          <button 
            onClick={() => handleManualOverride(false)}
            className="bg-orange-500/20 text-orange-400 border border-orange-500/50 hover:bg-orange-500/40 px-5 py-2.5 rounded-xl font-medium transition-all flex items-center gap-2"
          >
            <LogOut className="w-4 h-4" /> Turnike Çıkış
          </button>
          <Link href="/uyeler" className="bg-primary/20 text-primary border border-primary/50 hover:bg-primary/40 px-6 py-2.5 rounded-xl font-medium transition-all flex items-center gap-2 ml-4">
            Yeni Üye Ekle <ArrowUpRight className="w-4 h-4" />
          </Link>
        </div>
      </header>

      {/* İstatistik Kartları */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {stats.map((stat, i) => (
          <div key={i} className="glass-panel p-6 flex items-start justify-between group hover:border-white/20 transition-all">
            <div>
              <p className="text-gray-400 font-medium mb-1">{stat.name}</p>
              <h3 className="text-3xl font-bold">{stat.value}</h3>
            </div>
            <div className={`p-3 bg-white/5 rounded-xl ${stat.color} group-hover:scale-110 transition-transform`}>
              <stat.icon className="w-6 h-6" />
            </div>
          </div>
        ))}
      </div>

      {/* Son Hareketler & Bildirimler Alanı */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="glass-panel p-6">
          <div className="flex justify-between items-center mb-6">
            <h3 className="text-xl font-bold">Son Check-in İşlemleri</h3>
            <Link href="/checkin-gecmisi" className="text-sm text-primary hover:underline">Tümünü Gör</Link>
          </div>
          <div className="space-y-4">
            {dashboardData?.sonCheckInler?.length === 0 ? (
              <p className="text-gray-500 text-sm">Bugün henüz giriş yapan üye yok.</p>
            ) : (
              dashboardData?.sonCheckInler?.map((log: any, i: number) => (
                <div key={i} className="flex items-center justify-between p-4 bg-white/5 rounded-xl border border-white/5">
                  <div className="flex items-center gap-4">
                    <div className="w-10 h-10 rounded-full bg-gradient-to-tr from-blue-500 to-purple-500 flex items-center justify-center font-bold text-xs">
                      {log.uyeAdSoyad ? log.uyeAdSoyad.substring(0,2).toUpperCase() : "?"}
                    </div>
                    <div>
                      <p className="font-semibold">{log.uyeAdSoyad || "Bilinmeyen Üye"}</p>
                      <p className="text-xs text-gray-400">
                        {log.girisTarihi ? `Giriş: ${new Date(log.girisTarihi).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' })}` : 'Bugün'}
                        {log.cikisTarihi ? ` • Çıkış: ${new Date(log.cikisTarihi).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' })}` : ''}
                      </p>
                    </div>
                  </div>
                  <span className={`px-3 py-1 text-xs font-bold rounded-full border ${
                    log.izinVerildi 
                      ? "bg-emerald-500/20 text-emerald-400 border-emerald-500/20" 
                      : "bg-red-500/20 text-red-400 border-red-500/20"
                  }`}>
                    {log.izinVerildi ? "Onaylandı" : "Reddedildi"}
                  </span>
                </div>
              ))
            )}
          </div>
        </div>

        <div className="glass-panel p-6">
          <div className="flex justify-between items-center mb-6">
            <h3 className="text-xl font-bold">Yaklaşan Üyelik Bitişleri</h3>
            <Link href="/uyeler" className="text-sm text-primary hover:underline">Tümünü Gör</Link>
          </div>
          <div className="space-y-4">
            {dashboardData?.yaklasanBitisler?.length === 0 ? (
              <p className="text-gray-500 text-sm">Yakın zamanda bitişi yaklaşan üyelik yok.</p>
            ) : (
              dashboardData?.yaklasanBitisler?.map((sub: any, i: number) => {
                const diffDays = Math.ceil((new Date(sub.bitisTarihi).getTime() - new Date().getTime()) / (1000 * 3600 * 24));
                return (
                  <div key={i} className="flex items-center justify-between p-4 bg-white/5 rounded-xl border border-white/5">
                    <div className="flex items-center gap-4">
                      <div className="w-10 h-10 rounded-full bg-gradient-to-tr from-amber-500 to-orange-500 flex items-center justify-center font-bold text-xs">
                        {sub.uyeAdSoyad.substring(0,2).toUpperCase()}
                      </div>
                      <div>
                        <p className="font-semibold">{sub.uyeAdSoyad}</p>
                        <p className={`text-xs ${diffDays < 0 ? 'text-red-400' : 'text-amber-400'}`}>
                          {diffDays < 0 ? `${Math.abs(diffDays)} gün gecikti` : `${diffDays} gün kaldı`}
                        </p>
                      </div>
                    </div>
                    <Link href="/uyeler" className="px-4 py-1.5 bg-white/10 hover:bg-white/20 text-sm font-medium rounded-lg transition-all text-center">
                      İncele
                    </Link>
                  </div>
                );
              })
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
