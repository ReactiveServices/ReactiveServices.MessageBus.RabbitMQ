#language: pt-BR

Funcionalidade: Padrão Publish Subscriber
	Como um componente de software
	Desejo publicar uma mensagem no barramento de mensagens
	Para que outros componentes de software possam receber minha mensagem

@stable @fast
Cenário: Publicar uma mensagem para um assinante
	Dado que um assinante se inscreva para receber a mensagem EventOccurred
	Quando a mensagem EventOccurred for publicada no barramento
	Então a mensagem EventOccurred deve ser entregue ao assinante

@stable @fast
Cenário: Assinar o recebimento de uma mensagem já havendo duas mensagens na fila
	Dado que um assinante solicite a preparação de uma inscrição para receber a mensagem EventOccurred
	E que '2' mensagens do tipo EventOcurred sejam publicadas
	Quando um assinante se inscrever para receber a mensagem EventOccurred
	E mais de '2' segundos se passarem
	Então '2' mensagens EventOccurred devem ser entregues ao assinante

@stable @fast
Cenário: Publicar uma mensagem para dois assinantes
	Dado que um assinante se inscreva para receber a mensagem EventOccurred
	E que outro assinante se inscreva para receber a mensagem EventOccurred
	Quando a mensagem EventOccurred for publicada no barramento
	Então a mensagem EventOccurred deve ser entregue ao assinante
	E a mensagem EventOccurred deve ser entregue ao outro assinante

@stable @fast
Cenário: Publicar duas mensagens para dois assinantes
	Dado que um assinante se inscreva para receber a mensagem EventOccurred
	E que um assinante se inscreva para receber a mensagem OtherEventOccurred
	E que outro assinante se inscreva para receber a mensagem EventOccurred
	E que outro assinante se inscreva para receber a mensagem OtherEventOccurred
	Quando a mensagem EventOccurred for publicada no barramento
	E a mensagem OtherEventOccurred for publicada no barramento
	Então a mensagem EventOccurred deve ser entregue ao assinante
	E a mensagem OtherEventOccurred deve ser entregue ao assinante
	E a mensagem EventOccurred deve ser entregue ao outro assinante
	E a mensagem OtherEventOccurred deve ser entregue ao outro assinante

@stable @fast
Cenário: Publicar uma mensagem sem nenhum assinante
	Dado que nenhum assinante se inscreva para receber a mensagem EventOccurred
	Quando a mensagem EventOccurred for publicada no barramento
	Então a mensagem EventOccurred não deve ser entregue ao assinante

@stable @fast
Cenário: Publicar uma mensagem de um tipo havendo um assinante apenas para mensagens de um outro tipo
	Dado que um assinante se inscreva para receber a mensagem EventOccurred
	Quando a mensagem OtherEventOccurred for publicada no barramento
	Então a mensagem OtherEventOccurred não deve ser entregue ao assinante
	E a mensagem EventOccurred não deve ser entregue ao assinante

@stable @fast
Cenário: Publicar duas mensagens para dois assinantes, um assinante para cada mensagem
	Dado que um assinante se inscreva para receber a mensagem EventOccurred
	E que outro assinante se inscreva para receber a mensagem OtherEventOccurred
	Quando a mensagem EventOccurred for publicada no barramento
	E a mensagem OtherEventOccurred for publicada no barramento
	Então a mensagem EventOccurred deve ser entregue ao assinante
	E a mensagem OtherEventOccurred deve ser entregue ao outro assinante
	E a mensagem OtherEventOccurred não deve ser entregue ao assinante
	E a mensagem EventOccurred não deve ser entregue ao outro assinante

@stable @fast
Cenário: Publicar uma mensagem e constatar que sua validade não expirou
	Dado que jamais tenha havido uma inscrição para a mensagem EventOccurred
	E que um assinante solicite a preparação de uma inscrição para receber a mensagem EventOccurred
	E que a mensagem EventOccurred tenha uma validade de '2' segundos 
	E que a mensagem EventOccurred seja publicada no barramento
	Quando menos de '2' segundos se passarem
	E o assinante se inscrever para receber a mensagem EventOccurred
	Então a mensagem EventOccurred deve ser entregue ao assinante

@stable @fast
Cenário: Publicar uma mensagem e constatar que sua validade expirou
	Dado que jamais tenha havido uma inscrição para a mensagem EventOccurred
	E que um assinante solicite a preparação de uma inscrição para receber a mensagem EventOccurred
	E que a mensagem EventOccurred tenha uma validade de '2' segundos 
	E que a mensagem EventOccurred seja publicada no barramento
	Quando mais de '2' segundos se passarem
	E o assinante se inscrever para receber a mensagem EventOccurred
	Então a mensagem EventOccurred não deve ser entregue ao assinante

@stable @fast
Cenário: Publicar uma mensagem com multiplos campos
	Dado que um assinante se inscreva para receber a mensagem EventOccurred
	E a mensagem EventOccurred é preenchida com informações relevantes
	Quando a mensagem EventOccurred for publicada no barramento
	Então a mensagem EventOccurred deve ser entregue ao assinante
	E todas as informações relevantes devem estar presentes na mensagem EventOccurred

