import { useAuth } from "@clerk/react";
import { Navigate, Outlet } from "react-router";

import Loading from "@/shared/ui/Loading";

export default function GuestRoute() {
  const { isLoaded, isSignedIn } = useAuth();

  if (!isLoaded) {
    return <Loading />;
  }

  if (isSignedIn) {
    return <Navigate to="/chats" replace />;
  }

  return <Outlet />;
}
