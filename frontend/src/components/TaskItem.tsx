import React, { useState } from 'react';
import { MAX_TITLE_LENGTH } from '../constants/config';
import { STRINGS } from '../constants/strings';
import { Task, TaskPriority, TaskStatus, UpdateTaskInput } from '../types/task';
import { PrioritySelect } from './PrioritySelect';
import { StatusSelect } from './StatusSelect';

interface TaskItemProps {
  task: Task;
  onAdvanceStatus: (id: string) => void;
  onUpdate: (id: string, input: UpdateTaskInput) => void;
  onDelete: (id: string) => void;
}

export const TaskItem: React.FC<TaskItemProps> = ({
  task,
  onAdvanceStatus,
  onUpdate,
  onDelete,
}) => {
  const [isEditing, setIsEditing] = useState(false);
  const [editTitle, setEditTitle] = useState(task.title);
  const [editPriority, setEditPriority] = useState<TaskPriority>(task.priority);

  const isCompleted = task.status === 'completed';

  const handleSave = () => {
    const trimmed = editTitle.trim();
    if (!trimmed) return;

    onUpdate(task.id, {
      title: trimmed,
      priority: editPriority,
    });
    setIsEditing(false);
  };

  const handleCancel = () => {
    setEditTitle(task.title);
    setEditPriority(task.priority);
    setIsEditing(false);
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleSave();
    } else if (e.key === 'Escape') {
      handleCancel();
    }
  };

  const statusLabel = STRINGS.statuses[task.status];
  const advanceAria = STRINGS.actions.advanceStatusAria.replace('{status}', statusLabel);

  return (
    <article
      className={`task-item ${task.status.toLowerCase()} ${isCompleted ? 'completed' : ''}`}
      aria-label={`Task: ${task.title}`}
    >
      <button
        type="button"
        className={`status-cycle-btn ${task.status.toLowerCase()}`}
        onClick={() => onAdvanceStatus(task.id)}
        aria-label={advanceAria}
        title={advanceAria}
      >
        {isCompleted && (
          <svg
            width="14"
            height="14"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="3"
            strokeLinecap="round"
            strokeLinejoin="round"
            aria-hidden="true"
          >
            <polyline points="20 6 9 17 4 12" />
          </svg>
        )}
      </button>

      <div className="task-content">
        {isEditing ? (
          <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
            <input
              type="text"
              className="task-edit-input"
              value={editTitle}
              onChange={(e) => setEditTitle(e.target.value)}
              onKeyDown={handleKeyDown}
              maxLength={MAX_TITLE_LENGTH}
              autoFocus
              aria-label="Edit task title"
            />
            <PrioritySelect value={editPriority} onChange={setEditPriority} />
            <button
              type="button"
              className="btn-primary"
              style={{ padding: '0.4rem 0.8rem', fontSize: '0.85rem' }}
              onClick={handleSave}
            >
              {STRINGS.actions.saveEdit}
            </button>
            <button
              type="button"
              className="btn-clear-completed"
              style={{ color: 'var(--color-text-muted)' }}
              onClick={handleCancel}
            >
              {STRINGS.actions.cancelEdit}
            </button>
          </div>
        ) : (
          <>
            <span className="task-title">{task.title}</span>
            <div className="task-meta">
              <span className={`badge badge-${task.priority}`} aria-label={`Priority: ${task.priority}`}>
                <span className="badge-dot" aria-hidden="true" />
                <span>{STRINGS.priorities[task.priority]}</span>
              </span>

              <StatusSelect
                value={task.status}
                onChange={(newStatus: TaskStatus) => onUpdate(task.id, { status: newStatus })}
                ariaLabel={`Status for ${task.title}`}
              />
            </div>
          </>
        )}
      </div>

      {!isEditing && (
        <div className="task-actions">
          <button
            type="button"
            className="btn-icon"
            onClick={() => setIsEditing(true)}
            aria-label={`${STRINGS.actions.editTask}: ${task.title}`}
            title={STRINGS.actions.editTask}
          >
            <svg
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
              aria-hidden="true"
            >
              <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
              <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
            </svg>
          </button>

          <button
            type="button"
            className="btn-icon btn-icon-danger"
            onClick={() => onDelete(task.id)}
            aria-label={`${STRINGS.actions.deleteTask}: ${task.title}`}
            title={STRINGS.actions.deleteTask}
          >
            <svg
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
              aria-hidden="true"
            >
              <polyline points="3 6 5 6 21 6" />
              <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
              <line x1="10" y1="11" x2="10" y2="17" />
              <line x1="14" y1="11" x2="14" y2="17" />
            </svg>
          </button>
        </div>
      )}
    </article>
  );
};