@stable @fast
Cenário: Publicar uma mensagem em um tópico assinado
	Dado que um assinante se inscreva para receber a mensagem EventOccurred no tópico Confirmed
	Quando a mensagem EventOccurred for publicada no tópico Confirmed
	Então a mensagem EventOccurred deve ser entregue ao assinante

@stable @fast
Cenário: Publicar uma mensagem em um tópico não assinado
	Dado que um assinante se inscreva para receber a mensagem EventOccurred no tópico Confirmed
	Quando a mensagem EventOccurred for publicada no tópico Pending
	Então a mensagem EventOccurred não deve ser entregue ao assinante

@stable @fast
Cenário: Limpar as filas de inscrição para um dado tipo de mensagem e id de inscrição
	Dado que haja uma fila de inscrição abandonada para a mensagem EventOccurred e id de inscrição ABC
	Quando for solicitado que a exclusão da fila de inscrição para a mensagem EventOccurred e id de inscrição ABC
	Então a fila de inscrição para a mensagem EventOccurred e id de inscrição ABC deve ser apagada

@stable @fast
Cenário: Preparar uma inscrição futura
	Dado que jamais tenha havido uma inscrição para a mensagem EventOccurred
	E que um assinante solicite a preparação de uma inscrição para receber a mensagem EventOccurred
	E que a mensagem EventOccurred seja publicada no barramento
	Quando o assinante se inscrever para receber a mensagem EventOccurred
	Então a mensagem EventOccurred deve ser entregue ao assinante

@stable @slow
Esquema do Cenário: Publicar um grande volume de uma única mensagem para um único assinante, sem timeout
	Dado que um assinante se inscreva para receber a mensagem EventOccurred
	Quando '<messageCount>' mensagens EventOccurred forem publicadas no barramento
	Então '<messageCount>' mensagens EventOccurred devem ser entregues ao assinante
	
	Exemplos:
		| messageCount	|
		| 1				|
		| 10			|
		| 50			|
		| 100			|

@stable @slow
Esquema do Cenário: Publicar um grande volume de diferentes mensagens para dois assinantes distintos, sem timeout
	Dado que um assinante se inscreva para receber a mensagem EventOccurred
	E que um assinante se inscreva para receber a mensagem OtherEventOccurred
	E que outro assinante se inscreva para receber a mensagem EventOccurred
	E que outro assinante se inscreva para receber a mensagem OtherEventOccurred
	Quando '<messageCount>' mensagens EventOccurred forem publicadas no barramento
	E '<messageCount>' mensagens OtherEventOccurred forem publicadas no barramento
	Então '<messageCount>' mensagens EventOccurred devem ser entregues ao assinante
	E '<messageCount>' mensagens OtherEventOccurred devem ser entregues ao assinante
	E '<messageCount>' mensagens EventOccurred devem ser entregues ao outro assinante
	E '<messageCount>' mensagens OtherEventOccurred devem ser entregues ao outro assinante
	
	Exemplos:
		| messageCount	|
		| 1				|
		| 10			|
		| 50			|
		| 100			|

@stable @slow
Esquema do Cenário: Publicar um grande volume de uma única mensagem para um único assinante, com timeout
	Dado que um assinante se inscreva para receber a mensagem EventOccurred
	E que a mensagem EventOccurred tenha uma validade de '<timeout>' segundos
	Quando '<messageCount>' mensagens EventOccurred forem publicadas no barramento
	Então '<messageCount>' mensagens EventOccurred devem ser entregues ao assinante
	
	Exemplos:
		| messageCount	| timeout	|
		| 1				| 1			|
		| 3				| 1			|
		| 10			| 2			|
		| 40			| 8			|
		| 60			| 16		|
		| 80			| 16		|

@stable @slow
Esquema do Cenário: Publicar um grande volume de diferentes mensagens para dois assinantes distintos, com timeout
	Dado que um assinante se inscreva para receber a mensagem EventOccurred
	E que um assinante se inscreva para receber a mensagem OtherEventOccurred
	E que outro assinante se inscreva para receber a mensagem EventOccurred
	E que outro assinante se inscreva para receber a mensagem OtherEventOccurred
	E que a mensagem EventOccurred tenha uma validade de '<timeout>' segundos
	E que a mensagem OtherEventOccurred tenha uma validade de '<timeout>' segundos
	Quando '<messageCount>' mensagens EventOccurred forem publicadas no barramento
	E '<messageCount>' mensagens OtherEventOccurred forem publicadas no barramento
	Então '<messageCount>' mensagens EventOccurred devem ser entregues ao assinante
	E '<messageCount>' mensagens OtherEventOccurred devem ser entregues ao assinante
	E '<messageCount>' mensagens EventOccurred devem ser entregues ao outro assinante
	E '<messageCount>' mensagens OtherEventOccurred devem ser entregues ao outro assinante
	
	Exemplos:
		| messageCount	| timeout	|
		| 1				| 1			|
		| 5				| 3			|
		| 10			| 6			|
		| 50			| 18		|
		| 100			| 40		|
