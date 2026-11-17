const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export async function loginUser({ email, password }) {
  if (!API_BASE_URL) throw new Error("base url belum dikonfigurasi");

  let response;
  try {
    response = await fetch(`${API_BASE_URL}/api/v1/auth/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      body: JSON.stringify({
        email: email.trim(),
        password,
      }),
    });
  } catch {
    throw new Error(
      "Tidak dapat menghubungi API, periksa backend URL dan konfigurasi CORS.",
    );
  }

  const contentType = response.headers.get("content-type") ?? "";
  const isJson = contentType.includes("json");
  const data = isJson ? await response.json().catch(() => null) : null;

  if (!response.ok) {
    if (response.status == 401) {
      throw new Error("Email atau password salah");
    }

    if (response.status == 400) {
      const errors = Object.values(data?.errors ?? {}).flat();
      throw new Error(errors[0] ?? "Data login tidak valid");
    }

    if (response.status == 429) {
      throw new Error("Terlalu banyak percobaan login");
    }

    throw new Error(
      response.status >= 500
        ? "Server sedang mengalami gangguan"
        : "login gagal. silahkan coba lagi",
    );
  }

  if (!data?.user?.id || !data?.user?.email) {
    throw new Error("format response login API tidak sesuai");
  }

  return data;
}
