import { apiClient } from "@/shared/api/api-client";

import type { SearchableUser } from "./user-types";

export async function syncCurrentUser(token?: string): Promise<void> {
  await apiClient.put("/users/sync", undefined, {
    headers: token
      ? {
          Authorization: `Bearer ${token}`,
        }
      : undefined,
  });
}

export async function searchUsers(
  filter: string,
  signal?: AbortSignal,
): Promise<SearchableUser[]> {
  const response = await apiClient.get<SearchableUser[]>("/users/search", {
    params: { filter },
    signal,
  });

  return response.data;
}
