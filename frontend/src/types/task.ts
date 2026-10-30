export type TaskStatus = 'pending' | 'inProgress' | 'completed';

export type TaskPriority = 'low' | 'medium' | 'high';

export type StatusFilterOption = 'all' | TaskStatus;

export interface Task {
  id: string;
  title: string;
  status: TaskStatus;
  priority: TaskPriority;
  createdAt: string;
  updatedAt?: string | null;
}

export interface TaskSummary {
  total: number;
  pending: number;
  inProgress: number;
  completed: number;
  progressPercentage: number;
}

export interface CreateTaskInput {
  title: string;
  priority: TaskPriority;
}

export interface UpdateTaskInput {
  title?: string;
  priority?: TaskPriority;
  status?: TaskStatus;
}
