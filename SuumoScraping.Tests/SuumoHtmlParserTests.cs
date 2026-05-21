namespace SuumoScraping.Tests
{
    using System.Linq;
    using Microsoft.Extensions.Logging.Abstractions;
    using SuumoScraping.Domain.Exceptions;
    using SuumoScraping.Infrastructure.Scraping;
    using Xunit;

    public class SuumoHtmlParserTests
    {
        [Fact]
        public void ParseAreaPage_WithValidHtml_ReturnsBukkensAndNextPage()
        {
            // Arrange
            var url = "https://suumo.jp/ms/chuko/saitama/sc_toda/";
            var html = """
                <div class="property_unit-content">
                    <div>
                        <h2><a href="/ms/chuko/saitama/sc_toda/nc_12345/">トダマンション 3F</a></h2>
                    </div>
                </div>
                <div class="property_unit-content">
                    <div>
                        <h2><a href="/ms/chuko/saitama/sc_toda/nc_67890/">トダレジデンス 5F</a></h2>
                    </div>
                </div>
                <p class="pagination-parts">
                    <a href="/ms/chuko/saitama/sc_toda/?pn=2">次へ</a>
                </p>
                """;

            var parser = new SuumoHtmlParser(NullLogger<SuumoHtmlParser>.Instance);

            // Act
            var result = parser.ParseAreaPage(url, html);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Bukkens.Count);
            Assert.Equal("トダマンション 3F", result.Bukkens[0].Title);
            Assert.Equal("/ms/chuko/saitama/sc_toda/nc_12345/", result.Bukkens[0].DetailUrl);
            Assert.Equal("トダレジデンス 5F", result.Bukkens[1].Title);
            Assert.Equal("/ms/chuko/saitama/sc_toda/nc_67890/", result.Bukkens[1].DetailUrl);
            Assert.Equal("https://suumo.jp/ms/chuko/saitama/sc_toda/?pn=2", result.NextPageUrl);
        }

        [Fact]
        public void ParseBukkenDetail_WithValidHtml_ReturnsParsedDetails()
        {
            // Arrange
            var url = "https://suumo.jp/ms/chuko/saitama/sc_toda/nc_12345/";
            var gaiyoHtml = """
                <table summary="表" class="tbl_gaiyo">
                    <tbody>
                        <tr>
                            <th><div>価格</div></th>
                            <td>5500万円～6200万円</td>
                        </tr>
                        <tr>
                            <th><div>専有面積</div></th>
                            <td>72.5m2（21.9坪）（壁芯）</td>
                        </tr>
                        <tr>
                            <th><div>交通</div></th>
                            <td>
                                ＪＲ京浜東北線「浦和」駅 徒歩8分<br>
                                ＪＲ埼京線「武蔵浦和」駅 徒歩15分<br>
                                
                            </td>
                        </tr>
                        <tr>
                            <th><div>所在地</div></th>
                            <td>埼玉県さいたま市浦和区仲町１</td>
                        </tr>
                        <tr>
                            <th><div>間取り</div></th>
                            <td>3LDK</td>
                        </tr>
                        <tr>
                            <th>会社概要</th>
                            <td>
                                <div>
                                    <p>
                                        売主<br>
                                        国土交通大臣（2）第9999号<br>
                                        テスト不動産株式会社<br>
                                        東京都千代田区神田駿河台１丁目
                                    </p>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
                """;

            var tokuchoHtml = """
                <table summary="表">
                    <tbody>
                        <tr>
                            <td>テストマンション浦和仲町</td>
                        </tr>
                    </tbody>
                </table>
                <div id="mainContents">
                    <a class="jscNyroModal nyroModal" href="#">
                        <img src="https://img.suumo.jp/xyz.jpg?w=100&h=100&amp;x=1" rel="https://img.suumo.jp/xyz.jpg?w=100&h=100&amp;x=1" alt="外観" />
                    </a>
                </div>
                """;

            var parser = new SuumoHtmlParser(NullLogger<SuumoHtmlParser>.Instance);

            // Act
            var result = parser.ParseBukkenDetail(url, gaiyoHtml, tokuchoHtml);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("テストマンション浦和仲町", result.Title);
            Assert.Equal("埼玉県さいたま市浦和区仲町１", result.Address);
            Assert.Equal("3LDK", result.Layout);
            Assert.Equal("72.5m2（21.9坪）（壁芯）", result.FloorAreaRaw);
            Assert.Equal(72.5m, result.FloorAreaSqm);
            Assert.Equal(21.9m, result.FloorTubo);
            Assert.Equal("壁芯", result.FloorAreaMeasuringMethod);
            
            Assert.Equal("5500万円～6200万円", result.PriceRaw);
            Assert.Equal(55000000m, result.PriceMin);
            Assert.Equal(62000000m, result.PriceMax);

            Assert.Equal(2, result.Accesses.Count);
            Assert.Equal("ＪＲ京浜東北線「浦和」駅 徒歩8分", result.Accesses[0]);
            Assert.Equal("ＪＲ埼京線「武蔵浦和」駅 徒歩15分", result.Accesses[1]);

            Assert.Equal("テスト不動産株式会社", result.Company.Name);
            Assert.Equal("東京都千代田区神田駿河台１丁目", result.Company.Address);
            Assert.Equal("国土交通大臣（2）第9999号", result.Company.TakkenLicense);
            Assert.Equal("売主", result.Company.TransactionAspect);

            Assert.Single(result.Images);
            Assert.Equal("https://img.suumo.jp/xyz.jpg?w=452&h=339&x=1", result.Images[0].Url);
            Assert.Equal("外観", result.Images[0].Alt);
        }

        [Fact]
        public void ParseBukkenDetail_WithInvalidHtml_ThrowsSuumoParseException()
        {
            // Arrange
            var url = "https://suumo.jp/ms/chuko/saitama/sc_toda/nc_invalid/";
            var invalidGaiyoHtml = "<html><body><div>物件概要テーブルがありません。</div></body></html>";
            var tokuchoHtml = "";

            var parser = new SuumoHtmlParser(NullLogger<SuumoHtmlParser>.Instance);

            // Act & Assert
            var exception = Assert.Throws<SuumoParseException>(() => parser.ParseBukkenDetail(url, invalidGaiyoHtml, tokuchoHtml));
            Assert.Equal(url, exception.Url);
            Assert.Equal("bukkengaiyo_table", exception.ElementName);
            Assert.Contains("物件概要テーブルのノード取得に失敗しました", exception.Message);
        }
    }
}
