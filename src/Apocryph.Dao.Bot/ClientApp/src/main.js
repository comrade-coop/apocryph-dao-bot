import 'terminal.css'
import 'web3'
import 'web3modal'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import VueIpfs from './plugins/vue-ipfs'

createApp(App)
.use(router)
.use(VueIpfs, { url: process.env.VUE_APP_IPFS_URL})
.mount('#app')
