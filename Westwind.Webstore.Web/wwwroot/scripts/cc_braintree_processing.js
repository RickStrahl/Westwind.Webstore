var brainTree = window.brainTree = {
    hostedFieldsInstance: null,
    paypalCheckoutInstance: null,
    selectedPaymentMethod: "card",
    isSubmitting: false,
    allowFormSubmit: false,

    initialize: function () {
        brainTree.clearPaymentMethod();
        brainTree.bindPaymentMethodSelector();

        $("#OrderForm").on("submit.braintree", function (event) {
            return brainTree.handleSubmit(event);
        });

        $("#btnSubmit").prop("disabled", true);

        braintree.client.create({
            authorization: ccConfig.clientToken
        }, function (clientErr, clientInstance) {
            if (clientErr) {
                brainTree.showError("Payment form initialization failed. Please reload the page and try again.", clientErr);
                return;
            }

            brainTree.initializeHostedFields(clientInstance);
            brainTree.initializeDataCollector(clientInstance);
            brainTree.initializePayPal(clientInstance);
        });
    },

    bindPaymentMethodSelector: function () {
        $("#braintree-card-option").on("click", function () {
            brainTree.selectPaymentMethod("card");
        });

        $("#braintree-paypal-option").on("click", function () {
            brainTree.selectPaymentMethod("paypal");
        });
    },

    selectPaymentMethod: function (paymentMethod, preservePaymentMethod) {
        var paymentMethodChanged = brainTree.selectedPaymentMethod !== paymentMethod;
        brainTree.selectedPaymentMethod = paymentMethod;

        var usePayPal = paymentMethod === "paypal";
        $("#braintree-card-panel").toggleClass("d-none", usePayPal);
        $("#braintree-paypal-panel").toggleClass("d-none", !usePayPal);
        $("#braintree-card-option")
            .toggleClass("active", !usePayPal)
            .attr("aria-pressed", (!usePayPal).toString());
        $("#braintree-paypal-option")
            .toggleClass("active", usePayPal)
            .attr("aria-pressed", usePayPal.toString());

        if (!preservePaymentMethod && paymentMethodChanged) {
            brainTree.clearPaymentMethod();
            $("#paypal-payment-status").removeClass("text-success text-danger").text("");
        }
    },

    initializeHostedFields: function (clientInstance) {
        braintree.hostedFields.create({
            client: clientInstance,
            styles: {
                "input": {
                    "font-family": "-apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif",
                    "font-size": "16px",
                    "color": "#212529"
                },
                "input::placeholder": {
                    "color": "#6c757d"
                },
                "input.invalid": {
                    "color": "#dc3545"
                },
                "input.valid": {
                    "color": "#198754"
                }
            },
            fields: {
                number: {
                    container: "#braintree-card-number",
                    placeholder: "Card number"
                },
                cvv: {
                    container: "#braintree-cvv",
                    placeholder: "CVV"
                },
                expirationDate: {
                    container: "#braintree-expiration-date",
                    placeholder: "MM / YY"
                },
                postalCode: {
                    container: "#braintree-postal-code",
                    placeholder: "Postal code"
                },
                cardholderName: {
                    container: "#braintree-cardholder-name",
                    placeholder: "Name as shown on card"
                }
            }
        }, function (hostedFieldsErr, hostedFieldsInstance) {
            if (hostedFieldsErr) {
                brainTree.showError("Credit card fields could not be loaded. Please reload the page and try again.", hostedFieldsErr);
                return;
            }

            brainTree.hostedFieldsInstance = hostedFieldsInstance;
            $("#btnSubmit").prop("disabled", false);
        });
    },

    initializeDataCollector: function (clientInstance) {
        braintree.dataCollector.create({
            client: clientInstance,
            kount: true
        }, function (dataCollectorErr, dataCollectorInstance) {
            if (dataCollectorErr) {
                console.warn("Braintree fraud data collection could not be initialized.", dataCollectorErr);
                return;
            }

            $("#DeviceData").val(dataCollectorInstance.deviceData);
        });
    },

    initializePayPal: function (clientInstance) {
        braintree.paypalCheckout.create({
            client: clientInstance
        }, function (paypalCheckoutErr, paypalCheckoutInstance) {
            if (paypalCheckoutErr) {
                brainTree.disablePayPal("PayPal is currently unavailable.", paypalCheckoutErr);
                return;
            }

            brainTree.paypalCheckoutInstance = paypalCheckoutInstance;
            paypalCheckoutInstance.loadPayPalSDK({
                currency: ccConfig.currency
            }, function (loadPayPalSdkErr) {
                if (loadPayPalSdkErr) {
                    brainTree.disablePayPal("PayPal is currently unavailable.", loadPayPalSdkErr);
                    return;
                }

                paypal.Buttons({
                    fundingSource: paypal.FUNDING.PAYPAL,
                    style: {
                        color: "blue",
                        shape: "rect",
                        label: "paypal",
                        height: 40
                    },
                    createOrder: function () {
                        brainTree.clearPaymentMethod();
                        $("#paypal-payment-status").removeClass("text-success text-danger").text("");

                        return paypalCheckoutInstance.createPayment({
                            flow: "checkout",
                            amount: ccConfig.amount,
                            currency: ccConfig.currency,
                            userAction: "CONTINUE"
                        });
                    },
                    onApprove: function (data) {
                        return paypalCheckoutInstance.tokenizePayment(data).then(function (payload) {
                            brainTree.selectPaymentMethod("paypal", true);
                            brainTree.setPaymentMethod(payload);
                            $("#paypal-payment-status")
                                .removeClass("text-danger")
                                .addClass("text-success")                                
                                .text("PayPal authorization complete. Place your order to finish payment.");                            
                        }).catch(function (tokenizeErr) {
                            brainTree.showError("PayPal authorization could not be completed. Please try again.", tokenizeErr);
                        });
                    },
                    onCancel: function () {
                        brainTree.clearPaymentMethod();
                        $("#paypal-payment-status")
                            .removeClass("text-success")
                            .addClass("text-danger")
                            .text("PayPal authorization was cancelled.");
                    },
                    onError: function (paypalErr) {
                        brainTree.clearPaymentMethod();
                        brainTree.showError("PayPal authorization could not be completed. Please try again.", paypalErr);
                    }
                }).render("#paypal-button")
                    .then(function () {
                        $("#btnSubmit").prop("disabled", false);
                    })
                    .catch(function (renderErr) {
                        brainTree.disablePayPal("PayPal is currently unavailable.", renderErr);
                    });
            });
        });
    },

    handleSubmit: function (event) {
        if (brainTree.allowFormSubmit) {
            return true;
        }

        event.preventDefault();

        if (brainTree.isSubmitting) {
            return false;
        }

        if (brainTree.selectedPaymentMethod === "paypal") {
            if ($("#Nonce").val() && $("#Descriptor").val() === "PayPalAccount") {
                brainTree.submitOrder();
            } else {
                toastr.error("Please authorize the payment with PayPal before placing your order.");
            }
            return false;
        }

        if (!brainTree.hostedFieldsInstance) {
            toastr.error("The credit card form is still loading. Please try again in a moment.");
            return false;
        }

        brainTree.setSubmitting(true);
        brainTree.hostedFieldsInstance.tokenize(function (tokenizeErr, payload) {
            if (tokenizeErr) {
                brainTree.setSubmitting(false);
                brainTree.handleHostedFieldsError(tokenizeErr);
                return;
            }

            brainTree.setPaymentMethod(payload);
            brainTree.submitOrder();
        });

        return false;
    },

    handleHostedFieldsError: function (tokenizeErr) {
        if (tokenizeErr.code === "HOSTED_FIELDS_FIELDS_EMPTY") {
            toastr.error("Please enter your credit card information.");
            return;
        }

        if (tokenizeErr.code === "HOSTED_FIELDS_FIELDS_INVALID") {
            toastr.error("Please correct the highlighted credit card fields.");
            return;
        }

        toastr.error(tokenizeErr.message || "The credit card could not be validated. Please check the card information and try again.");
        console.error("Braintree Hosted Fields tokenization failed.", tokenizeErr);
    },

    setPaymentMethod: function (payload) {
        $("#Nonce").val(payload.nonce);
        $("#Descriptor").val(payload.type);
    },

    clearPaymentMethod: function () {
        $("#Nonce").val("");
        $("#Descriptor").val("");
    },

    setSubmitting: function (isSubmitting) {
        brainTree.isSubmitting = isSubmitting;
        $("#btnSubmit").prop("disabled", isSubmitting);
    },

    submitOrder: function () {
        brainTree.setSubmitting(true);
        brainTree.allowFormSubmit = true;

        if (window.progressWindowShowProgress) {
            progressWindowShowProgress();
        }

        $("#OrderForm").trigger("submit");
    },

    disablePayPal: function (message, error) {
        $("#braintree-paypal-option").prop("disabled", true);
        $("#paypal-payment-status").addClass("text-danger").text(message);
        console.error(message, error);
    },

    showError: function (message, error) {
        toastr.error(message);
        console.error(message, error);
    }
};

$(function () {
    brainTree.initialize();
});
