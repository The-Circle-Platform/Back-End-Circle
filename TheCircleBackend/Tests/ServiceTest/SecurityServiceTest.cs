using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCircleBackend.Domain.DTO;
using TheCircleBackend.Domain.DTO.EncryptedPayload;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;
using TheCircleBackend.Helper;
using Xunit;

namespace Tests.ServiceTest
{
    public class SecurityServiceTest
    {
        // US-5
        [Fact]
        public void EncryptionProcessWorks()
        {
            //Arrange
            var SecurityHelper = new SecurityHelper();
            var MockKeyRepo = new Mock<IKeyRepo>();
            MockKeyRepo.Setup(p => p.GetKeys(1)).Returns(value: null);

            var ServiceInQuestion = new SecurityService(SecurityHelper, MockKeyRepo.Object);
            //Creates signature
            ChatMessage message = new() { Message = "Hello", ReceiverId = 1, WebUserId = 1 };
            
            var DTO = new ChatMessageDTOIncoming()
            {
                OriginalData = message
            };
            var KeyPairs = SecurityHelper.GetKeyString();
            var signature = ServiceInQuestion.SignData(DTO.OriginalData, KeyPairs.privateKeyString);

            DTO.Signature = signature;
            //Act
            var Result = ServiceInQuestion.HoldsIntegrity(DTO.OriginalData, DTO.Signature, KeyPairs.publicKeyString);

            //Assert
            Assert.True(Result);
        }
        // US-5
        [Fact]
        public void WrongKeyGivesFalse()
        {
            //Arrange
            var SecurityHelper = new SecurityHelper();
            var MockKeyRepo = new Mock<IKeyRepo>();
            MockKeyRepo.Setup(p => p.GetKeys(1)).Returns(value: null);
            var ServiceInQuestion = new SecurityService(SecurityHelper, MockKeyRepo.Object);
            //Creates signature
            ChatMessage message = new() { Message = "Hello", ReceiverId = 1, WebUserId = 1 };

            var KeyPairs = SecurityHelper.GetKeyString();
            var OtherKeyPairs = SecurityHelper.GetKeyString();
            var signature = ServiceInQuestion.SignData(message, KeyPairs.privateKeyString);
            var IncomingMessage = new ChatMessageDTOIncoming()
            {
                OriginalData = message,
                Signature = signature
            };

            //Act
            var Result = ServiceInQuestion.HoldsIntegrity(IncomingMessage.OriginalData, IncomingMessage.Signature, OtherKeyPairs.publicKeyString);

            //Assert
            Assert.False(Result);
        }

        // US-5
        [Fact]
        public void NotFoundKeyShouldGiveNull()
        {
            //Arrange
            var SecurityHelper = new SecurityHelper();
            var MockKeyRepo = new Mock<IKeyRepo>();
            MockKeyRepo.Setup(p => p.GetKeys(1)).Returns(value: null);
            var ServiceInQuestion = new SecurityService(SecurityHelper, MockKeyRepo.Object);

            //Creates signature
            ChatMessage message = new() { Message = "Hello", ReceiverId = 1, WebUserId = 1 };
            var DTO = new ChatMessageDTOIncoming()
            {
                OriginalData  = message,
            };

            //Act

            var value = Assert.Throws<InvalidOperationException>(() => ServiceInQuestion.GetKeys(1));

            //Assert
            Assert.Equal("Keys not found", value.Message);
        }

        // UC-5
        [Fact]
        public void UnformattedKeyInsert()
        {
            //Arrange
            var SecurityHelper = new SecurityHelper();
            var MockKeyRepo = new Mock<IKeyRepo>();
            MockKeyRepo.Setup(p => p.GetKeys(1)).Returns(value: null);

            var ServiceInQuestion = new SecurityService(SecurityHelper, MockKeyRepo.Object);
            //Creates signature
            ChatMessage message = new() { Message = "Hello", ReceiverId = 1, WebUserId = 1 };
            var DTO = new ChatMessageDTOIncoming()
            {
                OriginalData = message,
            };

            //Act
            var signature = Assert.Throws<Exception>(() => ServiceInQuestion.SignData(DTO.OriginalData, "Irrelevant"));

            //Assert
            Assert.Equal(signature.Message, "Deserialisatie is misgegaan.");
        }

        // UC-5
        [Fact]
        public void UnformattedKeyInsertHoldIntegrityEdition()
        {
            //Arrange
            var SecurityHelper = new SecurityHelper();
            var MockKeyRepo = new Mock<IKeyRepo>();
            MockKeyRepo.Setup(p => p.GetKeys(1)).Returns(value: null);

            var ServiceInQuestion = new SecurityService(SecurityHelper, MockKeyRepo.Object);
            //Creates signature
            ChatMessage message = new() { Message = "Hello", ReceiverId = 1, WebUserId = 1 };
            var DTO = new ChatMessageDTOIncoming()
            {
                OriginalData = message,
            };
            var KeyPairs = SecurityHelper.GetKeyString();
            var signature = ServiceInQuestion.SignData(DTO.OriginalData, KeyPairs.privateKeyString);
            
            DTO.Signature = signature; 

            //Act
            var Result = ServiceInQuestion.HoldsIntegrity(DTO.OriginalData, DTO.Signature, "Irrelevante data");

            //Assert
            Assert.False(Result);
        }
    }
}
