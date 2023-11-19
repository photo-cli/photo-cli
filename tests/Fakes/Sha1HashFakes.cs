namespace PhotoCli.Tests.Fakes;

public static class Sha1HashFakes
{
	public static string Sample(int sampleId)
	{
		return sampleId switch
		{
			1 => "bf8b4530d8d246dd74ac53a13471bba17941dff7",
			2 => "c4ea21bb365bbeeaf5f2c654883e56d11e43c44e",
			3 => "3907fc960f2873f40c8f35643dd444e0468be131",
			4 => "d470205a1d331a9d3765b3762b7c954bb8efc6ea",
			5 => "9f4e6d352ec172e1059571250655e376769080fe",
			6 => "620d23336a12ab54f9f0190fe93960a4dba2df59",
			_ => throw new NotImplementedException()
		};
	}

	public const string Kenya = "5842c73cfdc5f347551bb6016e00c71bb1393169";
	public const string ItalyFlorence = "03cb14d5c68beed97cbe73164de9771d537fcd96";
	public const string ItalyArezzo1 = "5d66eec547469a1817bda4abe35c801359b2bb55";
	public const string ItalyArezzo2 = "629b0b141634d6c0906e49af448bec8d755ba32c";
	public const string ItalyArezzo3 = "620d23336a12ab54f9f0190fe93960a4dba2df59";
	public const string ItalyArezzo4 = "3b0a3215b4f66d7ff4804dd223f192c21aee71bc";
	public const string ItalyArezzo5 = "d470205a1d331a9d3765b3762b7c954bb8efc6ea";
	public const string ItalyArezzo6 = "f670f2bb6c54898894b06b083185b05086bd4e6e";
	public const string ItalyArezzo7 = "6b89a245809031ecc47789cdeaa332545330fc39";
	public const string ItalyArezzo8 = "dd42edcde2433a7df4a3d67bf61944a20884da89";
	public const string ItalyArezzo9 = "a0ab699f5f99fce8ff49163e87c7590c2c9a66eb";
	public const string UnitedKingdom = "bb649a18b3e7bb3df3701587a13f833749091817";
	public const string Spain1 = "3907fc960f2873f40c8f35643dd444e0468be131";
	public const string Spain2 = "9f4e6d352ec172e1059571250655e376769080fe";
	public const string NoGpsCoordinate = "90d835861e1aa3c829e3ab28a7f01ec3a090f664";
	public const string NoPhotoTakenDate = "cf756397cc3ca81b2650c8801fd64e172504015a";
}
