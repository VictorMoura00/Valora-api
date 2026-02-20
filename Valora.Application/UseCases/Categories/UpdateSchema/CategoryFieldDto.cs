using System;
using System.Collections.Generic;
using System.Text;
using Valora.Domain.Entities;

namespace Valora.Application.UseCases.Categories.UpdateSchema;

public record CategoryFieldDto(string Name, FieldType Type, bool IsRequired);