import { Server } from 'socket.io';
import Redis from 'ioredis';
import config from './config.js';

function toCamelCase(str) {
  return str.charAt(0).toLowerCase() + str.slice(1);
}

function camelizeKeys(obj) {
  if (Array.isArray(obj)) return obj.map(camelizeKeys);
  if (obj !== null && typeof obj === 'object') {
    return Object.fromEntries(
      Object.entries(obj).map(([key, value]) => [toCamelCase(key), camelizeKeys(value)])
    );
  }
  return obj;
}

const redisSub = new Redis({
  host: config.redis.host,
  port: config.redis.port,
  retryStrategy(times) {
    const delay = Math.min(times * 200, 5000);
    return delay;
  },
});

const io = new Server(config.socketio.port, {
  cors: config.socketio.cors,
});

console.log(`Socket.IO server listening on port ${config.socketio.port}`);

io.on('connection', (socket) => {
  console.log(`Client connected: ${socket.id}`);

  socket.on('join:room', (roomId) => {
    socket.join(`room:${roomId}`);
    console.log(`Socket ${socket.id} joined room:${roomId}`);
  });

  socket.on('leave:room', (roomId) => {
    socket.leave(`room:${roomId}`);
    console.log(`Socket ${socket.id} left room:${roomId}`);
  });

  socket.on('disconnect', () => {
    console.log(`Client disconnected: ${socket.id}`);
  });
});

redisSub.psubscribe('chat:room:*:message', (err) => {
  if (err) {
    console.error('Failed to psubscribe:', err);
    return;
  }
  console.log('Subscribed to chat:room:*:message');
});

redisSub.subscribe(config.channels.roomCreated, (err) => {
  if (err) {
    console.error('Failed to subscribe to room created:', err);
    return;
  }
  console.log(`Subscribed to ${config.channels.roomCreated}`);
});

redisSub.on('pmessage', (pattern, channel, message) => {
  try {
    const raw = JSON.parse(message);
    const data = camelizeKeys(raw);
    const roomId = data.roomId;

    if (roomId) {
      io.to(`room:${roomId}`).emit('message:new', data);
      console.log(`Broadcast message to room:${roomId}`);
    }
  } catch (err) {
    console.error('Failed to process pmessage:', err);
  }
});

redisSub.on('message', (channel, message) => {
  try {
    const raw = JSON.parse(message);
    const data = camelizeKeys(raw);

    if (channel === config.channels.roomCreated) {
      io.emit('room:created', data);
      console.log('Broadcast room:created to all clients');
    }
  } catch (err) {
    console.error('Failed to process message:', err);
  }
});

process.on('SIGINT', () => {
  console.log('Shutting down...');
  redisSub.quit();
  io.close();
  process.exit(0);
});
