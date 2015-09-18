#language: pt-BR

Funcionalidade: Padrão Request Response
	Como um componente de software
	Desejo publicar uma solicitação no barramento de mensagens e receber uma resposta a esta solicitação
	Para que eu posso contar com a participação de outros componentes de software na resolução de meus problemas

@stable @fast
Esquema do Cenário: Publicar uma solicitação e receber uma resposta
	Dado que exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestX'
	E que exista um solicitante 'RequesterA' pronto para enviar solicitações do tipo 'RequestX'
	Quando o solicitante 'RequesterA' enviar uma solicitação 'RX' do tipo 'RequestX' em modo <sincronoOuAssincrono> 
	Então o solicitante 'RequesterA' deve receber uma resposta à solicitação 'RX' enviada pelo solicitado 'ResponderA'
	Exemplos:
		| sincronoOuAssincrono	|
		| sincrono				|
		| assincrono			|

@stable @slow
Esquema do Cenário: Publicar uma solicitação e não receber uma resposta
	Dado que não exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestX'
	E que exista um solicitante 'RequesterA' pronto para enviar solicitações do tipo 'RequestX'
	Quando o solicitante 'RequesterA' enviar uma solicitação 'RX' do tipo 'RequestX' em modo <sincronoOuAssincrono>
	Então o solicitante 'RequesterA' não deve receber uma resposta à solicitação 'RX' enviada pelo solicitado 'ResponderA'
	Exemplos:
		| sincronoOuAssincrono	|
		| sincrono				|
		| assincrono			|

@stable @fast
Esquema do Cenário: Publicar uma solicitação e receber uma resposta com uma inscrição específica
	Dado que exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestX' com uma inscrição SubscriptionX
	E que exista um solicitante 'RequesterA' pronto para enviar solicitações do tipo 'RequestX' com uma inscrição SubscriptionX
	Quando o solicitante 'RequesterA' enviar uma solicitação 'RX' do tipo 'RequestX' em modo <sincronoOuAssincrono> 
	Então o solicitante 'RequesterA' deve receber uma resposta à solicitação 'RX' enviada pelo solicitado 'ResponderA'
	Exemplos:
		| sincronoOuAssincrono	|
		| sincrono				|
		| assincrono			|

@stable @slow
Esquema do Cenário: Publicar uma solicitação e não receber uma resposta devido a inscrição incorreta
	Dado que exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestX' com uma inscrição SubscriptionX
	E que exista um solicitante 'RequesterA' pronto para enviar solicitações do tipo 'RequestX'
	Quando o solicitante 'RequesterA' enviar uma solicitação 'RX' do tipo 'RequestX' em modo <sincronoOuAssincrono>
	Então o solicitante 'RequesterA' não deve receber uma resposta à solicitação 'RX' enviada pelo solicitado 'ResponderA'
	Exemplos:
		| sincronoOuAssincrono	|
		| sincrono				|
		| assincrono			|

@stable @fast
Esquema do Cenário: Publicar duas solicitações de tipos distintos e receber duas respostas de tipos distintos
	Dado que exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestX'
	E que exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestY'
	E que exista um solicitante 'RequesterA' pronto para enviar solicitações do tipo 'RequestX'
	E que exista um solicitante 'RequesterA' pronto para enviar solicitações do tipo 'RequestY'
	Quando o solicitante 'RequesterA' enviar uma solicitação 'RX' do tipo 'RequestX' em modo <sincronoOuAssincrono>
	E o solicitante 'RequesterA' enviar uma solicitação 'RY' do tipo 'RequestY' em modo <sincronoOuAssincrono>
	Então o solicitante 'RequesterA' deve receber uma resposta à solicitação 'RX' enviada pelo solicitado 'ResponderA'
	E o solicitante 'RequesterA' deve receber uma resposta à solicitação 'RY' enviada pelo solicitado 'ResponderA'
	Exemplos:
		| sincronoOuAssincrono	|
		| sincrono				|
		| assincrono			|

