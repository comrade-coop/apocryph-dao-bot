🚧 _(This project is under active development, please check below for updates)_ 🚧

# Apocryph DAO Bot
Discord bot for interacting with [Apocryph DAO](https://github.com/comrade-coop/apocryph-dao)
running on Ethereum. You can use the bot to seamlessly 
query information about the DAO, vote, buy / sell tokens on 
the bonding curve and more relevant activities.

## Getting Started
You can interact with Apocryph DAO Bot in [Apocryph Discord server](https://discord.gg/C4e37Xhvt4).
The bot responds on direct messages and also post various updates
in the following channels:
 - `#voting` channel
 - `#events` channel

To engage the bot, you have to:
1. Join Apocryph Discord server; 
2. Have a valid Ethereum address;
3. Have a browser with configured compatible wallet like: MetaMask, WalletConnect and others;
4. Introduce yourself to the bot. 

### Introduction to the bot
To fully benefit from interactions with the bot, you have to introduce
yourself to the bot. This process is essentially mapping your
Ethereum address to your Discord identifier.

To do this follow the steps below:
1. Open a direct message to the both
2. Enter the following slash command: `/introduce <your-ethereum-address>`
3. The bot will respond with a link.
4. Open the link in a browser with your wallet configured.
5. The link will open a small user interface for signing a message.
6. Copy the signed message back and send it back to the bot using `/confirm <signed-message>`

## Interactions with the bot
In addition to receiving various notification, you can also interact
with the bot using direct messages.

### Execute basic queries
Open a direct message session with the bot and use the follwoing
slash commands to:
1. `/balance` to get your token balance 
2. `/weight` to get your voting weight

### Voting process
When a voting is initiated the bot post a message containing
the details of the voting in the `#voting` channel.

The message also contains two links, for voting "yes" or "no".
In order to vote you have to:
1. Open the link corresponding to your intetion in a browser with your wallet configured
2. Sign the transaction representing your vote
3. Close the link and observe `#events` channel to see a confirmation of your vote

When the voting completes, the bot will edit the message in the `#voting` channel to
remove the voting links and present the results from the vote.

### Buy tokens on the bonding curve
As part of Apocryph DAO you can buy tokens on a bonding curve.
You can do this using direct messages with the bot, in the following way:
1. Send `/approve <ammount>` to allow certain amount of tokens 
from your address to be used by the DAO;
2. The bot will then send you back a link to sign the transaction for
the approval.
3. Send `/buy` to open the trading user interface
4. The bot will send you back a link to a user interface for buying 
token on specific levels. 

## Under the hood
The bot is implemented in C# (.NET 5.0) and [Perper](https://github.com/obecto/perper) - 
the same technology that powers up Apocryph Core.

To find out more about the bot architecture, please check our 
[architecture documentation](docs/architecture-overview.md)

## Contributing
If you are interested in contributing to Apocryph project, please
check our [project board](https://github.com/orgs/comrade-coop/projects/1) 
for issues open for contributors. Please make sure to check our 
project [contribution guidelines](CONTRIBUTING.md), beforehand.  
