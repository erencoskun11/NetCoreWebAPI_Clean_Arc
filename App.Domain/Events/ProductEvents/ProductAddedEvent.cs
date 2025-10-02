namespace App.Domain.Events.ProductEvents;

    public record ProductAddedEvent(int Id,string Name,decimal Price) : IEventOrMessage;
   

