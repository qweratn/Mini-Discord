import { Bubble, BubbleContent } from "@/components/ui/bubble";
import { formatMessageTime } from "@/feature/chats/chat-formatters";
import type { ChatMessage } from "@/feature/chats/chat-types";
import { cn } from "@/lib/utils";

import { ChatAvatar } from "./ChatAvatar";

type MessageBubbleProps = {
  message: ChatMessage;
  isCurrentUser: boolean;
};

export function MessageBubble({
  message,
  isCurrentUser,
}: MessageBubbleProps) {
  return (
    <div
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
          <time dateTime={message.sentAt} className="text-xs text-[#737a93]">
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
}
