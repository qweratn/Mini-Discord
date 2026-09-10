import type {
  Chat,
  CreatedChat,
  ChatMember,
  ChatMessagesPage,
} from "@/feature/chats/chat-types";
import { apiClient } from "@/shared/api/api-client";

export async function getUsersChats(signal?: AbortSignal): Promise<Chat[]> {
  const response = await apiClient.get<Chat[]>("/chats", { signal });

  return response.data;
}

export async function createDirectChat(
  companionId: string,
  signal?: AbortSignal,
): Promise<CreatedChat> {
  const response = await apiClient.post<CreatedChat>(
    "/chats/direct",
    companionId,
    {
      headers: { "Content-Type": "application/json" },
      signal,
    },
  );

  return response.data;
}

export async function createServerChat(
  name: string,
  signal?: AbortSignal,
): Promise<CreatedChat> {
  const response = await apiClient.post<CreatedChat>("/chats/server", name, {
    headers: { "Content-Type": "application/json" },
    signal,
  });

  return response.data;
}

export async function addChatMember(
  chatId: string,
  userId: string,
  signal?: AbortSignal,
): Promise<void> {
  await apiClient.put(
    `/chats/${chatId}/members/${userId}`,
    undefined,
    { signal },
  );
}

export async function getChatMembers(
  chatId: string,
  signal?: AbortSignal,
): Promise<ChatMember[]> {
  const response = await apiClient.get<ChatMember[]>(
    `/chats/${chatId}/members`,
    { signal },
  );

  return response.data;
}

export async function getChatMessages(
  chatId: string,
  beforeMessageId?: string,
  signal?: AbortSignal,
): Promise<ChatMessagesPage> {
  const response = await apiClient.get<ChatMessagesPage>(
    `/chats/${chatId}/messages`,
    {
      params: beforeMessageId ? { beforeMessageId } : undefined,
      signal,
    },
  );

  return response.data;
}
