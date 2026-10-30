import React from 'react';
import { STRINGS } from '../constants/strings';
import { TaskStatus } from '../types/task';

interface StatusSelectProps {
  id?: string;
  value: TaskStatus;
  onChange: (value: TaskStatus) => void;
  disabled?: boolean;
  ariaLabel?: string;
}

export const StatusSelect: React.FC<StatusSelectProps> = ({
  id,
  value,
  onChange,
  disabled = false,
  ariaLabel = STRINGS.actions.statusSelectAria,
}) => {
  return (
    <div className="select-wrapper">
      <select
        id={id}
        className="select-input"
        value={value}
        onChange={(e) => onChange(e.target.value as TaskStatus)}
        disabled={disabled}
        aria-label={ariaLabel}
      >
        <option value="pending">{STRINGS.statuses.pending}</option>
        <option value="inProgress">{STRINGS.statuses.inProgress}</option>
        <option value="completed">{STRINGS.statuses.completed}</option>
      </select>
    </div>
  );
};
