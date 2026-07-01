import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
// The `test` block configures Vitest; cast is needed because Vite's own
// UserConfig type doesn't know about Vitest's config extension.
export default defineConfig({
  plugins: [react()],
  test: {
    environment: 'jsdom',
    setupFiles: ['./src/setupTests.ts'],
    globals: true,
  },
} as ReturnType<typeof defineConfig>)
