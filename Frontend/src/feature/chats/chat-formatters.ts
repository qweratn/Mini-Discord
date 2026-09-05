export function formatMessageTime(value: string | null): string {
  if (!value) {
    return "";
  }

  return new Intl.DateTimeFormat("ru-RU", {
    hour: "2-digit",
    minute: "2-digit",
  }).format(new Date(value));
}

export function getInitials(name: string): string {
  return name.trim().slice(0, 2).toUpperCase();
}

export function getRequestErrorMessage(error: unknown): string {
  return error instanceof Error
    ? error.message
    : "Произошла неизвестная ошибка";
}
