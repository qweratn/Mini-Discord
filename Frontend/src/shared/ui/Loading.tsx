import { Spinner } from "@/components/ui/spinner";

export default function Loading() {
  return (
    <div
      className="fixed inset-0 z-250 flex items-center justify-center bg-black"
      style={{ backgroundColor: "rgba(0,0,0,0.4)" }}
    >
      <div className="h-15 w-15 bg-white flex items-center justify-center rounded-lg border-none">
        <Spinner className="size-8" />
      </div>
    </div>
  );
}
