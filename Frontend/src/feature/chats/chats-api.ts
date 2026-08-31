import type { Chat } from "@/feature/chats/chat-types";
import { apiClient } from "@/shared/api/api-client";

export async function getUsersChats(signal?: AbortSignal): Promise<Chat[]> {
  const response = await apiClient.get<Chat[]>("/chats", { signal });

  return response.data;
}
