﻿using Microcoin.RSAEncryptions;

namespace Microcoin.RSAEncryptions
{
    public class ReceiverSignOptions : IReceiverSignOptions
    {
        public string PublicKey { get; set; }

        public ReceiverSignOptions(string publicKey)
        {
            PublicKey = publicKey;
        }
    }
}
