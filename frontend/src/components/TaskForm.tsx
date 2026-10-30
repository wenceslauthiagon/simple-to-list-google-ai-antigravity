import React, { useState } from 'react';
import { MAX_TITLE_LENGTH } from '../constants/config';
import { STRINGS } from '../constants/strings';
import { CreateTaskInput, TaskPriority } from '../types/task';
import { PrioritySelect } from './PrioritySelect';

interface TaskFormProps {
  onCreate: (input: CreateTaskInput) => Promise<boolean>;
  isSubmitting: boolean;
}

export const TaskForm: React.FC<TaskFormProps> = ({ onCreate, isSubmitting }) => {
  const [title, setTitle] = useState('');
  const [priority, setPriority] = useState<TaskPriority>('medium');
  const [validationError, setValidationError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const trimmed = title.trim();
    if (!trimmed) {
      setValidationError(STRINGS.form.validationEmpty);
      return;
    }

    if (trimmed.length > MAX_TITLE_LENGTH) {
      setValidationError(STRINGS.form.validationTooLong);
      return;
    }

    setValidationError(null);
    const success = await onCreate({ title: trimmed, priority });
    if (success) {
      setTitle('');
      setPriority('medium');
    }
  };

  return (
    <section className="card" aria-label="Create task">
      <form onSubmit={handleSubmit} className="task-form">
        <input
          type="text"
          className="task-form-input"
          placeholder={STRINGS.form.titlePlaceholder}
          value={title}
          onChange={(e) => {
            setTitle(e.target.value);
            if (validationError) setValidationError(null);
          }}
          disabled={isSubmitting}
          maxLength={MAX_TITLE_LENGTH}
          aria-label={STRINGS.form.titlePlaceholder}
          required
        />

        <PrioritySelect
          value={priority}
          onChange={setPriority}
          disabled={isSubmitting}
        />

        <button
          type="submit"
          className="btn-primary"
          disabled={isSubmitting || !title.trim()}
        >
          {isSubmitting ? STRINGS.form.addingButton : STRINGS.form.addButton}
        </button>
      </form>

      {validationError && (
        <div style={{ color: 'var(--color-danger)', fontSize: '0.85rem', marginTop: '0.5rem' }}>
          {validationError}
        </div>
      )}
    </section>
  );
};
