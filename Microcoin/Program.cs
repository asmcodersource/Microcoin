using Microcoin;

Peer peer = new Peer();
Console.Write("Initialize wallet keys...");
peer.LoadOrCreateWalletKeys("wallet.keys");
Console.WriteLine("Ok");
Console.Write("Initialize inner pipeline of handling network messages...");
peer.InitializeAcceptancePools();
Console.WriteLine("Ok");
Console.Write("Initialize mining...");
peer.InitializeMining();
Console.WriteLine("Ok");
Console.Write("Initialize network system, and execute NetworkExplore procedure...");
peer.InitializeNetworking();
Console.WriteLine("Ok");
