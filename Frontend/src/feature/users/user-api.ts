import { apiClient } from "@/shared/api/api-client";

export async function syncCurrentUser(token?: string): Promise<void> {
  await apiClient.put("/users/sync", undefined, {
    headers: token
      ? {
          Authorization: `Bearer ${token}`,
        }
      : undefined,
  });
}
