"use client";

import { useState, useEffect } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { X, Loader2 } from "lucide-react";
import { api } from "@/lib/api";

interface EditMemberModalProps {
  memberId: number;
  onClose: () => void;
}

export default function EditMemberModal({ memberId, onClose }: EditMemberModalProps) {
  const queryClient = useQueryClient();

  // Form State
  const [ad, setAd] = useState("");
  const [soyad, setSoyad] = useState("");
  const [telefon, setTelefon] = useState("");
  const [email, setEmail] = useState("");
  const [cardUid, setCardUid] = useState("");
  const [error, setError] = useState("");

  // Fetch Member Details
  const { data: member, isLoading, error: fetchError } = useQuery({
    queryKey: ["member", memberId],
    queryFn: async () => {
      const res = await api.get(`/members/${memberId}`);
      return res.data.data;
    },
  });

  // Populate form when data is loaded
  useEffect(() => {
    if (member) {
      setAd(member.ad || "");
      setSoyad(member.soyad || "");
      setTelefon(member.telefon || "");
      setEmail(member.email || "");
      setCardUid(member.cardUid || "");
    }
  }, [member]);

  const mutation = useMutation({
    mutationFn: async () => {
      await api.put(`/members/${memberId}`, {
        ad,
        soyad,
        telefon,
        email,
        cardUid,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["members"] });
      queryClient.invalidateQueries({ queryKey: ["member", memberId] });
      onClose();
    },
    onError: (err: any) => {
      setError(err.response?.data?.mesaj || "Güncelleme işlemi sırasında bir hata oluştu.");
    },
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
          <h2 className="text-2xl font-bold">Üye Bilgilerini Güncelle</h2>
          <button onClick={onClose} className="p-2 hover:bg-white/10 rounded-full transition-colors">
            <X className="w-5 h-5 text-gray-400" />
          </button>
        </div>

        {isLoading ? (
          <div className="flex justify-center items-center py-20 text-gray-400">
            <Loader2 className="w-8 h-8 animate-spin" />
          </div>
        ) : fetchError ? (
          <div className="p-6 text-center text-red-400">
            Üye detayları yüklenemedi. Lütfen tekrar deneyin.
          </div>
        ) : (
          <form onSubmit={handleSubmit} className="p-6 space-y-6">
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
                  <input
                    required
                    type="text"
                    value={ad}
                    onChange={(e) => setAd(e.target.value)}
                    className="w-full bg-white/5 border border-white/10 rounded-xl py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-primary/50"
                  />
                </div>
                <div>
                  <label className="block text-sm text-gray-400 mb-1">Soyad *</label>
                  <input
                    required
                    type="text"
                    value={soyad}
                    onChange={(e) => setSoyad(e.target.value)}
                    className="w-full bg-white/5 border border-white/10 rounded-xl py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-primary/50"
                  />
                </div>
                <div>
                  <label className="block text-sm text-gray-400 mb-1">Telefon *</label>
                  <input
                    required
                    type="text"
                    value={telefon}
                    onChange={(e) => setTelefon(e.target.value)}
                    placeholder="05551234567"
                    className="w-full bg-white/5 border border-white/10 rounded-xl py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-primary/50"
                  />
                </div>
                <div>
                  <label className="block text-sm text-gray-400 mb-1">E-posta (Opsiyonel)</label>
                  <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    className="w-full bg-white/5 border border-white/10 rounded-xl py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-primary/50"
                  />
                </div>
                <div className="col-span-2">
                  <label className="block text-sm text-gray-400 mb-1">RFID Kart ID</label>
                  <input
                    type="text"
                    value={cardUid}
                    onChange={(e) => setCardUid(e.target.value)}
                    placeholder="Kart okutun..."
                    className="w-full bg-white/5 border border-white/10 rounded-xl py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-primary/50"
                  />
                </div>
              </div>
            </div>

            <div className="pt-4 border-t border-white/10 flex justify-end gap-3">
              <button
                type="button"
                onClick={onClose}
                className="px-5 py-2.5 rounded-xl border border-white/10 text-gray-300 hover:bg-white/5 transition-colors"
              >
                İptal
              </button>
              <button
                type="submit"
                disabled={mutation.isPending}
                className="px-5 py-2.5 rounded-xl bg-primary hover:bg-primary-hover text-white font-semibold shadow-lg transition-colors flex items-center gap-2 disabled:opacity-70"
              >
                {mutation.isPending ? <Loader2 className="w-5 h-5 animate-spin" /> : "Güncellemeleri Kaydet"}
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  );
}
