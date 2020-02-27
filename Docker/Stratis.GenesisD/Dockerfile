FROM microsoft/dotnet:3.1-sdk

RUN git clone https://github.com/genesisofficial/genesisfullnode.git \
    && cd /genesisfullnode/src/Stratis.GenesisD \
	&& dotnet build
	
VOLUME /root/.genesisnetworknode

WORKDIR /genesisfullnode/src/Stratis.GenesisD

COPY genesis.conf.docker /root/.genesisnetworknode/gnet/GenesisNetworkMainnet/gnet.conf

EXPOSE 9465 9463 

CMD ["dotnet", "run"]
