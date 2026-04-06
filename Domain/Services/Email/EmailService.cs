using Domain.Constants;
using Domain.Data.Models.Email;
using Resend;

namespace Domain.Services;

public class EmailService(IResend resend) : IEmailService
{
    private readonly IResend _resend = resend;

    private static string From =>
        $"{Constant.Settings.EmailServiceSettings.FromName} <{Constant.Settings.EmailServiceSettings.FromEmail}>";

    // ─────────────────────────────────────────────────────────────────────────
    //  PUBLIC METHODS
    // ─────────────────────────────────────────────────────────────────────────

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

    public async Task SendOrderCreatedEmailAsync(OrderCreatedEmailData data)
    {
        var shortId = data.OrderId[..8].ToUpper();
        var message = new EmailMessage
        {
            From = From,
            Subject = $"Pedido #{shortId} recebido com sucesso!"
        };
        message.To.Add(data.BuyerEmail);
        message.HtmlBody = BuildOrderCreatedTemplate(data);

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
            Subject = $"Confirmação de pagamento — Pedido #{orderId[..8].ToUpper()}"
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

    public async Task SendPasswordRecoveryEmailAsync(
        string recipientName,
        string recipientEmail,
        string token,
        DateTime expiresAt)
    {
        var message = new EmailMessage
        {
            From = From,
            Subject = "Código de recuperação de senha — Terra & Tallow"
        };
        message.To.Add(recipientEmail);
        message.HtmlBody = BuildPasswordRecoveryTemplate(recipientName, token, expiresAt);

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
            Subject = $"Seu pedido #{orderId[..8].ToUpper()} foi enviado!"
        };
        message.To.Add(recipientEmail);
        message.HtmlBody = BuildOrderShippedTemplate(recipientName, orderId, trackingCode, shippingService);

        await _resend.EmailSendAsync(message);
    }

    private static string WrapLayout(string preheader, string bodyContent) => $"""
        <!DOCTYPE html>
        <html lang="pt-BR">
        <head>
          <meta charset="UTF-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1.0" />
          <title>Terra &amp; Tallow</title>
        </head>
        <body style="margin:0;padding:0;background:#f5f0eb;font-family:'Segoe UI',Helvetica,Arial,sans-serif;color:#2c2825;">

          <div style="display:none;font-size:1px;color:#f5f0eb;line-height:1px;max-height:0;overflow:hidden;mso-hide:all;">{preheader}</div>

          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f5f0eb;">
            <tr>
              <td align="center" style="padding:24px 16px;">
                <table width="600" cellpadding="0" cellspacing="0" style="max-width:600px;width:100%;">

                  {bodyContent}

                </table>
              </td>
            </tr>
          </table>

        </body>
        </html>
        """;

    private static string TopBar() => """
        <tr>
          <td style="padding-bottom:16px;">
            <table width="100%" cellpadding="0" cellspacing="0">
              <tr>
                <td style="vertical-align:middle;">
                  <span style="font-size:20px;font-weight:700;color:#2c2825;letter-spacing:0.5px;">Terra &amp; Tallow</span>
                </td>
                <td style="text-align:right;vertical-align:middle;font-size:12px;color:#5a4f47;line-height:1.8;">
                  <span style="color:#c8a97e;font-weight:600;">terraetallow.com.br</span><br>
                  Telefone: (11) XXXXX-XXXX
                </td>
              </tr>
            </table>
          </td>
        </tr>
        """;

    private static string Footer() => """
        <tr>
          <td>
            <table width="100%" cellpadding="0" cellspacing="0" style="border-radius:0 0 8px 8px;overflow:hidden;">
              <tr>
                <td style="background:#2c2825;padding:24px 32px;text-align:center;">
                  <p style="margin:0 0 4px;color:#f5ebe0;font-size:15px;font-weight:600;letter-spacing:0.5px;">Terra &amp; Tallow</p>
                  <p style="margin:0 0 2px;color:#c8a97e;font-size:12px;">Alameda Conde de Porto Alegre, 1137 — Santa Maria</p>
                  <p style="margin:0 0 12px;color:#c8a97e;font-size:12px;">São Caetano do Sul — SP, 09561-000</p>
                  <p style="margin:0;color:#8a7b72;font-size:11px;">Você está recebendo este e-mail porque realizou uma ação em nossa loja.</p>
                </td>
              </tr>
            </table>
          </td>
        </tr>
        """;

