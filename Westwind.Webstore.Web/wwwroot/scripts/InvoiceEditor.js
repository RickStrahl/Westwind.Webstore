page = {};

// globals{} is passed in from main page
page = {
    invoice: null,
    initialize: function() {
        page.loadInvoice();
    },
    loadInvoice: function(invNo) {
        return ajaxJson("/admin/ordermanager/api/invoice/" + globals.invoiceNumber,
            function(inv) {
                page.invoice = invoice;
                
                debugger;
            },
            function(err) {
                toastr.error("failed to load api invoice: " + err.message);
            }
        );
    }
}

$( function() {
    page.initialize();
})