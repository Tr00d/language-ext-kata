using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace language_ext.kata.Account
{
    public class AccountService
    {
        private readonly IBusinessLogger businessLogger;
        private readonly TwitterService twitterService;
        private readonly UserService userService;

        public AccountService(UserService userService, TwitterService twitterService, IBusinessLogger businessLogger)
        {
            this.userService = userService;
            this.twitterService = twitterService;
            this.businessLogger = businessLogger;
        }

        private Try<TwitterContext> FindUser(TwitterContext context) =>
            Try(() => this.userService.FindById(context.UserId))
                .Map(user => context with {UserEmail = user.Email, UserName = user.Name});

        private Try<TwitterContext> RegisterOnTwitter(TwitterContext context) =>
            Try(() => this.twitterService.Register(context.UserEmail, context.UserName))
                .Map(accountId => context with {UserAccount = accountId});

        private Try<TwitterContext> AuthenticateOnTwitter(TwitterContext context) =>
            Try(() => this.twitterService.Authenticate(context.UserEmail, context.UserName))
                .Map(token => context with {TwitterToken = token});

        private Try<TwitterContext> TweetOnTwitter(TwitterContext context) =>
            Try(() => this.twitterService.Tweet(context.TwitterToken, $"Hello I am {context.UserName}"))
                .Map(tweet => context with {TweetUrl = tweet});

        private static TwitterContext InitializeContext(Guid id) => new() {UserId = id};

        public string Register(Guid id) =>
            Try(InitializeContext(id))
                .Bind(this.FindUser)
                .Bind(this.RegisterOnTwitter)
                .Bind(this.AuthenticateOnTwitter)
                .Bind(this.TweetOnTwitter)
                .Do(context => this.userService.UpdateTwitterAccountId(context.UserId, context.UserAccount))
                .Do(context => this.businessLogger.LogSuccessRegister(context.UserId))
                .Map(context => context.TweetUrl)
                .IfFail(exception =>
                {
                    this.businessLogger.LogFailureRegister(id, exception);
                    return null;
                });
    }
}