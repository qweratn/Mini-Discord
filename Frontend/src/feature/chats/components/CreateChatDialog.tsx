import { Dialog } from "@base-ui/react/dialog";
import { MessageCircleIcon, UserRoundIcon, UsersRoundIcon, XIcon } from "lucide-react";
import { useState } from "react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  UserSearch,
  type SearchableUser,
} from "@/feature/users/components/UserSearch";
import { cn } from "@/lib/utils";

type ChatMode = "direct" | "group";

type CreateChatDialogProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  users?: SearchableUser[];
  onSubmit?: (value: {
    mode: ChatMode;
    name?: string;
    users: SearchableUser[];
  }) => void;
};

// Preview data only. Pass `users` from the caller when a data source is ready.
const previewUsers: SearchableUser[] = [
  { id: "1", username: "Алина Воронова", email: "alina@example.com" },
  { id: "2", username: "Максим Орлов", email: "maxim@example.com" },
  { id: "3", username: "София Белова", email: "sofia@example.com" },
  { id: "4", username: "Илья Лебедев", email: "ilya@example.com" },
];

export function CreateChatDialog({
  open,
  onOpenChange,
  users = previewUsers,
  onSubmit,
}: CreateChatDialogProps) {
  const [mode, setMode] = useState<ChatMode>("direct");
  const [query, setQuery] = useState("");
  const [groupName, setGroupName] = useState("");
  const [selectedUsers, setSelectedUsers] = useState<SearchableUser[]>([]);
  const selectedIds = selectedUsers.map((user) => user.id);
  const canSubmit = mode === "direct"
    ? selectedUsers.length === 1
    : groupName.trim().length >= 2 && selectedUsers.length > 0;

  function reset() {
    setMode("direct");
    setQuery("");
    setGroupName("");
    setSelectedUsers([]);
  }

  function setOpen(nextOpen: boolean) {
    if (!nextOpen) reset();
    onOpenChange(nextOpen);
  }

  function selectMode(nextMode: ChatMode) {
    setMode(nextMode);
    if (nextMode === "direct") {
      setSelectedUsers((current) => current.slice(0, 1));
    }
  }

  function toggleUser(user: SearchableUser) {
    if (mode === "direct") {
      setSelectedUsers([user]);
      return;
    }

    setSelectedUsers((current) =>
      current.some((item) => item.id === user.id)
        ? current.filter((item) => item.id !== user.id)
        : [...current, user],
    );
  }

  function submit() {
    if (!canSubmit) return;
    onSubmit?.({
      mode,
      name: mode === "group" ? groupName.trim() : undefined,
      users: selectedUsers,
    });
    setOpen(false);
  }

  return (
    <Dialog.Root open={open} onOpenChange={setOpen}>
      <Dialog.Portal>
        <Dialog.Backdrop className="fixed inset-0 z-50 bg-[#030614]/75 backdrop-blur-sm transition-opacity duration-300 data-[ending-style]:opacity-0 data-[starting-style]:opacity-0" />
        <Dialog.Viewport className="fixed inset-0 z-50 flex items-end justify-center sm:items-center sm:p-6">
          <Dialog.Popup className="chat-dialog-enter flex max-h-[92dvh] w-full flex-col overflow-hidden rounded-t-[1.75rem] border border-[#303a61] bg-[#11182e] text-white shadow-2xl shadow-black/50 outline-none sm:max-w-[620px] sm:rounded-[1.75rem]">
            <header className="flex items-start justify-between border-b border-[#293250] px-5 py-5 sm:px-6">
              <div>
                <Dialog.Title className="text-2xl font-bold tracking-tight">Новый чат</Dialog.Title>
                <Dialog.Description className="mt-1 text-sm text-[#9199b2]">
                  Выберите формат и найдите людей
                </Dialog.Description>
              </div>
              <Dialog.Close className="flex size-9 items-center justify-center rounded-full text-[#929ab3] transition-all hover:rotate-6 hover:bg-white/7 hover:text-white" aria-label="Закрыть">
                <XIcon className="size-5" />
              </Dialog.Close>
            </header>

            <div className="min-h-0 flex-1 overflow-y-auto px-5 py-5 sm:px-6">
              <div className="relative grid grid-cols-2 gap-1 rounded-xl border border-[#303a5b] bg-[#0b1124] p-1">
                <span className={cn(
                  "absolute top-1 bottom-1 w-[calc(50%-6px)] rounded-lg bg-[#27315b] shadow-sm transition-transform duration-300 ease-out",
                  mode === "group" && "translate-x-[calc(100%+4px)]",
                )} />
                {([
                  ["direct", UserRoundIcon, "Личный чат"],
                  ["group", UsersRoundIcon, "Группа"],
                ] as const).map(([value, Icon, label]) => (
                  <Button key={value} variant="ghost" onClick={() => selectMode(value)} className={cn(
                    "relative z-10 h-10 gap-2 text-[#8f97b1] hover:bg-transparent hover:text-white",
                    mode === value && "text-white",
                  )}>
                    <Icon className="size-4" />{label}
                  </Button>
                ))}
              </div>

              {mode === "group" && (
                <label className="chat-mode-section mt-5 block">
                  <span className="mb-2 block text-xs font-semibold tracking-wide text-[#9da5bd] uppercase">Название группы</span>
                  <Input value={groupName} onChange={(event) => setGroupName(event.target.value)} maxLength={64} placeholder="Например, Команда продукта" className="h-11 border-[#303a5b] bg-[#0c1328] text-white placeholder:text-[#666f89] focus-visible:border-[#6475ed]" />
                  <span className="mt-1.5 block text-right text-xs text-[#646d87]">{groupName.length}/64</span>
                </label>
              )}

              {mode === "group" && selectedUsers.length > 0 && (
                <div className="mt-4 flex flex-wrap gap-2">
                  {selectedUsers.map((user, index) => (
                    <Button key={user.id} variant="ghost" onClick={() => toggleUser(user)} style={{ animationDelay: `${index * 40}ms` }} className="selected-user-chip h-7 rounded-full border border-[#48548a] bg-[#252e55] px-2 text-xs text-[#dce1ff] hover:bg-[#303a67] hover:text-white">
                      {user.username}<XIcon className="size-3.5" />
                    </Button>
                  ))}
                </div>
              )}

              <UserSearch
                className="mt-5"
                users={users}
                query={query}
                onQueryChange={setQuery}
                selectedUserIds={selectedIds}
                onUserToggle={toggleUser}
                selectionMode={mode === "direct" ? "single" : "multiple"}
                label={mode === "direct" ? "Найти собеседника" : "Добавить участников"}
              />
            </div>

            <footer className="flex items-center justify-between gap-3 border-t border-[#293250] bg-[#0e152a] px-5 py-4 sm:px-6">
              <p className="hidden text-xs text-[#737c96] sm:block">
                {mode === "direct"
                  ? selectedUsers[0]?.username ?? "Выберите одного человека"
                  : selectedUsers.length ? `${selectedUsers.length + 1} участников вместе с вами` : "Добавьте хотя бы одного участника"}
              </p>
              <Button disabled={!canSubmit} onClick={submit} className="ml-auto h-11 min-w-40 bg-[#6475ed] px-5 text-white shadow-lg shadow-[#5362db]/20 transition-all hover:-translate-y-0.5 hover:bg-[#7584f5] hover:shadow-xl">
                {mode === "direct" ? <MessageCircleIcon /> : <UsersRoundIcon />}
                {mode === "direct" ? "Начать чат" : `Создать группу${selectedUsers.length ? ` · ${selectedUsers.length}` : ""}`}
              </Button>
            </footer>
          </Dialog.Popup>
        </Dialog.Viewport>
      </Dialog.Portal>
    </Dialog.Root>
  );
}