    private static string BuildOrderCreatedTemplate(OrderCreatedEmailData d)
    {
        var shortId = d.OrderId[..8].ToUpper();
        var orderDate = d.OrderDate.ToString("dd/MM/yyyy 'às' HH:mm");
        var complement = !string.IsNullOrWhiteSpace(d.ShippingComplement) ? $", {d.ShippingComplement}" : string.Empty;

        var itemsHtml = BuildItemsRows(d.Items);
        var totalsHtml = BuildTotalsRows(d.SubTotalAmount, d.ShippingAmount, d.TotalAmount, d.ShippingService, d.ShippingDeliveryTime);
        var deliveryAlert = BuildDeliveryAlert(d.ShippingDeliveryTime);

        var body = $"""
            {TopBar()}

            <!-- CARD -->
            <tr>
              <td style="background:#ffffff;border-radius:8px 8px 0 0;overflow:hidden;">

                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="background:#2c2825;padding:22px 32px;">
                      <table width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                          <td style="vertical-align:middle;">
                            <!-- checkmark circle -->
                            <table cellpadding="0" cellspacing="0" style="display:inline-table;vertical-align:middle;">
                              <tr>
                                <td style="width:38px;height:38px;background:#c8a97e;border-radius:50%;text-align:center;vertical-align:middle;font-size:18px;color:#2c2825;font-weight:700;line-height:38px;">&#10003;</td>
                                <td style="padding-left:14px;vertical-align:middle;">
                                  <span style="font-size:16px;color:#f5ebe0;font-weight:400;">Recebemos seu pedido:&nbsp;</span>
                                  <span style="font-size:16px;color:#c8a97e;font-weight:700;">#{shortId}</span>
                                </td>
                              </tr>
                            </table>
                          </td>
                          <td style="text-align:right;vertical-align:middle;">
                            <span style="font-size:11px;color:#c8a97e;letter-spacing:0.5px;">{orderDate}</span>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>

                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="padding:28px 32px 8px;">
                      <p style="margin:0 0 8px;font-size:15px;color:#2c2825;">Olá <strong>{d.BuyerName}</strong>,</p>
                      <p style="margin:0;font-size:14px;color:#5a4f47;line-height:1.6;">
                        Seu pedido foi registrado com sucesso. Você receberá um novo e-mail assim que o pagamento for confirmado.
                      </p>
                    </td>
                  </tr>
                </table>

                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="padding:20px 32px;">
                      <table width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                          <td width="32%" style="text-align:center;vertical-align:top;">
                            <table width="100%" cellpadding="0" cellspacing="0">
                              <tr>
                                <td style="background:#c8a97e;border-radius:6px;padding:12px 8px;text-align:center;">
                                  <div style="font-size:18px;margin-bottom:4px;">&#128203;</div>
                                  <div style="font-size:12px;font-weight:700;color:#2c2825;line-height:1.4;">Pedido<br>realizado</div>
                                </td>
                              </tr>
                            </table>
                          </td>
                          <td width="2%" style="text-align:center;vertical-align:middle;font-size:16px;color:#c8a97e;padding:0 2px;">&#8250;</td>
                          <!-- Stage 2: inactive -->
                          <td width="32%" style="text-align:center;vertical-align:top;">
                            <table width="100%" cellpadding="0" cellspacing="0">
                              <tr>
                                <td style="background:#f0ebe5;border-radius:6px;padding:12px 8px;text-align:center;">
                                  <div style="font-size:18px;margin-bottom:4px;">&#128179;</div>
                                  <div style="font-size:12px;color:#a0908a;line-height:1.4;">Aprovação<br>do pagamento</div>
                                </td>
                              </tr>
                            </table>
                          </td>
                          <td width="2%" style="text-align:center;vertical-align:middle;font-size:16px;color:#c8a97e;padding:0 2px;">&#8250;</td>
                          <!-- Stage 3: inactive -->
                          <td width="32%" style="text-align:center;vertical-align:top;">
                            <table width="100%" cellpadding="0" cellspacing="0">
                              <tr>
                                <td style="background:#f0ebe5;border-radius:6px;padding:12px 8px;text-align:center;">
                                  <div style="font-size:18px;margin-bottom:4px;">&#128666;</div>
                                  <div style="font-size:12px;color:#a0908a;line-height:1.4;">Produto em<br>transporte</div>
                                </td>
                              </tr>
                            </table>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>

                {deliveryAlert}

                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr><td style="padding:0 32px;"><hr style="border:none;border-top:1px solid #ede8e2;margin:0;" /></td></tr>
                </table>

                <table width="100%" cellpadding="0" cellspacing="0" style="padding:0 32px;">
                  <tr>
                    <td style="padding:0 32px;">
                      <table width="100%" cellpadding="0" cellspacing="0">

                        <tr style="border-bottom:1px solid #ede8e2;">
                          <td style="padding:12px 0 10px;font-size:13px;font-weight:600;color:#8a7b72;border-bottom:2px solid #ede8e2;">Produto</td>
                          <td style="padding:12px 8px 10px;font-size:13px;font-weight:600;color:#8a7b72;text-align:center;width:50px;border-bottom:2px solid #ede8e2;">Qtd.</td>
                          <td style="padding:12px 0 10px;font-size:13px;font-weight:600;color:#8a7b72;text-align:right;width:100px;border-bottom:2px solid #ede8e2;">Preço</td>
                        </tr>

                        {itemsHtml}

                        {totalsHtml}

                      </table>
                    </td>
                  </tr>
                </table>

                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr><td style="height:8px;"></td></tr>
                </table>

                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="padding:0 32px 16px;">
                      <table width="100%" cellpadding="0" cellspacing="0" style="background:#faf7f4;border-radius:8px;overflow:hidden;">
                        <tr>
                          <td style="padding:18px 20px;">
                            <table width="100%" cellpadding="0" cellspacing="0">
                              <tr>
                                <td style="padding-bottom:10px;">
                                  <span style="font-size:16px;vertical-align:middle;">&#127968;</span>&nbsp;
                                  <span style="font-size:14px;font-weight:700;color:#2c2825;vertical-align:middle;">Endere&#231;o de entrega</span>
                                </td>
                              </tr>
                              <tr>
                                <td style="font-size:14px;color:#5a4f47;line-height:1.8;">
                                  {d.ShippingAddress}, {d.ShippingNumber}{complement}<br>
                                  {d.ShippingNeighborhood}<br>
                                  {d.ShippingCity} / {d.ShippingState} &mdash; CEP: {d.ShippingZipcode}
                                </td>
                              </tr>
                            </table>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>

                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="padding:0 32px 32px;">
                      <table width="100%" cellpadding="0" cellspacing="0" style="background:#faf7f4;border-radius:8px;overflow:hidden;">
                        <tr>
                          <td style="padding:18px 20px;">
                            <table width="100%" cellpadding="0" cellspacing="0">
                              <tr>
                                <td style="padding-bottom:10px;">
                                  <span style="font-size:16px;vertical-align:middle;">&#128179;</span>&nbsp;
                                  <span style="font-size:14px;font-weight:700;color:#2c2825;vertical-align:middle;">Forma de pagamento</span>
                                </td>
                              </tr>
                              <tr>
                                <td style="font-size:13px;color:#a0908a;">
                                  Aguardando confirma&#231;&#227;o de pagamento
                                </td>
                              </tr>
                            </table>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>

              </td>
            </tr>

            {Footer()}
            """;

        return WrapLayout(
            $"Recebemos seu pedido #{shortId}! Acompanhe o andamento pelo e-mail.",
            body);
    }

