const hre = require("hardhat");
const { expect } = require("chai");

async function main() {
  const EventContract = await hre.ethers.getContractFactory("VotingEventContract");
  const eventContract = await EventContract.deploy();
  await eventContract.deployed();

  console.log("VotingEventContract deployed to:", eventContract.address);

  // After deployment execution
  await expect(eventContract.emitProposal(10)).to.emit(eventContract, 'Proposal').withArgs(10);
  await expect(eventContract.emitProposal(11)).to.emit(eventContract, 'Proposal').withArgs(11);

  await expect(eventContract.emitEnaction(13)).to.emit(eventContract, 'Enaction').withArgs(13);
  await expect(eventContract.emitEnaction(14)).to.emit(eventContract, 'Enaction').withArgs(14);

  await expect(eventContract.emitVote(100, '0x307c60a4C8648F82C3F3a5243e563052178554c7', 2))
      .to.emit(eventContract, 'Vote').withArgs(100, '0x307c60a4C8648F82C3F3a5243e563052178554c7', 2);

      await expect(eventContract.emitVote(110, '0x307c60a4C8648F82C3F3a5243e563052178554c7', 1))
      .to.emit(eventContract, 'Vote').withArgs(110, '0x307c60a4C8648F82C3F3a5243e563052178554c7', 1);
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
