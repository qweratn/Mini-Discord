import { EyeIcon, EyeOffIcon } from "lucide-react";
import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { getAuthErrorMessage } from "@/lib/auth-errors";
import { toastManager } from "@/lib/toast";
import Loading from "@/shared/ui/Loading";
import { useSignIn } from "@clerk/react";

export default function SignIn() {
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const { signIn, errors } = useSignIn();
  const navigate = useNavigate();
  const emailError = errors.fields.identifier;
  const passwordError = errors.fields.password;
  const globalError = errors.global?.[0];

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    try {
      setIsLoading(true);
      setFormError(null);
      const formData = new FormData(event.currentTarget);
      const emailAddress = String(formData.get("email") ?? "").trim();
      const password = String(formData.get("password") ?? "");

      const { error } = await signIn.password({
        emailAddress,
        password,
      });

      if (error) {
        console.error(error);
        return;
      }

      if (signIn.status === "complete") {
        const { error: finalizeError } = await signIn.finalize({
          navigate: () => {
            toastManager.add({
              type: "success",
              title: "Вход выполнен",
              description: "С возвращением в Mini Discord!",
            });
            navigate("/chats", {
              replace: true,
              viewTransition: true,
            });
          },
        });

        if (finalizeError) {
          console.error(finalizeError);
          setFormError(getAuthErrorMessage(finalizeError));
        }

        return;
      }

      if (
        signIn.status === "needs_client_trust" ||
        signIn.status === "needs_second_factor"
      ) {
        const { error: verificationError } =
          await signIn.mfa.sendEmailCode();

        if (verificationError) {
          console.error(verificationError);
          setFormError(
            getAuthErrorMessage(
              verificationError,
              "Не удалось отправить код подтверждения",
            ),
          );
          return;
        }

        navigate("/verify-otp", {
          viewTransition: true,
          state: { email: emailAddress, flow: "sign-in" },
        });

        return;
      }

      console.error("Вход не завершён", { status: signIn.status });
      setFormError("Не удалось завершить вход. Попробуйте ещё раз");
    } catch (error) {
      console.error(JSON.stringify(error, null, 2));
      setFormError(getAuthErrorMessage(error));
      return;
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      {isLoading && <Loading />}
      <main className="relative min-h-dvh overflow-hidden bg-[#0a0e20] text-white">
        <div
          aria-hidden="true"
          className="pointer-events-none absolute left-[10%] top-[24%] size-80 rounded-full bg-[#6277ef]/12 blur-3xl"
        />

        <div className="relative mx-auto grid min-h-dvh w-full max-w-6xl items-center gap-10 px-4 py-8 sm:px-6 lg:grid-cols-[minmax(0,0.95fr)_minmax(420px,0.85fr)] lg:gap-16 lg:px-8">
          <section className="auth-enter-left hidden min-w-0 lg:flex lg:flex-col lg:items-start">
            <h1 className="text-5xl font-bold leading-none tracking-tight">
              С возвращением!
            </h1>
            <p className="mt-5 max-w-sm text-lg leading-7 text-[#9d9faf]">
              Твои друзья и разговоры уже ждут тебя.
            </p>

            <img
              src="hero-auth.png"
              alt="Персонаж Mini Discord с ноутбуком"
              width={1254}
              height={1254}
              className="auth-mascot-float mt-1 h-auto w-full max-w-lg object-contain [view-transition-name:mascot]"
            />
          </section>

          <Card className="auth-enter-right mx-auto w-full max-w-lg border-[#34364d] bg-[#161a2e]/95 text-white shadow-2xl shadow-black/25 backdrop-blur-xl [view-transition-name:auth-card]">
            <CardHeader className="items-center px-6 pt-8 text-center sm:px-10 sm:pt-10">
              <CardTitle className="text-3xl font-bold tracking-tight">
                Войти в аккаунт
              </CardTitle>
              <CardDescription className="text-base text-[#9d9faf]">
                Рады видеть тебя снова
              </CardDescription>
            </CardHeader>

            <CardContent className="px-6 pb-8 pt-8 sm:px-10 sm:pb-10">
              <form className="space-y-5" onSubmit={handleSubmit}>
                <div className="space-y-2">
                  <Label htmlFor="email" className="text-[#d8d9e2]">
                    Электронная почта
                  </Label>
                  <Input
                    id="email"
                    name="email"
                    type="email"
                    autoComplete="email"
                    placeholder="you@example.com"
                    required
                    aria-invalid={Boolean(emailError)}
                    aria-describedby={emailError ? "sign-in-email-error" : undefined}
                    className="h-12 border-[#3a3d57] bg-[#0f1326]/70 px-4 text-base text-white placeholder:text-[#74778b] focus-visible:border-[#6277ef] focus-visible:ring-[#6277ef]/25"
                  />
                  {emailError && (
                    <p
                      id="sign-in-email-error"
                      role="alert"
                      className="text-sm text-red-400"
                    >
                      {getAuthErrorMessage(emailError)}
                    </p>
                  )}
                </div>

                <div className="space-y-2">
                  <Label htmlFor="password" className="text-[#d8d9e2]">
                    Пароль
                  </Label>
                  <div className="relative">
                    <Input
                      id="password"
                      name="password"
                      type={showPassword ? "text" : "password"}
                      autoComplete="current-password"
                      placeholder="Введите пароль"
                      required
                      aria-invalid={Boolean(passwordError)}
                      aria-describedby={
                        passwordError ? "sign-in-password-error" : undefined
                      }
                      className="h-12 border-[#3a3d57] bg-[#0f1326]/70 px-4 pr-12 text-base text-white placeholder:text-[#74778b] focus-visible:border-[#6277ef] focus-visible:ring-[#6277ef]/25"
                    />
                    <Button
                      type="button"
                      variant="ghost"
                      size="icon"
                      aria-label={
                        showPassword ? "Скрыть пароль" : "Показать пароль"
                      }
                      aria-pressed={showPassword}
                      onClick={() => setShowPassword((value) => !value)}
                      className="absolute right-2 top-1/2 -translate-y-1/2 text-[#9d9faf] hover:bg-white/5 hover:text-white"
                    >
                      {showPassword ? <EyeOffIcon /> : <EyeIcon />}
                    </Button>
                  </div>
                  {passwordError && (
                    <p
                      id="sign-in-password-error"
                      role="alert"
                      className="text-sm text-red-400"
                    >
                      {getAuthErrorMessage(passwordError)}
                    </p>
                  )}
                </div>

                <div className="flex flex-wrap items-center justify-between gap-3">
                  <a
                    href="#"
                    className="text-sm font-medium text-[#7d8eff] transition-colors hover:text-[#9aa7ff] hover:underline"
                  >
                    Забыли пароль?
                  </a>
                </div>

                {(formError || globalError) && (
                  <p
                    role="alert"
                    className="rounded-lg border border-red-400/25 bg-red-400/10 px-3 py-2 text-sm text-red-300"
                  >
                    {formError ?? getAuthErrorMessage(globalError)}
                  </p>
                )}

                <Button
                  type="submit"
                  size="lg"
                  disabled={isLoading}
                  className="h-12 w-full bg-[#6277ef] text-base font-semibold text-white shadow-lg shadow-[#6277ef]/15 hover:bg-[#7185ff]"
                >
                  {isLoading ? "Входим..." : "Войти"}
                </Button>

                <p className="pt-1 text-center text-sm text-[#9d9faf]">
                  Нет аккаунта?{" "}
                  <Link
                    to="/sign-up"
                    viewTransition
                    className="font-medium text-[#7d8eff] hover:text-[#9aa7ff] hover:underline"
                  >
                    Зарегистрироваться
                  </Link>
                </p>
              </form>

              <Link
                to="/"
                viewTransition
                className="mt-6 block text-center text-sm text-[#74778b] transition-colors hover:text-[#b7b9c6]"
              >
                Вернуться на главную
              </Link>
            </CardContent>
          </Card>
        </div>
      </main>
    </>
  );
}
