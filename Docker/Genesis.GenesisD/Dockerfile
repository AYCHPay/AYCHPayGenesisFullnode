FROM mcr.microsoft.com/dotnet/core/sdk:3.1

RUN git clone https://github.com/genesisofficial/genesisfullnode.git \
    && cd /genesisfullnode/src/Genesis.GenesisD \
	&& dotnet build
	
VOLUME /root/.genesisnetworknode

WORKDIR /genesisfullnode/src/Genesis.GenesisD

COPY genesis.conf.docker /root/.genesisnetworknode/gnet/GenesisNetworkMainnet/gnet.conf

EXPOSE 9465 9463 

CMD ["dotnet", "run"]
