# API de Carteira de Investimentos

Esta é uma API RESTful para gerenciamento de uma carteira de investimentos pessoais, desenvolvida como um projeto de avaliação. A API permite ao usuário registrar operações de compra e venda de ativos, além de consultar um resumo consolidado da sua carteira.

[![CI](https://github.com/vinicius-de-souza/capital-api/actions/workflows/ci.yml/badge.svg)](https://github.com/vinicius-de-souza/capital-api/actions/workflows/ci.yml)

## Como Executar (Requisito: "Comando Único")

O projeto é 100% containerizado com Docker. O único pré-requisito para executar a aplicação completa (API + Banco de Dados) é ter o **Docker Desktop** instalado e em execução.

### Passo 1: Obter o Código

```bash
git clone https://github.com/vinicius-de-souza/capital-api.git
cd capital-api
```

### Passo 2: Modificar o Arquivo de Senha (`.env`)

Por razões de segurança, a senha do banco de dados não está no código. Modifique o `.env` na raiz do projeto e defina uma senha:

```bash
DB_PASSWORD=SuaSenhaForteAqui123
```

### Passo 3: Executar com Docker Compose

Este é o "comando único" que levanta todo o ambiente:

```bash
docker-compose up --build
```

**O que este comando faz?**

1. **Constrói** a imagem Docker da API .NET (usando o `Dockerfile`).
2. **Baixa** a imagem oficial do MySQL 8.0.
3. **Inicia** os dois containers (API e Banco de Dados) numa rede privada.
4. **Aguarda** o banco de dados ficar 100% pronto (usando o `healthcheck`).
5. **Aplica** automaticamente as *migrations* do Entity Framework (criando as tabelas `Ativos` e `Transacoes`).
6. **Inicia** a API .NET.

### Passo 4: Acessar a Aplicação

Após a execução, a aplicação estará disponível:

* **Swagger (UI):** `http://localhost:8080/swagger`
* **Banco de Dados (Host):** `localhost:3306` (Usuário: `root`, Senha: a que você definiu no `.env`)

---

## Decisões de Arquitetura e Padrões de Projeto

O objetivo foi construir uma aplicação robusta, testável e manutenível, seguindo os padrões da indústria.

* **Clean Architecture:** A solução é segregada em quatro camadas (Domínio, Aplicação, Infraestrutura e API). Isso garante a separação de responsabilidades e um núcleo de negócio agnóstico de tecnologia.
* **Domain-Driven Design (DDD):** A lógica de negócio (invariantes) está protegida dentro das "Entidades Ricas" (`Ativo` e `Transacao`). Regras como "saldo insuficiente para venda" ou o "cálculo do preço médio ponderado" estão no Domínio, garantindo consistência.
* **CQRS (Command Query Responsibility Segregation):** Foi implementado um "CQRS simples" com a biblioteca **MediatR**. As operações de escrita (ex: `ComprarAtivoCommand`) são separadas das operações de leitura (ex: `ObterResumoCarteiraQuery`), simplificando os *handlers* e suas dependências.
* **Padrão Repositório e Unit of Work:** A camada de Infraestrutura abstrai o acesso a dados usando o padrão Repositório, e o `IUnitOfWork` garante que as transações (ex: atualizar um `Ativo` e adicionar uma `Transacao`) sejam salvas de forma atômica (`SaveChangesAsync`).
* **Middleware de Exceção Global:** Em vez de blocos `try-catch` nos *Controllers*, um *middleware* customizado captura todas as exceções (incluindo `DomainException` ou `ArgumentException`) e as traduz em respostas `HTTP 400` ou `HTTP 500` padronizadas.
* **Logging Estruturado com Serilog:** Toda a aplicação utiliza logging estruturado. Um *middleware* de **Correlation ID** foi implementado para anexar um ID de rastreabilidade a cada log de uma requisição, facilitando a depuração.

---

## Estratégia de Testes

A estratégia de testes focou em cobrir os pontos mais críticos da arquitetura, conforme solicitado na avaliação:

1. **Testes de Unidade (Domínio):** Focados em validar a lógica de negócio pura (cálculo de preço médio, validações de saldo, etc.) sem qualquer dependência externa.
2. **Testes de Unidade (Aplicação):** Focados em validar a lógica de orquestração dos *handlers* do MediatR. Foi utilizada a biblioteca **Moq** para simular as dependências (Repositório e Unit of Work) e garantir que os fluxos corretos são executados.

### Testes Não Implementados (Próximos Passos)

* **Testes de Integração:** Para este *showcase*, não foram implementados testes de integração (que validariam a camada de Infraestrutura e a conexão real com o banco de dados). Em um projeto de produção, este seria o próximo passo lógico para garantir que os mapeamentos do EF Core e a string de conexão estão corretos.

---

## Continuous Integration (CI)

Conforme o requisito, o repositório possui integração com um serviço de CI (GitHub Actions).

O pipeline, definido em `.github/workflows/ci.yml`, é disparado a cada `push` ou `pull request` e executa automaticamente os seguintes passos:

1. Configura o ambiente .NET 8.
2. Restaura as dependências (NuGet).
3. Compila toda a solução em modo `Release`.
4. **Executa a suíte completa de Testes Unitários (`dotnet test`)**.

Isso garante que o código no repositório esteja sempre íntegro e que os testes estejam a passar.
