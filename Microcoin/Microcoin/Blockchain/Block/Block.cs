using Mining;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Block
{
    [Serializable]
    public class Block
    {
        public List<Transaction.Transaction> Transactions { get; set; } = new List<Transaction.Transaction>();
        public MiningBlockInfo MiningBlockInfo { get; set; } = new MiningBlockInfo();
        public string Hash { get; set; } = "";


        /// <returns>Special hash of block, it dosn't include all field of object</returns>
        public string GetMiningBlockHash()
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                var temp1 = JsonSerializer.Serialize(Transactions);
                var temp2 = JsonSerializer.Serialize(MiningBlockInfo);
                var temp3 = Encoding.UTF8.GetBytes(temp1 + temp2);
                byte[] hash = sha256Hash.ComputeHash(temp3);
                return Convert.ToBase64String(hash);
            }
        }

        public static int GetHashComplexity(string hash)
        {
            byte[] hashBytes = Convert.FromBase64String(hash);
            for (int i = 0; i < hashBytes.Length; i++)
            {
                byte b = hashBytes[i];
                for (int j = 0; j < 8; j++)
                    if ((b >> j & 1) == 1)
                        return i * 8 + j;
            }
            return hashBytes.Length * 8;
        }

        public static Block? ParseBlockFromJson(string blockJson)
            => JsonSerializer.Deserialize<Block>(blockJson);
    }
}
