﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace ReactiveServices.MessageBus.Tests.Specifications
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Padrão Publish Subscriber")]
    public partial class PadraoPublishSubscriberFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "PublishSubscriber.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("pt-BR"), "Padrão Publish Subscriber", "Como um componente de software\nDesejo publicar uma mensagem no barramento de mens" +
                    "agens\nPara que outros componentes de software possam receber minha mensagem", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar uma mensagem para um assinante")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void PublicarUmaMensagemParaUmAssinante()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar uma mensagem para um assinante", new string[] {
                        "stable",
                        "fast"});
#line 9
this.ScenarioSetup(scenarioInfo);
#line 10
 testRunner.Given("que um assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 11
 testRunner.When("a mensagem EventOccurred for publicada no barramento", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 12
 testRunner.Then("a mensagem EventOccurred deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Assinar o recebimento de uma mensagem já havendo duas mensagens na fila")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void AssinarORecebimentoDeUmaMensagemJaHavendoDuasMensagensNaFila()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Assinar o recebimento de uma mensagem já havendo duas mensagens na fila", new string[] {
                        "stable",
                        "fast"});
#line 15
this.ScenarioSetup(scenarioInfo);
#line 16
 testRunner.Given("que um assinante solicite a preparação de uma inscrição para receber a mensagem E" +
                    "ventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 17
 testRunner.And("que \'2\' mensagens do tipo EventOcurred sejam publicadas", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 18
 testRunner.When("um assinante se inscrever para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 19
 testRunner.And("mais de \'2\' segundos se passarem", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 20
 testRunner.Then("\'2\' mensagens EventOccurred devem ser entregues ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar uma mensagem para dois assinantes")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void PublicarUmaMensagemParaDoisAssinantes()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar uma mensagem para dois assinantes", new string[] {
                        "stable",
                        "fast"});
#line 23
this.ScenarioSetup(scenarioInfo);
#line 24
 testRunner.Given("que um assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 25
 testRunner.And("que outro assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 26
 testRunner.When("a mensagem EventOccurred for publicada no barramento", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 27
 testRunner.Then("a mensagem EventOccurred deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line 28
 testRunner.And("a mensagem EventOccurred deve ser entregue ao outro assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar duas mensagens para dois assinantes")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void PublicarDuasMensagensParaDoisAssinantes()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar duas mensagens para dois assinantes", new string[] {
                        "stable",
                        "fast"});
#line 31
this.ScenarioSetup(scenarioInfo);
#line 32
 testRunner.Given("que um assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 33
 testRunner.And("que um assinante se inscreva para receber a mensagem OtherEventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 34
 testRunner.And("que outro assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 35
 testRunner.And("que outro assinante se inscreva para receber a mensagem OtherEventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 36
 testRunner.When("a mensagem EventOccurred for publicada no barramento", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 37
 testRunner.And("a mensagem OtherEventOccurred for publicada no barramento", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 38
 testRunner.Then("a mensagem EventOccurred deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line 39
 testRunner.And("a mensagem OtherEventOccurred deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 40
 testRunner.And("a mensagem EventOccurred deve ser entregue ao outro assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 41
 testRunner.And("a mensagem OtherEventOccurred deve ser entregue ao outro assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar uma mensagem sem nenhum assinante")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void PublicarUmaMensagemSemNenhumAssinante()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar uma mensagem sem nenhum assinante", new string[] {
                        "stable",
                        "fast"});
#line 44
this.ScenarioSetup(scenarioInfo);
#line 45
 testRunner.Given("que nenhum assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 46
 testRunner.When("a mensagem EventOccurred for publicada no barramento", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 47
 testRunner.Then("a mensagem EventOccurred não deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar uma mensagem de um tipo havendo um assinante apenas para mensagens de um" +
            " outro tipo")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void PublicarUmaMensagemDeUmTipoHavendoUmAssinanteApenasParaMensagensDeUmOutroTipo()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar uma mensagem de um tipo havendo um assinante apenas para mensagens de um" +
                    " outro tipo", new string[] {
                        "stable",
                        "fast"});
#line 50
this.ScenarioSetup(scenarioInfo);
#line 51
 testRunner.Given("que um assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 52
 testRunner.When("a mensagem OtherEventOccurred for publicada no barramento", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 53
 testRunner.Then("a mensagem OtherEventOccurred não deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line 54
 testRunner.And("a mensagem EventOccurred não deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar duas mensagens para dois assinantes, um assinante para cada mensagem")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void PublicarDuasMensagensParaDoisAssinantesUmAssinanteParaCadaMensagem()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar duas mensagens para dois assinantes, um assinante para cada mensagem", new string[] {
                        "stable",
                        "fast"});
#line 57
this.ScenarioSetup(scenarioInfo);
#line 58
 testRunner.Given("que um assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 59
 testRunner.And("que outro assinante se inscreva para receber a mensagem OtherEventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 60
 testRunner.When("a mensagem EventOccurred for publicada no barramento", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 61
 testRunner.And("a mensagem OtherEventOccurred for publicada no barramento", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 62
 testRunner.Then("a mensagem EventOccurred deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line 63
 testRunner.And("a mensagem OtherEventOccurred deve ser entregue ao outro assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 64
 testRunner.And("a mensagem OtherEventOccurred não deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 65
 testRunner.And("a mensagem EventOccurred não deve ser entregue ao outro assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar uma mensagem e constatar que sua validade não expirou")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void PublicarUmaMensagemEConstatarQueSuaValidadeNaoExpirou()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar uma mensagem e constatar que sua validade não expirou", new string[] {
                        "stable",
                        "fast"});
#line 68
this.ScenarioSetup(scenarioInfo);
#line 69
 testRunner.Given("que jamais tenha havido uma inscrição para a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 70
 testRunner.And("que um assinante solicite a preparação de uma inscrição para receber a mensagem E" +
                    "ventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 71
 testRunner.And("que a mensagem EventOccurred tenha uma validade de \'2\' segundos", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 72
 testRunner.And("que a mensagem EventOccurred seja publicada no barramento", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 73
 testRunner.When("menos de \'2\' segundos se passarem", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 74
 testRunner.And("o assinante se inscrever para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 75
 testRunner.Then("a mensagem EventOccurred deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar uma mensagem e constatar que sua validade expirou")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void PublicarUmaMensagemEConstatarQueSuaValidadeExpirou()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar uma mensagem e constatar que sua validade expirou", new string[] {
                        "stable",
                        "fast"});
#line 78
this.ScenarioSetup(scenarioInfo);
#line 79
 testRunner.Given("que jamais tenha havido uma inscrição para a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 80
 testRunner.And("que um assinante solicite a preparação de uma inscrição para receber a mensagem E" +
                    "ventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 81
 testRunner.And("que a mensagem EventOccurred tenha uma validade de \'2\' segundos", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 82
 testRunner.And("que a mensagem EventOccurred seja publicada no barramento", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 83
 testRunner.When("mais de \'2\' segundos se passarem", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 84
 testRunner.And("o assinante se inscrever para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 85
 testRunner.Then("a mensagem EventOccurred não deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar uma mensagem com multiplos campos")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void PublicarUmaMensagemComMultiplosCampos()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar uma mensagem com multiplos campos", new string[] {
                        "stable",
                        "fast"});
#line 88
this.ScenarioSetup(scenarioInfo);
#line 89
 testRunner.Given("que um assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 90
 testRunner.And("a mensagem EventOccurred é preenchida com informações relevantes", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 91
 testRunner.When("a mensagem EventOccurred for publicada no barramento", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 92
 testRunner.Then("a mensagem EventOccurred deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line 93
 testRunner.And("todas as informações relevantes devem estar presentes na mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar uma mensagem em um tópico assinado")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void PublicarUmaMensagemEmUmTopicoAssinado()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar uma mensagem em um tópico assinado", new string[] {
                        "stable",
                        "fast"});
#line 96
this.ScenarioSetup(scenarioInfo);
#line 97
 testRunner.Given("que um assinante se inscreva para receber a mensagem EventOccurred no tópico Conf" +
                    "irmed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 98
 testRunner.When("a mensagem EventOccurred for publicada no tópico Confirmed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 99
 testRunner.Then("a mensagem EventOccurred deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar uma mensagem em um tópico não assinado")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void PublicarUmaMensagemEmUmTopicoNaoAssinado()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar uma mensagem em um tópico não assinado", new string[] {
                        "stable",
                        "fast"});
#line 102
this.ScenarioSetup(scenarioInfo);
#line 103
 testRunner.Given("que um assinante se inscreva para receber a mensagem EventOccurred no tópico Conf" +
                    "irmed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 104
 testRunner.When("a mensagem EventOccurred for publicada no tópico Pending", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 105
 testRunner.Then("a mensagem EventOccurred não deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Limpar as filas de inscrição para um dado tipo de mensagem e id de inscrição")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void LimparAsFilasDeInscricaoParaUmDadoTipoDeMensagemEIdDeInscricao()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Limpar as filas de inscrição para um dado tipo de mensagem e id de inscrição", new string[] {
                        "stable",
                        "fast"});
#line 108
this.ScenarioSetup(scenarioInfo);
#line 109
 testRunner.Given("que haja uma fila de inscrição abandonada para a mensagem EventOccurred e id de i" +
                    "nscrição ABC", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 110
 testRunner.When("for solicitado que a exclusão da fila de inscrição para a mensagem EventOccurred " +
                    "e id de inscrição ABC", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 111
 testRunner.Then("a fila de inscrição para a mensagem EventOccurred e id de inscrição ABC deve ser " +
                    "apagada", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Preparar uma inscrição futura")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("fast")]
        public virtual void PrepararUmaInscricaoFutura()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Preparar uma inscrição futura", new string[] {
                        "stable",
                        "fast"});
#line 114
this.ScenarioSetup(scenarioInfo);
#line 115
 testRunner.Given("que jamais tenha havido uma inscrição para a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 116
 testRunner.And("que um assinante solicite a preparação de uma inscrição para receber a mensagem E" +
                    "ventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 117
 testRunner.And("que a mensagem EventOccurred seja publicada no barramento", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 118
 testRunner.When("o assinante se inscrever para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 119
 testRunner.Then("a mensagem EventOccurred deve ser entregue ao assinante", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar um grande volume de uma única mensagem para um único assinante, sem time" +
            "out")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("slow")]
        [NUnit.Framework.TestCaseAttribute("1", null)]
        [NUnit.Framework.TestCaseAttribute("10", null)]
        [NUnit.Framework.TestCaseAttribute("50", null)]
        [NUnit.Framework.TestCaseAttribute("100", null)]
        public virtual void PublicarUmGrandeVolumeDeUmaUnicaMensagemParaUmUnicoAssinanteSemTimeout(string messageCount, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "stable",
                    "slow"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar um grande volume de uma única mensagem para um único assinante, sem time" +
                    "out", @__tags);
#line 122
this.ScenarioSetup(scenarioInfo);
#line 123
 testRunner.Given("que um assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 124
 testRunner.When(string.Format("\'{0}\' mensagens EventOccurred forem publicadas no barramento", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 125
 testRunner.Then(string.Format("\'{0}\' mensagens EventOccurred devem ser entregues ao assinante", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar um grande volume de diferentes mensagens para dois assinantes distintos," +
            " sem timeout")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("slow")]
        [NUnit.Framework.TestCaseAttribute("1", null)]
        [NUnit.Framework.TestCaseAttribute("10", null)]
        [NUnit.Framework.TestCaseAttribute("50", null)]
        [NUnit.Framework.TestCaseAttribute("100", null)]
        public virtual void PublicarUmGrandeVolumeDeDiferentesMensagensParaDoisAssinantesDistintosSemTimeout(string messageCount, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "stable",
                    "slow"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar um grande volume de diferentes mensagens para dois assinantes distintos," +
                    " sem timeout", @__tags);
#line 135
this.ScenarioSetup(scenarioInfo);
#line 136
 testRunner.Given("que um assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 137
 testRunner.And("que um assinante se inscreva para receber a mensagem OtherEventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 138
 testRunner.And("que outro assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 139
 testRunner.And("que outro assinante se inscreva para receber a mensagem OtherEventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 140
 testRunner.When(string.Format("\'{0}\' mensagens EventOccurred forem publicadas no barramento", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 141
 testRunner.And(string.Format("\'{0}\' mensagens OtherEventOccurred forem publicadas no barramento", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 142
 testRunner.Then(string.Format("\'{0}\' mensagens EventOccurred devem ser entregues ao assinante", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line 143
 testRunner.And(string.Format("\'{0}\' mensagens OtherEventOccurred devem ser entregues ao assinante", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 144
 testRunner.And(string.Format("\'{0}\' mensagens EventOccurred devem ser entregues ao outro assinante", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 145
 testRunner.And(string.Format("\'{0}\' mensagens OtherEventOccurred devem ser entregues ao outro assinante", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar um grande volume de uma única mensagem para um único assinante, com time" +
            "out")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("slow")]
        [NUnit.Framework.TestCaseAttribute("1", "1", null)]
        [NUnit.Framework.TestCaseAttribute("3", "1", null)]
        [NUnit.Framework.TestCaseAttribute("10", "2", null)]
        [NUnit.Framework.TestCaseAttribute("40", "8", null)]
        [NUnit.Framework.TestCaseAttribute("60", "16", null)]
        [NUnit.Framework.TestCaseAttribute("80", "16", null)]
        public virtual void PublicarUmGrandeVolumeDeUmaUnicaMensagemParaUmUnicoAssinanteComTimeout(string messageCount, string timeout, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "stable",
                    "slow"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar um grande volume de uma única mensagem para um único assinante, com time" +
                    "out", @__tags);
#line 155
this.ScenarioSetup(scenarioInfo);
#line 156
 testRunner.Given("que um assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 157
 testRunner.And(string.Format("que a mensagem EventOccurred tenha uma validade de \'{0}\' segundos", timeout), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 158
 testRunner.When(string.Format("\'{0}\' mensagens EventOccurred forem publicadas no barramento", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 159
 testRunner.Then(string.Format("\'{0}\' mensagens EventOccurred devem ser entregues ao assinante", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Publicar um grande volume de diferentes mensagens para dois assinantes distintos," +
            " com timeout")]
        [NUnit.Framework.CategoryAttribute("stable")]
        [NUnit.Framework.CategoryAttribute("slow")]
        [NUnit.Framework.TestCaseAttribute("1", "1", null)]
        [NUnit.Framework.TestCaseAttribute("5", "3", null)]
        [NUnit.Framework.TestCaseAttribute("10", "6", null)]
        [NUnit.Framework.TestCaseAttribute("50", "18", null)]
        [NUnit.Framework.TestCaseAttribute("100", "40", null)]
        public virtual void PublicarUmGrandeVolumeDeDiferentesMensagensParaDoisAssinantesDistintosComTimeout(string messageCount, string timeout, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "stable",
                    "slow"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publicar um grande volume de diferentes mensagens para dois assinantes distintos," +
                    " com timeout", @__tags);
#line 171
this.ScenarioSetup(scenarioInfo);
#line 172
 testRunner.Given("que um assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line 173
 testRunner.And("que um assinante se inscreva para receber a mensagem OtherEventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 174
 testRunner.And("que outro assinante se inscreva para receber a mensagem EventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 175
 testRunner.And("que outro assinante se inscreva para receber a mensagem OtherEventOccurred", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 176
 testRunner.And(string.Format("que a mensagem EventOccurred tenha uma validade de \'{0}\' segundos", timeout), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 177
 testRunner.And(string.Format("que a mensagem OtherEventOccurred tenha uma validade de \'{0}\' segundos", timeout), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 178
 testRunner.When(string.Format("\'{0}\' mensagens EventOccurred forem publicadas no barramento", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line 179
 testRunner.And(string.Format("\'{0}\' mensagens OtherEventOccurred forem publicadas no barramento", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 180
 testRunner.Then(string.Format("\'{0}\' mensagens EventOccurred devem ser entregues ao assinante", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line 181
 testRunner.And(string.Format("\'{0}\' mensagens OtherEventOccurred devem ser entregues ao assinante", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 182
 testRunner.And(string.Format("\'{0}\' mensagens EventOccurred devem ser entregues ao outro assinante", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line 183
 testRunner.And(string.Format("\'{0}\' mensagens OtherEventOccurred devem ser entregues ao outro assinante", messageCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion