﻿using System.Security.Cryptography;

namespace TheCircleBackend.DomainServices.IHelpers
{
    public interface ISecurityService
    {
        bool HoldsIntegrity(object inputData, byte[] signature, string publicKey);
        (string privKey, string pubKey) GenerateKeys();
        byte[] SignData(object inputData, string privateKey);
        byte[] ConvertItemIntoBytes(object item, string key);
        // byte[] EncryptHash(byte[] inputData, string privateKey);
        (string privKey, string pubKey) GetKeys(int userId);
        bool StoreKeys(int UserId, string privKey, string pubKey);
        (string privKey, string pubKey) GetServerKeys();
        Task<string> DecryptMessage(string message);
    }
}
