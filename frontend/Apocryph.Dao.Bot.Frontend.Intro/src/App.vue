<template>
    <div id="app">
        <p>{{ signedMessage }}</p>
        <button @click.prevent="onConnect()">Connect</button>
        <button @click.prevent="onSign()">Sign</button>
    </div>
</template>

<script>
    import Web3Service from "@/utils/web3.service"

    export default {
        name: 'app',
        data: function () {
            return {
                signedMessage: ''
            }
            
        },
        methods: {
            onConnect() {
                Web3Service.connect();
            },
            onSign() {
                let urlParams = new URLSearchParams(window.location.search)
                
                if (urlParams.has('message')) {
                    const message = urlParams.get('message')
                    this.signedMessage = Web3Service.sign(message);
                }
            }
        }
    };
</script>

<style>
</style>

