using Domain.Constants;
using Resend;

namespace Domain.Services;

public class EmailService(IResend resend) : IEmailService
{
    private readonly IResend _resend = resend;

    private static string From =>
        $"{Constant.Settings.EmailServiceSettings.FromName} <{Constant.Settings.EmailServiceSettings.FromEmail}>";

    public async Task SendWelcomeEmailAsync(string recipientName, string recipientEmail)
    {
        var message = new EmailMessage
        {
            From = From,
            Subject = "Bem-vindo(a) à Terra & Tallow!"
        };
        message.To.Add(recipientEmail);
        message.HtmlBody = BuildWelcomeTemplate(recipientName);

        await _resend.EmailSendAsync(message);
    }

    public async Task SendOrderCreatedEmailAsync(
        string recipientName,
        string recipientEmail,
        string orderId,
        decimal totalAmount,
        string shippingCity,
        string shippingState,
        int itemsCount)
    {
        var message = new EmailMessage
        {
            From = From,
            Subject = $"Pedido #{orderId[..8].ToUpper()} recebido com sucesso!"
        };
        message.To.Add(recipientEmail);
        message.HtmlBody = BuildOrderCreatedTemplate(recipientName, orderId, totalAmount, shippingCity, shippingState, itemsCount);

        await _resend.EmailSendAsync(message);
    }

    public async Task SendPaymentApprovedEmailAsync(
        string recipientName,
        string recipientEmail,
        string orderId,
        decimal totalAmount,
        string paymentMethod)
    {
        var message = new EmailMessage
        {
            From = From,
            Subject = $"Pagamento confirmado — Pedido #{orderId[..8].ToUpper()}"
        };
        message.To.Add(recipientEmail);
        message.HtmlBody = BuildPaymentApprovedTemplate(recipientName, orderId, totalAmount, paymentMethod);

        await _resend.EmailSendAsync(message);
    }

    public async Task SendOrderInPreparationEmailAsync(
        string recipientName,
        string recipientEmail,
        string orderId,
        string? estimatedDelivery)
    {
        var message = new EmailMessage
        {
            From = From,
            Subject = $"Seu pedido #{orderId[..8].ToUpper()} está sendo preparado!"
        };
        message.To.Add(recipientEmail);
        message.HtmlBody = BuildOrderInPreparationTemplate(recipientName, orderId, estimatedDelivery);

        await _resend.EmailSendAsync(message);
    }

    public async Task SendOrderShippedEmailAsync(
        string recipientName,
        string recipientEmail,
        string orderId,
        string trackingCode,
        string shippingService)
    {
        var message = new EmailMessage
        {
            From = From,
            Subject = $"Pedido #{orderId[..8].ToUpper()} enviado! Rastreie agora"
        };
        message.To.Add(recipientEmail);
        message.HtmlBody = BuildOrderShippedTemplate(recipientName, orderId, trackingCode, shippingService);

        await _resend.EmailSendAsync(message);
    }

    #region .: HTML TEMPLATES :.

    private const string EmailCss = """
        <style>
          body { margin:0; padding:0; background:#f5f0eb; font-family:'Segoe UI',Helvetica,Arial,sans-serif; color:#2c2825; }
          .wrapper { max-width:600px; margin:0 auto; padding:32px 16px; }
          .card { background:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,.06); }
          .header { background:#2c2825; padding:32px 40px; text-align:center; }
          .header h1 { margin:0; color:#f5ebe0; font-size:22px; font-weight:600; letter-spacing:1px; }
          .header span { color:#c8a97e; font-size:12px; letter-spacing:3px; text-transform:uppercase; display:block; margin-top:4px; }
          .body { padding:40px; }
          .greeting { font-size:18px; font-weight:600; margin-bottom:16px; color:#2c2825; }
          .text { font-size:15px; line-height:1.7; color:#5a4f47; margin-bottom:16px; }
          .divider { border:none; border-top:1px solid #ede8e2; margin:24px 0; }
          .info-box { background:#faf7f4; border-left:3px solid #c8a97e; border-radius:0 8px 8px 0; padding:20px 24px; margin:24px 0; }
          .info-row { display:flex; justify-content:space-between; padding:8px 0; border-bottom:1px solid #ede8e2; font-size:14px; }
          .info-row:last-child { border-bottom:none; }
          .info-label { color:#8a7b72; }
          .info-value { font-weight:600; color:#2c2825; text-align:right; }
          .badge { display:inline-block; background:#2c2825; color:#f5ebe0; padding:6px 14px; border-radius:20px; font-size:13px; font-weight:600; letter-spacing:1px; }
          .tracking-box { background:#2c2825; border-radius:10px; padding:24px; text-align:center; margin:24px 0; }
          .tracking-label { color:#c8a97e; font-size:12px; letter-spacing:2px; text-transform:uppercase; margin-bottom:8px; }
          .tracking-code { color:#f5ebe0; font-size:24px; font-weight:700; letter-spacing:3px; }
          .cta { text-align:center; margin:32px 0; }
          .btn { display:inline-block; background:#c8a97e; color:#2c2825 !important; text-decoration:none; padding:14px 32px; border-radius:8px; font-weight:700; font-size:15px; }
          .footer { padding:24px 40px; text-align:center; }
          .footer p { font-size:12px; color:#a0908a; margin:4px 0; }
        </style>
        """;

