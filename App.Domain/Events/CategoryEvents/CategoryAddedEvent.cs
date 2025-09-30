namespace App.Domain.Events.CategoryEvents;

    public record CategoryAddedEvent(int Id,string name,DateTime? Created,DateTime? Updated) : IEventOrMessage;
 
