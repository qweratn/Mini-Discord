import type { HubConnection } from "@microsoft/signalr";
import { createContext, useContext } from "react";

export const SignalRContext =
    createContext<HubConnection | null>(null);

export function useSignalR() {
    return useContext(SignalRContext);
}
