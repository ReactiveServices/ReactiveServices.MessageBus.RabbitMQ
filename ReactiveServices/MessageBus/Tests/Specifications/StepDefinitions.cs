using FluentAssertions;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ReactiveServices.Extensions;
using TechTalk.SpecFlow;

namespace ReactiveServices.MessageBus.Tests.Specifications
{
    [Binding]
    [Log(AttributeExclude = true)]
    [LogException(AttributeExclude = true)]
    sealed class StepDefinitions : IDisposable
    {
        private StepDefinitionsExecutor Executor;

        static StepDefinitions()
        {
            AppDomain.CurrentDomain.LogExceptions();
        }

        [BeforeScenario]
        public void Setup()
        {
            Executor = new StepDefinitionsExecutor();

            Executor.InitializeDependencyResolver();
            Executor.InitializeMessageBus();
            Executor.ResetInMemoryTestData();
            Executor.RemoveRequestQueues();
        }

        [AfterScenario]
        public void TearDown()
        {
            Executor.RemoveAllSubscriptions();
            Executor.RemoveRequestQueues();
            Executor.Dispose();
            Executor = null;
        }

        #region Publish Subscribe

        #region Given

        [Given(@"que um assinante se inscreva para receber a mensagem EventOccurred")]
        public void DadoQueUmAssinanteSeInscrevaParaReceberAMensagemEventOccurred()
        {
            Executor.PerformSubscription<EventOccurred>(
                Executor.MessagesReceivedBySubscriber1,
                Executor.MessageHandlerForSubscriber1,
                "DadoQueUmAssinanteSeInscrevaParaReceberAMensagemEventOccurred",
                null
            );
        }

        [Given(@"que '(.*)' mensagens do tipo EventOcurred sejam publicadas")]
        public void DadoQueMensagensDoTipoEventOcurredSejamPublicadas(int numeroDeMensagens)
        {
            Executor.PerformPublication<EventOccurred>(messageCount: numeroDeMensagens);
        }

        [Given(@"que um assinante se inscreva para receber a mensagem OtherEventOccurred")]
        public void DadoQueUmAssinanteSeInscrevaParaReceberAMensagemOtherEventOccurred()
        {
            Executor.PerformSubscription<OtherEventOccurred>(
                Executor.MessagesReceivedBySubscriber1,
                Executor.MessageHandlerForSubscriber1,
                "DadoQueUmAssinanteSeInscrevaParaReceberAMensagemOtherEventOccurred",
                null
            );
        }

        [Given(@"que nenhum assinante se inscreva para receber a mensagem EventOccurred")]
        public void DadoQueNenhumAssinanteSeInscrevaParaReceberAMensagemEventOccurred()
        {
            // Não faz nada mesmo
        }

        [Given(@"que outro assinante se inscreva para receber a mensagem EventOccurred")]
        public void DadoQueOutroAssinanteSeInscrevaParaReceberAMensagemEventOccurred()
        {
            Executor.PerformSubscription<EventOccurred>(
                Executor.MessagesReceivedBySubscriber2,
                Executor.MessageHandlerForSubscriber2,
                "DadoQueOutroAssinanteSeInscrevaParaReceberAMensagemEventOccurred",
                null
            );
        }
        [Given(@"que outro assinante se inscreva para receber a mensagem OtherEventOccurred")]
        public void DadoQueOutroAssinanteSeInscrevaParaReceberAMensagemOtherEventOccurred()
        {
            Executor.PerformSubscription<OtherEventOccurred>(
                Executor.MessagesReceivedBySubscriber2,
                Executor.MessageHandlerForSubscriber2,
                "DadoQueOutroAssinanteSeInscrevaParaReceberAMensagemOtherEventOccurred",
                null
            );
        }

        [Given(@"que a mensagem EventOccurred tenha uma validade de '(.*)' segundos")]
        public void DadoQueAMensagemEventOccurredTenhaUmaValidadeDeSegundos(int elapsedTimeInSeconds)
        {
            Executor.SetMessageValidity<EventOccurred>(elapsedTimeInSeconds);
        }

        [Given(@"que a mensagem OtherEventOccurred tenha uma validade de '(.*)' segundos")]
        public void DadoQueAMensagemOtherEventOccurredTenhaUmaValidadeDeSegundos(int elapsedTimeInSeconds)
        {
            Executor.SetMessageValidity<OtherEventOccurred>(elapsedTimeInSeconds);
        }

