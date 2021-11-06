<template>
 <section>
  
  <form action="#" data-form-type="other">
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
        <button class="btn btn-default btn-ghost" role="button" name="connectMetamask" id="connectMetamask">Connect MetaMask</button>
        <button class="btn btn-default btn-ghost" role="button" name="signMessage" id="signMessage">Sign Message</button>
      </div>
      
      </div>

    </fieldset>
  </form>
</section>
</template>

<script>

export default {
  name: 'Introduction',
  data() {
    return {
      signedMessage: null
    }
  },
  computed: {
    message() {
      return this.$route.params.message;
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
