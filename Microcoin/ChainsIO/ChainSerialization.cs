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

        public static async Task SerilizeChainToStream(AbstractChain chain, Stream stream, CancellationToken cancellationToken)
        {
            StreamWriter streamWriter = new StreamWriter(stream);
            await streamWriter.WriteAsync(new StringBuilder(SerializeChain(chain)), cancellationToken);
            await streamWriter.FlushAsync();
        }

        public static async Task SerializeChainHeaderToStream(ChainHeader chainHeader, Stream stream, CancellationToken cancellationToken)
        {
            StreamWriter streamWriter = new StreamWriter(stream);
            await streamWriter.WriteAsync(new StringBuilder(SerializeChainHeader(chainHeader)), cancellationToken);
            await streamWriter.FlushAsync();
        }

        public static async Task SerilizeBlockToStream(Block block, Stream stream, CancellationToken cancellationToken)
        {
            StreamWriter streamWriter = new StreamWriter(stream);
            await streamWriter.WriteAsync(new StringBuilder(SerializeBlock(block)), cancellationToken);
            await streamWriter.FlushAsync();
        }
    }
}
