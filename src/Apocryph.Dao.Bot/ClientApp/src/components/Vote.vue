<template>
  <section>
    <form>
      <fieldset>
        <legend>{{ title }}</legend>

        <div class="form-group">
          <label for="expiration">Expiration block: </label>
          <span> {{ expirationBlock }}</span>
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

            <button
              class="btn btn-primary"
              role="button"
              name="enact"
              id="enact"
              @click="enact($event)"
            >
              Enact
            </button>

            <div class="terminal-alert terminal-alert-primary" v-if="success">
              Thank you for voting
            </div>
            <div class="terminal-alert terminal-alert-error" v-if="error">
              {{errorMessage}}
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
    const abi = JSON.stringify(require("../abi/DeadlineQuorumVoting.json").abi);
    const signer = provider.getSigner();
    return {
      provider,
      signer,
      abi,
    };
  },
  data() {
    return {
      voteId: null,
      contractAddress: null,
      success: false,
      error: false,
      errorMessage: "Voting failed",
      expirationBlock: null,
      title: null,
      description: null,
      actionsHash: null,
      isVotable: false,
      isEnactable: false,
    };
  },
  computed: {
    showVotingButtons() {
      if (this.isVotable) {
        return this.success == false && this.error == false;
      }
      return false;
    },
  },
  methods: {
    async vote(option, e) {
      if (e) e.preventDefault();
      const vm = this;

      try {
        await this.provider.send("eth_requestAccounts", []);
        const votingContract = new ethers.Contract(
          vm.contractAddress,
          this.abi,
          this.signer
        );

        await votingContract.vote(vm.voteId, option);
        vm.success = true;
      } catch (err) {
        var rawMessage  = err.data.message
        if(rawMessage.startsWith('Reverted'))
          rawMessage  = ethers.utils.toUtf8String(rawMessage.substring(9, rawMessage.length))
        vm.errorMessage = rawMessage
        vm.error = true;
      }
    },
    async enact(e) {
      if (e) e.preventDefault();
      const vm = this;

      try {
        await this.provider.send("eth_requestAccounts", []);
        const votingContract = new ethers.Contract(
          vm.contractAddress,
          this.abi,
          this.signer
        );

        await votingContract.enact(vm.voteId);
        vm.success = true;
      } catch (err) {
        console.error(err);
        vm.error = true;
      }
    },
    async initButtons() {
      const vm = this;
      /*const votingContract = new ethers.Contract(
        vm.contractAddress,
        this.abi,
        this.signer
      );*/
      vm.isVotable = true //await votingContract.isVotable(vm.voteId);
      vm.isEnactable = true //await votingContract.isEnactable(vm.voteId);
    },
    async initVm() {
      const vm = this;
      const cid = this.$route.params.cid;

      try {
        let json = await axios
          .get(`api/ipfs/proposal?cid=${cid}`)
          .catch(function (error) {
            console.error(error);
          });

        vm.voteId = this.$route.params.voteId
     

        vm.contractAddress = json.data.contractAddress;
        vm.expirationBlock = json.data.expirationBlock;
        vm.title = json.data.title;
        vm.description = json.data.description;
        vm.actionsHash = json.data.actionsHash;
      } catch (err) {
        console.error(err);
      }
    },
  },
  watch: {
    $route: "initialize",
  },
  async created() {
    await this.initVm();
    await this.initButtons();
  },
};
</script>