        [Given(@"a mensagem EventOccurred é preenchida com informações relevantes")]
        public void QuandoAMensagemEventOccurredEPreenchidaComInformacoesRelevantes()
        {
            Executor.RelevantField1Value = "RelevantField1Value";
            Executor.RelevantField2Value = "RelevantField2Value";
        }

        [Given(@"que um assinante se inscreva para receber a mensagem EventOccurred no tópico Confirmed")]
        public void DadoQueUmAssinanteSeInscrevaParaReceberAMensagemEventOccurredNoTopicoConfirmed()
        {
            Executor.PerformSubscription<EventOccurred>(
                Executor.MessagesReceivedBySubscriber1,
                Executor.MessageHandlerForSubscriber1,
                "DadoQueUmAssinanteSeInscrevaParaReceberAMensagemEventOccurredNoTopicoConfirmed",
                "Confirmed"
            );
        }

        [Given(@"que jamais tenha havido uma inscrição para a mensagem EventOccurred")]
        public void DadoQueJamaisTenhaHavidoUmaInscricaoParaAMensagemEventOccurred()
        {
            Executor.DeleteSubscriptionQueue<EventOccurred>("DadoQueUmAssinanteSeInscrevaParaReceberAMensagemEventOccurred");
        }

        [Given(@"que um assinante solicite a preparação de uma inscrição para receber a mensagem EventOccurred")]
        public void DadoQueUmAssinanteSoliciteAPreparacaoDeUmaInscricaoParaReceberAMensagemEventOccurred()
        {
            Executor.PrepareSubscription<EventOccurred>("DadoQueUmAssinanteSeInscrevaParaReceberAMensagemEventOccurred");
        }

        [Given(@"que a mensagem EventOccurred seja publicada no barramento")]
        public void DadoQueAMensagemEventOccurredSejaPublicadaNoBarramento()
        {
            Executor.PerformPublication<EventOccurred>();
        }

        [Given(@"que haja uma fila de inscrição abandonada para a mensagem EventOccurred e id de inscrição ABC")]
        public void DadoQueHajaUmaFilaDeInscricaoAbandonadaParaAMensagemEventOccurredEIdDeInscricaoAbc()
        {
            Executor.PrepareSubscription<EventOccurred>("ABC");
        }

        [Given(@"que seja preparada uma inscrição para a mensagem EventOccurred")]
        public void DadoQueSejaPreparadaUmaInscricaoParaAMensagemEventOccurred()
        {
            Executor.PrepareSubscription<EventOccurred>("DadoQueUmAssinanteSeInscrevaParaReceberAMensagemEventOccurred");
        }

        [Given(@"o servidor de mensageria for reiniciado")]
        public void DadoOServidorDeMensageriaForReiniciado()
        {
            Executor.RestartMessageBroker();
        }

        #endregion
        #region When

        [When(@"um assinante se inscrever para receber a mensagem EventOccurred")]
        public void QuandoUmAssinanteSeInscreverParaReceberAMensagemEventOccurred()
        {
            Executor.PerformSubscription<EventOccurred>(
                Executor.MessagesReceivedBySubscriber1,
                Executor.MessageHandlerForSubscriber1,
                "DadoQueUmAssinanteSeInscrevaParaReceberAMensagemEventOccurred",
                null
            );
        }

        [When(@"a mensagem EventOccurred for publicada no barramento")]
        public void QuandoAMensagemEventOccurredForPublicadaNoBarramento()
        {
            Executor.PerformPublication<EventOccurred>(configureMessage: m =>
            {
                m.RelevantField1 = Executor.RelevantField1Value;
                m.RelevantField2 = Executor.RelevantField2Value;
            });
        }

        [When(@"a mensagem OtherEventOccurred for publicada no barramento")]
        public void QuandoAMensagemOtherEventOccurredForPublicadaNoBarramento()
        {
            Executor.PerformPublication<OtherEventOccurred>();
        }

        [When(@"mais de '(.*)' segundos se passarem")]
        public void QuandoMaisDeSegundosSePassarem(int elapsedTimeInSeconds)
        {
            Executor.Wait(elapsedTimeInSeconds + 1);
        }

        [When(@"menos de '(.*)' segundos se passarem")]
        public void QuandoMenosDeSegundosSePassarem(int elapsedTimeInSeconds)
        {
            // DOes nothing
        }

        [When(@"a mensagem EventOccurred for publicada no tópico Confirmed")]
        public void QuandoAMensagemEventOccurredForPublicadaNoTopicoConfirmed()
        {
            Executor.PerformPublication<EventOccurred>("Confirmed");
        }

