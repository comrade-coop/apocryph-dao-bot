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
        <button class="btn btn-default btn-ghost" role="button" name="connectMetamask" id="connectMetamask" @click="onConnect">Connect MetaMask</button>
        <button class="btn btn-default btn-ghost" role="button" name="signMessage" id="signMessage" @click="onMessageSign">Sign Message</button>
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
  },
  data() {
    return {
      signedMessage: null,
      isConnected: false,
      isMessageSigned: false
    }
  },
  computed: {
    session() {
      return this.$route.params.session;
    },
    message() {
      return this.$route.params.message;
    }
  },
  methods:{
    async onConnect(e) {
      if(e) e.preventDefault();
      this.isConnected = await Web3Service.connect();
    },
    async onMessageSign(e)  {
      if(e) e.preventDefault();
      var vm = this;
      
      this.signedMessage = await Web3Service.sign(this.message);
      
      if(this.signedMessage) {
        this.isMessageSigned = true;
        axios.post('/api/webinput', {
          session: vm.session,
          message: `/connect ${vm.signedMessage}`
        })
        .then(() => {
          this.isMessageSigned = true; 
        })
        .catch(function (error) {
          this.isMessageSigned = false;
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
</style>
