import {
  Fragment,
  useCallback,
  useEffect,
  useLayoutEffect,
  useRef,
  useState,
} from "react";

import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { Spinner } from "@/components/ui/spinner";
import {
  formatMessageDate,
  getRequestErrorMessage,
  isSameMessageDay,
} from "@/feature/chats/chat-formatters";
import { getChatMessages } from "@/feature/chats/chats-api";
import type { ChatMessage } from "@/feature/chats/chat-types";

import { MessageBubble } from "./MessageBubble";

type ChatMessagesProps = {
  chatId: string;
  currentUsername: string;
};

type PendingScrollAdjustment = {
  scrollHeight: number;
  scrollTop: number;
};

export function ChatMessages({
  chatId,
  currentUsername,
}: ChatMessagesProps) {
  const scrollContainerRef = useRef<HTMLDivElement>(null);
  const loadMoreTriggerRef = useRef<HTMLDivElement>(null);
  const paginationControllerRef = useRef<AbortController | null>(null);
  const paginationLockRef = useRef(false);
  const initialScrollCompletedRef = useRef(false);
  const pendingScrollAdjustmentRef =
    useRef<PendingScrollAdjustment | null>(null);

  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [nextBeforeMessageId, setNextBeforeMessageId] = useState<
    string | null
  >(null);
  const [hasMore, setHasMore] = useState(false);
  const [isInitialLoading, setIsInitialLoading] = useState(true);
  const [isLoadingMore, setIsLoadingMore] = useState(false);
  const [initialError, setInitialError] = useState<string | null>(null);
  const [paginationError, setPaginationError] = useState<string | null>(null);
  const [reloadKey, setReloadKey] = useState(0);

  useEffect(() => {
    const controller = new AbortController();

    async function loadInitialMessages() {
      try {
        setIsInitialLoading(true);
        setInitialError(null);
        setPaginationError(null);
        initialScrollCompletedRef.current = false;

        const page = await getChatMessages(
          chatId,
          undefined,
          controller.signal,
        );

        setMessages([...page.items].reverse());
        setNextBeforeMessageId(page.nextBeforeMessageId);
        setHasMore(page.hasMore);
      } catch (requestError: unknown) {
        if (!controller.signal.aborted) {
          setInitialError(getRequestErrorMessage(requestError));
        }
      } finally {
        if (!controller.signal.aborted) {
          setIsInitialLoading(false);
        }
      }
    }

    void loadInitialMessages();

    return () => controller.abort();
  }, [chatId, reloadKey]);

  useEffect(() => {
    return () => paginationControllerRef.current?.abort();
  }, []);

  useLayoutEffect(() => {
    const container = scrollContainerRef.current;

    if (!container) {
      return;
    }

    const pendingAdjustment = pendingScrollAdjustmentRef.current;

    if (pendingAdjustment) {
      container.scrollTop =
        container.scrollHeight -
        pendingAdjustment.scrollHeight +
        pendingAdjustment.scrollTop;
      pendingScrollAdjustmentRef.current = null;
      return;
    }

    if (
      !isInitialLoading &&
      !initialError &&
      !initialScrollCompletedRef.current
    ) {
      container.scrollTop = container.scrollHeight;
      initialScrollCompletedRef.current = true;
    }
  }, [initialError, isInitialLoading, messages]);

  const loadOlderMessages = useCallback(async () => {
    if (
      !hasMore ||
      !nextBeforeMessageId ||
      paginationLockRef.current ||
      !initialScrollCompletedRef.current
    ) {
      return;
    }

    const container = scrollContainerRef.current;
    const controller = new AbortController();

    paginationControllerRef.current = controller;
    paginationLockRef.current = true;
    setIsLoadingMore(true);
    setPaginationError(null);

    if (container) {
      pendingScrollAdjustmentRef.current = {
        scrollHeight: container.scrollHeight,
        scrollTop: container.scrollTop,
      };
    }

    try {
      const page = await getChatMessages(
        chatId,
        nextBeforeMessageId,
        controller.signal,
      );
      const olderMessages = [...page.items].reverse();

      setMessages((currentMessages) => [
        ...olderMessages,
        ...currentMessages,
      ]);
      setNextBeforeMessageId(page.nextBeforeMessageId);
      setHasMore(page.hasMore);
    } catch (requestError: unknown) {
      pendingScrollAdjustmentRef.current = null;

      if (!controller.signal.aborted) {
        setPaginationError(getRequestErrorMessage(requestError));
      }
    } finally {
      if (!controller.signal.aborted) {
        setIsLoadingMore(false);
      }

      paginationLockRef.current = false;
    }
  }, [chatId, hasMore, nextBeforeMessageId]);

  useEffect(() => {
    const container = scrollContainerRef.current;
    const trigger = loadMoreTriggerRef.current;

    if (!container || !trigger || isInitialLoading || initialError) {
      return;
    }

    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting) {
          void loadOlderMessages();
        }
      },
      {
        root: container,
        rootMargin: "200px 0px 0px",
      },
    );

    observer.observe(trigger);

    return () => observer.disconnect();
  }, [initialError, isInitialLoading, loadOlderMessages]);

  return (
    <div
      ref={scrollContainerRef}
      className="min-h-0 flex-1 overflow-y-auto px-4 py-6 sm:px-7 lg:px-10"
    >
      <div className="mx-auto w-full max-w-3xl">
        <div ref={loadMoreTriggerRef} className="h-px" aria-hidden="true" />

        {isLoadingMore && (
          <div className="flex items-center justify-center gap-2 py-4 text-sm text-[#8f96ae]">
            <Spinner />
            Загружаем предыдущие сообщения...
          </div>
        )}

        {!isLoadingMore && paginationError && (
          <div className="py-4 text-center">
            <p className="text-sm text-red-300">{paginationError}</p>
            <Button
              type="button"
              size="sm"
              variant="outline"
              className="mt-3 border-[#44518c] bg-transparent text-[#a8b1ff]"
              onClick={() => void loadOlderMessages()}
            >
              Повторить загрузку
            </Button>
          </div>
        )}

        {isInitialLoading && (
          <div className="flex items-center justify-center gap-2 py-16 text-sm text-[#8f96ae]">
            <Spinner />
            Загружаем сообщения...
          </div>
        )}

        {!isInitialLoading && initialError && (
          <div className="py-16 text-center">
            <p className="text-sm text-red-300">{initialError}</p>
            <Button
              type="button"
              size="sm"
              variant="outline"
              className="mt-4 border-[#44518c] bg-transparent text-[#a8b1ff]"
              onClick={() => setReloadKey((current) => current + 1)}
            >
              Повторить
            </Button>
          </div>
        )}

        {!isInitialLoading && !initialError && messages.length === 0 && (
          <p className="py-16 text-center text-sm text-[#8f96ae]">
            В этом чате пока нет сообщений
          </p>
        )}

        {!isInitialLoading && !initialError && messages.length > 0 && (
          <div className="space-y-6 pb-2">
            {messages.map((message, index) => {
              const previousMessage = messages[index - 1];
              const startsNewDay =
                !previousMessage ||
                !isSameMessageDay(previousMessage.sentAt, message.sentAt);

              return (
                <Fragment key={message.id}>
                  {startsNewDay && (
                    <div className="flex items-center gap-4 pt-2">
                      <Separator className="flex-1 bg-[#28304c]" />
                      <time
                        dateTime={message.sentAt}
                        className="text-xs font-medium text-[#7f859c]"
                      >
                        {formatMessageDate(message.sentAt)}
                      </time>
                      <Separator className="flex-1 bg-[#28304c]" />
                    </div>
                  )}

                  <MessageBubble
                    message={message}
                    animationDelay={Math.min(index, 10) * 25}
                    isCurrentUser={
                      message.author.username === currentUsername
                    }
                  />
                </Fragment>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
}
