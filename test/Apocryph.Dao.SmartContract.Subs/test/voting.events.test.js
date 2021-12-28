const { expect } = require("chai");
const { ethers } = require("hardhat");

describe("VotingEventContract", function () {

  const contractName = "VotingEventContract";

  async function deployContract(name) {
    const Contract = await hre.ethers.getContractFactory(name);
    const contract = await Contract.deploy();
    await contract.deployed();
    return contract;
  }

  it("Should emit Proposal event", async function () {
    const contract = await deployContract(contractName);
    await expect(contract.emitProposal(10))
      .to.emit(contract, 'Proposal').withArgs(10);
  });

  it("Should emit Vote event", async function () {
    const contract = await deployContract(contractName);
    await expect(contract.emitVote(100, '0x307c60a4C8648F82C3F3a5243e563052178554c7', 2))
      .to.emit(contract, 'Vote').withArgs(100, '0x307c60a4C8648F82C3F3a5243e563052178554c7', 2);
  });
});
