import "./App.css";

function App() {
  return (
    <main className="flex min-h-screen items-center justify-center bg-slate-950">
      <div className="rounded-xl border border-slate-800 bg-slate-900 p-8 shadow-xl">
        <h1 className="text-3xl font-bold text-white">Mini Discord</h1>

        <p className="mt-2 text-slate-400">
          React, Vite and Tailwind are working.
        </p>

        <button
          className="
            mt-6 rounded-lg bg-indigo-600 px-4 py-2
            font-medium text-white transition-colors
            hover:bg-indigo-500
          "
        >
          Sign in
        </button>
      </div>
    </main>
  );
}

export default App;
