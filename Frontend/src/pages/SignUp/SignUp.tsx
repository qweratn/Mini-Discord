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
import Loading from "@/shared/ui/Loading";
import { useSignUp } from "@clerk/react";

export default function SignUp() {
  const { signUp } = useSignUp();
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const formData = new FormData(event.currentTarget);

    try {
      setIsLoading(true);

      const username = String(formData.get("username") ?? "").trim();
      const emailAddress = String(formData.get("email") ?? "").trim();
      const password = String(formData.get("password") ?? "");

      const { error } = await signUp.password({
        username,
        emailAddress,
        password,
      });

      if (error) {
        console.error(error);
        return;
      }

      const { error: verificationError } =
        await signUp.verifications.sendEmailCode();

      if (verificationError) {
        console.error(verificationError);
        return;
      }

      navigate("/verify-otp", {
        viewTransition: true,
        state: { email: emailAddress, flow: "sign-up" },
      });
    } catch (error) {
      console.error(JSON.stringify(error, null, 2));
      return;
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      {isLoading && <Loading />}
      <main className="auth-page relative min-h-dvh overflow-hidden bg-[#0a0e20] text-white">
        <div
          aria-hidden="true"
          className="auth-glow pointer-events-none absolute right-[9%] top-[20%] size-96 rounded-full bg-[#6277ef]/14 blur-3xl"
        />
        <div
          aria-hidden="true"
          className="pointer-events-none absolute -bottom-36 -left-28 size-96 rounded-full bg-[#7c3aed]/8 blur-3xl"
        />

        <div className="relative mx-auto grid min-h-dvh w-full max-w-6xl items-center gap-10 px-4 py-8 sm:px-6 lg:grid-cols-[minmax(420px,0.85fr)_minmax(0,0.95fr)] lg:gap-16 lg:px-8">
          <Card className="auth-enter-left mx-auto w-full max-w-lg border-[#34364d] bg-[#161a2e]/95 text-white shadow-2xl shadow-black/25 backdrop-blur-xl [view-transition-name:auth-card]">
            <CardHeader className="items-center px-6 pt-7 text-center sm:px-10 sm:pt-8">
              <CardTitle className="text-3xl font-bold tracking-tight">
                Создать аккаунт
              </CardTitle>
              <CardDescription className="text-base text-[#9d9faf]">
                Пара минут — и ты внутри
              </CardDescription>
            </CardHeader>

            <CardContent className="px-6 pb-7 pt-6 sm:px-10 sm:pb-8">
              <form className="auth-form space-y-4" onSubmit={handleSubmit}>
                <div className="space-y-2">
                  <Label htmlFor="username" className="text-[#d8d9e2]">
                    Имя пользователя
                  </Label>
                  <Input
                    id="username"
                    name="username"
                    autoComplete="username"
                    placeholder="Как тебя называть?"
                    required
                    className="h-11 border-[#3a3d57] bg-[#0f1326]/70 px-4 text-base text-white placeholder:text-[#74778b] focus-visible:border-[#6277ef] focus-visible:ring-[#6277ef]/25"
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="sign-up-email" className="text-[#d8d9e2]">
                    Электронная почта
                  </Label>
                  <Input
                    id="sign-up-email"
                    name="email"
                    type="email"
                    autoComplete="email"
                    placeholder="you@example.com"
                    required
                    className="h-11 border-[#3a3d57] bg-[#0f1326]/70 px-4 text-base text-white placeholder:text-[#74778b] focus-visible:border-[#6277ef] focus-visible:ring-[#6277ef]/25"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="sign-up-password" className="text-[#d8d9e2]">
                    Пароль
                  </Label>
                  <div className="relative">
                    <Input
                      id="sign-up-password"
                      name="password"
                      type={showPassword ? "text" : "password"}
                      autoComplete="new-password"
                      placeholder="Не менее 8 символов"
                      minLength={8}
                      required
                      className="h-11 border-[#3a3d57] bg-[#0f1326]/70 px-4 pr-11 text-sm text-white placeholder:text-[#74778b] focus-visible:border-[#6277ef] focus-visible:ring-[#6277ef]/25"
                    />
                    <Button
                      type="button"
                      variant="ghost"
                      size="icon-sm"
                      aria-label={
                        showPassword ? "Скрыть пароли" : "Показать пароли"
                      }
                      aria-pressed={showPassword}
                      onClick={() => setShowPassword((value) => !value)}
                      className="absolute right-2 top-1/2 -translate-y-1/2 text-[#9d9faf] hover:bg-white/5 hover:text-white"
                    >
                      {showPassword ? <EyeOffIcon /> : <EyeIcon />}
                    </Button>
                  </div>
                </div>

                <Button
                  type="submit"
                  size="lg"
                  disabled={isLoading}
                  className="h-12 w-full bg-[#6277ef] text-base font-semibold text-white shadow-lg shadow-[#6277ef]/20 transition-transform hover:-translate-y-0.5 hover:bg-[#7185ff] hover:shadow-[#6277ef]/30"
                >
                  {isLoading ? "Создаём аккаунт..." : "Зарегистрироваться"}
                </Button>

                <p className="pt-1 text-center text-sm text-[#9d9faf]">
                  Уже есть аккаунт?{" "}
                  <Link
                    to="/sign-in"
                    viewTransition
                    className="font-medium text-[#7d8eff] hover:text-[#9aa7ff] hover:underline"
                  >
                    Войти
                  </Link>
                </p>
              </form>

              <Link
                to="/"
                viewTransition
                className="mt-5 block text-center text-sm text-[#74778b] transition-colors hover:text-[#b7b9c6]"
              >
                Вернуться на главную
              </Link>
            </CardContent>
          </Card>

          <section className="auth-enter-right hidden min-w-0 lg:flex lg:flex-col lg:items-end lg:text-right">
            <h1 className="text-5xl font-bold leading-none tracking-tight">
              Твоё место начинается здесь.
            </h1>
            <p className="mt-5 max-w-md text-lg leading-7 text-[#9d9faf]">
              Создай аккаунт и присоединяйся к своим людям.
            </p>

            <img
              src="/hero-auth.png"
              alt="Персонаж Mini Discord с ноутбуком"
              width={1254}
              height={1254}
              className="auth-mascot-float mt-1 h-auto w-full max-w-lg -scale-x-100 object-contain [view-transition-name:mascot]"
            />
          </section>
        </div>
      </main>
    </>
  );
}
