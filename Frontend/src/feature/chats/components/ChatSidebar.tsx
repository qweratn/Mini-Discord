import { useUser } from "@clerk/react";
import {
  ChevronRightIcon,
  SearchIcon,
  SquarePenIcon,
  UsersRoundIcon,
} from "lucide-react";
import { useEffect, useState } from "react";
import { Link } from "react-router";

import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Separator } from "@/components/ui/separator";
import { Spinner } from "@/components/ui/spinner";
import {
  formatMessageTime,
  getRequestErrorMessage,
} from "@/feature/chats/chat-formatters";
import { getUsersChats } from "@/feature/chats/chats-api";
import type { Chat } from "@/feature/chats/chat-types";
import { cn } from "@/lib/utils";
import { toastManager } from "@/lib/toast";

import { ChatAvatar } from "./ChatAvatar";

type ChatSidebarProps = {
  activeChatId: string | null;
  isMobileChatOpen: boolean;
  onChatSelect: (chat: Chat) => void;
  onChatsLoaded: (chats: Chat[]) => void;
};

export function ChatSidebar({
  activeChatId,
  isMobileChatOpen,
  onChatSelect,
  onChatsLoaded,
}: ChatSidebarProps) {
  const { user } = useUser();
  const [chats, setChats] = useState<Chat[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [reloadKey, setReloadKey] = useState(0);
  const username = user?.username ?? user?.fullName ?? "Пользователь";

  useEffect(() => {
    const controller = new AbortController();

    async function loadChats() {
      try {
        setIsLoading(true);
        setError(null);

        const loadedChats = await getUsersChats(controller.signal);

        setChats(loadedChats);
        onChatsLoaded(loadedChats);
      } catch (requestError: unknown) {
        if (controller.signal.aborted) {
          return;
        }

        const message = getRequestErrorMessage(requestError);

        setError(message);
        toastManager.add({
          type: "error",
          title: "Ошибка при загрузке чатов",
          description: message,
        });
      } finally {
        if (!controller.signal.aborted) {
          setIsLoading(false);
        }
      }
    }

    void loadChats();

    return () => controller.abort();
  }, [onChatsLoaded, reloadKey]);

  return (
    <aside
      className={cn(
        "h-full w-full shrink-0 flex-col bg-[#0e1428] lg:flex lg:w-80 lg:border-r lg:border-[#29304e]",
        isMobileChatOpen ? "hidden" : "flex",
      )}
    >
      <div className="flex items-center justify-between px-5 pt-6 pb-5">
        <div>
          <p className="app-logo-transition text-xs font-medium tracking-[0.18em] text-[#707791] uppercase">
            Mini Discord
          </p>
          <h1 className="mt-1 text-2xl font-bold tracking-tight">Сообщения</h1>
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

        {isLoading && (
          <div className="flex items-center justify-center gap-2 py-10 text-sm text-[#8f96ae]">
            <Spinner />
            Загружаем чаты...
          </div>
        )}

        {!isLoading && error && (
          <div className="px-2 py-8 text-center">
            <p className="text-sm text-red-300">{error}</p>
            <Button
              type="button"
              size="sm"
              variant="outline"
              className="mt-4 border-[#44518c] bg-transparent text-[#a8b1ff]"
              onClick={() => setReloadKey((current) => current + 1)}
            >
              Повторить
            </Button>
          </div>
        )}

        {!isLoading && !error && chats.length === 0 && (
          <p className="px-2 py-10 text-center text-sm text-[#8f96ae]">
            У вас пока нет чатов
          </p>
        )}

        {!isLoading && !error && chats.length > 0 && (
          <div className="mt-3 space-y-1.5">
            {chats.map((chat, index) => (
              <Button
                key={chat.chatId}
                type="button"
                variant="ghost"
                onClick={() => onChatSelect(chat)}
                style={{ animationDelay: `${Math.min(index, 8) * 45}ms` }}
                className={cn(
                  "chat-list-item h-auto w-full justify-start gap-3 rounded-xl px-3 py-3 text-left text-white hover:bg-white/5 hover:text-white",
                  chat.chatId === activeChatId &&
                    "border border-[#384372] bg-[#222a4d] shadow-md shadow-black/10 hover:bg-[#273057]",
                )}
              >
                <ChatAvatar name={chat.name} imageUrl={chat.imageUrl} />
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
                    {chat.lastMessage ?? "Сообщений пока нет"}
                  </span>
                </span>
              </Button>
            ))}
          </div>
        )}
      </div>

      <div className="p-3 lg:pt-2">
        <Separator className="mb-3 bg-[#29304e]" />
        <Link to="/profile" viewTransition aria-label="Открыть профиль">
          <Card className="border-[#30395e] bg-[#1d2545] py-0 text-white transition-colors hover:bg-[#242d51]">
            <CardContent className="flex items-center gap-3 p-3">
              <ChatAvatar
                name={username}
                imageUrl={user?.imageUrl}
                className="app-profile-avatar size-10"
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
  );
}
