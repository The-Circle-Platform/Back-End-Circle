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

            var KeyPair = ServiceInQuestion.GenerateKeyPairs;

            var TestDataBytes = ServiceInQuestion.ConvertItem(testData);

            var PublicKey = KeyPair.publicKey;
            var PrivateKey = KeyPair.privateKey;

            var Signature = ServiceInQuestion.SignData(TestDataBytes, PrivateKey);
            //Act

            var Result = ServiceInQuestion.VerifySignedData(TestDataBytes, PublicKey, Signature);
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

            var Signature = ServiceInQuestion.SignData(TestDataBytes, PrivateKey);
            //Act

            var Result = ServiceInQuestion.VerifySignedData(TestDataBytes, PublicKey, Signature);
            //Assert
            Assert.False(Result);
        }

        [Fact]
        public void ExternalKeysShouldBeCompatibel()
        {
            var PrivateKey = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<RSAParameters xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <D>K6uQu2D0LQHSieBb2tZUOADlIaEShfzgCZpfze4KWXSxLQ5Lqg0wZieQmqI9j7Hz4gCiChHvm2zZDh76aHDYaAkfny8v/lusNZZIlBDPNjt5AlcUfK06V+ZsxFJxxP/uBZOdQ5QWtn/3G9BJBpKyzBvXLZf5zociXWi3pDH7QSU=</D>\r\n  <DP>nzhz61N+mH5L+r4cADSKRomhFLx3+ci4Ruaqqs4zJMXDGT98cUjqLOFXBO/SYOthxJp/0Y2Uv7kF2hxNeRJyVQ==</DP>\r\n  <DQ>AujHv1YV/IKR5X5tRprT19cA8mofWfefedyFtW/sWrVS/nCzem9WOfS85MipjZg3VVOfMM4iiupFBLFP2K79zQ==</DQ>\r\n  <Exponent>AQAB</Exponent>\r\n  <InverseQ>gyIEECWXneF088Ut2Q/2Gv3Uj83f0gObg2TqZrhQakBD+8KwBx29Meo5toZjkcGfFEKMGpDhW4wBJZPnouRU3A==</InverseQ>\r\n  <Modulus>t+wRShu6zgmAXX3VDHTrwPoDj+dTT5DBkyoW31dpKli3XOzYkl2VOD8nJB/rPL85VRQe6EKfb7Io8m2bmqWt7b+oeKJPeg1nUC5YvkT6BmkgQ4PegGboXM1mHk9pDgBTBHI8RGJzIWqJRTNVxznIMRQWw4eX6i6wK2ikjkEKKqk=</Modulus>\r\n  <P>3MSwPZFTa2g97FFzwXqUvooyUWyN7+guGaq3Ak/dyTcPWBHh6XnkoCn2vWhSgL2fkhjJX3kFLJAZlMsgD2y+Cw==</P>\r\n  <Q>1UYO7HrwIoWOn04byQy6UiO7rmi9BNxVL4jA/ZqSOJseSTgaGNwb3DIRnfCiJk4nYsdzPIOJLXn3r1fHTPuOmw==</Q>\r\n</RSAParameters>";
            var PublicKey = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<RSAParameters xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <Exponent>AQAB</Exponent>\r\n  <Modulus>t+wRShu6zgmAXX3VDHTrwPoDj+dTT5DBkyoW31dpKli3XOzYkl2VOD8nJB/rPL85VRQe6EKfb7Io8m2bmqWt7b+oeKJPeg1nUC5YvkT6BmkgQ4PegGboXM1mHk9pDgBTBHI8RGJzIWqJRTNVxznIMRQWw4eX6i6wK2ikjkEKKqk=</Modulus>\r\n</RSAParameters>";
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

            var Signature = ServiceInQuestion.SignData(TestDataBytes, DeserialisedPrivateKey);
            //Act

            var Result = ServiceInQuestion.VerifySignedData(TestDataBytes, DeserialisedPublicKey, Signature);
            //Assert
            Assert.True(Result);
        }

        [Fact]
        public void TestEncryptiom()
        {
            //Arrange
            var PrivateKey = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<RSAParameters xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <D>K6uQu2D0LQHSieBb2tZUOADlIaEShfzgCZpfze4KWXSxLQ5Lqg0wZieQmqI9j7Hz4gCiChHvm2zZDh76aHDYaAkfny8v/lusNZZIlBDPNjt5AlcUfK06V+ZsxFJxxP/uBZOdQ5QWtn/3G9BJBpKyzBvXLZf5zociXWi3pDH7QSU=</D>\r\n  <DP>nzhz61N+mH5L+r4cADSKRomhFLx3+ci4Ruaqqs4zJMXDGT98cUjqLOFXBO/SYOthxJp/0Y2Uv7kF2hxNeRJyVQ==</DP>\r\n  <DQ>AujHv1YV/IKR5X5tRprT19cA8mofWfefedyFtW/sWrVS/nCzem9WOfS85MipjZg3VVOfMM4iiupFBLFP2K79zQ==</DQ>\r\n  <Exponent>AQAB</Exponent>\r\n  <InverseQ>gyIEECWXneF088Ut2Q/2Gv3Uj83f0gObg2TqZrhQakBD+8KwBx29Meo5toZjkcGfFEKMGpDhW4wBJZPnouRU3A==</InverseQ>\r\n  <Modulus>t+wRShu6zgmAXX3VDHTrwPoDj+dTT5DBkyoW31dpKli3XOzYkl2VOD8nJB/rPL85VRQe6EKfb7Io8m2bmqWt7b+oeKJPeg1nUC5YvkT6BmkgQ4PegGboXM1mHk9pDgBTBHI8RGJzIWqJRTNVxznIMRQWw4eX6i6wK2ikjkEKKqk=</Modulus>\r\n  <P>3MSwPZFTa2g97FFzwXqUvooyUWyN7+guGaq3Ak/dyTcPWBHh6XnkoCn2vWhSgL2fkhjJX3kFLJAZlMsgD2y+Cw==</P>\r\n  <Q>1UYO7HrwIoWOn04byQy6UiO7rmi9BNxVL4jA/ZqSOJseSTgaGNwb3DIRnfCiJk4nYsdzPIOJLXn3r1fHTPuOmw==</Q>\r\n</RSAParameters>";
            var PublicKey = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<RSAParameters xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <Exponent>AQAB</Exponent>\r\n  <Modulus>t+wRShu6zgmAXX3VDHTrwPoDj+dTT5DBkyoW31dpKli3XOzYkl2VOD8nJB/rPL85VRQe6EKfb7Io8m2bmqWt7b+oeKJPeg1nUC5YvkT6BmkgQ4PegGboXM1mHk9pDgBTBHI8RGJzIWqJRTNVxznIMRQWw4eX6i6wK2ikjkEKKqk=</Modulus>\r\n</RSAParameters>";
            
            
            var ServiceInQuestion = new SecurityHelper();
            var KeyPair = ServiceInQuestion.GetKeyString();

            var load = "Hello wereld";

            var test = ServiceInQuestion.EncryptData(load, KeyPair.publicKeyString);

            //Act
            var result = ServiceInQuestion.DecryptData(test, KeyPair.privateKeyString);

            //Assert
            var encodedResult = Encoding.UTF8.GetString(result);

            Assert.Equal("Hello wereld", $"{encodedResult.Replace("\"", "")}");
        }
    }
}
