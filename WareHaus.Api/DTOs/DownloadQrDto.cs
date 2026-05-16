namespace WareHaus.Api.DTOs;

public record DownloadQrRequestDto(
    string Option
);

public record DownloadQrFileDto(
    byte[] Bytes,
    string ContentType,
    string FileName
);