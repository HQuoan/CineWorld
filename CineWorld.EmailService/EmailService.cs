using CineWorld.EmailService;
using MailKit.Net.Smtp;
using MimeKit;

namespace CineWorld.EmailService
{
  public class EmailService : IEmailService
  {
    public async Task<ResponseEmailDto> SendEmailAsync(EmailRequest emailRequest)
    {
      try
      {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(EmailConfig.USERNAME));
        email.To.Add(MailboxAddress.Parse(emailRequest.To));
        email.Subject = emailRequest.Subject;

        var bodyBuilder = new BodyBuilder
        {
          HtmlBody = GenMessage()
        };
        email.Body = bodyBuilder.ToMessageBody();

        using (var smtp = new SmtpClient())
        {
          await smtp.ConnectAsync(EmailConfig.SERVER, EmailConfig.PORT, MailKit.Security.SecureSocketOptions.StartTls);
          await smtp.AuthenticateAsync(EmailConfig.USERNAME, EmailConfig.PASSWORD);
          await smtp.SendAsync(email);
          await smtp.DisconnectAsync(true);
        }

        return new ResponseEmailDto() { 
          Message = "Email sent successfully!"
        };
      }
      catch (SmtpCommandException ex)
      {

        return new ResponseEmailDto()
        {
          IsSuccess = false,
          Message = $"Failed to send email: {ex.Message} (SMTP Code: {ex.StatusCode})"
        };
      }
      catch (Exception ex)
      {
        return new ResponseEmailDto()
        {
          IsSuccess = false,
          Message = $"An error occurred while sending email: {ex.Message}"
        };
      }
    }

