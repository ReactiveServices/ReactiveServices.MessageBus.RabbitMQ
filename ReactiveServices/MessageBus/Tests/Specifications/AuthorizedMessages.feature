#language: pt-BR

Funcionalidade: Authorized Messages
	Como um trocador de mensagens
	Desejo que determinados tipos de mensagens tenham seu envio ou recebimento autorizados por um serviço de autorização
	Para que eu possa manter a troca de mensagens confiável em casos críticos de segurança

@stable @fast
Cenário: Impedir prosseguimento de um envio ou recebimento de mensagem caso o arquivo de configurações esteja ausente
	Dado que o arquivo de configuração de autorização de mensagens não exista
	Quando a operação de envio ou recebimento de mensagens for executada
	Então a operação deve ser abortada com erro devido à ausência do arquivo de configuração de autorização de mensagens

@stable @fast
Cenário: Carregar configurações do arquivo
	Dado que o arquivo de configuração de autorização de mensagens exista
	Quando a operação de envio ou recebimento de mensagens for executada
	Então o arquivo de configuração de autorização de mensagens deve ser carregado para memória e considerado antes de se executar o envio ou recebimento

@stable @fast
Cenário: Autorizar envio e recebimento de mensagens devido ao paramentro AllowByDefault
	Dado que o arquivo de configuração de autorização de mensagens exista
	E que o arquivo de configuração de autorização de mensagens informe que mensagens não descriminadas devem ser autorizadas
	E uma mensagem a ser enviada ou recebida for uma mensagem não discriminada no arquivo de configuração de autorização de mensagens
	Quando a operação de envio ou recebimento de mensagens for executada
	Então o envio ou recebimento deve ser realizado com sucesso

@stable @fast
Cenário: Negar envio e recebimento de mensagens devido ao paramentro DenyByDefault
	Dado que o arquivo de configuração de autorização de mensagens exista
	E que o arquivo de configuração de autorização de mensagens informe que mensagens não descriminadas não devem ser autorizadas
	E uma mensagem a ser enviada ou recebida for uma mensagem não discriminada no arquivo de configuração de autorização de mensagens
	Quando a operação de envio ou recebimento de mensagens for executada
	Então o envio ou recebimento não deve ser realizado

@unstable @fast
Cenário: Autorizar envio e recebimento de mensagens devido a resposta positiva de um serviço de autorização
	Dado que o arquivo de configuração de autorização de mensagens exista
	E uma mensagem a ser enviada ou recebida for uma mensagem discriminada no arquivo de configuração de autorização de mensagens
	E o serviço de autorização de mensagens estiver configurado para autorizar o envio ou recebimento da mensagem
	Quando a operação de envio ou recebimento de mensagens for executada
	Então o envio ou recebimento deve ser realizado com sucesso

@stable @fast
Cenário: Negar envio e recebimento de mensagens devido a resposta negativa de um serviço de autorização
	Dado que o arquivo de configuração de autorização de mensagens exista
	E uma mensagem a ser enviada ou recebida for uma mensagem discriminada no arquivo de configuração de autorização de mensagens
	E o serviço de autorização de mensagens estiver configurado para não autorizar o envio ou recebimento da mensagem
	Quando a operação de envio ou recebimento de mensagens for executada
	Então o envio ou recebimento não deve ser realizado
