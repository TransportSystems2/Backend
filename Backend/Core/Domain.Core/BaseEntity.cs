﻿using System;

namespace TransportSystems.Backend.Core.Domain.Core
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTime AddedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}