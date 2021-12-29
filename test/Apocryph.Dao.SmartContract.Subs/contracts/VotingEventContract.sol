//SPDX-License-Identifier: Unlicense
pragma solidity ^0.8.0;

contract VotingEventContract {

    enum VoteStatus { Nil, Yes, No }

    event Proposal(uint256 indexed voteId);
    function emitProposal(uint256 voteId) public {
       emit Proposal(voteId);
    }

    event Enaction(uint256 indexed voteId);
    function emitEnaction(uint256 voteId) public {
       emit Enaction(voteId);
    }

    event Vote(uint256 indexed voteId, address indexed voter, VoteStatus value);
    function emitVote(uint256 voteId, address voter, VoteStatus value) public {
       emit Vote(voteId, voter, value);
    }
}
