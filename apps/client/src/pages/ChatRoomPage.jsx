import { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getChatRoom, sendMessage } from '../api/chatApi';
import { useSocketEvent, useRoomSocket } from '../hooks/useSocket';
import MessageList from '../components/MessageList';
import MessageInput from '../components/MessageInput';
import toast from 'react-hot-toast';

export default function ChatRoomPage({ userName }) {
  const { id: roomId } = useParams();
  const navigate = useNavigate();
  const [room, setRoom] = useState(null);
  const [loading, setLoading] = useState(true);
  const [sending, setSending] = useState(false);
  const messagesEndRef = useRef(null);

  useRoomSocket(roomId);

  useEffect(() => {
    const fetchRoom = async () => {
      try {
        setLoading(true);
        const data = await getChatRoom(roomId);
        setRoom(data);
      } catch (err) {
        toast.error('Oda yüklenemedi');
        navigate('/');
      } finally {
        setLoading(false);
      }
    };

    fetchRoom();
  }, [roomId, navigate]);

  useSocketEvent('message:new', (message) => {
    if (message.roomId === roomId) {
      setRoom((prev) => {
        if (!prev) return prev;
        return {
          ...prev,
          messages: [...(prev.messages || []), message],
        };
      });
    }
  });

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [room?.messages]);

  const handleSend = async (text) => {
    if (!userName) {
      toast.error('Lütfen önce bir kullanıcı adı belirleyin');
      return;
    }

    setSending(true);
    try {
      await sendMessage(roomId, userName, text);
    } catch (err) {
      toast.error('Mesaj gönderilemedi');
    } finally {
      setSending(false);
    }
  };

  if (loading) {
    return (
      <div className="flex-1 flex items-center justify-center">
        <div className="animate-spin w-10 h-10 border-2 border-vscode-accent border-t-transparent rounded-full"></div>
      </div>
    );
  }

  if (!room) return null;

  return (
    <div className="flex flex-col h-full">
      <div className="px-6 py-4 border-b border-vscode-border flex items-center gap-4 bg-vscode-sidebar">
        <button
          onClick={() => navigate('/')}
          className="text-vscode-muted hover:text-vscode-text transition-colors lg:hidden"
        >
          <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
          </svg>
        </button>
        <div className="w-11 h-11 rounded-xl bg-vscode-accent flex items-center justify-center text-white font-bold shadow-md shadow-vscode-accent/20">
          {room.name[0].toUpperCase()}
        </div>
        <div>
          <h2 className="font-semibold text-vscode-text text-lg">{room.name}</h2>
          <p className="text-sm text-vscode-muted">
            {room.messages?.length || 0} mesaj
          </p>
        </div>
      </div>

      <MessageList messages={room.messages} />
      <div ref={messagesEndRef} />

      <MessageInput onSend={handleSend} disabled={sending} />
    </div>
  );
}
