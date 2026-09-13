import { useAuth, useClerk, useUser } from "@clerk/react";
import {
  CameraIcon,
  ChevronDownIcon,
  LoaderCircleIcon,
  LogOutIcon,
  MailIcon,
  MessageCircleIcon,
  Trash2Icon,
  UserRoundIcon,
} from "lucide-react";
import { type ChangeEvent, useRef, useState } from "react";
import { Link } from "react-router";

import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { syncCurrentUser } from "@/feature/users/user-api";
import { toastManager } from "@/lib/toast";

const MAX_AVATAR_SIZE = 10 * 1024 * 1024;

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
  const { getToken } = useAuth();
  const { user } = useUser();
  const avatarInputRef = useRef<HTMLInputElement>(null);
  const [isAvatarUpdating, setIsAvatarUpdating] = useState(false);
  const username = user?.username ?? user?.fullName ?? "anton_dev";
  const email = user?.primaryEmailAddress?.emailAddress ?? "anton@example.com";
  const imageUrl = user?.imageUrl;

  async function updateAvatar(file: File | null) {
    if (!user) {
      return;
    }

    setIsAvatarUpdating(true);

    try {
      await user.setProfileImage({ file });
      await user.reload();

      const freshToken = await getToken({ skipCache: true });

      if (!freshToken) {
        throw new Error("Не удалось получить токен для синхронизации профиля.");
      }

      await syncCurrentUser(freshToken);

      toastManager.add({
        type: "success",
        title: file ? "Аватар обновлён" : "Аватар удалён",
        description: file
          ? "Новое изображение уже отображается в профиле."
          : "Вместо фотографии теперь используются инициалы.",
      });
    } catch (error) {
      console.error("Не удалось обновить аватар", error);
      toastManager.add({
        type: "error",
        title: "Не удалось полностью обновить профиль",
        description:
          "Проверьте подключение к серверу и попробуйте повторить операцию.",
      });
    } finally {
      setIsAvatarUpdating(false);
    }
  }

  async function handleAvatarChange(event: ChangeEvent<HTMLInputElement>) {
    const file = event.target.files?.[0];

    event.target.value = "";

    if (!file) {
      return;
    }

    if (!file.type.startsWith("image/")) {
      toastManager.add({
        type: "error",
        title: "Неверный формат файла",
        description: "Выберите изображение в формате PNG, JPEG, WebP или GIF.",
      });
      return;
    }

    if (file.size > MAX_AVATAR_SIZE) {
      toastManager.add({
        type: "error",
        title: "Файл слишком большой",
        description: "Максимальный размер изображения — 10 МБ.",
      });
      return;
    }

    await updateAvatar(file);
  }

  return (
    <main className="h-dvh overflow-hidden bg-[#080c1c] text-white">
      <div className="flex h-full min-h-0 w-full flex-col sm:flex-row">
        <aside className="profile-sidebar flex w-full shrink-0 items-center border-b border-[#282e4a] bg-[#101529] px-3 py-3 sm:h-full sm:w-64 sm:flex-col sm:items-stretch sm:border-r sm:border-b-0 sm:px-3 sm:py-5 lg:w-72 lg:px-5 2xl:w-80 2xl:px-6">
          <Link
            to="/chats"
            viewTransition
            className="app-logo-transition shrink-0 px-2 text-lg font-bold tracking-tight text-white sm:px-3 sm:text-xl lg:text-2xl"
          >
            Mini Discord
          </Link>

          <nav
            aria-label="Основная навигация"
            className="profile-navigation ml-auto flex gap-1 sm:mt-8 sm:ml-0 sm:flex-col sm:gap-2"
          >
            <Link
              to="/chats"
              viewTransition
              aria-label="Чаты"
              className="flex min-h-10 flex-none items-center gap-2 rounded-xl px-3 text-[#b6bacd] transition-colors hover:bg-white/5 hover:text-white sm:min-h-12 sm:gap-3 sm:px-3.5"
            >
              <MessageCircleIcon className="size-5" />
              <span className="hidden font-medium min-[430px]:inline sm:inline">
                Чаты
              </span>
            </Link>
            <Link
              to="/profile"
              aria-current="page"
              aria-label="Мой профиль"
              className="flex min-h-10 flex-none items-center gap-2 rounded-xl bg-[#252c50] px-3 text-[#7d8eff] sm:min-h-12 sm:gap-3 sm:px-3.5"
            >
              <UserRoundIcon className="size-5" />
              <span className="hidden font-medium min-[430px]:inline sm:inline">
                Мой профиль
              </span>
            </Link>
          </nav>

          <Card className="profile-sidebar-account mt-auto hidden border-[#30385d] bg-[#22294b] py-0 text-white shadow-lg shadow-black/15 sm:flex">
            <CardContent className="flex items-center gap-3 p-3">
              <Avatar className="app-profile-avatar size-10 bg-[linear-gradient(145deg,#7587ff,#312489)] ring-1 ring-white/15">
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

        <section className="profile-content-enter profile-main min-h-0 min-w-0 flex-1 overflow-hidden px-4 py-3 sm:px-6 sm:py-5 lg:px-8 lg:py-6 xl:px-12">
          <div className="mx-auto flex h-full min-h-0 w-full max-w-5xl flex-col 2xl:max-w-6xl">
            <header className="profile-page-heading shrink-0">
              <h1 className="text-2xl font-bold tracking-tight sm:text-3xl lg:text-4xl">
                Мой профиль
              </h1>
              <p className="profile-page-subtitle mt-1 text-sm text-[#969bb5] sm:text-base">
                Управляй данными своего аккаунта
              </p>
            </header>

            <Card className="profile-hero-card relative mt-3 shrink-0 overflow-hidden border-[#3a4270] bg-[linear-gradient(120deg,#302681_0%,#18255b_50%,#0f1835_100%)] py-0 text-white shadow-2xl shadow-black/20 sm:mt-4">
              <div
                aria-hidden="true"
                className="pointer-events-none absolute -right-20 -bottom-36 size-80 rounded-full bg-[#707bf5]/8"
              />
              <div
                aria-hidden="true"
                className="pointer-events-none absolute right-28 bottom-16 size-28 rounded-full bg-[#707bf5]/6"
              />

              <CardContent className="relative grid grid-cols-[auto_minmax(0,1fr)] items-center gap-4 px-4 py-4 sm:gap-6 sm:px-6 sm:py-5 lg:gap-8 lg:px-8">
                <Avatar className="profile-hero-avatar size-20 bg-[linear-gradient(145deg,#7587ff,#312489)] ring-1 ring-white/15 sm:size-28 lg:size-32 2xl:size-36">
                    <AvatarImage src={imageUrl} alt={`Аватар ${username}`} />
                    <AvatarFallback className="bg-transparent text-2xl font-semibold text-white sm:text-3xl">
                      {username.slice(0, 1).toUpperCase()}
                    </AvatarFallback>
                </Avatar>

                <div className="min-w-0">
                  <h2 className="truncate text-xl font-bold sm:text-2xl lg:text-3xl">
                    {username}
                  </h2>
                  <p className="profile-member-since mt-1 truncate text-xs text-[#b0b4ca] sm:text-sm">
                    {formatMemberSince(user?.createdAt)}
                  </p>

                  <input
                    ref={avatarInputRef}
                    type="file"
                    accept="image/png,image/jpeg,image/webp,image/gif"
                    aria-label="Выбрать новый аватар"
                    className="sr-only"
                    disabled={isAvatarUpdating}
                    onChange={(event) => void handleAvatarChange(event)}
                  />

                  <div className="profile-avatar-actions mt-3 flex flex-wrap gap-2">
                    <Button
                      type="button"
                      size="sm"
                      disabled={!user || isAvatarUpdating}
                      onClick={() => avatarInputRef.current?.click()}
                      className="bg-[#5f6ff1] text-white hover:bg-[#7180f8]"
                      aria-label="Выбрать новое фото"
                    >
                      {isAvatarUpdating ? (
                        <LoaderCircleIcon className="animate-spin" />
                      ) : (
                        <CameraIcon />
                      )}
                      <span className="min-[430px]:hidden">Фото</span>
                      <span className="hidden min-[430px]:inline">
                        Выбрать фото
                      </span>
                    </Button>
                    <Button
                      type="button"
                      size="sm"
                      variant="outline"
                      disabled={!user || !user.hasImage || isAvatarUpdating}
                      onClick={() => void updateAvatar(null)}
                      className="border-white/20 bg-white/5 text-white hover:bg-white/10 hover:text-white"
                      aria-label="Удалить фотографию"
                    >
                      <Trash2Icon />
                      <span className="hidden min-[430px]:inline">Удалить</span>
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>

            <Card className="profile-details-card mt-3 shrink-0 gap-0 overflow-hidden border-[#2b3355] bg-[#11172c]/95 py-0 text-white shadow-xl shadow-black/10 sm:mt-4">
              <CardHeader className="profile-details-header shrink-0 px-4 pt-4 sm:px-6 sm:pt-5">
                <CardTitle className="text-lg font-bold sm:text-xl">
                  Личные данные
                </CardTitle>
              </CardHeader>

              <CardContent className="px-4 pb-0 sm:px-6">
                <dl className="pt-1 sm:pt-2">
                  <div className="profile-details-row flex items-center gap-3 py-2 sm:gap-4 sm:py-3">
                    <span className="profile-details-icon flex size-10 shrink-0 items-center justify-center rounded-full bg-[#1c2349] text-[#6577f7] sm:size-11">
                      <UserRoundIcon className="size-5" />
                    </span>
                    <div className="min-w-0">
                      <dt className="text-xs text-[#969bb5] sm:text-sm">
                        Имя пользователя
                      </dt>
                      <dd className="mt-0.5 truncate text-sm text-white sm:text-base">
                        {username}
                      </dd>
                    </div>
                  </div>

                  <Separator className="bg-[#252c49]" />

                  <div className="profile-details-row flex items-center gap-3 py-2 sm:gap-4 sm:py-3">
                    <span className="profile-details-icon flex size-10 shrink-0 items-center justify-center rounded-full bg-[#1c2349] text-[#6577f7] sm:size-11">
                      <MailIcon className="size-5" />
                    </span>
                    <div className="min-w-0">
                      <dt className="text-xs text-[#969bb5] sm:text-sm">
                        Электронная почта
                      </dt>
                      <dd className="mt-0.5 truncate text-sm text-white sm:text-base">
                        {email}
                      </dd>
                    </div>
                  </div>
                </dl>
              </CardContent>

              <Separator className="bg-[#252c49]" />

              <CardContent className="profile-details-footer flex shrink-0 justify-end px-4 py-3 sm:px-6 sm:py-4">
                <Button
                  type="button"
                  variant="destructive"
                  size="lg"
                  onClick={() => void signOut({ redirectUrl: "/" })}
                  className="h-10 w-full px-5 sm:w-auto"
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
