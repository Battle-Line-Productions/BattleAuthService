﻿namespace BattleAuth.Contracts.Contracts.V1.Requests
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }

        public string Code { get; set; }

        public string Password { get; set; }

        public string PasswordConfirmation { get; set; }
    }
}