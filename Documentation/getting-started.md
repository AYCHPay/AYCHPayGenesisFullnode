
# Getting started - Building and running a Genesis Full Node 

---------------

## Supported Platforms

* <b>Windows</b> - works from Windows 7 and later, on both x86 and x64 architecture. Most of the development and testing is happening here.
* <b>Linux</b> - works and Ubuntu 14.04 and later (x64). It's been known to run on some other distros so your mileage may vary.
* <b>MacOS</b> - works from OSX 10.12 and later. 

## Prerequisites

To install and run the node, you need
* [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
* [Git](https://git-scm.com/)

## Build instructions

### Get the repository and its dependencies

```
git clone https://github.com/genesisofficial/genesisfullnode.git
cd genesisfullnode/src
```

### Build and run the code
With this node, you can connect to either the Genesis network or the Bitcoin network, either on MainNet or TestNet.
So you have 4 options:

1. To run a <b>Genesis</b> node on <b>Mainnet</b>, do
```
cd Genesis.GenesisD
dotnet run
```  

2. To run a <b>Genesis</b>  node on <b>Testnet</b>, do
```
cd Genesis.GenesisD
dotnet run -testnet
```  

3. To run a <b>Bitcoin</b> node on <b>Mainnet</b>, do
```
cd Stratis.BitcoinD
dotnet run
```  

4. To run a <b>Bitcoin</b> node on <b>Testnet</b>, do
```
cd Stratis.BitcoinD
dotnet run -testnet
```  

### Advanced options

You can get a list of command line arguments to pass to the node with the -help command line argument. For example:
```
cd Genesis.GenesisD
dotnet run -help
```  

### Script
Ease of use scripts for multiple platforms can be found [here](https://github.com/genesisofficial/genesisfullnode/Scripts/)

Swagger Endpoints
-------------------

Once the node is running, a Swagger interface (web UI for testing an API) is available.

* For Genesis Mainnet: http://localhost:4640/swagger/
* For Genesis Testnet: http://localhost:9465/swagger/
* For Bitcoin Mainnet: http://localhost:37220/swagger/
* For Bitcoin Testnet: http://localhost:38220/swagger/
