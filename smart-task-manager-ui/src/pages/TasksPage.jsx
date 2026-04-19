import { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { apiDelete, apiGet, apiPost, apiPut } from "../api/client.js";
import { clearToken } from "../auth/token.js";
import TaskForm from "../components/TaskForm.jsx";
import TaskList from "../components/TaskList.jsx";

const PAGE_SIZE_OPTIONS = [5, 10, 20, 50];

function TasksPage() {
  const navigate = useNavigate();
  const [tasks, setTasks] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalCount, setTotalCount] = useState(0);

  const loadTasks = useCallback(async () => {
    setError("");
    setIsLoading(true);

    try {
      const response = await apiGet(`/api/tasks?page=${page}&pageSize=${pageSize}`);
      setTasks(response?.data ?? []);
      setTotalCount(response?.totalCount ?? 0);
    } catch (requestError) {
      if (requestError.status === 401) {
        clearToken();
        navigate("/login", { replace: true });
        return;
      }
      setError(requestError.message);
    } finally {
      setIsLoading(false);
    }
  }, [navigate, page, pageSize]);

  useEffect(() => {
    void loadTasks();
  }, [loadTasks]);

  async function handleCreateTask(payload) {
    setError("");
    await apiPost("/api/tasks", payload);
    await loadTasks();
  }

  async function handleUpdateTask(taskId, payload) {
    setError("");
    await apiPut(`/api/tasks/${taskId}`, payload);
    await loadTasks();
  }

  async function handleDeleteTask(taskId) {
    setError("");
    await apiDelete(`/api/tasks/${taskId}`);

    // If the current page becomes empty after delete, move one page back.
    if (tasks.length === 1 && page > 1) {
      setPage((previousPage) => previousPage - 1);
      return;
    }

    await loadTasks();
  }

  function handleLogout() {
    clearToken();
    navigate("/login", { replace: true });
  }

  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
  const canGoPrevious = page > 1 && !isLoading;
  const canGoNext = page < totalPages && !isLoading;

  return (
    <main className="tasks-page">
      <header className="tasks-header">
        <div>
          <h1>Tasks</h1>
          <p className="subtitle">Plan, track, and close work quickly.</p>
        </div>
        <button className="ghost-button" type="button" onClick={handleLogout}>
          Logout
        </button>
      </header>

      <section className="tasks-layout">
        <aside className="tasks-sidebar">
          <TaskForm onCreate={handleCreateTask} />
        </aside>

        <section className="panel task-list-panel">
          <div className="panel-heading">
            <div>
              <h2>Task list</h2>
              <p className="panel-subtitle">{totalCount} total tasks</p>
            </div>
          </div>

          {error ? <p className="error-text">{error}</p> : null}
          {isLoading ? <p className="empty-state">Loading tasks...</p> : null}

          {!isLoading ? (
            <TaskList
              tasks={tasks}
              onUpdateTask={handleUpdateTask}
              onDeleteTask={handleDeleteTask}
            />
          ) : null}

          <div className="pagination-controls">
            <div className="page-meta">
              <span>
                Page {page} of {totalPages}
              </span>
            </div>

            <div className="page-actions">
              <label className="field-label" htmlFor="page-size">
                Page size
              </label>
              <select
                id="page-size"
                value={pageSize}
                onChange={(event) => {
                  setPageSize(Number(event.target.value));
                  setPage(1);
                }}
              >
                {PAGE_SIZE_OPTIONS.map((sizeOption) => (
                  <option key={sizeOption} value={sizeOption}>
                    {sizeOption}
                  </option>
                ))}
              </select>

              <button
                className="ghost-button"
                type="button"
                onClick={() => setPage((previousPage) => previousPage - 1)}
                disabled={!canGoPrevious}
              >
                Previous
              </button>
              <button
                type="button"
                onClick={() => setPage((previousPage) => previousPage + 1)}
                disabled={!canGoNext}
              >
                Next
              </button>
            </div>
          </div>
        </section>
      </section>
    </main>
  );
}

export default TasksPage;
