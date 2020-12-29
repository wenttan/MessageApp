using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using MessageApp.Models;
using Twilio.Exceptions;

namespace MessageApp.App_Code
{
    class TwilioAPI
    {
        public MessageResult SendSMS(Message message)
        {
            MessageResult result = new MessageResult();

            try
            {
                // Getting account config from db
                SMSConfiguration objConfig = new SMSConfiguration();
                var config = objConfig.Configuration();

                string accountSid = config.AccountSID;
                string authToken = config.AuthToken;
                string from = config.FromNumber;

                TwilioClient.Init(accountSid, authToken);

                try
                {
                    result.MessageId = message.Id;

                    var response = MessageResource.Create(
                        body: message.MessageText,
                        from: new Twilio.Types.PhoneNumber(from),
                        to: new Twilio.Types.PhoneNumber(message.To)
                    );

                    result.Sent = response.DateSent.Value;

                    if (response.Status.ToString() != "sent")
                    {
                        result.ConfirmationCode = "Error-" + response.ErrorCode.ToString() + "-" + response.ErrorMessage;
                    }
                    else
                    {
                        result.ConfirmationCode = response.Status.ToString();
                    }

                }
                catch (ApiException e)
                {
                    result.Sent = DateTime.Now;
                    result.ConfirmationCode = "Error-" + e.Code.ToString() + "-" + e.Message;
                }
                
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }
    }
}
