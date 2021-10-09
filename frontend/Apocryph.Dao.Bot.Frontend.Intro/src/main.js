import Vue from 'vue';
import App from './App.vue';
import Web3Api from './utils/web3.service'

Web3Api.init();

Vue.config.productionTip = true;

new Vue({
    render: h => h(App)
}).$mount('#app');
