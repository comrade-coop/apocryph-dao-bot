import 'terminal.css'
import 'web3'
import 'web3modal'
import {
	createApp
} from 'vue'
import App from './App.vue'
import router from './router'
import $bus from './services/event';

const app = createApp(App).use(router)
app.config.globalProperties.$bus = $bus;
app.mount('#app')