    private static string BuildItemsRows(List<OrderItemEmailData> items)
    {
        var sb = new System.Text.StringBuilder();

        foreach (var item in items)
        {
            var sku = item.ProductId.Length >= 8 ? item.ProductId[..8].ToUpper() : item.ProductId.ToUpper();
            var imgHtml = !string.IsNullOrWhiteSpace(item.ProductImageUrl)
                ? $"""<td style="width:64px;padding-right:14px;vertical-align:top;"><img src="{item.ProductImageUrl}" width="60" height="60" alt="{item.ProductName}" style="border-radius:6px;display:block;object-fit:cover;border:1px solid #ede8e2;" /></td>"""
                : string.Empty;

            sb.Append($"""
                <tr>
                  <td style="padding:14px 0;border-bottom:1px solid #f0ebe5;vertical-align:top;">
                    <table cellpadding="0" cellspacing="0">
                      <tr>
                        {imgHtml}
                        <td style="vertical-align:top;">
                          <div style="font-size:14px;font-weight:600;color:#2c2825;margin-bottom:3px;">{item.ProductName}</div>
                          <div style="font-size:11px;color:#a0908a;">SKU: {sku}</div>
                        </td>
                      </tr>
                    </table>
                  </td>
                  <td style="padding:14px 8px;border-bottom:1px solid #f0ebe5;text-align:center;vertical-align:top;font-size:14px;color:#2c2825;">{item.Quantity}</td>
                  <td style="padding:14px 0;border-bottom:1px solid #f0ebe5;text-align:right;vertical-align:top;font-size:14px;color:#2c2825;white-space:nowrap;">R$ {item.UnitPrice:N2}</td>
                </tr>
                """);
        }

        return sb.ToString();
    }

