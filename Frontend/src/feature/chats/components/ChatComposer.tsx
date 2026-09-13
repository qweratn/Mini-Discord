import { SendIcon } from "lucide-react";
import { type FormEvent, useState } from "react";

import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Spinner } from "@/components/ui/spinner";
import { getRequestErrorMessage } from "@/feature/chats/chat-formatters";
import { sendChatMessage } from "@/feature/chats/chats-api";
import { toastManager } from "@/lib/toast";

const MAX_MESSAGE_LENGTH = 2000;

type ChatComposerProps = {
  chatId: string;
  chatName: string;
  onMessageSent: () => void;
};

export function ChatComposer({
  chatId,
  chatName,
  onMessageSent,
}: ChatComposerProps) {
  const [content, setContent] = useState("");
  const [isSending, setIsSending] = useState(false);
  const normalizedContent = content.trim();

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!normalizedContent || isSending) return;

    try {
      setIsSending(true);
      await sendChatMessage(chatId, normalizedContent);
      setContent("");
      onMessageSent();
    } catch (error: unknown) {
      toastManager.add({
        type: "error",
        title: "Не удалось отправить сообщение",
        description: getRequestErrorMessage(error),
      });
    } finally {
      setIsSending(false);
    }
  }

  return (
    <div className="border-t border-[#202844] px-4 py-4 sm:px-6 lg:px-8">
      <form onSubmit={(event) => void submit(event)}>
        <Card className="mx-auto max-w-4xl border-[#354064] bg-[#121a31] py-0 text-white shadow-lg shadow-black/10 2xl:max-w-6xl">
          <CardContent className="flex items-center gap-3 p-2 pl-4">
            <Input
              aria-label="Сообщение"
              value={content}
              onChange={(event) => setContent(event.target.value)}
              maxLength={MAX_MESSAGE_LENGTH}
              disabled={isSending}
              autoComplete="off"
              placeholder={`Написать в ${chatName}`}
              className="h-11 border-0 bg-transparent px-0 text-base shadow-none placeholder:text-[#798099] focus-visible:border-0 focus-visible:ring-0"
            />
            <Button
              type="submit"
              size="icon-lg"
              aria-label="Отправить сообщение"
              disabled={!normalizedContent || isSending}
              className="size-11 rounded-full bg-[#5f6ff1] text-white hover:bg-[#7180f8]"
            >
              {isSending ? <Spinner /> : <SendIcon className="size-5" />}
            </Button>
          </CardContent>
        </Card>
      </form>
    </div>
  );
}
