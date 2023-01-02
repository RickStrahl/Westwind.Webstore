using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Westwind.Data.EfCore;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Business
{
    public class LookupBusiness : WebStoreBusinessObject<Lookup>

    {
        public LookupBusiness(WebStoreContext context) : base(context)
        {
        }

        /// <summary>
        /// Returns a list of all countries and codes
        /// </summary>
        /// <returns></returns>
        public List<CountryListItem> GetCountries()
        {
            return Context.Lookups
                .Where(l => l.Key == "COUNTRY")
                .OrderBy(l => l.CData1)
                .Select(c => new CountryListItem() {Country = c.CData1, CountryCode = c.CData})
                .ToList();
        }

        /// <summary>
        /// Returns all the US and Canadian states
        /// </summary>
        /// <returns></returns>
        public List<StateListItem> GetStates()
        {
            return Context.Lookups
                .Where(l => l.Key == "STATE")
                .OrderBy(l => l.CData1)
                .Select(c => new StateListItem() {State = c.CData1, StateCode = c.CData})
                .ToList();
        }


        /// <summary>
        /// Returns a Promo Code percentage from the look ups table.
        /// if not found returns 0.00.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public decimal GetPromoCodePercentage(string code)
        {
            if (string.IsNullOrEmpty(code)) return 0;

            code = code.ToUpper();

            return Context.Lookups
                .Where(l => l.Key == "PROMO" && l.CData.ToUpper() == code)
                .Select(l => l.NData / 100)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns a list of selected items
        /// </summary>
        /// <returns></returns>
        public List<FeaturedProduct> GetFeaturedProducts()
        {
            var list = Context.Products
                .FromSqlRaw(
                    @"SELECT top 100 CData as Sku, Products.Description, Products.ItemImage FROM Lookups, Products
WHERE [Key] = 'FEATUREDSKU' and Lookups.CData = products.sku
ORDER BY Lookups.nData Desc")
                .Select(p => new FeaturedProduct() {Sku = p.Sku, Description = p.Description, ItemImage = p.ItemImage})
                .ToList();

            return list;
        }

        public bool InsertInitialData()
        {
            string sql = @"
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'05zbjbu84w', NULL, N'COUNTRY', N'GH', N'Ghana', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'09oique5gk', NULL, N'REFERAL', N'', N'Search Engine', CAST(80.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'0fvc72zfhp', NULL, N'STATE', N'PR', N'Puerto Rico', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'0h4xyqn0ey', NULL, N'STATE', N'NF', N'Newfoundland', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'0h5rs7e7dd', NULL, N'STATE', N'IA', N'Iowa', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'0oaoosa53h', NULL, N'COUNTRY', N'TK', N'Tokelau', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'0p2m4fsjbn', NULL, N'COUNTRY', N'AS', N'American Samoa', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'0sikhgscfm', NULL, N'COUNTRY', N'LB', N'Lebanon', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'0x7r0ymcrg', NULL, N'COUNTRY', N'JO', N'Jordan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'0xt7jhi7jn', NULL, N'STATE', N'ON', N'Ontario', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'184d32vgcu', NULL, N'COUNTRY', N'MN', N'Monlia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'1c897i5jiy', NULL, N'COUNTRY', N'SI', N'Slovenia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'1fbx10xh95', NULL, N'COUNTRY', N'MP', N'Northern Mariana Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'1g1otywaff', NULL, N'REFERAL', N'', N'Code Magazine Article', CAST(78.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'1mt4j2gha7', NULL, N'COUNTRY', N'MT', N'Malta', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'1ser0dcenz', NULL, N'STATE', N'VA', N'Virginia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'2f3k5p3fna', NULL, N'REFERAL', N'', N'Developer Conference', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'2feyphs55q', NULL, N'Country', N'TW', N'Taiwan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'2mxvxrdb8o', NULL, N'COUNTRY', N'MM', N'Myanmar', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'2npmkgw7jj', NULL, N'COUNTRY', N'SN', N'Senegal', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'2qkvwccbjb', NULL, N'STATE', N'MS', N'Mississippi', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'2ued7ai9ga', NULL, N'STATE', N'WI', N'Wisconsin', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'2y5qa4qd8q', NULL, N'STATE', N'MB', N'Manitoba', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'2yencxn8vc', NULL, N'COUNTRY', N'DE', N'Germany', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'325m21b82o', NULL, N'COUNTRY', N'GB', N'United Kingdom', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'328pfn2fdr', NULL, N'COUNTRY', N'NC', N'New Caledonia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'32p0o32hni', NULL, N'PROMO', N'MM_FREE_UPGRADE', NULL, CAST(99.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'34jy438he2', NULL, N'COUNTRY', N'MO', N'Macau', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'36xbyeddse', NULL, N'COUNTRY', N'AR', N'Argentina', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'39gz960fo5', NULL, N'STATE', N'NM', N'New Mexico', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'3fxuxyia0j', NULL, N'PROMO', N'CURRENCY_55', NULL, CAST(55.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'3g4pu2jehg', NULL, N'COUNTRY', N'PN', N'Pitcairn', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'3p3g2rh525', NULL, N'COUNTRY', N'VI', N'Virgin Islands (U.S.)', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'3zbgpeiitz', NULL, N'COUNTRY', N'PS', N'Palestinian Territory', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'4427353d75', NULL, N'PROMO', N'MM_RUNTIME_UPGRADE_RESELLER', NULL, CAST(60.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'4c92udh88p', NULL, N'COUNTRY', N'CZ', N'Czech Republic', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'4g83vg1dy6', NULL, N'COUNTRY', N'KN', N'Saint Kitts And Nevis', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'4m5861eavj', NULL, N'COUNTRY', N'FK', N'Falkland Islands (Malvinas)', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'4nfmba4huq', NULL, N'PROMO', N'PRODUCT_UPGRADE', NULL, CAST(50.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'4nq5ghkfot', NULL, N'COUNTRY', N'SY', N'Syrian Arab Republic', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'4r35j2y6ab', NULL, N'COUNTRY', N'BW', N'Botswana', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'4rdp7wtjbe', NULL, N'STATE', N'CT', N'Connecticut', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'4vpejez7he', NULL, N'COUNTRY', N'PL', N'Poland', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'4wbucqqhkx', NULL, N'COUNTRY', N'AT', N'Austria', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'4x1s2cwh3k', NULL, N'COUNTRY', N'CD', N'Con', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'5133hca5kk', NULL, N'COUNTRY', N'GP', N'Guadeloupe', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'56kijcn9kj', NULL, N'COUNTRY', N'TN', N'Tunisia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'5aom5syhy6', NULL, N'COUNTRY', N'TV', N'Tuvalu', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'5dfs0ie5yj', NULL, N'STATE', N'MN', N'Minnesota', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'5e0801xb1r', NULL, N'COUNTRY', N'YT', N'Mayotte', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'5qexxopgqu', NULL, N'COUNTRY', N'YE', N'Yemen', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'5tq2fqh0cu', NULL, N'COUNTRY', N'MV', N'Maldives', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'5wedk6njc9', NULL, N'COUNTRY', N'CO', N'Colombia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'5yo7dtmeqz', NULL, N'COUNTRY', N'NZ', N'New Zealand', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'62urtru0pu', NULL, N'COUNTRY', N'UY', N'Uruguay', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'63an8ywcrr', NULL, N'COUNTRY', N'RU', N'Russian Federation', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'697gw38e5t', NULL, N'COUNTRY', N'PG', N'Papua New Guinea', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'69bx19ki5m', NULL, N'STATE', N'QC', N'Quebec', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'69hrrv4cji', NULL, N'COUNTRY', N'MR', N'Mauritania', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'6ambbtgbdv', NULL, N'COUNTRY', N'SJ', N'Svalbard And Jan Mayen Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'6ch39mxhi3', NULL, N'COUNTRY', N'UA', N'Ukraine', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'6cyau71eai', NULL, N'STATE', N'AK', N'Alaska', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'6efgevk6of', NULL, N'COUNTRY', N'MU', N'Mauritius', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'6i6wcsxf5x', NULL, N'COUNTRY', N'KR', N'Korea', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'6njnoss5nf', NULL, N'COUNTRY', N'MK', N'Macedonia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'6no1qyy00c', NULL, N'COUNTRY', N'AF', N'Afghanistan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'704mk0t57y', NULL, N'COUNTRY', N'TZ', N'Tanzania', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'78b7iv6gfh', NULL, N'COUNTRY', N'AI', N'Anguilla', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'7dekpcaewm', NULL, N'COUNTRY', N'WF', N'Wallis And Futuna Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'7f2wkstii0', NULL, N'COUNTRY', N'LC', N'Saint Lucia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'7nnswumcdv', NULL, N'COUNTRY', N'PH', N'Philippines', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'7qco94pf2y', NULL, N'COUNTRY', N'BD', N'Bangladesh', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'7qgbaihdfo', NULL, N'COUNTRY', N'CN', N'China', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'7qihafxbok', NULL, N'COUNTRY', N'BY', N'Belarus', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'7sp69un6bh', NULL, N'COUNTRY', N'VE', N'Venezuela', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'7uciyzq50z', NULL, N'REFERAL', N'', N'Advertisement', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'806jqj0fik', NULL, N'COUNTRY', N'CM', N'Cameroon', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'81z8hktdo2', NULL, N'COUNTRY', N'AN', N'Netherlands Antilles', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'82ete05hdc', NULL, N'STATE', N'TN', N'Tennessee', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'85tdsswf9b', NULL, N'COUNTRY', N'TL', N'Timor-Leste', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'86h1jd4ayw', NULL, N'COUNTRY', N'CR', N'Costa Rica', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'87s06qz6o2', NULL, N'STATE', N'SC', N'South Carolina', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'8ezmn6tjnr', NULL, N'COUNTRY', N'MS', N'Montserrat', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'8gc92kpgqv', NULL, N'STATE', N'GA', N'Georgia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'8n8vd55aw8', NULL, N'COUNTRY', N'AZ', N'Azerbaijan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'8oaepr9iia', NULL, N'STATE', N'AB', N'Alberta', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'8rgxwn28si', NULL, N'PROMO', N'CURRENCY_40', NULL, CAST(40.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'8y7nybv5h9', NULL, N'COUNTRY', N'AQ', N'Antarctica', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'9265267gyc', NULL, N'COUNTRY', N'TD', N'Chad', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'94hvhm09kf', NULL, N'COUNTRY', N'QA', N'Qatar', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'9f0dm6a08o', NULL, N'COUNTRY', N'CU', N'Cuba', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'9nv52f464x', NULL, N'COUNTRY', N'VG', N'Virgin Islands (British)', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'9ri9pkn8hk', NULL, N'COUNTRY', N'BH', N'Bahrain', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'9vv5n180xy', NULL, N'STATE', N'IN', N'Indiana', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'9vyqm3j7jp', NULL, N'COUNTRY', N'CI', N'Cote D''ivoire', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'9z6bjq99qw', NULL, N'COUNTRY', N'UZ', N'Uzbekistan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'a663uaghrv', NULL, N'STATE', N'TX', N'Texas', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'a6koih290b', NULL, N'COUNTRY', N'TH', N'Thailand', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'aafjczo802', NULL, N'COUNTRY', N'NA', N'Namibia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ah0k6x5deh', NULL, N'COUNTRY', N'GQ', N'Equatorial Guinea', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ahe75piikq', NULL, N'REFERAL', N'', N'Other - please leave a comment', CAST(-1.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ai1afx9j6t', NULL, N'COUNTRY', N'BI', N'Burundi', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ar1cmcxawx', NULL, N'COUNTRY', N'US', N'United States', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'azqj5gn67s', NULL, N'COUNTRY', N'GF', N'French Guiana', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'b1cb95hb', NULL, N'STATE', N'WV', N'West Virginia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'b1o296ng22', NULL, N'COUNTRY', N'TF', N'French Southern Territories', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'b43yx2ba80', NULL, N'PROMO', N'CURRENCY_20', NULL, CAST(20.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'b6k5n8fdq0', NULL, N'COUNTRY', N'MD', N'Moldova', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'b7afispdcz', NULL, N'COUNTRY', N'TG', N'To', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'b7ezkxgcrr', NULL, N'STATE', N'NE', N'Nebraska', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'b8ehp2af8m', NULL, N'COUNTRY', N'CV', N'Cape Verde', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'bi8zi6r8bs', NULL, N'COUNTRY', N'LU', N'Luxembourg', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'bj9ugtwjn8', NULL, N'COUNTRY', N'TO', N'Tonga', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'bkosiojfsn', NULL, N'COUNTRY', N'KP', N'Korea', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'bohqnnm03m', NULL, N'COUNTRY', N'GN', N'Guinea', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'bseskgzhw8', NULL, N'STATE', N'ME', N'Maine', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'bz162935dc', NULL, N'STATE', N'AP', N'Armed Forces Pacific', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'c6766r77u5', NULL, N'COUNTRY', N'CX', N'Christmas Island', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'cawb6m60of', NULL, N'STATE', N'HI', N'Hawaii', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'cbatkjj0p6', NULL, N'PROMO', N'USERGROUP', N'', CAST(10.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'cbw9f3v880', NULL, N'STATE', N'PA', N'Pennsylvania', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ch0zora6ht', NULL, N'REFERAL', N'', N'White Paper or Article', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'cic5np1dh1', NULL, N'COUNTRY', N'AE', N'United Arab Emirates', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'cjyurx2e9x', NULL, N'STATE', N'NY', N'New York', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'cmt7h7ffz0', NULL, N'COUNTRY', N'PA', N'Panama', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'cq3wihf7c9', NULL, N'COUNTRY', N'RE', N'Reunion', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'cukuwt5cu2', NULL, N'STATE', N'MH', N'Marshall Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'd0v6irjiqj', NULL, N'COUNTRY', N'CY', N'Cyprus', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'd1fy9ttgpx', NULL, N'COUNTRY', N'KE', N'Kenya', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'd562buv842', NULL, N'COUNTRY', N'ST', N'Sao Tome And Principe', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'dbkukecc5v', NULL, N'COUNTRY', N'TM', N'Turkmenistan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'dd1h7ymhmb', NULL, N'STATE', N'WY', N'Wyoming', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'dhwx83qbic', NULL, N'COUNTRY', N'TC', N'Turks And Caicos Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'dqmx2ox67b', NULL, N'REFERAL', N'', N'White Paper or Article by Rick Strahl', CAST(98.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'drqb53d6bn', NULL, N'COUNTRY', N'SG', N'Singapore', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'dtvdy0djxp', NULL, N'COUNTRY', N'JP', N'Japan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'dxcat2o5s8', NULL, N'STATE', N'AL', N'Alabama', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'dxxsssb9c8', NULL, N'STATE', N'NJ', N'New Jersey', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'dyxtvc7gny', NULL, N'COUNTRY', N'SE', N'Sweden', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'dzof00jhku', NULL, N'COUNTRY', N'HN', N'Honduras', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'e0behm35fi', NULL, N'COUNTRY', N'MG', N'Madagascar', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ec4jnktanm', NULL, N'STATE', N'FL', N'Florida', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'eh4vh068zj', NULL, N'STATE', N'MI', N'Michigan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ehbih0rby0', NULL, N'COUNTRY', N'MH', N'Marshall Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ekmdd8ahdz', NULL, N'COUNTRY', N'PE', N'Peru', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'enm79u1hfd', NULL, N'STATE', N'GU', N'Guam', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'enwryfy7i9', NULL, N'COUNTRY', N'RW', N'Rwanda', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'evmfbjeje3', NULL, N'COUNTRY', N'BR', N'Brazil', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'exjd88ufi3', NULL, N'STATE', N'AE', N'Armed Forces Canada', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ey7f46g9mo', NULL, N'COUNTRY', N'VC', N'Saint Vincent And The Grenadines', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'eyvh1q1dya', NULL, N'COUNTRY', N'AW', N'Aruba', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'eze8bq77kf', NULL, N'STATE', N'RI', N'Rhode Island', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'f2zbbgq77i', NULL, N'STATE', N'FM', N'Federated States of Micronesia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'f5bjhh4b69', NULL, N'COUNTRY', N'CF', N'Central African Republic', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'f5d6vh9c7j', NULL, N'COUNTRY', N'ML', N'Mali', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'f9iw2dp07d', NULL, N'COUNTRY', N'EE', N'Estonia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'fco3csxdc6', NULL, N'COUNTRY', N'NO', N'Norway', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'fd7n1817d3', NULL, N'COUNTRY', N'CL', N'Chile', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ffsm4ah5jp', NULL, N'COUNTRY', N'TR', N'Turkey', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'fg7ojxdend', NULL, N'COUNTRY', N'GS', N'South Georgia & South Sandwich Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'fiyz2owgrc', NULL, N'STATE', N'SK', N'Saskatchewan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'fjxhwgy6oz', NULL, N'COUNTRY', N'SR', N'Suriname', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'fmvp41gee9', NULL, N'COUNTRY', N'HM', N'Heard And Mc Donald Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'fqpjz1khbc', NULL, N'COUNTRY', N'RO', N'Romania', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'fuaviocf7d', NULL, N'COUNTRY', N'BN', N'Brunei Darussalam', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'fvztxanino', NULL, N'PROMO', N'MM_RUNTIME_UPGRADE', NULL, CAST(50.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'fz7sm7wd1h', NULL, N'COUNTRY', N'EG', N'Egypt', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'g4ubf5f991', NULL, N'STATE', N'VT', N'Vermont', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'g765c79b22', NULL, N'REFERAL', N'', N'Universal Thread Web Site', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'g8wog8ibjj', NULL, N'COUNTRY', N'GA', N'Gabon', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'gdtqp7iiy6', NULL, N'COUNTRY', N'BZ', N'Belize', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'gmu2ax79xn', NULL, N'COUNTRY', N'AU', N'Australia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'gpp5pcz6ok', NULL, N'STATE', N'DE', N'Delaware', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'h2waknh5zg', NULL, N'COUNTRY', N'BO', N'Bolivia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'h7r5jydawr', NULL, N'COUNTRY', N'BS', N'Bahamas', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'hd1tgxtckt', NULL, N'PROMO', N'CURRENCY_30', NULL, CAST(30.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'hfjderhj84', NULL, N'STATE', N'PE', N'Prince Edward Island', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'hvzbybzfzs', NULL, N'COUNTRY', N'CG', N'Con', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'hwvvo5r9y7', NULL, N'PROMO', N'FREE_UPGRADE', NULL, CAST(99.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'i4p83uw8do', NULL, N'STATE', N'NV', N'Nevada', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'i96ctoa7ef', NULL, N'STATE', N'OR', N'Oren', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ia1ftbn8if', NULL, N'STATE', N'MO', N'Missouri', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ic4mxm3fy4', NULL, N'COUNTRY', N'LI', N'Liechtenstein', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'idcixrr62e', NULL, N'COUNTRY', N'IE', N'Ireland', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'iewi058eq6', NULL, N'STATE', N'ID', N'Idaho', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'if9tbuvitw', NULL, N'COUNTRY', N'PM', N'Saint Pierre and Miquelon', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ijik9806oa', NULL, N'REFERAL', N'', N'Microsoft Link or Referral', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ipgmqe2ast', NULL, N'STATE', N'DC', N'District of Columbia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'iz2h9hh0bm', NULL, N'STATE', N'PW', N'Palau', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'izvgcszjnk', NULL, N'COUNTRY', N'CC', N'Cocos (Keeling) Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'j1fdeo85a8', NULL, N'STATE', N'NT', N'Northwest Territories', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'j91he8qiwd', NULL, N'COUNTRY', N'VU', N'Vanuatu', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'j9dk8r7ghm', NULL, N'COUNTRY', N'MC', N'Monaco', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'jbug0e4ewt', NULL, N'COUNTRY', N'IT', N'Italy', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'jc5869c7jr', NULL, N'STATE', N'MA', N'Massachusetts', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'jioh9ofbum', NULL, N'STATE', N'NC', N'North Carolina', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'jnidhes54u', NULL, N'COUNTRY', N'FI', N'Finland', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'joyb89ufjf', NULL, N'REFERAL', NULL, N'Blog Post by Rick Strahl', CAST(98.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'js2a286hnd', NULL, N'COUNTRY', N'FO', N'Faroe Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'jsux4zf75o', NULL, N'COUNTRY', N'RS', N'Serbia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'k2qz4pyifc', NULL, N'COUNTRY', N'GD', N'Grenada', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'k6uyr0t7sf', NULL, N'COUNTRY', N'CK', N'Cook Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'keptk3db0e', NULL, N'COUNTRY', N'DZ', N'Algeria', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'kes5c1zhn6', NULL, N'COUNTRY', N'CA', N'Canada', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'kjikh4e9d3', NULL, N'COUNTRY', N'SO', N'Somalia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'kjkyufeo', NULL, N'STATE', N'CA', N'California', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'kmoegerj7d', NULL, N'STATE', N'WA', N'Washington', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'kup25x65mj', NULL, N'COUNTRY', N'SZ', N'Swaziland', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'kysc951de9', NULL, N'COUNTRY', N'DM', N'Dominica', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'kyz7f3fh9y', NULL, N'COUNTRY', N'PW', N'Palau', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'm1q16m59ck', NULL, N'COUNTRY', N'TJ', N'Tajikistan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'm3opj5adff', NULL, N'COUNTRY', N'ZA', N'South Africa', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'm5qnczf6d2', NULL, N'COUNTRY', N'KG', N'Kyrgyzstan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'mbo6209e56', NULL, N'COUNTRY', N'HR', N'Croatia (Hrvatska)', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'mcernhjajt', NULL, N'COUNTRY', N'ET', N'Ethiopia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'mg1fz5cj82', NULL, N'STATE', N'AZ', N'Arizona', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'mmxvr7c8jr', NULL, N'COUNTRY', N'EC', N'Ecuador', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'mpyf3ude06', NULL, N'PROMO', N'CURRENCY_10', NULL, CAST(10.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'mx3msf2i4a', NULL, N'COUNTRY', N'DO', N'Dominican Republic', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'n1y7iswaf1', NULL, N'COUNTRY', N'ZW', N'Zimbabwe', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'n3pn0pfj3k', NULL, N'STATE', N'AE', N'Armed Forces Africa', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'n5u8309f1j', NULL, N'STATE', N'OK', N'Oklahoma', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'nc0fmgvifv', NULL, N'COUNTRY', N'KW', N'Kuwait', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ndyu97iea5', NULL, N'COUNTRY', N'GM', N'Gambia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'neex1hpf0h', NULL, N'PROMO', N'CURRENCY_60', NULL, CAST(60.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'nfboubbibt', NULL, N'STATE', N'VI', N'Virgin Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'nh4jg6ajp0', NULL, N'COUNTRY', N'FR', N'France', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'nn6vio771f', NULL, N'STATE', N'UT', N'Utah', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'nqzfgq2c0r', NULL, N'COUNTRY', N'LS', N'Lesotho', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'nvrpyhfj6m', NULL, N'COUNTRY', N'EH', N'Western Sahara', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'nwhoyzzakf', NULL, N'STATE', N'YT', N'Yukon', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'nwn7g9rb9a', NULL, N'COUNTRY', N'UM', N'United States Minor Outlying Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'nyctqh5h7u', NULL, N'REFERAL', N'', N'Referral from Vendor', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'o8trsfwi66', NULL, N'COUNTRY', N'KI', N'Kiribati', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'o93qxe0hpw', NULL, N'STATE', N'KS', N'Kansas', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'oanqqukca3', NULL, N'COUNTRY', N'GR', N'Greece', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ocxrnfsa0x', NULL, N'COUNTRY', N'NE', N'Niger', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'odc5dgdjoo', NULL, N'COUNTRY', N'PK', N'Pakistan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ohgecg3dz9', NULL, N'COUNTRY', N'VN', N'Viet Nam', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'oot88dg57h', NULL, N'COUNTRY', N'DK', N'Denmark', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'oqya2j6cmv', NULL, N'COUNTRY', N'PY', N'Paraguay', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'orpq8tock0', NULL, N'COUNTRY', N'WS', N'Samoa', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'oy4tj5u90j', NULL, N'COUNTRY', N'SC', N'Seychelles', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'p1mmhokjat', NULL, N'COUNTRY', N'NL', N'Netherlands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'p4fo2t9b5c', NULL, N'COUNTRY', N'IS', N'Iceland', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'p8k78bz5mc', NULL, N'COUNTRY', N'MZ', N'Mozambique', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'pad4se3dfk', NULL, N'COUNTRY', N'HU', N'Hungary', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'pb8dsy100i', NULL, N'COUNTRY', N'BG', N'Bulgaria', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'pdfrp0jbu3', NULL, N'STATE', N'AA', N'Armed Forces Americas', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'pgdi21xei2', NULL, N'COUNTRY', N'ZZ', N'Other - not listed', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'phmhqafd0h', NULL, N'COUNTRY', N'KH', N'Cambodia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'pnunwcr9s8', NULL, N'COUNTRY', N'SB', N'Solomon Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'prr092ei1j', NULL, N'COUNTRY', N'IO', N'British Indian Ocean Territory', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'pstxzbrfpp', NULL, N'PROMO', N'New Promo Code', NULL, CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'pto95zch1x', NULL, N'REFERAL', NULL, N'Scott Hanselman''s Tool List', CAST(91.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'pwgppmpee2', NULL, N'COUNTRY', N'LV', N'Latvia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'px482h9j3x', NULL, N'COUNTRY', N'UG', N'Uganda', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'pzbrshcbpr', NULL, N'COUNTRY', N'LR', N'Liberia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'q1bf1qmf85', NULL, N'COUNTRY', N'ES', N'Spain', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'q2hwtf9hap', NULL, N'COUNTRY', N'NG', N'Nigeria', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'q4sjnf0e0s', NULL, N'COUNTRY', N'AL', N'Albania', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'q65g5x80zd', NULL, N'COUNTRY', N'FM', N'Micronesia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'q67ue2zjyp', NULL, N'COUNTRY', N'BJ', N'Benin', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'qbt466h97b', NULL, N'COUNTRY', N'NP', N'Nepal', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'qfi661ci4a', NULL, N'COUNTRY', N'IR', N'Iran (Islamic Republic Of)', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'qgfvcuq7nz', NULL, N'COUNTRY', N'OM', N'Oman', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'qic50dpdhs', NULL, N'COUNTRY', N'HT', N'Haiti', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'qita7o008q', NULL, N'STATE', N'AR', N'Arkansas', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'qmb3qbgi18', NULL, N'COUNTRY', N'LK', N'Sri Lanka', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'qrg1it3i18', NULL, N'COUNTRY', N'AO', N'Anla', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'qu3pux6dpo', NULL, N'COUNTRY', N'JM', N'Jamaica', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'qvtbxkw9m9', NULL, N'COUNTRY', N'ER', N'Eritrea', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'qxaaoezaqh', NULL, N'STATE', N'AE', N'Armed Forces Europe', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'qxdiyhwd3i', NULL, N'COUNTRY', N'MX', N'Mexico', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'r0e9c26iqi', NULL, N'COUNTRY', N'GE', N'Georgia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'r7m2yaj040', NULL, N'COUNTRY', N'PT', N'Portugal', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'r8rgr6977v', NULL, N'COUNTRY', N'MA', N'Morocco', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'r9mvwki6o4', NULL, N'COUNTRY', N'BT', N'Bhutan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'r9y9v71ach', NULL, N'COUNTRY', N'AM', N'Armenia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'rfdsehdfw8', NULL, N'COUNTRY', N'NF', N'Norfolk Island', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'rmjea1veep', NULL, N'COUNTRY', N'SL', N'Sierra Leone', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'rnjc8bjeai', NULL, N'COUNTRY', N'AG', N'Antigua and Barbuda', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'rqev2ia81q', NULL, N'COUNTRY', N'BF', N'Burkina Faso', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'rrhq2jc0fw', NULL, N'COUNTRY', N'LT', N'Lithuania', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'rsb015cep0', NULL, N'REFERAL', N'', N'Referred by another Developer', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'rvwnz50c9a', NULL, N'REFERAL', N'', N'Download Site', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'rx6uiyvj3m', NULL, N'COUNTRY', N'BM', N'Bermuda', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N's3gwc3qjaw', NULL, N'STATE', N'MP', N'Northern Mariana Islands', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N's3sgjiie4t', NULL, N'STATE', N'BC', N'British Columbia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N's52r1ibaaa', NULL, N'COUNTRY', N'AD', N'Andorra', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N's53e4ju0p3', NULL, N'COUNTRY', N'SV', N'El Salvador', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N's6fhrvhasp', NULL, N'COUNTRY', N'SA', N'Saudi Arabia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'sevvupm7aj', NULL, N'COUNTRY', N'SD', N'Sudan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'sg5n6vo6d7', NULL, N'STATE', N'LA', N'Louisiana', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'srfu6kxfmu', NULL, N'COUNTRY', N'KZ', N'Kazakhstan', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'srof8ptcit', NULL, N'COUNTRY', N'MY', N'Malaysia', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'suu1yo7hc8', NULL, N'COUNTRY', N'KM', N'Comoros', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'sva25hsg73', NULL, N'COUNTRY', N'VA', N'Holy See (Vatican City State)', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'swd559d0m5', NULL, N'COUNTRY', N'SH', N'Saint Helena', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'sy4e3i5jzo', NULL, N'STATE', N'CO', N'Colorado', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'sypjts7a4v', NULL, N'PROMO', N'CURRENCY_50', NULL, CAST(50.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'syqybo9bdr', NULL, N'COUNTRY', N'BE', N'Belgium', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N't4efvix86e', NULL, N'STATE', N'AE', N'Armed Forces Middle East', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N't9mno1yi1n', NULL, N'COUNTRY', N'HK', N'Hong Kong', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'tf2d8tnfmh', NULL, N'COUNTRY', N'GL', N'Greenland', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'tfvspot99w', NULL, N'COUNTRY', N'LY', N'Libyan Arab Jamahiriya', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'thn50gqbbm', NULL, N'COUNTRY', N'DJ', N'Djibouti', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'tjh54uba8j', NULL, N'COUNTRY', N'CH', N'Switzerland', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'tyec624d78', NULL, N'COUNTRY', N'MW', N'Malawi', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'u5hvef46bb', NULL, N'STATE', N'MD', N'Maryland', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'uahk7w2i9z', NULL, N'COUNTRY', N'TT', N'Trinidad And Toba', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ubm8nd8f35', NULL, N'COUNTRY', N'BB', N'Barbados', CAST(0.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ubvsq38e7k', NULL, N'REFERAL', N'', N'Code Magazine Ad', CAST(76.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ugskv9ccfa', NULL, N'COUNTRY', N'PF', N'French Polynesia', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'ukp2iv5fbx', NULL, N'COUNTRY', N'GW', N'Guinea-Bissau', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'uvoe8vre5r', NULL, N'PROMO', N'WCRUNTIMEUPGRADE', N'', CAST(50.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'uykjqfvcja', NULL, N'STATE', N'ND', N'North Dakota', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'v2cud2i0ma', NULL, N'STATE', N'SD', N'South Dakota', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'v4d9xjs65k', NULL, N'COUNTRY', N'BV', N'Bouvet Island', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'v4fq5amamj', NULL, N'COUNTRY', N'NU', N'Niue', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'vaexvdb96y', NULL, N'REFERAL', N'', N'West Wind Technologies Web Site', CAST(99.0000 AS Decimal(18, 4)))
INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'vaio7aed16', NULL, N'STATE', N'IL', N'Illinois', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'vbf1ewmd5d', NULL, N'COUNTRY', N'NI', N'Nicaragua', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'vdpyqyvbhi', NULL, N'COUNTRY', N'TP', N'East Timor', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N've6evv0cqg', NULL, N'STATE', N'AS', N'American Samoa', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'vexmymw69i', NULL, N'COUNTRY', N'IQ', N'Iraq', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'vmcf9i68zh', NULL, N'COUNTRY', N'IL', N'Israel', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'vqfwxbyehw', NULL, N'COUNTRY', N'ID', N'Indonesia', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'vy60s0e8k5', NULL, N'COUNTRY', N'ZM', N'Zambia', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'vyra73s5h1', NULL, N'PROMO', N'RESELLER_UPGRADE', NULL, CAST(60.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'w52ott6h9z', NULL, N'COUNTRY', N'GI', N'Gibraltar', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'wd5tefs00y', NULL, N'STATE', N'OH', N'Ohio', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'weuukj9ir0', NULL, N'STATE', N'NH', N'New Hampshire', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'wmjiyvtic0', NULL, N'COUNTRY', N'GY', N'Guyana', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'wqv1ydzjzg', NULL, N'COUNTRY', N'NR', N'Nauru', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'wyu32nve32', NULL, N'STATE', N'KY', N'Kentucky', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'x0zcx3yiyy', NULL, N'COUNTRY', N'GU', N'Guam', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'xdvh2bo6t7', NULL, N'COUNTRY', N'SK', N'Slovakia (Slovak Republic)', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'xgsnwexcsx', NULL, N'COUNTRY', N'LA', N'Lao People''s Democratic Republic', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'xijgnskin5', NULL, N'COUNTRY', N'IN', N'India', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'xj6dddwf5j', NULL, N'COUNTRY', N'BA', N'Bosnia and Herzevina', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'xmrgtmbijb', NULL, N'COUNTRY', N'SM', N'San Marino', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'xwcxfvy8w5', NULL, N'STATE', N'MT', N'Montana', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'xwddfniaag', NULL, N'REFERAL', NULL, N'Stack Overflow', CAST(81.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'y6xwogjaw5', NULL, N'COUNTRY', N'PR', N'Puerto Rico', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'yrxbgc8987', NULL, N'COUNTRY', N'GT', N'Guatemala', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'yt2xbypag9', NULL, N'PROMO', N'RESELLER', N'', CAST(20.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'z2outpijet', NULL, N'COUNTRY', N'FJ', N'Fiji', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'za0yxdoak8', NULL, N'PROMO', N'VENDOR', N'', CAST(20.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'zcemp7vi4o', NULL, N'STATE', N'NS', N'Nova Scotia', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'zfv5iu0geo', NULL, N'COUNTRY', N'MQ', N'Martinique', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'zoriaokjrg', NULL, N'COUNTRY', N'KY', N'Cayman Islands', CAST(0.0000 AS Decimal(18, 4)))INSERT [dbo].[Lookups] ([Id], [Type], [Key], [CData], [CData1], [NData]) VALUES (N'zy04d5t0hi', NULL, N'STATE', N'NB', N'New Brunswick', CAST(0.0000 AS Decimal(18, 4)))
";

            if (Db.ExecuteNonQuery(sql) < 0)
            {
                SetError(Db.ErrorMessage);
                return false;
            }

            // 1 Admin customer, 1 test product
            sql = @"
INSERT [dbo].[Customers] ([Id], [Email], [Firstname], [Lastname], [Company], [Password], [ValidationKey], [IsAdminUser], [Telephone], [Notes], [CustomerNotes], [ReferralCode], [IsActive], [LastOrder], [Entered], [Updated], [LanguageId], [Theme], [ExtraPropertiesStorage], [OldPk]) VALUES (N'81bo4r0faa', N'testuser@test.com', N'Jim', N'Jimson', N'Big Time LLC.', N'test', N'', 1, N'(111) 111-1111', N'', NULL, N'Big Time LLC Store', 1, CAST(N'2022-10-17T09:17:00.0000000' AS DateTime2), CAST(N'1999-11-18T16:00:00.0000000' AS DateTime2), CAST(N'2022-10-25T14:56:26.9823529' AS DateTime2), N'en', N'Light', NULL, 0)
INSERT [dbo].[Addresses] ([Id], [CustomerId], [AddressName], [StreetAddress], [City], [PostalCode], [State], [Country], [CountryCode], [AddressType], [SortOrder], [AddressFullname], [AddressCompany], [Telephone]) VALUES (N'dkx6e48ir8', N'81bo4r0faa', NULL, N'11 Nowhere Lane', N'Nowhere', N'96771', N'HI', N'United States', N'US', N'Billing', 0, N'Jim Jimson', N'Big Time LLC', N'')
";

            if (Db.ExecuteNonQuery(sql) < 0)
            {
                SetError(Db.ErrorMessage);
                return false;
            }


            return true;
        }
    }

    public class FeaturedProduct
    {
        public string Sku { get; set;  }
        public string Description { get; set; }

        public string ItemImage { get; set; }
    }


    public class CategoryBusiness : EntityFrameworkBusinessObject<WebStoreContext,Category>
    {
        public CategoryBusiness(WebStoreContext context) : base(context)
        {
        }

        public Dictionary<string,string> GetCategoryList()
        {
            var productBusiness = BusinessFactory.Current.GetProductBusiness(Context);

            var list = Context.Categories
                .OrderBy(c => c.CategoryName)
                .Select(c => new KeyValuePair<string, string>(c.CategoryName, c.Description));

            var dict = new Dictionary<string, string>(list);

            var delKeys = new List<string>();
            foreach (var kv in dict)
            {
                if (!productBusiness.HasAnyCategory(kv.Key))
                    delKeys.Add(kv.Key);
            }
            foreach (var key in delKeys)
            {
                dict.Remove(key);
            }

            return dict;
        }
    }




    public class CountryListItem
    {
        public string CountryCode { get; set; }
        public string Country { get; set;  }
    }

    public class StateListItem
    {
        public string StateCode { get; set; }
        public string State { get; set;  }
    }

}

