import { Route, Routes } from "react-router";
import Home from "./pages/Home/Home";
import SignIn from "./pages/SignIn/SignIn";
import SignUp from "./pages/SignUp/SignUp";
import VerifyOtp from "./pages/VerifyOtp/VerifyOtp";

function App() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/sign-in" element={<SignIn />} />
      <Route path="/sign-up" element={<SignUp />} />
      <Route path="/verify-otp" element={<VerifyOtp />} />

      <Route path="*" element={<div>404</div>} />
    </Routes>
  );
}

export default App;
