import { Link, useParams } from 'react-router-dom';

export default function ChatRoomList({ rooms }) {
  const { id: activeId } = useParams();

  if (!rooms || rooms.length === 0) {
    return (
      <div className="p-4 text-center text-slate-500">
        <p>Henüz oda yok.</p>
        <p className="text-sm mt-1">Yeni bir oda oluşturun!</p>
      </div>
    );
  }

  return (
    <div className="space-y-1 p-2">
      {rooms.map((room) => (
        <Link
          key={room.id}
          to={`/room/${room.id}`}
          className={`block px-4 py-3 rounded-xl transition-all ${
            activeId === room.id
              ? 'bg-blue-600/20 border border-blue-500/30 text-white'
              : 'hover:bg-slate-700/50 text-slate-300 hover:text-white'
          }`}
        >
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-full bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center text-white font-bold flex-shrink-0">
              {room.name[0].toUpperCase()}
            </div>
            <div className="flex-1 min-w-0">
              <p className="font-medium truncate">{room.name}</p>
              <p className="text-xs text-slate-500 mt-0.5">
                {room.messages?.length || 0} mesaj
              </p>
            </div>
          </div>
        </Link>
      ))}
    </div>
  );
}
