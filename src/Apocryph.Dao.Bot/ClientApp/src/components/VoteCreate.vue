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
              <label for="title">Title:</label>
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
          <label for="actionsBytes">Actions Bytes:</label>
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
              @click="createVoteProposal"
              v-if="!success"
            >
              Create vote proposal
            </button>
          </div>
        </div>

        <div class="form-group">
          <div class="terminal-alert terminal-alert-primary" v-if="success==true">
            Vote proposal created
          </div>
          <div class="terminal-alert terminal-alert-error" v-if="success==false">
            <dl>{{ errorMessage }}</dl>
          </div>
        </div>
      </fieldset>
    </form>
  </section>
</template>
 
<script>
import * as ethers from "ethers"
import axios from "axios"

export default {
  name: "VoteCreate",
  setup() {
    axios.defaults.baseURL = process.env.VUE_APP_BASE_API_URL
  },
  data() {
    return {
     success: null,
      errorMessage: "Vote proposal creation failed"
    }
  },
  methods: {
    async createVoteProposal(e) {
      if (e) e.preventDefault()
      var vm = this

      try {
        const provider = new ethers.providers.Web3Provider(
          window.ethereum,
          "any"
        )
        await provider.send("eth_requestAccounts", [])
        const signer = provider.getSigner()

        const abi = JSON.stringify(
          require("../abi/DeadlineQuorumVoting.json").abi
        )

        const votingContract = new ethers.Contract(
          vm.contractAddress,
          abi,
          signer
        )

        const actionsHash = ethers.utils.keccak256(vm.actionsBytes)

        let response = await axios
          .post("api/ipfs/proposal", {
            contractAddress: vm.contractAddress,
            title: vm.title,
            description: vm.description,
            actionsHash: actionsHash,
            actionsBytes: vm.actionsBytes,
          })
          .catch(function (err) {
            this.success = false
            console.error(err)
          })

        console.log(response.data.cid)

        const rationaleHash = ethers.utils.hexlify(
          ethers.utils.base58.decode(response.data.cid).slice(2)
        )

        await votingContract.propose(rationaleHash, actionsHash)

        vm.success = true

      } catch (err) {

        vm.errorMessage = "Vote proposal creation failed"
        if (err.data) { // smart contract errors
          if (err.data.message.startsWith("Reverted")) {
            vm.errorMessage = ethers.utils.toUtf8String(
              err.data.message.substring(9, err.data.message.length)
            )
          }
        } else if (err.message) { // MetaMask errors
          vm.errorMessage = err.message
        } 

        vm.success = false
        console.error(err)
      }
    },
  }
}
</script>