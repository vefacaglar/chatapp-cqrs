import { useContext, useEffect, useRef, useState } from 'react';
import { SocketContext } from '../context/socketContext';

export function useSocket() {
  return useContext(SocketContext);
}

export function useSocketEvent(event, handler) {
  const socket = useSocket();
  const handlerRef = useRef(handler);

  useEffect(() => {
    handlerRef.current = handler;
  }, [handler]);

  useEffect(() => {
    if (!socket) return;

    const listener = (...args) => handlerRef.current(...args);
    socket.on(event, listener);

    return () => {
      socket.off(event, listener);
    };
  }, [socket, event]);
}

export function useRoomSocket(roomId) {
  const socket = useSocket();

  useEffect(() => {
    if (!socket || !roomId) return;

    socket.emit('join:room', roomId);

    return () => {
      socket.emit('leave:room', roomId);
    };
  }, [socket, roomId]);
}

export function useSocketConnected() {
  const socket = useSocket();
  const [connected, setConnected] = useState(() => socket?.connected ?? false);

  useEffect(() => {
    if (!socket) return;

    const onConnect = () => setConnected(true);
    const onDisconnect = () => setConnected(false);

    socket.on('connect', onConnect);
    socket.on('disconnect', onDisconnect);

    return () => {
      socket.off('connect', onConnect);
      socket.off('disconnect', onDisconnect);
    };
  }, [socket]);

  return connected;
}
