using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.Helper;
using Xunit;

namespace Tests.ServiceTest
{
    public class SecurityHelperTests
    {

        [Fact]
        public void SignatureShouldBeVerifiedWithPrivateKey()
        {
            //Arrange
            var ServiceInQuestion = new SecurityHelper();
            var testData = new List<ChatMessage>()
            {
                new ChatMessage() {
                    Id = 1,
                    ReceiverId = 1,
                    Message = "Hello world",
                    Date = DateTime.Now,
                    WebUserId = 1,
                    Writer = new WebsiteUser() { Id = 3, IsOnline = true, UserName = "Boi"}
                },
                new ChatMessage() {
                    Id = 2,
                    ReceiverId = 1,
                    Message = "How are you",
                    Date = DateTime.Now,
                    WebUserId = 1,
                    Writer = new WebsiteUser() { Id = 3, IsOnline = true, UserName = "Boi"}
                }
            };

            var KeyPair = ServiceInQuestion.GenerateKeyPairs();

            var TestDataBytes = ServiceInQuestion.ConvertItem(testData);

            var PublicKey = KeyPair.publicKey;
            var PrivateKey = KeyPair.privateKey;

            var Signature = ServiceInQuestion.SignData(TestDataBytes, PrivateKey, true);
            //Act

            var Result = ServiceInQuestion.VerifySignedData(TestDataBytes, PublicKey, Signature, false);
            //Assert
            Assert.True(Result);
        }

        [Fact]
        public void SignatureShouldNotAcceptOtherPrivateKey()
        {
            //Arrange
            
            var ServiceInQuestion = new SecurityHelper();
            var testData = new List<ChatMessage>()
            {
                new ChatMessage() {
                    Id = 1,
                    ReceiverId = 1,
                    Message = "Hello world",
                    Date = DateTime.Now,
                    WebUserId = 1,
                    Writer = new WebsiteUser() { Id = 3, IsOnline = true, UserName = "Boi"}
                },
                new ChatMessage() {
                    Id = 2,
                    ReceiverId = 1,
                    Message = "How are you",
                    Date = DateTime.Now,
                    WebUserId = 1,
                    Writer = new WebsiteUser() { Id = 3, IsOnline = true, UserName = "Boi"}
                }
            };

            var KeyPair = ServiceInQuestion.GenerateKeyPairs();
            var OtherKeyPair = ServiceInQuestion.GenerateKeyPairs();

            var TestDataBytes = ServiceInQuestion.ConvertItem(testData);

            var PublicKey = OtherKeyPair.publicKey;
            var PrivateKey = KeyPair.privateKey;

            var Signature = ServiceInQuestion.SignData(TestDataBytes, PrivateKey, true);
            //Act

            var Result = ServiceInQuestion.VerifySignedData(TestDataBytes, PublicKey, Signature, false);
            //Assert
            Assert.False(Result);
        }

