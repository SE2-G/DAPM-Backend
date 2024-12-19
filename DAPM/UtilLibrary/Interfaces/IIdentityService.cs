﻿// Author: s205135

using UtilLibrary.models;

namespace UtilLibrary.Interfaces
{
    public interface IIdentityService
    {
        public Identity? GetIdentity();
        public Identity GenerateNewIdentity();
    }
}
