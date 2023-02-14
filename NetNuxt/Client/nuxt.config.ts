// https://nuxt.com/docs/api/configuration/nuxt-config
import VitePluginCompression from 'vite-plugin-compression'
import { resolve } from 'path'

export default defineNuxtConfig({
  vite: {
    build: {
      outDir: resolve(resolve(__dirname, '..'), 'wwwroot')
    },
    plugins: [
      VitePluginCompression()
    ]
  },
  nitro: {
    output: {
      // dir: resolve(resolve(__dirname, '..'), 'wwwroot'),
      publicDir: resolve(resolve(__dirname, '..'), 'wwwroot'),
      // serverDir: resolve(resolve(__dirname, '..'), 'wwwroot'),
    }
  }
})
