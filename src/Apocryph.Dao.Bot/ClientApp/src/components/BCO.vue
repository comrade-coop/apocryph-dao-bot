<template>
  <section>
    <div class="terminal-nav">
      <div class="terminal-logo">
        <div style="white-space: nowrap">
          <b>Bounded Curve Offering</b>
        </div>
      </div>
      <nav class="terminal-menu">
        <ul>
          <li>
            <label for="price">Available: </label>
            {{ toCurrency(balance) }} CRYPTH
          </li>
        </ul>
      </nav>
    </div>
  </section>
  <section>
    <div class="button-grid">
      <div class="form-group">
        <BcoTrade
          title="Sell"
          buttonStyle="btn btn-primary btn-ghost"
          :getPrice="getSellPriceCallback"
          :getTotalAmount="getSellTotalPriceCallback"
          :trade="sellCallback"
          :bus="bus"
        >
        </BcoTrade>
      </div>
      <div class="form-group">
        <BcoTrade
          title="Buy"
          buttonStyle="btn btn-error btn-ghost"
          :getPrice="getBuyPriceCallback"
          :getTotalAmount="getBuyTotalPriceCallback"
          :trade="buyCallback"
          :bus="bus"
        >
        </BcoTrade>
      </div>
    </div>
    <br />
  </section>
</template>

<script>
import * as ethers from "ethers";
import BcoTrade from "./BcoTrade";
export default {
  name: "BCO",
  components: {
    BcoTrade,
  },
  emits: ["reload"],
  setup() {
    const provider = new ethers.providers.Web3Provider(window.ethereum, "any");
    const signer = provider.getSigner();

    const abi = JSON.stringify(require("../abi/BondingCurve.json").abi);
    const contract = new ethers.Contract(
      process.env.VUE_APP_BCO_ADDRESS,
      abi,
      signer
    );

    const filter = {
      address: process.env.VUE_APP_BCO_ADDRESS,
      topics: [
        ethers.utils.id("Buy(address,uint256,uint256)"),
        ethers.utils.id("Sell(address,uint256,uint256)"),
      ],
    };
    provider.on(filter, () => {
      this.bus.$emit("reload");
    });

    const erc20Abi = JSON.stringify(require("../abi/ERC20.json"));
    const tokenBContract = new ethers.Contract(
      process.env.VUE_APP_BCO_TOKENB_ADDRESS,
      erc20Abi,
      provider
    );

    return {
      contract,
      tokenBContract,
      provider,
      signer,
      abi,
    };
  },
  data() {
    return {
      balance: 0,
      decimals: 0,
      bus: window.vue,
    };
  },
  methods: {
    toCurrency(value) {
      return value.toLocaleString("en-US", {
        maximumFractionDigits: 2,
      });
    },
    async calculateBuyTotalAmountCallback() {
      this.totalBuyAmount = 0;
      this.showBuyTotalAmountSpinner = true;
      setTimeout(() => {
        this.totalBuyAmount = this.buyPrice * this.buyAmount;
        this.showBuyTotalAmountSpinner = false;
      }, 3000);
    },
    async buyCallback() {
      await this.provider.send("eth_requestAccounts", []); // do we need this here?
      const address = await this.signer.getAddress();

      const tokenBBalance = (
        await this.tokenBContract.balanceOf(this.signer.address)
      ).toString();

      const amount = this.amount; // TODO: convert this.amount to uint256
      await this.contract.buy(amount, tokenBBalance, address);
    },
    async sellCallback() {
      await this.provider.send("eth_requestAccounts", []); // do we need this here?
      const address = await this.signer.getAddress();

      const tokenBBalance = (
        await this.tokenBContract.balanceOf(this.signer.address)
      ).toString();

      const amount = this.amount; // TODO: convert this.amount to uint256
      await this.contract.buy(amount, tokenBBalance, address);
    },
    async getBuyPriceCallback() {
      const price = await this.contract.getBuyPrice(1);
      const result = ethers.FixedNumber.fromValue(price, 0).toString();
      return result;
    },
    async getBuyTotalPriceCallback(amount) {
      const price = await this.contract.getBuyPrice(amount);
      const result = ethers.FixedNumber.fromValue(price, 0).toString();
      return result;
    },
    async getSellPriceCallback() {
      const price = await this.contract.getSellPrice(1);
      const result = ethers.FixedNumber.fromValue(price, 0).toString();
      return result;
    },
    async getSellTotalPriceCallback(amount) {
      const price = await this.contract.getSellPrice(amount);
      const result = ethers.FixedNumber.fromValue(price, 0).toString();
      return result;
    },
  },
  async mounted() {
    this.decimals = await this.contract.priceDivisor();
    const balance = await this.contract.balanceA();
    this.balance = ethers.FixedNumber.fromValue(balance, 10).toString();

    this.bus.$on("reload", async () => {
      const balance = await this.contract.balanceA();
      this.balance = ethers.FixedNumber.fromValue(balance, 10).toString();
    });
  },
};
</script>

<style>
#totalAmountSpinner {
  height: 30px;
  width: 30px;
  padding: 0;
  margin: 0;
}

.trade-panel input {
  width: 80%;
}

.trade-panel span {
  padding-left: 16px;
}

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
</style>
