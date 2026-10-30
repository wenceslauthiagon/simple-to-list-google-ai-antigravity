import React from 'react';
import { STRINGS } from '../constants/strings';
import { TaskPriority } from '../types/task';

interface PrioritySelectProps {
  id?: string;
  value: TaskPriority;
  onChange: (value: TaskPriority) => void;
  disabled?: boolean;
  ariaLabel?: string;
}

export const PrioritySelect: React.FC<PrioritySelectProps> = ({
  id,
  value,
  onChange,
  disabled = false,
  ariaLabel = STRINGS.actions.prioritySelectAria,
}) => {
  return (
    <div className="select-wrapper">
      <select
        id={id}
        className="select-input"
        value={value}
        onChange={(e) => onChange(e.target.value as TaskPriority)}
        disabled={disabled}
        aria-label={ariaLabel}
      >
        <option value="low">{STRINGS.priorities.low}</option>
        <option value="medium">{STRINGS.priorities.medium}</option>
        <option value="high">{STRINGS.priorities.high}</option>
      </select>
    </div>
  );
};
