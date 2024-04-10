[Baking Bad](https://github.com/baking-bad) team glad to introduce shooter demo game build with Unity
and [Tezos Unity SDK](https://github.com/trilitech/tezos-unity-sdk)

## Gamers readme

The game is single-player shooter with survival elements.
The game process starts with authentication through Tezos wallet or Kukai embed social login.

<p align="center">
  <img width="600" src="https://github.com/baking-bad/tezos-unity-game/blob/feat/docs/images/connect.png?raw=true">
</p>

After connected, the user will be prompted to sign game-generated payload in order to make sure that user have access
to his address private keys. After success authentication user can start game.

<p align="center">
  <img width="800" src="https://github.com/baking-bad/tezos-unity-game/blob/feat/docs/images/game.png?raw=true">
</p>

Player spawns with one low-DPS default weapon (Walky, #4). The goal of the game is to survive as long, as
possible. Current player HP displays on top left corner (#1). During game session different amount and types of mobs
will be spawned depend on current threat value (#3). There's a certain chance of falling out some drop after killing a
mob: additional HP, armor and various ammo. Additionally to common creeps at every 10 waves Boss will be spawned - mega
creep with increased HP, current level of which you can see on #2. Аfter killing a boss, there's a chance on drop
another more powerful weapons and NFT. 
All NFT's are [FA2](https://gitlab.com/tezos/tzip/-/blob/master/proposals/tzip-12/tzip-12.md) tokens and has their own
different drop chances, because all of them can enhance player in-game stats.
For example Armor increases max HP level by 10%. Here is the full list of all available NFT's with their drop chances:

| Token      | Type      |                                    Description                                     |                   Drop chance |
|------------|-----------|:----------------------------------------------------------------------------------:|------------------------------:|
| Armor      | Armor     |                           Increases armor level by 10%.                            | Drops 1 time from first  boss |
| Viper      | Gun       |  Advanced light caliber handgun. Features increased fire rate and clip capacity.   |                         7.55% |
| Claw       | Gun       |              A heavy assault pistol. High recoil and one-off damage.               |                         7.55% |
| Defender   | Shotgun   |           A weighted version of the basic shotgun. increased ammunition.           |                         7.55% |
| DoomGuy    | Shotgun   |                           A good old demon-killing gun.                            |                         7.55% |
| Peacock    | SMG       |           A perfect tool to destroy predators. Or to become the one of.            |                         3.77% |
| Sealer     | SMG       | High-tech space marine assault rifle. Perfect firepower and ergonomic performance. |                         3.77% |
| ZOOOKA     | Explosive |               The big bad boom is a proven way to get rid of demons.               |                         1.89% |
| 	Roaster   | Explosive |      A scaled down version of the standard mines. Provides more ease of use.       |                         1.89% |
| 	Haste 1   | Module    | Improved servo drives for the armored suit. Increases movement speed level by 5%.  |                        11.32% |
| 	Haste 2   | Module    | Improved servo drives for the armored suit. Increases movement speed level by 10%. |                         7.55% |
| 	Haste 3   | Module    | Improved servo drives for the armored suit. Increases movement speed level by 15%. |                         3.77% |
| 	Robust    | Module    |  Vitality module for the armored suit. Increases the initial health level by 10%.  |                        11.32% |
| 	Robust 2  | Module    |  Vitality module for the armored suit. Increases the initial health level by 15%.  |                         7.55% |
| 	Robust 3  | Module    |  Vitality module for the armored suit. Increases the initial health level by 20%.  |                         3.77% |
| 	Precise 1 | Module    |            Combat module for the armored suit. Increases damage by 5%.             |                         7.55% |
| 	Precise 2 | Module    |            Combat module for the armored suit. Increases damage by 10%.            |                         3.77% |
| 	Precise 3 | Module    |            Combat module for the armored suit. Increases damage by 15%.            |                         1.89% |

After player killed boss and NFT dropped from him, claim rewards button will appear in main menu with total NFT's amount
available.

<p align="center">
  <img width="400" src="https://github.com/baking-bad/tezos-unity-game/blob/feat/docs/images/claim.png?raw=true">
</p>

After click on claim and solving captcha, game server will send NFT to player's Tezos address and after approximately 30
seconds player will be notified that token-transfer operation successfully completed with hash:

<p align="center">
  <img width="800" src="https://github.com/baking-bad/tezos-unity-game/blob/feat/docs/images/success-operation.png?raw=true">
</p>

Players can copy provided operation hash and see more details about it on https://tzkt.io/{operationHash}

Next, players will be able to see all their NFT's on inventory page and enhance players stats by equipping tokens with
drag and drop from inventory tab to left. Note: equipped state did not persistently saved after game tab reloading, so
make sure that all your tokens are equipped after such cases.

<p align="center">
  <img width="800" src="https://github.com/baking-bad/tezos-unity-game/blob/feat/docs/images/armor.png?raw=true">
</p>


When equipped, players can see how this tokens enhance players properties in
effects tab.

<p align="center">
  <img width="800" src="https://github.com/baking-bad/tezos-unity-game/blob/feat/docs/images/stats.png?raw=true">
</p>

And last tab - players own stats reduced from all matches.

## Developers docs

This section will describe how core features was
developed using [Tezos Unity SDK](https://github.com/trilitech/tezos-unity-sdk).
First of all, we connected SDK through Unity package manager using git url:

<p align="center">
  <img width="800" src="https://github.com/baking-bad/tezos-unity-game/blob/feat/docs/images/package-manager.png?raw=true">
</p>

After waiting a while for the dependencies to be resolved we need to drag and drop 2 prefabs on our game scene:
`TezosManager` and `TezosAuthenticator` which are located in `Runtime/Prefabs` SDK directory.

Adding `TezosManager` on our scene creates `TezosManager` singleton object which is entrypoint to whole SDK features.
For example here in `UserDataManager` we are subscribing to SDK events:

* WalletConnected
* WalletDisconnected
* PayloadSigned
* ContractCallCompleted

https://github.com/baking-bad/tezos-unity-game/blob/bc2b576c7172f798a06f9be985f6fb44b817401a/Assets/Scripts/Managers/UserDataManager.cs#L67-L70

`UserDataManager` also is a game singleton object that stores all user-specific data in game, for example:

* List of user tokens
* List of contract tokens
* List of rewards (Nft's that user able to claim)

So, this collections filling in with appropriate values after `WalletConnected`, `ContractCallCompleted` SDK events in
`LoadGameNfts` method.

https://github.com/baking-bad/tezos-unity-game/blob/bc2b576c7172f798a06f9be985f6fb44b817401a/Assets/Scripts/Managers/UserDataManager.cs#L193
https://github.com/baking-bad/tezos-unity-game/blob/bc2b576c7172f798a06f9be985f6fb44b817401a/Assets/Scripts/Managers/UserDataManager.cs#L249
https://github.com/baking-bad/tezos-unity-game/blob/bc2b576c7172f798a06f9be985f6fb44b817401a/Assets/Scripts/Managers/UserDataManager.cs#L286

Also, this game implement transfer token feature, which is executed when double-clicking on tokens in inventory tab:

<p align="center">
  <img width="800" src="https://github.com/baking-bad/tezos-unity-game/blob/feat/docs/images/transfer.png?raw=true">
</p>

This actions performs easily with next SDK code:

https://github.com/baking-bad/tezos-unity-game/blob/cad51934c4decf8652b9f57381268019f71e6eca/Assets/Scripts/Managers/UserDataManager.cs#L320

That's pretty much it, more detailed SDK docs [available here](https://opentezos.com/gaming/unity-sdk/).