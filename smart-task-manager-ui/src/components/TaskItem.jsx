import { useState } from "react";

function toDateInputValue(dateTimeValue) {
  if (!dateTimeValue) {
    return "";
  }

  const date = new Date(dateTimeValue);
  if (Number.isNaN(date.getTime())) {
    return "";
  }

  return date.toISOString().slice(0, 10);
}

function formatDueDate(dateTimeValue) {
  if (!dateTimeValue) {
    return "No due date";
  }

  const date = new Date(dateTimeValue);
  if (Number.isNaN(date.getTime())) {
    return "No due date";
  }

  return date.toLocaleDateString();
}

function isCompleted(status) {
  return status === 1 || status === "Completed";
}

function TaskItem({ task, onUpdateTask, onDeleteTask }) {
  const [isEditing, setIsEditing] = useState(false);
  const [title, setTitle] = useState(task.title);
  const [description, setDescription] = useState(task.description ?? "");
  const [dueDate, setDueDate] = useState(toDateInputValue(task.dueDate));
  const [error, setError] = useState("");
  const [isSaving, setIsSaving] = useState(false);

  async function handleMarkCompleted() {
    setError("");
    setIsSaving(true);

    try {
      await onUpdateTask(task.id, {
        title: task.title,
        description: task.description,
        dueDate: task.dueDate ?? null,
        status: 1,
      });
    } catch (requestError) {
      setError(requestError.message);
    } finally {
      setIsSaving(false);
    }
  }

  async function handleSaveChanges(event) {
    event.preventDefault();
    setError("");
    setIsSaving(true);

    try {
      await onUpdateTask(task.id, {
        title: title.trim(),
        description: description.trim() || null,
        dueDate: dueDate || null,
        status: isCompleted(task.status) ? 1 : 0,
      });
      setIsEditing(false);
    } catch (requestError) {
      setError(requestError.message);
    } finally {
      setIsSaving(false);
    }
  }

  async function handleDelete() {
    const confirmed = window.confirm("Delete this task?");
    if (!confirmed) {
      return;
    }

    setError("");
    setIsSaving(true);

    try {
      await onDeleteTask(task.id);
    } catch (requestError) {
      setError(requestError.message);
    } finally {
      setIsSaving(false);
    }
  }

  if (isEditing) {
    return (
      <article className="task-item">
        <form className="form-grid compact" onSubmit={handleSaveChanges}>
          <label className="field-label" htmlFor={`edit-title-${task.id}`}>
            Title
          </label>
          <input
            id={`edit-title-${task.id}`}
            type="text"
            required
            maxLength={200}
            value={title}
            onChange={(event) => setTitle(event.target.value)}
          />

          <label
            className="field-label"
            htmlFor={`edit-description-${task.id}`}
          >
            Description
          </label>
          <textarea
            id={`edit-description-${task.id}`}
            rows={2}
            value={description}
            onChange={(event) => setDescription(event.target.value)}
          />

          <label className="field-label" htmlFor={`edit-due-date-${task.id}`}>
            Due date
          </label>
          <input
            id={`edit-due-date-${task.id}`}
            type="date"
            value={dueDate}
            onChange={(event) => setDueDate(event.target.value)}
          />

          {error ? <p className="error-text">{error}</p> : null}

          <div className="button-row">
            <button type="submit" disabled={isSaving}>
              {isSaving ? "Saving..." : "Save"}
            </button>
            <button
              className="ghost-button"
              type="button"
              onClick={() => setIsEditing(false)}
              disabled={isSaving}
            >
              Cancel
            </button>
          </div>
        </form>
      </article>
    );
  }

  return (
    <article className="task-item">
      <div className="task-row">
        <h3 className={isCompleted(task.status) ? "task-title done" : "task-title"}>
          {task.title}
        </h3>
        <span className={isCompleted(task.status) ? "status done" : "status"}>
          {isCompleted(task.status) ? "Completed" : "Pending"}
        </span>
      </div>

      <p className="task-description">
        {task.description || "No description provided."}
      </p>
      <p className="task-date">Due: {formatDueDate(task.dueDate)}</p>

      {error ? <p className="error-text">{error}</p> : null}

      <div className="button-row">
        {!isCompleted(task.status) ? (
          <button type="button" onClick={handleMarkCompleted} disabled={isSaving}>
            Complete
          </button>
        ) : null}
        <button
          className="ghost-button"
          type="button"
          onClick={() => setIsEditing(true)}
          disabled={isSaving}
        >
          Edit
        </button>
        <button
          className="danger-button"
          type="button"
          onClick={handleDelete}
          disabled={isSaving}
        >
          Delete
        </button>
      </div>
    </article>
  );
}

export default TaskItem;
