---
title: "Pass Braintree device data into transaction requests"
kind: gap
status: pending
created: 2026-10-13
summary: "The checkout collects Braintree device data, but the controller and processor never pass it to Braintree, so configured fraud collection has no effect."
---

## Why
`CreditCardPayment_BrainTree_Partial.cshtml` posts `InvoiceModel.DeviceData`, but `ShoppingCartController.ProcessCreditCard` copies only the nonce and descriptor. `BraintreeCreditCardProcessor.Process` also never assigns `TransactionRequest.DeviceData`.

## What to do
Carry device data through the invoice payment model/entity into the Braintree `TransactionRequest`, and add focused coverage for the mapping.

## Where
- `Westwind.Webstore.Web/Views/ShoppingCart/ShoppingCartController.cs`
- `Westwind.Webstore.Business/InvoiceBusiness.cs`
- `Westwind.CreditCardProcessing/Westwind.CreditCardProcessing/BraintreeCreditCardProcessor.cs`
