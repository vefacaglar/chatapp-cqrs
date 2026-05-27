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
  log "pnpm not found, installing..."
  npm install -g pnpm
  ok "pnpm installed"
else
  ok "pnpm available: $(pnpm --version)"
fi

# ── 2. .NET SDK ──
if ! command -v dotnet &> /dev/null; then
  err "dotnet SDK not found. Please install .NET 10 SDK: https://dotnet.microsoft.com/download"
  exit 1
fi
ok ".NET SDK available: $(dotnet --version)"

# ── 3. Docker ──
if ! command -v docker &> /dev/null; then
  err "Docker not found. Please install Docker: https://www.docker.com/"
  exit 1
fi
ok "Docker available"

# ── 4. JS dependencies ──
log "Installing JS dependencies (pnpm install)..."
pnpm install
ok "JS dependencies installed"

# ── 5. .NET restore ──
log "Restoring NuGet packages..."
dotnet restore
ok "NuGet packages restored"

# ── 6. Docker Compose ──
log "Starting Docker services..."
docker compose up -d
ok "Docker services started"

# ── 7. EF Migrations ──
log "Applying database migrations..."
dotnet ef database update \
  --project apps/api/ChatApp.Infrastructure \
  --startup-project apps/api/ChatApp.Api 2>/dev/null || warn "Migration skipped (database may not be ready)"

echo ""
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}  Setup complete!${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo -e "To start all services:"
echo -e "  ${BLUE}pnpm dev:all${NC}"
echo ""
echo -e "To start individually:"
echo -e "  ${BLUE}pnpm dev${NC}                  # JS applications"
echo -e "  ${BLUE}dotnet run --project apps/api/ChatApp.Api${NC}  # .NET API"
echo ""
