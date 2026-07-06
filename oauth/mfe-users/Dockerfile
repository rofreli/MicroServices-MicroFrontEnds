FROM node:20-alpine AS build
WORKDIR /app
# Base URL of the BFF gateway (the only backend the browser talks to).
ARG VITE_OAUTH_API_URL=http://localhost:5002
ENV VITE_OAUTH_API_URL=$VITE_OAUTH_API_URL
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
