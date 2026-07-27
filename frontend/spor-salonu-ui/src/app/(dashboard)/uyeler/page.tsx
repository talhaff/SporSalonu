"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { Users, Plus, Search, Loader2 } from "lucide-react";
import AddMemberModal from "@/components/members/AddMemberModal";

export default function MembersPage() {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const { data, isLoading, error } = useQuery({
    queryKey: ["members"],
    queryFn: async () => {
      const res = await api.get("/members");
      return res.data.data;
    },
  });

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-700">
      <header className="flex justify-between items-end">
        <div>
          <h1 className="text-4xl font-black mb-2 flex items-center gap-3">
            <Users className="w-8 h-8 text-primary" /> Müşteriler
          </h1>
          <p className="text-gray-400">Salona kayıtlı tüm üyeler ve üyelik durumları.</p>
        </div>
        <button 
          onClick={() => setIsModalOpen(true)}
          className="bg-primary hover:bg-primary-hover px-6 py-2.5 rounded-xl font-medium transition-all flex items-center gap-2 shadow-lg shadow-primary/25"
        >
          <Plus className="w-5 h-5" /> Yeni Üye Ekle
        </button>
      </header>

      <div className="glass-panel p-6">
        <div className="flex justify-between items-center mb-6">
          <div className="relative w-72">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
            <input 
              type="text" 
              placeholder="Üye adı veya telefon..." 
              className="w-full bg-white/5 border border-white/10 rounded-xl py-2.5 pl-10 pr-4 text-sm focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all text-white placeholder-gray-400"
            />
          </div>
        </div>

        {isLoading ? (
          <div className="flex justify-center items-center py-20 text-gray-400">
            <Loader2 className="w-8 h-8 animate-spin" />
          </div>
        ) : error ? (
          <div className="text-red-400 text-center py-10">Veriler yüklenirken hata oluştu.</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="border-b border-white/10 text-gray-400 text-sm">
                  <th className="pb-4 pl-4 font-medium">Ad Soyad</th>
                  <th className="pb-4 font-medium">Telefon</th>
                  <th className="pb-4 font-medium">Aktif Paket</th>
                  <th className="pb-4 font-medium">Bitiş Tarihi</th>
                  <th className="pb-4 font-medium text-right pr-4">Bakiye</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-white/5 text-sm">
                {data?.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="text-center py-8 text-gray-500">
                      Henüz kayıtlı üye bulunmamaktadır.
                    </td>
                  </tr>
                ) : (
                  data?.map((member: any) => (
                    <tr key={member.id} className="hover:bg-white/5 transition-colors group">
                      <td className="py-4 pl-4 font-medium flex items-center gap-3">
                        <div className="w-8 h-8 rounded-full bg-gradient-to-tr from-blue-500 to-purple-500 flex items-center justify-center font-bold text-xs">
                          {member.adSoyad.substring(0, 2).toUpperCase()}
                        </div>
                        {member.adSoyad}
                      </td>
                      <td className="py-4 text-gray-300">{member.telefon}</td>
                      <td className="py-4">
                        {member.aktifPaket ? (
                          <span className="px-2.5 py-1 bg-white/10 rounded-md text-xs font-medium border border-white/5">
                            {member.aktifPaket}
                          </span>
                        ) : (
                          <span className="text-gray-500">-</span>
                        )}
                      </td>
                      <td className="py-4">
                        {member.uyelikBitisi 
                          ? new Date(member.uyelikBitisi).toLocaleDateString('tr-TR') 
                          : <span className="text-gray-500">-</span>}
                      </td>
                      <td className="py-4 text-right pr-4">
                        <span className={`font-semibold ${member.kalanBakiye > 0 ? 'text-red-400' : 'text-emerald-400'}`}>
                          ₺{member.kalanBakiye.toLocaleString('tr-TR', { minimumFractionDigits: 2 })}
                        </span>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {isModalOpen && <AddMemberModal onClose={() => setIsModalOpen(false)} />}
    </div>
  );
}
