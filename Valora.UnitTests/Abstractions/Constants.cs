using Valora.Domain.Entities;

namespace Valora.UnitTests.Abstractions;

public static class Constants
{
    public static class Category
    {
        public static readonly Guid Id = Guid.Parse("91e74d57-b48b-4f4b-9087-50464da060c1");
        public const string Name = "Filmes";
        public const string Description = "Descrição de teste";
    }

    public static class Item
    {
        public static readonly Guid Id = Guid.NewGuid();
        public const string ValidTitle = "O Senhor dos Anéis";
        public const int ValidYear = 2001;
    }
}