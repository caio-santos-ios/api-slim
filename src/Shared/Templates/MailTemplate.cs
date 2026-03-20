namespace api_slim.src.Shared.Templates
{

    public static class MailTemplate
    {
        private static readonly string UiURI =  Environment.GetEnvironmentVariable("UI_URI") ?? "";
        public static string ForgotPasswordWebV1(string code)
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
        public static string FirstAccess(string userName, string email, string passowrd, string path)
        {
            string azulMarinho = "#003366";   
            string verdeAgua = "#66CC99";     
            string prata = "#C0C0C0";         
            string fundoSuporte = "#f8f9fa";
            string fundoLogo = "#EBEBEB"; 

            return $@"
            <html>
                <head>
                    <style>
                        @import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@400;600;700&display=swap');

                        body {{
                            margin: 0;
                            padding: 0;
                            background-color: {prata};
                        }}
                        .container {{
                            font-family: 'Montserrat', sans-serif;
                            background-color: #ffffff;
                            padding: 0 0 40px 0;
                            border-radius: 12px;
                            max-width: 550px;
                            margin: 30px auto;
                            color: {azulMarinho};
                            border-bottom: 6px solid {verdeAgua};
                            overflow: hidden;
                        }}
                        .header {{
                            text-align: center;
                            background-color: {fundoLogo};
                            padding: 35px 20px;
                            margin-bottom: 25px;
                        }}
                        .logo {{
                            max-width: 160px;
                            height: auto;
                            display: block;
                            margin: 0 auto 15px auto;
                        }}
                        .content {{
                            padding: 0 40px;
                        }}
                        h2 {{
                            color: {azulMarinho};
                            font-weight: 700;
                            font-size: 22px;
                            margin: 0;
                        }}
                        .step-card {{
                            background-color: {fundoSuporte};
                            padding: 20px;
                            border-left: 4px solid {azulMarinho};
                            border-radius: 6px;
                            margin-bottom: 20px;
                        }}
                        .step-title {{
                            font-weight: 700;
                            font-size: 13px;
                            margin-bottom: 10px;
                            display: block;
                            color: {azulMarinho};
                            text-transform: uppercase;
                        }}
                        ul {{ padding-left: 18px; margin: 0; color: #444; }}
                        li {{ margin-bottom: 8px; font-size: 13px; line-height: 1.4; }}

                        .button {{
                            display: inline-block;
                            padding: 18px 40px;
                            margin: 10px 0 25px 0;
                            background-color: {verdeAgua};
                            color: {azulMarinho} !important;
                            text-decoration: none;
                            border-radius: 50px;
                            font-weight: 700;
                            font-size: 15px;
                            text-transform: uppercase;
                        }}
                        .credential-section {{
                            border: 2px dashed {prata};
                            padding: 20px;
                            border-radius: 10px;
                            margin: 20px 40px 0 40px;
                            text-align: center;
                        }}
                        .label {{ font-size: 11px; color: #777; text-transform: uppercase; }}
                        .value {{ font-size: 18px; font-weight: 700; display: block; margin-bottom: 10px; color: {verdeAgua}; }}

                        .footer {{
                            margin-top: 30px;
                            font-size: 11px;
                            color: #777;
                            text-align: center;
                            padding: 0 40px;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <h2>Bem-vindo ao Pasbem</h2>
                        </div>

                        <div class=""content"">
                            <p>Olá, <strong>{userName}</strong>,</p>
                            <p>Seu primeir acesso está pronto! Siga as credenciais para acessar o ERP:</p>

                            <center>
                                <a href=""{UiURI}{path}"" class=""button"">Acessar Agora</a>
                            </center>
                        </div>

                        <div class=""credential-section"">
                            <span class=""label"">E-mail</span>
                            <span class=""value"">{email}</span>
                            <span class=""label"">Senha Temporária</span>
                            <span class=""value"">{passowrd}</span>
                        </div>

                        <div class=""footer"">
                            <strong>Pasbem Tecnologia</strong><br>
                            © {DateTime.Now.Year}
                        </div>
                    </div>
                </body>
            </html>";
        }
        public static string GetPwaAccessTemplate(string userName, string login, string tempPassword, string appLink, string logoPath)
        {
            string azulMarinho = "#003366";   
            string verdeAgua = "#66CC99";     
            string prata = "#C0C0C0";         
            string fundoSuporte = "#f8f9fa";
            string fundoLogo = "#EBEBEB"; 

            return $@"
            <html>
                <head>
                    <style>
                        @import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@400;600;700&display=swap');

                        body {{
                            margin: 0;
                            padding: 0;
                            background-color: {prata};
                        }}
                        .container {{
                            font-family: 'Montserrat', sans-serif;
                            background-color: #ffffff;
                            padding: 0 0 40px 0;
                            border-radius: 12px;
                            max-width: 550px;
                            margin: 30px auto;
                            color: {azulMarinho};
                            border-bottom: 6px solid {verdeAgua};
                            overflow: hidden;
                        }}
                        .header {{
                            text-align: center;
                            background-color: {fundoLogo};
                            padding: 35px 20px;
                            margin-bottom: 25px;
                        }}
                        .logo {{
                            max-width: 160px;
                            height: auto;
                            display: block;
                            margin: 0 auto 15px auto;
                        }}
                        .content {{
                            padding: 0 40px;
                        }}
                        h2 {{
                            color: {azulMarinho};
                            font-weight: 700;
                            font-size: 22px;
                            margin: 0;
                        }}
                        .step-card {{
                            background-color: {fundoSuporte};
                            padding: 20px;
                            border-left: 4px solid {azulMarinho};
                            border-radius: 6px;
                            margin-bottom: 20px;
                        }}
                        .step-title {{
                            font-weight: 700;
                            font-size: 13px;
                            margin-bottom: 10px;
                            display: block;
                            color: {azulMarinho};
                            text-transform: uppercase;
                        }}
                        ul {{ padding-left: 18px; margin: 0; color: #444; }}
                        li {{ margin-bottom: 8px; font-size: 13px; line-height: 1.4; }}

                        .button {{
                            display: inline-block;
                            padding: 18px 40px;
                            margin: 10px 0 25px 0;
                            background-color: {verdeAgua};
                            color: {azulMarinho} !important;
                            text-decoration: none;
                            border-radius: 50px;
                            font-weight: 700;
                            font-size: 15px;
                            text-transform: uppercase;
                        }}
                        .credential-section {{
                            border: 2px dashed {prata};
                            padding: 20px;
                            border-radius: 10px;
                            margin: 20px 40px 0 40px;
                            text-align: center;
                        }}
                        .label {{ font-size: 11px; color: #777; text-transform: uppercase; }}
                        .value {{ font-size: 18px; font-weight: 700; display: block; margin-bottom: 10px; color: {verdeAgua}; }}

                        .footer {{
                            margin-top: 30px;
                            font-size: 11px;
                            color: #777;
                            text-align: center;
                            padding: 0 40px;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <h2>Bem-vindo ao Pasbem</h2>
                        </div>

                        <div class=""content"">
                            <p>Olá, <strong>{userName}</strong>,</p>
                            <p>Seu acesso exclusivo está pronto! Siga os passos para instalar o App no seu celular:</p>

                            <div class=""step-card"">
                                <span class=""step-title"">🍎 IPHONE (SAFARI)</span>
                                <ul>
                                    <li>Acesse: <strong>{appLink}</strong></li>
                                    <li>Toque no ícone de <strong>Compartilhar</strong>.</li>
                                    <li>Selecione <strong>'Adicionar à Tela de Início'</strong>.</li>
                                </ul>
                            </div>

                            <div class=""step-card"">
                                <span class=""step-title"">🤖 ANDROID (CHROME)</span>
                                <ul>
                                    <li>Acesse: <strong>{appLink}</strong></li>
                                    <li>Toque nos <strong>três pontos</strong> no canto superior.</li>
                                    <li>Selecione <strong>'Instalar aplicativo'</strong>.</li>
                                </ul>
                            </div>

                            <center>
                                <a href=""{appLink}"" class=""button"">Acessar Agora</a>
                            </center>
                        </div>

                        <div class=""credential-section"">
                            <span class=""label"">Usuário</span>
                            <span class=""value"">{login}</span>
                            <span class=""label"">Senha Temporária</span>
                            <span class=""value"">{tempPassword}</span>
                        </div>

                        <div class=""footer"">
                            <strong>Pasbem Tecnologia</strong><br>
                            © {DateTime.Now.Year}
                        </div>
                    </div>
                </body>
            </html>";
        }
        public static string FirstAccessPainel(string userName, string login, string tempPassword, string appLink)
        {
            string azulMarinho = "#003366";   
            string verdeAgua = "#66CC99";     
            string prata = "#C0C0C0";         
            string fundoSuporte = "#f8f9fa";
            string fundoLogo = "#EBEBEB"; 

            return $@"
            <html>
                <head>
                    <style>
                        @import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@400;600;700&display=swap');

                        body {{
                            margin: 0;
                            padding: 0;
                            background-color: {prata};
                        }}
                        .container {{
                            font-family: 'Montserrat', sans-serif;
                            background-color: #ffffff;
                            padding: 0 0 40px 0;
                            border-radius: 12px;
                            max-width: 550px;
                            margin: 30px auto;
                            color: {azulMarinho};
                            border-bottom: 6px solid {verdeAgua};
                            overflow: hidden;
                        }}
                        .header {{
                            text-align: center;
                            background-color: {fundoLogo};
                            padding: 35px 20px;
                            margin-bottom: 25px;
                        }}
                        .logo {{
                            max-width: 100px;
                            height: auto;
                            display: block;
                            margin: 0 auto 15px auto;
                        }}
                        .content {{
                            padding: 0 40px;
                        }}
                        h2 {{
                            color: {azulMarinho};
                            font-weight: 700;
                            font-size: 22px;
                            margin: 0;
                        }}
                        .step-card {{
                            background-color: {fundoSuporte};
                            padding: 20px;
                            border-left: 4px solid {azulMarinho};
                            border-radius: 6px;
                            margin-bottom: 20px;
                        }}
                        .step-title {{
                            font-weight: 700;
                            font-size: 13px;
                            margin-bottom: 10px;
                            display: block;
                            color: {azulMarinho};
                            text-transform: uppercase;
                        }}
                        ul {{ padding-left: 18px; margin: 0; color: #444; }}
                        li {{ margin-bottom: 8px; font-size: 13px; line-height: 1.4; }}

                        .button {{
                            display: inline-block;
                            padding: 18px 40px;
                            margin: 10px 0 25px 0;
                            background-color: {verdeAgua};
                            color: {azulMarinho} !important;
                            text-decoration: none;
                            border-radius: 50px;
                            font-weight: 700;
                            font-size: 15px;
                            text-transform: uppercase;
                        }}
                        .credential-section {{
                            border: 2px dashed {prata};
                            padding: 20px;
                            border-radius: 10px;
                            margin: 20px 40px 0 40px;
                            text-align: center;
                        }}
                        .label {{ font-size: 11px; color: #777; text-transform: uppercase; }}
                        .value {{ font-size: 18px; font-weight: 700; display: block; margin-bottom: 10px; color: {verdeAgua}; }}

                        .footer {{
                            margin-top: 30px;
                            font-size: 11px;
                            color: #777;
                            text-align: center;
                            padding: 0 40px;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <img src='https://api.pasbem.com.br/uploads/logo/logo.png' class='logo' alt='Logo'>
                            <h2>Bem-vindo ao Pasbem</h2>
                        </div>

                        <div class=""content"">
                            <p>Olá, <strong>{userName}</strong>,</p>
                            <p>Seu acesso exclusivo está pronto! Siga as credenciais para acessar o painel do gestor:</p>
                        </div>

                        <div class=""credential-section"">
                            <span class=""label"">Usuário</span>
                            <span class=""value"">{login}</span>
                            <span class=""label"">Senha Temporária</span>
                            <span class=""value"">{tempPassword}</span>
                        </div>

                        <div class=""content"">
                            <center>
                                <a href=""{appLink}"" class=""button"">Acessar Agora</a>
                            </center>
                        </div>

                        <div class=""footer"">
                            <strong>Pasbem Tecnologia</strong><br>
                            © {DateTime.Now.Year}
                        </div>
                    </div>
                </body>
            </html>";
        }
        public static string ForgotPasswordWeb(string userName, string code)
        {
            string azulMarinho = "#003366";   
            string verdeAgua = "#66CC99";     
            string prata = "#C0C0C0";         
            string fundoSuporte = "#f8f9fa";
            string fundoLogo = "#EBEBEB";

            return $@"
            <html>
                <head>
                    <style>
                        @import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@400;600;700&display=swap');

                        body {{
                            margin: 0;
                            padding: 0;
                            background-color: {prata};
                        }}
                        .container {{
                            font-family: 'Montserrat', sans-serif;
                            background-color: #ffffff;
                            padding: 0 0 40px 0;
                            border-radius: 12px;
                            max-width: 550px;
                            margin: 30px auto;
                            color: {azulMarinho};
                            border-bottom: 6px solid {verdeAgua};
                            overflow: hidden;
                        }}
                        .header {{
                            text-align: center;
                            background-color: {fundoLogo};
                            padding: 35px 20px;
                            margin-bottom: 25px;
                        }}
                        .logo {{
                            max-width: 100px;
                            height: auto;
                            display: block;
                            margin: 0 auto 15px auto;
                        }}
                        .content {{
                            padding: 0 40px;
                        }}
                        h2 {{
                            color: {azulMarinho};
                            font-weight: 700;
                            font-size: 22px;
                            margin: 0;
                        }}
                        .step-card {{
                            background-color: {fundoSuporte};
                            padding: 20px;
                            border-left: 4px solid {azulMarinho};
                            border-radius: 6px;
                            margin-bottom: 20px;
                        }}
                        .step-title {{
                            font-weight: 700;
                            font-size: 13px;
                            margin-bottom: 10px;
                            display: block;
                            color: {azulMarinho};
                            text-transform: uppercase;
                        }}
                        ul {{ padding-left: 18px; margin: 0; color: #444; }}
                        li {{ margin-bottom: 8px; font-size: 13px; line-height: 1.4; }}

                        .button {{
                            display: inline-block;
                            padding: 18px 40px;
                            margin: 10px 0 25px 0;
                            background-color: {verdeAgua};
                            color: {azulMarinho} !important;
                            text-decoration: none;
                            border-radius: 50px;
                            font-weight: 700;
                            font-size: 15px;
                            text-transform: uppercase;
                        }}
                        .credential-section {{
                            border: 2px dashed {prata};
                            padding: 20px;
                            border-radius: 10px;
                            margin: 20px 40px 0 40px;
                            text-align: center;
                        }}
                        .label {{ font-size: 11px; color: #777; text-transform: uppercase; }}
                        .value {{ font-size: 18px; font-weight: 700; display: block; margin-bottom: 10px; color: {verdeAgua}; }}

                        .footer {{
                            margin-top: 30px;
                            font-size: 11px;
                            color: #777;
                            text-align: center;
                            padding: 0 40px;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <img src='https://api.pasbem.com.br/uploads/logo/logo.png' class='logo' alt='Logo'>
                        </div>

                        <div class=""content"">
                            <p>Olá, <strong>{userName}</strong>,</p>
                            <p>Você solicitou a alteração da sua senha. Código de recuperação da nova senha:</p>
                        </div>

                        <div class=""credential-section"">
                            <span class=""value"">{code}</span>
                        </div>

                        <div class=""footer"">
                            <strong>Pasbem Tecnologia</strong><br>
                            © {DateTime.Now.Year}
                        </div>
                    </div>
                </body>
            </html>";
        }
    }
}