﻿using Microcoin.Blockchain.TransactionsPool;
using Microcoin.Blockchain.Transaction;
using Newtonsoft.Json;
using Microcoin.Blockchain.BlocksPool;
using Microcoin.PipelineHandling;

namespace Microcoin
{
    internal class Peer
    {
        public BlocksPool BlocksPool { get; protected set; } = new BlocksPool();
        public TransactionsPool TransactionsPool { get; protected set; } = new TransactionsPool();
        public PeerNetworking PeerNetworking { get; protected set; } 
        public PeerWalletKeys PeerWalletKeys { get; protected set; }

        public void SendCoins(String receiverPublicKey, decimal coinsCount )
        {
            if (PeerNetworking == null || PeerWalletKeys == null)
                throw new NullReferenceException("Peer is not initialized");

            Transaction transaction = new Transaction();
            transaction.ReceiverPublicKey = receiverPublicKey;
            transaction.TransferAmount = coinsCount;
            PeerWalletKeys.SignTransaction( transaction );

            var transactionBroadcast = JsonConvert.SerializeObject( transaction );
            PeerNetworking.NetworkNode.SendMessage(transactionBroadcast);
        }

        public void InitializeAcceptancePools()
        {
            TransactionsPool.InitializeHandlerPipeline();
            BlocksPool.InitializeHandlerPipeline(TransactionsPool.HandlePipeline);
        }

        public void InitializeNetworking()
        {
            PeerNetworking = new PeerNetworking();
            PeerNetworking.CreateDefaultNode();
            PeerNetworking.CreateDefaultRouting();
            PeerNetworking.PostInitialize();
            PeerNetworking.TransactionReceived += (transaction) => TransactionsPool.HandleTransaction(transaction);
            PeerNetworking.BlockReceived += (block) => BlocksPool.HandleBlock(block); 
        }

        public void LoadOrCreateWalletKeys(string filePath = "wallet.keys")
        {
            PeerWalletKeys = new PeerWalletKeys();
            if (File.Exists(filePath))
            {
                PeerWalletKeys.LoadKeys(filePath);
            }
            else
            {
                PeerWalletKeys.CreateKeys();
                PeerWalletKeys.SaveKeys(filePath);
            }
        }
    }
}