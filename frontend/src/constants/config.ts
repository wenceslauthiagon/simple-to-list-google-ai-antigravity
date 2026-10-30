export const API_BASE_URL =
  (import.meta.env.VITE_API_URL as string | undefined) || 'http://localhost:5000';

export const SEARCH_DEBOUNCE_MS = 300;

export const MAX_TITLE_LENGTH = 200;
export const MIN_TITLE_LENGTH = 1;
