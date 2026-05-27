#!/usr/bin/env bash
set -e

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

log() { echo -e "${BLUE}[setup]${NC} $1"; }
ok()  { echo -e "${GREEN}[ok]${NC} $1"; }
warn(){ echo -e "${YELLOW}[warn]${NC} $1"; }
err() { echo -e "${RED}[error]${NC} $1"; }

cd "$(dirname "$0")/.."

# ── 1. pnpm ──
if ! command -v pnpm &> /dev/null; then
  log "pnpm bulunamadi, yukleniyor..."
  npm install -g pnpm
  ok "pnpm yuklendi"
else
  ok "pnpm mevcut: $(pnpm --version)"
fi

# ── 2. .NET SDK ──
if ! command -v dotnet &> /dev/null; then
  err "dotnet SDK bulunamadi. Lutfen .NET 10 SDK yukleyin: https://dotnet.microsoft.com/download"
  exit 1
fi
ok ".NET SDK mevcut: $(dotnet --version)"

# ── 3. Docker ──
if ! command -v docker &> /dev/null; then
  err "Docker bulunamadi. Lutfen Docker yukleyin: https://www.docker.com/"
  exit 1
fi
ok "Docker mevcut"

# ── 4. JS bagliliklari ──
log "JS bagliliklari yukleniyor (pnpm install)..."
pnpm install
ok "JS baglilikleri yuklendi"

# ── 5. .NET restore ──
log "NuGet paketleri restore ediliyor..."
dotnet restore
ok "NuGet paketleri restore edildi"

# ── 6. Docker Compose ──
log "Docker servisleri baslatiliyor..."
docker compose up -d
ok "Docker servisleri baslatildi"

# ── 7. EF Migrations ──
log "Veritabani migration uygulanıyor..."
dotnet ef database update \
  --project apps/api/ChatApp.Infrastructure \
  --startup-project apps/api/ChatApp.Api 2>/dev/null || warn "Migration atlaniyor (veritabani hazir olmayabilir)"

echo ""
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}  Kurulum tamamlandi!${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo -e "Tum servisleri baslatmak icin:"
echo -e "  ${BLUE}pnpm dev:all${NC}"
echo ""
echo -e "Ayri ayri baslatmak icin:"
echo -e "  ${BLUE}pnpm dev${NC}                  # JS uygulamalari"
echo -e "  ${BLUE}dotnet run --project apps/api/ChatApp.Api${NC}  # .NET API"
echo ""
