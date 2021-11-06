import Vue from 'vue';
import App from './App.vue';
import Web3Api from './utils/web3.service'

Web3Api.init();

Vue.config.productionTip = true;
 
new Vue({
    created() {
        let urlParams = new URLSearchParams(window.location.search);
        console.log(urlParams.has('message')); // true
        console.log(urlParams.get('message')); // "MyParam"
      },
    render: h => h(App)
})
.$mount('#app');
