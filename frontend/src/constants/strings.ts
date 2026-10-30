export const STRINGS = {
  appTitle: 'To-Do List',
  appSubtitle: 'Professional Task Management with Clean Architecture & MongoDB',

  form: {
    titlePlaceholder: 'What needs to be done? (1-200 characters)',
    addButton: 'Add Task',
    addingButton: 'Adding...',
    priorityLabel: 'Priority',
    validationEmpty: 'Task title cannot be empty.',
    validationTooLong: 'Task title cannot exceed 200 characters.',
  },

  search: {
    placeholder: 'Search tasks by keyword...',
    clearSearch: 'Clear search',
    ariaLabel: 'Search tasks',
  },

  filter: {
    all: 'All',
    pending: 'Pending',
    inProgress: 'In Progress',
    completed: 'Completed',
  },

  progress: {
    label: 'Progress',
    summaryPattern: '{completed} of {total} completed, {percentage}%',
    allDone: 'All tasks completed! Great work!',
    empty: 'No tasks yet. Create one above to get started.',
  },

  actions: {
    clearCompleted: 'Clear completed',
    clearingCompleted: 'Clearing...',
    deleteTask: 'Delete task',
    editTask: 'Edit task',
    saveEdit: 'Save',
    cancelEdit: 'Cancel',
    advanceStatusAria: 'Advance task status (current: {status})',
    statusSelectAria: 'Change task status',
    prioritySelectAria: 'Change task priority',
  },

  priorities: {
    low: 'Low',
    medium: 'Medium',
    high: 'High',
  },

  statuses: {
    pending: 'Pending',
    inProgress: 'In Progress',
    completed: 'Completed',
  },

  states: {
    loading: 'Loading tasks...',
    errorPrefix: 'Error: ',
    retry: 'Retry',
    emptyList: 'No tasks found matching your criteria.',
    emptyAll: 'No tasks yet. Add a new task above!',
  },
} as const;
