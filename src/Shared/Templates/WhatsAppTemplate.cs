namespace api_slim.src.Handlers;

public static class WhatsAppTemplate
{
    #region BOAS VINDAS
    public static string Welcome(string beneficiaryName) =>
        $"""
        Maravilhoso dia, *{beneficiaryName}*! 😊 💙

        Seja muito bem-vindo(a) à *Pasbem*!

        É uma alegria ter você conosco em nosso *Programa Assistencial de Saúde e Bem-Estar.*

        A partir de agora, você não tem apenas uma assistência, mas um ecossistema completo desenhado para sustentar o seu florescer.
        🚀 *Para começar sua jornada, siga estes passos:*

        *Baixe nosso App:* É por lá que você acessa o *Bem + Cuidado* (Telemedicina 24h com 12+ especialidades) e monitora sua evolução no *Bem Vital.*

        *Ative seu Bem Card:* No módulo *Multibem*, você tem a liberdade de escolher seus profissionais e/ou serviços e produtos associados a saúde e bem estar usando a bandeira VISA.

        *Fale com sua Concierge:* Teve dúvida sobre um agendamento ou precisa de um orçamento? Nossa *Concierge de Saúde* é o seu suporte humano para facilitar tudo.
        ✨ *Dica de Ouro:* Não espere ficar doente para nos usar! Acesse o *Bem Vital* hoje mesmo e descubra o seu *IPV (Índice de Performance Vital)* inicial. Prevenção é a nossa base.

        Estamos aqui para cuidar de você e da sua família. Que sua experiência conosco seja extraordinária! 😊 🙏 💙

        *Equipe Pasbem* 🌿
        _A saúde que você escolhe, o cuidado que você merece_
        """;

    public static string AppDownloadInstructions() =>
    """
    Como prometido, aqui está o caminho para você ter todo o *Ecossistema Pasbem* na palma da sua mão! 📱 🌿
    Nosso aplicativo é moderno e não ocupa espaço na memória do seu celular. Para instalá-lo, basta acessar o link abaixo pelo navegador do seu aparelho e seguir o passo a passo conforme o seu sistema:

    🔗 *Acesse agora:* [pasbem.com.br/aplicativo]


    🤖 *Se o seu celular for ANDROID:*

    Abra o link acima no navegador *Google Chrome.*

    Clique nos *três pontinhos* (⋮) no canto superior direito.

    Selecione a opção *"Instalar aplicativo"* ou *"Adicionar à tela inicial".*

    Prontinho! O ícone da Pasbem aparecerá junto com seus outros apps.


    🍎 *Se o seu celular for IPHONE (iOS):*

    Abra o link acima no navegador *Safari.*

    Clique no ícone de *"Compartilhamento"* (o quadrado com uma seta para cima na parte inferior).

    Role as opções e clique em *"Adicionar à Tela de Início".*

    Clique em *"Adicionar"* no canto superior direito e pronto!


    🔑 *Seu Primeiro Acesso:*
    Utilize o seu CPF e a senha provisória que enviamos para o seu e-mail. Caso precise de ajuda, é só chamar sua *Concierge de Saúde* por aqui!.
    Comece agora a monitorar sua saúde e a usar seus benefícios! 😊 🙏 💙
    """;
    #endregion
    // ── CONSULTA TELEMEDICINA ─────────────────────────────────────────────────────────────

    public static string AppointmentConfirmation(string name, string specialty, string professional, string date, string time, string link, string module) =>
        $"""
        Maravilhoso dia, Sr(a). *{name}*! Como você está hoje? 😊
        Seu agendamento para o módulo *{module}* foi realizado com sucesso! Confira os detalhes:

        🏥 *Especialidade:* {specialty}
        👨‍⚕️ *Profissional:* {professional}
        📅 *Data e Hora:* {date} às {time}
        🔗 *Link da sua consulta:* {link}
        📋 *Instruções Importantes:*

        *Prepare-se:* Certifique-se de estar em um local silencioso e com boa conexão de internet.

        *Antecedência:* Recomendamos acessar o link 03 minutos antes do horário marcado.

        *Alterações:* Qualquer mudança ou cancelamento deve ser feito com até 48h de antecedência para evitar incidência de multa.

        Que Deus lhe abençoe e que seu dia seja incrível! 😊 💙
        """;

    public static string AppointmentDayReminder(string name, string specialty, string date, string time, string link) =>
        $"""
        Olá, Sr(a). *{name}*, tudo bem? Passando para lembrar que amanhã cuidaremos da sua saúde! 🙏 😊
        📌 Sua consulta de *{specialty}* está marcada para amanhã:

        ⏰ *Horário:* {time}
        📅 *Data:* {date}
        🔗 *Acesse por aqui:* {link}

        Falta pouco para o seu atendimento. Tenha um dia extraordinário! 😊 💙
        """;

