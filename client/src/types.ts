export interface FocusTask {
  id: number;
  title: string;
  description?: string;
  order: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskDto {
  title: string;
  description?: string;
}

export interface UpdateTaskDto {
  title?: string;
  description?: string;
  order?: number;
}
