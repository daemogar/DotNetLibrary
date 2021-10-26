namespace System.Security.Claims;

/// <summary>
/// Create a simple claim that is useful for serializing and deserializing.
/// </summary>
public class SerializableClaim
{
	/// <summary>
	/// This is the key of the key/value claim.
	/// </summary>
	public string Type { get; init; }

	/// <summary>
	/// The value of the claim.
	/// </summary>
	public string Value { get; init; }

	/// <summary>
	/// The issuer of the claim.
	/// </summary>
	public string Issuer { get; init; }

	/// <summary>
	/// Create a simple claim without any data. This constructor is intended
	/// for deserializing a json object.
	/// </summary>
	public SerializableClaim()
		: this(null!, null!, null!) { }

	/// <summary>
	/// Create a simple claim with an issuer.
	/// </summary>
	/// <param name="type">This is the key of the key/value claim.</param>
	/// <param name="value">The value of the claim.</param>
	/// <param name="issuer">The issuer of the claim.</param>
	public SerializableClaim(string type, string value, string issuer)
	{
		Type = type;
		Value = value;
		Issuer = issuer;
	}

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