        [Fact]
        public void ExternalKeysShouldBeCompatibel()
        {
            var PrivateKey = "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAMJefUpb8rGnf7WiUtC4funvWKjgLQIoXrUbJItDb2Q5Dq4NAaYMtih/2iq4eABCn9keb+NWe2F2ZcbyI7iLwj+UiLVpCffgj3CfoeYExE1RqLn1S5CXI9kMJNaTsQXPSA/BnjppX+5Z0zAI+TZef44B6NwRIsE/dYb0dtMejT+VAgMBAAECgYAJ+JLw15qxpmgUx0j8UBqioZaowydL7wo8vDG5uzHhsFOidiRZgllt5nEos+HkEYblunv+65bUvyAlfpJ6iyDhzxgs9fSapdkhiz057BVkmwOqzIDDefHjpqh00k+sEZWeZKq0flXG12yF8LI4c1qXnjTnTUCVzIJXhe4kPqufGQJBAMkIY+8QG4EO4RsvOkdS4Bmz+GSZr7n9FLKsWEQU958v99aGC4T8OLaMFpztRrDwj7tZcvEWl7qVHbI5aTrjnbcCQQD3g6oZdQSLEvU4F4NIiUijTMtMgImzKujbhLdchETqrG0G4UUzGl5Itp/NMhLjscsykgl5mlI/4N2We2Hoi80TAkAEP5olDjkWlCLruSbJJRY5VNVWAu10x8VtNTk0TyEgixn4vaJ2sAHe0b0UmesZiCvxcKV+NNUGC2qyPoZbyT2nAkEA4VhTPngmWcQ51Aa8NQcgReS91vnT5HZ1qJ5tHmMiJ5IydSgVi5A/NO5oETa8secGPBVvYPIaXiQJOl885a6aVwJBALIPMLuUps82cKIanFRh0OC8vIZwtFgv8PUpCAYDG1LKo6RDaSoRL7qtighAagxp+pYcU0rQAyuwHf9mHGiyESM=";
            var PublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDCXn1KW/Kxp3+1olLQuH7p71io4C0CKF61GySLQ29kOQ6uDQGmDLYof9oquHgAQp/ZHm/jVnthdmXG8iO4i8I/lIi1aQn34I9wn6HmBMRNUai59UuQlyPZDCTWk7EFz0gPwZ46aV/uWdMwCPk2Xn+OAejcESLBP3WG9HbTHo0/lQIDAQAB";
            //Arrange

            var ServiceInQuestion = new SecurityHelper();
            var testData = new List<ChatMessage>()
            {
                new ChatMessage() {
                    Id = 1,
                    ReceiverId = 1,
                    Message = "Hello world",
                    Date = DateTime.Now,
                    WebUserId = 1,
                    Writer = new WebsiteUser() { Id = 3, IsOnline = true, UserName = "Boi"}
                },
                new ChatMessage() {
                    Id = 2,
                    ReceiverId = 1,
                    Message = "How are you",
                    Date = DateTime.Now,
                    WebUserId = 1,
                    Writer = new WebsiteUser() { Id = 3, IsOnline = true, UserName = "Boi"}
                }
            };
            var DeserialisedPublicKey = ServiceInQuestion.DeserialiseKey(PublicKey);
            var DeserialisedPrivateKey = ServiceInQuestion.DeserialiseKey(PrivateKey);

            var TestDataBytes = ServiceInQuestion.ConvertItem(testData);

            var Signature = ServiceInQuestion.SignData(TestDataBytes, DeserialisedPrivateKey, true);
            //Act

            var Result = ServiceInQuestion.VerifySignedData(TestDataBytes, DeserialisedPublicKey, Signature, false);
            //Assert
            Assert.True(Result);
        }

        [Fact]
        public void EnvironmentKeysCheck()
        {
            var PrivateKey = Environment.GetEnvironmentVariable("SERVER_PRIVKEY");
            var PublicKey = Environment.GetEnvironmentVariable("SERVER_PUBKEY");
            //Arrange

            var ServiceInQuestion = new SecurityHelper();
            var testData = new List<ChatMessage>()
            {
                new ChatMessage() {
                    Id = 1,
                    ReceiverId = 1,
                    Message = "Hello world",
                    Date = DateTime.Now,
                    WebUserId = 1,
                    Writer = new WebsiteUser() { Id = 3, IsOnline = true, UserName = "Boi"}
                },
                new ChatMessage() {
                    Id = 2,
                    ReceiverId = 1,
                    Message = "How are you",
                    Date = DateTime.Now,
                    WebUserId = 1,
                    Writer = new WebsiteUser() { Id = 3, IsOnline = true, UserName = "Boi"}
                }
            };
            var DeserialisedPublicKey = ServiceInQuestion.DeserialiseKey(PublicKey);
            var DeserialisedPrivateKey = ServiceInQuestion.DeserialiseKey(PrivateKey);

            var TestDataBytes = ServiceInQuestion.ConvertItem(testData);

            var Signature = ServiceInQuestion.SignData(TestDataBytes, DeserialisedPrivateKey, true);
            //Act

            var Result = ServiceInQuestion.VerifySignedData(TestDataBytes, DeserialisedPublicKey, Signature, false);
            //Assert
            Assert.True(Result);
        }


    }
}
