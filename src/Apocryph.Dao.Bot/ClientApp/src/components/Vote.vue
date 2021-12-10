<template>
  <section>
    <form>
      <fieldset>
        <legend>Vote {{ voteId }}</legend>
        <div class="form-group">
          <label for="address">Description:</label>
          <input
            id="description"
            name="description"
            type="text"
            v-model="description"
            disabled
          />
        </div>

        <div class="form-group">
          <label for="link">Link:</label>
          <input id="link" name="link" type="text" v-model="link" disabled />
        </div>

        <div class="form-group">
          <label for="type">Type:</label>
          <input id="type" name="type" type="text" v-model="type" disabled />
        </div>

        <div class="form-group">
          <label for="expiration">Expiration:</label>
          <input
            id="expiration"
            name="expiration"
            type="text"
            v-model="expiration"
            disabled
          />
        </div>

        <div class="form-group">
          <div class="button-grid">
            <button
              class="btn btn-default"
              role="button"
              name="connectMetamask"
              id="connectMetamask"
              @click="onConnect"
              v-if="showConnectMetamask"
            >
              Connect MetaMask
            </button>

            <button
              class="btn btn-primary"
              role="button"
              name="voteYes"
              id="voteYes"
              v-if="showVotingButtons"
              @click="voteYes"
            >
              Yes
            </button>

            <button
              class="btn btn-error"
              role="button"
              name="voteNo"
              id="voteNo"
              v-if="showVotingButtons"
              @click="voteNo"
            >
              No
            </button>

            <button
              class="btn btn-default btn-ghost"
              role="button"
              name="voteNo"
              id="voteNo"
              v-if="showVotingButtons"
              @click="votePass"
            >
              Pass
            </button>

            <div class="terminal-alert terminal-alert-primary" v-if="success">
              Thank you for voting
            </div>
            <div class="terminal-alert terminal-alert-error" v-if="error">
              Voting failed, or you voted already
            </div>
          </div>
        </div>
      </fieldset>
    </form>
  </section>
</template>
 
<script>
import { onMounted } from "vue";
import Web3Service from "../services/web3.service";
import signalR from "@/services/signalr";
import axios from "axios";

export default {
  name: "SignAddress",
  setup() {
    axios.defaults.baseURL = process.env.VUE_APP_BASE_API_URL;
    onMounted(() => {
      Web3Service.init();
    });
  },
  data() {
    return {
      connected: false,
      success: false,
      error: false,
    };
  },
  computed: {
    session() {
      return this.$route.params.session;
    },
    voteId() {
      return this.$route.params.voteId;
    },
    showConnectMetamask() {
      return this.connected === false;
    },
    showVotingButtons() {
      return this.connected === true;
    },
  },
  methods: {
    async onConnect(e) {
      if (e) e.preventDefault();

      this.connected = await Web3Service.connect();
    },
    async voteYes() {
      const vm = this;
      await Web3Service.vote(vm.voteId, 1);
    },
    async voteNo() {
      const vm = this;
      await Web3Service.vote(vm.voteId, 2);
    },
    async votePass() {
      const vm = this;
      await Web3Service.vote(vm.voteId, 0);
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
  }
};
</script>