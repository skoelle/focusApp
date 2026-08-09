// Copyright (c) 2026 Stefan Koelle (https://stefankoelle.de)
// Licensed under the MIT License. See LICENSE file in project root for details.
import React, { useState, useEffect } from 'react';
import { FocusTask } from '../types';
import './TaskCard.css';

interface TaskCardProps {
  task: FocusTask;
  onUpdate: (title: string, description?: string) => void;
  onDelete: () => void;
}

const TaskCard: React.FC<TaskCardProps> = ({ task, onUpdate, onDelete }) => {
  const [isEditing, setIsEditing] = useState(false);
  const [editTitle, setEditTitle] = useState(task.title);
  const [editDescription, setEditDescription] = useState(task.description || '');

  useEffect(() => {
    setEditTitle(task.title);
    setEditDescription(task.description || '');
  }, [task.id, task.title, task.description]);

  const handleSave = () => {
    if (editTitle.trim()) {
      onUpdate(editTitle, editDescription);
      setIsEditing(false);
    }
  };

  const handleCancel = () => {
    setEditTitle(task.title);
    setEditDescription(task.description || '');
    setIsEditing(false);
  };

  if (isEditing) {
    return (
      <div className="task-card editing">
        <div className="edit-form">
          <input
            type="text"
            value={editTitle}
            onChange={(e) => setEditTitle(e.target.value)}
            className="edit-input"
            autoFocus
          />
          <textarea
            value={editDescription}
            onChange={(e) => setEditDescription(e.target.value)}
            className="edit-textarea"
            rows={3}
          />
          <div className="edit-actions">
            <button className="btn-save" onClick={handleSave}>
              ✓ Speichern
            </button>
            <button className="btn-cancel" onClick={handleCancel}>
              ✗ Abbrechen
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="task-card">
      <div className="task-handle">⋮⋮</div>
      <div className="task-content">
        <h3 className="task-title">{task.title}</h3>
        {task.description && <p className="task-description">{task.description}</p>}
        <div className="task-meta">
          <span className="task-date">
            {new Date(task.updatedAt).toLocaleDateString('de-DE', {
              year: 'numeric',
              month: 'short',
              day: 'numeric',
            })}
          </span>
        </div>
      </div>
      <div className="task-actions">
        <button className="btn-edit" onClick={() => setIsEditing(true)} title="Bearbeiten">
          ✎
        </button>
        <button className="btn-delete" onClick={onDelete} title="Löschen">
          ✕
        </button>
      </div>
    </div>
  );
};

export default TaskCard;
