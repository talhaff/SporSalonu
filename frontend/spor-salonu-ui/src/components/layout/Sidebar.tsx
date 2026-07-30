"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { 
  LayoutDashboard, 
  Users, 
  CreditCard, 
  QrCode, 
  Clock,
  LogOut, 
  Bell
} from "lucide-react";
import { api } from "@/lib/api";

const menuItems = [
  { name: "Dashboard", href: "/", icon: LayoutDashboard },
  { name: "Müşteriler", href: "/uyeler", icon: Users },
  { name: "Kasa & Finans", href: "/finans", icon: CreditCard },
  { name: "Check-in", href: "/checkin", icon: QrCode },
  { name: "Geçiş Geçmişi", href: "/checkin-gecmisi", icon: Clock },
];

export default function Sidebar() {
  const pathname = usePathname();
  const router = useRouter();
  const queryClient = useQueryClient();

  const { data: notifications } = useQuery({
    queryKey: ["notifications"],
    queryFn: async () => {
      const res = await api.get("/notifications");
      return res.data.data;
    },
    refetchInterval: 30000 // Her 30 saniyede bir kontrol et
  });

  const markAsReadMutation = useMutation({
    mutationFn: async () => {
      await api.put("/notifications/tumunu-okundu");
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["notifications"] });
    }
  });

  const handleLogout = async () => {
    try {
      await api.post("/auth/cikis");
      router.push("/login");
    } catch (error) {
      console.error(error);
    }
  };

  const unreadCount = notifications?.length || 0;

  return (
    <aside className="w-64 h-screen fixed left-0 top-0 border-r border-white/10 glass-panel !rounded-none flex flex-col">
      <div className="p-6">
        <h2 className="text-3xl font-black tracking-tighter">
          Vip<span className="text-primary">Gym</span>
        </h2>
      </div>

      <nav className="flex-1 px-4 space-y-2 mt-4">
        {menuItems.map((item) => {
          const isActive = pathname === item.href;
          const Icon = item.icon;
          return (
            <Link
              key={item.href}
              href={item.href}
              className={`flex items-center gap-3 px-4 py-3 rounded-xl transition-all ${
                isActive
                  ? "bg-primary/20 text-primary border border-primary/30"
                  : "text-gray-400 hover:text-white hover:bg-white/5"
              }`}
            >
              <Icon className="w-5 h-5" />
              <span className="font-medium">{item.name}</span>
            </Link>
          );
        })}
      </nav>

      <div className="p-4 space-y-2 border-t border-white/10">
        <button 
          onClick={() => unreadCount > 0 && markAsReadMutation.mutate()}
          className="w-full flex items-center gap-3 px-4 py-3 rounded-xl text-gray-400 hover:text-white hover:bg-white/5 transition-all"
        >
          <Bell className="w-5 h-5" />
          <span className="font-medium text-left flex-1">Bildirimler</span>
          {unreadCount > 0 && (
            <span className="bg-primary text-white text-xs font-bold px-2 py-0.5 rounded-full animate-pulse">
              {unreadCount}
            </span>
          )}
        </button>

        <button
          onClick={handleLogout}
          className="w-full flex items-center gap-3 px-4 py-3 rounded-xl text-red-400 hover:text-red-300 hover:bg-red-500/10 transition-all"
        >
          <LogOut className="w-5 h-5" />
          <span className="font-medium">Çıkış Yap</span>
        </button>
      </div>
    </aside>
  );
}