    private string GenMessage()
    {
      var currentDate = DateTime.Now.ToString("MM/dd/yyyy");
      return $@"<html dir=""ltr"" xmlns=""http://www.w3.org/1999/xhtml"" xmlns:o=""urn:schemas-microsoft-com:office:office"">
  <head>
    <meta charset=""UTF-8"">
    <meta content=""width=device-width, initial-scale=1"" name=""viewport"">
    <meta name=""x-apple-disable-message-reformatting"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta content=""telephone=no"" name=""format-detection"">
    <title></title>
    <!--[if (mso 16)]>
    <style type=""text/css"">
    a {{text-decoration: none;}}
    </style>
    <![endif]-->
    <!--[if gte mso 9]><style>sup {{ font-size: 100% !important; }}</style><![endif]-->
    <!--[if gte mso 9]>
<noscript>
         <xml>
           <o:OfficeDocumentSettings>
           <o:AllowPNG></o:AllowPNG>
           <o:PixelsPerInch>96</o:PixelsPerInch>
           </o:OfficeDocumentSettings>
         </xml>
      </noscript>
<![endif]-->
  </head>
  <body class=""body"">
    <div dir=""ltr"" class=""es-wrapper-color"">
      <!--[if gte mso 9]>
			<v:background xmlns:v=""urn:schemas-microsoft-com:vml"" fill=""t"">
				<v:fill type=""tile"" color=""#fafafa""></v:fill>
			</v:background>
		<![endif]-->
      <table width=""100%"" cellspacing=""0"" cellpadding=""0"" class=""es-wrapper"">
        <tbody>
          <tr>
            <td valign=""top"" class=""esd-email-paddings"">
              <table cellpadding=""0"" cellspacing=""0"" align=""center"" class=""es-content esd-header-popover"">
                <tbody>
                  <tr>
                    <td align=""center"" class=""esd-stripe esd-synchronizable-module"">
                      <table align=""center"" cellpadding=""0"" cellspacing=""0"" width=""600"" bgcolor=""rgba(0, 0, 0, 0)"" class=""es-content-body"" style=""background-color:transparent"">
                        <tbody>
                          <tr>
                            <td align=""left"" class=""esd-structure es-p20"">
                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tbody>
                                  <tr>
                                    <td width=""560"" align=""center"" valign=""top"" class=""esd-container-frame"">
                                      <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                        <tbody>
                                          <tr>
                                            <td align=""center"" class=""esd-block-text es-infoblock"">
                                              <p>
                                                <a target=""_blank"">View online version</a>
                                              </p>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </td>
                  </tr>
                </tbody>
              </table>
              <table cellpadding=""0"" cellspacing=""0"" align=""center"" class=""es-header"">
                <tbody>
                  <tr>
                    <td align=""center"" class=""esd-stripe"">
                      <table bgcolor=""#ffffff"" align=""center"" cellpadding=""0"" cellspacing=""0"" width=""600"" class=""es-header-body"">
                        <tbody>
                          <tr>
                            <td align=""left"" class=""esd-structure es-p20"">
                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tbody>
                                  <tr>
                                    <td width=""560"" valign=""top"" align=""center"" class=""es-m-p0r esd-container-frame"">
                                      <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                        <tbody>
                                          <tr>
                                            <td align=""center"" class=""esd-block-image es-p10b"" style=""font-size:0px"">
                                              <a target=""_blank"">
                                                <img src=""https://eroqzme.stripocdn.email/content/guids/CABINET_887f48b6a2f22ad4fb67bc2a58c0956b/images/93351617889024778.png"" alt=""Logo"" width=""200"" title=""Logo"" style=""display:block;font-size:12px"">
                                              </a>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </td>
                  </tr>
                </tbody>
              </table>
              <table cellpadding=""0"" cellspacing=""0"" align=""center"" class=""es-content"">
                <tbody>
                  <tr>
                    <td align=""center"" class=""esd-stripe"">
                      <table bgcolor=""#ffffff"" align=""center"" cellpadding=""0"" cellspacing=""0"" width=""600"" class=""es-content-body"">
                        <tbody>
                          <tr>
                            <td align=""left"" class=""esd-structure es-p15t es-p20r es-p20l"">
                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tbody>
                                  <tr>
                                    <td width=""560"" align=""center"" valign=""top"" class=""esd-container-frame"">
                                      <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                        <tbody>
                                          <tr>
                                            <td align=""center"" class=""esd-block-image es-p10t es-p10b"" style=""font-size:0px"">
                                              <a target=""_blank"">
                                                <img src=""https://eroqzme.stripocdn.email/content/guids/CABINET_c0e87147643dfd412738cb6184109942/images/151618429860259.png"" alt="""" width=""100"" style=""display:block"">
                                              </a>
                                            </td>
                                          </tr>
                                          <tr>
                                            <td align=""center"" class=""esd-block-text es-p10b"">
                                              <h1 class=""es-m-txt-c"" style=""font-size:46px;line-height:100%"">
                                                Thanks for&nbsp;choosing us!
                                              </h1>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </td>
                  </tr>
                </tbody>
              </table>
              <table cellpadding=""0"" cellspacing=""0"" align=""center"" class=""es-content"">
                <tbody>
                  <tr>
                    <td align=""center"" class=""esd-stripe"">
                      <table bgcolor=""#ffffff"" align=""center"" cellpadding=""0"" cellspacing=""0"" width=""600"" class=""es-content-body"">
                        <tbody>
                          <tr>
                            <td align=""left"" class=""esd-structure es-p20t es-p10b es-p20r es-p20l"">
                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tbody>
                                  <tr>
                                    <td width=""560"" align=""center"" valign=""top"" class=""esd-container-frame"">
                                      <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                        <tbody>
                                          <tr>
                                            <td align=""center"" class=""esd-block-text es-p5t es-p5b es-p40r es-p40l es-m-p0r es-m-p0l"">
                                              <p>
                                                Your order&nbsp;has now been completed!&nbsp;<br>We’ve attached your <strong>receipt </strong>to this email.
                                              </p>
                                            </td>
                                          </tr>
                                          <tr>
                                            <td align=""center"" class=""esd-block-button es-p10t es-p10b"">
                                              <span class=""es-button-border"">
                                                <a href="""" target=""_blank"" class=""es-button"">
                                                  MY ACCOUNT
                                                </a>
                                              </span>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                          </tr>
                          <tr>
                            <td align=""left"" class=""esd-structure es-p10t es-p10b es-p20r es-p20l esdev-adapt-off"">
                              <table width=""560"" cellpadding=""0"" cellspacing=""0"" class=""esdev-mso-table"">
                                <tbody>
                                  <tr>
                                    <td valign=""top"" class=""esdev-mso-td"">
                                      <table cellpadding=""0"" cellspacing=""0"" align=""left"" class=""es-left"">
                                        <tbody>
                                          <tr>
                                            <td width=""70"" align=""center"" class=""es-m-p0r esd-container-frame"">
                                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                <tbody>
                                                  <tr>
                                                    <td align=""center"" class=""esd-block-image"" style=""font-size:0px"">
                                                      <a target=""_blank"">
                                                        <img src=""https://eroqzme.stripocdn.email/content/guids/CABINET_c67048fd0acf81b47e18129166337c05/images/79021618299486570.png"" alt="""" width=""70"" class=""adapt-img"" style=""display:block"">
                                                      </a>
                                                    </td>
                                                  </tr>
                                                </tbody>
                                              </table>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                    <td width=""20""></td>
                                    <td valign=""top"" class=""esdev-mso-td"">
                                      <table cellpadding=""0"" cellspacing=""0"" align=""left"" class=""es-left"">
                                        <tbody>
                                          <tr>
                                            <td width=""265"" align=""center"" class=""esd-container-frame"">
                                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                <tbody>
                                                  <tr>
                                                    <td align=""left"" class=""esd-block-text"">
                                                      <p>
                                                        <strong>Polo</strong>
                                                      </p>
                                                    </td>
                                                  </tr>
                                                  <tr>
                                                    <td align=""left"" class=""esd-block-text es-p5t"">
                                                      <p>
                                                        Size: M<br>Color: White
                                                      </p>
                                                    </td>
                                                  </tr>
                                                </tbody>
                                              </table>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                    <td width=""20""></td>
                                    <td valign=""top"" class=""esdev-mso-td"">
                                      <table cellpadding=""0"" cellspacing=""0"" align=""left"" class=""es-left"">
                                        <tbody>
                                          <tr>
                                            <td width=""80"" align=""left"" class=""esd-container-frame"">
                                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                <tbody>
                                                  <tr>
                                                    <td align=""center"" class=""esd-block-text"">
                                                      <p>
                                                        2 pcs
                                                      </p>
                                                    </td>
                                                  </tr>
                                                </tbody>
                                              </table>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                    <td width=""20""></td>
                                    <td valign=""top"" class=""esdev-mso-td"">
                                      <table cellpadding=""0"" cellspacing=""0"" align=""right"" class=""es-right"">
                                        <tbody>
                                          <tr>
                                            <td width=""85"" align=""left"" class=""esd-container-frame"">
                                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                <tbody>
                                                  <tr>
                                                    <td align=""right"" class=""esd-block-text"">
                                                      <p>
                                                        $20
                                                      </p>
                                                    </td>
                                                  </tr>
                                                </tbody>
                                              </table>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                          </tr>
                          <tr>
                            <td align=""left"" class=""esd-structure es-p10t es-p10b es-p20r es-p20l esdev-adapt-off"">
                              <table width=""560"" cellpadding=""0"" cellspacing=""0"" class=""esdev-mso-table"">
                                <tbody>
                                  <tr>
                                    <td valign=""top"" class=""esdev-mso-td"">
                                      <table cellpadding=""0"" cellspacing=""0"" align=""left"" class=""es-left"">
                                        <tbody>
                                          <tr>
                                            <td width=""70"" align=""center"" class=""es-m-p0r esd-container-frame"">
                                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                <tbody>
                                                  <tr>
                                                    <td align=""center"" class=""esd-block-image"" style=""font-size:0px"">
                                                      <a target=""_blank"">
                                                        <img src=""https://eroqzme.stripocdn.email/content/guids/CABINET_c67048fd0acf81b47e18129166337c05/images/43961618299486640.png"" alt="""" width=""70"" class=""adapt-img"" style=""display:block"">
                                                      </a>
                                                    </td>
                                                  </tr>
                                                </tbody>
                                              </table>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                    <td width=""20""></td>
                                    <td valign=""top"" class=""esdev-mso-td"">
                                      <table cellpadding=""0"" cellspacing=""0"" align=""left"" class=""es-left"">
                                        <tbody>
                                          <tr>
                                            <td width=""265"" align=""center"" class=""esd-container-frame"">
                                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                <tbody>
                                                  <tr>
                                                    <td align=""left"" class=""esd-block-text"">
                                                      <p>
                                                        <strong>T-shirt</strong>
                                                      </p>
                                                    </td>
                                                  </tr>
                                                  <tr>
                                                    <td align=""left"" class=""esd-block-text es-p5t"">
                                                      <p>
                                                        Size: M<br>Color: White
                                                      </p>
                                                    </td>
                                                  </tr>
                                                </tbody>
                                              </table>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                    <td width=""20""></td>
                                    <td valign=""top"" class=""esdev-mso-td"">
                                      <table cellpadding=""0"" cellspacing=""0"" align=""left"" class=""es-left"">
                                        <tbody>
                                          <tr>
                                            <td width=""80"" align=""left"" class=""esd-container-frame"">
                                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                <tbody>
                                                  <tr>
                                                    <td align=""center"" class=""esd-block-text"">
                                                      <p>
                                                        1 pcs
                                                      </p>
                                                    </td>
                                                  </tr>
                                                </tbody>
                                              </table>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                    <td width=""20""></td>
                                    <td valign=""top"" class=""esdev-mso-td"">
                                      <table cellpadding=""0"" cellspacing=""0"" align=""right"" class=""es-right"">
                                        <tbody>
                                          <tr>
                                            <td width=""85"" align=""left"" class=""esd-container-frame"">
                                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                <tbody>
                                                  <tr>
                                                    <td align=""right"" class=""esd-block-text"">
                                                      <p>
                                                        $20
                                                      </p>
                                                    </td>
                                                  </tr>
                                                </tbody>
                                              </table>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                          </tr>
                          <tr>
                            <td align=""left"" class=""esd-structure es-p10t es-p20r es-p20l"">
                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tbody>
                                  <tr>
                                    <td width=""560"" align=""center"" class=""es-m-p0r esd-container-frame"">
                                      <table cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-width:2px 0;border-style:solid solid;border-color:#efefef rgba(0,0,0,0)"">
                                        <tbody>
                                          <tr>
                                            <td align=""right"" class=""esd-block-text es-p10t es-p20b"">
                                              <p class=""es-m-txt-r"">
                                                Subtotal:&nbsp;<strong>$40.00</strong><br>Shipping:&nbsp;<strong>$0.00</strong><br>Tax:&nbsp;<strong>$10.00</strong><br>Total:&nbsp;<strong>$50.00</strong>
                                              </p>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                          </tr>
                          <tr>
                            <td align=""left"" class=""esd-structure es-p20t es-p10b es-p20r es-p20l"">
                              <!--[if mso]><table width=""560"" cellpadding=""0"" cellspacing=""0""><tr><td width=""280"" valign=""top""><![endif]-->
                              <table cellpadding=""0"" cellspacing=""0"" align=""left"" class=""es-left"">
                                <tbody>
                                  <tr>
                                    <td width=""280"" align=""center"" class=""es-m-p0r esd-container-frame es-m-p20b"">
                                      <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                        <tbody>
                                          <tr>
                                            <td align=""left"" class=""esd-block-text"">
                                              <p>
                                                Customer:<strong>sarah_powell@domain.com</strong>
                                              </p>
                                              <p>
                                                Order number:&nbsp;<strong>#65000500</strong>
                                              </p>
                                              <p>
                                                Invoice date:&nbsp;<strong>Apr 17, 2021</strong>
                                              </p>
                                              <p>
                                                Payment method:&nbsp;<strong>PayPal</strong>
                                              </p>
                                              <p>
                                                Currency:&nbsp;<strong>USD</strong>
                                              </p>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                              <!--[if mso]></td><td width=""0""></td><td width=""280"" valign=""top""><![endif]-->
                              <table cellpadding=""0"" cellspacing=""0"" align=""right"" class=""es-right"">
                                <tbody>
                                  <tr>
                                    <td width=""280"" align=""center"" class=""es-m-p0r esd-container-frame"">
                                      <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                        <tbody>
                                          <tr>
                                            <td align=""left"" class=""esd-block-text"">
                                              <p class=""es-m-txt-l"">
                                                Shipping Method:<strong>UPS - Ground</strong>
                                              </p>
                                              <p class=""es-m-txt-l"">
                                                Shipping address:
                                              </p>
                                              <p class=""es-m-txt-l"">
                                                <strong>Sarah Powell,<br>600 Montgomery St,<br>San Francisco, CA 94111</strong>
                                              </p>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                              <!--[if mso]></td></tr></table><![endif]-->
                            </td>
                          </tr>
                          <tr>
                            <td align=""left"" class=""esd-structure es-p15t es-p10b es-p20r es-p20l"">
                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tbody>
                                  <tr>
                                    <td width=""560"" align=""left"" class=""esd-container-frame"">
                                      <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                        <tbody>
                                          <tr>
                                            <td align=""center"" class=""esd-block-text es-p10t es-p10b"">
                                              <p>
                                                Got a question?&nbsp;Email us at&nbsp;<a target=""_blank"" href=""mailto:"">support@</a><a target=""_blank"" href=""mailto:"">stylecasual</a><a target=""_blank"" href=""mailto:"">.com</a>&nbsp;or give us a call at&nbsp;<a target=""_blank"" href=""tel:"">+000 123 456</a>.
                                              </p>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </td>
                  </tr>
                </tbody>
              </table>
              <table cellpadding=""0"" cellspacing=""0"" align=""center"" class=""es-footer"">
                <tbody>
                  <tr>
                    <td align=""center"" class=""esd-stripe"">
                      <table align=""center"" cellpadding=""0"" cellspacing=""0"" width=""600"" class=""es-footer-body"" style=""background-color:transparent"">
                        <tbody>
                          <tr>
                            <td align=""left"" class=""esd-structure es-p20t es-p20b es-p20r es-p20l"">
                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tbody>
                                  <tr>
                                    <td width=""560"" align=""left"" class=""esd-container-frame"">
                                      <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                        <tbody>
                                          <tr>
                                            <td align=""center"" class=""esd-block-social es-p15t es-p15b"" style=""font-size:0"">
                                              <table cellpadding=""0"" cellspacing=""0"" class=""es-table-not-adapt es-social"">
                                                <tbody>
                                                  <tr>
                                                    <td align=""center"" valign=""top"" class=""es-p40r"">
                                                      <a target=""_blank"" href="""">
                                                        <img title=""Facebook"" src=""https://eroqzme.stripocdn.email/content/assets/img/social-icons/logo-black/facebook-logo-black.png"" alt=""Fb"" width=""32"">
                                                      </a>
                                                    </td>
                                                    <td align=""center"" valign=""top"" class=""es-p40r"">
                                                      <a target=""_blank"" href="""">
                                                        <img title=""X"" src=""https://eroqzme.stripocdn.email/content/assets/img/social-icons/logo-black/x-logo-black.png"" alt=""X"" width=""32"">
                                                      </a>
                                                    </td>
                                                    <td align=""center"" valign=""top"" class=""es-p40r"">
                                                      <a target=""_blank"" href="""">
                                                        <img title=""Instagram"" src=""https://eroqzme.stripocdn.email/content/assets/img/social-icons/logo-black/instagram-logo-black.png"" alt=""Inst"" width=""32"">
                                                      </a>
                                                    </td>
                                                    <td align=""center"" valign=""top"">
                                                      <a target=""_blank"" href="""">
                                                        <img title=""Youtube"" src=""https://eroqzme.stripocdn.email/content/assets/img/social-icons/logo-black/youtube-logo-black.png"" alt=""Yt"" width=""32"">
                                                      </a>
                                                    </td>
                                                  </tr>
                                                </tbody>
                                              </table>
                                            </td>
                                          </tr>
                                          <tr>
                                            <td align=""center"" class=""esd-block-text es-p35b"">
                                              <p>
                                                Style Casual&nbsp;© 2021 Style Casual, Inc. All Rights Reserved.
                                              </p>
                                              <p>
                                                4562 Hazy Panda Limits, Chair Crossing, Kentucky, US, 607898
                                              </p>
                                            </td>
                                          </tr>
                                          <tr>
                                            <td esd-tmp-menu-padding=""5|5"" esd-tmp-divider=""1|solid|#cccccc"" class=""esd-block-menu"">
                                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""es-menu"">
                                                <tbody>
                                                  <tr>
                                                    <td align=""center"" valign=""top"" width=""33.33%"" class=""es-p10t es-p10b es-p5r es-p5l"" style=""padding-top:5px;padding-bottom:5px"">
                                                      <a target=""_blank"" href=""https://"">
                                                        Visit Us
                                                      </a>
                                                    </td>
                                                    <td align=""center"" valign=""top"" width=""33.33%"" class=""es-p10t es-p10b es-p5r es-p5l"" style=""padding-top:5px;padding-bottom:5px;border-left:1px solid #cccccc"">
                                                      <a target=""_blank"" href=""https://"">
                                                        Privacy Policy
                                                      </a>
                                                    </td>
                                                    <td align=""center"" valign=""top"" width=""33.33%"" class=""es-p10t es-p10b es-p5r es-p5l"" style=""padding-top:5px;padding-bottom:5px;border-left:1px solid #cccccc"">
                                                      <a target=""_blank"" href=""https://"">
                                                        Terms of Use
                                                      </a>
                                                    </td>
                                                  </tr>
                                                </tbody>
                                              </table>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </td>
                  </tr>
                </tbody>
              </table>
              <table cellpadding=""0"" cellspacing=""0"" align=""center"" class=""es-content esd-footer-popover"">
                <tbody>
                  <tr>
                    <td align=""center"" class=""esd-stripe"">
                      <table align=""center"" cellpadding=""0"" cellspacing=""0"" width=""600"" bgcolor=""rgba(0, 0, 0, 0)"" class=""es-content-body"" style=""background-color:transparent"">
                        <tbody>
                          <tr>
                            <td align=""left"" class=""esd-structure es-p20"">
                              <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tbody>
                                  <tr>
                                    <td width=""560"" align=""center"" valign=""top"" class=""esd-container-frame"">
                                      <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                        <tbody>
                                          <tr>
                                            <td align=""center"" class=""esd-block-text es-infoblock"">
                                              <p>
                                                <a target=""_blank""></a>No longer want to receive these emails?&nbsp;<a href="""" target=""_blank"">Unsubscribe</a>.<a target=""_blank""></a>
                                              </p>
                                            </td>
                                          </tr>
                                        </tbody>
                                      </table>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </td>
                  </tr>
                </tbody>
              </table>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </body>
</html>";
    }



  }
}
