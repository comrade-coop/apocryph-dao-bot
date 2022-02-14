import 'terminal.css'
import 'web3'
import 'web3modal'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

createApp(App)
.use(router)
.mount('#app')
