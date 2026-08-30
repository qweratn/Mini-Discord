import { useClerk, useUser } from "@clerk/react";
import {
  ChevronDownIcon,
  LogOutIcon,
  MailIcon,
  MessageCircleIcon,
  UserRoundIcon,
} from "lucide-react";
import { Link } from "react-router";

import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";

function formatMemberSince(createdAt: Date | null | undefined) {
  if (!createdAt) {
    return "Участник Mini Discord";
  }

  return `В Mini Discord с ${new Intl.DateTimeFormat("ru-RU", {
    day: "numeric",
    month: "long",
    year: "numeric",
  }).format(createdAt)}`;
}

export default function Profile() {
  const { signOut } = useClerk();
  const { user } = useUser();
  const username = user?.username ?? user?.fullName ?? "anton_dev";
  const email = user?.primaryEmailAddress?.emailAddress ?? "anton@example.com";
  const imageUrl = user?.imageUrl;

  return (
    <main className="h-dvh touch-pan-y overflow-x-hidden overflow-y-auto overscroll-y-contain bg-[#080c1c] text-white">
      <div className="mx-auto flex min-h-full w-full max-w-[1600px] flex-col sm:flex-row">
        <aside className="flex w-full shrink-0 flex-col border-b border-[#282e4a] bg-[#101529] px-4 py-5 sm:w-64 sm:border-r sm:border-b-0 sm:px-3 sm:py-6 lg:w-72 lg:px-5">
          <Link
            to="/chats"
            viewTransition
            className="px-3 text-xl font-bold tracking-tight text-white lg:text-2xl"
          >
            Mini Discord
          </Link>

          <nav
            aria-label="Основная навигация"
            className="mt-5 flex gap-2 sm:mt-10 sm:flex-col"
          >
            <Link
              to="/chats"
              viewTransition
              className="flex min-h-12 flex-1 items-center gap-3 rounded-xl px-3.5 text-[#b6bacd] transition-colors hover:bg-white/5 hover:text-white sm:flex-none"
            >
              <MessageCircleIcon className="size-5" />
              <span className="font-medium">Чаты</span>
            </Link>
            <Link
              to="/profile"
              aria-current="page"
              className="flex min-h-12 flex-1 items-center gap-3 rounded-xl bg-[#252c50] px-3.5 text-[#7d8eff] sm:flex-none"
            >
              <UserRoundIcon className="size-5" />
              <span className="font-medium">Мой профиль</span>
            </Link>
          </nav>

          <Card className="mt-auto hidden border-[#30385d] bg-[#22294b] py-0 text-white shadow-lg shadow-black/15 sm:flex">
            <CardContent className="flex items-center gap-3 p-3">
              <Avatar className="size-10 bg-[linear-gradient(145deg,#7587ff,#312489)] ring-1 ring-white/15">
                <AvatarImage src={imageUrl} alt={`Аватар ${username}`} />
                <AvatarFallback className="bg-transparent font-semibold text-white">
                  {username.slice(0, 1).toUpperCase()}
                </AvatarFallback>
              </Avatar>
              <span className="min-w-0 flex-1 truncate text-sm font-semibold">
                {username}
              </span>
              <ChevronDownIcon className="size-4 shrink-0 text-[#a6aac0]" />
            </CardContent>
          </Card>
        </aside>

        <section className="min-w-0 flex-1 px-4 py-8 sm:px-7 lg:px-10 lg:py-12 xl:px-14">
          <div className="mx-auto w-full max-w-5xl">
            <header>
              <h1 className="text-3xl font-bold tracking-tight sm:text-4xl lg:text-5xl">
                Мой профиль
              </h1>
              <p className="mt-2 text-base text-[#969bb5] sm:text-lg">
                Управляй данными своего аккаунта
              </p>
            </header>

            <Card className="relative mt-8 overflow-hidden border-[#3a4270] bg-[linear-gradient(120deg,#302681_0%,#18255b_50%,#0f1835_100%)] py-0 text-white shadow-2xl shadow-black/20 lg:min-h-72">
              <div
                aria-hidden="true"
                className="pointer-events-none absolute -right-20 -bottom-36 size-80 rounded-full bg-[#707bf5]/8"
              />
              <div
                aria-hidden="true"
                className="pointer-events-none absolute right-28 bottom-16 size-28 rounded-full bg-[#707bf5]/6"
              />

              <CardContent className="relative flex h-full flex-col items-start gap-6 px-5 py-7 sm:flex-row sm:items-center sm:px-8 sm:py-8 lg:gap-9 lg:px-10">
                <Avatar className="size-32 bg-[linear-gradient(145deg,#7587ff,#312489)] ring-1 ring-white/15 sm:size-40 lg:size-44">
                  <AvatarImage src={imageUrl} alt={`Аватар ${username}`} />
                  <AvatarFallback className="bg-transparent text-4xl font-semibold text-white">
                    {username.slice(0, 1).toUpperCase()}
                  </AvatarFallback>
                </Avatar>

                <div className="min-w-0 flex-1">
                  <h2 className="truncate text-2xl font-bold sm:text-3xl">
                    {username}
                  </h2>
                  <p className="mt-2 text-sm text-[#b0b4ca] sm:text-base">
                    {formatMemberSince(user?.createdAt)}
                  </p>
                </div>

                {/* Кнопка будет возвращена, когда появится редактирование профиля.
                <Button
                  type="button"
                  size="lg"
                  className="h-12 w-full gap-2 bg-[#5f6ff1] px-5 text-base font-semibold text-white shadow-lg shadow-[#4a55db]/20 hover:bg-[#6e7df7] sm:w-auto lg:self-end"
                >
                  <PencilIcon className="size-5" />
                  Редактировать профиль
                </Button>
                */}
              </CardContent>
            </Card>

            <Card className="mt-7 gap-0 overflow-hidden border-[#2b3355] bg-[#11172c]/95 py-0 text-white shadow-xl shadow-black/10">
              <CardHeader className="px-5 pt-6 sm:px-8 sm:pt-8">
                <CardTitle className="text-xl font-bold sm:text-2xl">
                  Личные данные
                </CardTitle>
              </CardHeader>

              <CardContent className="px-5 pb-0 sm:px-8">
                <dl className="mt-5">
                  <div className="flex items-center gap-4 py-4 sm:gap-5">
                    <span className="flex size-12 shrink-0 items-center justify-center rounded-full bg-[#1c2349] text-[#6577f7]">
                      <UserRoundIcon className="size-6" />
                    </span>
                    <div className="min-w-0">
                      <dt className="text-sm text-[#969bb5]">
                        Имя пользователя
                      </dt>
                      <dd className="mt-1 truncate text-base text-white sm:text-lg">
                        {username}
                      </dd>
                    </div>
                  </div>

                  <Separator className="bg-[#252c49]" />

                  <div className="flex items-center gap-4 py-4 sm:gap-5">
                    <span className="flex size-12 shrink-0 items-center justify-center rounded-full bg-[#1c2349] text-[#6577f7]">
                      <MailIcon className="size-6" />
                    </span>
                    <div className="min-w-0">
                      <dt className="text-sm text-[#969bb5]">
                        Электронная почта
                      </dt>
                      <dd className="mt-1 truncate text-base text-white sm:text-lg">
                        {email}
                      </dd>
                    </div>
                  </div>
                </dl>
              </CardContent>

              <Separator className="bg-[#252c49]" />

              <CardContent className="flex justify-end px-5 py-5 sm:px-8">
                <Button
                  type="button"
                  variant="destructive"
                  size="lg"
                  onClick={() => void signOut({ redirectUrl: "/" })}
                  className="h-11 w-full px-5 sm:w-auto"
                >
                  <LogOutIcon className="size-5" />
                  Выйти из профиля
                </Button>
              </CardContent>
            </Card>
          </div>
        </section>
      </div>
    </main>
  );
}
