# 📦 Items

## 📌 Domain Overview
O **Item** é a materialização de uma entidade avaliável no sistema Valora. Enquanto a `Category` atua como o molde (template), o `Item` é o objeto do mundo real que foi cadastrado e que receberá as avaliações dos usuários.

Por exemplo: se a categoria é *'Filmes'* (o molde), o item é *'O Senhor dos Anéis: O Retorno do Rei'* (a coisa real). Ele armazena os valores específicos exigidos pelo `Schema` da sua respectiva categoria utilizando o **Attribute Pattern** (Dicionário de dados flexível).

## 📜 Business Rules
- **Relacionamento Obrigatório:** Um item não existe no vácuo; ele deve sempre ser vinculado a uma categoria existente e válida (`CategoryId`).
- **Unicidade de Escopo:** Não podem existir dois itens com o mesmo nome exato *dentro da mesma categoria*. (Ex: Não podem existir dois itens "iPhone 15" na categoria "Smartphones", mas a validação ignora itens de categorias diferentes).
- **Validação Cruzada de Schema:** Ao criar ou atualizar um item, o dicionário de `Attributes` enviado pelo usuário deve ser validado contra o `Schema` atual da Categoria. Campos marcados como obrigatórios (`IsRequired = true`) não podem faltar, e os tipos devem corresponder.
- **Busca Otimizada:** O `Name` do item é extraído do dicionário e mantido como uma propriedade fixa na raiz da classe para permitir buscas textuais e indexação de alta performance no banco de dados.
- **Exclusão Lógica (Soft Delete):** Assim como as categorias, itens são inativados via flag `IsDeleted = true` para preservar o histórico de avaliações passadas.

## 🏛️ Entity Properties

- **`Id`** *(Guid)*: Identificador único do item.
- **`CategoryId`** *(Guid)*: Referência (Chave Estrangeira lógica) para a categoria deste item.
- **`Name`** *(String)*: Nome de exibição ou título principal do item.
- **`Attributes`** *(IReadOnlyDictionary<string, object>)*: Dicionário flexível contendo as chaves e valores dinâmicos. Ex: `{"Diretor": "Peter Jackson", "Ano": 2003}`.
- **`CreatedAt`** *(DateTimeOffset)*: Data e hora do registro inicial.
- **`UpdatedAt`** *(DateTimeOffset?)*: Data e hora da última alteração.
- **`IsDeleted`** *(Boolean)*: Flag de controle para exclusão lógica.

## 🚀 Use Cases (CQRS)

*(Nota: Marcados com [ ] pois serão os próximos a serem desenvolvidos)*

### ✍️ Comandos (Escrita - Alteram o estado)
- [x] **CreateItemCommand**: Cria um novo item associado a uma categoria. O Handler validará os atributos enviados contra o Schema da Categoria.
- [x] **UpdateItemCommand**: Atualiza o nome ou os atributos dinâmicos do item. Também revalida os dados contra o Schema.
- [x] **DeleteItemCommand**: Exclui um item aplicando o Soft Delete (`entity.Delete()`).

### 📖 Consultas (Leitura - Não alteram o estado)
- [x] **GetItemByIdQuery**: Retorna os detalhes completos de um item específico, incluindo seus atributos dinâmicos.
- [x] **ListItemsByCategoryQuery**: Retorna a lista paginada de itens atrelados a uma `CategoryId` específica (usado ao entrar na página de uma Categoria).
- [x] **SearchItemsQuery**: Uma consulta global leve e paginada para buscar itens pelo `Name` (ideal para a barra de pesquisa principal do sistema).

## 📡 Eventos Produzidos (Integração)
*(Eventos disparados via Service Bus / Wolverine após o sucesso das transações)*
- `ItemCreatedEvent`
- `ItemUpdatedEvent`
- `ItemDeletedEvent`

## 🌐 Endpoints da API
| Método | Rota HTTP | Handler Responsável | Retorno |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/items` | `CreateItemCommand` | `201 Created` |
| `PUT` | `/api/items/{id}` | `UpdateItemCommand` | `204 No Content` |
| `DELETE` | `/api/items/{id}` | `DeleteItemCommand` | `204 No Content` |
| `GET`  | `/api/categories/{categoryId}/items` | `ListItemsByCategoryQuery` | `200 OK (PaginatedList)` |
| `GET`  | `/api/items/search` | `SearchItemsQuery` | `200 OK (PaginatedList)` |
| `GET`  | `/api/items/{id}` | `GetItemByIdQuery` | `200 OK (Response)` |