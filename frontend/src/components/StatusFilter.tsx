import React from 'react';
import { STRINGS } from '../constants/strings';
import { StatusFilterOption, TaskSummary } from '../types/task';

interface StatusFilterProps {
  activeFilter: StatusFilterOption;
  onSelect: (filter: StatusFilterOption) => void;
  summary: TaskSummary;
  onClearCompleted: () => void;
  isClearing: boolean;
}

export const StatusFilter: React.FC<StatusFilterProps> = ({
  activeFilter,
  onSelect,
  summary,
  onClearCompleted,
  isClearing,
}) => {
  const filters: Array<{ id: StatusFilterOption; label: string; count: number }> = [
    { id: 'all', label: STRINGS.filter.all, count: summary.total },
    { id: 'pending', label: STRINGS.filter.pending, count: summary.pending },
    { id: 'inProgress', label: STRINGS.filter.inProgress, count: summary.inProgress },
    { id: 'completed', label: STRINGS.filter.completed, count: summary.completed },
  ];

  return (
    <div className="filters-row">
      <div className="filter-group" role="tablist" aria-label="Filter tasks by status">
        {filters.map((f) => (
          <button
            key={f.id}
            type="button"
            role="tab"
            aria-selected={activeFilter === f.id}
            className={`filter-pill ${activeFilter === f.id ? 'active' : ''}`}
            onClick={() => onSelect(f.id)}
          >
            <span>{f.label}</span>
            <span className="filter-counter">{f.count}</span>
          </button>
        ))}
      </div>

      {summary.completed > 0 && (
        <button
          type="button"
          className="btn-clear-completed"
          onClick={onClearCompleted}
          disabled={isClearing}
          aria-label={STRINGS.actions.clearCompleted}
        >
          {isClearing ? STRINGS.actions.clearingCompleted : STRINGS.actions.clearCompleted}
        </button>
      )}
    </div>
  );
};