@stable @fast
Esquema do Cenário: Publicar duas solicitações de mesmo tipo e receber duas respostas de mesmo tipo
	Dado que exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestX'
	E que exista um solicitante 'RequesterA' pronto para enviar solicitações do tipo 'RequestX'
	Quando o solicitante 'RequesterA' enviar uma solicitação 'RX1' do tipo 'RequestX' em modo <sincronoOuAssincrono>
	E o solicitante 'RequesterA' enviar uma solicitação 'RX2' do tipo 'RequestX' em modo <sincronoOuAssincrono>
	Então o solicitante 'RequesterA' deve receber uma resposta à solicitação 'RX1' enviada pelo solicitado 'ResponderA'
	Então o solicitante 'RequesterA' deve receber uma resposta à solicitação 'RX2' enviada pelo solicitado 'ResponderA'
	Exemplos:
		| sincronoOuAssincrono	|
		| sincrono				|
		| assincrono			|

@stable @fast
Esquema do Cenário: Publicar duas solicitações distintas a partir de um único solicitante a dois solicitados distintos, cada um tratando um único tipo de solicitação
	Dado que exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestX'
	E que exista um solicitado 'ResponderB' aguardando por solicitações do tipo 'RequestY'
	E que exista um solicitante 'RequesterA' pronto para enviar solicitações do tipo 'RequestX'
	E que exista um solicitante 'RequesterA' pronto para enviar solicitações do tipo 'RequestY'
	Quando o solicitante 'RequesterA' enviar uma solicitação 'RX' do tipo 'RequestX' em modo <sincronoOuAssincrono>
	E o solicitante 'RequesterA' enviar uma solicitação 'RY' do tipo 'RequestY' em modo <sincronoOuAssincrono>
	Então o solicitante 'RequesterA' deve receber uma resposta à solicitação 'RX' enviada pelo solicitado 'ResponderA'
	E o solicitante 'RequesterA' deve receber uma resposta à solicitação 'RY' enviada pelo solicitado 'ResponderB'
	Exemplos:
		| sincronoOuAssincrono	|
		| sincrono				|
		| assincrono			|

@stable @slow
Esquema do Cenário: Publicar duas solicitações distintas a partir de dois solicitantes distintos a um único solicitado
	Dado que exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestX'
	E que exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestY'
	E que exista um solicitante 'RequesterA' pronto para enviar solicitações do tipo 'RequestX'
	E que exista um solicitante 'RequesterB' pronto para enviar solicitações do tipo 'RequestY'
	Quando o solicitante 'RequesterA' enviar uma solicitação 'RX' do tipo 'RequestX' em modo <sincronoOuAssincrono>
	E o solicitante 'RequesterB' enviar uma solicitação 'RY' do tipo 'RequestY' em modo <sincronoOuAssincrono>
	Então o solicitante 'RequesterA' deve receber uma resposta à solicitação 'RX' enviada pelo solicitado 'ResponderA'
	E o solicitante 'RequesterB' deve receber uma resposta à solicitação 'RY' enviada pelo solicitado 'ResponderA'
	Exemplos:
		| sincronoOuAssincrono	|
		| sincrono				|
		| assincrono			|

@stable @fast
Esquema do Cenário: Publicar duas solicitações distintas a partir de dois solicitantes distintos a dois solicitados distintos, cada um tratando um único tipo de solicitação
	Dado que exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestX'
	E que exista um solicitado 'ResponderB' aguardando por solicitações do tipo 'RequestY'
	E que exista um solicitante 'RequesterA' pronto para enviar solicitações do tipo 'RequestX'
	E que exista um solicitante 'RequesterB' pronto para enviar solicitações do tipo 'RequestY'
	Quando o solicitante 'RequesterA' enviar uma solicitação 'RX' do tipo 'RequestX' em modo <sincronoOuAssincrono>
	E o solicitante 'RequesterB' enviar uma solicitação 'RY' do tipo 'RequestY' em modo <sincronoOuAssincrono>
	Então o solicitante 'RequesterA' deve receber uma resposta à solicitação 'RX' enviada pelo solicitado 'ResponderA'
	E o solicitante 'RequesterB' deve receber uma resposta à solicitação 'RY' enviada pelo solicitado 'ResponderB'
	Exemplos:
		| sincronoOuAssincrono	|
		| sincrono				|
		| assincrono			|

