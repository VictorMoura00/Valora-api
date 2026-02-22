using System;
using System.Collections.Generic;

namespace Valora.Application.UseCases.Items.Search;

public record SearchItemsQuery(
    string SearchTerm,
    int PageNumber = 1,
    int PageSize = 10);

