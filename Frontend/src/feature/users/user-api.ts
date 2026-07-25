import { apiClient } from "@/shared/api/api-client";

export async function syncCurrentUser(): Promise<void> {
  await apiClient.put("/users/sync");
}
