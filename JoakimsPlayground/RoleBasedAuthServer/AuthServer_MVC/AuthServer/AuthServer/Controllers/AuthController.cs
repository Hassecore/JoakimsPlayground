using AuthServer.Common;
using AuthServer.Models;
using AuthServer.Services.Auth;
using AuthServer.Services.Caching;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthServer.Controllers
{
	/// <summary>
	/// 
	/// To do:
	/// - Clean up unused stuff
	/// - Add controller/method/view which requires auth
	/// - Add auth
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// 
	/// </summary>
	//[Route("auth")]
	public class AuthController : Controller
	{
		private readonly ILogger<AuthController> _logger;

		private readonly IAuthService _authService;

		private readonly ICachingService _cachingService;

		private readonly IConfiguration _configuration;

		public AuthController(ILogger<AuthController> logger, IAuthService authService, ICachingService cachingService, IConfiguration configuration)
		{
			_logger = logger;
			_authService = authService;
			_cachingService = cachingService;
			_configuration = configuration;
		}

		public IActionResult Index()
		{
			var code_challenge = Request.Query[Constants.AuthKeys.CodeChallengeKey].SingleOrDefault();
			var redirect_url = Request.Query[Constants.AuthKeys.RedirectUrl].SingleOrDefault();
			//var redirect_url = 
			if (!string.IsNullOrEmpty(code_challenge) &&
				!string.IsNullOrEmpty(redirect_url))
			{
				// Handle code_challenge
				var expiryTimeInMinutes = _configuration.GetValue<int>("Cache:CodeChallengeExpiryTimeInMinutes");
				_cachingService.Set(code_challenge, redirect_url, new TimeSpan(0, expiryTimeInMinutes, 0));
			}

			return View();
		}

		public IActionResult UserLogin()
		{
			return View();
		}

		public IActionResult SignUp()
		{
			return View();
		}

		public IActionResult GoogleLogin()
		{
			var test1= Request.Query[Constants.AuthKeys.RedirectUrl];
			var code_challenge= Request.Query[Constants.AuthKeys.CodeChallengeKey];
			var test = HttpContext.Request;

			//var redirectUrl = $"{test.Scheme}://{test.Host}/signin-google"; // Your client app URL
			var callbackUrl = $"{test.Scheme}://{test.Host}/complete-google"; // Your client app URL
			var properties = new AuthenticationProperties { RedirectUri = callbackUrl, Items = { { Constants.AuthKeys.CodeChallengeKey, code_challenge } } };
			return Challenge(properties, GoogleDefaults.AuthenticationScheme);
		}

		[HttpGet]
		[Route("complete-google")]
		public async Task<ActionResult> GoogleResponse()
		{
			var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
			if (!result.Succeeded)
				return BadRequest(); // TODO: Handle this better.

			var codeChallenge = result.Properties.Items[Constants.AuthKeys.CodeChallengeKey];
			var redirectUrl = _cachingService.Get<string>(codeChallenge);
			if (redirectUrl == null)
			{
				return BadRequest("Invalid or expired code challenge.");
			}

			var registerDto = new RegisterDto
			{
				Email = result.Principal.FindFirstValue(ClaimTypes.Email),
				UserName = result.Principal.FindFirstValue(ClaimTypes.Email),
			};

			var (status, message, authCode) = await _authService.HandleGoogleUserAsync(registerDto, UserRoles.User.ToString());

			if (status == 0)
			{
				TempData["LoginFailedMessage"] = $"Login failed. {message}";

				return RedirectToAction("UserLogin", new { redirect_url = redirectUrl, code_challenge = codeChallenge });
			}
			//var email = result.Principal.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
			// You can also retrieve other user info like name, profile picture, etc.
			//return Redirect(redirectUrl);
			return Redirect(redirectUrl + $"?code={authCode}");
		}

		[HttpGet]
		[Route("signin-google1")]
		public async Task<ActionResult> GoogleResponse1()
		{
			return Redirect("https://chatgpt.com/");
		}

		public async Task<IActionResult> RegisterForm(RegisterDto registerDto)
		{
			// Add validations here.

			var (status, message, authCode) = await _authService.RegisterAsync(registerDto, UserRoles.User.ToString());

			var redirectUrl = Request.Form["redirect_url"];
			if (!string.IsNullOrEmpty(redirectUrl))
			{
				return Redirect(redirectUrl + $"?code={authCode}");
			}

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> LoginForm(LoginDto model, string? redirectUrl = null, string? codeChallenge = null)
		{
			var test = Request.QueryString;

			// Add validations here.

			var (status, message) = await _authService.AuthorizeAsync(model);

			if (status == 0)
			{
				TempData["LoginFailedMessage"] = $"Login failed. {message}";

				// Preserve redirect_url in case of login failure
				if (!string.IsNullOrEmpty(redirectUrl) &&
					!string.IsNullOrEmpty(codeChallenge))
				{
					return RedirectToAction("UserLogin", new { redirect_url = redirectUrl, code_challenge = codeChallenge });
				}

				return RedirectToAction("UserLogin");
			}

			// Successful login - redirect to the specified URL or default to Index
			if (!string.IsNullOrEmpty(redirectUrl))// && Url.IsLocalUrl(redirectUrl))
			{
				return Redirect(redirectUrl + $"?code={message}");
			}

			return RedirectToAction("Index");
		}

		//[HttpPost]
		//[Route("login")]
		//public async Task<IActionResult> Login(LoginDto model)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		ModelState.AddModelError(string.Empty, "Incorrect log in model.");
		//		return View(model);
		//	}

		//	var (status, message) = await _authService.LoginAsync(model);

		//	if (status == 0)
		//	{
		//		ModelState.AddModelError(string.Empty, message);
		//		return View(model);
		//	}

		//	return RedirectToAction("Index", "Auth");
		//}

		//[Route("register")]
		//public IActionResult Register(RegisterDto model)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		ModelState.AddModelError(string.Empty, "Incorrect log in model.");
		//		return View(model);
		//	}

		//	var (status, message) =  _authService.RegisterAsync(model, UserRoles.Admin.ToString()).Result;

		//	if (status == 0)
		//	{
		//		ModelState.AddModelError(string.Empty, message);
		//		return View(model);
		//	}

		//	return RedirectToAction("Index", "Auth");
		//}

		[HttpPost]
		[Route("register")]
		public async Task<ActionResult<string>> RegisterAsync([FromBody] RegisterDto model)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest("Invalid payload");

				var (status, message, authCode) = await _authService.RegisterAsync(model, UserRoles.Admin.ToString());
				if (status == 0)
					return BadRequest(message);


				var (tokenStatus, tokenMessage) = await _authService.Token(authCode); // If regged through endpoint, just give JWT right away. For debug/dev - does not follow OAuth 2.0 flow strictly.
				if (tokenStatus == 0)
					return BadRequest(tokenMessage);

				return Ok(tokenMessage);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		[HttpPost]
		[Route("connect/authorize")]
		public async Task<ActionResult<string>> AuthorizeAsync([FromBody] LoginDto model)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest("Invalid payload.");

				var (status, message) = await _authService.AuthorizeAsync(model);
				if (status == 0)
					return BadRequest(message);

				var (status2, message2) = await _authService.Token(message); // If authed through endpoint, just give JWT right away. For debug/dev - does not follow OAuth 2.0 flow strictly.
				if (status2 == 0)
					return BadRequest(message2);

				return Ok(message2);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		[HttpPost]
		[Route("connect/token")]
		public async Task<ActionResult<string>> Token([FromBody] TokenEndpointDto request)
		{
			if (_configuration.GetValue<bool>("PKCE_Enabled"))
			{
				var codeChallengeResult = _authService.ValidateCodeVerifier(request.CodeVerifier);
				if (!codeChallengeResult)
				{
					return BadRequest("Invalid code verifier.");
				}
			}

			if (string.IsNullOrEmpty(request?.AuthorizationCode))
			{
				return BadRequest("Authorization code is required.");
			}

			var (status, message) = await _authService.Token(request.AuthorizationCode);
			if (status == 0)
			{
				return BadRequest(message);
			}

			return Ok(message);
		}


	}
}