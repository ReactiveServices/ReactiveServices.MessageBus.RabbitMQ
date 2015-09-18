#language: pt-BR

Funcionalidade: Storage Types
	Como um trocador de mensagens
	Desejo ser capaz de informar se um determinado tipo de mensagem deve ter seu armazenamento persistente ou transiente
	Para que eu possa evitar que determinados tipos de mensagem gastem recursos de armazenamento persistente sem necessidade

@stable @fast
Cenário: Publicar uma mensagem do tipo persistente, sem preparação prévia e sem reiniciar o servidor
	Dado que um assinante se inscreva para receber a mensagem EventOccurred
	Quando a mensagem EventOccurred for publicada como persistente no barramento
	Então a mensagem EventOccurred deve ser entregue ao assinante

@unstable @fast
Cenário: Publicar uma mensagem do tipo transiente, sem preparação prévia e sem reiniciar o servidor
	Dado que um assinante se inscreva para receber a mensagem EventOccurred
	Quando a mensagem EventOccurred for publicada como transiente no barramento
	Então a mensagem EventOccurred deve ser entregue ao assinante

@stable @slow
Cenário: Publicar uma mensagem do tipo persistente, sem preparação prévia, antes da inscrição e reiniciando o servidor
	Quando a mensagem EventOccurred for publicada como persistente no barramento
	E o servidor de mensageria for reiniciado
	E um assinante se inscrever para receber a mensagem EventOccurred
	Então a mensagem EventOccurred não deve ser entregue ao assinante

@stable @slow
Cenário: Publicar uma mensagem do tipo transiente, sem preparação prévia, antes da inscrição e reiniciando o servidor
	Quando a mensagem EventOccurred for publicada como transiente no barramento
	E o servidor de mensageria for reiniciado
	E um assinante se inscrever para receber a mensagem EventOccurred
	Então a mensagem EventOccurred não deve ser entregue ao assinante

@stable @slow
Cenário: Publicar uma mensagem do tipo persistente, com preparação prévia, antes da inscrição e reiniciando o servidor
	Dado que seja preparada uma inscrição para a mensagem EventOccurred
	Quando a mensagem EventOccurred for publicada como persistente no barramento
	E o servidor de mensageria for reiniciado
	E um assinante se inscrever para receber a mensagem EventOccurred
	Então a mensagem EventOccurred deve ser entregue ao assinante

@stable @slow
Cenário: Publicar uma mensagem do tipo transiente, com preparação prévia, antes da inscrição e reiniciando o servidor
	Dado que seja preparada uma inscrição para a mensagem EventOccurred
	Quando a mensagem EventOccurred for publicada como transiente no barramento
	E o servidor de mensageria for reiniciado
	E um assinante se inscrever para receber a mensagem EventOccurred
	Então a mensagem EventOccurred não deve ser entregue ao assinante
