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

    public static class EncodeService
    {
        public static string Encode(string plainText)
        {
            string cipherText = IBCCrypt.Encrypt(plainText, Constants.KeyCryptography);

            return cipherText;
        }

        public static string Decode(string cipherText)
        {
            string plainText = IBCCrypt.Decrypt(cipherText, Constants.KeyCryptography);

            return plainText;
        }
    }
}