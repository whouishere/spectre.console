# O que este fork é?

Este é um fork do repósitorio principal do `Spectre.Console` com o código de navegação no prompt de texto do [atreyurock](https://github.com/atreyurock/spectre.console).

Eu precisei dessas mudanças, porém elas foram [rejeitadas](https://github.com/spectreconsole/spectre.console/pull/586#issuecomment-1172191862) por causa de um [CLA](https://drewdevault.com/2018/10/05/Dont-sign-a-CLA.html) [estúpido](https://drewdevault.com/2023/07/04/Dont-sign-a-CLA-2.html).

Todos os créditos vão ao [atreyurock](https://github.com/atreyurock) e [matkoch](https://github.com/matkoch). Eu apenas fiz um _merge_ do código deles sem quaisquer modificações.

Caso esse fork esteja desatualizada com o repositório do `Spectre.Console`, por favor abra uma _issue_ no GitHub. Eu irei tentar adicionar os commits mais recentes o mais rápido possível.

# `Spectre.Console`

_[![Spectre.Console NuGet Versão](https://img.shields.io/nuget/v/spectre.console.svg?style=flat&label=NuGet%3A%20Spectre.Console)](https://www.nuget.org/packages/spectre.console)_

Uma biblioteca .NET que torna mais fácil criar aplicativos de console bonitos e multiplataforma. 
É fortemente inspirada na excelente [biblioteca Rich](https://github.com/willmcgugan/rich) 
para Python.

## Índice de Conteúdo

1. [Funcionalidades](#funcionalidades)
2. [Instalação](#instalação)
3. [Documentação](#documentação)
4. [Exemplos](#exemplos)
5. [Patrocinadores](#patrocinadores)
5. [Licença](#licença)

## Funcionalidades

* Desenvolvida com testes unitários em mente.
* Suporta tabelas, grades, painéis, e uma linguagem de marcação inspirada em [rich](https://github.com/willmcgugan/rich).
* Suporta os parâmetros SRG mais comuns quando se trata de estilo de texto, 
  como negrito, esmaecido, itálico, sublinhado, tachado 
  e texto piscando.
* Suporta cores de 3/4/8/24 bits no terminal.
  A biblioteca detectará os recursos do terminal atual 
  e reduz as cores conforme necessário.

![Exemplo](docs/input/assets/images/example.png)

## Instalação

A maneira mais rápida de começar a usar o `Spectre.Console` é instalar o pacote NuGet.

```csharp
dotnet add package Spectre.Console
```

## Documentação

A documentação do `Spectre.Console` pode ser encontrada em 
https://spectreconsole.net/

## Exemplos

Para ver o `Spectre.Console` em ação, instale a ferramenta global 
[dotnet-example](https://github.com/patriksvensson/dotnet-example).

```
> dotnet tool restore
```

Agora você pode listar os exemplos disponíveis neste repositório:

```
> dotnet example
```

E para executar um exemplo:

```
> dotnet example tables
```

## Patrocinadores

As seguintes pessoas estão [patrocinando](https://github.com/sponsors/patriksvensson)
o Spectre.Console para mostrar o seu apoio e garantir a longevidade do projeto.

* [Rodney Littles II](https://github.com/RLittlesII)
* [Martin Björkström](https://github.com/bjorkstromm)
* [Dave Glick](https://github.com/daveaglick)
* [Kim Gunanrsson](https://github.com/kimgunnarsson)
* [Andrew McClenaghan](https://github.com/andymac4182)
* [C. Augusto Proiete](https://github.com/augustoproiete)
* [Viktor Elofsson](https://github.com/vktr)
* [Steven Knox](https://github.com/stevenknox)
* [David Pendray](https://github.com/dpen2000)
* [Elmah.io](https://github.com/elmahio)

Eu estou muito agradecido. 
**Muito obrigado!**

## Licença

Copyright © Patrik Svensson, Phil Scott, Nils Andresen

Spectre.Console é fornecido no estado em que se encontra sob a licença do MIT. Para obter mais informações, consulte o arquivo [LICENSE](LICENSE.md).

* Para SixLabors.ImageSharp, consulte https://github.com/SixLabors/ImageSharp/blob/master/LICENSE