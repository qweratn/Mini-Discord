import { useAuth } from "@clerk/react";
import { useEffect, useRef } from "react";

import { syncCurrentUser } from "./user-api";

export function useUserSync(isApiReady: boolean) {
  const { isSignedIn, sessionId } = useAuth();
  const syncedSessionId = useRef<string | null>(null);

  useEffect(() => {
    if (!isSignedIn) {
      syncedSessionId.current = null;
      return;
    }

    if (
      !isApiReady ||
      !sessionId ||
      syncedSessionId.current === sessionId
    ) {
      return;
    }

    syncedSessionId.current = sessionId;

    void syncCurrentUser().catch((error: unknown) => {
      syncedSessionId.current = null;
      console.error("Failed to synchronize user.", error);
    });
  }, [isApiReady, isSignedIn, sessionId]);
}
