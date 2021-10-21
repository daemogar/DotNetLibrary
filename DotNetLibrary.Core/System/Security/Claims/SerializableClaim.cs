namespace System.Security.Claims;

/// <summary>
/// Create a simple claim with an issuer.
/// </summary>
/// <param name="Type">This is the key of the key/value claim.</param>
/// <param name="Value">The value of the claim.</param>
/// <param name="Issuer">The issuer of the claim.</param>
public record struct SerializableClaim(
	string Type, string Value, string Issuer)
{
	/// <summary>
	/// Create a simple claim without an issuer.
	/// </summary>
	/// <param name="type">This is the key of the key/value claim.</param>
	/// <param name="value">The value of the claim.</param>
	public SerializableClaim(string type, string value)
		: this(type, value, null!) { }

	/// <summary>
	/// Create a simple claim from <seealso cref="Claim"/>.
	/// </summary>
	/// <param name="claim">A <seealso cref="Claim"/> to be simplified.</param>
	public SerializableClaim(Claim claim)
		: this(claim.Type, claim.Value, claim.Issuer) { }

	/// <summary>
	/// Convert simple claim into a <seealso cref="Claim"/>.
	/// </summary>
	/// <returns><seealso cref="Claim"/> of simple type.</returns>
	public Claim AsClaim() => new(Type, Value, Issuer);
}
