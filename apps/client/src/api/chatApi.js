import axios from 'axios';

const api = axios.create({
  baseURL: '/api/v1/chat',
  headers: {
    'Content-Type': 'application/json',
  },
});

export const getChatRooms = async () => {
  const { data } = await api.get('/');
  return data;
};

export const getChatRoom = async (id) => {
  const { data } = await api.get(`/${id}`);
  return data;
};

export const createChatRoom = async (name) => {
  const { data } = await api.post('/', { name });
  return data;
};

export const sendMessage = async (roomId, userName, message) => {
  const { data } = await api.post('/message', { roomId, userName, message });
  return data;
};

export default api;
