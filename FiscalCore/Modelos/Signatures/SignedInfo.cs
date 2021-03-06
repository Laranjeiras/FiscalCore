namespace FiscalCore.Modelos.Signatures
{
    public class SignedInfo
    {
        public CanonicalizationMethod CanonicalizationMethod { get; set; }

        public SignatureMethod SignatureMethod { get; set; }

        public Reference Reference { get; set; }
    }
}
