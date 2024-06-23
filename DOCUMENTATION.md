The [Baking Bad](https://github.com/baking-bad) team is glad to introduce this demo game built with Unity
and the [Tezos Unity SDK](https://github.com/trilitech/tezos-unity-sdk).

## Gamers readme

The game is a single-player shooter with survival elements.
The game process starts with authentication through a Tezos wallet.
You can use a wallet in a browser extension or mobile app such as [Temple](https://templewallet.com/) or the [Kukai](https://wallet.kukai.app/) social wallet.

<p align="center">
  <img width="600" src="https://github.com/baking-bad/tezos-unity-game/blob/master/images/connect.png?raw=true">
</p>

The game prompts you to connect your wallet.
Then to verify that you have the private keys for the account, it prompts you to sign a payload of random text.
The game does not require any payment.

## Developer information

The following sections provide information about how the game is set up so you can use it as a model for your own games.
For more developer information about the Tezos Unity SDK, see https://docs.tezos.com/unity.

### Authentication

The game uses the user's Tezos account as a source of authentication.
It prompts the user to connect their Tezos wallet so it can retrieve the user's account address.
For more information about connecting to user wallets, see [Connecting accounts](https://docs.tezos.com/unity/connecting-accounts) in the SDK documentation.

When the wallet is connected, the game prompts the user to sign a payload to prove that they have the key for the account.
The process follows these general steps:

1. The Unity client passes the public key of the user's account to the  backend's [/payload/get/](https://game.baking-bad.org/back/api/payload/get/) endpoint.
2. The backend generates a random payload string with this code: https://github.com/k-karuna/tezos_game_back/blob/e6bc9c021b86704ec1ce1b5e3fd799977d05034f/api/views.py#L44 and returns it to the Unity client.
3. The Unity game client calls the SDK `RequestSignPayload` method with the payload: https://github.com/baking-bad/tezos-unity-game/blob/7e3fb6454896896f7e0ac77f09d2b5f02e104aa7/Assets/Scripts/Managers/UserDataManager.cs#L108.
4. The user signs the payload in their Tezos wallet.
5. The SDK triggers the `PayloadSigned` event and the SDK handler receives the signed payload: https://github.com/baking-bad/tezos-unity-game/blob/7e3fb6454896896f7e0ac77f09d2b5f02e104aa7/Assets/Scripts/Managers/UserDataManager.cs#L76.
6. The game client passes the signed payload to the backend's [/payload/verify/](https://game.baking-bad.org/back/api/payload/verify/) endpoint.
7. The backend performs signature validation with the provided `public_key` and `signature` parameters and returns a success or failure message to the Unity client.
8. If the validation succeeded, the user can start playing the game.

### Backend API

The backend accepts requests from the game client through a REST API.
Its specs are available here: https://game.baking-bad.org/back/swagger/.
All corresponding handlers for each HTTP endpoint can be found here: https://github.com/k-karuna/tezos_game_back/blob/main/api/views.py

### Game description

<p align="center">
  <img width="800" src="https://github.com/baking-bad/tezos-unity-game/blob/master/images/game.png?raw=true">
</p>

The player spawns with one low-DPS default weapon (Walky), which is shown as the active weapon (#1 in the annotated screencapture above). The goal of the game is to survive as long as possible.

Controls list:

| Type         | Type       |
|--------------|------------|
| Moving       | W, A, S, D |
| Shot         | Left mouse |
| Swap gun     | E          |
| Acceleration | Space      |
| Pause menu   | Esc        |

Once every 4 seconds, the player can use acceleration; its cooldown displays in #2.
The current player's HP appears in the top left corner (#3).
After killing a mob, sometimes a powerup appears: additional HP, shield
(damage immunity), and various ammo.
Additionally, at every 10 waves a boss is spawned - a mega creep
with increased HP, the current level of which you can see on #4.

After killing a boss, there's a chance that more powerful gear drops.
This gear is represented by Tezos [FA2](https://gitlab.com/tezos/tzip/-/blob/master/proposals/tzip-12/tzip-12.md) tokens and each has their own different drop chances, because all of them
can enhance player in-game stats in different ways. For example, armor reduces the damage received by the player by 10%. Here is the full
list of all available tokens with their drop chances:

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

When the player kills a boss that drops a token, the claim rewards button appears in the main menu:

<p align="center">
  <img width="400" src="https://github.com/baking-bad/tezos-unity-game/blob/master/images/claim.png?raw=true">
</p>

After the player clicks the button and solves a captcha, the game client triggers the backend to send the tokens to the player's Tezos address.
After approximately 30 seconds, the player is notified that the token transfer operation successfully completed and the UI shows the hash of the operation:

<p align="center">
  <img width="800" src="https://github.com/baking-bad/tezos-unity-game/blob/master/images/success-operation.png?raw=true">
</p>

Players can copy the provided operation hash and see more details about it in a block explorer, such as https://tzkt.io/{operationHash}.

Next, players can see all their tokens on the inventory page and enhance their stats by dragging them from the inventory to their character.
Note: The character's loadout is not saved after the game client reloads, so players must make sure to equip their tokens before each game.

<p align="center">
  <img width="800" src="https://github.com/baking-bad/tezos-unity-game/blob/master/images/armor.png?raw=true">
</p>

When equipped, players can see how their tokens enhance their character's properties in the effects tab.

<p align="center">
  <img width="800" src="https://github.com/baking-bad/tezos-unity-game/blob/master/images/effects.png?raw=true">
</p>

The last tab shows the player's all-time stats, which are stored in hte backend database.

<p align="center">
  <img width="800" src="https://github.com/baking-bad/tezos-unity-game/blob/master/images/stats.png?raw=true">
</p>

### Token management

The backend uses a smart contract to manage tokens: https://tzkt.io/KT1TSZfPJ5uZW1GjcnXmvt1npAQ2nh5S1FAj/operations/.
This token contract is compliant with the [FA2](https://gitlab.com/tezos/tzip/-/blob/master/proposals/tzip-12/tzip-12.md) standard, which allows other applications such as wallets and block explorers to show information about them and handle transfers.

The contract pre-minted 1000 of each type of token.
For example, you can see the total supply of the Armor token here:
https://tzkt.io/KT1TSZfPJ5uZW1GjcnXmvt1npAQ2nh5S1FAj/tokens/1/holders.
When the user clicks the `Claim Reward` button in the main menu, the game client sends an HTTP request to https://game.baking-bad.org/back/api/drop/transfer/
with the solved captcha and the Tezos address.
Then the backend calls the contract's `transfer` entrypoint from
the contract administrator account, whose private key is stored on the backend.
The contract updates its ledger to show that the user account now has one token of the specified token type.

## Developers docs

This section describes how core features were developed using the [Tezos Unity SDK](https://github.com/trilitech/tezos-unity-sdk).

First of all, we installed the SDK through the Unity package manager using the git URL of the SDK:

<p align="center">
  <img width="800" src="https://github.com/baking-bad/tezos-unity-game/blob/master/images/package-manager.png?raw=true">
</p>

After waiting a while for the dependencies to be resolved we dragged and dropped 2 prefabs on our game scene:
`TezosManager` and `TezosAuthenticator` which are located in the `Runtime/Prefabs` SDK directory.

Adding `TezosManager` to our scene creates the `TezosManager` singleton object, which is the entrypoint to the SDK features.
For example, here in `UserDataManager` the application subscribes to these SDK events:

* WalletConnected
* WalletDisconnected
* PayloadSigned
* ContractCallCompleted

https://github.com/baking-bad/tezos-unity-game/blob/bc2b576c7172f798a06f9be985f6fb44b817401a/Assets/Scripts/Managers/UserDataManager.cs#L67-L70

`UserDataManager` is a game singleton object that stores all user-specific data in the game, including:

* List of user tokens
* List of contract tokens
* List of rewards (tokens that the user is able to claim)

This object fills in appropriate values after the `WalletConnected`, `ContractCallCompleted` SDK events in the `LoadGameNfts` method.
Here is the relevant code:

https://github.com/baking-bad/tezos-unity-game/blob/bc2b576c7172f798a06f9be985f6fb44b817401a/Assets/Scripts/Managers/UserDataManager.cs#L193
https://github.com/baking-bad/tezos-unity-game/blob/bc2b576c7172f798a06f9be985f6fb44b817401a/Assets/Scripts/Managers/UserDataManager.cs#L249
https://github.com/baking-bad/tezos-unity-game/blob/bc2b576c7172f798a06f9be985f6fb44b817401a/Assets/Scripts/Managers/UserDataManager.cs#L286

Also, the game allows users to transfer tokens by double-clicking on tokens in the inventory tab:

<p align="center">
  <img width="800" src="https://github.com/baking-bad/tezos-unity-game/blob/master/images/transfer.png?raw=true">
</p>

The game uses the [`TokenContract`](https://docs.tezos.com/unity/reference/TokenContract) object's [`transfer`](https://docs.tezos.com/unity/reference/TokenContract#transfer) method to transfer tokens:

https://github.com/baking-bad/tezos-unity-game/blob/cad51934c4decf8652b9f57381268019f71e6eca/Assets/Scripts/Managers/UserDataManager.cs#L320

For more detailed information on using the SDK, see [Tezos Unity SDK](https://docs.tezos.com/unity).
