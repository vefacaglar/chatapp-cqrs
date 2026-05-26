import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

export default function WelcomePage({ onSetUserName }) {
  const [name, setName] = useState('');
  const navigate = useNavigate();

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!name.trim()) return;

    onSetUserName(name.trim());
    navigate('/');
  };

  return (
    <div className="flex-1 flex items-center justify-center p-4">
      <div className="bg-slate-800 rounded-2xl p-8 w-full max-w-md border border-slate-700 shadow-2xl text-center">
        <div className="w-16 h-16 rounded-full bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center mx-auto mb-6">
          <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
          </svg>
        </div>

        <h1 className="text-2xl font-bold text-white mb-2">ChatApp'e Hoş Geldin</h1>
        <p className="text-slate-400 mb-6">Sohbete katılmak için kullanıcı adını gir</p>

        <form onSubmit={handleSubmit}>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Kullanıcı adınız..."
            className="w-full px-4 py-3 bg-slate-700 border border-slate-600 rounded-xl text-white placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent mb-4"
            autoFocus
          />
          <button
            type="submit"
            disabled={!name.trim()}
            className="w-full py-3 bg-blue-600 hover:bg-blue-700 disabled:bg-slate-600 disabled:cursor-not-allowed text-white rounded-xl font-medium transition-colors"
          >
            Sohbete Başla
          </button>
        </form>
      </div>
    </div>
  );
}
