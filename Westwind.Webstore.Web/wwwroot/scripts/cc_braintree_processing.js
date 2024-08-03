window.brainTree = {};

brainTree = {
    canSubmit: false,
    brainTreeInstance: null,
    initialize: function () {

        // render the drop-in UI
        brainTree.processBraintreeHosted();

        // trigger validation on submit
        $("#btnSubmit").click(function () {
            var nonce = $("#Nonce").val();
            if (!nonce) {
                brainTree.requestPayment(true);
                return false;
            }
            
            return true;
        });
    },
    recalculate: function () {
        $("#btnRecalculate").click();
    },
    processBraintreeHosted: function() {
        var amount = ccConfig.amount;
        var clientToken = ccConfig.clientToken;
        
        braintree.dropin.create(
            {
                authorization: clientToken,
                container: '#dropin-container',
                dataCollector: {
                    kount: true
                },
                card: {
                    cardholderName: true,
                    //amount: amount,
                    currency: "USD"
                },
                paypal: {
                    flow: "checkout",
                    amount: amount,
                    currency: "USD",
                    buttonStyle: {
                        color: 'blue',
                        shape: 'rect',
                        size: 'medium'
                    }
                }
            },
            function (createErr, instance) {
                brainTree.brainTreeInstance = instance;
                $("#btnPreValidate")
                    .on("click", brainTree.requestPayment);
            });
    },
    requestPayment: function(forceSubmit = false) {
        brainTree.brainTreeInstance.requestPaymentMethod(function (err, payload) {
            if (err) {
                toastr.error(err.message);
                return;
            }
            
            // Submit payload.nonce to your server
            document.getElementById("Nonce").value = payload.nonce;
            $("#Nonce").val(payload.nonce);
            $("#Descriptor").val(payload.type);

            // to get this enable 'Advanced Fraud Protection' in the Admin console
            $("#DeviceData").val(payload.deviceData);

            if (forceSubmit === true) {
                if(window.progressWindowShowProgress)
                    progressWindowShowProgress();
                
                $("#OrderForm").submit();
            }
        });
    }
}

$(function () {
    brainTree.initialize();
});
