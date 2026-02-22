# ⭐ Evaluations (Avaliações)

## 📌 Domain Overview
A **Evaluation** (Avaliação) é a razão de existir do sistema Valora. Ela representa o veredito, a opinião e a pontuação dada por um usuário a um determinado `Item`.

Enquanto a `Category` dita as regras e o `Item` guarda os dados técnicos, a `Evaluation` é puramente gerada pelo comportamento do usuário. Ela pode ser simples (apenas uma nota geral e um comentário) ou complexa (contendo notas fragmentadas por dimensões/critérios usando o conceito de *Value Objects*).

## 📜 Business Rules
- **Relacionamentos Obrigatórios:** Uma avaliação não pode existir solta. Ela precisa estar obrigatoriamente vinculada a um `ItemId` (o que está sendo avaliado) e a um `UserId` (quem está avaliando).
- **Unicidade de Autor (One-per-User):** Um usuário só pode ter uma única avaliação ativa por item. Se ele quiser mudar de opinião, o sistema deve atualizar a avaliação existente em vez de criar uma duplicata. (Isso evita manipulação de médias).
- **Validação de Pontuação (`RatingScore`):** A nota principal deve respeitar limites rígidos definidos pelo domínio (ex: valor mínimo de 1 e máximo de 5 estrelas/pontos).
- **Dimensões Flexíveis (`Dimensions`):** Assim como os itens possuem atributos dinâmicos, as avaliações podem ter notas fracionadas por critérios (Ex: Jogo avaliado com Gráficos: 5, Áudio: 4, Jogabilidade: 5). Isso é armazenado num dicionário de notas.
- **Auditoria e Limites:** Comentários (`Comment`) são opcionais, mas, se fornecidos, possuem limite máximo de caracteres (ex: 1000) para evitar abusos no banco de dados.
- **Exclusão Lógica (Soft Delete):** Segue o mesmo padrão do restante do sistema para evitar perda de métricas passadas.

## 🏛️ Entity Properties

- **`Id`** *(Guid)*: Identificador único da avaliação.
- **`ItemId`** *(Guid)*: Referência (Chave Estrangeira) do item que recebeu esta avaliação.
- **`UserId`** *(Guid)*: Identificador do usuário logado que escreveu a avaliação.
- **`Score`** *(RatingScore / Int)*: A nota geral dada ao item (Value Object com regras de mínimo e máximo).
- **`Comment`** *(String?)*: Texto opcional detalhando a opinião do usuário.
- **`Dimensions`** *(IReadOnlyDictionary<string, int>)*: Dicionário opcional contendo as notas específicas por critério. Ex: `{"Atendimento": 5, "Preço": 3}`.
- **`CreatedAt`** *(DateTimeOffset)*: Data e hora em que a avaliação foi feita.
- **`UpdatedAt`** *(DateTimeOffset?)*: Data e hora da última edição da avaliação.
- **`IsDeleted`** *(Boolean)*: Flag de controle para exclusão lógica.

## 🚀 Use Cases (CQRS)

*(Nota: Marcados com [ ] pois serão os próximos a serem desenvolvidos)*

### ✍️ Comandos (Escrita - Alteram o estado)
- [ ] **CreateEvaluationCommand**: Registra uma nova avaliação de um usuário para um item, validando limites de nota e garantindo que ele não avaliou esse item antes.
- [ ] **UpdateEvaluationCommand**: Permite que o autor edite a sua própria avaliação (alterando nota, dimensões ou comentário).
- [ ] **DeleteEvaluationCommand**: Exclui (Soft Delete) uma avaliação. Pode ser acionado pelo próprio autor ou por um administrador (moderação).

### 📖 Consultas (Leitura - Não alteram o estado)
- [ ] **GetEvaluationByIdQuery**: Traz os detalhes de uma avaliação específica.
- [ ] **ListEvaluationsByItemQuery**: Retorna a lista paginada de todas as avaliações de um `Item`. (Usado para montar a seção de reviews na tela de detalhes do Item).
- [ ] **GetEvaluationByUserAndItemQuery**: Busca rápida para saber se o usuário logado já avaliou o item atual (útil para o frontend saber se exibe o botão "Avaliar" ou "Editar Avaliação").

## 📡 Eventos Produzidos (Integração)
*(Eventos disparados via Service Bus / Wolverine após o sucesso das transações)*
- `EvaluationCreatedEvent` (Útil para disparar o recálculo da nota média do Item no futuro)
- `EvaluationUpdatedEvent`
- `EvaluationDeletedEvent`

## 🌐 Endpoints da API
| Método | Rota HTTP | Handler Responsável | Retorno |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/evaluations` | `CreateEvaluationCommand` | `201 Created` |
| `PUT` | `/api/evaluations/{id}` | `UpdateEvaluationCommand` | `204 No Content` |
| `DELETE` | `/api/evaluations/{id}` | `DeleteEvaluationCommand` | `204 No Content` |
| `GET`  | `/api/items/{itemId}/evaluations` | `ListEvaluationsByItemQuery` | `200 OK (PaginatedList)` |
| `GET`  | `/api/items/{itemId}/evaluations/me` | `GetEvaluationByUserAndItemQuery`| `200 OK (Response / Null)` |
| `GET`  | `/api/evaluations/{id}` | `GetEvaluationByIdQuery` | `200 OK (Response)` |