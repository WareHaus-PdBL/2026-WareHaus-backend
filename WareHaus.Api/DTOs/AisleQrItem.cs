namespace WareHaus.Api.DTOs
{
    public class AisleQrItem
    {
        public string ShelfCode { get; set; }
        public string QrFilePath { get; set; }

        public AisleQrItem(string shelfCode, string qrFilePath)
        {
            ShelfCode = shelfCode;
            QrFilePath = qrFilePath;
        }
    }
}
