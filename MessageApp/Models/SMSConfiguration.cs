using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using MessageApp.App_Code;

namespace MessageApp.Models
{
    public class SMSConfiguration
    {
        [Key]
        public int Id { get; set; }
        public string AccountSID { get; set; }
        public string AuthToken { get; set; }
        public string FromNumber { get; set; }


        public SMSConfiguration Configuration()
        {
            SMSConfiguration myConfig = new SMSConfiguration();

            try
            {
                using (var db = new MessageAppDBContext())
                {
                    var config = db.SMSConfiguration.Find(1);

                    myConfig.AccountSID = Cipher.Decrypt(config.AccountSID);
                    myConfig.AuthToken = Cipher.Decrypt(config.AuthToken);
                    myConfig.FromNumber = config.FromNumber;
                }
                
            }
            catch (Exception)
            {
                throw;
            }

            return myConfig;
        }
    }

}
