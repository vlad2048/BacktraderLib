namespace Feed.Symbology;

public enum MicStatus
{
	Active,
	Expired,
	Updated,
}

public sealed record Mic(
	string Name,
	string OperatingMic,
	bool IsRoot,
	string MarketNameInstitutionDescr,
	string LegalEntityName,
	string Lei,
	string MarketCategoryCode,
	string Acronym,
	string IsoCountry,
	string City,
	string Website,
	MicStatus Status,
	DateOnly CreationDate,
	DateOnly LastUpdateDate,
	DateOnly? LastValidationDate,
	DateOnly? ExpiryDate,
	string Comments,
	int Utils_Level,
	string Utils_FullName
);