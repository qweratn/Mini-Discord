import { useEffect, useState } from "react";

import { Bubble, BubbleContent } from "@/components/ui/bubble";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { Spinner } from "@/components/ui/spinner";
import {
  formatMessageTime,
  getRequestErrorMessage,
} from "@/feature/chats/chat-formatters";
import { getChatMessages } from "@/feature/chats/chats-api";
import type { ChatMessage } from "@/feature/chats/chat-types";
import { cn } from "@/lib/utils";

import { ChatAvatar } from "./ChatAvatar";

type ChatMessagesProps = {
  chatId: string;
  currentUsername: string;
};

export function ChatMessages({
  chatId,
  currentUsername,
}: ChatMessagesProps) {
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [reloadKey, setReloadKey] = useState(0);

  useEffect(() => {
    const controller = new AbortController();

    async function loadMessages() {
      try {
        setIsLoading(true);
        setError(null);

        const page = await getChatMessages(
          chatId,
          undefined,
          controller.signal,
        );

        // API возвращает новые сообщения первыми, для чата нужен прямой порядок.
        setMessages([...page.items].reverse());
      } catch (requestError: unknown) {
        if (!controller.signal.aborted) {
          setError(getRequestErrorMessage(requestError));
        }
      } finally {
        if (!controller.signal.aborted) {
          setIsLoading(false);
        }
      }
    }

    void loadMessages();

    return () => controller.abort();
  }, [chatId, reloadKey]);

  return (
    <div className="min-h-0 flex-1 overflow-y-auto px-4 py-6 sm:px-7 lg:px-10">
      <div className="mx-auto w-full max-w-3xl">
        <div className="flex items-center gap-4">
          <Separator className="flex-1 bg-[#28304c]" />
          <span className="text-xs font-medium text-[#7f859c]">Сообщения</span>
          <Separator className="flex-1 bg-[#28304c]" />
        </div>

        {isLoading && (
          <div className="flex items-center justify-center gap-2 py-16 text-sm text-[#8f96ae]">
            <Spinner />
            Загружаем сообщения...
          </div>
        )}

        {!isLoading && error && (
          <div className="py-16 text-center">
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

        {!isLoading && !error && messages.length === 0 && (
          <p className="py-16 text-center text-sm text-[#8f96ae]">
            В этом чате пока нет сообщений
          </p>
        )}

        {!isLoading && !error && messages.length > 0 && (
          <div className="mt-8 space-y-6">
            {messages.map((message) => {
              const isCurrentUser =
                message.author.username === currentUsername;

              return (
                <div
                  key={message.id}
                  className={cn(
                    "flex items-end gap-3 sm:gap-4",
                    isCurrentUser && "flex-row-reverse",
                  )}
                >
                  <ChatAvatar
                    name={message.author.username}
                    imageUrl={message.author.imageUrl}
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
                        {message.author.username}
                      </p>
                      <time className="text-xs text-[#737a93]">
                        {formatMessageTime(message.sentAt)}
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
        )}
      </div>
    </div>
  );
}
