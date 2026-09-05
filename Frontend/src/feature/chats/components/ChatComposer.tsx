import { SendIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";

type ChatComposerProps = {
  chatName: string;
};

export function ChatComposer({ chatName }: ChatComposerProps) {
  return (
    <div className="border-t border-[#202844] px-4 py-4 sm:px-6 lg:px-8">
      <Card className="mx-auto max-w-4xl border-[#354064] bg-[#121a31] py-0 text-white shadow-lg shadow-black/10 2xl:max-w-6xl">
        <CardContent className="flex items-center gap-3 p-2 pl-4">
          <Input
            aria-label="Сообщение"
            placeholder={`Написать в ${chatName}`}
            readOnly
            className="h-11 border-0 bg-transparent px-0 text-base shadow-none placeholder:text-[#798099] focus-visible:border-0 focus-visible:ring-0"
          />
          <Button
            type="button"
            size="icon-lg"
            aria-label="Отправить сообщение"
            className="size-11 rounded-full bg-[#5f6ff1] text-white hover:bg-[#7180f8]"
          >
            <SendIcon className="size-5" />
          </Button>
        </CardContent>
      </Card>
    </div>
  );
}
