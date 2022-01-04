import 'terminal.css'
import 'web3'
import 'web3modal'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import VueIpfs from './plugins/vue-ipfs'
import Web3Service from "@/services/web3.service";

Web3Service.init();
 
createApp(App)
.use(router)
.use(VueIpfs)
.mount('#app')
