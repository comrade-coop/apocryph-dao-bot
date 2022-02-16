<template>
	<form>
		<fieldset>
			<legend>{{title}}</legend>
			<div class="form-group trade-panel">
				<label for="price">Price: </label> {{toCurrency(price)}} DAI
			</div>
			<div class="form-group trade-panel">
				<label for="amount">Amount:</label>
				<div>
					<input type="number" id="amount" name="amount" v-model="amount" v-on:input="calculateTotalAmount" />
					<span>CRYPTH</span>
				</div>
			</div>
			<div class="form-group trade-panel">
				<label>Total: </label><span> ~{{toCurrency(totalAmount)}} DAI </span>
				<i class="fa-solid fa-spinner fa-spin" v-show="showTotalAmountSpinner"></i>
			</div>
			<div class="form-group">
				<button v-bind:class="buttonStyle" role="button" name="sell" id="sell" @click="buttonClick">
					{{title.toUpperCase()}}
				</button>
			</div>
		</fieldset>
	</form>
</template>

<script>
	import debounce from "lodash.debounce";
	export default {
		name: "BcoTrade",
		props: {
			title: String,
			buttonStyle: String,
			getPrice: Function,
			getTotalAmount: Function,
			trade: Function,
			bus: null
		},
		setup() {

		},
		data() {
			return {
				price: 0,
				amount: 0,
				showTotalAmount: false,
				totalAmount: 0,
				showTotalAmountSpinner: false
			}
		},
		methods: {
			toCurrency(value) {
				return value.toLocaleString('en-US', {
					maximumFractionDigits: 2
				})
			},
			async buttonClick(e) {
				if (e) e.preventDefault()
				await this.trade(this.amount)
			}
		},
		async created() {
			this.calculateTotalAmount = debounce(async() => {
				this.totalAmount = 0
				this.showTotalAmountSpinner = true
				setTimeout(async () => {
					this.totalAmount = await this.getPrice(this.amount)
					this.showTotalAmountSpinner = false
				}, 500)
			}, 1000)
		},
		async mounted() {
			this.price = await this.getPrice(1)
			this.bus.$on('reload', async () => {
				this.price = await this.getPrice(1)
			})
		},
		async beforeUnmount(){
			this.calculateTotalAmount.cancel()
		}
	}
</script>

<style>
</style>
