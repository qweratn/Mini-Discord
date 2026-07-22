import { ArrowLeftIcon } from "lucide-react";
import {
  useEffect,
  useRef,
  useState,
  type ChangeEvent,
  type ClipboardEvent,
  type KeyboardEvent,
} from "react";
import { Link, useLocation, useNavigate } from "react-router";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { toastManager } from "@/lib/toast";
import Loading from "@/shared/ui/Loading";
import { useSignIn, useSignUp } from "@clerk/react";

const CODE_LENGTH = 6;
const RESEND_DELAY = 30;

type LocationState = {
  email?: string;
  flow?: "sign-in" | "sign-up";
};

function maskEmail(email: string) {
  const [name, domain] = email.split("@");

  if (!name || !domain) return "указанную электронную почту";

  const visiblePart = name.slice(0, Math.min(2, name.length));
  return `${visiblePart}${"•".repeat(Math.max(3, name.length - visiblePart.length))}@${domain}`;
}

export default function VerifyOtp() {
  const location = useLocation();
  const locationState = location.state as LocationState | null;
  const [code, setCode] = useState(() => Array(CODE_LENGTH).fill(""));
  const [secondsLeft, setSecondsLeft] = useState(RESEND_DELAY);
  const [isLoading, setIsLoading] = useState(false);
  const inputsRef = useRef<Array<HTMLInputElement | null>>([]);
  const { signUp } = useSignUp();
  const { signIn } = useSignIn();
  const navigate = useNavigate();
  const flow =
    locationState?.flow ??
    (signIn.status === "needs_client_trust" ||
    signIn.status === "needs_second_factor"
      ? "sign-in"
      : "sign-up");
  const isSignIn = flow === "sign-in";
  const email =
    locationState?.email ??
    (isSignIn ? signIn.identifier : signUp.emailAddress) ??
    "";
  const returnPath = isSignIn ? "/sign-in" : "/sign-up";

  useEffect(() => {
    if (secondsLeft <= 0) return;

    const timer = window.setInterval(() => {
      setSecondsLeft((value) => Math.max(0, value - 1));
    }, 1000);

    return () => window.clearInterval(timer);
  }, [secondsLeft]);

  const isComplete = code.every(Boolean);

  function updateCode(index: number, value: string) {
    const digit = value.replace(/\D/g, "").slice(-1);

    setCode((current) => {
      const next = [...current];
      next[index] = digit;
      return next;
    });

    if (digit && index < CODE_LENGTH - 1) {
      inputsRef.current[index + 1]?.focus();
    }
  }

  function handleKeyDown(
    index: number,
    event: KeyboardEvent<HTMLInputElement>,
  ) {
    if (event.key === "Backspace" && !code[index] && index > 0) {
      inputsRef.current[index - 1]?.focus();
    }

    if (event.key === "ArrowLeft" && index > 0) {
      event.preventDefault();
      inputsRef.current[index - 1]?.focus();
    }

    if (event.key === "ArrowRight" && index < CODE_LENGTH - 1) {
      event.preventDefault();
      inputsRef.current[index + 1]?.focus();
    }
  }

  function handlePaste(event: ClipboardEvent<HTMLInputElement>) {
    const pastedCode = event.clipboardData
      .getData("text")
      .replace(/\D/g, "")
      .slice(0, CODE_LENGTH);

    if (!pastedCode) return;

    event.preventDefault();
    const nextCode = Array(CODE_LENGTH).fill("");
    pastedCode.split("").forEach((digit, index) => {
      nextCode[index] = digit;
    });
    setCode(nextCode);
    inputsRef.current[Math.min(pastedCode.length, CODE_LENGTH) - 1]?.focus();
  }

  async function finalizeAuthentication() {
    const auth = isSignIn ? signIn : signUp;

    if (auth.status !== "complete") {
      console.error("Верификация не завершила авторизацию", {
        flow,
        status: auth.status,
      });
      return;
    }

    const { error } = await auth.finalize({
      navigate: () => {
        toastManager.add({
          type: "success",
          title: isSignIn ? "Вход выполнен" : "Регистрация завершена",
          description: isSignIn
            ? "С возвращением в Mini Discord!"
            : "Добро пожаловать в Mini Discord!",
        });
        navigate("/chats", {
          replace: true,
          viewTransition: true,
        });
      },
    });

    if (error) {
      console.error(error);
    }
  }

  async function verifyOtp(otpCode: string) {
    const { error } = isSignIn
      ? await signIn.mfa.verifyEmailCode({ code: otpCode })
      : await signUp.verifications.verifyEmailCode({ code: otpCode });

    if (error) {
      console.error(error);
      return;
    }

    await finalizeAuthentication();
  }

  const handleVerify = async () => {
    try {
      setIsLoading(true);

      const otpCode = code.join("");

      if (otpCode.length !== CODE_LENGTH) {
        return;
      }

      await verifyOtp(otpCode);
    } catch (error) {
      console.error(error);
      return;
    } finally {
      setIsLoading(false);
    }
  };

  async function handleResend() {
    try {
      setIsLoading(true);

      const { error } = isSignIn
        ? await signIn.mfa.sendEmailCode()
        : await signUp.verifications.sendEmailCode();

      if (error) {
        console.error(error);
        return;
      }

      setCode(Array(CODE_LENGTH).fill(""));
      setSecondsLeft(RESEND_DELAY);
      inputsRef.current[0]?.focus();
    } catch (error) {
      console.error(error);
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <>
      {isLoading && <Loading />}
      <main className="auth-page relative min-h-dvh overflow-hidden bg-[#0a0e20] text-white">
        <div
          aria-hidden="true"
          className="auth-glow pointer-events-none absolute left-[8%] top-[20%] size-96 rounded-full bg-[#6277ef]/14 blur-3xl"
        />
        <div
          aria-hidden="true"
          className="pointer-events-none absolute -bottom-36 -right-28 size-96 rounded-full bg-[#7c3aed]/8 blur-3xl"
        />

        <div className="relative mx-auto grid min-h-dvh w-full max-w-6xl items-center gap-10 px-4 py-8 sm:px-6 lg:grid-cols-[minmax(0,0.95fr)_minmax(420px,0.85fr)] lg:gap-16 lg:px-8">
          <section className="auth-enter-left hidden min-w-0 lg:flex lg:flex-col lg:items-start">
            <h1 className="max-w-lg text-5xl font-bold leading-[1.05] tracking-tight">
              Остался последний шаг.
            </h1>
            <p className="mt-5 max-w-md text-lg leading-7 text-[#9d9faf]">
              {isSignIn
                ? "Подтверди вход — и можно продолжать общение."
                : "Подтверди почту — и можно начинать общение."}
            </p>

            <img
              src="/hero-auth.png"
              alt="Персонаж Mini Discord с ноутбуком"
              width={1254}
              height={1254}
              className="auth-mascot-float mt-1 h-auto w-full max-w-lg object-contain [view-transition-name:mascot]"
            />
          </section>

          <Card className="auth-enter-right mx-auto w-full max-w-lg border-[#34364d] bg-[#161a2e]/95 text-white shadow-2xl shadow-black/25 backdrop-blur-xl [view-transition-name:auth-card]">
            <CardHeader className="items-center px-6 pt-8 text-center sm:px-10 sm:pt-10">
              <CardTitle className="text-3xl font-bold tracking-tight">
                Введи код
              </CardTitle>
              <CardDescription className="max-w-sm text-base leading-6 text-[#9d9faf]">
                Мы отправили шестизначный код на{" "}
                <span className="font-medium text-[#d8d9e2]">
                  {maskEmail(email)}
                </span>
              </CardDescription>
            </CardHeader>

            <CardContent className="px-6 pb-8 pt-7 sm:px-10 sm:pb-10">
              <form
                className="auth-form space-y-6"
                onSubmit={(event) => {
                  event.preventDefault();
                  void handleVerify();
                }}
              >
                <fieldset>
                  <legend className="sr-only">Код подтверждения</legend>
                  <div className="grid grid-cols-6 gap-2 sm:gap-3">
                    {code.map((digit, index) => (
                      <Input
                        key={index}
                        ref={(element) => {
                          inputsRef.current[index] = element;
                        }}
                        value={digit}
                        onChange={(event: ChangeEvent<HTMLInputElement>) =>
                          updateCode(index, event.target.value)
                        }
                        onKeyDown={(event) => handleKeyDown(index, event)}
                        onPaste={handlePaste}
                        onFocus={(event) => event.currentTarget.select()}
                        type="text"
                        inputMode="numeric"
                        autoComplete={index === 0 ? "one-time-code" : "off"}
                        maxLength={1}
                        aria-label={`Цифра ${index + 1} из ${CODE_LENGTH}`}
                        autoFocus={index === 0}
                        className="h-13 rounded-xl border-[#3a3d57] bg-[#0f1326]/70 p-0 text-center text-xl font-semibold text-white caret-[#8796ff] focus-visible:border-[#6277ef] focus-visible:ring-[#6277ef]/25 sm:h-14 sm:text-2xl"
                      />
                    ))}
                  </div>
                </fieldset>

                <Button
                  type="submit"
                  size="lg"
                  disabled={!isComplete}
                  className="h-12 w-full bg-[#6277ef] text-base font-semibold text-white shadow-lg shadow-[#6277ef]/20 transition-transform enabled:hover:-translate-y-0.5 enabled:hover:bg-[#7185ff] disabled:bg-[#3a416d] disabled:text-[#9297b8]"
                >
                  Подтвердить
                </Button>

                <div className="text-center text-sm text-[#9d9faf]">
                  {secondsLeft > 0 ? (
                    <p>
                      Отправить код повторно через{" "}
                      <span className="font-medium tabular-nums text-[#d8d9e2]">
                        0:{secondsLeft.toString().padStart(2, "0")}
                      </span>
                    </p>
                  ) : (
                    <button
                      type="button"
                      onClick={handleResend}
                      disabled={isLoading}
                      className="font-medium text-[#7d8eff] transition-colors hover:text-[#9aa7ff] hover:underline focus-visible:rounded-sm focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-[#6277ef]"
                    >
                      Отправить код повторно
                    </button>
                  )}
                </div>
              </form>

              <Link
                to={returnPath}
                viewTransition
                className="mt-7 flex items-center justify-center gap-2 text-sm text-[#74778b] transition-colors hover:text-[#b7b9c6]"
              >
                <ArrowLeftIcon className="size-4" aria-hidden="true" />
                {isSignIn
                  ? "Вернуться ко входу"
                  : "Изменить электронную почту"}
              </Link>
            </CardContent>
          </Card>
        </div>
      </main>
    </>
  );
}
