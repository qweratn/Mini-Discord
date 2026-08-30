import { Route, Routes } from "react-router";
import { useUserSync } from "@/feature/users/use-user-sync";
import GuestRoute from "@/shared/auth/GuestRoute";
import ProtectedRoute from "@/shared/auth/ProtectedRoute";
import { useApiClientAuth } from "@/shared/api/use-api-client-auth";
import Loading from "@/shared/ui/Loading";
import Chats from "./pages/Chats/Chats";
import Home from "./pages/Home/Home";
import Profile from "./pages/Profile/Profile";
import SignIn from "./pages/SignIn/SignIn";
import SignUp from "./pages/SignUp/SignUp";
import VerifyOtp from "./pages/VerifyOtp/VerifyOtp";

function App() {
  const isApiReady = useApiClientAuth();
  useUserSync(isApiReady);

  if (!isApiReady) {
    return <Loading />;
  }

  return (
    <Routes>
      <Route element={<GuestRoute />}>
        <Route path="/" element={<Home />} />
        <Route path="/sign-in" element={<SignIn />} />
        <Route path="/sign-up" element={<SignUp />} />
        <Route path="/verify-otp" element={<VerifyOtp />} />
      </Route>

      <Route element={<ProtectedRoute />}>
        <Route path="/chats" element={<Chats />} />
        <Route path="/profile" element={<Profile />} />
      </Route>

      <Route path="*" element={<div>404</div>} />
    </Routes>
  );
}

export default App;