    private static string BuildTotalsRows(
        decimal subTotal, decimal shipping, decimal total,
        string shippingService, string? deliveryTime)
    {
        var shippingLabel = !string.IsNullOrWhiteSpace(deliveryTime)
            ? $"Frete ({shippingService}) — {deliveryTime}"
            : $"Frete ({shippingService})";

        return $"""
            <tr>
              <td colspan="2" style="padding:10px 0 4px;text-align:right;font-size:13px;color:#5a4f47;"></td>
              <td></td>
            </tr>
            <tr>
              <td colspan="2" style="padding:4px 0;text-align:right;font-size:13px;color:#5a4f47;">Subtotal</td>
              <td style="padding:4px 0;text-align:right;font-size:13px;color:#2c2825;white-space:nowrap;">R$ {subTotal:N2}</td>
            </tr>
            <tr>
              <td colspan="2" style="padding:4px 0;text-align:right;font-size:13px;color:#5a4f47;">{shippingLabel}</td>
              <td style="padding:4px 0;text-align:right;font-size:13px;color:#2c2825;white-space:nowrap;">R$ {shipping:N2}</td>
            </tr>
            <tr>
              <td colspan="2" style="padding:12px 0 14px;text-align:right;font-size:15px;font-weight:700;color:#2c2825;border-top:2px solid #2c2825;">Total</td>
              <td style="padding:12px 0 14px;text-align:right;font-size:15px;font-weight:700;color:#2c2825;border-top:2px solid #2c2825;white-space:nowrap;">R$ {total:N2}</td>
            </tr>
            """;
    }

