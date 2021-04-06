namespace BattleAuth.Contracts.Options
{
    using System;

    public class JwtSettings
    {
        public string Secret { get; set; }

        public TimeSpan TokenLifetime { get; set; }

        public string Private { get; set; }

        public string Public { get; set; }

        public string Audience { get; set; }
    }
}
