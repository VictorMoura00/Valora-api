using System;

namespace Valora.Domain.Common.Exceptions;

public class DomainException(string message) : Exception(message);