import { useEffect, useState } from 'react';
import { io } from 'socket.io-client';
import { SocketContext } from './socketContext';

const SOCKET_URL = import.meta.env.VITE_SOCKET_URL || 'http://localhost:3001';

export function SocketProvider({ children }) {
  const [socket] = useState(() =>
    io(SOCKET_URL, {
      transports: ['polling'],
      reconnection: true,
      reconnectionAttempts: 10,
      reconnectionDelay: 1000,
    })
  );

  useEffect(() => {
    const onConnect = () => {
      console.log('Socket.IO connected:', socket.id);
    };

    const onDisconnect = (reason) => {
      console.log('Socket.IO disconnected:', reason);
    };

    const onConnectError = (err) => {
      console.error('Socket.IO connection error:', err.message);
    };

    const onAny = (event, ...args) => {
      console.log(`[DEBUG] Socket event received: "${event}"`, args);
    };

    socket.on('connect', onConnect);
    socket.on('disconnect', onDisconnect);
    socket.on('connect_error', onConnectError);
    socket.onAny(onAny);

    return () => {
      socket.off('connect', onConnect);
      socket.off('disconnect', onDisconnect);
      socket.off('connect_error', onConnectError);
      socket.offAny(onAny);
    };
  }, [socket]);

  return (
    <SocketContext.Provider value={socket}>
      {children}
    </SocketContext.Provider>
  );
}
