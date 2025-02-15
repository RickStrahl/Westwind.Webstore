using System.Linq;
using NUnit.Framework;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Test;
using Westwind.Webstore.Business.Utilities;

namespace Westwind.WebStore.Business.Test;


/// <summary>
/// These tests are meant to help populate the database with seed data
/// if it's not already there. Useful for testing and development.
///
/// </summary>
[TestFixture]
public class DbInitializationTests
{

    public BusinessFactory BusinessFactory { get; set; }


    [SetUp]
    public void Init()
    {
        // Load Factory and Provider
        BusinessFactory = TestHelpers.GetBusinessFactoryWithProvider();
    }

    /// <summary>
    /// ***  WARNING ***
    /// Warning this will update your database - only run against
    /// a test database!
    /// </summary>
    [Test, Explicit]
    public void SeedDatabaseDataTest()
    {
        // Create States and Countries
        var lookupBus = BusinessFactory.GetLookupBusiness();
        Assert.That(lookupBus.InsertInitialData());
    }

    /// <summary>
    /// WARNING - This will update your database with seed data
    /// Imports seed data for lookups and categories from JSON
    ///
    /// You can update the JSON below with your own data to
    /// force specific data into the Lookups and Category tables
    /// </summary>
    [Test, Explicit]
    public void ImportSeedCategoriesAndLookupsDataJsonTest()
    {
        Assert.That(ImportExportSeedData.Import(JsonText, true));

        var bus = BusinessFactory.Current.GetAdminBusiness();
        Assert.That(bus.Context.Lookups.Any());
        Assert.That(bus.Context.Categories.Any());
    }


