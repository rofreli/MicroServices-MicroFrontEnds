import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import federation from '@originjs/vite-plugin-federation'

const MFE_BU_URL = process.env.VITE_MFE_BUSINESS_UNITS_URL ?? 'http://localhost:3001'
const MFE_USERS_URL = process.env.VITE_MFE_USERS_URL ?? 'http://localhost:3002'

export default defineConfig({
  plugins: [
    react(),
    federation({
      name: 'shell',
      remotes: {
        mfeBusinessUnits: {
          external: `${MFE_BU_URL}/assets/remoteEntry.js`,
          from: 'vite',
          externalType: 'url',
        },
        mfeUsers: {
          external: `${MFE_USERS_URL}/assets/remoteEntry.js`,
          from: 'vite',
          externalType: 'url',
        },
      },
      shared: ['react', 'react-dom', 'react-router-dom'],
    }),
  ],
  build: { target: 'esnext', minify: false, cssCodeSplit: false },
  server: { port: 3000 },
})
