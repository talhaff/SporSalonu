"use client";

import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { CheckCircle2, XCircle } from "lucide-react";

interface CheckInResult {
  izinVerildi: boolean;
  mesaj: string;
  uyeAdSoyad: string | null;
  uyelikBitisi: string | null;
}

export default function CheckInToastListener() {
  const [toast, setToast] = useState<CheckInResult | null>(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5000/hubs/checkin", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();

    connection.on("OnCheckIn", (data: CheckInResult) => {
      setToast(data);
      // 5 saniye sonra gizle
      setTimeout(() => setToast(null), 5000);
    });

    connection.start().catch(err => console.error("SignalR Bağlantı Hatası:", err));

    return () => {
      connection.stop();
    };
  }, []);

  if (!toast) return null;

  return (
    <div className="fixed bottom-6 right-6 z-50 animate-in slide-in-from-right-4 fade-in duration-300">
      <div className={`flex items-start gap-3 p-4 rounded-xl shadow-2xl border backdrop-blur-md max-w-sm w-full ${
        toast.izinVerildi 
          ? "bg-emerald-900/90 border-emerald-500/30 text-emerald-50" 
          : "bg-red-900/90 border-red-500/30 text-red-50"
      }`}>
        <div className="mt-0.5">
          {toast.izinVerildi ? (
            <CheckCircle2 className="w-6 h-6 text-emerald-400" />
          ) : (
            <XCircle className="w-6 h-6 text-red-400" />
          )}
        </div>
        <div>
          <h4 className="font-bold text-lg mb-1">{toast.uyeAdSoyad || "Bilinmeyen Kart"}</h4>
          <p className="text-sm opacity-90">{toast.mesaj}</p>
        </div>
      </div>
    </div>
  );
}
