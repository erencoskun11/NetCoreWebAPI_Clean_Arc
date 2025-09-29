using App.Domain.Events;

namespace App.Domain.Commands;

public record ReserveCategoryCommand(int Id , string Name):IEventOrMessage;


