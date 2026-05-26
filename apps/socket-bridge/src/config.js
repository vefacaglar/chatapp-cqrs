export default {
  redis: {
    host: process.env.REDIS_HOST || 'localhost',
    port: parseInt(process.env.REDIS_PORT || '6379'),
  },
  socketio: {
    port: parseInt(process.env.SOCKETIO_PORT || '3001'),
    cors: {
      origin: process.env.CORS_ORIGIN || '*',
      methods: ['GET', 'POST'],
    },
  },
  channels: {
    roomCreated: 'chat:room:created',
    messagePattern: 'chat:room:*:message',
  },
};
