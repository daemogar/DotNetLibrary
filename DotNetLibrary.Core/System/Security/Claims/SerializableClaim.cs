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

	/// <summary>
	/// Checks to see if one <seealso cref="SerializableClaim" /> has
	/// the same <see cref="Type" /> and <see cref="Value" /> as another 
	/// <seealso cref="SerializableClaim" />.
	/// </summary>
	/// <param name="obj">The other <seealso cref="SerializableClaim" /> to compare with.</param>
	/// <returns>Returns true if the <see cref="Type" /> and <see cref="Value" /> from both <seealso cref="SerializableClaim" /> objects are the the same.</returns>
	public override bool Equals(object? obj)
		=> obj is SerializableClaim claim && claim.Type is not null
			&& Type.Equals(claim.Type) && (
				(Value is null && claim.Value is null)
				|| Value?.Equals(claim.Value) == true);
}
