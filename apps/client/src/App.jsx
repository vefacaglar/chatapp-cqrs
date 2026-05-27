import { useState, useEffect } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import { SocketProvider } from './context/SocketContext';
import { getChatRooms } from './api/chatApi';
import { useSocketEvent } from './hooks/useSocket';
import ChatRoomList from './components/ChatRoomList';
import CreateRoomModal from './components/CreateRoomModal';
import ChatRoomPage from './pages/ChatRoomPage';
import WelcomePage from './pages/WelcomePage';

function AppLayout({ userName, rooms, onRefresh }) {
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [sidebarOpen, setSidebarOpen] = useState(false);

  useSocketEvent('room:created', () => {
    onRefresh();
  });

  return (
    <div className="flex h-screen overflow-hidden">
      {sidebarOpen && (
        <div
          className="fixed inset-0 bg-black/60 backdrop-blur-sm z-30 lg:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}

      <aside
        className={`fixed lg:static inset-y-0 left-0 z-40 w-72 bg-vscode-sidebar border-r border-vscode-border flex flex-col transform transition-transform lg:transform-none ${
          sidebarOpen ? 'translate-x-0' : '-translate-x-full lg:translate-x-0'
        }`}
      >
        <div className="p-4 border-b border-vscode-border">
          <div className="flex items-center justify-between">
            <h1 className="text-xl font-bold text-vscode-text flex items-center gap-2">
              <div className="w-8 h-8 rounded-lg bg-vscode-accent flex items-center justify-center shadow-md shadow-vscode-accent/20">
                <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
                </svg>
              </div>
              ChatApp
            </h1>
            <button
              onClick={() => setSidebarOpen(false)}
              className="text-vscode-muted hover:text-vscode-text lg:hidden transition-colors"
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
          <p className="text-sm text-vscode-muted mt-2 ml-10">Hoş geldin, {userName}</p>
        </div>

        <div className="p-3">
          <button
            onClick={() => setShowCreateModal(true)}
            className="w-full py-2.5 bg-vscode-accent hover:bg-vscode-accent-hover text-white rounded-lg font-medium transition-all duration-200 flex items-center justify-center gap-2 hover:shadow-lg hover:shadow-vscode-accent/30 active:scale-[0.98]"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
            </svg>
            Yeni Oda
          </button>
        </div>

        <div className="flex-1 overflow-y-auto">
          <ChatRoomList rooms={rooms} />
        </div>
      </aside>

      <main className="flex-1 flex flex-col min-w-0 bg-vscode-bg">
        <div className="lg:hidden p-3 border-b border-vscode-border">
          <button
            onClick={() => setSidebarOpen(true)}
            className="text-vscode-muted hover:text-vscode-text transition-colors"
          >
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          </button>
        </div>

        <Routes>
          <Route
            path="/"
            element={
              <div className="flex-1 flex items-center justify-center text-vscode-dim">
                <div className="text-center">
                  <svg className="w-20 h-20 mx-auto mb-4 text-vscode-border" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
                  </svg>
                  <p className="text-lg">Bir oda seçin veya yeni oda oluşturun</p>
                </div>
              </div>
            }
          />
          <Route path="/room/:id" element={<ChatRoomPage userName={userName} />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </main>

      <CreateRoomModal
        isOpen={showCreateModal}
        onClose={() => setShowCreateModal(false)}
        onCreated={onRefresh}
      />
    </div>
  );
}

export default function App() {
  const [userName, setUserName] = useState(() => localStorage.getItem('chatapp-username') || '');
  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchRooms = async () => {
    try {
      const data = await getChatRooms();
      setRooms(data);
    } catch (err) {
      console.error('Failed to fetch rooms:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (userName) {
      fetchRooms();
    }
  }, [userName]);

  const handleSetUserName = (name) => {
    setUserName(name);
    localStorage.setItem('chatapp-username', name);
  };

  if (!userName) {
    return (
      <SocketProvider>
        <BrowserRouter>
          <Toaster position="top-right" toastOptions={{ style: { background: '#252526', color: '#cccccc', border: '1px solid #3c3c3c' } }} />
          <WelcomePage onSetUserName={handleSetUserName} />
        </BrowserRouter>
      </SocketProvider>
    );
  }

  return (
    <SocketProvider>
      <BrowserRouter>
        <Toaster position="top-right" toastOptions={{ style: { background: '#252526', color: '#cccccc', border: '1px solid #3c3c3c' } }} />
        {loading ? (
          <div className="flex h-screen items-center justify-center bg-vscode-bg">
            <div className="animate-spin w-10 h-10 border-2 border-vscode-accent border-t-transparent rounded-full"></div>
          </div>
        ) : (
          <AppLayout userName={userName} rooms={rooms} onRefresh={fetchRooms} />
        )}
      </BrowserRouter>
    </SocketProvider>
  );
}
