using System;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WCMS.Web
{
	public class UserManager : UserManager<IdentityUser>
	{
		private static readonly UserStore<IdentityUser> USER_STORE = new UserStore<IdentityUser>();
		private static readonly UserManager INSTANCE = new UserManager();

		private UserManager() : base(USER_STORE)
		{
		}

        /*
		public static UserManager Create()
		{
			// We have to create our own user manager in order to override some default behavior:
			//
			//	- Override default password length requirement (6) with a length of 4
			//	- Override user name requirements to allow spaces and dots
			var passwordValidator = new MinimumLengthValidator(4);
			var userValidator = new UserValidator<IdentityUser, String>(INSTANCE)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true,
			};

			INSTANCE.UserValidator = userValidator;
			INSTANCE.PasswordValidator = passwordValidator;

			return INSTANCE;
		}
        */
		public static void Seed()
		{

		}
	}
}