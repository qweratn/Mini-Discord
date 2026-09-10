import { Dialog } from "@base-ui/react/dialog";
import { UserPlusIcon, XIcon } from "lucide-react";
import { useEffect, useState } from "react";

import { Button } from "@/components/ui/button";
import { Spinner } from "@/components/ui/spinner";
import { getRequestErrorMessage } from "@/feature/chats/chat-formatters";
import { addChatMember, getChatMembers } from "@/feature/chats/chats-api";
import { UserSearch } from "@/feature/users/components/UserSearch";
import { searchUsers } from "@/feature/users/user-api";
import type { SearchableUser } from "@/feature/users/user-types";
import { toastManager } from "@/lib/toast";

type AddChatMemberDialogProps = {
  chatId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onMemberAdded: () => void;
};

export function AddChatMemberDialog({
  chatId,
  open,
  onOpenChange,
  onMemberAdded,
}: AddChatMemberDialogProps) {
  const [query, setQuery] = useState("");
  const [users, setUsers] = useState<SearchableUser[]>([]);
  const [selectedUser, setSelectedUser] = useState<SearchableUser | null>(null);
  const [memberIds, setMemberIds] = useState<Set<string>>(new Set());
  const [isSearching, setIsSearching] = useState(false);
  const [isAdding, setIsAdding] = useState(false);
  const [searchError, setSearchError] = useState<string | null>(null);

  useEffect(() => {
    if (!open) return;

    const controller = new AbortController();

    void getChatMembers(chatId, controller.signal)
      .then((members) => {
        setMemberIds(new Set(members.map((member) => member.id)));
      })
      .catch(() => {
        // The backend still protects the endpoint from duplicate memberships.
      });

    return () => controller.abort();
  }, [chatId, open]);

  useEffect(() => {
    const normalizedQuery = query.trim();

    if (!open || normalizedQuery.length < 2) return;

    const controller = new AbortController();
    const timeoutId = window.setTimeout(async () => {
      try {
        setSearchError(null);
        const foundUsers = await searchUsers(normalizedQuery, controller.signal);
        setUsers(foundUsers.filter((user) => !memberIds.has(user.id)));
      } catch (error: unknown) {
        if (controller.signal.aborted) return;
        setUsers([]);
        setSearchError(getRequestErrorMessage(error));
      } finally {
        if (!controller.signal.aborted) setIsSearching(false);
      }
    }, 350);

    return () => {
      window.clearTimeout(timeoutId);
      controller.abort();
    };
  }, [memberIds, open, query]);

  function reset() {
    setQuery("");
    setUsers([]);
    setSelectedUser(null);
    setMemberIds(new Set());
    setIsSearching(false);
    setSearchError(null);
  }

  function setOpen(nextOpen: boolean) {
    if (isAdding && !nextOpen) return;
    if (!nextOpen) reset();
    onOpenChange(nextOpen);
  }

  function changeQuery(nextQuery: string) {
    setQuery(nextQuery);
    setSearchError(null);

    if (nextQuery.trim().length < 2) {
      setUsers([]);
      setIsSearching(false);
      return;
    }

    setIsSearching(true);
  }

  async function addMember() {
    if (!selectedUser || isAdding) return;

    try {
      setIsAdding(true);
      await addChatMember(chatId, selectedUser.id);

      toastManager.add({
        type: "success",
        title: "Участник добавлен",
        description: `${selectedUser.username} теперь состоит на сервере.`,
      });
      onMemberAdded();
      reset();
      onOpenChange(false);
    } catch (error: unknown) {
      toastManager.add({
        type: "error",
        title: "Не удалось добавить участника",
        description: getRequestErrorMessage(error),
      });
    } finally {
      setIsAdding(false);
    }
  }

  return (
    <Dialog.Root open={open} onOpenChange={setOpen}>
      <Dialog.Portal>
        <Dialog.Backdrop className="fixed inset-0 z-50 bg-[#030614]/75 backdrop-blur-sm transition-opacity duration-300 data-[ending-style]:opacity-0 data-[starting-style]:opacity-0" />
        <Dialog.Viewport className="fixed inset-0 z-50 flex items-end justify-center sm:items-center sm:p-6">
          <Dialog.Popup className="chat-dialog-enter flex max-h-[92dvh] w-full flex-col overflow-hidden rounded-t-[1.75rem] border border-[#303a61] bg-[#11182e] text-white shadow-2xl shadow-black/50 outline-none sm:max-w-[560px] sm:rounded-[1.75rem]">
            <header className="flex items-start justify-between border-b border-[#293250] px-5 py-5 sm:px-6">
              <div>
                <Dialog.Title className="text-2xl font-bold tracking-tight">
                  Добавить участника
                </Dialog.Title>
                <Dialog.Description className="mt-1 text-sm text-[#9199b2]">
                  Найдите пользователя по имени или email
                </Dialog.Description>
              </div>
              <Dialog.Close
                aria-label="Закрыть"
                disabled={isAdding}
                className="flex size-9 items-center justify-center rounded-full text-[#929ab3] transition-all hover:bg-white/7 hover:text-white disabled:opacity-50"
              >
                <XIcon className="size-5" />
              </Dialog.Close>
            </header>

            <div className="min-h-0 flex-1 overflow-y-auto px-5 py-5 sm:px-6">
              <UserSearch
                users={users}
                query={query}
                onQueryChange={changeQuery}
                selectedUserIds={selectedUser ? [selectedUser.id] : []}
                onUserToggle={setSelectedUser}
                label="Пользователь"
                isLoading={isSearching}
                error={searchError}
                disabled={isAdding}
              />
            </div>

            <footer className="flex items-center justify-end border-t border-[#293250] bg-[#0e152a] px-5 py-4 sm:px-6">
              <Button
                type="button"
                disabled={!selectedUser || isAdding}
                onClick={() => void addMember()}
                className="h-11 min-w-44 bg-[#6475ed] px-5 text-white hover:bg-[#7584f5]"
              >
                {isAdding ? <Spinner /> : <UserPlusIcon />}
                {isAdding ? "Добавляем..." : "Добавить участника"}
              </Button>
            </footer>
          </Dialog.Popup>
        </Dialog.Viewport>
      </Dialog.Portal>
    </Dialog.Root>
  );
}
