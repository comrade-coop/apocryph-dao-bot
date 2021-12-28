require('dotenv').config()
require("@nomiclabs/hardhat-waffle");
 
module.exports = {
  solidity: "0.8.4",
  networks: {}
};

if (process.env.POLYGON_PRIVATE_KEY) {
  module.exports.networks.polygon_mumbai = {
    url: `${process.env.POLYGON_URL}`,
    chainId: 80001,
    gas: 'auto',
    gasPrice: 'auto',
    accounts: [`0x${process.env.POLYGON_PRIVATE_KEY}`],
    timeout: 20000
  }
}