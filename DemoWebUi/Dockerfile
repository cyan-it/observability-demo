FROM node:21-alpine as build-stage

WORKDIR /app
COPY package*.json /app/
RUN npm install
COPY . /app
RUN npm run build

FROM nginx:alpine

COPY --from=build-stage /app/dist/demo-web-ui/browser /usr/share/nginx/html
COPY src/runtime-config.js /usr/share/nginx/html/runtime-config.js
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
