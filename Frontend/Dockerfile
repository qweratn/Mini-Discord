FROM node:24-alpine AS build
WORKDIR /app
RUN corepack enable \
    && corepack prepare pnpm@10.10.0 --activate
COPY package.json pnpm-lock.yaml ./
RUN pnpm install --frozen-lockfile
COPY . .
ARG CLERK_PUBLISHABLE_VALUE
ARG API_URL=/api
ARG SIGNALR_URL=/hubs/chat
RUN test -n "$CLERK_PUBLISHABLE_VALUE" \
    && VITE_CLERK_PUBLISHABLE_KEY="$CLERK_PUBLISHABLE_VALUE" \
       VITE_API_URL="$API_URL" \
       VITE_SIGNALR_URL="$SIGNALR_URL" \
       pnpm build

FROM nginx:1.29-alpine AS runtime
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
