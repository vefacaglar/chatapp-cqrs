import { Link, useParams } from 'react-router-dom';

export default function ChatRoomList({ rooms }) {
  const { id: activeId } = useParams();

  if (!rooms || rooms.length === 0) {
    return (
      <div className="p-4 text-center text-vscode-dim">
        <p>No rooms yet.</p>
        <p className="text-sm mt-1">Create a new room!</p>
      </div>
    );
  }

  return (
    <div className="space-y-1 p-2">
      {rooms.map((room) => (
        <Link
          key={room.id}
          to={`/room/${room.id}`}
          className={`block px-3 py-2.5 rounded-lg transition-all duration-200 ${
            activeId === room.id
              ? 'bg-vscode-active text-vscode-text shadow-sm'
              : 'hover:bg-vscode-hover text-vscode-muted hover:text-vscode-text'
          }`}
        >
          <div className="flex items-center gap-3">
            <div className={`w-10 h-10 rounded-lg flex items-center justify-center text-white font-bold flex-shrink-0 text-sm transition-all duration-200 ${
              activeId === room.id
                ? 'bg-vscode-accent shadow-md shadow-vscode-accent/20'
                : 'bg-vscode-input'
            }`}>
              {room.name?.[0]?.toUpperCase() || '?'}
            </div>
            <div className="flex-1 min-w-0">
              <p className="font-medium truncate text-sm">{room.name}</p>
              <p className="text-xs text-vscode-dim mt-0.5">
                {room.messages?.length || 0} messages
              </p>
            </div>
          </div>
        </Link>
      ))}
    </div>
  );
}
