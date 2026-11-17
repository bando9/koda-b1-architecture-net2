import { useState } from "react";
import { registerUser } from "../services/auth-api";
import FormFields from "../components/forms/form-fields";

function RegisterPage() {
  const [formData, setFormData] = useState({
    displayName: "",
    email: "",
    password: "",
  });

  const [isLoading, setIsLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const [registeredUser, setRegisteredUser] = useState(null);

  function handleChange(event) {
    const { name, value } = event.target;

    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));

    setErrorMessage("");
    setSuccessMessage("");
    setRegisteredUser(null);
  }

  async function handleSubmit(event) {
    event.preventDefault();

    if (isLoading) return;

    setErrorMessage("");
    setSuccessMessage("");
    setRegisteredUser(null);

    if (
      !formData.displayName.trim() ||
      !formData.email.trim() ||
      !formData.password
    ) {
      setErrorMessage("Email dan password wajib diisi.");
      return;
    }

    setIsLoading(true);

    try {
      const response = await registerUser(formData);
      const user = response?.user ?? response;

      setRegisteredUser(user);
      setSuccessMessage("akun berhasil dibuat. silahkan login");

      setFormData({
        displayName: "",
        email: "",
        password: "",
      });
    } catch (error) {
      setErrorMessage(
        error instanceof Error
          ? error.message
          : "Terjadi kesalahan saat login.",
      );
    } finally {
      setIsLoading(false);
    }
  }

  const isFormIncomplete =
    !formData.displayName.trim() ||
    !formData.email.trim() ||
    !formData.password;

  return (
    <main className="min-h-screen items-center bg-slate-950 px-4 py-10 text-slate-900">
      <div className="mx-auto grid min-h-[85vh] max-w-5xl items-center overflow-hidden rounded-3xl bg-white shadow-2xl md:grid-cols-2">
        <section className="flex items-center p-7 sm:p-12">
          <div className="w-full">
            <p className="text-sm font-bold tracking-widest text-orange-600">
              KODASTORE
            </p>

            <h2 className="mt-3 text-3xl font-bold text-slate-900">Sign up</h2>

            <p className="mt-3 text-sm text-slate-500">
              Buat akun baru KodaStore untuk mulai berbelanja.
            </p>

            <form onSubmit={handleSubmit} className="mt-8 space-y-5">
              <FormFields
                id="displayName"
                label="Nama Tampilan"
                type="text"
                value={formData.displayName}
                onChange={handleChange}
                autoComplete="name"
                placeholder="Nama lengkap atau panggilan"
                disabled={isLoading}
              />

              <FormFields
                id="email"
                label="Email"
                type="email"
                value={formData.email}
                onChange={handleChange}
                autoComplete="email"
                placeholder="email@example.com"
                disabled={isLoading}
              />

              <FormFields
                id="password"
                label="Password"
                type="password"
                value={formData.password}
                onChange={handleChange}
                autoComplete="new-password"
                placeholder="Buat password"
                disabled={isLoading}
              />

              <button
                type="submit"
                disabled={isLoading || isFormIncomplete}
                className="w-full rounded-xl bg-orange-600 px-5 py-3 font-semibold text-white transition hover:bg-orange-700 disabled:cursor-not-allowed disabled:opacity-60"
              >
                {isLoading ? "Mendaftarkan..." : "Daftar"}
              </button>
            </form>

            {errorMessage && (
              <div
                role="alert"
                className="mt-5 rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700"
              >
                {errorMessage}
              </div>
            )}

            {successMessage && (
              <div
                role="status"
                className="mt-5 rounded-xl border border-green-200 bg-green-50 p-4 text-sm text-green-800"
              >
                <p className="font-semibold">Registrasi berhasil</p>

                <p className="mt-1">{successMessage}</p>

                {registeredUser && (
                  <p className="mt-2">
                    User: {registeredUser.displayName} ({registeredUser.email})
                  </p>
                )}
              </div>
            )}
          </div>
        </section>
      </div>
    </main>
  );
}

export default RegisterPage;
