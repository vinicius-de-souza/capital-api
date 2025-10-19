# API de Carteira de Investimentos

Esta � uma API RESTful para gerenciamento de uma carteira de investimentos pessoais, desenvolvida como um projeto de avalia��o. A API permite ao usu�rio registrar opera��es de compra e venda de ativos, al�m de consultar um resumo consolidado da sua carteira.

[![CI](https://github.com/vinicius-de-souza/capital-api/actions/workflows/ci.yml/badge.svg)](https://github.com/vinicius-de-souza/capital-api/actions/workflows/ci.yml)

## Como Executar (Requisito: "Comando �nico")

O projeto � 100% containerizado com Docker. O �nico pr�-requisito para executar a aplica��o completa (API + Banco de Dados) � ter o **Docker Desktop** instalado e em execu��o.

### Passo 1: Obter o C�digo

```bash
git clone https://github.com/vinicius-de-souza/capital-api.git
cd capital-api
```

### Passo 2: Modificar o Arquivo de Senha (`.env`)

Por raz�es de seguran�a, a senha do banco de dados n�o est� no c�digo. Modifique o `.env` na raiz do projeto e defina uma senha:

```bash
DB_PASSWORD=SuaSenhaForteAqui123
```

### Passo 3: Executar com Docker Compose

Este � o "comando �nico" que levanta todo o ambiente:

```bash
docker-compose up --build
```

**O que este comando faz?**

1. **Constr�i** a imagem Docker da API .NET (usando o `Dockerfile`).
2. **Baixa** a imagem oficial do MySQL 8.0.
3. **Inicia** os dois containers (API e Banco de Dados) numa rede privada.
4. **Aguarda** o banco de dados ficar 100% pronto (usando o `healthcheck`).
5. **Aplica** automaticamente as *migrations* do Entity Framework (criando as tabelas `Ativos` e `Transacoes`).
6. **Inicia** a API .NET.

### Passo 4: Acessar a Aplica��o

Ap�s a execu��o, a aplica��o estar� dispon�vel:

* **Swagger (UI):** `http://localhost:8080/swagger`
* **Banco de Dados (Host):** `localhost:3306` (Usu�rio: `root`, Senha: a que voc� definiu no `.env`)

---

## Decis�es de Arquitetura e Padr�es de Projeto

O objetivo foi construir uma aplica��o robusta, test�vel e manuten�vel, seguindo os padr�es da ind�stria.

* **Clean Architecture:** A solu��o � segregada em quatro camadas (Dom�nio, Aplica��o, Infraestrutura e API). Isso garante a separa��o de responsabilidades e um n�cleo de neg�cio agn�stico de tecnologia.
* **Domain-Driven Design (DDD):** A l�gica de neg�cio (invariantes) est� protegida dentro das "Entidades Ricas" (`Ativo` e `Transacao`). Regras como "saldo insuficiente para venda" ou o "c�lculo do pre�o m�dio ponderado" est�o no Dom�nio, garantindo consist�ncia.
* **CQRS (Command Query Responsibility Segregation):** Foi implementado um "CQRS simples" com a biblioteca **MediatR**. As opera��es de escrita (ex: `ComprarAtivoCommand`) s�o separadas das opera��es de leitura (ex: `ObterResumoCarteiraQuery`), simplificando os *handlers* e suas depend�ncias.
* **Padr�o Reposit�rio e Unit of Work:** A camada de Infraestrutura abstrai o acesso a dados usando o padr�o Reposit�rio, e o `IUnitOfWork` garante que as transa��es (ex: atualizar um `Ativo` e adicionar uma `Transacao`) sejam salvas de forma at�mica (`SaveChangesAsync`).
* **Middleware de Exce��o Global:** Em vez de blocos `try-catch` nos *Controllers*, um *middleware* customizado captura todas as exce��es (incluindo `DomainException` ou `ArgumentException`) e as traduz em respostas `HTTP 400` ou `HTTP 500` padronizadas.
* **Logging Estruturado com Serilog:** Toda a aplica��o utiliza logging estruturado. Um *middleware* de **Correlation ID** foi implementado para anexar um ID de rastreabilidade a cada log de uma requisi��o, facilitando a depura��o.

---

## Estrat�gia de Testes

A estrat�gia de testes focou em cobrir os pontos mais cr�ticos da arquitetura, conforme solicitado na avalia��o:

1. **Testes de Unidade (Dom�nio):** Focados em validar a l�gica de neg�cio pura (c�lculo de pre�o m�dio, valida��es de saldo, etc.) sem qualquer depend�ncia externa.
2. **Testes de Unidade (Aplica��o):** Focados em validar a l�gica de orquestra��o dos *handlers* do MediatR. Foi utilizada a biblioteca **Moq** para simular as depend�ncias (Reposit�rio e Unit of Work) e garantir que os fluxos corretos s�o executados.

### Testes N�o Implementados (Pr�ximos Passos)

* **Testes de Integra��o:** Para este *showcase*, n�o foram implementados testes de integra��o (que validariam a camada de Infraestrutura e a conex�o real com o banco de dados). Em um projeto de produ��o, este seria o pr�ximo passo l�gico para garantir que os mapeamentos do EF Core e a string de conex�o est�o corretos.

---

## Continuous Integration (CI)

Conforme o requisito, o reposit�rio possui integra��o com um servi�o de CI (GitHub Actions).

O pipeline, definido em `.github/workflows/ci.yml`, � disparado a cada `push` ou `pull request` e executa automaticamente os seguintes passos:

1. Configura o ambiente .NET 8.
2. Restaura as depend�ncias (NuGet).
3. Compila toda a solu��o em modo `Release`.
4. **Executa a su�te completa de Testes Unit�rios (`dotnet test`)**.

Isso garante que o c�digo no reposit�rio esteja sempre �ntegro e que os testes estejam a passar.