    private static string BuildDeliveryAlert(string? deliveryTime)
    {
        if (string.IsNullOrWhiteSpace(deliveryTime))
            return string.Empty;

        return $"""
            <table width="100%" cellpadding="0" cellspacing="0">
              <tr>
                <td style="padding:0 32px 20px;">
                  <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                      <td style="background:#fdf6ec;border:1px solid #e8c99a;border-radius:6px;padding:12px 16px;font-size:13px;color:#7a5c2e;line-height:1.6;">
                        <strong>&#128337; PRAZO E ENTREGA:</strong>&nbsp;
                        Seu pedido ser&#225; entregue em at&#233; <strong>{deliveryTime}</strong> ap&#243;s a confirma&#231;&#227;o do pagamento.
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
            </table>
            """;
    }

    private static string BuildWelcomeTemplate(string name)
    {
        var baseUrl = Constant.Settings.BaseUrl;

        var body = $"""
            {TopBar()}
            <tr>
              <td style="background:#ffffff;border-radius:8px 8px 0 0;">
                <!-- Banner -->
                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="background:#2c2825;padding:32px;text-align:center;">
                      <p style="margin:0 0 6px;font-size:22px;font-weight:700;color:#f5ebe0;letter-spacing:1px;">Terra &amp; Tallow</p>
                      <p style="margin:0;font-size:12px;color:#c8a97e;letter-spacing:3px;text-transform:uppercase;">Cuidado com inten&#231;&#227;o</p>
                    </td>
                  </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="padding:36px 40px;">
                      <p style="margin:0 0 16px;font-size:18px;font-weight:600;color:#2c2825;">Ol&#225;, {name}! 🌿</p>
                      <p style="margin:0 0 14px;font-size:15px;line-height:1.7;color:#5a4f47;">
                        Seja muito bem-vindo(a) &#224; <strong>Terra &amp; Tallow</strong>. Estamos muito felizes em t&#234;-lo(a) aqui!
                      </p>
                      <p style="margin:0 0 24px;font-size:15px;line-height:1.7;color:#5a4f47;">
                        Nossa loja foi criada com o prop&#243;sito de levar produtos naturais e artesanais de qualidade at&#233; voc&#234;.
                        Cada item &#233; produzido com cuidado, usando ingredientes selecionados e muito amor.
                      </p>
                      <hr style="border:none;border-top:1px solid #ede8e2;margin:0 0 24px;" />
                      <p style="margin:0 0 24px;font-size:15px;line-height:1.7;color:#5a4f47;">
                        Agora que voc&#234; faz parte da nossa comunidade, explore nosso cat&#225;logo e encontre o produto perfeito para voc&#234;.
                      </p>
                      <table cellpadding="0" cellspacing="0" style="margin:0 auto;">
                        <tr>
                          <td style="background:#c8a97e;border-radius:8px;padding:14px 36px;">
                            <a href="{baseUrl}" style="font-size:15px;font-weight:700;color:#2c2825;text-decoration:none;">Explorar a loja</a>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
            {Footer()}
            """;

        return WrapLayout(
            $"Olá {name}, sua conta foi criada com sucesso. Explore nosso catálogo!",
            body);
    }

