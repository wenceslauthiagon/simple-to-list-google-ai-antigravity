import React from 'react';
import { STRINGS } from '../constants/strings';
import { TaskSummary } from '../types/task';
import { clampPercentage, formatProgress } from '../utils/formatters';

interface ProgressBarProps {
  summary: TaskSummary;
}

export const ProgressBar: React.FC<ProgressBarProps> = ({ summary }) => {
  const safePercentage = clampPercentage(summary.progressPercentage);
  const formattedText = formatProgress(
    summary.completed,
    summary.total,
    summary.progressPercentage
  );

  return (
    <section className="card progress-card" aria-label={STRINGS.progress.label}>
      <div className="progress-header">
        <span className="progress-label">{STRINGS.progress.label}</span>
        <span className="progress-count">{formattedText}</span>
      </div>
      <div
        className="progress-track"
        role="progressbar"
        aria-valuenow={safePercentage}
        aria-valuemin={0}
        aria-valuemax={100}
        aria-label={STRINGS.progress.label}
      >
        <div className="progress-fill" style={{ width: `${safePercentage}%` }} />
      </div>
    </section>
  );
};
