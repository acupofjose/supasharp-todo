FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

RUN curl -sL https://deb.nodesource.com/setup_16.x | bash - \
 && apt-get install -y --no-install-recommends nodejs \
 && echo "node version: $(node --version)" \
 && echo "npm version: $(npm --version)" \
 && rm -rf /var/lib/apt/lists/*

COPY . .

RUN npm install && npm run build:tailwind:wasm

WORKDIR /app/SupasharpTodo.BlazorWASM
RUN dotnet restore
RUN dotnet publish -c release /p:APP_TASKS=prerender -o /out --no-restore

FROM joseluisq/static-web-server AS runtime
WORKDIR /public
COPY --from=build /out .

CMD ["-p", "5100", "-d", "wwwroot", "-g", "trace"]