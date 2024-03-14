using Microcoin.Blockchain.Block;
using Microcoin.Blockchain.Chain;
using System.Text.Json;

namespace Microcoin.ChainsIO
{
    public static class ChainStreaming
    {
        public static async Task<AbstractChain> ReadChainFromStream(Stream stream, CancellationToken cancellationToken)
        {
            var jsonStreamParser = new JsonStreamParser.JsonStreamParser();
            var chainHeader = (await jsonStreamParser.ParseJsonObject(stream, cancellationToken)).Deserialize<ChainHeader>();
            if (chainHeader is null)
                throw new Exception("Deserialized object has wrong type, must be 'Chain'");
            var blocksList = new List<Block>();
            var chain = new Chain();
            for (int i = 0; i < chainHeader.BlocksCount; i++)
            {
                var chainBlock = (await jsonStreamParser.ParseJsonObject(stream, cancellationToken)).Deserialize<Block>();
                if (chainBlock is null)
                    throw new Exception("Deserialized object has wrong type, must be 'Block'");
                chain.AddTailBlock(chainBlock);
            }
            return chain;
        }

        public static async Task WriteChainToStream(Stream stream, AbstractChain chain, CancellationToken cancellationToken)
        {
            StreamWriter streamWriter = new StreamWriter(stream);
            await ChainSerialization.SerializeChainHeaderToStream(new ChainHeader(chain), streamWriter, cancellationToken);
            var blocks = chain.GetBlocksList();
            foreach (var block in blocks)
                await ChainSerialization.SerilizeBlockToStream(block, streamWriter, cancellationToken);
            await streamWriter.FlushAsync();
        }
    }
}
