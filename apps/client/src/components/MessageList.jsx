export default function MessageList({ messages }) {
  if (!messages || messages.length === 0) {
    return (
      <div className="flex-1 flex items-center justify-center text-vscode-dim">
        <p>No messages yet. Send the first one!</p>
      </div>
    );
  }

  return (
    <div className="flex-1 overflow-y-auto p-4 space-y-3">
      {messages.map((msg, index) => (
        <div key={msg.id || index} className="flex gap-3 group">
          <div className="w-9 h-9 rounded-lg bg-vscode-input flex items-center justify-center text-vscode-text text-sm font-bold flex-shrink-0 ring-2 ring-vscode-border group-hover:ring-vscode-accent/50 transition-all duration-200">
            {(msg.userName || '?')[0].toUpperCase()}
          </div>
          <div className="flex-1 min-w-0">
            <div className="flex items-baseline gap-2">
              <span className="font-semibold text-vscode-blue text-sm">
                {msg.userName}
              </span>
              <span className="text-xs text-vscode-dim">
                {msg.createdAt ? new Date(msg.createdAt).toLocaleTimeString('en-US') : ''}
              </span>
            </div>
            <p className="text-vscode-text-bright mt-1 break-words bg-vscode-sidebar/50 rounded-lg px-3 py-2 inline-block">
              {msg.message}
            </p>
          </div>
        </div>
      ))}
    </div>
  );
}
