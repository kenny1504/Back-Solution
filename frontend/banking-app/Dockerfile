FROM node:18-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

# ---- runtime ----
FROM nginx:alpine
ARG APP_NAME=banking-app

COPY --from=build /app/dist/${APP_NAME} /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
