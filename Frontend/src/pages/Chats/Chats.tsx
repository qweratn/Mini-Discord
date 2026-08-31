import { useUser } from "@clerk/react";
import {
  ArrowLeftIcon,
  ChevronRightIcon,
  MoreHorizontalIcon,
  SearchIcon,
  SendIcon,
  SquarePenIcon,
  UserPlusIcon,
  UsersRoundIcon,
} from "lucide-react";
import { useEffect, useState } from "react";
import { Link } from "react-router";

import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Bubble, BubbleContent } from "@/components/ui/bubble";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Separator } from "@/components/ui/separator";
import { Spinner } from "@/components/ui/spinner";
import { getUsersChats } from "@/feature/chats/chats-api";
import type { Chat } from "@/feature/chats/chat-types";
import { cn } from "@/lib/utils";
import { toastManager } from "@/lib/toast";

const messages = [
  {
    author: "Алексей",
    initials: "А",
    time: "10:24",
    content: "Привет! Как продвигается страница чатов?",
    color: "bg-[#435bb3]",
  },
  {
    author: "Мария",
    initials: "М",
    time: "10:26",
    content: "Макет уже почти готов ✨",
    color: "bg-[#397a72]",
  },
  {
    author: "anton_dev",
    initials: "AD",
    time: "10:28",
    content: "Отлично, сегодня подключу API.",
    color: "bg-[#4f46a8]",
  },
  {
    author: "Алексей",
    initials: "А",
    time: "10:29",
    content: "Супер, жду ревью!",
    color: "bg-[#435bb3]",
  },
];

const members = [
  { name: "anton_dev", initials: "AD", color: "bg-[#4f46a8]" },
  { name: "Алексей", initials: "А", color: "bg-[#435bb3]" },
  { name: "Мария", initials: "М", color: "bg-[#397a72]" },
  { name: "Иван", initials: "И", color: "bg-[#575b72]" },
  { name: "Ольга", initials: "О", color: "bg-[#665071]" },
  { name: "Дмитрий", initials: "Д", color: "bg-[#4e5d78]" },
  { name: "София", initials: "С", color: "bg-[#6c5369]" },
  { name: "Максим", initials: "М", color: "bg-[#4e596f]" },
];

function UserAvatar({
  name,
  initials,
  imageUrl,
  className,
}: {
  name: string;
  initials: string;
  imageUrl?: string | null;
  className?: string;
}) {
  return (
    <Avatar
      className={cn("size-11 ring-1 ring-white/10 bg-[#4f46a8]", className)}
    >
      {imageUrl && <AvatarImage src={imageUrl} alt={`Аватар ${name}`} />}
      <AvatarFallback className={cn("font-semibold text-white bg-[#4f46a8]")}>
        {initials}
      </AvatarFallback>
    </Avatar>
  );
}

