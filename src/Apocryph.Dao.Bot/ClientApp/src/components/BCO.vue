<template>
	<section>
		<div class="terminal-nav">
			<div class="terminal-logo">
				<div style="white-space: nowrap;">
					<b>Bounded Curve Offering</b>
				</div>
			</div>
			<nav class="terminal-menu">
				<ul>
					<li>
						<label for="price">Available: </label> {{toCurrency(balance)}} CRYPTH
					</li>
				</ul>
			</nav>
		</div>
	</section>
	<section>
		<div class="button-grid">
			<div class="form-group">
				<BcoTrade title="Sell" buttonStyle="btn btn-primary btn-ghost" :getPrice="getSellPriceCallback" :getTotalAmount="getSellTotalPriceCallback"
					:trade="sellCallback">
				</BcoTrade>
			</div>
			<div class="form-group">
				<BcoTrade title="Buy" buttonStyle="btn btn-error btn-ghost" :getPrice="getBuyPriceCallback" :getTotalAmount="getBuyTotalPriceCallback"
					:trade="buyCallback">
				</BcoTrade>
			</div>
		</div>
		<br />
	</section>
</template>

<script>
	import * as ethers from "ethers"
	import BcoTrade from './BcoTrade'
	export default {
		name: "BCO",
		components: {
			BcoTrade
		},
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
				balance: 0,
				decimals: 0
			}
		},
		methods: {
			toCurrency(value) {
				return value.toLocaleString('en-US', {
					maximumFractionDigits: 2
				})
			},
			async calculateBuyTotalAmountCallback() {
				this.totalBuyAmount = 0
				this.showBuyTotalAmountSpinner = true
				setTimeout(() => {
					this.totalBuyAmount = this.buyPrice * this.buyAmount
					this.showBuyTotalAmountSpinner = false
				}, 3000);
			},
			async buyCallback() {
				//if (e) e.preventDefault()
				//console.log(this)

				await this.provider.send("eth_requestAccounts", [])
				//var vm = this
			},
			async sellCallback() {
				//if (e) e.preventDefault()
				//var vm = this
			},
			async getBuyPriceCallback() {
				const price = await this.contract.getBuyPrice(1)
				const result = ethers.FixedNumber.fromValue(price, 0).toString()
				return result
			},
			async getBuyTotalPriceCallback(amount) {
				const price = await this.contract.getBuyPrice(amount)
				const result = ethers.FixedNumber.fromValue(price, 0).toString()
				return result
			},
			async getSellPriceCallback() {
				const price = await this.contract.getSellPrice(1)
				const result = ethers.FixedNumber.fromValue(price, 0).toString()
				return result
			},
			async getSellTotalPriceCallback(amount) {
				const price = await this.contract.getSellPrice(amount)
				const result = ethers.FixedNumber.fromValue(price, 0).toString()
				return result
			}
		},
		async mounted() {

			this.decimals =  await this.contract.priceDivisor()
			const balance = await this.contract.balanceA()
			this.balance = ethers.FixedNumber.fromValue(balance, 10).toString()

			//const totalBalance = await this.contract.totalBalanceA()
			//this.totalBalance = ethers.FixedNumber.fromValue(totalBalance, decimals).toString()
			// x = bco.balanceA(); price = calculateDueBalanceB(x) - calculateDueBalanceB(x-buyAmount)

			//const initialPrice = 0.03
			//const finalPrice = 1
			//const tokenASupply = await this.contract.totalBalanceA()
			//const lastValue = await this.contract.calculateDueBalanceB(tokenASupply)
			//const value = await this.contract.calculateDueBalanceB(tokenASupply.add(-1000))
			//console.log(value.toBigInt())
			//console.log(lastValue.toBigInt())
			//const price = lastValue.toBigInt() - value.toBigInt()
			//console.log(price)

			//console.log(ethers.FixedNumber.fromValue(price, decimals).toString())
			//console.log(ethers.FixedNumber.fromValue(lastValue, decimals).toString())
			//console.log(ethers.FixedNumber.fromValue(value, decimals).toString())






			//const interpolatedPrice1 = (initialPrice + (finalPrice - initialPrice) * 1 / tokenASupply)
			//const interpolatedPrice2 = (initialPrice + (finalPrice - initialPrice) * 2 / tokenASupply)
			//const interpolatedPrice3 = (initialPrice + (finalPrice - initialPrice) * 3 / tokenASupply)

			//console.log(ethers.FixedNumber.fromValue(interpolatedPrice1).toString())
			//console.log(ethers.FixedNumber.fromValue(interpolatedPrice2, decimals).toString())
			//console.log(ethers.FixedNumber.fromValue(interpolatedPrice3, decimals).toString())



			try {

				/*
				
				const value = await bondingCurve.calculateDueBalanceB(tokenASupply - i)
				          if (lastValue !== undefined) {
				            const price = (lastValue - value)
				            const interpolatedPrice = (initialPrice + (finalPrice - initialPrice) * i / tokenASupply)
				            expect(Math.abs(price - interpolatedPrice)).to.be.at.most(1)
				
				*/
				//var b1 = await this.contract.calculateDueBalanceB(balance.add(1))
				//var b2 = await this.contract.calculateDueBalanceB(balance.add(-1))
				//const price = b1.toBigInt() - b2.toBigInt()

				//this.buyPrice = ethers.FixedNumber.fromValue(price).toString()
				// console.log(ethers.FixedNumber.fromValue(b2, decimals).toString())



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
		grid-template-columns: repeat(auto-fit,
				minmax(calc(var(--page-width) / 12), 1fr));
	}
</style>