    private static string BuildPaymentApprovedTemplate(
        string name, string orderId, decimal totalAmount, string paymentMethod)
    {
        var shortId = orderId[..8].ToUpper();
        var methodLabel = paymentMethod.ToUpper() switch
        {
            "PIX" => "Pix",
            "BOLETO" => "Boleto",
            "CREDIT_CARD" => "Cart&#227;o de cr&#233;dito",
            _ => paymentMethod
        };

        var body = $"""
            {TopBar()}
            <tr>
              <td style="background:#ffffff;border-radius:8px 8px 0 0;">
                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="background:#2c2825;padding:22px 32px;">
                      <table cellpadding="0" cellspacing="0">
                        <tr>
                          <td style="width:38px;height:38px;background:#4caf50;border-radius:50%;text-align:center;vertical-align:middle;font-size:18px;color:#fff;font-weight:700;line-height:38px;">&#10003;</td>
                          <td style="padding-left:14px;vertical-align:middle;">
                            <span style="font-size:16px;color:#f5ebe0;">Pagamento confirmado &mdash; </span>
                            <span style="font-size:16px;color:#c8a97e;font-weight:700;">Pedido #{shortId}</span>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="padding:32px 40px;">
                      <p style="margin:0 0 16px;font-size:15px;color:#2c2825;">&#243;tima not&#237;cia, <strong>{name}</strong>! Seu pagamento foi confirmado.</p>
                      <!-- Info box -->
                      <table width="100%" cellpadding="0" cellspacing="0" style="background:#faf7f4;border-left:3px solid #c8a97e;border-radius:0 8px 8px 0;margin-bottom:24px;">
                        <tr>
                          <td style="padding:20px 24px;">
                            <table width="100%" cellpadding="0" cellspacing="0" style="font-size:14px;">
                              <tr>
                                <td style="padding:6px 0;color:#8a7b72;border-bottom:1px solid #ede8e2;">N&#250;mero do pedido</td>
                                <td style="padding:6px 0;text-align:right;font-weight:700;border-bottom:1px solid #ede8e2;"><span style="background:#2c2825;color:#f5ebe0;padding:4px 12px;border-radius:20px;font-size:13px;">#{shortId}</span></td>
                              </tr>
                              <tr>
                                <td style="padding:6px 0;color:#8a7b72;border-bottom:1px solid #ede8e2;">Valor pago</td>
                                <td style="padding:6px 0;text-align:right;font-weight:700;border-bottom:1px solid #ede8e2;">R$ {totalAmount:N2}</td>
                              </tr>
                              <tr>
                                <td style="padding:6px 0;color:#8a7b72;border-bottom:1px solid #ede8e2;">Forma de pagamento</td>
                                <td style="padding:6px 0;text-align:right;font-weight:700;border-bottom:1px solid #ede8e2;">{methodLabel}</td>
                              </tr>
                              <tr>
                                <td style="padding:6px 0;color:#8a7b72;">Status</td>
                                <td style="padding:6px 0;text-align:right;font-weight:700;color:#4caf50;">Aprovado</td>
                              </tr>
                            </table>
                          </td>
                        </tr>
                      </table>
                      <p style="margin:0;font-size:13px;color:#a0908a;">Voc&#234; receber&#225; um novo e-mail quando seu pedido for enviado, com o c&#243;digo de rastreio. 📦</p>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
            {Footer()}
            """;

        return WrapLayout(
            $"Seu pagamento de R$ {totalAmount:N2} foi aprovado. Pedido #{shortId} em preparação!",
            body);
    }

    private static string BuildOrderInPreparationTemplate(
        string name, string orderId, string? estimatedDelivery)
    {
        var shortId = orderId[..8].ToUpper();
        var deliveryRow = !string.IsNullOrWhiteSpace(estimatedDelivery)
            ? $"""
              <tr>
                <td style="padding:6px 0;color:#8a7b72;">Prazo estimado de prepara&#231;&#227;o</td>
                <td style="padding:6px 0;text-align:right;font-weight:700;">10 dias</td>
              </tr>
              """
            : string.Empty;

        var body = $"""
            {TopBar()}
            <tr>
              <td style="background:#ffffff;border-radius:8px 8px 0 0;">
                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="background:#2c2825;padding:22px 32px;">
                      <span style="font-size:20px;vertical-align:middle;">&#129420;</span>
                      <span style="font-size:16px;color:#f5ebe0;vertical-align:middle;">&nbsp;Pedido em prepara&#231;&#227;o &mdash; <strong style="color:#c8a97e;">#{shortId}</strong></span>
                    </td>
                  </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="padding:32px 40px;">
                      <p style="margin:0 0 16px;font-size:15px;color:#2c2825;">Ol&#225;, <strong>{name}</strong>! Seu pedido est&#225; sendo preparado com muito cuidado.</p>
                      <table width="100%" cellpadding="0" cellspacing="0" style="background:#faf7f4;border-left:3px solid #c8a97e;border-radius:0 8px 8px 0;margin-bottom:24px;">
                        <tr>
                          <td style="padding:20px 24px;">
                            <table width="100%" cellpadding="0" cellspacing="0" style="font-size:14px;">
                              <tr>
                                <td style="padding:6px 0;color:#8a7b72;border-bottom:1px solid #ede8e2;">N&#250;mero do pedido</td>
                                <td style="padding:6px 0;text-align:right;font-weight:700;border-bottom:1px solid #ede8e2;"><span style="background:#2c2825;color:#f5ebe0;padding:4px 12px;border-radius:20px;font-size:13px;">#{shortId}</span></td>
                              </tr>
                              <tr>
                                <td style="padding:6px 0;color:#8a7b72;border-bottom:1px solid #ede8e2;">Status</td>
                                <td style="padding:6px 0;text-align:right;font-weight:700;color:#e8a030;border-bottom:1px solid #ede8e2;">Em prepara&#231;&#227;o</td>
                              </tr>
                              {deliveryRow}
                            </table>
                          </td>
                        </tr>
                      </table>
                      <p style="margin:0;font-size:13px;color:#a0908a;">Em breve voc&#234; receber&#225; o c&#243;digo de rastreio para acompanhar sua entrega. 🌿</p>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
            {Footer()}
            """;

        return WrapLayout(
            $"Seu pedido #{shortId} está sendo separado e embalado pela nossa equipe.",
            body);
    }

