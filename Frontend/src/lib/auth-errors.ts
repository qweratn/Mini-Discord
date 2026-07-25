const errorMessages: Record<string, string> = {
  form_identifier_not_found: "Аккаунт с такой почтой не найден",
  form_password_incorrect: "Неверный пароль",
  form_password_length_too_short: "Пароль слишком короткий",
  form_password_not_strong_enough: "Используйте более надёжный пароль",
  form_password_pwned: "Этот пароль найден в утечках. Выберите другой",
  form_identifier_exists: "Аккаунт с такой почтой уже существует",
  form_identifier_taken: "Эта почта уже используется",
  form_username_invalid_character: "Имя содержит недопустимые символы",
  form_username_invalid_length: "Некорректная длина имени пользователя",
  form_username_taken: "Это имя пользователя уже занято",
  verification_failed: "Не удалось подтвердить данные",
  form_code_incorrect: "Неверный код подтверждения",
  too_many_requests: "Слишком много попыток. Попробуйте немного позже",
};

type AuthError = {
  code?: string;
  longMessage?: string;
  message?: string;
};

export function getAuthErrorMessage(
  error: unknown,
  fallback = "Что-то пошло не так. Попробуйте ещё раз",
) {
  if (!error || typeof error !== "object") return fallback;

  const authError = error as AuthError;

  if (authError.code && errorMessages[authError.code]) {
    return errorMessages[authError.code];
  }

  return authError.longMessage || authError.message || fallback;
}