        [When(@"a mensagem EventOccurred for publicada no tópico Pending")]
        public void QuandoAMensagemEventOccurredForPublicadaNoTopicoPending()
        {
            Executor.PerformPublication<EventOccurred>("Pending");
        }

        [When(@"'(.*)' mensagens EventOccurred forem publicadas no barramento")]
        public void QuandoMensagensEventOccurredForemPublicadasNoBarramento(int messageCount)
        {
            Executor.PerformPublication<EventOccurred>(messageCount: messageCount);
        }
        [When(@"'(.*)' mensagens OtherEventOccurred forem publicadas no barramento")]
        public void QuandoMensagensOtherEventOccurredForemPublicadasNoBarramento(int messageCount)
        {
            Executor.PerformPublication<OtherEventOccurred>(messageCount: messageCount);
        }

        [When(@"o assinante se inscrever para receber a mensagem EventOccurred")]
        public void QuandoOAssinanteSeInscreverParaReceberAMensagemEventOccurred()
        {
            Executor.PerformSubscription<EventOccurred>(
                Executor.MessagesReceivedBySubscriber1,
                Executor.MessageHandlerForSubscriber1,
                "DadoQueUmAssinanteSeInscrevaParaReceberAMensagemEventOccurred",
                null
            );
            Executor.Wait(1);
        }

        [When(@"for solicitado que a exclusão da fila de inscrição para a mensagem EventOccurred e id de inscrição ABC")]
        public void QuandoForSolicitadoQueAExclusaoDaFilaDeInscricaoParaAMensagemEventOccurredEIdDeInscricaoAbc()
        {
            Executor.DeleteSubscriptionQueue<EventOccurred>("ABC");
        }

        [When(@"a mensagem EventOccurred for publicada como persistente no barramento")]
        public void QuandoAMensagemEventOccurredForPublicadaComoPersistenteNoBarramento()
        {
            Executor.PerformPublication<EventOccurred>(storageType: StorageType.Persistent);
        }

        [When(@"a mensagem EventOccurred for publicada como transiente no barramento")]
        public void QuandoAMensagemEventOccurredForPublicadaComoTransienteNoBarramento()
        {
            Executor.PerformPublication<EventOccurred>(storageType: StorageType.NonPersistent);
        }

        [When(@"o servidor de mensageria for reiniciado")]
        public void QuandoOServidorDeMensageriaForReiniciado()
        {
            Executor.RestartMessageBroker();
        }

        #endregion
        #region Then

        [Then(@"a mensagem EventOccurred deve ser entregue ao assinante")]
        public void EntaoAMensagemEventOccurredDeveSerEntregueAoAssinante()
        {
            Executor.WaitMessageToBeProcessed();
            Executor.MessagesReceivedBySubscriber1.Should().NotBeEmpty();
            Executor.MessagesReceivedBySubscriber1.Should().ContainSingle(m => m is EventOccurred);
        }
        [Then(@"a mensagem OtherEventOccurred deve ser entregue ao assinante")]
        public void EntaoAMensagemOtherEventOccurredDeveSerEntregueAoAssinante()
        {
            Executor.WaitMessageToBeProcessed();
            Executor.MessagesReceivedBySubscriber1.Should().NotBeEmpty();
            Executor.MessagesReceivedBySubscriber1.Should().ContainSingle(m => m is OtherEventOccurred);
        }

        [Then(@"a mensagem EventOccurred deve ser entregue ao outro assinante")]
        public void EntaoAMensagemEventOccurredDeveSerEntregueAoOutroAssinante()
        {
            Executor.WaitMessageToBeProcessed();
            Executor.MessagesReceivedBySubscriber2.Should().NotBeEmpty();
            Executor.MessagesReceivedBySubscriber2.Should().ContainSingle(m => m is EventOccurred);
        }
        [Then(@"a mensagem OtherEventOccurred deve ser entregue ao outro assinante")]
        public void EntaoAMensagemOtherEventOccurredDeveSerEntregueAoOutroAssinante()
        {
            Executor.WaitMessageToBeProcessed();
            Executor.MessagesReceivedBySubscriber2.Should().NotBeEmpty();
            Executor.MessagesReceivedBySubscriber2.Should().ContainSingle(m => m is OtherEventOccurred);
        }

