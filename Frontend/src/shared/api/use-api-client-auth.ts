import { useAuth } from "@clerk/react";
import { useLayoutEffect } from "react";

import { apiClient } from "./api-client";

export function useApiClientAuth() {
  const { getToken, isLoaded } = useAuth();

  useLayoutEffect(() => {
    if (!isLoaded) {
      return;
    }

    const interceptorId = apiClient.interceptors.request.use(
      async (config) => {
        const token = await getToken();

        if (token) {
          config.headers.set("Authorization", `Bearer ${token}`);
        }

        return config;
      },
    );

    return () => {
      apiClient.interceptors.request.eject(interceptorId);
    };
  }, [getToken, isLoaded]);

  return isLoaded;
}
