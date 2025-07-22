using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drosy.Infrastructure.Helper.Email
{
    public static class EmailTemplates {

        public static string GetEmailConfirmEmailBody(string callbackUrl, string userName, int expiration)
        {
            return $@"            
        <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>اعادة تعيين كلمة المرور</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        text-align: center;
                        padding: 20px;
                    }}
                    .container {{
                        background: white;
                        padding: 20px;
                        border-radius: 8px;
                        max-width: 500px;
                        margin: auto;
                        box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
                    }}
                    .button {{
                        display: inline-block;
                        padding: 12px 20px;
                        font-size: 16px;
                        color: white;
                        background: linear-gradient(135deg, rgb(38, 175, 217) 0%, rgb(72, 220, 198) 100%);
                        text-decoration: none;
                        border-radius: 5px;
                        margin-top: 20px;
                    }}
                    .button:hover {{
                        background-color: #0056b3;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <p>مرحبًا {userName}،</p>
                    <p>لقد تلقينا طلبًا لإعادة تعيين كلمة المرور الخاصة بحسابك. إذا كنت أنت من قام بهذا الطلب، فيرجى الضغط على الزر أو الرابط أدناه لإعادة تعيين كلمة المرور:</p>
                    <a href='{callbackUrl}' class='button'>رابط إعادة تعيين كلمة المرور</a>
                    <p>يرجى ملاحظة أن هذا الرابط صالح لمدة دقيقة {expiration} فقط، وبعد ذلك سيتعين عليك طلب رابط جديد.
                        إذا لم تقم بطلب إعادة تعيين كلمة المرور، يمكنك تجاهل هذه الرسالة. لن يتم إجراء أي تغيير على حسابك.</p>
                    <p>
                        شكرًا لك،
                        فريق الدعم - ارام
                    </p>
                </div>
            </body>
            </html>";
        }
    }
}
