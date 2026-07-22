import { Toast } from "@base-ui/react/toast";
import { CheckCircle2Icon, CircleAlertIcon, XIcon } from "lucide-react";

import { cn } from "@/lib/utils";
import { toastManager } from "@/lib/toast";

function ToastList() {
  const { toasts } = Toast.useToastManager();

  return toasts.map((toast) => {
    const isError = toast.type === "error";

    return (
      <Toast.Root
        key={toast.id}
        toast={toast}
        className={cn(
          "relative w-full rounded-xl border bg-[#161a2e] text-white shadow-2xl shadow-black/40 transition-[transform,opacity] duration-300 data-ending-style:translate-y-4 data-ending-style:opacity-0 data-starting-style:translate-y-4 data-starting-style:opacity-0",
          isError ? "border-red-400/40" : "border-emerald-400/40",
        )}
      >
        <Toast.Content className="flex items-start gap-3 p-4">
          {isError ? (
            <CircleAlertIcon className="mt-0.5 size-5 shrink-0 text-red-400" />
          ) : (
            <CheckCircle2Icon className="mt-0.5 size-5 shrink-0 text-emerald-400" />
          )}
          <div className="min-w-0 flex-1">
            <Toast.Title className="text-sm font-semibold" />
            <Toast.Description className="mt-1 text-sm text-[#b7b9c6]" />
          </div>
          <Toast.Close
            aria-label="Закрыть уведомление"
            className="rounded-md p-1 text-[#9d9faf] transition-colors hover:bg-white/5 hover:text-white focus-visible:outline-2 focus-visible:outline-[#6277ef]"
          >
            <XIcon className="size-4" />
          </Toast.Close>
        </Toast.Content>
      </Toast.Root>
    );
  });
}

export function Toaster() {
  return (
    <Toast.Provider toastManager={toastManager} timeout={4500} limit={3}>
      <Toast.Portal>
        <Toast.Viewport className="fixed bottom-4 right-4 z-50 flex w-[calc(100vw-2rem)] max-w-sm flex-col-reverse gap-3 sm:bottom-6 sm:right-6">
          <ToastList />
        </Toast.Viewport>
      </Toast.Portal>
    </Toast.Provider>
  );
}
