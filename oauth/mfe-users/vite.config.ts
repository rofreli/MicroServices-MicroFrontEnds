import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import federation from '@originjs/vite-plugin-federation'

export default defineConfig({
  plugins: [
    react(),
    federation({
      name: 'mfeUsers',
      filename: 'remoteEntry.js',
      exposes: {
        './App': './src/App',
        './routes': './src/routes',
      },
      shared: ['react', 'react-dom', 'react-router-dom'],
    }),
  ],
  build: { target: 'esnext', minify: false, cssCodeSplit: false },
  server: { port: 3002, cors: true },
  preview: { port: 3002, cors: true },
})
