import { UsersRoundIcon } from "lucide-react";
import { useEffect, useState } from "react";

import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { Spinner } from "@/components/ui/spinner";
import { getRequestErrorMessage } from "@/feature/chats/chat-formatters";
import { getChatMembers } from "@/feature/chats/chats-api";
import type { ChatMember } from "@/feature/chats/chat-types";

import { ChatAvatar } from "./ChatAvatar";

type ChatMembersProps = {
  chatId: string;
  refreshKey?: number;
};

export function ChatMembers({ chatId, refreshKey = 0 }: ChatMembersProps) {
  const [members, setMembers] = useState<ChatMember[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [reloadKey, setReloadKey] = useState(0);

  useEffect(() => {
    const controller = new AbortController();

    async function loadMembers() {
      try {
        setIsLoading(true);
        setError(null);

        const loadedMembers = await getChatMembers(
          chatId,
          controller.signal,
        );

        setMembers(loadedMembers);
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

    void loadMembers();

    return () => controller.abort();
  }, [chatId, refreshKey, reloadKey]);

  return (
    <aside className="chat-members-enter hidden h-full w-64 shrink-0 flex-col border-l border-[#29304e] bg-[#0e1428] xl:flex 2xl:w-72">
      <div className="px-5 pt-7 pb-5">
        <div className="flex items-center gap-2">
          <UsersRoundIcon className="size-5 text-[#7f8cff]" />
          <h2 className="font-bold">Участники</h2>
          {!isLoading && !error && (
            <span className="text-sm text-[#858ca5]">— {members.length}</span>
          )}
        </div>
      </div>
      <Separator className="bg-[#29304e]" />

      {isLoading && (
        <div className="flex items-center justify-center gap-2 py-10 text-sm text-[#8f96ae]">
          <Spinner />
          Загрузка...
        </div>
      )}

      {!isLoading && error && (
        <div className="px-4 py-8 text-center">
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

      {!isLoading && !error && members.length === 0 && (
        <p className="px-4 py-10 text-center text-sm text-[#8f96ae]">
          Участников пока нет
        </p>
      )}

      {!isLoading && !error && members.length > 0 && (
        <div className="min-h-0 flex-1 space-y-1 overflow-y-auto px-3 py-4">
          {members.map((member, index) => (
            <Button
              key={member.id}
              type="button"
              variant="ghost"
              style={{ animationDelay: `${Math.min(index, 10) * 35}ms` }}
              className="chat-member-enter h-auto w-full justify-start gap-3 px-2.5 py-2 text-[#c7cad7] hover:bg-white/5 hover:text-white"
            >
              <ChatAvatar
                name={member.name}
                imageUrl={member.imageUrl}
                className="size-9"
              />
              <span className="min-w-0 text-left">
                <span className="block truncate text-sm font-medium">
                  {member.name}
                </span>
                <span className="block truncate text-xs font-normal text-[#858ca5]">
                  {member.email}
                </span>
              </span>
            </Button>
          ))}
        </div>
      )}
    </aside>
  );
}
