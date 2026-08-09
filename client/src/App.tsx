import React, { useState, useEffect } from 'react';
import { DragDropContext, Droppable, Draggable, DropResult } from '@hello-pangea/dnd';
import { focusTaskApi } from './api';
import { FocusTask } from './types';
import TaskCard from './components/TaskCard';
import TaskForm from './components/TaskForm';
import './styles/design-system.css';
import './App.css';

const App: React.FC = () => {
  const [tasks, setTasks] = useState<FocusTask[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (error) {
      const timer = setTimeout(() => setError(null), 5000);
      return () => clearTimeout(timer);
    }
  }, [error]);

  useEffect(() => {
    loadTasks();
  }, []);

  const loadTasks = async () => {
    try {
      setLoading(true);
      const data = await focusTaskApi.getTasks();
      setTasks(data);
      setError(null);
    } catch (err) {
      setError('Failed to load tasks');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleAddTask = async (title: string, description?: string) => {
    try {
      const newTask = await focusTaskApi.createTask({ title, description });
        setTasks([newTask, ...tasks]);
    } catch (err) {
      setError('Failed to create task');
      console.error(err);
    }
  };

  const handleUpdateTask = async (id: number, title: string, description?: string) => {
    try {
      const updated = await focusTaskApi.updateTask(id, { title, description });
      setTasks(tasks.map(t => t.id === id ? updated : t));
    } catch (err) {
      setError('Failed to update task');
      console.error(err);
    }
  };

  const handleDeleteTask = async (id: number) => {
    try {
      await focusTaskApi.deleteTask(id);
      setTasks(tasks.filter(t => t.id !== id));
    } catch (err) {
      setError('Failed to delete task');
      console.error(err);
    }
  };

  const handleDragEnd = async (result: DropResult) => {
    const { source, destination } = result;

    if (!destination) return;
    if (source.index === destination.index) return;

    const newTasks = Array.from(tasks);
    const [movedTask] = newTasks.splice(source.index, 1);
    newTasks.splice(destination.index, 0, movedTask);

    setTasks(newTasks);

    // Update order in backend
    try {
      const orders = newTasks.map((task, index) => ({
        id: task.id,
        order: index + 1,
      }));
      await focusTaskApi.reorderTasks(orders);
    } catch (err) {
      setError('Failed to reorder tasks');
      console.error(err);
      loadTasks(); // Reload on error
    }
  };

  return (
    <div className="app">
      <div className="container">
        <header className="header">
          <h1>🎯 Focus Tasks</h1>
          <p>Deine aktiven Schwerpunkte für die nächsten 2-3 Wochen</p>
        </header>

        {error && <div className="error-message">{error}</div>}

        <TaskForm onAdd={handleAddTask} />

        {loading ? (
          <div className="loading">Lade Tasks...</div>
        ) : tasks.length === 0 ? (
          <div className="empty-state">
            <p>Keine Tasks vorhanden. Füge einen neuen Task hinzu!</p>
          </div>
        ) : (
          <DragDropContext onDragEnd={handleDragEnd}>
            <Droppable droppableId="tasks">
              {(provided, snapshot) => (
                <div
                  className={`tasks-list ${snapshot.isDraggingOver ? 'dragging' : ''}`}
                  {...provided.droppableProps}
                  ref={provided.innerRef}
                >
                  {tasks.map((task, index) => (
                    <Draggable key={task.id} draggableId={task.id.toString()} index={index}>
                      {(provided, snapshot) => (
                        <div
                          ref={provided.innerRef}
                          {...provided.draggableProps}
                          {...provided.dragHandleProps}
                          className={snapshot.isDragging ? 'dragging' : ''}
                        >
                          <TaskCard
                            task={task}
                            onUpdate={(title, desc) => handleUpdateTask(task.id, title, desc)}
                            onDelete={() => handleDeleteTask(task.id)}
                          />
                        </div>
                      )}
                    </Draggable>
                  ))}
                  {provided.placeholder}
                </div>
              )}
            </Droppable>
          </DragDropContext>
        )}
      </div>
    </div>
  );
};

export default App;
