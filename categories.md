# 🏷️ Categories

## 📌 Domain Overview
A **Category** é o alicerce do sistema Valora. Como o sistema se propõe a avaliar 'qualquer coisa', a categoria atua como o **molde (template)** que define as características estruturais de um item. 

Por exemplo: a categoria *'Filmes'* define que os itens avaliados sob ela terão campos dinâmicos como *'Diretor'* e *'Ano de Lançamento'* predefinidos em seu `Schema`.

## 📜 Business Rules
- **Unicidade:** Não podem existir duas categorias ativas com o mesmo nome exato no banco de dados. (Gera conflito).
- **Limites de Dados:** O nome da categoria possui um limite estrito de 50 caracteres, e a descrição suporta no máximo 200 caracteres.
- **Exclusão Lógica (Soft Delete):** Uma categoria nunca é apagada fisicamente do banco de dados (MongoDB). Isso garante que avaliações e itens criados no passado não fiquem órfãos ou quebrem o sistema. Ao ser excluída, ela apenas recebe a flag `IsDeleted = true`.
- **Tipagem Dinâmica:** Os campos adicionados ao `Schema` devem obrigatoriamente pertencer aos tipos suportados pelo enum de domínio `FieldType` (Texto, Número, Data, etc.).

## 🏛️ Entity Properties

- **`Id`** *(Guid)*: Identificador único da categoria.
- **`Name`** *(String)*: Nome de exibição da categoria.
- **`Description`** *(String)*: Breve explicação sobre o que esta categoria agrupa.
- **`Schema`** *(List<CategoryField>)*: Coleção de campos dinâmicos contendo Nome, Tipo e Obrigatoriedade (`IsRequired`), que dita as propriedades dos itens pertencentes a esta categoria.
- **`IsDeleted`** *(Boolean)*: Flag de controle para exclusão lógica.

## 🚀 Use Cases (CQRS)

### ✍️ Comandos (Escrita - Alteram o estado)
- [x] **CreateCategoryCommand**: Cria uma nova categoria junto com a definição inicial do seu formulário dinâmico (`Schema`).
- [ ] **UpdateCategoryCommand**: Atualiza os dados básicos da categoria (como `Name` e `Description`).
- [ ] **UpdateCategorySchemaCommand**: Separa a responsabilidade de editar os campos dinâmicos (adicionar/remover/editar itens do `Schema`) da edição dos dados básicos. Isso aplica o SRP e evita sobreposição ou perda de dados acidental.
- [ ] **DeleteCategoryCommand**: Exclui uma categoria aplicando o Soft Delete (`entity.Delete()`).

### 📖 Consultas (Leitura - Não alteram o estado)
- [x] **GetCategoryByIdQuery**: Traz todos os detalhes de uma categoria específica, incluindo seu `Schema` completo para a tela de edição ou visualização detalhada.
- [x] **ListCategoriesQuery**: Traz a lista paginada de categorias ativas, ideal para montar grids ou tabelas de administração com metadados de paginação (`TotalPages`, `TotalCount`, etc).
- [ ] **GetCategoryOptionsQuery**: Uma query extremamente leve focada no Frontend. Retorna uma lista não-paginada contendo apenas `Id` e `Name`. Serve estritamente para popular Dropdowns/ComboBoxes.

## 📡 Eventos Produzidos (Integração)
*(Eventos disparados via Service Bus / Wolverine após o sucesso das transações)*
- `CategoryCreatedEvent`
- `CategoryUpdatedEvent`
- `CategorySchemaUpdatedEvent`
- `CategoryDeletedEvent`

## 🌐 Endpoints da API
| Método | Rota HTTP | Handler Responsável | Retorno |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/categories` | `CreateCategoryCommand` | `201 Created` |
| `PUT` | `/api/categories/{id}` | `UpdateCategoryCommand` | `204 No Content` |
| `PUT` | `/api/categories/{id}/schema` | `UpdateCategorySchemaCommand` | `204 No Content` |
| `DELETE` | `/api/categories/{id}` | `DeleteCategoryCommand` | `204 No Content` |
| `GET`  | `/api/categories` | `ListCategoriesQuery` | `200 OK (PaginatedList)` |
| `GET`  | `/api/categories/options` | `GetCategoryOptionsQuery` | `200 OK (List)` |
| `GET`  | `/api/categories/{id}` | `GetCategoryByIdQuery` | `200 OK (Response)` |