import { getToken } from "../auth/token.js";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

function buildUrl(path) {
  if (!API_BASE_URL) {
    throw new Error("VITE_API_BASE_URL is not configured.");
  }

  const sanitizedBaseUrl = API_BASE_URL.endsWith("/")
    ? API_BASE_URL.slice(0, -1)
    : API_BASE_URL;

  return `${sanitizedBaseUrl}${path}`;
}

async function request(path, options = {}) {
  const { method = "GET", body, auth = true } = options;
  const headers = {
    "Content-Type": "application/json",
  };

  if (auth) {
    const token = getToken();
    if (token) {
      headers.Authorization = `Bearer ${token}`;
    }
  }

  const response = await fetch(buildUrl(path), {
    method,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  });

  const contentType = response.headers.get("content-type") ?? "";
  const payload = contentType.includes("application/json")
    ? await response.json()
    : null;

  if (!response.ok) {
    const message =
      payload?.message ?? `Request failed with status ${response.status}.`;
    const error = new Error(message);
    error.status = response.status;
    throw error;
  }

  return payload;
}

export function apiGet(path, options = {}) {
  return request(path, { ...options, method: "GET" });
}

export function apiPost(path, body, options = {}) {
  return request(path, { ...options, method: "POST", body });
}

export function apiPut(path, body, options = {}) {
  return request(path, { ...options, method: "PUT", body });
}

export function apiDelete(path, options = {}) {
  return request(path, { ...options, method: "DELETE" });
}

