import { createBrowserRouter, Navigate } from "react-router-dom";
import { getToken } from "./auth/token.js";
import LoginPage from "./pages/LoginPage.jsx";
import RegisterPage from "./pages/RegisterPage.jsx";
import TasksPage from "./pages/TasksPage.jsx";
import ProtectedRoute from "./components/ProtectedRoute.jsx";

function HomeRedirect() {
  return getToken() ? (
    <Navigate to="/tasks" replace />
  ) : (
    <Navigate to="/login" replace />
  );
}

export const appRouter = createBrowserRouter([
  {
    path: "/",
    element: <HomeRedirect />,
  },
  {
    path: "/login",
    element: <LoginPage />,
  },
  {
    path: "/register",
    element: <RegisterPage />,
  },
  {
    path: "/tasks",
    element: (
      <ProtectedRoute>
        <TasksPage />
      </ProtectedRoute>
    ),
  },
  {
    path: "*",
    element: <Navigate to="/" replace />,
  },
]);