@unstable @fast
Esquema do Cenário: Publicar duas solicitações do mesmo tipo a partir de dois solicitantes distintos a um único solicitado
	Dado que exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestX'
	E que exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestX'
	E que exista um solicitante 'RequesterA' pronto para enviar solicitações do tipo 'RequestX'
	E que exista um solicitante 'RequesterB' pronto para enviar solicitações do tipo 'RequestX'
	Quando o solicitante 'RequesterA' enviar uma solicitação 'RX1' do tipo 'RequestX' em modo <sincronoOuAssincrono>
	E o solicitante 'RequesterB' enviar uma solicitação 'RX2' do tipo 'RequestX' em modo <sincronoOuAssincrono>
	Então o solicitante 'RequesterA' deve receber uma resposta à solicitação 'RX1' ou uma resposta à solicitação 'RX2'
	E o solicitante 'RequesterB' deve receber uma resposta à solicitação 'RX2' ou uma resposta à solicitação 'RX1'
	Exemplos:
		| sincronoOuAssincrono	|
		| sincrono				|
		| assincrono			|

@unstable @fast
Esquema do Cenário: Publicar duas solicitações do mesmo tipo a partir de dois solicitantes distintos a dois solicitados distintos, cada um tratando um único tipo de solicitação
	Dado que exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestX'
	E que exista um solicitado 'ResponderB' aguardando por solicitações do tipo 'RequestX'
	E que exista um solicitante 'RequesterA' pronto para enviar solicitações do tipo 'RequestX'
	E que exista um solicitante 'RequesterB' pronto para enviar solicitações do tipo 'RequestX'
	Quando o solicitante 'RequesterA' enviar uma solicitação 'RX1' do tipo 'RequestX' em modo <sincronoOuAssincrono>
	E o solicitante 'RequesterB' enviar uma solicitação 'RX2' do tipo 'RequestX' em modo <sincronoOuAssincrono>
	Então o solicitante 'RequesterA' deve receber uma resposta à solicitação 'RX1' ou uma resposta à solicitação 'RX2'
	E o solicitante 'RequesterB' deve receber uma resposta à solicitação 'RX2' ou uma resposta à solicitação 'RX1'
	Exemplos:
		| sincronoOuAssincrono	|
		| sincrono				|
		| assincrono			|

@unstable @slow
Esquema do Cenário: Publicar um grande volume de uma mesma solicitação para dois solicitados distintos
	Dado que exista um solicitado 'ResponderA' aguardando por solicitações do tipo 'RequestX'
	E que exista um solicitado 'ResponderB' aguardando por solicitações do tipo 'RequestX'
	E que exista um solicitante 'RequesterA' pronto para enviar solicitações do tipo 'RequestX'
	Quando o solicitante 'RequesterA' enviar <messageCount> solicitações do tipo 'RequestX' em modo <sincronoOuAssincrono>
	Então o solicitante 'RequesterA' deve receber <messageCount> respostas às solicitações do tipo 'RequestX' enviadas por qualquer solicitado
	E deve haver ao menos uma resposta à solicitação de tipo 'RequestX' que tenha sido enviada pelo solicitado 'ResponderA'
	E deve haver ao menos uma resposta à solicitação de tipo 'RequestX' que tenha sido enviada pelo solicitado 'ResponderB'
	
	Exemplos:
		| messageCount	| sincronoOuAssincrono	|
		| 20			| sincrono				|
		| 20			| assincrono			|
		| 40			| sincrono				|
		| 40			| assincrono			|
