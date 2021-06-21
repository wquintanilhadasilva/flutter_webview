# POC

POC para aplicativo em Flutter, autenticando com redes sociais e um provedor OpenID, exibindo a tela de login em webview e após autenticação, sair do webview para as views nativas em flutter.

A aplicação flutter, após receber o login com sucesso (escutando evento de transição de página na webview), recupera os cookies visto que o jwt-token vem num cookie e o armazena para ser utilizado nas requisições aos microsserviços.

O plugin utilizado para isso foi o flutter_inappwebview.

Todo o processo de autenticação é realizado no backend que é desenvolvido em .net5.

## Back-end

Dois projetos no backend:

* Projeto SSO: Responsável por realizar a autenticação no Facebook, no Google, no Twitter e no OpenID (usei o keyckloak mas o foco é o acesso.gov (precisa cadastrar a aplicação lá no ministério da economia))
* Projeto WebApi: Microsserviços que serão chamados após autenticação.

O backend, após receber a confirmação da autenticação no provedor externo, gera um token chamado jwt-token e o devolve como um cookie para o client usar para chamar os microsserviços
