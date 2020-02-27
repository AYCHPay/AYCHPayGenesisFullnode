FROM mcr.microsoft.com/dotnet/core/sdk:3.1

RUN git clone https://github.com/genesisofficial/genesisfullnode.git \
    && cd /genesisfullnode/src/Stratis.GenesisD \
	&& dotnet build
	
VOLUME /root/.genesisnetworknode

WORKDIR /genesisfullnode/src/Stratis.GenesisD

COPY genesis.conf.docker /root/.genesisnetworknode/tgnet/GenesisNetworkTestnet/tgnet.conf

EXPOSE 9465 9463 

CMD ["dotnet", "run", "-testnet"]