    private static string BuildOrderShippedTemplate(
        string name, string orderId, string trackingCode, string shippingService)
    {
        var shortId = orderId[..8].ToUpper();

        var body = $"""
            {TopBar()}
            <tr>
              <td style="background:#ffffff;border-radius:8px 8px 0 0;">
                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="background:#2c2825;padding:22px 32px;">
                      <span style="font-size:20px;vertical-align:middle;">&#128666;</span>
                      <span style="font-size:16px;color:#f5ebe0;vertical-align:middle;">&nbsp;Pedido enviado! &mdash; <strong style="color:#c8a97e;">#{shortId}</strong></span>
                    </td>
                  </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="padding:32px 40px;">
                      <p style="margin:0 0 20px;font-size:15px;color:#2c2825;">Boa not&#237;cia, <strong>{name}</strong>! Seu pedido est&#225; a caminho.</p>
                      <!-- Tracking box -->
                      <table width="100%" cellpadding="0" cellspacing="0" style="background:#2c2825;border-radius:10px;margin-bottom:24px;">
                        <tr>
                          <td style="padding:24px;text-align:center;">
                            <p style="margin:0 0 8px;font-size:11px;color:#c8a97e;letter-spacing:3px;text-transform:uppercase;">C&#243;digo de rastreio</p>
                            <p style="margin:0;font-size:26px;font-weight:700;color:#f5ebe0;letter-spacing:4px;">{trackingCode}</p>
                          </td>
                        </tr>
                      </table>
                      <!-- Info box -->
                      <table width="100%" cellpadding="0" cellspacing="0" style="background:#faf7f4;border-left:3px solid #c8a97e;border-radius:0 8px 8px 0;margin-bottom:24px;">
                        <tr>
                          <td style="padding:20px 24px;">
                            <table width="100%" cellpadding="0" cellspacing="0" style="font-size:14px;">
                              <tr>
                                <td style="padding:6px 0;color:#8a7b72;border-bottom:1px solid #ede8e2;">N&#250;mero do pedido</td>
                                <td style="padding:6px 0;text-align:right;font-weight:700;border-bottom:1px solid #ede8e2;"><span style="background:#2c2825;color:#f5ebe0;padding:4px 12px;border-radius:20px;font-size:13px;">#{shortId}</span></td>
                              </tr>
                              <tr>
                                <td style="padding:6px 0;color:#8a7b72;border-bottom:1px solid #ede8e2;">Transportadora</td>
                                <td style="padding:6px 0;text-align:right;font-weight:700;border-bottom:1px solid #ede8e2;">{shippingService}</td>
                              </tr>
                              <tr>
                                <td style="padding:6px 0;color:#8a7b72;">Status</td>
                                <td style="padding:6px 0;text-align:right;font-weight:700;color:#4caf50;">Enviado</td>
                              </tr>
                            </table>
                          </td>
                        </tr>
                      </table>
                      <!-- CTA -->
                      <table cellpadding="0" cellspacing="0" style="margin:0 auto 20px;">
                        <tr>
                          <td style="background:#c8a97e;border-radius:8px;padding:14px 36px;">
                            <a href="https://www.correios.com.br/rastreamento-de-objetos" style="font-size:15px;font-weight:700;color:#2c2825;text-decoration:none;">Rastrear nos Correios</a>
                          </td>
                        </tr>
                      </table>
                      <p style="margin:0;font-size:13px;color:#a0908a;text-align:center;">Obrigado pela sua compra! Esperamos que voc&#234; adore seus produtos. 💛</p>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
            {Footer()}
            """;

        return WrapLayout(
            $"Seu pedido #{shortId} foi enviado! Rastreie com o código {trackingCode}.",
            body);
    }

