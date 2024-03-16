using Microcoin.Network.NodeNet.Message;
using Microcoin.RSAEncryptions;
using Transaction;

namespace Tests
{
    public class RSAEncryptionsUnitTests
    {
        [Fact]
        public void RSAEncryptions_MessageEncryption_Test()
        {
            // Create any keys
            var signOptions = RSAEncryption.CreateSignOptions();
            // Sign message
            Message message = new Message(
                new MessageInfo(signOptions.PublicKey, "here is some public key"),
                "here is some data"
            );
            MessageSigner messageSigner = new MessageSigner();
            messageSigner.SetSignOptions(signOptions);
            messageSigner.Sign(message);
            // Validate message sign
            MessageValidator messageValidator = new MessageValidator();
            messageValidator.SetValidateOptions(MessageValidator.GetReceiverValidateOptions(message));
            var success = messageValidator.Validate(message);
            // Is sign validation works fine?
            Assert.True(success);
        }

        [Fact]
        public void RSAEncryptions_TransactionEncryption_Test()
        {
            // Create any keys
            var signOptions = RSAEncryption.CreateSignOptions();
            // Sign transaction
            var transaction = new Transaction.Transaction();
            TransactionSigner transactionSigner = new TransactionSigner();
            transactionSigner.SetSignOptions(signOptions);
            transactionSigner.Sign(transaction);
            // Validate transaction sign
            TransactionValidator trasnactionValidator = new TransactionValidator();
            trasnactionValidator.SetValidateOptions(TransactionValidator.GetReceiverValidateOptions(transaction));
            var success = trasnactionValidator.Validate(transaction);
            // Is sign validation works fine?
            Assert.True(success);
        }
    }
}