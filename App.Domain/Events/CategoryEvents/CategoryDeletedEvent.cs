namespace App.Domain.Events.CategoryEvents;

public record CategoryDeletedEvent(int Id,string Name) : IEventOrMessage;


