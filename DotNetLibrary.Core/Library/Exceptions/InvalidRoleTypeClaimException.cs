using System.Security.Claims;

namespace DotNetLibrary.Exceptions
{
	public class InvalidRoleTypeClaimException : NullClaimException
	{
		public InvalidRoleTypeClaimException(Claim claim)
			: base(claim.Type, claim.Value)
		{
		}
	}
}
