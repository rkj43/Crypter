﻿using Crypter.Contracts.Enum;
using System;

namespace Crypter.Contracts.Requests.Anonymous
{
    public class AnonymousSignatureRequest
    {
        public Guid Id { get; set; }
        public ResourceType Type { get; set; }

        /// <summary>
        /// Do not use!
        /// For deserialization purposes only.
        /// </summary>
        public AnonymousSignatureRequest()
        { }

        public AnonymousSignatureRequest(Guid id, ResourceType type)
        {
            Id = id;
            Type = type;
        }
    }
}
