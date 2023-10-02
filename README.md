# mevsharp
[![Tests](https://github.com/mevsharp/mevsharp/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/mevsharp/mevsharp/actions/workflows/tests.yml)

[![Follow us on Twitter](https://img.shields.io/twitter/follow/mevsharp?style=social&label=Follow)](https://twitter.com/mevsharp)
[![Chat on Discord](https://img.shields.io/discord/1153016005866561688?style=social&logo=discord)](https://discord.gg/qCeecx2sTw)

mevsharp is a highly configurable MEV middleware built to improve anti-censoring and increase client diversity. mevsharp is built on .NET and runs on Linux, Windows, macOS x64 and has Linux/macOS ARM64 support.


## Installation
```
git clone https://github.com/mevsharp/mevsharp/
```


### Docker
It is recommended to run mevsharp in a docker container.
To install and run mevsharp through docker, you can use this (adjust relays):
```
docker pull mevsharp/mevsharp:latest
docker run --rm -d -p 18550:18550 --name mevsharp mevsharp/mevsharp:latest --network goerli --relay https://publickey@relay1.example --relay https://publickey@relay2.example --listen http://0.0.0.0:18550
```

To verify that everything started up correctly, you can run:
```
docker logs mevsharp -f
```

mevsharp is now be now accessible for your validator on port 18550:
```
mevsharp@mevsharptest01:~$ curl -i http://localhost:18550/eth/v1/builder/status
HTTP/1.1 200 OK
Content-Length: 3
Content-Type: application/json; charset=utf-8
Date: Wed, 05 Jul 2023 11:02:01 GMT
Server: Kestrel
```

### Binary releases
You can find the latest binary releases at https://github.com/mevsharp/mevsharp/releases.
Binaries are available for Linux (x64, arm64), Windows (x64) and macOS (x64, arm64).


### Compiling from source
The prerequisite is to have dotnet 7 installed on your system.
To built mevsharp, run this:

```
cd MEVSharp.UI.API
dotnet publish -c Release -p:PublishSingleFile=true --self-contained true -p:Deterministic=true -o build
build/mevsharp --network goerli --relay https://publickey@relay1.example --relay https://publickey@relay2.example --listen http://localhost:18550
```

#### Tests
You can run tests by executing:
```
cd src
dotnet test
```


## Configuration
mevsharp can be configured by using the CLI at run-time, or by configuring the appsettings.json file.

### CLI
```
mevsharp@mevsharptest01:~$ mevsharp --help
Description:
  mevsharp Command Line Interface

Usage:
  mevsharp [options]

Options:
  --relay <relay>                                            []
  --relays <relays>                                          []
  --listen <listen>                                          [default: http://0.0.0.0:18550]
  --network <network>                                        [default: mainnet]
  --genesis-fork-version <genesis-fork-version>              []
  --min-bid <min-bid>                                        [default: 0.0]
  --relay-check                                              [default: False]
  --request-timeout-getheader <request-timeout-getheader>    [default: 750]
  --request-timeout-getpayload <request-timeout-getpayload>  [default: 4000]
  --request-timeout-regval <request-timeout-regval>          [default: 3000]
  --log-no-version                                           [default: False]
  --version                                                  Show version information
  -?, -h, --help                                             Show help and usage information
```


## Relays
A list of public relays can be found here:

[EthStaker](https://github.com/eth-educators/ethstaker-guides/blob/main/MEV-relay-list.md)

[Lido](https://research.lido.fi/t/lido-on-ethereum-call-for-relay-providers/2844)


## Contributing
We would love for you to help contributing to mevsharp! ❤️ Before you start work on a feature or fix, please read and follow our [contribution guide](https://github.com/mevsharp/mevsharp/blob/main/CONTRIBUTING.md).


## Security
If you believe you have found a security vulnerability in our code, please report it to us as described in our [security policy](https://github.com/mevsharp/mevsharp/security/policy).


## License
mevsharp is an open-source software licensed under [MIT](https://github.com/mevsharp/mevsharp/blob/main/LICENSE).

mevsharp is using code/libraries from [Nethermind](https://github.com/NethermindEth/nethermind) and other projects such as [SSZSharp](https://github.com/hexafluoride/SszSharp) and [Lantern](https://github.com/uink45/Beacon-Chain-Light-Client-Prototype). Licenses for external projects apply on their code/libraries.