    private static string BuildBaseTemplate(string title, string preheader, string content) =>
        $"""
        <!DOCTYPE html>
        <html lang="pt-BR">
        <head>
          <meta charset="UTF-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1.0" />
          <title>{title}</title>
          {EmailCss}
        </head>
        <body>
          <div class="wrapper">
            <div style="display:none;font-size:1px;color:#f5f0eb;line-height:1px;max-height:0;overflow:hidden;">{preheader}</div>
            <div class="card">
              <div class="header">
                <h1>Terra &amp; Tallow</h1>
                <span>Cuidado com intenção</span>
              </div>
              <div class="body">
                {content}
              </div>
              <div class="footer">
                <p>Terra &amp; Tallow &mdash; São Caetano do Sul, SP</p>
                <p>Você está recebendo este e-mail porque realizou uma ação em nossa loja.</p>
              </div>
            </div>
          </div>
        </body>
        </html>
        """;

    private static string BuildWelcomeTemplate(string name)
    {
        var baseUrl = Constant.Settings.BaseUrl;
        var content = $"""
            <p class="greeting">Olá, {name}! 🌿</p>
            <p class="text">
              Seja muito bem-vindo(a) à <strong>Terra &amp; Tallow</strong>. Estamos muito felizes em tê-lo(a) aqui!
            </p>
            <p class="text">
              Nossa loja foi criada com o propósito de levar produtos naturais e artesanais de qualidade até você.
              Cada item é produzido com cuidado, usando ingredientes selecionados e muito amor.
            </p>
            <hr class="divider" />
            <p class="text">
              Agora que você faz parte da nossa comunidade, explore nosso catálogo e encontre o produto perfeito para você.
            </p>
            <div class="cta">
              <a href="{baseUrl}" class="btn">Explorar a loja</a>
            </div>
            <p class="text" style="font-size:13px;color:#a0908a;text-align:center;">
              Qualquer dúvida, estamos à disposição. 💛
            </p>
            """;

        return BuildBaseTemplate(
            "Bem-vindo(a) à Terra & Tallow!",
            $"Olá {name}, sua conta foi criada com sucesso. Explore nosso catálogo!",
            content);
    }

    private static string BuildOrderCreatedTemplate(
        string name, string orderId, decimal totalAmount,
        string shippingCity, string shippingState, int itemsCount)
    {
        var shortId = orderId[..8].ToUpper();
        var productsLabel = itemsCount == 1 ? "produto" : "produtos";
        var content = $"""
            <p class="greeting">Pedido recebido! 🎉</p>
            <p class="text">
              Olá, <strong>{name}</strong>! Seu pedido foi registrado com sucesso e está aguardando confirmação de pagamento.
            </p>
            <div class="info-box">
              <div class="info-row">
                <span class="info-label">Número do pedido</span>
                <span class="info-value"><span class="badge">#{shortId}</span></span>
              </div>
              <div class="info-row">
                <span class="info-label">Itens</span>
                <span class="info-value">{itemsCount} {productsLabel}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Total</span>
                <span class="info-value">R$ {totalAmount:N2}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Entrega para</span>
                <span class="info-value">{shippingCity} — {shippingState}</span>
              </div>
            </div>
            <p class="text">
              Assim que seu pagamento for confirmado, começaremos a preparar seu pedido com todo o carinho. ✨
            </p>
            <p class="text" style="font-size:13px;color:#a0908a;">
              Se você tiver alguma dúvida sobre o seu pedido, responda este e-mail ou entre em contato conosco.
            </p>
            """;

        return BuildBaseTemplate(
            $"Pedido #{shortId} recebido!",
            $"Seu pedido de R$ {totalAmount:N2} foi recebido. Aguardamos a confirmação do pagamento.",
            content);
    }