    string JsonText = """
                      {
                          "categories": [
                              {
                                  "id": "0fxhxkhcqh",
                                  "parentId": null,
                                  "categoryName": "documentation",
                                  "description": "Documentation",
                                  "type": "",
                                  "keywords": ""
                              },
                              {
                                  "id": "x585q7m80m",
                                  "parentId": null,
                                  "categoryName": "services",
                                  "description": "Services",
                                  "type": "",
                                  "keywords": ""
                              }
                          ],
                          "lookups": [
                              {
                                  "id": "05zbjbu84w",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GH",
                                  "cData1": "Ghana",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "0oaoosa53h",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TK",
                                  "cData1": "Tokelau",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "0p2m4fsjbn",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AS",
                                  "cData1": "American Samoa",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "0sikhgscfm",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "LB",
                                  "cData1": "Lebanon",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "0x7r0ymcrg",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "JO",
                                  "cData1": "Jordan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "184d32vgcu",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MN",
                                  "cData1": "Mongolia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "1c897i5jiy",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SI",
                                  "cData1": "Slovenia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "1fbx10xh95",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MP",
                                  "cData1": "Northern Mariana Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "1mt4j2gha7",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MT",
                                  "cData1": "Malta",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "2mxvxrdb8o",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MM",
                                  "cData1": "Myanmar",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "2npmkgw7jj",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SN",
                                  "cData1": "Senegal",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "2yencxn8vc",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "DE",
                                  "cData1": "Germany",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "325m21b82o",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GB",
                                  "cData1": "United Kingdom",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "328pfn2fdr",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "NC",
                                  "cData1": "New Caledonia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "34jy438he2",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MO",
                                  "cData1": "Macau",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "36xbyeddse",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AR",
                                  "cData1": "Argentina",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "3g4pu2jehg",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "PN",
                                  "cData1": "Pitcairn",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "3p3g2rh525",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "VI",
                                  "cData1": "Virgin Islands (U.S.)",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "3zbgpeiitz",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "PS",
                                  "cData1": "Palestinian Territory",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "4c92udh88p",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CZ",
                                  "cData1": "Czech Republic",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "4g83vg1dy6",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "KN",
                                  "cData1": "Saint Kitts And Nevis",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "4m5861eavj",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "FK",
                                  "cData1": "Falkland Islands (Malvinas)",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "4vpejez7he",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "PL",
                                  "cData1": "Poland",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "4wbucqqhkx",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AT",
                                  "cData1": "Austria",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "4x1s2cwh3k",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CD",
                                  "cData1": "Congo",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "5133hca5kk",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GP",
                                  "cData1": "Guadeloupe",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "56kijcn9kj",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TN",
                                  "cData1": "Tunisia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "5aom5syhy6",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TV",
                                  "cData1": "Tuvalu",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "6efgevk6of",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MU",
                                  "cData1": "Mauritius",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "6i6wcsxf5x",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "KR",
                                  "cData1": "Korea",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "6njnoss5nf",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MK",
                                  "cData1": "Macedonia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "6no1qyy00c",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AF",
                                  "cData1": "Afghanistan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "704mk0t57y",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TZ",
                                  "cData1": "Tanzania",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "78b7iv6gfh",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AI",
                                  "cData1": "Anguilla",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "7dekpcaewm",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "WF",
                                  "cData1": "Wallis And Futuna Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "7f2wkstii0",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "LC",
                                  "cData1": "Saint Lucia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "7nnswumcdv",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "PH",
                                  "cData1": "Philippines",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "7qco94pf2y",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BD",
                                  "cData1": "Bangladesh",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "7qgbaihdfo",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CN",
                                  "cData1": "China",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "7qihafxbok",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BY",
                                  "cData1": "Belarus",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "7sp69un6bh",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "VE",
                                  "cData1": "Venezuela",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "806jqj0fik",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CM",
                                  "cData1": "Cameroon",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "81z8hktdo2",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AN",
                                  "cData1": "Netherlands Antilles",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "5e0801xb1r",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "YT",
                                  "cData1": "Mayotte",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "5qexxopgqu",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "YE",
                                  "cData1": "Yemen",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "5tq2fqh0cu",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MV",
                                  "cData1": "Maldives",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "5wedk6njc9",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CO",
                                  "cData1": "Colombia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "5yo7dtmeqz",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "NZ",
                                  "cData1": "New Zealand",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "62urtru0pu",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "UY",
                                  "cData1": "Uruguay",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "63an8ywcrr",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "RU",
                                  "cData1": "Russian Federation",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "697gw38e5t",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "PG",
                                  "cData1": "Papua New Guinea",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "69hrrv4cji",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MR",
                                  "cData1": "Mauritania",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "6ambbtgbdv",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SJ",
                                  "cData1": "Svalbard And Jan Mayen Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "6ch39mxhi3",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "UA",
                                  "cData1": "Ukraine",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "4nq5ghkfot",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SY",
                                  "cData1": "Syrian Arab Republic",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "4r35j2y6ab",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BW",
                                  "cData1": "Botswana",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "85tdsswf9b",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TL",
                                  "cData1": "Timor-Leste",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "86h1jd4ayw",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CR",
                                  "cData1": "Costa Rica",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "8ezmn6tjnr",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MS",
                                  "cData1": "Montserrat",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "8n8vd55aw8",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AZ",
                                  "cData1": "Azerbaijan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "8y7nybv5h9",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AQ",
                                  "cData1": "Antarctica",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "9265267gyc",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TD",
                                  "cData1": "Chad",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "94hvhm09kf",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "QA",
                                  "cData1": "Qatar",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "9f0dm6a08o",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CU",
                                  "cData1": "Cuba",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "9nv52f464x",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "VG",
                                  "cData1": "Virgin Islands (British)",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "9ri9pkn8hk",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BH",
                                  "cData1": "Bahrain",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "9vyqm3j7jp",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CI",
                                  "cData1": "Cote D'ivoire",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "9z6bjq99qw",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "UZ",
                                  "cData1": "Uzbekistan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "a6koih290b",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TH",
                                  "cData1": "Thailand",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "aafjczo802",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "NA",
                                  "cData1": "Namibia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ah0k6x5deh",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GQ",
                                  "cData1": "Equatorial Guinea",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ai1afx9j6t",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BI",
                                  "cData1": "Burundi",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ar1cmcxawx",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "US",
                                  "cData1": "United States",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "azqj5gn67s",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GF",
                                  "cData1": "French Guiana",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "b1o296ng22",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TF",
                                  "cData1": "French Southern Territories",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "b6k5n8fdq0",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MD",
                                  "cData1": "Moldova",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "b7afispdcz",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TG",
                                  "cData1": "Togo",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "b8ehp2af8m",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CV",
                                  "cData1": "Cape Verde",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "bi8zi6r8bs",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "LU",
                                  "cData1": "Luxembourg",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "bj9ugtwjn8",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TO",
                                  "cData1": "Tonga",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "bkosiojfsn",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "KP",
                                  "cData1": "Korea",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "bohqnnm03m",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GN",
                                  "cData1": "Guinea",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "c6766r77u5",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CX",
                                  "cData1": "Christmas Island",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "cic5np1dh1",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AE",
                                  "cData1": "United Arab Emirates",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "cmt7h7ffz0",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "PA",
                                  "cData1": "Panama",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "cq3wihf7c9",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "RE",
                                  "cData1": "Reunion",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "d0v6irjiqj",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CY",
                                  "cData1": "Cyprus",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "d1fy9ttgpx",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "KE",
                                  "cData1": "Kenya",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "d562buv842",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "ST",
                                  "cData1": "Sao Tome And Principe",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "dbkukecc5v",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TM",
                                  "cData1": "Turkmenistan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "dhwx83qbic",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TC",
                                  "cData1": "Turks And Caicos Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "drqb53d6bn",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SG",
                                  "cData1": "Singapore",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "dtvdy0djxp",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "JP",
                                  "cData1": "Japan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "dyxtvc7gny",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SE",
                                  "cData1": "Sweden",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "dzof00jhku",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "HN",
                                  "cData1": "Honduras",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "e0behm35fi",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MG",
                                  "cData1": "Madagascar",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ehbih0rby0",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MH",
                                  "cData1": "Marshall Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ekmdd8ahdz",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "PE",
                                  "cData1": "Peru",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "enwryfy7i9",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "RW",
                                  "cData1": "Rwanda",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "evmfbjeje3",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BR",
                                  "cData1": "Brazil",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "f5bjhh4b69",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CF",
                                  "cData1": "Central African Republic",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "f5d6vh9c7j",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "ML",
                                  "cData1": "Mali",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "f9iw2dp07d",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "EE",
                                  "cData1": "Estonia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "fco3csxdc6",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "NO",
                                  "cData1": "Norway",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "fd7n1817d3",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CL",
                                  "cData1": "Chile",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ffsm4ah5jp",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TR",
                                  "cData1": "Turkey",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "fg7ojxdend",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GS",
                                  "cData1": "South Georgia & South Sandwich Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ey7f46g9mo",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "VC",
                                  "cData1": "Saint Vincent And The Grenadines",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "eyvh1q1dya",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AW",
                                  "cData1": "Aruba",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "fjxhwgy6oz",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SR",
                                  "cData1": "Suriname",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "fmvp41gee9",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "HM",
                                  "cData1": "Heard And Mc Donald Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "fqpjz1khbc",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "RO",
                                  "cData1": "Romania",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "fuaviocf7d",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BN",
                                  "cData1": "Brunei Darussalam",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "g8wog8ibjj",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GA",
                                  "cData1": "Gabon",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "gdtqp7iiy6",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BZ",
                                  "cData1": "Belize",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "gmu2ax79xn",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AU",
                                  "cData1": "Australia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "h2waknh5zg",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BO",
                                  "cData1": "Bolivia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "h7r5jydawr",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BS",
                                  "cData1": "Bahamas",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "hvzbybzfzs",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CG",
                                  "cData1": "Congo",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "fz7sm7wd1h",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "EG",
                                  "cData1": "Egypt",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ic4mxm3fy4",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "LI",
                                  "cData1": "Liechtenstein",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "idcixrr62e",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "IE",
                                  "cData1": "Ireland",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "if9tbuvitw",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "PM",
                                  "cData1": "Saint Pierre and Miquelon",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "izvgcszjnk",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CC",
                                  "cData1": "Cocos (Keeling) Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "j91he8qiwd",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "VU",
                                  "cData1": "Vanuatu",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "j9dk8r7ghm",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MC",
                                  "cData1": "Monaco",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "jbug0e4ewt",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "IT",
                                  "cData1": "Italy",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "jnidhes54u",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "FI",
                                  "cData1": "Finland",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "js2a286hnd",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "FO",
                                  "cData1": "Faroe Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "jsux4zf75o",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "RS",
                                  "cData1": "Serbia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "k2qz4pyifc",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GD",
                                  "cData1": "Grenada",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "k6uyr0t7sf",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CK",
                                  "cData1": "Cook Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "keptk3db0e",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "DZ",
                                  "cData1": "Algeria",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "kes5c1zhn6",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CA",
                                  "cData1": "Canada",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "kup25x65mj",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SZ",
                                  "cData1": "Swaziland",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "kysc951de9",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "DM",
                                  "cData1": "Dominica",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "kyz7f3fh9y",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "PW",
                                  "cData1": "Palau",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "m1q16m59ck",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TJ",
                                  "cData1": "Tajikistan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "m3opj5adff",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "ZA",
                                  "cData1": "South Africa",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "m5qnczf6d2",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "KG",
                                  "cData1": "Kyrgyzstan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "mbo6209e56",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "HR",
                                  "cData1": "Croatia (Hrvatska)",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "mcernhjajt",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "ET",
                                  "cData1": "Ethiopia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "kjikh4e9d3",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SO",
                                  "cData1": "Somalia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "mmxvr7c8jr",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "EC",
                                  "cData1": "Ecuador",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "mx3msf2i4a",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "DO",
                                  "cData1": "Dominican Republic",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "n1y7iswaf1",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "ZW",
                                  "cData1": "Zimbabwe",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "nc0fmgvifv",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "KW",
                                  "cData1": "Kuwait",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ndyu97iea5",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GM",
                                  "cData1": "Gambia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "nh4jg6ajp0",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "FR",
                                  "cData1": "France",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "nqzfgq2c0r",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "LS",
                                  "cData1": "Lesotho",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "nvrpyhfj6m",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "EH",
                                  "cData1": "Western Sahara",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "oanqqukca3",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GR",
                                  "cData1": "Greece",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ocxrnfsa0x",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "NE",
                                  "cData1": "Niger",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "odc5dgdjoo",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "PK",
                                  "cData1": "Pakistan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ohgecg3dz9",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "VN",
                                  "cData1": "Viet Nam",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "oot88dg57h",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "DK",
                                  "cData1": "Denmark",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "oqya2j6cmv",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "PY",
                                  "cData1": "Paraguay",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "orpq8tock0",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "WS",
                                  "cData1": "Samoa",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "oy4tj5u90j",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SC",
                                  "cData1": "Seychelles",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "p1mmhokjat",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "NL",
                                  "cData1": "Netherlands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "p4fo2t9b5c",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "IS",
                                  "cData1": "Iceland",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "p8k78bz5mc",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MZ",
                                  "cData1": "Mozambique",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "pad4se3dfk",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "HU",
                                  "cData1": "Hungary",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "pb8dsy100i",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BG",
                                  "cData1": "Bulgaria",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "nwn7g9rb9a",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "UM",
                                  "cData1": "United States Minor Outlying Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "o3c3vfkd2y",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TW",
                                  "cData1": "Taiwan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "o8trsfwi66",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "KI",
                                  "cData1": "Kiribati",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "pgdi21xei2",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "ZZ",
                                  "cData1": "Other - not listed",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "phmhqafd0h",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "KH",
                                  "cData1": "Cambodia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "pnunwcr9s8",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SB",
                                  "cData1": "Solomon Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "prr092ei1j",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "IO",
                                  "cData1": "British Indian Ocean Territory",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "pwgppmpee2",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "LV",
                                  "cData1": "Latvia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "px482h9j3x",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "UG",
                                  "cData1": "Uganda",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "pzbrshcbpr",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "LR",
                                  "cData1": "Liberia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "q1bf1qmf85",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "ES",
                                  "cData1": "Spain",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "q2hwtf9hap",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "NG",
                                  "cData1": "Nigeria",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "q4sjnf0e0s",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AL",
                                  "cData1": "Albania",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "q65g5x80zd",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "FM",
                                  "cData1": "Micronesia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "q67ue2zjyp",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BJ",
                                  "cData1": "Benin",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "qbt466h97b",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "NP",
                                  "cData1": "Nepal",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "qfi661ci4a",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "IR",
                                  "cData1": "Iran (Islamic Republic Of)",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "qgfvcuq7nz",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "OM",
                                  "cData1": "Oman",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "qic50dpdhs",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "HT",
                                  "cData1": "Haiti",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "qxdiyhwd3i",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MX",
                                  "cData1": "Mexico",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "r0e9c26iqi",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GE",
                                  "cData1": "Georgia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "r7m2yaj040",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "PT",
                                  "cData1": "Portugal",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "r8rgr6977v",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MA",
                                  "cData1": "Morocco",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "r9mvwki6o4",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BT",
                                  "cData1": "Bhutan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "r9y9v71ach",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AM",
                                  "cData1": "Armenia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "rfdsehdfw8",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "NF",
                                  "cData1": "Norfolk Island",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "rmjea1veep",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SL",
                                  "cData1": "Sierra Leone",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "rnjc8bjeai",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AG",
                                  "cData1": "Antigua and Barbuda",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "rqev2ia81q",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BF",
                                  "cData1": "Burkina Faso",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "rrhq2jc0fw",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "LT",
                                  "cData1": "Lithuania",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "rx6uiyvj3m",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BM",
                                  "cData1": "Bermuda",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "qmb3qbgi18",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "LK",
                                  "cData1": "Sri Lanka",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "qrg1it3i18",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AO",
                                  "cData1": "Angola",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "qu3pux6dpo",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "JM",
                                  "cData1": "Jamaica",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "qvtbxkw9m9",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "ER",
                                  "cData1": "Eritrea",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "s52r1ibaaa",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "AD",
                                  "cData1": "Andorra",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "s53e4ju0p3",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SV",
                                  "cData1": "El Salvador",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "s6fhrvhasp",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SA",
                                  "cData1": "Saudi Arabia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "sevvupm7aj",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SD",
                                  "cData1": "Sudan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "srfu6kxfmu",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "KZ",
                                  "cData1": "Kazakhstan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "srof8ptcit",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MY",
                                  "cData1": "Malaysia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "suu1yo7hc8",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "KM",
                                  "cData1": "Comoros",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "sva25hsg73",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "VA",
                                  "cData1": "Holy See (Vatican City State)",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "swd559d0m5",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SH",
                                  "cData1": "Saint Helena",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "syqybo9bdr",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BE",
                                  "cData1": "Belgium",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "t9mno1yi1n",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "HK",
                                  "cData1": "Hong Kong",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "tf2d8tnfmh",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GL",
                                  "cData1": "Greenland",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "tfvspot99w",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "LY",
                                  "cData1": "Libyan Arab Jamahiriya",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "thn50gqbbm",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "DJ",
                                  "cData1": "Djibouti",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "tjh54uba8j",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "CH",
                                  "cData1": "Switzerland",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "tyec624d78",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MW",
                                  "cData1": "Malawi",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "uahk7w2i9z",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TT",
                                  "cData1": "Trinidad And Tobago",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ubm8nd8f35",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BB",
                                  "cData1": "Barbados",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ugskv9ccfa",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "PF",
                                  "cData1": "French Polynesia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ukp2iv5fbx",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GW",
                                  "cData1": "Guinea-Bissau",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "v4d9xjs65k",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BV",
                                  "cData1": "Bouvet Island",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "v4fq5amamj",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "NU",
                                  "cData1": "Niue",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "vbf1ewmd5d",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "NI",
                                  "cData1": "Nicaragua",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "vdpyqyvbhi",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "TP",
                                  "cData1": "East Timor",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "vexmymw69i",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "IQ",
                                  "cData1": "Iraq",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "vmcf9i68zh",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "IL",
                                  "cData1": "Israel",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "vqfwxbyehw",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "ID",
                                  "cData1": "Indonesia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "vy60s0e8k5",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "ZM",
                                  "cData1": "Zambia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "w52ott6h9z",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GI",
                                  "cData1": "Gibraltar",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "wmjiyvtic0",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GY",
                                  "cData1": "Guyana",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "wqv1ydzjzg",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "NR",
                                  "cData1": "Nauru",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "x0zcx3yiyy",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GU",
                                  "cData1": "Guam",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "xdvh2bo6t7",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SK",
                                  "cData1": "Slovakia (Slovak Republic)",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "xgsnwexcsx",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "LA",
                                  "cData1": "Lao People's Democratic Republic",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "xijgnskin5",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "IN",
                                  "cData1": "India",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "xj6dddwf5j",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "BA",
                                  "cData1": "Bosnia and Herzegovina",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "xmrgtmbijb",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "SM",
                                  "cData1": "San Marino",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "y6xwogjaw5",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "PR",
                                  "cData1": "Puerto Rico",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "yrxbgc8987",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "GT",
                                  "cData1": "Guatemala",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "z2outpijet",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "FJ",
                                  "cData1": "Fiji",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "zfv5iu0geo",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "MQ",
                                  "cData1": "Martinique",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "zoriaokjrg",
                                  "type": null,
                                  "key": "COUNTRY",
                                  "cData": "KY",
                                  "cData1": "Cayman Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "vergmmejvi",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "RUNTIME_UPGRADE",
                                  "cData1": "",
                                  "nData": 50.0000
                              },
                              {
                                  "id": "za0yxdoak8",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "VENDOR",
                                  "cData1": "",
                                  "nData": 20.0000
                              },
                              {
                                  "id": "yt2xbypag9",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "RESELLER",
                                  "cData1": "",
                                  "nData": 20.0000
                              },
                              {
                                  "id": "vyra73s5h1",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "RESELLER_UPGRADE",
                                  "cData1": null,
                                  "nData": 60.0000
                              },
                              {
                                  "id": "sypjts7a4v",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "CURRENCY_50",
                                  "cData1": null,
                                  "nData": 50.0000
                              },
                              {
                                  "id": "pstxzbrfpp",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "SORRY_10",
                                  "cData1": null,
                                  "nData": 10.0000
                              },
                              {
                                  "id": "neex1hpf0h",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "CURRENCY_60",
                                  "cData1": null,
                                  "nData": 60.0000
                              },
                              {
                                  "id": "mpyf3ude06",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "CURRENCY_10",
                                  "cData1": null,
                                  "nData": 10.0000
                              },
                              {
                                  "id": "kjd5f4r87m",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "DISCOUNT_10",
                                  "cData1": "",
                                  "nData": 10.0000
                              },
                              {
                                  "id": "jtx3d6tdm1",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "FREE_UPGRADE",
                                  "cData1": "",
                                  "nData": 100.0000
                              },
                              {
                                  "id": "hd1tgxtckt",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "CURRENCY_30",
                                  "cData1": null,
                                  "nData": 30.0000
                              },
                              {
                                  "id": "fvztxanino",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "MM_RUNTIME_UPGRADE",
                                  "cData1": null,
                                  "nData": 50.0000
                              },
                              {
                                  "id": "cbatkjj0p6",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "USERGROUP",
                                  "cData1": "",
                                  "nData": 10.0000
                              },
                              {
                                  "id": "b43yx2ba80",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "CURRENCY_20",
                                  "cData1": null,
                                  "nData": 20.0000
                              },
                              {
                                  "id": "3fxuxyia0j",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "CURRENCY_55",
                                  "cData1": null,
                                  "nData": 55.0000
                              },
                              {
                                  "id": "8rgxwn28si",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "CURRENCY_40",
                                  "cData1": null,
                                  "nData": 40.0000
                              },
                              {
                                  "id": "4nfmba4huq",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "PRODUCT_UPGRADE",
                                  "cData1": null,
                                  "nData": 50.0000
                              },
                              {
                                  "id": "4427353d75",
                                  "type": null,
                                  "key": "PROMO",
                                  "cData": "MM_RUNTIME_UPGRADE_RESELLER",
                                  "cData1": null,
                                  "nData": 60.0000
                              },
                              {
                                  "id": "39gz960fo5",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "NM",
                                  "cData1": "New Mexico",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "5dfs0ie5yj",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "MN",
                                  "cData1": "Minnesota",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "2qkvwccbjb",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "MS",
                                  "cData1": "Mississippi",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "2ued7ai9ga",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "WI",
                                  "cData1": "Wisconsin",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "2y5qa4qd8q",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "MB",
                                  "cData1": "Manitoba",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "1ser0dcenz",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "VA",
                                  "cData1": "Virginia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "0xt7jhi7jn",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "ON",
                                  "cData1": "Ontario",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "0fvc72zfhp",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "PR",
                                  "cData1": "Puerto Rico",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "0h4xyqn0ey",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "NF",
                                  "cData1": "Newfoundland",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "0h5rs7e7dd",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "IA",
                                  "cData1": "Iowa",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "87s06qz6o2",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "SC",
                                  "cData1": "South Carolina",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "4rdp7wtjbe",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "CT",
                                  "cData1": "Connecticut",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "6cyau71eai",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "AK",
                                  "cData1": "Alaska",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "69bx19ki5m",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "QC",
                                  "cData1": "Quebec",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "82ete05hdc",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "TN",
                                  "cData1": "Tennessee",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "a663uaghrv",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "TX",
                                  "cData1": "Texas",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "b1cb95hgob",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "WV",
                                  "cData1": "West Virginia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "9vv5n180xy",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "IN",
                                  "cData1": "Indiana",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "8oaepr9iia",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "AB",
                                  "cData1": "Alberta",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "8gc92kpgqv",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "GA",
                                  "cData1": "Georgia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "b7ezkxgcrr",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "NE",
                                  "cData1": "Nebraska",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "bseskgzhw8",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "ME",
                                  "cData1": "Maine",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "bz162935dc",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "AP",
                                  "cData1": "Armed Forces Pacific",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "cbw9f3v880",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "PA",
                                  "cData1": "Pennsylvania",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "cawb6m60of",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "HI",
                                  "cData1": "Hawaii",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "cjyurx2e9x",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "NY",
                                  "cData1": "New York",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "eze8bq77kf",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "RI",
                                  "cData1": "Rhode Island",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "f2zbbgq77i",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "FM",
                                  "cData1": "Federated States of Micronesia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "fiyz2owgrc",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "SK",
                                  "cData1": "Saskatchewan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "exjd88ufi3",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "AE",
                                  "cData1": "Armed Forces Canada",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "enm79u1hfd",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "GU",
                                  "cData1": "Guam",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ec4jnktanm",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "FL",
                                  "cData1": "Florida",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "eh4vh068zj",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "MI",
                                  "cData1": "Michigan",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "dxcat2o5s8",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "AL",
                                  "cData1": "Alabama",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "dxxsssb9c8",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "NJ",
                                  "cData1": "New Jersey",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "dd1h7ymhmb",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "WY",
                                  "cData1": "Wyoming",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "hfjderhj84",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "PE",
                                  "cData1": "Prince Edward Island",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "gpp5pcz6ok",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "DE",
                                  "cData1": "Delaware",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "iewi058eq6",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "ID",
                                  "cData1": "Idaho",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "g4ubf5f991",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "VT",
                                  "cData1": "Vermont",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "i4p83uw8do",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "NV",
                                  "cData1": "Nevada",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "i96ctoa7ef",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "OR",
                                  "cData1": "Oregon",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ia1ftbn8if",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "MO",
                                  "cData1": "Missouri",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "jc5869c7jr",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "MA",
                                  "cData1": "Massachusetts",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "jioh9ofbum",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "NC",
                                  "cData1": "North Carolina",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "j1fdeo85a8",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "NT",
                                  "cData1": "Northwest Territories",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ipgmqe2ast",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "DC",
                                  "cData1": "District of Columbia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "iz2h9hh0bm",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "PW",
                                  "cData1": "Palau",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "kjkyufegoo",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "CA",
                                  "cData1": "California",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "kmoegerj7d",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "WA",
                                  "cData1": "Washington",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "n3pn0pfj3k",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "AE",
                                  "cData1": "Armed Forces Africa",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "n5u8309f1j",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "OK",
                                  "cData1": "Oklahoma",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "mg1fz5cj82",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "AZ",
                                  "cData1": "Arizona",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "nfboubbibt",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "VI",
                                  "cData1": "Virgin Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "nwhoyzzakf",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "YT",
                                  "cData1": "Yukon",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "nn6vio771f",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "UT",
                                  "cData1": "Utah",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "pdfrp0jbu3",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "AA",
                                  "cData1": "Armed Forces Americas",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "o93qxe0hpw",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "KS",
                                  "cData1": "Kansas",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "qita7o008q",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "AR",
                                  "cData1": "Arkansas",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "sg5n6vo6d7",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "LA",
                                  "cData1": "Louisiana",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "qxaaoezaqh",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "AE",
                                  "cData1": "Armed Forces Europe",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "s3gwc3qjaw",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "MP",
                                  "cData1": "Northern Mariana Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "s3sgjiie4t",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "BC",
                                  "cData1": "British Columbia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "cukuwt5cu2",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "MH",
                                  "cData1": "Marshall Islands",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "t4efvix86e",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "AE",
                                  "cData1": "Armed Forces Middle East",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "sy4e3i5jzo",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "CO",
                                  "cData1": "Colorado",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "uykjqfvcja",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "ND",
                                  "cData1": "North Dakota",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "v2cud2i0ma",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "SD",
                                  "cData1": "South Dakota",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "u5hvef46bb",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "MD",
                                  "cData1": "Maryland",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "ve6evv0cqg",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "AS",
                                  "cData1": "American Samoa",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "vaio7aed16",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "IL",
                                  "cData1": "Illinois",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "xwcxfvy8w5",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "MT",
                                  "cData1": "Montana",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "wyu32nve32",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "KY",
                                  "cData1": "Kentucky",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "wd5tefs00y",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "OH",
                                  "cData1": "Ohio",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "weuukj9ir0",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "NH",
                                  "cData1": "New Hampshire",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "zcemp7vi4o",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "NS",
                                  "cData1": "Nova Scotia",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "zy04d5t0hi",
                                  "type": null,
                                  "key": "STATE",
                                  "cData": "NB",
                                  "cData1": "New Brunswick",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "hjmab2djjh",
                                  "type": null,
                                  "key": "TEST",
                                  "cData": "TEST1",
                                  "cData1": "Test1",
                                  "nData": 0.0000
                              },
                              {
                                  "id": "cq9eofceha",
                                  "type": null,
                                  "key": "TEST",
                                  "cData": "Test2",
                                  "cData1": "Test2",
                                  "nData": 0.0000
                              }
                          ]
                      }
                      """;


}
