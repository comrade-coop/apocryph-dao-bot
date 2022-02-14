<template>
  <div class="container">
    <main>
      <h1 class="terminal-prompt">APOCRYPH DAO</h1>
      <router-view/>
      <hr>
    </main>
  </div>
</template>

<style>
.components-grid {
  display: grid;
  grid-column-gap: 1.4em;
  grid-template-columns: auto;
  grid-template-rows: auto;
}

.button-grid {
  display: grid;
  grid-template-rows: auto;
  grid-gap: 1em;
  grid-template-columns: repeat(
        auto-fit,
        minmax(calc(var(--page-width) / 12), 1fr)
      );
}

@media only screen and (min-width: 70em) {
  .components-grid {
    grid-template-columns: 3fr 9fr;
  }
}
</style>
<script>
export default {
  name: "App",
  async created() {
    if(!window.ethereum) {
      alert("Please install MetaMask browser extension")
    }
  }
}
</script>