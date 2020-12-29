using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageApp.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string To { get; set; }
        public string MessageText { get; set; }
    }

    public class MessageResult
    {
        [Key]
        public int Id { get; set; }
        public int MessageId { get; set; }
        public DateTime Sent { get; set; }
        public string ConfirmationCode { get; set; }

        [ForeignKey("MessageId")]
        public Message Messages { get; set; }

    }


    public class MessageHistory
    {
        [Key]
        public int Id { get; set; }
        public DateTime Sent { get; set; }
        public string To { get; set; }
        public string MessageText { get; set; }
        public string ConfirmationCode { get; set; }
    }



}
