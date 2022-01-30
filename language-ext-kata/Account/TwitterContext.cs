using System;

namespace language_ext.kata.Account
{
    public record TwitterContext
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string TweetUrl { get; set; }
        public string UserAccount { get; set; }
        public string TwitterToken { get; set; }
    }
}