# Keylloger

Este código em C# é um registrador de teclas, projetado para capturar as teclas pressionadas em um sistema e enviá-las periodicamente por e-mail. Vou explicar as principais partes do código:

Configurações Iniciais:

As credenciais do Gmail (username e password) são definidas como constantes.
sendTime é o intervalo de tempo (em milissegundos) entre os envios de e-mail, definido como 60 segundos.
Classe Program:

O método Main instancia um objeto da classe SMS (que é o keylogger) e inicia o método Start desse objeto.
Classe SMS:

Contém atributos para armazenar o nome de usuário, senha, intervalo de envio, um objeto StringBuilder para o log e um objeto InputSimulator para simular entrada.
O construtor inicializa esses atributos, incluindo a obtenção do endereço IP externo.
BecomePersistent e SendingMail são métodos que aparentemente têm funcionalidades ainda não implementadas.
MainListener é o método principal que inicia um listener para capturar eventos do teclado e gerar um log com as teclas pressionadas. Esse log é enviado periodicamente por e-mail.
SonModding faz alguma manipulação nas teclas pressionadas antes de registrá-las no log.
Métodos Adicionais:

GetExternalIP utiliza uma solicitação web para obter o endereço IP externo do sistema.
Método de Envio de E-mail:

Usa a classe SmtpClient para enviar e-mails através do servidor SMTP do Gmail.
O corpo do e-mail contém as teclas pressionadas registradas no log.
Método Start:

Inicia a lógica para tornar o programa persistente (não implementada na versão fornecida).
Inicia uma nova thread para o método MainListener e coloca a thread principal para dormir indefinidamente.
