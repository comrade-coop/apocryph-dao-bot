import Web3Modal from 'web3modal';
import Web3 from "web3";
import {convertUtf8ToHex} from "@walletconnect/utils"
import VotingAbi from "./voting.abi"

const Web3Service = {
    web3Modal: null,
    provider: null,
    web3: null,

    init() {
        console.log("Initializing example YEAH!");

        // Tell Web3modal what providers we have available.
        // Built-in web browser provider (only one can exist as a time)
        // like MetaMask, Brave or Opera is added automatically by Web3modal
        const providerOptions = {
        };

        this.web3Modal = new Web3Modal({
            cacheProvider: false, // optional
            providerOptions, // required
            disableInjectedProvider: false, // optional. For MetaMask / Brave / Opera.
        });

        console.log("Web3Modal instance is", this.web3Modal);
    },

    async connect() {
        console.log("Opening a dialog", this.web3Modal);
        try {
            this.provider = await this.web3Modal.connect();
            console.log("Connection established!");
            this.web3 = new Web3(this.provider);
            return true;
        } catch (e) {
            console.log("Could not get a wallet connection", e);
            return false;
        }
    },

    async sign(message) {
        console.log("Signing a message", this.web3Modal);

        // encode message (hex)
        const hexMsg = convertUtf8ToHex(message);

        try {
            const accounts = await this.web3.eth.getAccounts();
            const address = accounts[0];
            // send message
            const result = await this.web3.eth.personal.sign(hexMsg, address);

            console.log("Signed message", result)
            return result;
        } catch (error) {
            console.error(error); // tslint:disable-line
        }
    },


    async vote(voteId, voteStatus) {
        console.log("Voting");

        try {
            const contractAddress = process.env.VUE_APP_VOTING_CONTRACT;
            const abi = VotingAbi._abi;
            const voteContract = new this.web3.eth.Contract(abi, contractAddress);
            
            voteContract.methods.vote(voteId, voteStatus).call().then(console.log);

        } catch (error) {
            console.error(error); // tslint:disable-line
        }
    },

    async voteActive(voteId) {

        console.log("Voting");

        try {
            const contractAddress = process.env.VUE_APP_VOTING_CONTRACT;
            const voteContract = new this.web3.eth.Contract(VotingAbi.abi, contractAddress);

            return await voteContract.methods.voteActive(voteId);

        } catch (error) {
            console.error(error); // tslint:disable-line
            return false;
        }
    }
};

export default Web3Service;