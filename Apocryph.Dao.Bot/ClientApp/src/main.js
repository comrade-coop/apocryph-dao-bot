import 'terminal.css'
import 'web3'
import 'web3modal'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import Web3Service from "@/services/web3.service";
//import axios from "axios";

Web3Service.init();

//axios.defaults.baseURL = process.env.VUE_APP_BASE_API_URL;
//console.log(axios.defaults.baseURL);

createApp(App).use(router).mount('#app')
