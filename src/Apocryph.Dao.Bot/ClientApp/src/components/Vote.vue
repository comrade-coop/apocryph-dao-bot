<template>
  <section>
    <form>
      <fieldset>
        <legend>{{ title }}</legend>

        <div class="form-group">
          <label for="expiration">Expiration block: </label>
          <span> {{expirationBlock}}</span>
        </div>

        <div class="form-group">
          <label>Description:</label>
          <p>{{ description }}</p>
        </div>

        <div class="form-group">
          <label>Actions:</label>
          <p class="break-word-container">{{ actionsHash }}</p>
        </div>
        
        <div class="form-group">
          <div class="button-grid">
            <button
              class="btn btn-default btn-ghost"
              role="button"
              name="voteNo"
              id="votePass"
              v-if="showVotingButtons"
              @click="vote(0, $event)"
            >
              Pass
            </button>

            <button
              class="btn btn-primary"
              role="button"
              name="voteYes"
              id="voteYes"
              v-if="showVotingButtons"
              @click="vote(1, $event)"
            >
              Yes
            </button>

            <button
              class="btn btn-error"
              role="button"
              name="voteNo"
              id="voteNo"
              v-if="showVotingButtons"
              @click="vote(2, $event)"
            >
              No
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
 <style>
 label {
   font-weight: bold;
 }
.break-word-container {
    display: table;
    table-layout: fixed;
    width: 100%;
    word-wrap: break-word;
}
</style>
<script>
import * as ethers from "ethers";
import axios from "axios";

export default {
  name: "Vote",
  setup() {
    axios.defaults.baseURL = process.env.VUE_APP_BASE_API_URL;
    const provider = new ethers.providers.Web3Provider(window.ethereum, "any");
    const abi = JSON.stringify(require("../abi/DeadlineVoting.json").abi);
    const signer = provider.getSigner();
    return {
      provider,
      signer,
      abi,
    };
  },
  data() {
    return {
      contractAddress: null,
      success: false,
      error: false,
      expirationBlock: null,
      title: null,
      description: null,
      actionsHash: null,
    };
  },
  computed: {
    voteId() {
      return this.$route.params.voteId;
    },
    showVotingButtons() {
      return this.success == false && this.error == false;
    },
  },
  methods: {
    async vote(option, e) {
      if (e) e.preventDefault()
      const vm = this;

      try {

        await this.provider.send("eth_requestAccounts", [])
        const votingContract = new ethers.Contract(
                  vm.contractAddress,
                  this.abi,
                  this.signer)

        await votingContract.vote(this.voteId(), option)
        vm.success = true

      } catch (err) {
        console.error(err)
        vm.error = true
      }
    },
    async initialize() {
      const vm = this;
      const all = require("it-all")
      const { concat: uint8ArrayConcat } = require("uint8arrays/concat")
      const { toString: uint8ArrayToString } = require("uint8arrays/to-string")
      const cid = this.$route.params.cid
      const ipfs = await this.$ipfs

      try {

        const data = uint8ArrayConcat(await all(ipfs.cat(cid)));
        const json = JSON.parse(uint8ArrayToString(data));

        vm.contractAddress = json.contractAddress
        vm.expirationBlock = json.expirationBlock
        vm.title = json.title
        vm.description = json.description
        vm.actionsHash = json.actionsHash

      } catch (err) {
        console.error(err);
      }
    }
  },
  watch: {
    $route: "initialize",
  },
  created() {
    this.initialize();
  },
};
</script>