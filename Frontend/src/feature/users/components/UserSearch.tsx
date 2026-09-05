import { CheckIcon, SearchIcon, UserRoundSearchIcon } from "lucide-react";

import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Spinner } from "@/components/ui/spinner";
import { cn } from "@/lib/utils";

export type SearchableUser = {
  id: string;
  username: string;
  email: string;
  imageUrl?: string | null;
};

type UserSearchProps = {
  users: SearchableUser[];
  query: string;
  onQueryChange: (query: string) => void;
  selectedUserIds?: string[];
  onUserToggle?: (user: SearchableUser) => void;
  label?: string;
  placeholder?: string;
  selectionMode?: "single" | "multiple";
  isLoading?: boolean;
  minQueryLength?: number;
  className?: string;
};

function initials(name: string) {
  return name.split(/\s+/).slice(0, 2).map((part) => part[0]).join("").toUpperCase();
}

export function UserSearch({
  users,
  query,
  onQueryChange,
  selectedUserIds = [],
  onUserToggle,
  label = "Найти пользователя",
  placeholder = "Имя пользователя или email",
  selectionMode = "single",
  isLoading = false,
  minQueryLength = 2,
  className,
}: UserSearchProps) {
  const normalizedQuery = query.trim().toLocaleLowerCase("ru");
  const visibleUsers = normalizedQuery.length < minQueryLength ? [] : users.filter(
    (user) => user.username.toLocaleLowerCase("ru").includes(normalizedQuery)
      || user.email.toLocaleLowerCase("ru").includes(normalizedQuery),
  );

  return (
    <div className={className}>
      <label htmlFor="user-search" className="mb-2 block text-xs font-semibold tracking-wide text-[#9da5bd] uppercase">
        {label}
      </label>
      <div className="relative">
        <SearchIcon className="pointer-events-none absolute top-1/2 left-3.5 size-4 -translate-y-1/2 text-[#737c98]" />
        <Input
          id="user-search"
          type="search"
          value={query}
          onChange={(event) => onQueryChange(event.target.value)}
          placeholder={placeholder}
          autoComplete="off"
          className="h-12 border-[#303a5b] bg-[#0c1328] pr-10 pl-10 text-white placeholder:text-[#666f89] focus-visible:border-[#6475ed] focus-visible:ring-[#6475ed]/20"
        />
        {isLoading && <Spinner className="absolute top-1/2 right-3.5 size-4 -translate-y-1/2 text-[#8390ff]" />}
      </div>

      <div className="mt-3 min-h-48">
        {normalizedQuery.length < minQueryLength && (
          <div className="user-search-empty flex h-48 flex-col items-center justify-center rounded-xl border border-dashed border-[#303957] bg-[#0d1428]/50 px-6 text-center">
            <div className="flex size-11 items-center justify-center rounded-full bg-[#20294d] text-[#8290ff]">
              <UserRoundSearchIcon className="size-5" />
            </div>
            <p className="mt-3 text-sm font-medium text-[#c9cee0]">Найдите нужного человека</p>
            <p className="mt-1 text-xs text-[#747d97]">Введите минимум {minQueryLength} символа</p>
          </div>
        )}

        {!isLoading && normalizedQuery.length >= minQueryLength && visibleUsers.length === 0 && (
          <div className="user-search-empty flex h-48 items-center justify-center rounded-xl border border-dashed border-[#303957] px-6 text-center text-sm text-[#858da7]">
            Никого не нашли. Проверьте имя или email.
          </div>
        )}

        {visibleUsers.length > 0 && (
          <div className="space-y-1" role="listbox" aria-multiselectable={selectionMode === "multiple"}>
            {visibleUsers.map((user, index) => {
              const selected = selectedUserIds.includes(user.id);
              return (
                <Button
                  key={user.id}
                  type="button"
                  variant="ghost"
                  role="option"
                  aria-selected={selected}
                  onClick={() => onUserToggle?.(user)}
                  style={{ animationDelay: `${Math.min(index, 8) * 45}ms` }}
                  className={cn(
                    "user-search-result h-auto w-full justify-start gap-3 rounded-xl border border-transparent px-3 py-2.5 text-left text-white hover:bg-white/5 hover:text-white",
                    selected && "border-[#495895] bg-[#222c52] hover:bg-[#27315c]",
                  )}
                >
                  <Avatar className="size-10 ring-2 ring-[#303a61]">
                    {user.imageUrl && <AvatarImage src={user.imageUrl} alt={user.username} />}
                    <AvatarFallback className="bg-[#29335b] text-xs font-semibold text-[#cbd2ff]">{initials(user.username)}</AvatarFallback>
                  </Avatar>
                  <span className="min-w-0 flex-1">
                    <span className="block truncate text-sm font-semibold">{user.username}</span>
                    <span className="mt-0.5 block truncate text-xs font-normal text-[#858da7]">{user.email}</span>
                  </span>
                  <span className={cn(
                    "flex size-6 items-center justify-center rounded-full border border-[#46506d] text-transparent transition-all duration-200",
                    selected && "border-[#7180f8] bg-[#7180f8] text-white shadow-md shadow-[#6272ec]/30",
                  )}>
                    <CheckIcon className="size-3.5" />
                  </span>
                </Button>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
}
