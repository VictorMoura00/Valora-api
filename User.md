# 👤 Users (Shadow Users)

## 📌 Domain Overview
O **User** no sistema Valora atua como um *Shadow User* (Usuário Sombra). Como a autenticação (login, senha, MFA) é delegada a um Identity Provider externo (ex: Keycloak), esta entidade existe puramente para sustentar as regras de negócio do nosso domínio (DDD), como autoria de avaliações, moderação e métricas de uso.

Ele armazena apenas o mínimo necessário para a aplicação funcionar (LGPD-friendly), reconhecendo o usuário pelo ID do token JWT (`sub`), seu `Email` e um `Nickname` público escolhido por ele.

## 📜 Business Rules
- **Sincronia de Identidade:** O `Id` do usuário no banco do Valora deve ser **exatamente o mesmo** `Id` (Claim `sub`) gerado pelo Identity Provider externo.
- **Just-In-Time Provisioning (JIT):** O registro do usuário é criado silenciosamente no banco do Valora no momento do seu primeiro acesso válido à API, extraindo o e-mail e o ID do JWT.
- **Privacidade e LGPD:** O sistema não armazena dados sensíveis ou senhas. O `Nickname` é a identidade pública do usuário nas avaliações e pode ser definido posteriormente (opcional na criação).
- **Métricas Nativas:** A propriedade herdada `CreatedAt` atua como a data do **Primeiro Login**. Sempre que o usuário interagir com o sistema logado, a data de **Último Login** (`LastLoginAt`) deve ser atualizada.
- **Moderação (Soft Ban):** Um usuário pode ser bloqueado (`IsBlocked = true`) por administradores em caso de violação de regras. Usuários bloqueados podem navegar e ler dados, mas são impedidos de criar ou editar `Evaluations`.
- **Exclusão Lógica (Soft Delete):** Se o usuário solicitar a exclusão da conta, aplicamos o Soft Delete. Suas avaliações passadas podem ser anonimizadas (ex: exibir "Usuário Excluído" em vez do Nickname), mas as notas médias dos itens não são quebradas.

## 🏛️ Entity Properties

- **`Id`** *(Guid)*: Identificador único (Sincronizado com a claim `sub` do JWT externo).
- **`Email`** *(String)*: E-mail de contato e chave secundária de identificação.
- **`Nickname`** *(String?)*: Nome de exibição público nas avaliações. Nulo até ser configurado na primeira sessão.
- **`LastLoginAt`** *(DateTimeOffset)*: Data e hora da última vez que o usuário consumiu a API autenticado.
- **`IsBlocked`** *(Boolean)*: Flag de moderação. Se `true`, o usuário não pode interagir (Postar/Editar avaliações).
- **`CreatedAt`** *(DateTimeOffset)*: Data e hora do **Primeiro Login** (criação do Shadow User).
- **`UpdatedAt`** *(DateTimeOffset?)*: Data e hora da última alteração de perfil.
- **`IsDeleted`** *(Boolean)*: Flag de controle para exclusão lógica.

## 🚀 Use Cases (CQRS)

*(Nota: Marcados com [ ] pois serão os próximos a serem desenvolvidos)*

### ✍️ Comandos (Escrita - Alteram o estado)
- [ ] **SyncUserLoginCommand**: Comando leve (geralmente acionado via middleware ou no primeiro carregamento do Frontend) que verifica se o usuário existe. Se não existir, cria. Se existir, atualiza o `LastLoginAt`.
- [ ] **UpdateProfileCommand**: Permite ao usuário logado definir ou alterar o seu `Nickname`.
- [ ] **BlockUserCommand / UnblockUserCommand**: Comandos restritos para uso de administradores para moderar contas maliciosas.

### 📖 Consultas (Leitura - Não alteram o estado)
- [ ] **GetLoggedUserProfileQuery**: Retorna os dados do usuário atualmente logado (extraindo o ID via `ICurrentUserService`).
- [ ] **CheckNicknameAvailabilityQuery**: Consulta rápida para o Frontend validar se um nickname digitado no formulário já está em uso por outra pessoa.

## 📡 Eventos Produzidos (Integração)
*(Eventos disparados via Service Bus / Wolverine após o sucesso das transações)*
- `UserRegisteredEvent` (Disparado apenas no primeiro login)
- `UserProfileUpdatedEvent`
- `UserBlockedEvent`

## 🌐 Endpoints da API
| Método | Rota HTTP | Handler Responsável | Retorno |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/users/sync` | `SyncUserLoginCommand` | `200 OK` |
| `PUT` | `/api/users/me/profile` | `UpdateProfileCommand` | `204 No Content` |
| `GET`  | `/api/users/me` | `GetLoggedUserProfileQuery` | `200 OK (ProfileDto)` |
| `GET`  | `/api/users/nickname-available` | `CheckNicknameAvailabilityQuery`| `200 OK (Boolean)` |
| `PUT`  | `/api/users/{id}/block` | `BlockUserCommand` | `204 No Content (Admin)` |
| `PUT`  | `/api/users/{id}/unblock` | `UnblockUserCommand` | `204 No Content (Admin)` |