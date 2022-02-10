<template>

	<section>
		<div class="terminal-nav">
			<header>
				<h2 style="width: 300px;">Bounded Curve Offering</h2>
			</header>
			<nav class="terminal-menu">
				<ul>
					<li>
						Available {{balance}} / {{totalBalance}} CRYPTH Tokens
					</li>
				</ul>
			</nav>
		</div>
	</section>


	<section>
		<div class="button-grid">
			<div class="form-group">

				<form>
					<fieldset>
						<legend>BUY</legend>
						<div class="form-group trade-panel">
							<label for="sellPrice">Price: {{buyPrice}} DAI</label>
						</div>

						<div class="form-group trade-panel">
							<label for="buyAmount">Amount:</label>
							<div>
								<input type="number" id="buyAmount" name="buyAmount" v-model="buyAmount" />
								<span>CRYPTH</span>
							</div>
						</div>

						<div class="form-group trade-panel">
							<label>Total: {{totalBuyAmount}} DAI</label>
						</div>

						<div class="form-group">
							<button class="btn btn-primary" role="button" name="buy" id="buy" @click="buy">
								BUY
							</button>
						</div>
					</fieldset>
				</form>
			</div>
			<div class="form-group">
				<form>
					<fieldset>
						<legend>SELL</legend>
						<div class="form-group trade-panel">
							<label for="sellPrice">Price: {{sellPrice}} DAI</label>
						</div>
						<div class="form-group trade-panel">
							<label for="sellAmount">Amount:</label>
							<div>
								<input type="number" id="sellAmount" name="sellAmount" v-model="sellAmount" />
								<span>CRYPTH</span>
							</div>
						</div>
						<div class="form-group trade-panel">
							<label>Total: {{totalSellAmount}} DAI</label>
						</div>
						<div class="form-group">
							<button class="btn btn-error" role="button" name="sell" id="sell" @click="sell">
								SELL
							</button>
						</div>
					</fieldset>
				</form>
			</div>
		</div>
		<br />
	</section>

</template>

<script>
	import * as ethers from "ethers"

	export default {
		name: "BCO",
		setup() {
			const provider = new ethers.providers.Web3Provider(window.ethereum, "any")
			const signer = provider.getSigner()
			const abi = JSON.stringify(
				require("../abi/BondingCurve.json").abi
			)
			const contract = new ethers.Contract(
				process.env.VUE_APP_BCO_ADDRESS,
				abi,
				signer
			);

			return {
				contract,
				provider,
				signer,
				abi
			}
		},
		data() {
			return {
				buyPrice: 0.03,
				sellPrice: 0.03,
				sellAmount: 0,
				buyAmount: 0,
				balance: null,
				availableTokens: null
			}
		},
		computed: {
			totalBuyAmount() {
				return this.buyPrice * this.buyAmount;
			},
			totalSellAmount() {
				return this.sellPrice * this.sellAmount;
			},

		},
		methods: {

			async buy(e) {
				if (e) e.preventDefault()
				console.log(this)

				await this.provider.send("eth_requestAccounts", [])
				//var vm = this
			},
			async sell(e) {
				if (e) e.preventDefault()
				//var vm = this
			}
		},
		async mounted() {
			const totalBalance = await this.contract.totalBalanceA()
			const balance = await this.contract.balanceA()
			this.totalBalance = ethers.FixedNumber.fromValue(totalBalance, 10).toString()
			this.balance = ethers.FixedNumber.fromValue(balance, 10).toString()

			try {
				var b1 = await this.contract.calculateDueBalanceB(balance + 1)
				console.log(b1)

				var b2 = await this.contract.calculateDueBalanceB(balance - 1)
				console.log(b2)

				var price = b1 - b2
				console.log(price)
			} catch (err) {
				if (err.data) { // smart contract errors
					if (err.data.message.startsWith("VM")) {
						var hex = err.data.data.substring(9, err.data.data.length)
						console.log(ethers.utils.toUtf8String(hex))
					}
				}
			}


			//  price = calculateDueBalanceB(x+1) - calculateDueBalanceB(x-1)

		}
	}
</script>

<style>
	.trade-panel input {
		width: 74%;
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
		grid-template-columns: repeat(auto-fit,
				minmax(calc(var(--page-width) / 12), 1fr));
	}
</style>
