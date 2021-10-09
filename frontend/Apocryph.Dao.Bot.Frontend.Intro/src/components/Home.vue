<template>
    <div class="home">
        <h1>{{ msg }}</h1>
        <p>Welcome to your new single-page application, built with <a href="https://vuejs.org" target="_blank">Vue.js</a>.</p>
        <button @click.prevent="onConnect()">Connect</button>
        <button @click.prevent="onSign()">Sign</button>
    </div>
</template>

<script>
    import Web3Service from "@/utils/web3.service"

    export default {
        name: 'Home',
        props: {
            msg: String
        },
        methods: {
            onConnect() {
                Web3Service.connect();
            },

            onSign() {
                Web3Service.sign();
            }
        }
    };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>

