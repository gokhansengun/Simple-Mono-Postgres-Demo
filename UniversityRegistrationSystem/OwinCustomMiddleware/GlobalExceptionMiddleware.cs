using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace UniversityRegistrationSystem.OwinCustomMiddleware
{
	public class GlobalExceptionMiddleware : OwinMiddleware
	{
		public GlobalExceptionMiddleware(OwinMiddleware next) : base(next)
		{ }

		public override async Task Invoke(IOwinContext context)
		{
			try
			{
				await Next.Invoke(context);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception occurred: " + ex.ToString());

				// rethrow the exception after logging
				throw;
			}
		}
	}
}

