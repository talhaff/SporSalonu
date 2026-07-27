import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { X, Loader2 } from "lucide-react";
import { api } from "@/lib/api";

interface AddMemberModalProps {
  onClose: () => void;
}

export default function AddMemberModal({ onClose }: AddMemberModalProps) {
  const queryClient = useQueryClient();

  // Form State
  const [ad, setAd] = useState("");
  const [soyad, setSoyad] = useState("");
  const [telefon, setTelefon] = useState("");
  const [email, setEmail] = useState("");
  
  // Paket Seçim State
  const [seciliPaket, setSeciliPaket] = useState<number | "">("");
  const [indirimTutari, setIndirimTutari] = useState<number>(0);
  const [pesinatTutari, setPesinatTutari] = useState<number>(0);

  const [error, setError] = useState("");

  // Paketleri Getir
  const { data: packages, isLoading: loadingPackages } = useQuery({
    queryKey: ["packages"],
    queryFn: async () => {
      const res = await api.get("/packages");
      return res.data.data;
    },
  });

  const mutation = useMutation({
    mutationFn: async () => {
      // 1. Üyeyi Kaydet
      const memberRes = await api.post("/members", { ad, soyad, telefon, email });
      const memberId = memberRes.data.data.id;

      // 2. Eğer paket seçildiyse, aboneliği başlat
      if (seciliPaket) {
        await api.post("/subscriptions", {
          memberId,
          packageId: Number(seciliPaket),
          baslangicTarihi: new Date().toISOString(),
          indirimTutari: Number(indirimTutari),
          pesinatTutari: Number(pesinatTutari),
          aciklama: "Yeni kayıt paketi"
        });
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["members"] });
      onClose();
    },
    onError: (err: any) => {
      setError(err.response?.data?.mesaj || "Kayıt işlemi sırasında bir hata oluştu.");
    }
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    mutation.mutate();
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div className="glass-panel w-full max-w-2xl max-h-[90vh] overflow-y-auto bg-slate-900/90 shadow-2xl relative">
        <div className="sticky top-0 bg-slate-900/95 p-6 border-b border-white/10 flex justify-between items-center z-10 backdrop-blur-md">
          <h2 className="text-2xl font-bold">Yeni Üye Ekle</h2>
          <button onClick={onClose} className="p-2 hover:bg-white/10 rounded-full transition-colors">
            <X className="w-5 h-5 text-gray-400" />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="p-6 space-y-8">
          {error && (
            <div className="p-3 bg-red-500/10 border border-red-500/50 rounded-lg text-red-400 text-sm">
              {error}
            </div>
          )}

          <div className="space-y-4">
            <h3 className="text-lg font-semibold text-primary border-b border-white/10 pb-2">Kişisel Bilgiler</h3>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm text-gray-400 mb-1">Ad *</label>
                <input required type="text" value={ad} onChange={e => setAd(e.target.value)}
                  className="w-full bg-white/5 border border-white/10 rounded-xl py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-primary/50" />
              </div>
              <div>
                <label className="block text-sm text-gray-400 mb-1">Soyad *</label>
                <input required type="text" value={soyad} onChange={e => setSoyad(e.target.value)}
                  className="w-full bg-white/5 border border-white/10 rounded-xl py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-primary/50" />
              </div>
              <div>
                <label className="block text-sm text-gray-400 mb-1">Telefon *</label>
                <input required type="text" value={telefon} onChange={e => setTelefon(e.target.value)} placeholder="05551234567"
                  className="w-full bg-white/5 border border-white/10 rounded-xl py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-primary/50" />
              </div>
              <div>
                <label className="block text-sm text-gray-400 mb-1">E-posta (Opsiyonel)</label>
                <input type="email" value={email} onChange={e => setEmail(e.target.value)}
                  className="w-full bg-white/5 border border-white/10 rounded-xl py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-primary/50" />
              </div>
            </div>
          </div>

          <div className="space-y-4">
            <h3 className="text-lg font-semibold text-emerald-400 border-b border-white/10 pb-2">Abonelik & Paket (Opsiyonel)</h3>
            <div>
              <label className="block text-sm text-gray-400 mb-1">Paket Seçimi</label>
              <select 
                value={seciliPaket} 
                onChange={e => setSeciliPaket(e.target.value ? Number(e.target.value) : "")}
                className="w-full bg-slate-800 border border-white/10 rounded-xl py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-emerald-500/50"
              >
                <option value="">-- Paket Seçmeden Sadece Üye Kaydı Yap --</option>
                {packages?.map((p: any) => (
                  <option key={p.id} value={p.id}>{p.ad} - ₺{p.fiyat}</option>
                ))}
              </select>
            </div>

            {seciliPaket && (
              <div className="grid grid-cols-2 gap-4 animate-in fade-in slide-in-from-top-2">
                <div>
                  <label className="block text-sm text-gray-400 mb-1">İndirim Tutarı (₺)</label>
                  <input type="number" min="0" value={indirimTutari} onChange={e => setIndirimTutari(Number(e.target.value))}
                    className="w-full bg-white/5 border border-white/10 rounded-xl py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-emerald-500/50" />
                </div>
                <div>
                  <label className="block text-sm text-gray-400 mb-1">Alınan Peşinat (₺)</label>
                  <input type="number" min="0" value={pesinatTutari} onChange={e => setPesinatTutari(Number(e.target.value))}
                    className="w-full bg-white/5 border border-white/10 rounded-xl py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-emerald-500/50" />
                  <p className="text-xs text-emerald-400/70 mt-1">Geri kalan tutar cari hesaba borç yazılır.</p>
                </div>
              </div>
            )}
          </div>

          <div className="pt-4 border-t border-white/10 flex justify-end gap-3">
            <button type="button" onClick={onClose} className="px-5 py-2.5 rounded-xl border border-white/10 text-gray-300 hover:bg-white/5 transition-colors">
              İptal
            </button>
            <button type="submit" disabled={mutation.isPending} className="px-5 py-2.5 rounded-xl bg-primary hover:bg-primary-hover text-white font-semibold shadow-lg transition-colors flex items-center gap-2 disabled:opacity-70">
              {mutation.isPending ? <Loader2 className="w-5 h-5 animate-spin" /> : "Kaydet ve Tamamla"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
