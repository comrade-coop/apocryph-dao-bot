<template>
  <section>
    <form>
      <fieldset>
        <legend>Vote Proposal Creation</legend>
        <div class="form-group">
          <label for="contractAddress">Contract address:</label>
          <input
            type="text"
            id="contractAddress"
            name="contractAddress"
            v-model="contractAddress"
            :disabled="success"
          />
        </div>

        <div class="form-group">
          <div class="button-grid">
            <div class="form-group">
              <label for="expiration">Expiration block:</label>
              <input
                id="expirationBlock"
                name="expirationBlock"
                type="number"
                v-model="expirationBlock"
                :disabled="success"
              />
            </div>
          </div>
        </div>

        <div class="form-group">
          <div class="button-grid">
            <div class="form-group">
              <label for="expiration">Title:</label>
              <input
                id="title"
                name="title"
                type="text"
                v-model="title"
                :disabled="success"
              />
            </div>
          </div>
        </div>

        <div class="form-group">
          <label for="description">Description:</label>
          <textarea
            id="description"
            name="description"
            rows="10"
            v-model="description"
            :disabled="success"
          />
        </div>

        <div class="form-group">
          <label for="contractAddress">Actions bytes:</label>
          <textarea
            type="text"
            id="actionsBytes"
            name="actionsBytes"
            rows="10"
            v-model="actionsBytes"
            :disabled="success"
          />
        </div>

        <div class="form-group">
          <div class="button-grid">
            <button
              class="btn btn-default"
              role="button"
              name="createVoteProposal"
              id="createVoteProposal"
              v-if="success == error"
              @click="createVoteProposal"
            >
              Create vote proposal
            </button>
          </div>
        </div>

        <div class="form-group">
          <div class="terminal-alert terminal-alert-primary" v-if="success">
            Vote proposal created
          </div>
          <div class="terminal-alert terminal-alert-error" v-if="error">
            <dl>Vote proposal creation failed</dl>
          </div>
        </div>
      </fieldset>
    </form>
  </section>
</template>
 
<script>
import * as ethers from "ethers";
import axios from "axios";

export default {
  name: "VoteCreate",
  setup() {
    axios.defaults.baseURL = process.env.VUE_APP_BASE_API_URL;
  },
  data() {
    return {
      error: false,
      success: false,
    };
  },
  methods: {
    async createVoteProposal(e) {
      if (e) e.preventDefault();
      var vm = this;
      try {
        const provider = new ethers.providers.Web3Provider(
          window.ethereum,
          "any"
        );
        await provider.send("eth_requestAccounts", []);
        const signer = provider.getSigner();

        const abi = JSON.stringify(
          require("../abi/DeadlineQuorumVoting.json").abi
        );
        const votingContract = new ethers.Contract(
          vm.contractAddress,
          abi,
          signer
        );

        let response = await axios
          .post(
            "api/ipfs/proposal",
             {
              contractAddress: vm.contractAddress,
              expirationBlock: vm.expirationBlock,
              title: vm.title,
              description: vm.description,
              actionsHash: vm.actionsBytes,
            } 
          )
          .catch(function (error) {
            this.error = true;
            alert(error);
          });

        console.log(response.data.cid)

        const rationaleHash = ethers.utils.hexlify(
          ethers.utils.base58.decode(response.data.cid).slice(2)
        );

        const actionsHash = ethers.utils.keccak256(vm.actionsBytes);
        await votingContract.propose(rationaleHash, actionsHash);

        vm.success = true;

      } catch (err) {
        console.error(err);
        // vm.error = true;
      }
    },
  },
};
</script>