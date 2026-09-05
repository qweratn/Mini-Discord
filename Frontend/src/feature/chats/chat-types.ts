export type Chat = {
  chatId: string;
  name: string;
  chatType: "server" | "direct";
  imageUrl: string | null;
  lastMessage: string | null;
  lastMessageAt: string | null;
};

export type ChatMember = {
  id: string;
  name: string;
  email: string;
  imageUrl: string | null;
};

export type MessageAuthor = {
  id: string;
  username: string;
  imageUrl: string | null;
};

export type ChatMessage = {
  id: string;
  chatId: string;
  content: string;
  author: MessageAuthor;
  sentAt: string;
};

export type ChatMessagesPage = {
  items: ChatMessage[];
  nextBeforeMessageId: string | null;
  hasMore: boolean;
};
