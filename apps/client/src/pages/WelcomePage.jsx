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
    <div className="flex-1 flex items-center justify-center p-4 min-h-screen bg-vscode-bg">
      <div className="bg-vscode-sidebar rounded-xl p-8 w-full max-w-md border border-vscode-border shadow-2xl shadow-black/50 text-center">
        <div className="w-20 h-20 rounded-2xl bg-vscode-accent flex items-center justify-center mx-auto mb-6 shadow-lg shadow-vscode-accent/30">
          <svg className="w-10 h-10 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
          </svg>
        </div>

        <h1 className="text-3xl font-bold text-vscode-text mb-2">ChatApp'e Hoş Geldin</h1>
        <p className="text-vscode-muted mb-8">Sohbete katılmak için kullanıcı adını gir</p>

        <form onSubmit={handleSubmit} className="space-y-4">
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Kullanıcı adınız..."
            className="w-full px-4 py-3.5 bg-vscode-input border border-vscode-input-border rounded-lg text-vscode-text placeholder-vscode-dim focus:outline-none focus:ring-2 focus:ring-vscode-accent/50 focus:border-vscode-accent transition-all duration-200"
            autoFocus
          />
          <button
            type="submit"
            disabled={!name.trim()}
            className="w-full py-3.5 bg-vscode-accent hover:bg-vscode-accent-hover disabled:bg-vscode-input disabled:text-vscode-dim disabled:cursor-not-allowed text-white rounded-lg font-semibold transition-all duration-200 hover:shadow-lg hover:shadow-vscode-accent/30 active:scale-[0.98]"
          >
            Sohbete Başla
          </button>
        </form>
      </div>
    </div>
  );
}
