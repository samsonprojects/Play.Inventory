namespace Play.Inventory.Contracts;


//correlationId is used to map a message to a specific instance of the state machine
// all messages that belong to one transaction should have the same correlation id
public record GrantItems(
    Guid UserId,
    Guid CatalogItemId,
    int Quantity,
    Guid CorrelationId);

public record InventoryItemsGranted(Guid CorrelationId);

public record SubtractItems(
    Guid UserId,
    Guid CatalogItemId,
    int Quantity,
    Guid CorrelationId);


public record InventoryItemsSubtracted(Guid CorrelationId);
public record InventoryItemUpdated(
         Guid UserId,
         Guid CatalogItemId,
         int NewTotalQuantity
     );