        [Then(@"a mensagem EventOccurred não deve ser entregue ao assinante")]
        public void EntaoAMensagemEventOccurredNaoDeveSerEntregueAoAssinante()
        {
            Executor.WaitMessageToBeProcessed();
            Executor.MessagesReceivedBySubscriber1.Should().NotContain(m => m is EventOccurred);
        }
        [Then(@"a mensagem OtherEventOccurred não deve ser entregue ao assinante")]
        public void EntaoAMensagemOtherEventOccurredNaoDeveSerEntregueAoAssinante()
        {
            Executor.WaitMessageToBeProcessed();
            Executor.MessagesReceivedBySubscriber1.Should().NotContain(m => m is OtherEventOccurred);
        }

        [Then(@"a mensagem EventOccurred não deve ser entregue ao outro assinante")]
        public void EntaoAMensagemEventOccurredNaoDeveSerEntregueAoOutroAssinante()
        {
            Executor.WaitMessageToBeProcessed();
            Executor.MessagesReceivedBySubscriber2.Should().NotContain(m => m is EventOccurred);
        }
        [Then(@"a mensagem OtherEventOccurred não deve ser entregue ao outro assinante")]
        public void EntaoAMensagemOtherEventOccurredNaoDeveSerEntregueAoOutroAssinante()
        {
            Executor.WaitMessageToBeProcessed();
            Executor.MessagesReceivedBySubscriber2.Should().NotContain(m => m is OtherEventOccurred);
        }

        [Then(@"todas as informações relevantes devem estar presentes na mensagem EventOccurred")]
        public void EntaoTodasAsInformacoesRelevantesDevemEstarPresentesNaMensagemEventOccurred()
        {
            Executor.WaitMessageToBeProcessed();
            var receivedMessage = Executor.MessagesReceivedBySubscriber1.SingleOrDefault(m => m is EventOccurred);
            Debug.Assert(receivedMessage != null, "receivedMessage != null");
            ((EventOccurred)receivedMessage).RelevantField1.Should().Be("RelevantField1Value");
            (receivedMessage as EventOccurred).RelevantField2.Should().Be("RelevantField2Value");
        }

        [Then(@"'(.*)' mensagens EventOccurred devem ser entregues ao assinante")]
        public void EntaoMensagensEventOccurredDevemSerEntreguesAoAssinante(int messageCount)
        {
            Executor.WaitMessageToBeProcessed();
            Executor.MessagesReceivedBySubscriber1.Where(m => m is EventOccurred).Should().HaveCount(messageCount);
        }
        [Then(@"'(.*)' mensagens OtherEventOccurred devem ser entregues ao assinante")]
        public void EntaoMensagensOtherEventOccurredDevemSerEntreguesAoAssinante(int messageCount)
        {
            Executor.WaitMessageToBeProcessed();
            Executor.MessagesReceivedBySubscriber1.Where(m => m is OtherEventOccurred).Should().HaveCount(messageCount);
        }
        [Then(@"'(.*)' mensagens EventOccurred devem ser entregues ao outro assinante")]
        public void EntaoMensagensEventOccurredDevemSerEntreguesAoOutroAssinante(int messageCount)
        {
            Executor.WaitMessageToBeProcessed();
            Executor.MessagesReceivedBySubscriber2.Where(m => m is EventOccurred).Should().HaveCount(messageCount);
        }
        [Then(@"'(.*)' mensagens OtherEventOccurred devem ser entregues ao outro assinante")]
        public void EntaoMensagensOtherEventOccurredDevemSerEntreguesAoOutroAssinante(int messageCount)
        {
            Executor.WaitMessageToBeProcessed();
            Executor.MessagesReceivedBySubscriber2.Where(m => m is OtherEventOccurred).Should().HaveCount(messageCount);
        }

        [Then(@"a fila de inscrição para a mensagem EventOccurred e id de inscrição ABC deve ser apagada")]
        public void EntaoAFilaDeInscricaoParaAMensagemEventOccurredEIdDeInscricaoAbcDeveSerApagada()
        {
            Executor.HasSubscription<EventOccurred>("ABC").Should().BeFalse();
        }

        #endregion

        #endregion

        #region Request Response

        [Given(@"que exista um solicitado '(.*)' aguardando por solicitações do tipo '(.*)'")]
        public void DadoQueExistaUmSolicitadoAguardandoPorSolicitacoesDoTipo(string responder, string requestType)
        {
            Executor.PrepareRequestResponder(responder, requestType);
        }

