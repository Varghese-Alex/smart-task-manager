import { useState } from "react";

function TaskForm({ onCreate }) {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [dueDate, setDueDate] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event) {
    event.preventDefault();
    setError("");
    setIsSubmitting(true);

    try {
      await onCreate({
        title: title.trim(),
        description: description.trim() || null,
        dueDate: dueDate || null,
      });

      setTitle("");
      setDescription("");
      setDueDate("");
    } catch (requestError) {
      setError(requestError.message);
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="panel task-form-panel">
      <h2>Add task</h2>

      <form className="form-grid" onSubmit={handleSubmit}>
        <label className="field-label" htmlFor="new-title">
          Title
        </label>
        <input
          id="new-title"
          type="text"
          required
          maxLength={200}
          value={title}
          onChange={(event) => setTitle(event.target.value)}
        />

        <label className="field-label" htmlFor="new-description">
          Description
        </label>
        <textarea
          id="new-description"
          rows={3}
          value={description}
          onChange={(event) => setDescription(event.target.value)}
        />

        <label className="field-label" htmlFor="new-due-date">
          Due date
        </label>
        <input
          id="new-due-date"
          type="date"
          value={dueDate}
          onChange={(event) => setDueDate(event.target.value)}
        />

        {error ? <p className="error-text">{error}</p> : null}

        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Adding..." : "Add task"}
        </button>
      </form>
    </section>
  );
}

export default TaskForm;
