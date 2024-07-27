﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Westwind.Webstore.Business.Entities;

#nullable disable

namespace Westwind.Webstore.Business.Migrations
{
    [DbContext(typeof(WebStoreContext))]
    [Migration("20240518165130_add-shipping-weight-to-lineitems")]
    partial class Addshippingweighttolineitems
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Westwind.Webstore.Business.Entities.Address", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("AddressCompany")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddressFullname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddressName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddressType")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CountryCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerId")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortOrder")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StreetAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telephone")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("Westwind.Webstore.Business.Entities.Category", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("CategoryName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Keywords")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentId")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Westwind.Webstore.Business.Entities.Customer", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Company")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerNotes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<DateTime>("Entered")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExtraPropertiesStorage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Firstname")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAdminUser")
                        .HasColumnType("bit");

                    b.Property<string>("LanguageId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastOrder")
                        .HasColumnType("datetime2");

                    b.Property<string>("Lastname")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OldPk")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ReferralCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telephone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Theme")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TwoFactorKey")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<string>("ValidationKey")
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Westwind.Webstore.Business.Entities.Invoice", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("BillingAddressJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Completed")
                        .HasColumnType("datetime2");

                    b.Property<string>("ConfirmationEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerId")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("ExtraPropertiesStorage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("InvoiceDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("InvoiceNumber")
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<decimal>("InvoiceTotal")
                        .HasColumnType("decimal(18,4)");

                    b.Property<bool>("IsShipping")
                        .HasColumnType("bit");

                    b.Property<bool>("IsTemporary")
                        .HasColumnType("bit");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OldPk")
                        .HasColumnType("int");

                    b.Property<string>("OrderStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PoNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PromoCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Shipping")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("ShippingAddressJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SoldBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("SubTotal")
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Tax")
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("TaxRate")
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Weight")
                        .HasColumnType("decimal(18,4)");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("Westwind.Webstore.Business.Entities.LineItem", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("AutoRegister")
                        .HasColumnType("bit");

                    b.Property<string>("CustomerId")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("DiscountPercent")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("ExtraData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InvoiceId")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("IsStockItem")
                        .HasColumnType("bit");

                    b.Property<string>("ItemImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LicenseSerial")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("Sku")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("SubscriptionAutoRenewal")
                        .HasColumnType("bit");

                    b.Property<DateTime>("SubscriptionExpires")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("UseLicensing")
                        .HasColumnType("bit");

                    b.Property<decimal>("Weight")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("InvoiceId");

                    b.ToTable("LineItems");
                });

            modelBuilder.Entity("Westwind.Webstore.Business.Entities.Lookup", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("CData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CData1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("NData")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Lookups");
                });

            modelBuilder.Entity("Westwind.Webstore.Business.Entities.Product", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Abstract")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("AutoRegister")
                        .HasColumnType("bit");

                    b.Property<string>("Categories")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Commission")
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("CommissionBasePrice")
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Cost")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailTo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Expected")
                        .HasColumnType("datetime2");

                    b.Property<bool>("InActive")
                        .HasColumnType("bit");

                    b.Property<string>("InfoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsFractional")
                        .HasColumnType("bit");

                    b.Property<bool>("IsStockItem")
                        .HasColumnType("bit");

                    b.Property<string>("ItemImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LicenseCount")
                        .HasColumnType("int");

                    b.Property<decimal>("ListPrice")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("LongDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Manufacturer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OldPk")
                        .HasColumnType("int");

                    b.Property<decimal>("OnOrder")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("ParentSku")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,4)");

                    b.Property<DateTime?>("ProductDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RedirectUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegistrationItemConfirmation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegistrationPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sku")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("SortOrder")
                        .HasColumnType("int");

                    b.Property<string>("SpecialsHeader")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SpecialsOrder")
                        .HasColumnType("int");

                    b.Property<decimal>("SpecialsPrice")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("SpecialsText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Stock")
                        .HasColumnType("decimal(18,4)");

                    b.Property<int>("SubscriptionRenewalDiscountPercent")
                        .HasColumnType("int");

                    b.Property<int>("SubscriptionRenewalMonths")
                        .HasColumnType("int");

                    b.Property<string>("SubscriptionRenewalRequestText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("UseLicensing")
                        .HasColumnType("bit");

                    b.Property<string>("VendorEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Version")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Weight")
                        .HasColumnType("decimal(18,4)");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Westwind.Webstore.Business.Entities.Address", b =>
                {
                    b.HasOne("Westwind.Webstore.Business.Entities.Customer", null)
                        .WithMany("Addresses")
                        .HasForeignKey("CustomerId");
                });

            modelBuilder.Entity("Westwind.Webstore.Business.Entities.Invoice", b =>
                {
                    b.HasOne("Westwind.Webstore.Business.Entities.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId");

                    b.OwnsOne("Westwind.Webstore.Business.Entities.InvoiceCreditCardData", "CreditCard", b1 =>
                        {
                            b1.Property<string>("InvoiceId")
                                .HasColumnType("nvarchar(20)");

                            b1.Property<string>("CardNumber")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Descriptor")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Expiration")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("IpAddress")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("LastFour")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Nonce")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("SecurityCode")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Type")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("InvoiceId");

                            b1.ToTable("Invoices");

                            b1.WithOwner()
                                .HasForeignKey("InvoiceId");
                        });

                    b.OwnsOne("Westwind.Webstore.Business.Entities.InvoiceCreditCardResultData", "CreditCardResult", b1 =>
                        {
                            b1.Property<string>("InvoiceId")
                                .HasColumnType("nvarchar(20)");

                            b1.Property<string>("AuthCode")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("AvsCode")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("ProcessingError")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("ProcessingResult")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("RawProcessingResult")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("TransactionId")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("InvoiceId");

                            b1.ToTable("Invoices");

                            b1.WithOwner()
                                .HasForeignKey("InvoiceId");
                        });

                    b.Navigation("CreditCard")
                        .IsRequired();

                    b.Navigation("CreditCardResult")
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Westwind.Webstore.Business.Entities.LineItem", b =>
                {
                    b.HasOne("Westwind.Webstore.Business.Entities.Invoice", null)
                        .WithMany("LineItems")
                        .HasForeignKey("InvoiceId");
                });

            modelBuilder.Entity("Westwind.Webstore.Business.Entities.Customer", b =>
                {
                    b.Navigation("Addresses");
                });

            modelBuilder.Entity("Westwind.Webstore.Business.Entities.Invoice", b =>
                {
                    b.Navigation("LineItems");
                });
#pragma warning restore 612, 618
        }
    }
}
