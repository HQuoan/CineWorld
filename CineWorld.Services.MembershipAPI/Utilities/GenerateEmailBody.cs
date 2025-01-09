using CineWorld.Services.MembershipAPI.Models;

namespace CineWorld.Services.MembershipAPI.Utilities
{
  public static class GenerateEmailBody
  {
    public static string PaymentSuccess(Receipt receipt, Package package, MemberShip memberShip)
    {
      var couponInfo = string.Empty;
      if (!string.IsNullOrEmpty(receipt.CouponCode))
      {
        couponInfo = $@"
        <tr>
            <th>Discount</th>
            <td>{receipt.DiscountAmount} {package.Currency}</td>
        </tr>";
      }

      return $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            background: #ffffff;
            border-radius: 10px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            padding: 20px;
        }}
        .header {{
            text-align: center;
            background-color: #007BFF;
            color: white;
            padding: 10px 0;
            border-radius: 10px 10px 0 0;
        }}
        .content {{
            padding: 20px;
            line-height: 1.6;
        }}
        .footer {{
            text-align: center;
            margin-top: 20px;
            font-size: 0.9em;
            color: #666666;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
        }}
        th, td {{
            padding: 10px;
            border: 1px solid #dddddd;
            text-align: left;
        }}
        .highlight {{
            font-weight: bold;
            color: #007BFF;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>CineWorld - Payment Successful.</h2>
        </div>
        <div class='content'>
            <p>Dear Customer,</p>
            <p>Thank you for your purchase! Your payment has been successfully processed. Below are the details of your receipt:</p>
            <table>
                <tr>
                    <th>Receipt ID</th>
                    <td>{receipt.ReceiptId}</td>
                </tr>
                <tr>
                    <th>Package</th>
                    <td class='highlight'>{package.Name} ({package.Description})</td>
                </tr>
                 <tr>
                    <th>Term In Months</th>
                    <td class='highlight'>{package.TermInMonths}</td>
                </tr>
                <tr>
                    <th>Price</th>
                    <td>{package.Price} {package.Currency}</td>
                </tr>
                {couponInfo}
                <tr>
                    <th>Total Amount</th>
                    <td class='highlight'>{receipt.TotalAmount} {package.Currency}</td>
                </tr>
                <tr>
                    <th>Payment Method</th>
                    <td>{receipt.PaymentMethod}</td>
                </tr>
                <tr>
                    <th>Date</th>
                    <td>{receipt.CreatedDate:yyyy-MM-dd HH:mm:ss}</td>
                </tr>
                <tr>
                    <th>Expiration Date</th>
                    <td>{memberShip.ExpirationDate:yyyy-MM-dd HH:mm:ss}</td>
                </tr>
            </table>
            <p>If you have any questions, feel free to contact our support team at <a href='huyvodtan@gmail.com'>support@cineworld.com</a>.</p>
            <p>Enjoy your membership!</p>
            <p>Best regards,<br>CineWorld Team</p>
        </div>
        <div class='footer'>
            &copy; {DateTime.UtcNow.Year} CineWorld. All rights reserved.
        </div>
    </div>
</body>
</html>";
    }

  }
}
