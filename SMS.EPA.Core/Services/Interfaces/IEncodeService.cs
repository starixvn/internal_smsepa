// -----------------------------------------------------------------------
// <copyright file="IEncodeService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.10">Created</change>
// </history>

namespace SMS.EPA.Core.Services.Interfaces
{
    public interface IEncodeService
    {
        string Encode(string plainText);

        string Decode(string cipherText);
    }
}