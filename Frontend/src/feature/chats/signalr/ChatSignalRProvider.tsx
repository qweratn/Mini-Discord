import {type ReactNode, useEffect, useState} from "react";
import {HubConnection, HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {useAuth} from "@clerk/react";
import { SignalRContext } from "./ signalr-context";

export function SignalRProvider({children,}: { children: ReactNode; }) {
    const {getToken, isLoaded, isSignedIn } = useAuth();
    const [connection, setConnection] =
        useState<HubConnection | null>(null);

    useEffect(() => {
        if (!isLoaded || !isSignedIn) {
            return;
        }

        const newConnection = new HubConnectionBuilder()
            .withUrl(import.meta.env.VITE_SIGNALR_URL, {
                accessTokenFactory: async () => {
                    return (await getToken()) ?? "";
                },
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        let disposed = false;
        async function startConnection() {
            try {
                await newConnection.start();

                if (!disposed) {
                    setConnection(newConnection);
                }
            } catch (error) {
                console.error(
                    "SignalR connection error:",
                    error,
                );
            }
        }

        void startConnection();

        return () => {
            disposed = true;
            void newConnection.stop();
        };

    }, [getToken, isLoaded, isSignedIn])

    return (
        <SignalRContext.Provider value={connection}>
            {children}
        </SignalRContext.Provider>
    );
}
