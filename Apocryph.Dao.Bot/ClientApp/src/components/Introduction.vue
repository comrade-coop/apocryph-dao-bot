<template>
 <section>
  
  <form>
    <fieldset>
      <legend>Address introduction</legend>
      <div class="form-group">
        <label for="message">Message:</label>
        <input id="message" name="message" type="text"
               v-model="message" disabled>
      </div>
       <div class="form-group">
        <label for="signedMessage">Signed message:</label>
        <input id="signedMessage" name="signedMessage" type="text"
               v-model="signedMessage" disabled>
      </div>
      
      <div class="form-group">
      <div class="button-grid">
        <button class="btn btn-default" role="button" name="connectMetamask" id="connectMetamask" 
                @click="onConnect"
                v-if="showConnectMetamask">Connect MetaMask</button>
        
        <button class="btn btn-default" role="button" name="signMessage" id="signMessage"
                @click="onMessageSign"
                v-if="showSignMessage">Sign Message</button>

        <div class="terminal-alert terminal-alert-primary"  v-if="!showSignMessage && !showConnectMetamask">Message signed, check private conversation with the bot</div>
        
      </div>
      
      </div>

    </fieldset>
  </form>
</section>
</template>

<script>
import Web3Service from "../services/web3.service"
import axios from 'axios'

export default {
  name: 'Introduction',
  setup(){
    Web3Service.init();
    axios.defaults.baseURL = process.env.VUE_APP_BASE_API_URL;
    console.log(axios.defaults.baseURL);
  },
  data() {
    return {
      connected: false,
      signed: false,
      signedMessage: null
    }
  },
  computed: {
    session() {
      return this.$route.params.session;
    },
    message() {
      return this.$route.params.message;
    },
    showConnectMetamask() {
      return this.connected === false;
    },
    showSignMessage() {
      return this.connected === true && this.signed === false;
    }
  },
  methods:{
    async onConnect(e) {
      if(e) e.preventDefault();
      
      this.connected = await Web3Service.connect();
    },
    async onMessageSign(e)  {
      if(e) e.preventDefault();
      
      const vm = this;
      
      this.signedMessage = await Web3Service.sign(this.message);
      
      if (this.signedMessage) {
        axios.post('/api/webinput', {
          session: vm.session,
          message: `/connect ${vm.signedMessage}`
        })
        .then(() => {
          this.signed = true; 
        })
        .catch(function (error) {
          this.signed = false;
          alert(error);
        });
      }
    }
  }
}
</script>

<style scoped>
 .button {
    background-color: #bbb;
    display: block;
    margin: 10px 0;
    padding: 10px;
    width: 100%;
}

 .button:active:hover:not([disabled]) {
   /*your styles*/
 }
 
</style>
