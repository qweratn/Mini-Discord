export function formatMessageTime(value: string | null): string {
  if (!value) {
    return "";
  }

  return new Intl.DateTimeFormat("ru-RU", {
    hour: "2-digit",
    minute: "2-digit",
  }).format(new Date(value));
}

export function formatMessageDate(value: string): string {
  return new Intl.DateTimeFormat("ru-RU", {
    day: "numeric",
    month: "long",
    year: "numeric",
  }).format(new Date(value));
}

export function isSameMessageDay(first: string, second: string): boolean {
  return new Date(first).toDateString() === new Date(second).toDateString();
}

export function getInitials(name: string): string {
  return name.trim().slice(0, 2).toUpperCase();
}

export function getRequestErrorMessage(error: unknown): string {
  return error instanceof Error
    ? error.message
    : "Произошла неизвестная ошибка";
}
