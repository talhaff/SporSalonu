import axios from "axios";

// HTTP-Only Cookie bazlı kimlik doğrulama olduğu için withCredentials = true kullanıyoruz.
export const api = axios.create({
  baseURL: "http://localhost:5000/api",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});

// Response Interceptor: 401 (Unauthorized) hatası yakalanırsa Login sayfasına yönlendirilebilir.
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      if (typeof window !== "undefined" && window.location.pathname !== "/login") {
        window.location.href = "/login";
      }
    }
    return Promise.reject(error);
  }
);