    private static string BuildPasswordRecoveryTemplate(string name, string token, DateTime expiresAt)
    {
        var expiry = expiresAt.ToString("HH:mm 'de' dd/MM/yyyy");

        var body = $"""
            {TopBar()}
            <tr>
              <td style="background:#ffffff;border-radius:8px 8px 0 0;">
                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="background:#2c2825;padding:22px 32px;">
                      <span style="font-size:20px;vertical-align:middle;">&#128274;</span>
                      <span style="font-size:16px;color:#f5ebe0;vertical-align:middle;">&nbsp;Recupera&#231;&#227;o de senha</span>
                    </td>
                  </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                  <tr>
                    <td style="padding:32px 40px;">
                      <p style="margin:0 0 8px;font-size:15px;color:#2c2825;">Ol&#225;, <strong>{name}</strong>!</p>
                      <p style="margin:0 0 24px;font-size:14px;line-height:1.7;color:#5a4f47;">
                        Recebemos uma solicita&#231;&#227;o para redefinir a senha da sua conta.
                        Use o c&#243;digo abaixo para continuar. Ele &#233; v&#225;lido por <strong>{Constant.Settings.AuthSettings.RecoveryPasswordExpiration} minutos</strong>.
                      </p>

                      <!-- OTP Token box -->
                      <table width="100%" cellpadding="0" cellspacing="0" style="margin-bottom:24px;">
                        <tr>
                          <td style="background:#faf7f4;border:2px dashed #c8a97e;border-radius:10px;padding:28px;text-align:center;">
                            <p style="margin:0 0 6px;font-size:11px;color:#a0908a;letter-spacing:3px;text-transform:uppercase;">Seu c&#243;digo</p>
                            <p style="margin:0;font-size:36px;font-weight:700;color:#2c2825;letter-spacing:10px;">{token}</p>
                          </td>
                        </tr>
                      </table>

                      <!-- Expiry alert -->
                      <table width="100%" cellpadding="0" cellspacing="0" style="margin-bottom:24px;">
                        <tr>
                          <td style="background:#fdf6ec;border:1px solid #e8c99a;border-radius:6px;padding:12px 16px;font-size:13px;color:#7a5c2e;line-height:1.6;">
                            &#9888;&#65039; Este c&#243;digo expira em <strong>{expiry}</strong>.
                            N&#227;o compartilhe com ningu&#233;m.
                          </td>
                        </tr>
                      </table>

                      <p style="margin:0;font-size:13px;color:#a0908a;line-height:1.6;">
                        Se voc&#234; n&#227;o solicitou a recupera&#231;&#227;o de senha, ignore este e-mail.
                        Sua senha permanece a mesma.
                      </p>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
            {Footer()}
            """;

        return WrapLayout(
            "Seu código de recuperação de senha Terra & Tallow está disponível.",
            body);
    }
}