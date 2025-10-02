namespace App.Domain.Events.ProductEvents;

public record ProductDeletedEvent(int Id ,string Name,decimal Price) : IEventOrMessage;


