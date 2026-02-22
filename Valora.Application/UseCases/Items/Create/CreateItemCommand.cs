using System;
using System.Collections.Generic;
using FluentValidation;

namespace Valora.Application.UseCases.Items.Create;

public record CreateItemCommand( Guid CategoryId, string Name, Dictionary<string, object> Attributes);


