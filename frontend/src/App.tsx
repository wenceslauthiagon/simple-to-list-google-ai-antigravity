import React from 'react';
import { ProgressBar } from './components/ProgressBar';
import { SearchInput } from './components/SearchInput';
import { StatusFilter } from './components/StatusFilter';
import { TaskForm } from './components/TaskForm';
import { TaskList } from './components/TaskList';
import { STRINGS } from './constants/strings';
import { useTasks } from './hooks/useTasks';

export const App: React.FC = () => {
  const {
    tasks,
    summary,
    statusFilter,
    searchKeyword,
    isLoading,
    isSubmitting,
    isClearing,
    error,
    setStatusFilter,
    setSearchKeyword,
    createTask,
    updateTask,
    advanceStatus,
    deleteTask,
    clearCompleted,
    retry,
  } = useTasks();

  return (
    <main className="app-container">
      <header className="app-header">
        <h1 className="app-title">{STRINGS.appTitle}</h1>
        <p className="app-subtitle">{STRINGS.appSubtitle}</p>
      </header>

      <ProgressBar summary={summary} />

      <TaskForm onCreate={createTask} isSubmitting={isSubmitting} />

      <section className="card controls-bar" aria-label="Task filters and search">
        <SearchInput
          value={searchKeyword}
          onChange={setSearchKeyword}
          onClear={() => setSearchKeyword('')}
        />

        <StatusFilter
          activeFilter={statusFilter}
          onSelect={setStatusFilter}
          summary={summary}
          onClearCompleted={clearCompleted}
          isClearing={isClearing}
        />
      </section>

      <TaskList
        tasks={tasks}
        isLoading={isLoading}
        error={error}
        onAdvanceStatus={advanceStatus}
        onUpdate={updateTask}
        onDelete={deleteTask}
        onRetry={retry}
      />
    </main>
  );
};

export default App;
