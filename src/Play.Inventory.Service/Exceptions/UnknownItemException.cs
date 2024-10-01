using System;
using System.Runtime.Serialization;

namespace Play.Inventory.Service.Exceptions
{
    [Serializable]
    internal class UnknownItemException : Exception
    {
        private Guid catalogItemId;


        public UnknownItemException(Guid itemId) : base($"Unknown item '{itemId}' ")
        {
            this.catalogItemId = itemId;
        }

        public Guid ItemId { get; }

    }
}