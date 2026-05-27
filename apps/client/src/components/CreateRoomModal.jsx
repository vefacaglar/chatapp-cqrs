import { useState } from 'react';
import { createChatRoom } from '../api/chatApi';

export default function CreateRoomModal({ isOpen, onClose, onCreated }) {
  const [name, setName] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  if (!isOpen) return null;

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!name.trim()) return;

    setLoading(true);
    setError('');

    try {
      const result = await createChatRoom(name.trim());
      setName('');
      onCreated(result);
      onClose();
    } catch (err) {
      setError('Failed to create room. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm">
      <div className="bg-vscode-sidebar rounded-xl p-6 w-full max-w-md mx-4 border border-vscode-border shadow-2xl shadow-black/50">
        <h2 className="text-xl font-semibold text-vscode-text mb-4">Create New Room</h2>

        <form onSubmit={handleSubmit} className="space-y-4">
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Enter room name..."
            className="w-full px-4 py-3 bg-vscode-input border border-vscode-input-border rounded-lg text-vscode-text placeholder-vscode-dim focus:outline-none focus:ring-2 focus:ring-vscode-accent/50 focus:border-vscode-accent transition-all duration-200"
            autoFocus
            disabled={loading}
          />

          {error && (
            <p className="text-vscode-error text-sm">{error}</p>
          )}

          <div className="flex gap-3 justify-end">
            <button
              type="button"
              onClick={onClose}
              disabled={loading}
              className="px-4 py-2 text-vscode-muted hover:text-vscode-text transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading || !name.trim()}
              className="px-6 py-2 bg-vscode-accent hover:bg-vscode-accent-hover disabled:bg-vscode-input disabled:text-vscode-dim disabled:cursor-not-allowed text-white rounded-lg font-medium transition-all duration-200 hover:shadow-lg hover:shadow-vscode-accent/30 active:scale-[0.98]"
            >
              {loading ? 'Creating...' : 'Create'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
