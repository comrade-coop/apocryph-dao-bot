<template>
 <section>
  
  <form action="#" data-form-type="other">
    <fieldset>
      <legend>Address introduction</legend>
      <div class="form-group">
        <label for="message">Message:</label>
        <input id="message" name="message" type="text" value="{{msg}}" disabled>
      </div>

       <div class="form-group">
        <label for="signedMessage">Signed message:</label>
        <input id="signedMessage" name="signedMessage" type="text" value="{{signedMessage}}" disabled>
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
  props: {
    msg: String
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
 .button {
    background-color: #bbb;
    display: block;
    margin: 10px 0;
    padding: 10px;
    width: 100%;
}
</style>
