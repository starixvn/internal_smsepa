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
    using Fanex.Common;

    public static class EncodeService
    {
        public static string Encode(string plainText)
        {
            string cipherText = string.Empty;

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                cipherText = Cryptography.CfsEnCrypt(Constants.KeyCryptography + Cryptography.CfsEnCrypt(plainText));
            }

            return cipherText;
        }

        public static string Decode(string cipherText)
        {
            string plainText = string.Empty;

            if (!string.IsNullOrWhiteSpace(cipherText))
            {
                plainText = Cryptography.CfsDeCrypt(Cryptography.CfsDeCrypt(cipherText).Replace(Constants.KeyCryptography, string.Empty));
            }

            return plainText;
        }
    }
}