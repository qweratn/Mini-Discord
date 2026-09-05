import {
  ArrowLeftIcon,
  MoreHorizontalIcon,
  UserPlusIcon,
} from "lucide-react";

import { Button } from "@/components/ui/button";
import type { Chat } from "@/feature/chats/chat-types";

import { ChatAvatar } from "./ChatAvatar";

type ChatHeaderProps = {
  chat: Chat;
  onBack: () => void;
};

export function ChatHeader({ chat, onBack }: ChatHeaderProps) {
  return (
    <header className="flex min-h-20 items-center gap-4 border-b border-[#28304c] px-4 sm:px-6">
      <Button
        type="button"
        variant="ghost"
        size="icon"
        aria-label="Вернуться к списку чатов"
        onClick={onBack}
        className="-ml-2 shrink-0 text-[#aab0c3] hover:bg-white/5 hover:text-white lg:hidden"
      >
        <ArrowLeftIcon className="size-5" />
      </Button>
      <ChatAvatar
        name={chat.name}
        imageUrl={chat.imageUrl}
        className="size-11"
      />
      <div className="min-w-0 flex-1">
        <h2 className="truncate text-lg font-bold sm:text-xl">{chat.name}</h2>
        <p className="mt-0.5 text-xs text-[#959bb1]">
          {chat.chatType === "server" ? "Групповой чат" : "Личный чат"}
        </p>
      </div>
      {chat.chatType === "server" && (
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
  );
}
