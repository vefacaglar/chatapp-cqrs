import { useState } from 'react';

export default function MessageInput({ onSend, disabled }) {
  const [message, setMessage] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!message.trim() || disabled) return;

    onSend(message.trim());
    setMessage('');
  };

  return (
    <form onSubmit={handleSubmit} className="p-4 border-t border-vscode-border bg-vscode-sidebar">
      <div className="flex gap-2">
        <input
          type="text"
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          placeholder="Mesajınızı yazın..."
          className="flex-1 px-4 py-3 bg-vscode-input border border-vscode-input-border rounded-lg text-vscode-text placeholder-vscode-dim focus:outline-none focus:ring-2 focus:ring-vscode-accent/50 focus:border-vscode-accent transition-all duration-200"
          disabled={disabled}
        />
        <button
          type="submit"
          disabled={disabled || !message.trim()}
          className="px-6 py-3 bg-vscode-accent hover:bg-vscode-accent-hover disabled:bg-vscode-input disabled:text-vscode-dim disabled:cursor-not-allowed text-white rounded-lg font-medium transition-all duration-200 flex items-center gap-2 hover:shadow-lg hover:shadow-vscode-accent/30 active:scale-[0.98]"
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
          </svg>
          Gönder
        </button>
      </div>
    </form>
  );
}
