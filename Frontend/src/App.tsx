import { Route, Routes } from "react-router";
import Home from "./pages/Home/Home";
import SignIn from "./pages/SignIn/SignIn";

function App() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/sign-in" element={<SignIn />} />

      <Route path="*" element={<div>404</div>} />
    </Routes>
  );
}

export default App;
