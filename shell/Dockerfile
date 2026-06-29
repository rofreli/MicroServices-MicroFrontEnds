FROM node:20-alpine AS build
WORKDIR /app

ARG VITE_MFE_BUSINESS_UNITS_URL=http://localhost:3001
ARG VITE_MFE_USERS_URL=http://localhost:3002
ARG VITE_OAUTH_URL=http://localhost:5001
ENV VITE_MFE_BUSINESS_UNITS_URL=$VITE_MFE_BUSINESS_UNITS_URL
ENV VITE_MFE_USERS_URL=$VITE_MFE_USERS_URL
ENV VITE_OAUTH_URL=$VITE_OAUTH_URL

COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
