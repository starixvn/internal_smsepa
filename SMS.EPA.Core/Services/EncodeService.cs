// -----------------------------------------------------------------------
// <copyright file="EncodeService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.10">Created</change>
// </history>

namespace SMS.EPA.Core.Services
{
    using Constants;
    using IBCCrypt;
    using SMS.EPA.Core.Services.Interfaces;

    public class EncodeService : IEncodeService
    {
        public string Encode(string plainText)
        {
            string cipherText = IBCCrypt.Encrypt(plainText, Constants.KeyCryptography);

            return cipherText;
        }

        public string Decode(string cipherText)
        {
            string plainText = IBCCrypt.Decrypt(cipherText, Constants.KeyCryptography);

            return plainText;
        }
    }
}