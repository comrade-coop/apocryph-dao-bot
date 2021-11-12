<template>
 <section>
  
  <form>
    <fieldset>
      <legend>Sign address</legend>
      <div class="form-group">
        <label for="address">Address:</label>
        <input id="address" name="address" type="text" v-model="address" disabled>
      </div>
       <div class="form-group">
        <label for="signedAddress">Signed address:</label>
        <input id="signedAddress" name="signedAddress" type="text"
               v-model="signedAddress" disabled>
      </div>
      
      <div class="form-group">
      <div class="button-grid">
        <button class="btn btn-default" role="button" name="connectMetamask" id="connectMetamask" 
                @click="onConnect"
                v-if="showConnectMetamask">Connect MetaMask</button>
        
        <button class="btn btn-default" role="button" name="signMessage" id="signMessage"
                @click="onAddressSign"
                v-if="showSignAddress">Sign Address</button>

        <div class="terminal-alert terminal-alert-primary"  v-if="success">Address signed, check private conversation with the bot</div>
        <div class="terminal-alert terminal-alert-error"  v-if="error">Failed to verify address</div>
        
      </div>
      
      </div>

    </fieldset>
  </form>
</section>
</template>

 
<script>
import Web3Service from "../services/web3.service"
//import SignalRService from "../services/signalr.service"
import axios from 'axios'
//import { useRoute } from 'vue-router'
import {onMounted} from "vue";
import SignalRService from "@/services/signalr.service";

//const route = useRoute();
//const session = route.params.session;
axios.defaults.baseURL = process.env.VUE_APP_BASE_API_URL;
//SignalRService.connect(process.env.VUE_APP_BASE_API_URL, session);
// general problem, i need session and vue instance to start singlarR

export default {
  name: 'SignAddress',
  setup() {
    
    
    onMounted(() => {
      Web3Service.init();
    });
    
  },
  data() {
    return {
      connected: false,
      signed: false,
      signedAddress: null,
      success: false,
      error: false
    }
  },
  computed: {
    session() {
      return this.$route.params.session;
    },
    address() {
      return this.$route.params.address;
    },
    showConnectMetamask() {
      return this.connected === false;
    },
    showSignAddress() {
      return this.connected === true && this.signed === false;
    }
  },
  methods:{
    async onConnect(e) {
      if(e) e.preventDefault();
      
      this.connected = await Web3Service.connect();
    },
    async onAddressSign(e)  {
      if(e) e.preventDefault();
      
      const vm = this;
      
      this.signedAddress = await Web3Service.sign(this.address);
      
      if (this.signedAddress) {
        axios.post('/api/webinput', {
          session: vm.session,
          signedAddress: vm.signedAddress,
          address: vm.address
        })
        .then(() => {
          this.signed = true; 
        })
        .catch(function (error) {
          this.signed = false;
          alert(error);
        });
      }
    },
    onError() {
      this.error = true;
      this.success = false;
    },
    onSuccess() {
      this.error = false;
      this.success = true;
    },
    initialize() {
      console.log("initialize: " + this.$route.params.session);
     
      SignalRService.connect(process.env.VUE_APP_BASE_API_URL, this.$route.params.session);
      SignalRService.subscribe(this, "onError");
      SignalRService.subscribe(this, "onSuccess");
    }
  },
  watch: {
    '$route': 'initialize'
  },
  created() {
    this.initialize();
  },
}
</script>