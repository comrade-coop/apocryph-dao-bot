<template>
  <section>
    <form>
      <fieldset>
        <legend>Sign address</legend>
        <div class="form-group">
          <label for="address">Address:</label>
          <label id="address">{{ address }} </label>
        </div>
        <div class="form-group">
          <label for="signedAddress">Signature:</label>
          <label id="signedAddress">{{ signature }}</label>
        </div>
        <div class="form-group">
          <div class="button-grid">
            <button
              class="btn btn-default"
              role="button"
              name="signMessage"
              id="signMessage"
              @click="onAddressSign"
              v-if="showSignAddress"
            >
              Sign Address
            </button>

            <div class="terminal-alert terminal-alert-primary" v-if="success">
              Address signed, check private conversation with the bot
            </div>
            <div class="terminal-alert terminal-alert-error" v-if="error">
              Failed to verify address
            </div>
          </div>
        </div>
      </fieldset>
    </form>
  </section>
</template>
 
<script>
import * as ethers from "ethers";
import signalR from "@/services/signalr";
import axios from "axios";

export default {
  name: "SignAddress",
  setup() {
    axios.defaults.baseURL = process.env.VUE_APP_BASE_API_URL;
  },
  data() {
    return {
      showSignAddress: true,
      error: false,
      success: false,
      signature: null
    };
  },
  computed: {
    session() {
      return this.$route.params.session;
    },
    address() {
      return this.$route.params.address;
    }
  },
  methods: {
    async onAddressSign(e) {
      if (e) e.preventDefault();

      const vm = this;

      const provider = new ethers.providers.Web3Provider(
        window.ethereum,
        "any"
      );

      await provider.send("eth_requestAccounts", []);
      const signer = provider.getSigner();

      this.signedAddress = await signer.signMessage(this.address);

      if (this.signedAddress) {
        axios
          .post("/api/webinput/intro-attempt", {
            session: vm.session,
            signedAddress: vm.signedAddress,
            address: vm.address,
          })
          .catch(function (error) {
            this.error = true;
            alert(error)
          });

          this.showSignAddress = false
      }
    },
    initialize() {
      const vm = this;
      signalR.connect(
        process.env.VUE_APP_BASE_API_URL,
        this.$route.params.session
      );
      signalR.connection.on("onError", () => {
        vm.error = true;
      });

      signalR.connection.on("onSuccess", () => {
        vm.success = true;
      });
    },
  },
  watch: {
    $route: "initialize",
  },
  created() {
    this.initialize();
  },
};
</script>