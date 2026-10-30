import React from 'react';
import { STRINGS } from '../constants/strings';
import { Task, UpdateTaskInput } from '../types/task';
import { TaskItem } from './TaskItem';

interface TaskListProps {
  tasks: Task[];
  isLoading: boolean;
  error: string | null;
  onAdvanceStatus: (id: string) => void;
  onUpdate: (id: string, input: UpdateTaskInput) => void;
  onDelete: (id: string) => void;
  onRetry: () => void;
}

export const TaskList: React.FC<TaskListProps> = ({
  tasks,
  isLoading,
  error,
  onAdvanceStatus,
  onUpdate,
  onDelete,
  onRetry,
}) => {
  if (isLoading && tasks.length === 0) {
    return (
      <div className="state-box" role="status" aria-live="polite">
        <p>{STRINGS.states.loading}</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="state-error" role="alert">
        <span>
          {STRINGS.states.errorPrefix}
          {error}
        </span>
        <button type="button" className="btn-retry" onClick={onRetry}>
          {STRINGS.states.retry}
        </button>
      </div>
    );
  }

  if (tasks.length === 0) {
    return (
      <div className="state-box" role="status">
        <p>{STRINGS.states.emptyList}</p>
      </div>
    );
  }

  return (
    <section className="task-list" aria-label="Task list">
      {tasks.map((task) => (
        <TaskItem
          key={task.id}
          task={task}
          onAdvanceStatus={onAdvanceStatus}
          onUpdate={onUpdate}
          onDelete={onDelete}
        />
      ))}
    </section>
  );
};