        [Given(@"que exista um solicitado '(.*)' aguardando por solicitações do tipo '(.*)' com uma inscrição SubscriptionX")]
        public void DadoQueExistaUmSolicitadoAguardandoPorSolicitacoesDoTipoComUmaInscricaoSubscriptionX(string responder, string requestType)
        {
            Executor.PrepareRequestResponder(responder, requestType, "SubscriptionX");
        }

        [Given(@"que exista um solicitante '(.*)' pronto para enviar solicitações do tipo '(.*)'")]
        public void DadoQueExistaUmSolicitanteProntoParaEnviarSolicitacoesDoTipo(string requester, string requestType)
        {
            Executor.PrepareRequestRequester(requester, requestType);
        }

        [Given(@"que exista um solicitante '(.*)' pronto para enviar solicitações do tipo '(.*)' com uma inscrição SubscriptionX")]
        public void DadoQueExistaUmSolicitanteProntoParaEnviarSolicitacoesDoTipoComUmaInscricaoSubscriptionX(string requester, string requestType)
        {
            Executor.PrepareRequestRequester(requester, requestType, Executor.SpecificRequestXSubscription);
        }

        [Given(@"que não exista um solicitado '(.*)' aguardando por solicitações do tipo '(.*)'")]
        public void DadoQueNaoExistaUmSolicitadoAguardandoPorSolicitacoesDoTipo(string responder, string requestType)
        {
            // Does nothing
        }

        [When(@"o solicitante '(.*)' enviar uma solicitação '(.*)' do tipo '(.*)' em modo (.*)")]
        public void QuandoOSolicitanteEnviarUmaSolicitacaoDoTipoEmModoSincrono(string requester, string requestId, string requestType, string requestMode)
        {
            Executor.SendRequest(requester, requestId, requestType, requestMode == "assincrono");
        }

        [When(@"o solicitante '(.*)' enviar (.*) solicitações do tipo '(.*)' em modo (.*)")]
        public void QuandoOSolicitanteEnviarSolicitacoesDoTipoEmModo(string requester, int requestCount, string requestType, string requestMode)
        {
            Executor.SendRequests(requester, requestCount, requestType, requestMode == "assincrono");
        }

        [Then(@"o solicitante '(.*)' deve receber uma resposta à solicitação '(.*)' enviada pelo solicitado '(.*)'")]
        public void EntaoOSolicitanteDeveReceberUmaRespostaASolicitacaoEnviadaPeloSolicitado(string requester, string requestId, string responder)
        {
            Executor.AssertThatRequestResponseWasReceived(requester, requestId, responder);
        }

        [Then(@"o solicitante '(.*)' não deve receber uma resposta à solicitação '(.*)' enviada pelo solicitado '(.*)'")]
        public void EntaoOSolicitanteNaoDeveReceberUmaRespostaASolicitacaoEnviadaPeloSolicitado(string requester, string requestId, string responder)
        {
            Executor.AssertThatRequestResponseWasNotReceived(requester, requestId, responder);
        }

        [Then(@"o solicitante '(.*)' deve receber (.*) respostas às solicitações do tipo '(.*)' enviadas por qualquer solicitado")]
        public void EntaoOSolicitanteDeveReceberRespostasAsSolicitacoesDoTipo(string requester, int requestCount, string requestType)
        {
            Executor.AssertThatRequestResponsesWereReceived(requester, requestCount, requestType);
        }

        [Then(@"o solicitante '(.*)' deve receber uma resposta à solicitação '(.*)' ou uma resposta à solicitação '(.*)'")]
        public void EntaoOSolicitanteDeveReceberUmaRespostaASolicitacaoOuUmaRespostaASolicitacao(string requester, string requestId1, string requestId2)
        {
            Executor.AssertThatOneOfTwoPossibleResponsesWasReceived(requester, requestId1, requestId2);
        }

        [Then(@"deve haver ao menos uma resposta à solicitação de tipo '(.*)' que tenha sido enviada pelo solicitado '(.*)'")]
        public void EntaoDeveHaverAoMenosUmaRespostaASolicitacaoDeTipoQueTenhaSidoEnviadaPeloSolicitado(string requestType, string responder)
        {
            Executor.AssertThatAtLeastOneRequestResponseWasReceived(requestType, responder);
        }

        #endregion

        #region Authorized Messages

        [Given(@"que o arquivo de configuração de autorização de mensagens não exista")]
        public void DadoQueOArquivoDeConfiguracaoDeAutorizacaoDeMensagensNaoExista()
        {
            Executor.EnsureAuthorizationFileDoesNotExist();
        }

