export default function MessageList({ messages }) {
  if (!messages || messages.length === 0) {
    return (
      <div className="flex-1 flex items-center justify-center text-slate-500">
        <p>Henüz mesaj yok. İlk mesajı gönderin!</p>
      </div>
    );
  }

  return (
    <div className="flex-1 overflow-y-auto p-4 space-y-3">
      {messages.map((msg, index) => (
        <div key={msg.id || index} className="flex gap-3">
          <div className="w-8 h-8 rounded-full bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center text-white text-sm font-bold flex-shrink-0">
            {(msg.userName || '?')[0].toUpperCase()}
          </div>
          <div className="flex-1 min-w-0">
            <div className="flex items-baseline gap-2">
              <span className="font-semibold text-blue-400 text-sm">
                {msg.userName}
              </span>
              <span className="text-xs text-slate-500">
                {msg.createdAt ? new Date(msg.createdAt).toLocaleTimeString('tr-TR') : ''}
              </span>
            </div>
            <p className="text-slate-200 mt-0.5 break-words">{msg.message}</p>
          </div>
        </div>
      ))}
    </div>
  );
}
