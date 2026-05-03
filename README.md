# 🚗 Sistema de Estacionamento — C# e POO

![Banner](https://github.com/user-attachments/assets/f705c2dd-2a66-4738-96c0-35cbbdc3fc8f)

> **Bootcamp WEX — End to End Engineering · DIO**

[![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-Language-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![POO](https://img.shields.io/badge/Paradigma-POO-0078D7?style=for-the-badge)](https://en.wikipedia.org/wiki/Object-oriented_programming)
[![LINQ](https://img.shields.io/badge/LINQ-Queries-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/linq/)
[![Status](https://img.shields.io/badge/Status-Concluído-00b300?style=for-the-badge)]()

---

## 1. Problema de Negócio

Estacionamentos que controlam entradas e saídas de veículos manualmente estão expostos a três falhas operacionais recorrentes: **cobranças incorretas** por cálculo manual de horas, **registro de placas duplicadas** que geram inconsistência no controle de vagas, e **remoção de veículos inexistentes** que cria estados inválidos no sistema.

O desafio foi construir um sistema que resolva esses três problemas simultaneamente: controlando o inventário de veículos em tempo real, validando entradas duplicadas antes do registro e calculando o valor a cobrar de forma automática e auditável no momento da saída.

---

## 2. Contexto

O sistema foi desenvolvido como desafio de projeto no **Bootcamp WEX — End to End Engineering**, com foco em aplicar os fundamentos de **Programação Orientada a Objetos (POO)** e **LINQ** em C# sobre um domínio operacional concreto.

A operação do estacionamento envolve três ações principais, cada uma com regras próprias:

- **Adicionar veículo** — registra a placa na lista de veículos ativos, com validação contra entradas vazias e duplicatas
- **Remover veículo** — verifica a existência da placa, coleta as horas estacionadas e calcula o valor total a cobrar
- **Listar veículos** — exibe o inventário atual com numeração de vagas, ou informa que o estacionamento está vazio

O modelo de precificação usa dois parâmetros configuráveis na inicialização: **preço inicial fixo** (taxa de entrada) e **preço por hora** (tarifa variável). Isso torna o sistema adaptável a diferentes políticas comerciais sem alteração de código.

---

## 3. Premissas da Análise

- As placas são armazenadas em **maiúsculas** (`ToUpper()`) como padrão de normalização, eliminando divergências de comparação por capitalização
- A busca de placas usa `StringComparison.OrdinalIgnoreCase`, garantindo que `abc1234` e `ABC1234` sejam tratadas como a mesma placa tanto na adição quanto na remoção
- A entrada de horas usa `int.TryParse()` em vez de `Convert.ToInt32()`, prevenindo exceções não tratadas por entrada inválida do usuário
- O preço inicial e o preço por hora são do tipo `decimal`, garantindo precisão monetária sem erros de arredondamento de ponto flutuante
- O menu do `Program.cs` opera em loop contínuo com `Console.Clear()` entre interações, simulando a experiência de um sistema de terminal profissional
- O escopo é um console application interativo, sem persistência em banco de dados — os dados existem apenas durante a execução

---

## 4. Estratégia da Solução

O desenvolvimento seguiu uma separação clara entre **interface com o usuário** e **lógica de negócio**:

**Passo 1 — Separação de responsabilidades**
`Estacionamento.cs` contém exclusivamente a lógica de negócio (validação, cálculo, gerenciamento de lista). `Program.cs` contém exclusivamente a interface com o usuário (menu, leitura de inputs, fluxo de navegação). Essa separação segue o princípio de responsabilidade única.

**Passo 2 — Inicialização com estado configurável**
O construtor de `Estacionamento` recebe `precoInicial` e `precoPorHora` como parâmetros, externalizando a política de preços para o ponto de entrada. O sistema não tem valores hardcoded — qualquer estacionamento pode usar o mesmo código com tarifas diferentes.

**Passo 3 — Validação em camadas no `AdicionarVeiculo()`**
Dois guards sequenciais: primeiro verifica se a placa está vazia ou nula, depois verifica se ela já existe na lista com comparação case-insensitive. Somente após passar nas duas verificações o veículo é registrado.

**Passo 4 — Remoção segura com cálculo integrado no `RemoverVeiculo()`**
A operação de remoção só avança se a placa existe (verificada via `.Any()`). Após confirmar a existência, coleta as horas com `int.TryParse()` para validação segura, calcula o valor (`precoInicial + precoPorHora × horas`) e usa `.First()` + `.Remove()` para remover a entrada exata da lista.

**Passo 5 — Listagem com numeração de vagas**
O loop `for` com índice `i` permite exibir `Vaga {i+1}` para cada veículo, transformando um índice zero-based em uma numeração de vaga legível para o operador.

---

## 5. Insights Técnicos

A implementação revela decisões de design com impacto direto na confiabilidade do sistema:

**`int.TryParse()` vs `Convert.ToInt32()`**
`Convert.ToInt32()` lança `FormatException` se o usuário digitar um valor não numérico. `int.TryParse()` retorna `false` sem exceção, permitindo exibir uma mensagem amigável e manter o programa em execução. Em sistemas interativos de terminal, onde a entrada humana é imprevisível, a versão segura não é opcional.

**`.Any()` vs `.Count > 0` para verificação de existência**
`.Any()` com predicado (`veiculos.Any(v => v.Equals(...))`) encerra a busca assim que encontra o primeiro match — comportamento de short-circuit. `.Count > 0` percorre toda a lista antes de retornar. Em listas longas, a diferença de performance é significativa. Além disso, `.Any()` é mais semântico: expressa "existe algum" em vez de "quantos existem".

**`.First()` + `.Remove()` para remoção precisa**
`veiculos.Remove(veiculos.First(x => x.Equals(placa, ...)))` garante que a placa removida é exatamente a que foi encontrada na busca, com a mesma comparação case-insensitive. Remover diretamente pela string digitada pelo usuário poderia falhar se a capitalização da placa armazenada fosse diferente.

**`StringComparison.OrdinalIgnoreCase` como padrão de comparação**
Usar `OrdinalIgnoreCase` em vez de `.ToUpper()` para comparação evita problemas de culture-specific casing em ambientes com locale diferente do padrão. É a forma recomendada pela Microsoft para comparações de strings que não dependem de idioma.

**`decimal` para aritmética monetária**
`precoInicial` e `precoPorHora` são `decimal`, não `double`. A diferença: `double` usa representação binária de ponto flutuante, que acumula imprecisão em operações repetidas. Em sistemas de cobrança, `R$ 0.01` de diferença por operação se torna `R$ 10,00` em mil cobranças.

---

## 6. Resultados

O sistema entrega as seguintes garantias operacionais validadas interativamente:

```
✅ Cadastro de veículo com validação de placa vazia
✅ Bloqueio de placa duplicada com mensagem descritiva
✅ Cálculo automático de cobrança: precoInicial + (precoPorHora × horas)
✅ Remoção com verificação de existência antes da operação
✅ Listagem com numeração de vagas em tempo real
✅ Entrada de horas com tratamento de valor inválido (TryParse)
```

Exemplo de fluxo completo no terminal:

```
Seja bem vindo ao sistema de estacionamento!
Digite o preço inicial: 5
Agora digite o preço por hora: 10

[Menu]
1 - Cadastrar veículo  → Placa: ABC1234 → "Veículo estacionado com sucesso!"
3 - Listar veículos    → "Vaga 1: ABC1234"
2 - Remover veículo    → Placa: ABC1234, Horas: 3
                       → "O veículo 'ABC1234' foi removido e o preço total foi de: R$ 35,00"
                          (5 + 10×3 = 35)
```

---

## 7. Próximos Passos

- [ ] Adicionar **data e hora de entrada** no registro do veículo, calculando as horas automaticamente no momento da saída sem depender de input manual
- [ ] Implementar **persistência em arquivo JSON** para manter o estado do estacionamento entre execuções do programa
- [ ] Modelar **tipos de vaga** (moto, carro, caminhão) com tarifas e capacidades diferentes por categoria
- [ ] Adicionar **testes unitários com xUnit** cobrindo os cenários de placa duplicada, remoção de placa inexistente e cálculo com e sem horas
- [ ] Evoluir para uma **API REST com ASP.NET Core**, expondo endpoints de entrada e saída de veículos consumíveis por uma interface web ou mobile

---

## 🗂️ Estrutura do Projeto

```
DesafioFundamentos/
├── Models/
│   └── Estacionamento.cs   # Lógica de negócio: add, remove, lista e cálculo
├── Program.cs              # Interface de terminal com menu interativo
├── DesafioFundamentos.md   # Instruções de execução
└── DesafioFundamentos.csproj
```

---

## 🛠️ Tecnologias Utilizadas

| Tecnologia | Versão | Papel no Projeto |
|---|---|---|
| C# / .NET | 6.0 | Linguagem e plataforma de execução |
| LINQ | — | Consultas e validações sobre a lista de veículos |
| POO | — | Encapsulamento de lógica de negócio na classe `Estacionamento` |
| Git + GitHub | — | Versionamento e documentação |

---

## ▶️ Como Executar

**Pré-requisito:** [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6) ou superior instalado.

```bash
# Clone o repositório
git clone https://github.com/Santosdevbjj/sistema-estacionamento.git
cd sistema-estacionamento

# Execute a aplicação
dotnet run
```

O programa solicitará o preço inicial e o preço por hora antes de exibir o menu. Navegue pelas opções digitando `1`, `2`, `3` ou `4`.

```
Seja bem vindo ao sistema de estacionamento!
Digite o preço inicial: _
Agora digite o preço por hora: _
```

---

## 📐 Decisões Técnicas

**Por que `private` nos campos `precoInicial`, `precoPorHora` e `veiculos`?**
Encapsular os campos como `private` garante que nenhum código externo possa alterar diretamente a lista de veículos ou os preços após a criação do objeto. Toda operação passa obrigatoriamente pelos métodos públicos, que contêm as validações. Isso é encapsulamento aplicado à proteção de invariantes de domínio.

**Por que o menu está em `Program.cs` e não em `Estacionamento.cs`?**
Misturar lógica de negócio com lógica de interface torna o código difícil de testar e de reusar. Com a separação atual, é possível instanciar `Estacionamento` em um teste unitário sem precisar simular `Console.ReadLine()`. A classe de negócio não sabe que existe um terminal — ela apenas expõe métodos.

**Por que o preço é calculado no momento da remoção e não no momento da entrada?**
O sistema de cobrança por hora pressupõe que o tempo de permanência só é conhecido na saída. Registrar um valor estimado na entrada criaria inconsistência se o usuário informasse horas incorretas. Centralizar o cálculo na remoção é a abordagem que reflete o fluxo real do negócio.

---

## 🧠 Aprendizados

O maior aprendizado foi entender a diferença prática entre **encapsulamento como conceito** e **encapsulamento como proteção**. Em exemplos didáticos, `private` parece apenas uma convenção. Neste projeto ficou claro o motivo real: sem `private` na lista `veiculos`, qualquer parte do programa poderia adicionar uma placa sem passar pela validação de duplicata — quebrando silenciosamente uma regra de negócio.

O desafio técnico mais interessante foi a remoção com comparação case-insensitive. A primeira versão usava `veiculos.Remove(placa)` diretamente, o que falhava quando a placa armazenada estava em maiúsculas e o usuário digitava em minúsculas. A correção com `.First(x => x.Equals(..., OrdinalIgnoreCase))` resolve o problema na raiz.

O que faria diferente: adicionaria o timestamp de entrada desde o início, eliminando a dependência de input manual de horas — que é o ponto de maior risco de erro humano em toda a operação.

---

## 🤝 Contribuição

Sugestões de novas funcionalidades, refatorações ou cenários de teste são bem-vindos. Abra uma issue ou envie um pull request.

---

## 📬 Contato

[![Portfólio](https://img.shields.io/badge/Portfólio-Sérgio_Santos-111827?style=for-the-badge&logo=githubpages&logoColor=00eaff)](https://portfoliosantossergio.vercel.app)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Sérgio_Santos-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://linkedin.com/in/santossergioluiz)
[![GitHub](https://img.shields.io/badge/GitHub-Santosdevbjj-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Santosdevbjj)
