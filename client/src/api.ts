import axios from 'axios';
import { FocusTask, CreateTaskDto, UpdateTaskDto } from './types';

const API_URL = '/api/focustasks';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const focusTaskApi = {
  getTasks: async (): Promise<FocusTask[]> => {
    const { data } = await api.get('');
    return data;
  },

  createTask: async (dto: CreateTaskDto): Promise<FocusTask> => {
    const { data } = await api.post('', dto);
    return data;
  },

  updateTask: async (id: number, dto: UpdateTaskDto): Promise<FocusTask> => {
    const { data } = await api.put(`/${id}`, dto);
    return data;
  },

  deleteTask: async (id: number): Promise<void> => {
    await api.delete(`/${id}`);
  },

  reorderTasks: async (
    orders: Array<{ id: number; order: number }>
  ): Promise<FocusTask[]> => {
    const { data } = await api.put('/reorder', { orders });
    return data;
  },
};
