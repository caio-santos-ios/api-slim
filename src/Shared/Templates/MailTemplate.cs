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
        // public static string GetPwaAccessTemplate(string userName, string login, string tempPassword, string appLink)
        // {
        //     // Definição da Paleta de Cores da Empresa
        //     string azulMarinho = "#003366";   // Cor Principal (Títulos e Identidade)
        //     string verdeAgua = "#66CC99";     // Cor de Ação (Botões e Destaques positivos)
        //     string prata = "#C0C0C0";         // Cor de Suporte (Bordas e Detalhes)
        //     string fundoSuporte = "#f8f9fa";  // Cinza claríssimo para contraste

        //     return $@"
        //     <html>
        //         <head>
        //             <style>
        //                 @import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@400;600;700&display=swap');

        //                 body {{
        //                     margin: 0;
        //                     padding: 0;
        //                     background-color: {prata}; /* Fundo externo em prata */
        //                 }}
        //                 .container {{
        //                     font-family: 'Montserrat', sans-serif;
        //                     background-color: #ffffff;
        //                     padding: 40px;
        //                     border-radius: 12px;
        //                     max-width: 550px;
        //                     margin: 30px auto;
        //                     color: {azulMarinho};
        //                     border-bottom: 6px solid {verdeAgua}; /* Detalhe visual inferior */
        //                 }}
        //                 .header {{
        //                     text-align: center;
        //                     margin-bottom: 30px;
        //                 }}
        //                 h2 {{
        //                     color: {azulMarinho};
        //                     font-weight: 700;
        //                     font-size: 24px;
        //                     margin: 0;
        //                 }}
        //                 .step-card {{
        //                     background-color: {fundoSuporte};
        //                     padding: 20px;
        //                     border-left: 4px solid {azulMarinho};
        //                     border-radius: 6px;
        //                     margin-bottom: 15px;
        //                 }}
        //                 .step-title {{
        //                     font-weight: 700;
        //                     font-size: 14px;
        //                     margin-bottom: 8px;
        //                     display: block;
        //                 }}
        //                 .button {{
        //                     display: inline-block;
        //                     padding: 18px 40px;
        //                     margin: 20px 0;
        //                     background-color: {verdeAgua};
        //                     color: {azulMarinho} !important; /* Azul marinho sobre verde para legibilidade */
        //                     text-decoration: none;
        //                     border-radius: 50px;
        //                     font-weight: 700;
        //                     font-size: 16px;
        //                     text-transform: uppercase;
        //                 }}
        //                 .credential-section {{
        //                     border: 2px dashed {prata};
        //                     padding: 20px;
        //                     border-radius: 10px;
        //                     margin-top: 30px;
        //                     text-align: center;
        //                 }}
        //                 .label {{
        //                     font-size: 12px;
        //                     color: #666;
        //                     text-transform: uppercase;
        //                 }}
        //                 .value {{
        //                     font-size: 18px;
        //                     font-weight: 700;
        //                     display: block;
        //                     margin-bottom: 10px;
        //                 }}
        //                 .footer {{
        //                     margin-top: 30px;
        //                     font-size: 11px;
        //                     color: #777;
        //                     text-align: center;
        //                     line-height: 1.6;
        //                 }}
        //             </style>
        //         </head>
        //         <body>
        //             <div class=""container"">
        //                 <div class=""header"">
        //                     <h2>Bem-vindo ao Pasbem</h2>
        //                 </div>

        //                 <p>Olá, <strong>{userName}</strong>,</p>
        //                 <p>Seu acesso está pronto! Para começar a usar, instale nosso Web App no seu celular:</p>

        //                 <div class=""step-card"">
        //                     <span class=""step-title"">🍎 PARA IPHONE (SAFARI)</span>
        //                     <small>Acesse o link, toque em <strong>Compartilhar</strong> e selecione <strong>'Adicionar à Tela de Início'</strong>.</small>
        //                 </div>

        //                 <div class=""step-card"">
        //                     <span class=""step-title"">🤖 PARA ANDROID (CHROME)</span>
        //                     <small>Acesse o link, toque nos <strong>três pontos</strong> e selecione <strong>'Instalar aplicativo'</strong>.</small>
        //                 </div>

        //                 <center>
        //                     <a href=""{appLink}"" class=""button"">Começar Agora</a>
        //                 </center>

        //                 <div class=""credential-section"">
        //                     <span class=""label"">Seu login</span>
        //                     <span class=""value"">{login}</span>
                            
        //                     <span class=""label"">Senha Temporária</span>
        //                     <span class=""value"" style=""color: {azulMarinho};"">{tempPassword}</span>
        //                 </div>

        //                 <div class=""footer"">
        //                     <strong>Pasbem Tecnologia</strong><br>
        //                     Este e-mail é automático. Utilize o link acima para baixar.<br>
        //                     © {DateTime.Now.Year}
        //                 </div>
        //             </div>
        //         </body>
        //     </html>";
        // }

        public static string GetPwaAccessTemplate(string userName, string login, string tempPassword, string appLink, string logoPath)
{
    // 1. Definição da Paleta de Cores
    string azulMarinho = "#003366";   
    string verdeAgua = "#66CC99";     
    string prata = "#C0C0C0";         
    string fundoSuporte = "#f8f9fa";
    string fundoLogo = "#EBEBEB"; // Cor que combina com o fundo da sua imagem

    // 2. Lógica para converter a imagem em Base64
    string base64Logo = "";
    try 
    {
        if (System.IO.File.Exists(logoPath))
        {
            byte[] imageArray = System.IO.File.ReadAllBytes(logoPath);
            string base64Image = Convert.ToBase64String(imageArray);
            base64Logo = $"data:image/png;base64,{base64Image}";
        }
    }
    catch (Exception ex)
    {
        System.Console.WriteLine("Erro ao carregar logo: " + ex.Message);
        // Se falhar, o template segue sem a imagem ou com o alt text
    }

    // 3. Template HTML
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
    }
}