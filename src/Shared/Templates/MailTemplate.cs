namespace api_slim.src.Shared.Templates
{

    public static class MailTemplate
    {
        private static readonly string UiURI =  Environment.GetEnvironmentVariable("UI_URI") ?? "";
        public static string ForgotPasswordWeb(string code)
        {
            return $@"
                <html>
                    <head>
                        <style>
                        .container {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            padding: 20px;
                            border-radius: 8px;
                            max-width: 600px;
                            margin: auto;
                            color: #333;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            margin-top: 20px;
                            background-color: #007bff;
                            color: #fff;
                            text-decoration: none;
                            border-radius: 5px;
                        }}
                        .footer {{
                            margin-top: 30px;
                            font-size: 12px;
                            color: #888;
                        }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                        <h2>Redefinição de Senha</h2>
                        <p>Você solicitou a alteração da sua senha. Código de recuperação da nova senha:</p>
                        <strong>{code}</strong>
                        <p>Se você não solicitou esta alteração, ignore este e-mail.</p>
                        <div class=""footer"">
                            <p>Este é um e-mail automático. Não responda esta mensagem.</p>
                        </div>
                        </div>
                    </body>
                </html>";
        }
        public static string ForgotPasswordApp(string code)
        {
            return $@"
                <html>
                    <head>
                        <style>
                        .container {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            padding: 20px;
                            border-radius: 8px;
                            max-width: 600px;
                            margin: auto;
                            color: #333;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            margin-top: 20px;
                            background-color: #007bff;
                            color: #fff;
                            text-decoration: none;
                            border-radius: 5px;
                        }}
                        .footer {{
                            margin-top: 30px;
                            font-size: 12px;
                            color: #888;
                        }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                        <h2>Redefinição de Senha</h2>
                        <p>Você solicitou a alteração da sua senha.</p>
                        <p>Código de alteração da senha: {code}.</p>                        
                        <p>Se você não solicitou esta alteração, ignore este e-mail.</p>
                        <div class=""footer"">
                            <p>Este é um e-mail automático. Não responda esta mensagem.</p>
                        </div>
                        </div>
                    </body>
                </html>";
        }
        public static string FirstAccess(string email, string passowrd)
        {
            return $@"
                <html>
                    <head>
                        <style>
                        .container {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            padding: 20px;
                            border-radius: 8px;
                            max-width: 600px;
                            margin: auto;
                            color: #333;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            margin-top: 20px;
                            background-color: #007bff;
                            color: #fff;
                            text-decoration: none;
                            border-radius: 5px;
                        }}
                        .footer {{
                            margin-top: 30px;
                            font-size: 12px;
                            color: #888;
                        }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <p>Dados do primeiro acesso ao sistema:</p>
                            <p>E-mail: {email}</p>                        
                            <p>Senha: {passowrd}</p>                        
                            <p>Se você não solicitou esta alteração, ignore este e-mail.</p>                        
                        </div>
                    </body>
                </html>";
        }
        public static string GetPwaAccessTemplate(string userName, string login, string tempPassword, string appLink)
        {
            return $@"
            <html>
                <head>
                    <style>
                        .container {{
                            font-family: 'Segoe UI', Arial, sans-serif;
                            background-color: #ffffff;
                            padding: 25px;
                            border-radius: 10px;
                            max-width: 600px;
                            margin: 0 auto;
                            color: #333;
                            border: 1px solid #ddd;
                        }}
                        .header {{
                            text-align: center;
                            padding-bottom: 20px;
                            border-bottom: 1px solid #eee;
                        }}
                        .step-title {{
                            color: #007bff;
                            font-size: 18px;
                            margin-top: 25px;
                            display: flex;
                            align-items: center;
                        }}
                        .instructions {{
                            background-color: #f9f9f9;
                            padding: 15px;
                            border-radius: 8px;
                            margin-top: 10px;
                        }}
                        ul {{ padding-left: 20px; }}
                        li {{ margin-bottom: 8px; }}
                        .credential-box {{
                            background-color: #f0f7ff;
                            padding: 20px;
                            border-radius: 8px;
                            margin-top: 30px;
                            border: 1px solid #b3d7ff;
                        }}
                        .button {{
                            display: block;
                            width: 200px;
                            text-align: center;
                            padding: 14px;
                            margin: 25px auto;
                            background-color: #007bff;
                            color: #ffffff !important;
                            text-decoration: none;
                            border-radius: 5px;
                            font-weight: bold;
                        }}
                        .footer {{
                            margin-top: 40px;
                            font-size: 12px;
                            color: #999;
                            text-align: center;
                            border-top: 1px solid #eee;
                            padding-top: 20px;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <h2>Bem-vindo ao Pasbem</h2>
                        </div>

                        <p>Olá, <strong>{userName}</strong>!</p>
                        <p>Para uma melhor experiência, instale nosso aplicativo seguindo o passo a passo abaixo:</p>

                        <div class=""step-title"">🍎 No iPhone (Safari)</div>
                        <div class=""instructions"">
                            <ul>
                                <li>Acesse o link: <strong>{appLink}</strong></li>
                                <li>Toque no ícone de <strong>Compartilhar</strong> (quadrado com uma seta para cima na barra inferior).</li>
                                <li>Role as opções e toque em <strong>'Adicionar à Tela de Início'</strong>.</li>
                                <li>Toque em <strong>'Adicionar'</strong> no canto superior direito.</li>
                            </ul>
                        </div>

                        <div class=""step-title"">🤖 No Android (Chrome)</div>
                        <div class=""instructions"">
                            <ul>
                                <li>Acesse o link: <strong>{appLink}</strong></li>
                                <li>Toque nos <strong>três pontinhos</strong> (canto superior direito).</li>
                                <li>Selecione <strong>'Instalar aplicativo'</strong> ou <strong>'Adicionar à tela inicial'</strong>.</li>
                                <li>Confirme a instalação para criar o ícone.</li>
                            </ul>
                        </div>

                        <a href=""{appLink}"" class=""button"">ABRIR APLICATIVO</a>

                        <div class=""credential-box"">
                            <strong>Suas Credenciais de Acesso:</strong><br><br>
                            <strong>Usuário:</strong> {login}<br>
                            <strong>Senha Temporária:</strong>{tempPassword}<br><br>
                            <small><i>*Ao entrar, você precisará criar uma nova senha pessoal.</i></small>
                        </div>

                        <div class=""footer"">
                            <p>Este e-mail foi enviado automaticamente. Por favor, não responda.</p>
                        </div>
                    </div>
                </body>
            </html>";
        }
    }
}