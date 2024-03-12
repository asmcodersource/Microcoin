﻿using Microcoin.Blockchain.Chain;
using Microcoin.Blockchain.Block;
using Newtonsoft.Json;
using System.Text;

namespace Microcoin.ChainsIO
{
    public static class ChainSerialization
    {
        public static String SerializeChain( AbstractChain chain )
            => JsonConvert.SerializeObject(chain);

        public static String SerializeBlock( Block block)
            => JsonConvert.SerializeObject(block);

        public static String SerializeChainHeader(ChainHeader chainHeader)
            => JsonConvert.SerializeObject(chainHeader);

        public static async Task SerilizeChainToStream(AbstractChain chain, StreamWriter stream, CancellationToken cancellationToken)
        {
            await stream.WriteAsync(new StringBuilder(SerializeChain(chain)), cancellationToken);
        }

        public static async Task SerializeChainHeaderToStream(ChainHeader chainHeader, StreamWriter stream, CancellationToken cancellationToken)
        {
            await stream.WriteAsync(new StringBuilder(SerializeChainHeader(chainHeader)), cancellationToken);
        }

        public static async Task SerilizeBlockToStream(Block block, StreamWriter stream, CancellationToken cancellationToken)
        {
            await stream.WriteAsync(new StringBuilder(SerializeBlock(block)), cancellationToken);
        }
    }
}