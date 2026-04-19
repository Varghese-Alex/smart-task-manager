import TaskItem from "./TaskItem.jsx";

function TaskList({ tasks, onUpdateTask, onDeleteTask }) {
  if (tasks.length === 0) {
    return <p className="panel">No tasks yet. Add your first one above.</p>;
  }

  return (
    <section className="task-list">
      {tasks.map((task) => (
        <TaskItem
          key={task.id}
          task={task}
          onUpdateTask={onUpdateTask}
          onDeleteTask={onDeleteTask}
        />
      ))}
    </section>
  );
}

export default TaskList;

