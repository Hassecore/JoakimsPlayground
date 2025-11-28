namespace AuthServer.Common
{
	public static class Constants
	{
		public const string CODE_CHALLENGE_METHOD = "S256";
		public const string GRANT_TYPE_AUTHORIZATION_CODE = "authorization_code";
		public const string AUTHORIZATION_CODE_PREFIX = "auth_code_";
		public const string CODE_CHALLENGE_CLIENT_ID_PREFIX = "code_challenge_for_client_id_";

		public static class AuthKeys
		{
			public const string CodeChallengeKey = "code_challenge";

			public const string OriginalReferrerKey = "original_referrer";
			
			public const string RedirectUrl = "redirect_url";
		}
	}
}
