using App.Domain.Events;

namespace App.Domain.Commands;

public record ReserveProductCommand(int Id,int Quantity) : IEventOrMessage;


