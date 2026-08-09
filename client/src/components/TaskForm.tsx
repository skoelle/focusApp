// Copyright (c) 2026 Stefan Koelle (https://stefankoelle.de)
// Licensed under the MIT License. See LICENSE file in project root for details.
import React, { useState } from 'react';
import './TaskForm.css';

interface TaskFormProps {
  onAdd: (title: string, description?: string) => void;
}

const TaskForm: React.FC<TaskFormProps> = ({ onAdd }) => {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [isExpanded, setIsExpanded] = useState(false);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (title.trim()) {
      onAdd(title, description);
      setTitle('');
      setDescription('');
      setIsExpanded(false);
    }
  };

  return (
    <form className="task-form" onSubmit={handleSubmit}>
      <input
        type="text"
        placeholder="Neuen Fokus-Task hinzufügen..."
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        onFocus={() => setIsExpanded(true)}
        className="task-input"
      />

      {isExpanded && (
        <div className="form-expanded">
          <textarea
            placeholder="Optionale Notiz (z.B. Gedanken, Checkliste, Status)"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="description-input"
            rows={3}
          />
          <div className="form-actions">
            <button type="submit" className="btn btn-primary">
              ➕ Hinzufügen
            </button>
            <button
              type="button"
              className="btn btn-secondary"
              onClick={() => {
                setIsExpanded(false);
                setDescription('');
              }}
            >
              Abbrechen
            </button>
          </div>
        </div>
      )}
    </form>
  );
};

export default TaskForm;
