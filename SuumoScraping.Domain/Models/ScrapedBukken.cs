namespace SuumoScraping.Domain.Models
{
    using System;
    using System.Collections.Generic;

    public record ScrapedBukkenSummary(string Title, string DetailUrl);

    public record ScrapedBukkenDetail(
        string Title,
        string Address,
        string Layout,
        string FloorAreaRaw,
        decimal FloorAreaSqm,
        decimal? FloorTubo,
        string FloorAreaMeasuringMethod,
        string PriceRaw,
        decimal PriceMin,
        decimal? PriceMax,
        IReadOnlyList<string> Accesses,
        string Balcony,
        string BuiltYears,
        string ManagementFee,
        string RepairingDeposit,
        string RepairingFund,
        string Floor,
        string Direction,
        string UseDistrict,
        string Structure,
        string RightsStyle,
        string MoveInTime,
        string Restriction,
        ScrapedCompany Company,
        IReadOnlyList<ScrapedImage> Images
    );

    public record ScrapedCompany(
        string Name,
        string Address,
        string TakkenLicense,
        string TransactionAspect
    );

    public record ScrapedImage(string Url, string Alt);
}
