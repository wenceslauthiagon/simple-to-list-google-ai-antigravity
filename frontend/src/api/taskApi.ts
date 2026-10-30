import { API_BASE_URL } from '../constants/config';
import {
  CreateTaskInput,
  Task,
  TaskStatus,
  TaskSummary,
  UpdateTaskInput,
} from '../types/task';

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    let errorDetail = 'An unexpected error occurred.';
    try {
      const problem = await response.json();
      errorDetail = problem.detail || problem.title || errorDetail;
    } catch {
      errorDetail = `${response.status} ${response.statusText}`;
    }
    throw new Error(errorDetail);
  }

  if (response.status === 204) {
    return {} as T;
  }

  return response.json();
}

export const taskApi = {
  async listTasks(status?: TaskStatus, search?: string): Promise<Task[]> {
    const params = new URLSearchParams();
    if (status) params.append('status', status);
    if (search && search.trim()) params.append('search', search.trim());

    const queryString = params.toString();
    const url = `${API_BASE_URL}/api/tasks${queryString ? `?${queryString}` : ''}`;
    const response = await fetch(url);
    return handleResponse<Task[]>(response);
  },

  async getSummary(): Promise<TaskSummary> {
    const response = await fetch(`${API_BASE_URL}/api/tasks/summary`);
    return handleResponse<TaskSummary>(response);
  },

  async createTask(input: CreateTaskInput): Promise<Task> {
    const response = await fetch(`${API_BASE_URL}/api/tasks`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(input),
    });
    return handleResponse<Task>(response);
  },

  async updateTask(id: string, input: UpdateTaskInput): Promise<Task> {
    const response = await fetch(`${API_BASE_URL}/api/tasks/${id}`, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(input),
    });
    return handleResponse<Task>(response);
  },

  async advanceTaskStatus(id: string): Promise<Task> {
    const response = await fetch(`${API_BASE_URL}/api/tasks/${id}/advance-status`, {
      method: 'POST',
    });
    return handleResponse<Task>(response);
  },

  async deleteTask(id: string): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/api/tasks/${id}`, {
      method: 'DELETE',
    });
    await handleResponse<void>(response);
  },

  async clearCompleted(): Promise<{ deletedCount: number }> {
    const response = await fetch(`${API_BASE_URL}/api/tasks/completed`, {
      method: 'DELETE',
    });
    return handleResponse<{ deletedCount: number }>(response);
  },
};
