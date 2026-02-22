namespace Valora.Application.UseCases.Items.Update;

public record UpdateItemCommand( Guid Id, string Name, Dictionary<string, object> Attributes);
