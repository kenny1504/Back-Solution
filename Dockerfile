# ---- build ----
FROM node:18-alpine AS build
WORKDIR /app

# Instala deps
COPY package*.json ./
RUN npm ci

# Copia fuente y compila
COPY . .
# Si tu script "build" ya hace ng build --configuration production, esto basta:
RUN npm run build

# ---- runtime ----
FROM nginx:alpine
ARG APP_NAME=banking-app
# Copia el artefacto angular
COPY --from=build /app/dist/${APP_NAME} /usr/share/nginx/html
# (Opcional) Nginx conf personalizada para SPA
# COPY ./nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
