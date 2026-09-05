import { useUser } from "@clerk/react";
import { useCallback, useState } from "react";

import { ChatComposer } from "@/feature/chats/components/ChatComposer";
import { ChatHeader } from "@/feature/chats/components/ChatHeader";
import { ChatMembers } from "@/feature/chats/components/ChatMembers";
import { ChatMessages } from "@/feature/chats/components/ChatMessages";
import { ChatSidebar } from "@/feature/chats/components/ChatSidebar";
import type { Chat } from "@/feature/chats/chat-types";
import { cn } from "@/lib/utils";

export default function Chats() {
  const { user } = useUser();
  const [activeChat, setActiveChat] = useState<Chat | null>(null);
  const [isMobileChatOpen, setIsMobileChatOpen] = useState(false);
  const username = user?.username ?? user?.fullName ?? "Пользователь";

  const handleChatsLoaded = useCallback((loadedChats: Chat[]) => {
    setActiveChat((currentChat) => {
      if (!currentChat) {
        return loadedChats[0] ?? null;
      }

      return (
        loadedChats.find((chat) => chat.chatId === currentChat.chatId) ??
        loadedChats[0] ??
        null
      );
    });
  }, []);

  function handleChatSelect(chat: Chat) {
    setActiveChat(chat);
    setIsMobileChatOpen(true);
  }

  return (
    <main className="h-dvh overflow-hidden bg-[#080c1c] text-white">
      <div className="flex h-full min-h-0 w-full">
        <ChatSidebar
          activeChatId={activeChat?.chatId ?? null}
          isMobileChatOpen={isMobileChatOpen}
          onChatSelect={handleChatSelect}
          onChatsLoaded={handleChatsLoaded}
        />

        <section
          className={cn(
            "h-full min-h-0 min-w-0 flex-1 flex-col bg-[#0a1022] lg:flex",
            isMobileChatOpen ? "flex" : "hidden",
          )}
        >
          {activeChat ? (
            <div
              key={activeChat.chatId}
              className="chat-workspace-enter flex min-h-0 flex-1 flex-col"
            >
              <ChatHeader
                chat={activeChat}
                onBack={() => setIsMobileChatOpen(false)}
              />
              <ChatMessages
                chatId={activeChat.chatId}
                currentUsername={username}
              />
              <ChatComposer chatName={activeChat.name} />
            </div>
          ) : (
            <div className="flex h-full items-center justify-center px-4 text-center">
              <div>
                <h2 className="text-xl font-bold">Выберите чат</h2>
                <p className="mt-2 text-sm text-[#969bb5]">
                  Выберите существующий чат или создайте новый.
                </p>
              </div>
            </div>
          )}
        </section>

        {activeChat?.chatType === "server" && (
          <ChatMembers key={activeChat.chatId} chatId={activeChat.chatId} />
        )}
      </div>
    </main>
  );
}