    public static string AppointmentOneHourReminder(string name, string professional, string time, string link) =>
        $"""
        Maravilhoso dia, Sr(a). *{name}*! Tudo em paz? 😊 💙
        Fique atento(a)! Sua consulta é *hoje, daqui a 1 hora.*
        Este é o momento ideal para conferir sua câmera e microfone.

        👨‍⚕️ *Médico:* {professional}
        ⏰ *Horário:* {time}
        🔗 *Acesse por aqui:* {link}

        Já estamos nos preparando para te receber. Até logo! 😊 💙
        """;

    public static string AppointmentFiveMinutesReminder(string name, string link) =>
        $"""
        Sr(a). *{name}*, sua consulta começará em *5 minutos*! 🙏 🚨
        Recomendamos que você entre na sala agora para aguardar o início do atendimento com tranquilidade.

        🔗 *Clique para entrar:* {link}

        Desejamos uma consulta abençoada! 😊 🙏 🚨
        """;

    // ── CONSULTAS PRESENCIAIS ──────────────────────────────────────────────────
    public static string InPersonConfirmation(string name, string patient, string procedure, string professional, string date, string time, string unit, string address) =>
    $"""
    🎉 *AGENDAMENTO CONFIRMADO* 🎉

    Maravilhoso dia, *{name}*! 😊 💙

    Ficamos felizes em viabilizar o seu cuidado. Seu agendamento presencial foi concluído com sucesso!

    👤 *PACIENTE:* {patient}
    🩺 *PROCEDIMENTO/CONSULTA:* {procedure}
    👨‍⚕️ *PROFISSIONAL:* {professional}
    📅 *DATA:* {date} ⏰ *HORA:* {time}
    🏥 *UNIDADE:* {unit}
    📍 *ENDEREÇO:* {address}
    📋 *Instruções Importantes:*

    *Antecedência:* Recomendamos chegar de 15 a 20 minutos antes para realizar seu cadastro na recepção.

    *Documentação:* Tenha em mãos seu documento de identidade oficial com foto.

    *Exames:* Não esqueça de levar exames anteriores relevantes para o seu atendimento(obrigatório em atendimentos de consultas).

    *Atrasos:* Para garantir a qualidade do atendimento de todos, atrasos podem resultar em reagendamento.

    Que Deus lhe abençoe sempre! 🙏 💙
    """;

    public static string InPersonDayReminder(string name, string patient, string procedure, string professional, string date, string time, string unit) =>
        $"""
        🌿 *LEMBRETE DE CUIDADO* 🌿

        Maravilhoso dia, *{name}*! 😊 💙
        Tudo em paz?
        Passando para lembrar que amanhã temos um compromisso com a sua saúde e bem-estar.

        👤 *PACIENTE:* {patient}
        🩺 *PROCEDIMENTO:* {procedure}
        👨‍⚕️ *PROFISSIONAL:* {professional}
        📅 *AMANHÃ:* {date} ⏰ *HORA:* {time}
        🏥 *LOCAL:* {unit}

        🔖 *Dica da sua Concierge:* Verifique o tempo de deslocamento para chegar com tranquilidade até o local do atendimento.

        Caso tenha alguma dúvida sobre o preparo para o procedimento, entre em contato conosco.

        Que seu dia seja extraordinário e sua consulta abençoada! 🙏 💙
        """;

    public static string InPersonSatisfactionSurvey(string name, string unit, string professional, string surveyLink) =>
        $"""
        Maravilhoso dia, *{name}*! 😊 💙 Esperamos que sua experiência na *{unit}* tenha sido excelente.

        Para que o continuemos a oferecer o melhor cuidado, a sua opinião é essencial.
        Poderia dedicar menos de 1 minuto para avaliar sua experiência?

        📋 *Gostaríamos de saber sobre:*

        A facilidade de recepção e cadastro na unidade.

        O tempo de espera para o início da consulta.

        A atenção e o cuidado do profissional *{professional}*.

        A infraestrutura e limpeza do local.

        🔗 *Responda aqui:* {surveyLink}

        A sua avaliação ajuda-nos a construir uma saúde cada vez mais humana e eficiente. Que o seu dia continue a ser extraordinário! 😊 🙏 💙
        """;

    #region OUTROS
    public static string HappyBirthday(string name) =>
    $"""
    Maravilhoso dia, *{name}*! 😊 💙 🎁 ✨
    Hoje o ecossistema Pasbem está em festa, pois celebramos o seu dia! 🎉

    Mais do que completar um ciclo, desejamos que este novo ano seja marcado por *vitalidade*, equilíbrio e muitos momentos de alegria. É uma honra para nós caminhar ao seu lado, cuidando da sua saúde para que você tenha toda a energia necessária para florescer e realizar seus sonhos.

    Que a sua performance vital esteja sempre em alta e que o seu dia seja tão extraordinário quanto a sua jornada conosco! 🚀 💪

    Felicidades, muita saúde e um novo ciclo abençoado! 🙏

    Com carinho,
    *Equipe Pasbem* 🌿
    """;
    #endregion
}