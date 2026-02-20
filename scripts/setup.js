// 1. Inicializar o Replica Set (Habilita transações ACID para o Unit of Work)
try {
    rs.status();
    print("Replica Set 'rs0' já está inicializado.");
} catch (e) {
    print("Inicializando Replica Set 'rs0'...");
    rs.initiate({
        _id: "rs0",
        members: [{ _id: 0, host: "mongo:27017" }]
    });
}

// Aguardar o nó se tornar PRIMARY para poder criar bancos e coleções
while (!db.isMaster().ismaster) {
    sleep(1000);
}

// Selecionar o banco de dados do projeto Valora
db = db.getSiblingDB('valora');

// 2. Criar coleção com Schema Validation (Defesa em Profundidade)
// Garante que ninguém (nem mesmo um bug no C#) insira uma categoria sem nome ou estado de deleção.
db.createCollection("categories", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["Name", "IsDeleted"],
            properties: {
                Name: {
                    bsonType: "string",
                    description: "O nome da categoria é obrigatório e deve ser texto."
                },
                IsDeleted: {
                    bsonType: "bool",
                    description: "A flag IsDeleted é obrigatória e deve ser booleana."
                }
            }
        }
    }
});

// 3. Índices de Performance e Integridade

// Índice Único: Previne Race Conditions (duas pessoas criando a mesma categoria no mesmo ms).
// Collation (strength: 2) garante que "Hardware" e "hardware" sejam vistos como iguais (Case Insensitive).
db.categories.createIndex(
    { "Name": 1 },
    { unique: true, collation: { locale: "pt", strength: 2 } }
);

// Índice Parcial (Soft Delete): Acelera drasticamente buscas indexando apenas o que NÃO está deletado.
db.categories.createIndex(
    { "CreatedAt": -1 },
    { partialFilterExpression: { IsDeleted: false }, name: "idx_active_categories" }
);

print("Configuração inicial do banco de dados Valora concluída com sucesso!");