        [Given(@"que o arquivo de configuração de autorização de mensagens exista")]
        public void DadoQueOArquivoDeConfiguracaoDeAutorizacaoDeMensagensExista()
        {
            Executor.EnsureAuthorizationFileExists();
        }

        [Given(@"que o arquivo de configuração de autorização de mensagens informe que mensagens não descriminadas devem ser autorizadas")]
        public void DadoQueOArquivoDeConfiguracaoDeAutorizacaoDeMensagensInformeQueMensagensNaoDescriminadasDevemSerAutorizadas()
        {
            Executor.ChangeAuthorizationFileToAllowAllByDefault();
        }

        [Given(@"que o arquivo de configuração de autorização de mensagens informe que mensagens não descriminadas não devem ser autorizadas")]
        public void DadoQueOArquivoDeConfiguracaoDeAutorizacaoDeMensagensInformeQueMensagensNaoDescriminadasNaoDevemSerAutorizadas()
        {
            Executor.ChangeAuthorizationFileToDenyAllByDefault();
        }

        [Given(@"uma mensagem a ser enviada ou recebida for uma mensagem não discriminada no arquivo de configuração de autorização de mensagens")]
        public void DadoUmaMensagemASerEnviadaOuRecebidaForUmaMensagemNaoDiscriminadaNoArquivoDeConfiguracaoDeAutorizacaoDeMensagens()
        {
            Executor.PrepareAMessageNotDiscriminatedInTheAuthorizationFile();
        }

        [Given(@"uma mensagem a ser enviada ou recebida for uma mensagem discriminada no arquivo de configuração de autorização de mensagens")]
        public void DadoUmaMensagemASerEnviadaOuRecebidaForUmaMensagemDiscriminadaNoArquivoDeConfiguracaoDeAutorizacaoDeMensagens()
        {
            Executor.PrepareAMessageDiscriminatedInTheAuthorizationFile();
        }

        [Given(@"o serviço de autorização de mensagens estiver configurado para autorizar o envio ou recebimento da mensagem")]
        public void DadoOServicoDeAutorizacaoDeMensagensEstiverConfiguradoParaAutorizarOEnvioOuRecebimentoDaMensagem()
        {
            Executor.PrepareAuthorizationServiceThatAllowAllMessages();
        }

        [Given(@"o serviço de autorização de mensagens estiver configurado para não autorizar o envio ou recebimento da mensagem")]
        public void DadoOServicoDeAutorizacaoDeMensagensEstiverConfiguradoParaNaoAutorizarOEnvioOuRecebimentoDaMensagem()
        {
            Executor.PrepareAuthorizationServiceThatDenyAllMessages();
        }


        [When(@"a operação de envio ou recebimento de mensagens for executada")]
        public void QuandoAOperacaoDeEnvioOuRecebimentoDeMensagensForExecutada()
        {
            Executor.TryToSendAndReceiveAuthorizedMessages();
        }


        [Then(@"a operação deve ser abortada com erro devido à ausência do arquivo de configuração de autorização de mensagens")]
        public void EntaoAOperacaoDeveSerAbortadaComErroDevidoAAusenciaDoArquivoDeConfiguracaoDeAutorizacaoDeMensagens()
        {
            Executor.AssertThatSendAndReceivingOfAuthorizedMessagesHasFailedDueToMissingAuthorizationFile();
        }

        [Then(@"o arquivo de configuração de autorização de mensagens deve ser carregado para memória e considerado antes de se executar o envio ou recebimento")]
        public void EntaoOArquivoDeConfiguracaoDeAutorizacaoDeMensagensDeveSerCarregadoParaMemoriaEConsideradoAntesDeSeExecutarOEnvioOuRecebimento()
        {
            Executor.AssertThatTheAuthorizationFileWasSuccessfullyLoaded();
        }

        [Then(@"o envio ou recebimento deve ser realizado com sucesso")]
        public void EntaoOEnvioOuRecebimentoDeveSerRealizadoComSucesso()
        {
            Executor.AssertThatSendAndReceivingOfAuthorizedMessagesHasSucceeded();
        }

        [Then(@"o envio ou recebimento não deve ser realizado")]
        public void EntaoOEnvioOuRecebimentoNaoDeveSerRealizado()
        {
            Executor.AssertThatSendAndReceivingOfAuthorizedMessagesHasFailedDueToAuthorizationDenial();
        }

        #endregion

        public void Dispose()
        {
            if (Executor != null)
                Executor.Dispose();
        }
    }
}