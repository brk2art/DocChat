import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    // During development, the page runs on :5173 and forwards /ask to the API, so both look like one site.
    proxy: { '/ask': 'http://localhost:5023' },
  },
  build: {
    // The built page goes into the API's wwwroot: in production the API serves it.
    outDir: '../DocChat.Api/wwwroot',
    emptyOutDir: true,
  },
})