    private static string BuildPaymentApprovedTemplate(
        string name, string orderId, decimal totalAmount, string paymentMethod)
    {
        var shortId = orderId[..8].ToUpper();
        var methodLabel = paymentMethod.ToUpper() switch
        {
            "PIX" => "Pix",
            "BOLETO" => "Boleto",
            "CREDIT_CARD" => "Cartão de crédito",
            _ => paymentMethod
        };

        var content = $"""
            <p class="greeting">Pagamento confirmado! ✅</p>
            <p class="text">
              Ótima notícia, <strong>{name}</strong>! Recebemos a confirmação do seu pagamento e seu pedido agora está na fila de preparação.
            </p>
            <div class="info-box">
              <div class="info-row">
                <span class="info-label">Número do pedido</span>
                <span class="info-value"><span class="badge">#{shortId}</span></span>
              </div>
              <div class="info-row">
                <span class="info-label">Valor pago</span>
                <span class="info-value">R$ {totalAmount:N2}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Forma de pagamento</span>
                <span class="info-value">{methodLabel}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Status</span>
                <span class="info-value" style="color:#4caf50;">Aprovado</span>
              </div>
            </div>
            <p class="text">
              Agora é com a gente! Em breve seu pedido será separado e embalado com muito cuidado. 📦
            </p>
            <p class="text" style="font-size:13px;color:#a0908a;">
              Você receberá um novo e-mail quando seu pedido for enviado, com o código de rastreio.
            </p>
            """;

        return BuildBaseTemplate(
            $"Pagamento confirmado — Pedido #{shortId}",
            $"Seu pagamento de R$ {totalAmount:N2} foi aprovado. Seu pedido está sendo preparado!",
            content);
    }

    private static string BuildOrderInPreparationTemplate(
        string name, string orderId, string? estimatedDelivery)
    {
        var shortId = orderId[..8].ToUpper();
        var deliveryRow = !string.IsNullOrWhiteSpace(estimatedDelivery)
            ? $"""
              <div class="info-row">
                <span class="info-label">Prazo estimado</span>
                <span class="info-value">{estimatedDelivery}</span>
              </div>
              """
            : string.Empty;

        var content = $"""
            <p class="greeting">Seu pedido está sendo preparado! 🧴</p>
            <p class="text">
              Olá, <strong>{name}</strong>! Seu pedido entrou na fase de preparação.
              Nossa equipe está selecionando e embalando seus produtos com todo o cuidado que eles merecem.
            </p>
            <div class="info-box">
              <div class="info-row">
                <span class="info-label">Número do pedido</span>
                <span class="info-value"><span class="badge">#{shortId}</span></span>
              </div>
              <div class="info-row">
                <span class="info-label">Status</span>
                <span class="info-value" style="color:#e8a030;">Em preparação</span>
              </div>
              {deliveryRow}
            </div>
            <p class="text">
              Em breve você receberá o código de rastreio para acompanhar sua entrega em tempo real. 🌿
            </p>
            <p class="text" style="font-size:13px;color:#a0908a;">
              A Terra &amp; Tallow agradece pela sua preferência!
            </p>
            """;

        return BuildBaseTemplate(
            $"Pedido #{shortId} em preparação",
            $"Seu pedido #{shortId} está sendo separado e embalado pela nossa equipe.",
            content);
    }

    private static string BuildOrderShippedTemplate(
        string name, string orderId, string trackingCode, string shippingService)
    {
        var shortId = orderId[..8].ToUpper();
        var content = $"""
            <p class="greeting">Seu pedido foi enviado! 📦</p>
            <p class="text">
              Boa notícia, <strong>{name}</strong>! Seu pedido foi despachado e está a caminho.
            </p>
            <div class="tracking-box">
              <div class="tracking-label">Código de rastreio</div>
              <div class="tracking-code">{trackingCode}</div>
            </div>
            <div class="info-box">
              <div class="info-row">
                <span class="info-label">Número do pedido</span>
                <span class="info-value"><span class="badge">#{shortId}</span></span>
              </div>
              <div class="info-row">
                <span class="info-label">Transportadora</span>
                <span class="info-value">{shippingService}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Status</span>
                <span class="info-value" style="color:#4caf50;">Enviado</span>
              </div>
            </div>
            <p class="text">
              Você pode rastrear sua encomenda pelo site dos Correios ou da transportadora usando o código acima. 🗺️
            </p>
            <div class="cta">
              <a href="https://www.correios.com.br/rastreamento-de-objetos" class="btn">Rastrear nos Correios</a>
            </div>
            <p class="text" style="font-size:13px;color:#a0908a;text-align:center;">
              Obrigado pela sua compra! Esperamos que você adore seus produtos. 💛
            </p>
            """;

        return BuildBaseTemplate(
            $"Pedido #{shortId} enviado!",
            $"Seu pedido #{shortId} foi enviado! Rastreie com o código {trackingCode}.",
            content);
    }

    #endregion .: HTML TEMPLATES :.
}
