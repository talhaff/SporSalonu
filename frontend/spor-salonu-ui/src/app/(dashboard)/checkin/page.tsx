"use client";

import { useState } from "react";
import { QrCode, Search, CheckCircle, XCircle } from "lucide-react";
import { api } from "@/lib/api";

export default function CheckInPage() {
  const [telefon, setTelefon] = useState("");
  const [status, setStatus] = useState<"idle" | "success" | "error" | "warning">("idle");
  const [message, setMessage] = useState("");
  const [memberData, setMemberData] = useState<any>(null);

  const handleCheckIn = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!telefon) return;
    
    setStatus("idle");
    setMemberData(null);
    setMessage("");

    try {
      // 1. Önce telefonla üyeyi bul
      const userRes = await api.get(`/members/telefon/${telefon}`);
      const member = userRes.data.data;
      
      // 2. Bulunan üyenin ID'si ile check-in yap
      const checkinRes = await api.post("/checkin", { memberId: member.id });
      const result = checkinRes.data.data;
      
      setMemberData(member);
      
      if (result.izinVerildi) {
        setStatus("success");
        setMessage("Giriş Onaylandı! İyi sporlar.");
      } else {
        setStatus("error");
        setMessage(result.mesaj || "Giriş reddedildi. Üyeliğiniz pasif veya süresi dolmuş olabilir.");
      }
    } catch (err: any) {
      setStatus("error");
      setMessage(err.response?.data?.mesaj || "Üye bulunamadı veya bir hata oluştu.");
    }
  };

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-700 max-w-4xl mx-auto mt-10">
      <header className="text-center mb-10">
        <div className="inline-flex items-center justify-center p-4 bg-primary/10 rounded-full mb-4">
          <QrCode className="w-12 h-12 text-primary" />
        </div>
        <h1 className="text-4xl font-black mb-2">Hızlı Check-in</h1>
        <p className="text-gray-400">Üyenin telefon numarasını girerek turnike girişini onaylayın.</p>
      </header>

      <div className="glass-panel p-8 max-w-xl mx-auto">
        <form onSubmit={handleCheckIn} className="space-y-6">
          <div className="relative">
            <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-6 h-6 text-gray-400" />
            <input
              type="text"
              placeholder="Telefon Numarası (Örn: 05551234567)"
              className="w-full bg-white/5 border border-white/10 rounded-2xl py-4 pl-14 pr-4 text-lg focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all text-white placeholder-gray-500 font-medium tracking-wider"
              value={telefon}
              onChange={(e) => setTelefon(e.target.value)}
              autoFocus
            />
          </div>
          <button
            type="submit"
            className="w-full bg-primary hover:bg-primary-hover text-white font-bold py-4 rounded-2xl transition-all shadow-lg shadow-primary/30 text-lg"
          >
            Sorgula ve Giriş Yap
          </button>
        </form>

        {/* Sonuç Alanı */}
        {status !== "idle" && (
          <div className={`mt-8 p-6 rounded-2xl border ${
            status === "success" 
              ? "bg-emerald-500/10 border-emerald-500/30 text-emerald-400" 
              : "bg-red-500/10 border-red-500/30 text-red-400"
          }`}>
            <div className="flex items-start gap-4">
              {status === "success" ? (
                <CheckCircle className="w-8 h-8 flex-shrink-0 mt-1" />
              ) : (
                <XCircle className="w-8 h-8 flex-shrink-0 mt-1" />
              )}
              
              <div>
                <h3 className="text-xl font-bold mb-1">
                  {status === "success" ? "GİRİŞ ONAYLANDI" : "GİRİŞ REDDEDİLDİ"}
                </h3>
                <p className="opacity-90 font-medium mb-3">{message}</p>
                
                {memberData && (
                  <div className="mt-4 pt-4 border-t border-current/20 space-y-2">
                    <p><span className="opacity-70">Ad Soyad:</span> <strong className="ml-2">{memberData.adSoyad}</strong></p>
                    <p><span className="opacity-70">Bakiye:</span> <strong className="ml-2">₺{memberData.kalanBakiye}</strong></p>
                  </div>
                )}
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