export default function Chats() {
  const { user } = useUser();
  const [activeChatId, setActiveChatId] = useState<string | null>(null);
  const [isMobileChatOpen, setIsMobileChatOpen] = useState(false);
  const username = user?.username ?? user?.fullName ?? "anton_dev";
  const userInitials = username.slice(0, 2).toUpperCase();
  const [chats, setChats] = useState<Chat[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [reloadKey, setReloadKey] = useState(0);
  const activeChat =
    chats.find((chat) => chat.chatId === activeChatId) ?? chats[0];

  useEffect(() => {
    async function loadChats() {
      try {
        setLoading(true);
        setLoadError(null);

        const loadedChats = await getUsersChats();

        setChats(loadedChats);
        setActiveChatId((currentChatId) =>
          loadedChats.some((chat) => chat.chatId === currentChatId)
            ? currentChatId
            : (loadedChats[0]?.chatId ?? null),
        );
      } catch (error: unknown) {
        const message =
          error instanceof Error
            ? error.message
            : "Неизвестная ошибка при загрузке чатов";

        console.error(error);
        setLoadError(message);
        toastManager.add({
          type: "error",
          title: "Ошибка при загрузке чатов",
          description: message,
        });
      } finally {
          setLoading(false);
      }
    }

    void loadChats();

  }, [reloadKey]);

    function formatMessageTime(value: string | null): string {
        if (!value) {
            return "";
        }

        return new Intl.DateTimeFormat("ru-RU", {
            hour: "2-digit",
            minute: "2-digit",
        }).format(new Date(value));
    }

    if (loading) {
    return (
      <main className="flex h-dvh items-center justify-center bg-[#080c1c] text-white">
        <div className="flex flex-col items-center gap-3 text-[#aab0c3]">
          <Spinner className="size-8" />
          <p>Загружаем чаты...</p>
        </div>
      </main>
    );
  }

  if (loadError) {
    return (
      <main className="flex h-dvh items-center justify-center bg-[#080c1c] px-4 text-white">
        <div className="flex max-w-md flex-col items-center text-center">
          <h1 className="text-xl font-bold">Не удалось загрузить чаты</h1>
          <p className="mt-2 text-sm text-[#969bb5]">{loadError}</p>
          <Button
            type="button"
            className="mt-5 bg-[#5f6ff1] text-white hover:bg-[#7180f8]"
            onClick={() => setReloadKey((current) => current + 1)}
          >
            Попробовать снова
          </Button>
        </div>
      </main>
    );
  }

  if (!activeChat) {
    return (
      <main className="flex h-dvh items-center justify-center bg-[#080c1c] px-4 text-white">
        <div className="text-center">
          <h1 className="text-xl font-bold">У вас пока нет чатов</h1>
          <p className="mt-2 text-sm text-[#969bb5]">
            Создайте новый чат, чтобы начать общение.
          </p>
        </div>
      </main>
    );
  }

  return (
    <main className="h-dvh overflow-hidden bg-[#080c1c] text-white">
      <div className="mx-auto flex h-full min-h-0 w-full max-w-[1700px]">
        <aside
          className={cn(
            "h-full w-full shrink-0 flex-col bg-[#0e1428] lg:flex lg:w-80 lg:border-r lg:border-[#29304e]",
            isMobileChatOpen ? "hidden" : "flex",
          )}
        >
          <div className="flex items-center justify-between px-5 pt-6 pb-5">
            <div>
              <p className="text-xs font-medium tracking-[0.18em] text-[#707791] uppercase">
                Mini Discord
              </p>
              <h1 className="mt-1 text-2xl font-bold tracking-tight">
                Сообщения
              </h1>
            </div>
            <Button
              type="button"
              size="icon-lg"
              aria-label="Создать чат"
              className="size-11 bg-[#5f6ff1] text-white shadow-lg shadow-[#5362db]/20 hover:bg-[#7180f8]"
            >
              <SquarePenIcon className="size-5" />
            </Button>
          </div>

          <div className="px-5">
            <div className="relative">
              <SearchIcon className="pointer-events-none absolute top-1/2 left-3.5 size-4 -translate-y-1/2 text-[#7d839a]" />
              <Input
                type="search"
                aria-label="Поиск чатов"
                placeholder="Поиск"
                readOnly
                className="h-11 border-[#303958] bg-[#12192f] pl-10 text-white shadow-none placeholder:text-[#747b94] focus-visible:border-[#5263cf] focus-visible:ring-[#5263cf]/20"
              />
            </div>
          </div>

          <div className="min-h-0 flex-1 overflow-y-auto px-3 pt-7">
            <div className="flex items-center justify-between px-2">
              <h2 className="text-sm font-semibold text-[#9da2b7]">
                Личные чаты и группы
              </h2>
              <span className="text-xs text-[#666d84]">{chats.length}</span>
            </div>

            <div className="mt-3 space-y-1.5">
              {chats.map((chat) => (
                <Button
                  key={chat.name}
                  type="button"
                  variant="ghost"
                  onClick={() => {
                    setActiveChatId(chat.chatId);
                    setIsMobileChatOpen(true);
                  }}
                  className={cn(
                    "h-auto w-full justify-start gap-3 rounded-xl px-3 py-3 text-left text-white hover:bg-white/5 hover:text-white",
                    chat.chatId === activeChatId &&
                      "border border-[#384372] bg-[#222a4d] shadow-md shadow-black/10 hover:bg-[#273057]",
                  )}
                >
                  <UserAvatar
                    name={chat.name}
                    initials={chat.name[0]}
                    imageUrl={chat?.imageUrl}
                  />
                  <span className="min-w-0 flex-1">
                    <span className="flex items-center gap-2">
                      <span className="truncate text-sm font-semibold">
                        {chat.name}
                      </span>
                      {chat.chatType === "server" && (
                        <UsersRoundIcon className="size-3.5 shrink-0 text-[#7f8cff]" />
                      )}
                      <span className="ml-auto shrink-0 text-xs font-normal text-[#7f859c]">
                        {formatMessageTime(chat.lastMessageAt)}
                      </span>
                    </span>
                    <span className="mt-1 block truncate text-xs font-normal text-[#959bb1]">
                      {chat.lastMessage}
                    </span>
                  </span>
                </Button>
              ))}
            </div>
          </div>

          <div className="p-3 lg:pt-2">
            <Separator className="mb-3 bg-[#29304e]" />
            <Link to="/profile" viewTransition aria-label="Открыть профиль">
              <Card className="border-[#30395e] bg-[#1d2545] py-0 text-white transition-colors hover:bg-[#242d51]">
                <CardContent className="flex items-center gap-3 p-3">
                  <UserAvatar
                    name={username}
                    initials={userInitials}
                    imageUrl={user?.imageUrl}
                    className="size-10"
                  />
                  <div className="min-w-0 flex-1">
                    <p className="truncate text-sm font-semibold">{username}</p>
                    <p className="mt-0.5 truncate text-xs text-[#949ab1]">
                      Мой профиль
                    </p>
                  </div>
                  <ChevronRightIcon className="size-4 shrink-0 text-[#8f96ae]" />
                </CardContent>
              </Card>
            </Link>
          </div>
        </aside>

        <section
          className={cn(
            "h-full min-h-0 min-w-0 flex-1 flex-col bg-[#0a1022] lg:flex",
            isMobileChatOpen ? "flex" : "hidden",
          )}
        >
          <header className="flex min-h-20 items-center gap-4 border-b border-[#28304c] px-4 sm:px-6">
            <Button
              type="button"
              variant="ghost"
              size="icon"
              aria-label="Вернуться к списку чатов"
              onClick={() => setIsMobileChatOpen(false)}
              className="-ml-2 shrink-0 text-[#aab0c3] hover:bg-white/5 hover:text-white lg:hidden"
            >
              <ArrowLeftIcon className="size-5" />
            </Button>
            <UserAvatar
              name={activeChat.name}
              initials={activeChat.name[0]}
              className="size-11"
            />
            <div className="min-w-0 flex-1">
              <h2 className="truncate text-lg font-bold sm:text-xl">
                {activeChat.name}
              </h2>
              <p className="mt-0.5 text-xs text-[#959bb1]">
                {activeChat.chatType === "server" ? "8 участников" : "Личный чат"}
              </p>
            </div>
            {activeChat.chatType === "server" && (
              <Button
                type="button"
                variant="outline"
                className="hidden h-10 border-[#44518c] bg-transparent px-4 text-[#8997ff] hover:bg-[#6577f7]/10 hover:text-[#a8b1ff] sm:flex"
              >
                <UserPlusIcon className="size-4" />
                Добавить участника
              </Button>
            )}
            <Button
              type="button"
              variant="ghost"
              size="icon"
              aria-label="Действия с чатом"
              className="text-[#8f96ae] hover:bg-white/5 hover:text-white xl:hidden"
            >
              <MoreHorizontalIcon />
            </Button>
          </header>

          <div className="min-h-0 flex-1 overflow-y-auto px-4 py-6 sm:px-7 lg:px-10">
            <div className="mx-auto w-full max-w-3xl">
              <div className="flex items-center gap-4">
                <Separator className="flex-1 bg-[#28304c]" />
                <span className="text-xs font-medium text-[#7f859c]">Сегодня</span>
                <Separator className="flex-1 bg-[#28304c]" />
              </div>

              <div className="mt-8 space-y-6">
                {messages.map((message, index) => {
                  const isCurrentUser = message.author === "anton_dev";
                  const displayName = isCurrentUser
                    ? username
                    : message.author;

                  return (
                    <div
                      key={`${message.author}-${message.time}-${index}`}
                      className={cn(
                        "flex items-end gap-3 sm:gap-4",
                        isCurrentUser && "flex-row-reverse",
                      )}
                    >
                      <UserAvatar
                        name={displayName}
                        initials={
                          isCurrentUser ? userInitials : message.initials
                        }
                        imageUrl={isCurrentUser ? user?.imageUrl : undefined}
                        className="size-9 sm:size-10"
                      />

                      <div
                        className={cn(
                          "flex min-w-0 flex-1 flex-col",
                          isCurrentUser ? "items-end" : "items-start",
                        )}
                      >
                        <div
                          className={cn(
                            "flex flex-wrap items-baseline gap-x-2 gap-y-1 px-1",
                            isCurrentUser && "flex-row-reverse",
                          )}
                        >
                          <p className="text-sm font-semibold text-[#f1f2f7]">
                            {displayName}
                          </p>
                          <time className="text-xs text-[#737a93]">
                            {message.time}
                          </time>
                        </div>

                        <Bubble
                          align={isCurrentUser ? "end" : "start"}
                          variant={isCurrentUser ? "default" : "secondary"}
                          className="mt-1.5 max-w-[85%] sm:max-w-[75%]"
                        >
                          <BubbleContent
                            className={cn(
                              "rounded-2xl px-4 py-2.5 text-sm leading-6 shadow-md shadow-black/10 sm:text-base",
                              isCurrentUser
                                ? "rounded-br-md border-[#8490ff]/25 bg-[#5f6ff1]! text-white!"
                                : "rounded-bl-md border-[#303a5a] bg-[#18213b]! text-[#dfe2ec]!",
                            )}
                          >
                            {message.content}
                          </BubbleContent>
                        </Bubble>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>
          </div>

          <div className="border-t border-[#202844] px-4 py-4 sm:px-6 lg:px-8">
            <Card className="mx-auto max-w-4xl border-[#354064] bg-[#121a31] py-0 text-white shadow-lg shadow-black/10">
              <CardContent className="flex items-center gap-3 p-2 pl-4">
                <Input
                  aria-label="Сообщение"
                  placeholder={`Написать в ${activeChat.name}`}
                  readOnly
                  className="h-11 border-0 bg-transparent px-0 text-base shadow-none placeholder:text-[#798099] focus-visible:border-0 focus-visible:ring-0"
                />
                <Button
                  type="button"
                  size="icon-lg"
                  aria-label="Отправить сообщение"
                  className="size-11 rounded-full bg-[#5f6ff1] text-white hover:bg-[#7180f8]"
                >
                  <SendIcon className="size-5" />
                </Button>
              </CardContent>
            </Card>
          </div>
        </section>

        <aside
          className={cn(
            "hidden h-full w-64 shrink-0 flex-col border-l border-[#29304e] bg-[#0e1428]",
            activeChat.chatType === "server" && "xl:flex",
          )}
        >
          <div className="px-5 pt-7 pb-5">
            <div className="flex items-center gap-2">
              <UsersRoundIcon className="size-5 text-[#7f8cff]" />
              <h2 className="font-bold">Участники</h2>
              <span className="text-sm text-[#858ca5]">— 8</span>
            </div>
          </div>
          <Separator className="bg-[#29304e]" />
          <div className="min-h-0 flex-1 space-y-1 overflow-y-auto px-3 py-4">
            {members.map((member) => (
              <Button
                key={member.name}
                type="button"
                variant="ghost"
                className="h-auto w-full justify-start gap-3 px-2.5 py-2 text-[#c7cad7] hover:bg-white/5 hover:text-white"
              >
                <UserAvatar
                  name={member.name}
                  initials={member.initials}
                  imageUrl={
                    member.name === "anton_dev" ? user?.imageUrl : undefined
                  }
                  className="size-9"
                />
                <span className="truncate text-sm font-medium">
                  {member.name === "anton_dev" ? username : member.name}
                </span>
              </Button>
            ))}
          </div>
        </aside>
      </div>
    </main>
  );
}
