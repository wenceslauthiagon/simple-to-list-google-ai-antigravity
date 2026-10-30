import { useCallback, useEffect, useState } from 'react';
import { taskApi } from '../api/taskApi';
import { SEARCH_DEBOUNCE_MS } from '../constants/config';
import {
  CreateTaskInput,
  StatusFilterOption,
  Task,
  TaskStatus,
  TaskSummary,
  UpdateTaskInput,
} from '../types/task';
import { useDebounce } from './useDebounce';

const initialSummary: TaskSummary = {
  total: 0,
  pending: 0,
  inProgress: 0,
  completed: 0,
  progressPercentage: 0,
};

export function useTasks() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [summary, setSummary] = useState<TaskSummary>(initialSummary);
  const [statusFilter, setStatusFilter] = useState<StatusFilterOption>('all');
  const [searchKeyword, setSearchKeyword] = useState<string>('');
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [isClearing, setIsClearing] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const debouncedSearch = useDebounce(searchKeyword, SEARCH_DEBOUNCE_MS);

  const refreshSummary = useCallback(async () => {
    try {
      const summaryData = await taskApi.getSummary();
      setSummary(summaryData);
    } catch {
      // Non-critical summary refresh failure handled silently
    }
  }, []);

  const loadTasks = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const filterParam: TaskStatus | undefined =
        statusFilter === 'all' ? undefined : statusFilter;

      const [taskList, summaryData] = await Promise.all([
        taskApi.listTasks(filterParam, debouncedSearch),
        taskApi.getSummary(),
      ]);

      setTasks(taskList);
      setSummary(summaryData);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load tasks');
    } finally {
      setIsLoading(false);
    }
  }, [statusFilter, debouncedSearch]);

  useEffect(() => {
    loadTasks();
  }, [loadTasks]);

  const createTask = async (input: CreateTaskInput): Promise<boolean> => {
    setIsSubmitting(true);
    setError(null);
    try {
      await taskApi.createTask(input);
      await loadTasks();
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create task');
      return false;
    } finally {
      setIsSubmitting(false);
    }
  };

  const updateTask = async (id: string, input: UpdateTaskInput): Promise<boolean> => {
    setError(null);
    try {
      const updated = await taskApi.updateTask(id, input);
      setTasks((prev) => prev.map((t) => (t.id === id ? updated : t)));
      await refreshSummary();
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update task');
      return false;
    }
  };

  const advanceStatus = async (id: string): Promise<boolean> => {
    setError(null);
    try {
      const updated = await taskApi.advanceTaskStatus(id);
      if (statusFilter !== 'all' && updated.status !== statusFilter) {
        setTasks((prev) => prev.filter((t) => t.id !== id));
      } else {
        setTasks((prev) => prev.map((t) => (t.id === id ? updated : t)));
      }
      await refreshSummary();
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to advance status');
      return false;
    }
  };

  const deleteTask = async (id: string): Promise<boolean> => {
    setError(null);
    try {
      await taskApi.deleteTask(id);
      setTasks((prev) => prev.filter((t) => t.id !== id));
      await refreshSummary();
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete task');
      return false;
    }
  };

  const clearCompleted = async (): Promise<boolean> => {
    setIsClearing(true);
    setError(null);
    try {
      await taskApi.clearCompleted();
      await loadTasks();
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to clear completed tasks');
      return false;
    } finally {
      setIsClearing(false);
    }
  };

  return {
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
    retry: loadTasks,
  };
}
