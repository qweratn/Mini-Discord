import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { getInitials } from "@/feature/chats/chat-formatters";
import { cn } from "@/lib/utils";

type ChatAvatarProps = {
  name: string;
  imageUrl?: string | null;
  className?: string;
};

export function ChatAvatar({
  name,
  imageUrl,
  className,
}: ChatAvatarProps) {
  return (
    <Avatar
      className={cn(
        "size-11 bg-[#4f46a8] ring-1 ring-white/10",
        className,
      )}
    >
      {imageUrl && <AvatarImage src={imageUrl} alt={`Аватар ${name}`} />}
      <AvatarFallback className="bg-[#4f46a8] font-semibold text-white">
        {getInitials(name)}
      </AvatarFallback>
    </Avatar>
  );
}
